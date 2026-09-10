using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;

public partial class Game
{
    private async void RunSupplyRouteSmoke()
    {
        try
        {
            async System.Threading.Tasks.Task Frames(int n=25) { for(int i=0;i<n;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
            var w=World.NewCampaign(2); w.Place(new(3,-3),false,BuildingKind.Sawmill); w.Place(new(6,-3),false,BuildingKind.Stockpile);
            w.Assign(6,Role.Sawyer); w.Assign(7,Role.Hauler);
            for(int i=0;i<700;i++) w.Tick(.1f);
            AdoptWorld(w); _paused=true; await Frames(); OpenEconomy();
            _supplyToggle.EmitSignal(BaseButton.SignalName.Pressed); await Frames();
            foreach(int width in new[]{1440,960})
            {
                GetWindow().Size=new(width,width==960?640:900); FrameMap(); await Frames();
                _drawerPages[4].ScrollVertical=(int)_supplyToggle.Position.Y; await Frames();
                if(_supplyMesh?.Visible!=true || _supplyMesh.Mesh.GetSurfaceCount()!=1 || !_supplySummary.Text.Contains("Gold")) throw new Exception("Supply overlay or legend missing");
                await Capture($"artifacts/f07b1-routes-{width}.png");
            }
            string saved=w.SaveJson(); var vertexCount=_supplyMesh!.Mesh.SurfaceGetArrays(0)[(int)Godot.Mesh.ArrayType.Vertex].AsVector3Array().Length;
            await Frames(60);
            if(w.SaveJson()!=saved || _supplyMesh.Mesh.SurfaceGetArrays(0)[(int)Godot.Mesh.ArrayType.Vertex].AsVector3Array().Length!=vertexCount) throw new Exception("Paused route changed");
            var first=_supplyLinks.GetChildren().OfType<Button>().First(b=>b.Visible); first.EmitSignal(BaseButton.SignalName.Pressed); await Frames();
            if(_selectedPerson!=(int)first.GetMeta("worker")) throw new Exception("Route worker link failed");
            ToggleWatch(); await Frames(); if(_supplyMesh.Visible) throw new Exception("Routes visible in Watch mode");
            ToggleWatch(); await Frames(); if(!_supplyMesh.Visible) throw new Exception("Routes did not return after Watch");
            AdoptWorld(World.NewCreative()); await Frames();
            if(_showSupplyRoutes || _supplyMesh.Visible) throw new Exception("World switch retained old route overlay");
            GD.Print("PASS: supply overlay, current-trip links, 960/1440 layout, paused stability, Watch hiding and world-switch reset."); GetTree().Quit();
        }
        catch(Exception e) { GD.PrintErr(e); GetTree().Quit(1); }
    }
}
