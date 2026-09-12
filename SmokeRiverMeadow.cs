using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckRiverMeadow()
    {
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        int comparisons=0;
        _goldenHour=false;_showWorldLabels=false;ApplyAtmosphere();
        foreach(string name in new[]{"before-initial","after-initial","before-settled","after-settled"})
        {
            if(name.StartsWith("before") && !File.Exists($"artifacts/landscape-{name}.json"))continue;
            if(name.StartsWith("before"))comparisons++;
            var w=name=="after-initial"?World.NewCampaign(10):World.LoadFile(name=="after-settled"?"artifacts/finale-campaign-local-normal.json":$"artifacts/landscape-{name}.json");
            AdoptWorld(w);_paused=true;CloseManagementUi();_noticeUntil=0;ApplyWorldLabels();
            if(name.StartsWith("after"))Check(w.Map.RiverMeadow && w.InitialLogs==168,"Missing meadow theme or changed material budget");
            foreach(int width in new[]{960,1440})
            {
                GetWindow().Size=new(width,width==960?640:900);_angle=.72f;FrameMap();await Frames();
                string saved=w.SaveJson();await Frames();Check(w.SaveJson()==saved,"Landscape changed paused simulation");
                await Capture($"artifacts/landscape-{name}-{width}.png");
            }
            _focus=new(4,0,3);_camera.Size=24;UpdateCamera();await Frames();await Capture($"artifacts/landscape-{name}-center.png");
            if(!name.StartsWith("after"))continue;
            var water=_landscape.GetNode<Node3D>("RiverMeadowWater");
            int CountMeshes(Node node)=> (node is MeshInstance3D?1:0)+node.GetChildren().Sum(CountMeshes);
            int meshes=CountMeshes(water);Check(meshes<=8,"Shore detail exceeds bounded material batches");
            string original=w.SaveJson();AdoptWorld(World.LoadJson(original));_paused=true;await Frames();
            Check(_world.SaveJson()==original && _world.Map.RiverMeadow,"Meadow theme/save roundtrip failed");
            if(name=="after-initial")
            {
                var site=new Cell(6,3);Check(_world.PlacementProblem(site,1,BuildingKind.Bridge)==null,"Crossing preview rejected");
                BeginPlacement(BuildingKind.Bridge);_rotation=1;
                var pointer=_camera.UnprojectPosition(new(site.X,0,site.Z));
                Input.ParseInputEvent(new InputEventMouseMotion{Position=pointer,GlobalPosition=pointer});await Frames();
                Check(_ghostValid && _hover==site,"Meadow crossing preview disagrees with placement");
                await Capture("artifacts/landscape-bridge-preview.png");await Press(Key.Escape);
            }
            GD.Print($"River meadow {name}: {w.Map.Land.Count()} land, {w.Map.Water.Count} water, {meshes} shore meshes, {w.InitialLogs} total logs.");
        }
        GD.Print($"PASS: meadow opening/settled captures at 960/1440, {comparisons} available before-state comparisons, central views, bounded shore batches, bridge preview and exact paused/theme reloads.");
    }
}
