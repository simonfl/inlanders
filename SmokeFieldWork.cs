using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;

public partial class Game
{
    private async void RunFieldWorkSmoke()
    {
        try
        {
            async System.Threading.Tasks.Task Frames() { for(int i=0;i<4;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
            void CheckStandingCrop(Cottage field,Vector3 tip)
            {
                var plants=_cropViews[field.Id].Body.GetChildren().OfType<Node3D>().Where(n=>n.HasMeta("standing") && n.GetMeta("standing").AsBool()).ToArray();
                int expected=field.Harvest*(field.Kind==BuildingKind.Farm?3:1);
                if(plants.Length!=expected || !plants.Any(n=>new Vector2(n.GlobalPosition.X-tip.X,n.GlobalPosition.Z-tip.Z).Length()<.2f))
                    throw new Exception("Contact is not at a rendered remaining crop, or harvest mask is wrong");
            }
            foreach(int rotated in new[]{0,1,2,3}) foreach(var kind in new[]{BuildingKind.Farm,BuildingKind.VegetableGarden})
            {
                var w=World.NewCreative(); foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
                var field=w.Place(new(3,0),rotated,kind) ?? throw new Exception("Field placement failed");
                w.Assign(0,Role.Farmer); AdoptWorld(w); _paused=true; CloseDrawer();
                var worker=w.People[0];
                foreach(var task in new[]{Work.Planting,Work.Harvesting})
                {
                    float middle=task==Work.Planting?2:1;
                    for(int i=0;i<5000 && !(worker.Task==task && worker.Timer>=middle);i++) w.Tick(.05f);
                    if(worker.Task!=task || worker.Timer>=middle+.1f) throw new Exception("Field work fixture stalled");
                    await Frames(); var view=_people[0];
                    bool sickle=task==Work.Harvesting && kind==BuildingKind.Farm;
                    if(view.Sickle.Visible!=sickle || view.SeedPouch.Visible!=(task==Work.Planting) || view.Spade.Visible || view.Carry.Visible) throw new Exception("Wrong field tool or phantom crop cargo");
                    var tip=sickle?view.Sickle.ToGlobal(new(.16f,0,-.48f)):view.Arm.ToGlobal(new(0,-.36f,0));
                    float distance=tip.DistanceTo(FieldWorkTarget(field,task==Work.Planting));
                    GD.Print($"FIELD CONTACT {kind} rotated={rotated} {task}: {distance:0.000}");
                    if(distance>.08f) throw new Exception("Field work misses soil/crop");
                    if(task==Work.Harvesting) CheckStandingCrop(field,tip);
                    var pose=view.Arm.GlobalTransform; string saved=w.SaveJson(); await Frames();
                    if(pose!=view.Arm.GlobalTransform || saved!=w.SaveJson()) throw new Exception("Paused field changed");
                    _focus=OnGround(field.Cell.X,field.Cell.Z); _camera.Size=10;
                    foreach(int width in new[]{960,1440})
                    {
                        GetWindow().Size=new(width,width==960?640:900); UpdateCamera(); await Frames();
                        await Capture($"artifacts/f03b3-{kind}-{task}-{rotated}-{width}.png");
                    }
                    GetWindow().Size=new(960,640); _camera.Size=20; UpdateCamera(); await Frames(); await Capture($"artifacts/f03b3-village-{kind}-{task}-{rotated}.png");
                    AdoptWorld(World.LoadJson(saved)); _paused=true; await Frames();
                    if(pose!=_people[0].Arm.GlobalTransform) throw new Exception("Reload changed field pose");
                    w=_world; worker=w.People[0]; field=w.Cottages.Single(c=>c.Id==field.Id);
                    for(int i=0;i<4;i++) w.Tick(.05f);
                    await Frames();
                    if(pose==_people[0].Arm.GlobalTransform) throw new Exception("Work progress did not advance the gesture");
                }
                int harvest=field.Harvest;
                for(int i=0;i<100 && worker.Carried==0;i++) w.Tick(.05f);
                await Frames();
                int expected=kind==BuildingKind.Farm?4:2;
                if(worker.Carried!=expected || field.Harvest!=harvest-expected || !_people[0].Carry.Visible || _people[0].Sickle.Visible || _people[0].SeedPouch.Visible) throw new Exception("Harvest transfer/tool cleanup failed");
                // The next load must work on a surviving plant, not empty cut rows.
                for(int i=0;i<5000 && !(worker.Task==Work.Harvesting && worker.Timer>=1);i++) w.Tick(.05f);
                if(worker.Task!=Work.Harvesting) throw new Exception("Partial harvest fixture stalled");
                await Frames();
                var nextTip=kind==BuildingKind.Farm?_people[0].Sickle.ToGlobal(new(.16f,0,-.48f)):_people[0].Arm.ToGlobal(new(0,-.36f,0));
                if(nextTip.DistanceTo(FieldWorkTarget(field,false))>.08f) throw new Exception("Partial harvest misses remaining crop");
                CheckStandingCrop(field,nextTip);
                w.Assign(0,Role.Unassigned); await Frames();
                if(_people[0].Sickle.Visible || _people[0].SeedPouch.Visible || _people[0].Rig.Position!=Vector3.Zero) throw new Exception("Interrupted field stance remained");
                w.Validate();
            }
            GD.Print("PASS: sowing/grain cutting/vegetable picking, rotated plots, contact, partial harvest, real cargo, pause/reload/interruption and camera captures."); GetTree().Quit();
        }
        catch(Exception e) { GD.PrintErr(e); GetTree().Quit(1); }
    }
}
