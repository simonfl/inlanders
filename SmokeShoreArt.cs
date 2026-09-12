using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckShoreArt()
    {
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        void Until(World w,Func<bool> done,string why){for(int i=0;i<12000 && !done();i++)w.Tick(.1f);w.Validate();Check(done(),why);}
        string prefix=OS.GetCmdlineUserArgs().Contains("--shore-before")?"shore-before":"shore-art";
        for(int r=0;r<4;r++)
        {
            var w=World.NewLakeMap();var cell=w.Map.Land.First(c=>w.PlacementProblem(c,r,BuildingKind.FishingDock)==null);
            var dock=w.Place(cell,r,BuildingKind.FishingDock)!;
            Until(w,()=>dock.Complete,"Dock construction stalled");
            AdoptWorld(w);_paused=true;CloseManagementUi();_noticeUntil=0;_angle=.72f;
            _focus=OnGround(cell.X,cell.Z);_camera.Size=7;UpdateCamera();
            foreach(int width in new[]{960,1440})
            {
                GetWindow().Size=new(width,width==960?640:900);await Frames();await Capture($"artifacts/{prefix}-dock-{r}-{width}.png");
            }
            w.Assign(0,Role.Fisher);Until(w,()=>dock.Boat?.Phase==BoatPhase.Fishing,"Rotated fishing route stalled");
            string saved=w.SaveJson();AdoptWorld(World.LoadJson(saved));await Frames();Check(_world.SaveJson()==saved,"Boat reload differs");
            w=_world;dock=w.Cottages.Single(c=>c.Kind==BuildingKind.FishingDock);
            var pose=_boatViews[dock.Id].Left.Transform;await Frames();Check(pose==_boatViews[dock.Id].Left.Transform,"Paused oar moved");
            Until(w,()=>dock.Boat?.Phase==BoatPhase.Returning,"Catch return stalled");
            Check(dock.Boat!.Fish>0,"Returning boat lacks catch");
            Until(w,()=>w.DeliveredFish>0,"Catch not delivered");await Frames();
            await Capture($"artifacts/{prefix}-landing-{r}.png");
            Check(w.SetWorkplacePaused(dock.Id,true),"Dock pause rejected");
            Until(w,()=>dock.Boat?.FisherId==null,"Dock recall failed");
            Check(w.RequestDemolition(dock.Id),"Idle dock demolition rejected");
            Until(w,()=>!w.Cottages.Contains(dock),"Dock material recovery stalled");
            var crossing=new World();var at=new Cell(3,-2);crossing.Map.Water.Add(at);
            var bridge=crossing.Place(at,r,BuildingKind.Bridge)!;Check(bridge!=null,"Bridge rotation rejected");
            Until(crossing,()=>bridge!.Complete,"Bridge construction stalled");AdoptWorld(crossing);CloseManagementUi();_noticeUntil=0;
            _focus=OnGround(at.X,at.Z);_camera.Size=6;UpdateCamera();await Frames();await Capture($"artifacts/{prefix}-bridge-{r}.png");
            saved=crossing.SaveJson();Check(World.LoadJson(saved).SaveJson()==saved,"Bridge save differs");
        }
        var river=World.NewLargeMap();
        for(int z=river.Map.MinZ;z<=river.Map.MaxZ;z++)if(river.Map.Contains(new(7,z)))river.Map.Water.Add(new(7,z));
        river=World.LoadJson(river.SaveJson());
        var span=river.Place(new(7,3),1,BuildingKind.Bridge)??throw new Exception("Divided river span rejected");
        Until(river,()=>span.Complete,"Divided river construction failed");
        var house=river.Place(new(9,5))??throw new Exception("Bridge did not unlock far-bank construction");
        var crossed=new System.Collections.Generic.HashSet<int>();
        Until(river,()=>
        {
            foreach(var p in river.People.Where(p=>World.At(p)==span.Cell))crossed.Add(p.Id);
            return crossed.Count>=2 && river.People.Any(p=>World.At(p)==span.Cell);
        },"Multiple workers did not use the span");
        AdoptWorld(river);CloseManagementUi();_noticeUntil=0;_focus=OnGround(7,3);_camera.Size=10;UpdateCamera();await Frames();
        await Capture($"artifacts/{prefix}-busy-crossing.png");
        Check(!river.RequestDemolition(span.Id),"Demolition stranded far-bank construction/workers");
        Until(river,()=>house.Complete,"Far-bank deliveries stalled");
        AdoptWorld(World.NewCampaign(9));await Frames();
        _dynamic.Hide();_landscape.Hide();_fishGroundView?.Hide();if(_pathView!=null)_pathView.Hide();_hud.Hide();_watchRoot.Hide();_showWorldLabels=false;ApplyWorldLabels();
        var sheet=new Node3D();AddChild(sheet);
        foreach(var kind in new[]{BuildingKind.FishingDock,BuildingKind.Bridge})for(int stage=0;stage<4;stage++)
        {
            var model=new Node3D{Position=new(stage*2.7f-4,0,kind==BuildingKind.Bridge?3:-1)};sheet.AddChild(model);MakeBuilding(model,new Cottage{Kind=kind},stage);
        }
        Box(sheet,new(0,-.45f,0),new(13,.15f,9),new("719da2"));_focus=new(0,0,0);_camera.Size=17;UpdateCamera();await Frames();await Capture($"artifacts/{prefix}-stages.png");
        GD.Print("PASS: dock/bridge four-orientation construction, fishing/return/delivery, recall/demolition, exact saves, paused oars and shore art captures.");
    }
}
