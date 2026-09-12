using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;

public partial class Game
{
    private async void RunSocialSmoke()
    {
        try
        {
            async System.Threading.Tasks.Task Frames() { for(int i=0;i<4;i++) await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame); }
            async System.Threading.Tasks.Task CaptureVisit(string name,Vector3 focus)
            {
                _focus=focus;
                foreach(int width in new[]{960,1440})
                {
                    GetWindow().Size=new(width,width==960?640:900); _camera.Size=10; UpdateCamera(); await Frames();
                    await Capture($"artifacts/f04b-{name}-{width}.png");
                }
                GetWindow().Size=new(960,640); _camera.Size=20; UpdateCamera(); await Frames(); await Capture($"artifacts/f04b-{name}-village.png");
            }
            var w=World.NewCreative(); foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
            var square=w.Place(new(3,0),false,BuildingKind.Square) ?? throw new Exception("Square fixture failed");
            AdoptWorld(w); _paused=true; CloseDrawer();
            for(int i=0;i<2000 && w.People.Count(p=>p.Task==Work.Leisure)<2;i++) w.Tick(.05f);
            await Frames(); var person=w.People.FirstOrDefault(p=>p.Task==Work.Leisure && SquareCompanion(p)!=null) ?? throw new Exception("No square pair");
            var companion=SquareCompanion(person)!;
            if(SquareCompanion(companion)?.Id!=person.Id) throw new Exception("Non-mutual conversation pair");
            foreach(var p in new[]{person,companion})
            {
                var partner=SquareCompanion(p)!; var facing=-_people[p.Id].Body.GlobalBasis.Z;
                var desired=new Vector3(partner.Position.X-p.Position.X,0,partner.Position.Y-p.Position.Y).Normalized();
                if(facing.Dot(desired)<.99f) throw new Exception("Visitors do not face one another");
            }
            var pose=_people[person.Id].Arm.GlobalTransform; string saved=w.SaveJson(); await Frames();
            if(saved!=w.SaveJson() || pose!=_people[person.Id].Arm.GlobalTransform) throw new Exception("Paused social pose changed");
            await CaptureVisit("square",OnGround(square.Cell.X,square.Cell.Z));
            AdoptWorld(World.LoadJson(saved)); _paused=true; await Frames(); w=_world; person=w.People[person.Id];
            if(pose!=_people[person.Id].Arm.GlobalTransform || SquareCompanion(person)?.Id!=companion.Id) throw new Exception("Reload changed conversation");
            bool quiet=false,gesture=false;
            for(int i=0;i<100 && person.Task==Work.Leisure;i++)
            {
                await Frames(); float arm=_people[person.Id].Arm.Rotation.X;
                quiet|=arm<.2f; gesture|=arm>.35f;
                w.Tick(.05f);
            }
            if(!quiet || !gesture) throw new Exception("Conversation lacks gesture or quiet interval");
            // Remove all other visitors through normal reassignment.
            for(int i=0;i<2000 && w.People.All(p=>p.Task!=Work.Leisure);i++) w.Tick(.05f);
            person=w.People.First(p=>p.Task==Work.Leisure);
            foreach(var other in w.People.Where(p=>p.Id!=person.Id)) w.Assign(other.Id,Role.Builder);
            await Frames();
            if(SquareCompanion(person)!=null || _people[person.Id].Arm.Rotation.X>.2f) throw new Exception("Lone visitor talks to absent partner");
            w.Assign(person.Id,Role.Builder); await Frames();
            if(person.Task==Work.Leisure || _people[person.Id].RestStool.Visible || _people[person.Id].LeftArm.Rotation!=Vector3.Zero) throw new Exception("Departure retained social props/gesture");
            w.Validate();
            foreach(bool rotated in new[]{false,true})
            {
                w=World.NewCreative(); foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
                var home=w.Place(new(3,0),rotated) ?? throw new Exception("Home fixture failed");
                AdoptWorld(w); _paused=true; person=w.People[0];
                for(int i=0;i<3000 && !(person.Task==Work.Resting && person.Timer>.9f);i++) w.Tick(.05f);
                if(person.Task!=Work.Resting) throw new Exception("No actual home rest");
                await Frames(); var rig=_people[0]; var outward=new Vector3(person.Position.X-home.Cell.X,0,person.Position.Y-home.Cell.Z).Normalized();
                if(!rig.RestStool.Visible || (-rig.Body.GlobalBasis.Z).Dot(outward)<.99f) throw new Exception("Rest faces wall or lacks seat");
                saved=w.SaveJson(); pose=rig.Head.GlobalTransform; await Frames();
                if(saved!=w.SaveJson() || pose!=rig.Head.GlobalTransform) throw new Exception("Paused home pose changed");
                await CaptureVisit($"home-{rotated}",OnGround(home.Cell.X,home.Cell.Z));
                AdoptWorld(World.LoadJson(saved)); _paused=true; await Frames();
                if(pose!=_people[0].Head.GlobalTransform || !_people[0].RestStool.Visible) throw new Exception("Reload changed home rest");
                w=_world; person=w.People[0]; int visits=person.RestVisits;
                for(int i=0;i<200 && person.Task==Work.Resting;i++) w.Tick(.05f);
                await Frames();
                if(person.RestVisits!=visits+1 || _people[0].RestStool.Visible) throw new Exception("Completed rest did not release seat or count correctly");
                AdoptWorld(World.LoadJson(saved)); _paused=true; _world.Assign(0,Role.Builder); await Frames();
                if(_people[0].RestStool.Visible || _world.People[0].RestVisits!=visits) throw new Exception("Interrupted rest retained seat or earned credit");
                _world.Validate();
            }
            w=World.NewCreative(); foreach(var p in w.People) w.Assign(p.Id,Role.Unassigned);
            foreach(var at in new[]{new Cell(0,0),new(3,0),new(6,0),new(0,6)})
                if(w.Place(at)==null) throw new Exception("Village home placement failed");
            if(w.Place(new(3,6),false,BuildingKind.Square)==null || w.Place(new(6,6),false,BuildingKind.VegetableGarden)==null) throw new Exception("Village venue placement failed");
            w.Assign(0,Role.Logger); w.Assign(1,Role.Logger); w.Assign(2,Role.Farmer); w.Assign(3,Role.Farmer);
            AdoptWorld(w); _paused=true;
            for(int i=0;i<10000 && !(w.People.Any(p=>p.Task==Work.Resting) && w.People.Any(p=>p.Task==Work.Leisure));i++) w.Tick(.05f);
            if(!w.People.Any(p=>p.Task==Work.Resting) || !w.People.Any(p=>p.Task==Work.Leisure)) throw new Exception("Working village did not show both visit routines");
            await Frames(); await CaptureVisit("inhabited",new(3,0,3)); w.Validate();
            await CheckSquareOrientations();
            GD.Print("PASS: mutual actual visitors, quiet/gesture intervals, lone/departed visitors, outward seated home rest, pause/reload/completion/interruption and 960/1440 inhabited-village captures."); GetTree().Quit();
        }
        catch(Exception e) { GD.PrintErr(e); GetTree().Quit(1); }
    }
}
