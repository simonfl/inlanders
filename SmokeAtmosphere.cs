using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckAtmosphereUi()
    {
        void Check(bool ok,string message) { if(!ok) throw new Exception(message); }
        async Task Frames() { for(int i=0;i<4;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
        var previous=_world; var size=GetWindow().Size;
        bool oldGolden=_goldenHour, oldMotion=_foliageMotion; string oldPath=_atmospherePath;
        _atmospherePath=Path.Combine(Path.GetTempPath(),"inlanders-atmosphere-"+Guid.NewGuid()+".cfg");
        try
        {
            AdoptWorld(World.NewCampaign(2)); _paused=true;
            _world.Place(new(3,-3),false,BuildingKind.Farm); _world.Place(new(6,-3),false,BuildingKind.Bakery);
            for(int i=0;i<6000 && _world.Cottages.Any(c=>!c.Complete);i++) _world.Tick(.1f);
            Check(_world.Cottages.All(c=>c.Complete),"Atmosphere fixture did not build");
            await Frames(); _focus=new(2,0,0); _camera.Size=23; UpdateCamera();
            _goldenHour=false; _foliageMotion=true; ApplyAtmosphere();
            string saved=_world.SaveJson();
            ToggleWatch(); await Frames(); await Capture("artifacts/f20-daylight.png"); ExitWatch();
            await OpenMenu(3); await UiClick(_lightMoodButton); await Frames();
            Check(_goldenHour && _sun.RotationDegrees.X == -36,"Golden-hour control did not update light");
            Check(_world.SaveJson()==saved,"Lighting changed settlement state");
            ToggleWatch(); await Frames(); await Capture("artifacts/f20-golden-hour.png"); ExitWatch();
            var crown=GetTree().GetNodesInGroup("foliage").Cast<Node3D>().First(n=>n.IsVisibleInTree() && !_ghostModel.IsAncestorOf(n));
            var pose=crown.Rotation; await Frames(); Check(crown.Rotation==pose,"Paused foliage moved");
            _world.Tick(2); UpdateAtmosphere(); Check(crown.Rotation!=pose,"Foliage did not move with village time");
            await OpenMenu(3); await UiClick(_foliageButton); await Frames();
            Check(!_foliageMotion && crown.Rotation==Vector3.Zero,"Motion toggle did not still foliage");
            _goldenHour=false; _foliageMotion=true; LoadAtmosphere(); ApplyAtmosphere();
            Check(_goldenHour && !_foliageMotion,"Atmosphere preferences did not persist");
            _foliageMotion=true; ApplyAtmosphere(); ToggleTreePlanting(); await Frames();
            Check(GetTree().GetNodesInGroup("foliage").Cast<Node3D>().Where(n=>_ghostModel.IsAncestorOf(n)).All(n=>n.Rotation==Vector3.Zero),"Placement preview swayed");
            _placing=false; RefreshGhost(); GetWindow().Size=new(960,640);
            await OpenMenu(3); _drawerPages[3].EnsureControlVisible(_foliageButton); await Frames();
            await Capture("artifacts/f20-controls-960.png");
            Check(_foliageButton.GetGlobalRect().End.Y<=_drawer.GetGlobalRect().End.Y,"Atmosphere controls overflow");
            GD.Print("SMOKE PASS: lighting presets, pause-aware foliage, motion toggle, static previews, local preferences, unchanged settlement and 960px controls.");
        }
        finally
        {
            if(File.Exists(_atmospherePath)) File.Delete(_atmospherePath);
            _atmospherePath=oldPath; _goldenHour=oldGolden; _foliageMotion=oldMotion;
            AdoptWorld(previous); ApplyAtmosphere(); GetWindow().Size=size;
        }
    }
}
