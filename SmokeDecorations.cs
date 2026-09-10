using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckDecorationUi()
    {
        var previous=_world; var size=GetWindow().Size;
        async Task Frames() { for(int i=0;i<4;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
        try
        {
            AdoptWorld(new World()); _paused=true;
            int i=0;
            foreach(var kind in Enum.GetValues<DecorationKind>().Where(k=>k!=DecorationKind.Sunflowers))
            {
                _decorationChoice.Select((int)kind); await UiClick(_decorateButton); await Frames();
                if(!_decorating || _ghostModelKey!="decoration:"+kind) throw new Exception("Decoration palette failed");
                var cell=new Cell(2+i,-2); _rotated=true; PlaceCottage(cell); i++;
                if(!_world.Decorations.Any(d=>d.Cell==cell && d.Rotated)) throw new Exception("Decoration click/rotation failed");
            }
            await Frames();
            if(_decorationView.GetChildCount()!=5) throw new Exception("Decoration models missing");
            CloseDrawer(); _placing=false; RefreshGhost(); _focus=new(4,0,-2); _camera.Size=12; UpdateCamera();
            await Frames(); await Capture("artifacts/f09-palette.png");
            string saved=_world.SaveJson(); AdoptWorld(World.LoadJson(saved)); _paused=true; await Frames();
            if(_decorationView.GetChildCount()!=5 || saved!=_world.SaveJson()) throw new Exception("Decoration load/pause failed");
            GetWindow().Size=new(960,640); await UiClick(_eraseDecorationButton); await Frames();
            await Capture("artifacts/f09-controls-960.png");
            PlaceCottage(new(2,-2)); await Frames();
            if(_world.Decorations.Count!=4 || _decorationView.GetChildCount()!=4) throw new Exception("Decoration removal failed");
            BeginPlacement(BuildingKind.Cottage); if(_decorating) throw new Exception("Building mode kept decorations");
            BeginDecorating(false); TogglePaths(1); if(_decorating) throw new Exception("Path mode kept decorations");
            BeginDecorating(false); ToggleTreePlanting(); if(_decorating) throw new Exception("Planting mode kept decorations");
            BeginDecorating(false); ToggleClearing(); if(_decorating) throw new Exception("Clearing mode kept decorations");
            GD.Print("PASS: decoration palette/clicks, rotation, rendered objects, removal, save/load, 960px controls and tool switching.");
        }
        finally { GetWindow().Size=size; AdoptWorld(previous); _paused=true; }
    }
}
