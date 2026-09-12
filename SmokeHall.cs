using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckHallArchitecture()
    {
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        await Frames();
        var thumbnail=_kindButtons[BuildingKind.GatheringHall].GetChild<HBoxContainer>(0).GetChild<TextureRect>(0).Texture;
        Check(thumbnail.GetImage().SavePng("artifacts/hall-thumbnail.png")==Error.Ok,"Hall thumbnail capture failed");
        for(int rotation=0;rotation<4;rotation++)
        {
            var w=World.LoadFile("artifacts/quarry-prepared.json");var old=w.CampaignHall!;
            Check(w.RequestDemolition(old.Id),"Cannot prepare rotated hall fixture");
            for(int i=0;i<8000 && w.Cottages.Contains(old);i++)w.Tick(.1f);
            Check(!w.Cottages.Contains(old),"Old hall did not finish material recovery");
            var cell=w.Map.Land.Where(c=>w.PlacementProblem(c,rotation,BuildingKind.GatheringHall)==null)
                .OrderBy(c=>(c.Point-old.Cell.Point).LengthSquared()).First();
            var hall=w.Place(cell,rotation,BuildingKind.GatheringHall)!;
            AdoptWorld(w);_paused=true;CloseManagementUi();
            for(int i=0;i<8000 && !hall.Complete;i++)w.Tick(.1f);
            Check(hall.Complete,"Rotated hall construction stalled");
            for(int i=0;i<4000 && !w.People.Any(p=>p.LeisureSiteId==hall.Id && p.Task==Work.Leisure);i++)w.Tick(.05f);
            Check(w.People.Any(p=>p.LeisureSiteId==hall.Id && p.Task==Work.Leisure),"No real hall visitor");w.Validate();
            var footprint=World.Footprint(cell,rotation,BuildingKind.GatheringHall).ToHashSet();
            Check(w.People.Where(p=>p.LeisureSiteId==hall.Id).All(p=>!footprint.Contains(p.Destination)),"Visitors entered blocked hall footprint");
            _focus=OnGround(cell.X,cell.Z);_camera.Size=11;_angle=.72f;UpdateCamera();_noticeUntil=0;
            foreach(int width in new[]{960,1440})
            {
                GetWindow().Size=new(width,width==960?640:900);await Frames();
                await Capture($"artifacts/hall-{rotation}-{width}.png");
            }
            string saved=w.SaveJson();await Frames();Check(saved==w.SaveJson(),"Paused hall review mutated village");
            AdoptWorld(World.LoadJson(saved));await Frames();Check(saved==_world.SaveJson(),"Hall visit reload differed");w=_world;
            for(int i=0;i<4000 && !w.People.Any(p=>p.LastLeisureSiteId==hall.Id);i++)w.Tick(.05f);
            Check(w.People.Any(p=>p.LastLeisureSiteId==hall.Id),"Reloaded hall visit did not finish");w.Validate();
        }
        AdoptWorld(World.LoadFile("artifacts/quarry-complete.json"));CloseManagementUi();_noticeUntil=0;
        _focus=new(-1,0,0);_camera.Size=22;_angle=.72f;UpdateCamera();await Frames();await Capture("artifacts/hall-village-1440.png");
        _dynamic.Hide();_landscape.Hide();if(_pathView!=null)_pathView.Hide();_hud.Hide();_watchRoot.Hide();
        _showWorldLabels=false;ApplyWorldLabels();
        var sheet=new Node3D();AddChild(sheet);
        for(int stage=0;stage<4;stage++)
        {
            var model=new Node3D{Position=new(stage*4-6,0,0)};sheet.AddChild(model);
            MakeBuilding(model,new Cottage{Kind=BuildingKind.GatheringHall},stage);
        }
        Box(sheet,new(0,-.12f,0),new(18,.15f,6),new("777f62"));
        _focus=Vector3.Zero;_camera.Size=21;UpdateCamera();await Frames();await Capture("artifacts/hall-stages.png");
        GD.Print("PASS: hall actual construction/recovery, four orientations, real visitors outside footprint, pause/reload and completed visits; 960/1440 and construction sheet.");
    }
}
