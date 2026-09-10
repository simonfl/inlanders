using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckGrowthUi()
    {
        void Check(bool ok,string message) { if(!ok) throw new Exception(message); }
        async Task Frames() { for(int i=0;i<4;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
        var previous=_world;
        try
        {
            AdoptWorld(World.NewScenario()); _paused=true;
            Check(_world.Place(new(0,0),false,BuildingKind.ForagerHut)!=null,"Forager fixture rejected");
            var farm=_world.Place(new(3,-3),true,BuildingKind.Farm) ?? throw new Exception("Farm fixture rejected");
            int farmId=farm.Id;
            var seen=new System.Collections.Generic.HashSet<int>();
            var construction=new System.Collections.Generic.HashSet<string>();
            for(int i=0;i<6000;i++)
            {
                _world.Tick(.1f);
                foreach(var site in _world.Cottages)
                {
                    int stage=site.Complete?3:site.Construction>.4f?2:site.Delivered>0?1:0;
                    string name=$"{site.Kind}-{stage}";
                    if(construction.Add(name))
                    {
                        await Frames(); _focus=new(2,0,-2); _camera.Size=12; UpdateCamera();
                        await Capture($"artifacts/f22-build-{name}.png");
                    }
                }
                if(!farm.Complete) continue;
                int key=farm.Harvest>0?40+farm.Harvest:farm.Planted?10*(1+(int)(farm.Growth*2.9f)):0;
                if(seen.Add(key))
                {
                    await Frames();
                    var view=_cropViews[farmId];
                    Check(view.Stage==key,"Crop view did not refresh");
                    Check(view.Body.RotationDegrees.Y==90,"Rotated field lost crop alignment");
                    if(farm.Harvest>0)
                        Check(view.Body.GetChildren().Cast<Node3D>().Count(n=>n.GetMeta("standing").AsBool())==farm.Harvest*3,"Harvest did not clear the correct rows");
                    _focus=new(2,0,-2); _camera.Size=12; UpdateCamera();
                    await Capture($"artifacts/f22-crop-{key}.png");
                    if(key==42)
                    {
                        string saved=_world.SaveJson();
                        await Frames(); Check(_world.SaveJson()==saved,"Paused crops changed the world");
                        AdoptWorld(World.LoadJson(saved)); _paused=true;
                        farm=_world.Cottages.Single(c=>c.Id==farmId); await Frames();
                        Check(_cropViews[farmId].Stage==42,"Partial harvest not restored");
                        Check(_cropViews[farmId].Body.GetChildren().Cast<Node3D>().Count(n=>n.GetMeta("standing").AsBool())==6,"Loaded crop rows incorrect");
                    }
                }
                if(seen.Contains(42) && farm.Harvest==0) break;
            }
            Check(construction.Count==8,"Missing farm or forager construction stages");
            Check(new[]{10,20,30,46,42}.All(seen.Contains),"Missing growth or partial-harvest stages");
            await Frames(); Check(_cropViews[farmId].Body.GetChildCount()==0,"Harvested field retained crops");
            GD.Print("SMOKE PASS: sprouting, growing, ripe and progressively harvested crops, rotated field, paused state and partial-harvest save/load.");
        }
        finally { AdoptWorld(previous); }
    }
}
