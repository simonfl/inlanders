using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    // Scripted player-input coverage. Waiting is accelerated by simulation ticks, not a native playtest.
    private async Task ProbeNeighborhoodJourney()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<4;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        async Task Until(Func<bool> done,string why,int ticks=24000)
        {
            for(int i=0;i<ticks && !done();i++)
            {
                _world.Tick(.1f);
                if(i%100==0){_world.Validate();RenderActors(0);UpdateHud();await Frames();}
            }
            Check(done(),why);RenderActors(0);UpdateHud();await Frames();
        }
        async Task<Cottage> Build(BuildingKind kind,Cell center,int? fixedRotation=null)
        {
            var choice=_world.Map.Land.Concat(_world.Map.Water).Distinct()
                .SelectMany(cell=>Enumerable.Range(0,4).Select(rotation=>new{cell,rotation}))
                .Where(p=>(fixedRotation==null || p.rotation==fixedRotation) && _world.PlacementProblem(p.cell,p.rotation,kind)==null)
                .OrderBy(p=>(p.cell.Point-center.Point).LengthSquared()).ThenBy(p=>p.cell.Z).ThenBy(p=>p.cell.X).ThenBy(p=>p.rotation).First();
            _focus=OnGround(choice.cell.X,choice.cell.Z);_camera.Size=20;UpdateCamera();
            await UiClick(_kindButtons[kind]);await Frames();
            for(int i=0;i<4 && _rotation!=choice.rotation;i++)await Press(Key.R);
            Check(_rotation==choice.rotation,"Building facing control failed");
            var point=_camera.UnprojectPosition(OnGround(choice.cell.X,choice.cell.Z));
            Check(!PointerOverHud(point),"Journey placement hidden by UI");
            Input.ParseInputEvent(new InputEventMouseMotion{Position=point,GlobalPosition=point});await Frames();
            int previous=_world.Cottages.Count;await Click(point);await Frames();
            Check(_world.Cottages.Count==previous+1,"Journey placement click refused: "+kind);
            var site=_world.Cottages.Last();await Press(Key.Escape);
            await Until(()=>site.Complete,"Journey construction failed: "+kind);return site;
        }
        Check(_world.HasWorkplaceFood && _world.Population==8,"Journey requires chosen fresh neighborhood");
        await Build(BuildingKind.Bridge,new(5,2),1);
        await Build(BuildingKind.Cottage,new(8,2));await Build(BuildingKind.Cottage,new(10,4));
        var venue=await Build(BuildingKind.SeatingGarden,new(8,3));
        SelectBuilding(venue.Id);await Frames();await UiClick(_welcomeHere);await Frames();
        Check(_world.Neighborhood!.VenueId==venue.Id,"Journey welcome venue click failed");
        await OpenMenu(2);await UiClick(_neighborhoodCommit);await Frames();
        Check(_world.Neighborhood.CommittedAt!=null,"Journey arrival click failed");
        await Until(()=>_world.Neighborhood.Complete,"Journey welcome did not finish");
        await CaptureReviewBundle();
        var hut=_world.Cottages.Single(c=>c.Kind==BuildingKind.ForagerHut);
        SelectBuilding(hut.Id);await Frames();
        // Use the same inspector control as ordinary production management.
        await UiClick(_productionPause);await Frames();
        Check(hut.WorkPaused,"Journey producer pause click failed");
        await Until(()=>_world.Food.Hunger>0,"Stopping the food source never caused a shortage");
        await CaptureReviewBundle();
        await Build(BuildingKind.Farm,new(17,3));await Build(BuildingKind.Farm,new(21,3));
        await Build(BuildingKind.Bakery,new(17,7));await Build(BuildingKind.Bakery,new(21,7));
        var pantry=await Build(BuildingKind.Pantry,new(14,5));
        SelectBuilding(pantry.Id);await Frames();
        while(pantry.PantryTarget<16){await UiClick(_pantryMore);await Frames();}
        await Until(()=>_world.Food.Hunger==0 && _world.EdibleStored>=12,"Player food-chain recovery failed");
        _world.Validate();await Press(Key.F5);string saved=_world.SaveJson();await Press(Key.F9);await Frames();
        Check(_world.SaveJson()==saved,"Journey save/load changed recovered state");
        _focus=OnGround(12,4);_camera.Size=29;UpdateCamera();ClearSelection();CloseDrawer();await Frames();await CaptureReviewBundle();
        GD.Print("PASS: scripted placement/facing, venue/arrival, completed welcome, producer pause, natural shortage, farm/bakery/pantry recovery and save/load through player controls; waits use simulation ticks");
    }
}
