using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckQuietCivicVisits()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<4;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        (Transform3D,Transform3D,Transform3D,Transform3D) Pose(int id)
        {var v=_people[id];return(v.Rig.Transform,v.Head.Transform,v.Arm.Transform,v.LeftArm.Transform);}
        _goldenHour=false;ApplyAtmosphere();_showWorldLabels=false;ApplyWorldLabels();
        for(int rotation=0;rotation<4;rotation++)
        {
            var w=World.NewCreative();foreach(var p in w.People)w.Assign(p.Id,Role.Unassigned);
            var cell=w.Map.Land.Where(c=>w.PlacementProblem(c,rotation,BuildingKind.GatheringHall)==null)
                .OrderBy(c=>(c.Point-new Cell(3,0).Point).LengthSquared()).First();
            var hall=w.Place(cell,rotation,BuildingKind.GatheringHall)!;
            AdoptWorld(w);_paused=true;CloseManagementUi();_noticeUntil=0;
            for(int i=0;i<3000 && w.People.Count(p=>p.Task==Work.Leisure && p.Timer>1)<3;i++)w.Tick(.05f);
            var visitors=w.People.Where(p=>p.Task==Work.Leisure && p.Timer>1).Select(p=>p.Id).ToArray();
            Check(visitors.Length>=3,"No actual civic group");
            await Frames();string baseline=w.SaveJson();var hallPose=Pose(visitors[0]);
            foreach(var identity in new[]{CivicIdentity.Chapel,CivicIdentity.PlantedCourt})
            {
                w.SetCivicIdentity(hall.Id,identity);await Frames();
                foreach(int id in visitors)
                {
                    var p=w.People.Single(p=>p.Id==id);var view=_people[id];
                    var inward=new Vector3(cell.X-p.Position.X,0,cell.Z-p.Position.Y).Normalized();
                    Check((-view.Body.GlobalBasis.Z).Dot(identity==CivicIdentity.Chapel?inward:-inward)>.99f,"Quiet facing differs from venue");
                    Check(!view.RestStool.Visible && view.Arm.Rotation.X<.31f && view.LeftArm.Rotation.X<.31f,"Quiet visitor retained seat or conversational gesture");
                }
                string saved=w.SaveJson();var pausedPose=Pose(visitors[0]);await Frames();
                Check(saved==w.SaveJson() && pausedPose==Pose(visitors[0]),"Pause changed quiet visit");
                _focus=OnGround(cell.X,cell.Z);_angle=.72f;_camera.Size=12;UpdateCamera();await Frames();
                await Capture($"artifacts/quiet-civic-{identity}-{rotation}.png");
                if(rotation==0){_camera.Size=21;UpdateCamera();await Frames();await Capture($"artifacts/quiet-civic-{identity}-village.png");}
                AdoptWorld(World.LoadJson(saved));_paused=true;await Frames();w=_world;
                Check(pausedPose==Pose(visitors[0]),"Reload changed quiet pose");
                var unchanged=World.LoadJson(baseline);int completed=w.People.Sum(p=>p.LeisureVisits);
                for(int i=0;i<300;i++){w.Tick(.05f);unchanged.Tick(.05f);}
                await Frames();
                Check(w.People.Sum(p=>p.LeisureVisits)>completed,"Quiet visitors never complete visits");
                w.SetCivicIdentity(hall.Id,CivicIdentity.Hall);
                Check(w.SaveJson()==unchanged.SaveJson(),"Quiet visits changed simulation continuation");
                AdoptWorld(World.LoadJson(saved));_paused=true;w=_world;
                w.Assign(visitors[0],Role.Builder);await Frames();
                Check(w.People.Single(p=>p.Id==visitors[0]).Task!=Work.Leisure && _people[visitors[0]].Head.Rotation==Vector3.Zero && !_people[visitors[0]].RestStool.Visible,"Interrupted quiet pose retained");
                w.Validate();
                AdoptWorld(World.LoadJson(baseline));_paused=true;await Frames();w=_world;
                Check(hallPose==Pose(visitors[0]),"Hall identity did not restore conversation");
            }
            // Approach remains walking regardless of the chosen architecture.
            w=World.NewCreative();foreach(var p in w.People)w.Assign(p.Id,Role.Unassigned);
            hall=w.Place(cell,rotation,BuildingKind.GatheringHall)!;w.SetCivicIdentity(hall.Id,CivicIdentity.Chapel);
            for(int i=0;i<3000 && !w.People.Any(p=>p.Task==Work.ToLeisure && p.Route.Count>0);i++)w.Tick(.05f);
            var arrival=w.People.First(p=>p.Task==Work.ToLeisure && p.Route.Count>0);
            AdoptWorld(w);_paused=true;await Frames();
            Check(_people[arrival.Id].Head.Rotation==Vector3.Zero && !_people[arrival.Id].RestStool.Visible,"Walking arrival uses quiet pose");
        }
        GD.Print("PASS: quiet civic groups in four orientations, actual arrival/completion, pause/reload, identity restoration, interruption and identical simulation continuation.");
    }
}
