using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckCivicIdentities()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<4;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        _goldenHour=false;ApplyAtmosphere();_showWorldLabels=false;ApplyWorldLabels();
        for(int rotation=0;rotation<4;rotation++)
        {
            var w=World.NewQuarryMap();w.Place(new(3,-5),0,BuildingKind.Sawmill);w.Place(new(11,-4),0,BuildingKind.Quarry);
            var cell=w.Map.Land.Where(c=>w.PlacementProblem(c,rotation,BuildingKind.GatheringHall)==null).OrderBy(c=>(c.Point-new Cell(0,-3).Point).LengthSquared()).First();
            var hall=w.Place(cell,rotation,BuildingKind.GatheringHall)!;var chosen=rotation%2==0?CivicIdentity.Chapel:CivicIdentity.PlantedCourt;
            Check(w.SetCivicIdentity(hall.Id,chosen),"Plan identity rejected");w.Assign(6,Role.Quarrier);w.Assign(7,Role.Sawyer);
            AdoptWorld(w);_paused=true;CloseManagementUi();_noticeUntil=0;_focus=OnGround(cell.X,cell.Z);_camera.Size=13;_angle=.72f;UpdateCamera();
            int last=-1;
            for(int tick=0;tick<10000;tick++)
            {
                int stage=hall.Complete?3:hall.Construction>.4f?2:hall.Delivered+hall.DeliveredStone>0?1:0;
                if(stage!=last){last=stage;await Frames();Check((int)_cottages[hall.Id].Body.GetMeta("civic_identity")== (int)chosen,"Construction appearance differs");if(rotation<2)await Capture($"artifacts/civic-construction-{chosen}-{stage}.png");}
                if(hall.Complete)break;w.Tick(.1f);
            }
            Check(hall.Complete && hall.Delivered==8 && hall.DeliveredStone==12,"Civic construction stalled or changed cost");
            for(int i=0;i<6000 && !w.People.Any(p=>p.LeisureSiteId==hall.Id && p.Task==Work.Leisure);i++)w.Tick(.1f);
            Check(w.People.Any(p=>p.LeisureSiteId==hall.Id && p.Task==Work.Leisure),"No actual civic visitor");
            Check(w.People.Where(p=>p.LeisureSiteId==hall.Id).All(p=>!World.Footprint(cell,rotation,hall.Kind).Contains(p.Destination)),"Visitor enters blocked venue");
            foreach(var identity in Enum.GetValues<CivicIdentity>())
            {
                w.SetCivicIdentity(hall.Id,CivicIdentity.Hall);string before=w.SaveJson();
                Check(w.SetCivicIdentity(hall.Id,identity),"Identity command rejected");await Frames();
                Check((int)_cottages[hall.Id].Body.GetMeta("civic_identity")== (int)identity && Math.Abs(_cottages[hall.Id].Body.RotationDegrees.Y-rotation*90)<.01,"Identity/rotation model failed");
                string saved=w.SaveJson();var copy=World.LoadJson(saved);Check(copy.SaveJson()==saved,"Identity reload differs");
                copy.SetCivicIdentity(hall.Id,CivicIdentity.Hall);Check(copy.SaveJson()==before,"Identity modified unrelated state");
                var unchanged=World.LoadJson(before);copy=World.LoadJson(saved);
                for(int i=0;i<400;i++){copy.Tick(.1f);unchanged.Tick(.1f);}copy.SetCivicIdentity(hall.Id,CivicIdentity.Hall);
                Check(copy.SaveJson()==unchanged.SaveJson(),"Identity changed service/production continuation");
                if(rotation==0)await Capture($"artifacts/civic-occupied-{identity}.png");
            }
            if(rotation==0)foreach(int width in new[]{960,1440})
            {
                GetWindow().Size=new(width,width==960?640:900);SelectBuilding(hall.Id);await Frames();if(!_identityDetails.Visible)await UiClick(_identityToggle);
                await UiClick(_identityChoices[CivicIdentity.Chapel]);await Frames();Check(hall.Identity==CivicIdentity.Chapel,"Inspector chapel click failed");
                await UiClick(_identityChoices[CivicIdentity.PlantedCourt]);await Frames();Check(hall.Identity==CivicIdentity.PlantedCourt,"Inspector court click failed");
                _inspectionScroll.EnsureControlVisible(_identityChoices[CivicIdentity.PlantedCourt]);await Frames();
                Check(_identityChoices.Values.All(b=>b.GetGlobalRect().End.X<=width),"Identity choices overflow");await Capture($"artifacts/civic-inspector-{width}.png");
                await UiClick(_identityChoices[CivicIdentity.Hall]);Check(hall.Identity==CivicIdentity.Hall,"Hall reset failed");
            }
            w.SetCivicIdentity(hall.Id,chosen);Check(w.RequestDemolition(hall.Id) && !w.SetCivicIdentity(hall.Id,CivicIdentity.Hall),"Dismantling accepts style change");
            for(int i=0;i<10000 && hall.DemolitionProgress==0;i++)w.Tick(.1f);await Frames();
            Check(hall.DemolitionProgress>0 && (int)_cottages[hall.Id].Body.GetMeta("civic_identity")== (int)chosen,"Dismantling lost identity");
            string dismantling=w.SaveJson();w=World.LoadJson(dismantling);Check(w.SaveJson()==dismantling,"Dismantling identity reload differs");
            for(int i=0;i<10000 && w.Cottages.Any(c=>c.Id==hall.Id);i++)w.Tick(.1f);
            Check(!w.Cottages.Any(c=>c.Id==hall.Id) && !w.SetCivicIdentity(hall.Id,chosen),"Demolished venue accepts identity");w.Validate();
        }
        var village=World.NewCreative();foreach(var tree in village.Trees.ToArray())village.SetClearing(tree.Cell,true);
        for(int i=0;i<3;i++)
        {
            var hall=village.Place(new(-5+i*5,-4),i,BuildingKind.GatheringHall)??throw new Exception("Gallery hall failed");village.SetCivicIdentity(hall.Id,(CivicIdentity)i);
        }
        foreach(var c in new[]{new Cell(-4,0),new Cell(4,0)})Check(village.Place(c,0,BuildingKind.Cottage)!=null,"Gallery cottage failed");
        var house=village.Cottages.First(c=>c.Kind==BuildingKind.Cottage);Check(!village.SetCivicIdentity(house.Id,CivicIdentity.Chapel) && !village.SetCivicIdentity(-1,CivicIdentity.Hall) && !village.SetCivicIdentity(village.Cottages[0].Id,(CivicIdentity)99),"Invalid identity command accepted");
        foreach(bool invalidEnum in new[]{false,true})
        {
            var data=System.Text.Json.Nodes.JsonNode.Parse(village.SaveJson())!;
            var entry=data["Buildings"]!.AsArray().First(c=>(int)c!["Id"]! ==(invalidEnum?village.Cottages[0].Id:house.Id))!;
            entry["Identity"]=invalidEnum?99:1;
            bool rejected=false;try{World.LoadJson(data.ToJsonString());}catch(InvalidOperationException){rejected=true;}
            Check(rejected,"Invalid identity save accepted");
        }
        AdoptWorld(village);CloseManagementUi();_paused=true;_noticeUntil=0;GetWindow().Size=new(1440,900);_focus=new(0,0,-3);_camera.Size=23;_angle=.72f;UpdateCamera();await Frames();await Capture("artifacts/civic-neighborhood.png");
        var canceled=World.NewQuarryMap();var plan=canceled.Place(new(0,-3),0,BuildingKind.GatheringHall)!;canceled.SetCivicIdentity(plan.Id,CivicIdentity.Chapel);Check(canceled.Cancel(plan.Id) && !canceled.SetCivicIdentity(plan.Id,CivicIdentity.Hall),"Canceled plan retained identity");canceled.Validate();
        // Construction sheet compares code-native models without pretending they are finished projects.
        _dynamic.Hide();_landscape.Hide();_decorationView?.Hide();_pathView?.Hide();_hud.Hide();
        var sheet=new Node3D();AddChild(sheet);
        for(int identity=0;identity<3;identity++)for(int stage=0;stage<4;stage++)
        {
            var body=new Node3D{Position=new(stage*4-6,0,identity*4-4)};sheet.AddChild(body);MakeBuilding(body,new Cottage{Kind=BuildingKind.GatheringHall,Identity=(CivicIdentity)identity},stage);
        }
        Box(sheet,new(0,-.12f,0),new(18,.15f,15),new("777f62"));_focus=Vector3.Zero;_camera.Size=28;UpdateCamera();await Frames();await Capture("artifacts/civic-stages.png");
        GD.Print("PASS: civic identities on plans, four orientations, actual construction/visitors, identical continuation, exact saves, 960/1440 inspector, cancellation/demolition and matched gallery/stages.");
    }
}
