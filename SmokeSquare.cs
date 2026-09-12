using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckSquareOrientations()
    {
        async Task Frames(){for(int i=0;i<4;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        for(int rotation=0;rotation<4;rotation++)
        {
            var w=World.NewCampaign(4);
            var cell=w.Map.Land.Where(c=>w.PlacementProblem(c,rotation,BuildingKind.Square)==null)
                .OrderBy(c=>(c.Point-new Cell(3,3).Point).LengthSquared()).First();
            var square=w.Place(cell,rotation,BuildingKind.Square)!;
            AdoptWorld(w);_paused=true;CloseManagementUi();
            for(int i=0;i<8000 && !square.Complete;i++)w.Tick(.1f);
            if(!square.Complete)throw new Exception("Square construction stalled");
            _focus=OnGround(cell.X,cell.Z);_camera.Size=11;UpdateCamera();
            foreach(int width in new[]{960,1440})
            {
                GetWindow().Size=new(width,width==960?640:900);await Frames();
                await Capture($"artifacts/f23b4-empty-{rotation}-{width}.png");
            }
            for(int i=0;i<5000 && !w.People.Any(p=>p.LeisureSiteId==square.Id && p.Task==Work.Leisure);i++)w.Tick(.05f);
            if(!w.People.Any(p=>p.LeisureSiteId==square.Id && p.Task==Work.Leisure))throw new Exception("No actual square visitor");
            w.Validate();
            var footprint=World.Footprint(cell,rotation,BuildingKind.Square).ToHashSet();
            if(w.People.Where(p=>p.LeisureSiteId==square.Id).Any(p=>footprint.Contains(p.Destination)))throw new Exception("Visitors overlap furniture footprint");
            _focus=OnGround(cell.X,cell.Z);_camera.Size=11;UpdateCamera();
            foreach(int width in new[]{960,1440})
            {
                GetWindow().Size=new(width,width==960?640:900);await Frames();
                await Capture($"artifacts/f23b4-square-{rotation}-{width}.png");
            }
            string saved=w.SaveJson();await Frames();
            if(w.SaveJson()!=saved)throw new Exception("Paused square review changed settlement");
            AdoptWorld(World.LoadJson(saved));_paused=true;w=_world;await Frames();
            // Supply the supper fixture with accounted bread; production is tested separately.
            w.Food.Bread+=w.SupperCost;w.Food.BakedBread+=w.SupperCost;
            w.Food.UsedGrain+=w.SupperCost/2;w.Food.GrownGrain+=w.SupperCost/2;
            if(!w.BeginSupper())throw new Exception("Square supper could not start");
            for(int i=0;i<5000 && !w.People.All(p=>p.Task==Work.Supper);i++)w.Tick(.05f);
            if(!w.People.All(p=>p.Task==Work.Supper) || w.People.Any(p=>footprint.Contains(p.Destination)))throw new Exception("Supper approach failed");
            w.Validate();_focus=OnGround(cell.X,cell.Z);_camera.Size=11;UpdateCamera();
            foreach(int width in new[]{960,1440})
            {
                GetWindow().Size=new(width,width==960?640:900);await Frames();
                await Capture($"artifacts/f23b4-supper-{rotation}-{width}.png");
            }
            saved=w.SaveJson();AdoptWorld(World.LoadJson(saved));_paused=true;w=_world;
            for(int i=0;i<1000 && !w.Food.SupperComplete;i++)w.Tick(.05f);
            if(!w.Food.SupperComplete)throw new Exception("Saved supper did not finish");w.Validate();
        }
        GD.Print("PASS: square construction, real breaks, four orientations, 960/1440, open approaches, pause/reload and supper completion.");
    }
}
