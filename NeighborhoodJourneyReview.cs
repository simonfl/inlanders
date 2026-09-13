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
        if(_world.IsInheritedShoreline)
        {
            await OpenMenu(2);await CaptureReviewBundle("inherited-opening");
            await UiClick(_journeyAction);await Frames();Check(_showFoodMap && !_drawer.Visible,"Food journey action failed");
            await CaptureReviewBundle("inherited-food-world");ToggleFoodMap();
            await Build(BuildingKind.Bridge,new(5,2),1);
            foreach(var location in new[]{new Cell(-5,-2),new(-2,-2),new(1,-2),new(1,6)})await Build(BuildingKind.Cottage,location);
            var inheritedVenue=_world.Cottages.Single(c=>c.Kind==BuildingKind.SeatingGarden);
            await OpenMenu(2);await UiClick(_neighborhoodVenue);await Frames();Check(_selectedSite==inheritedVenue.Id,"Inherited west venue was not selected");
            await UiClick(_welcomeHere);await Frames();
            var sources=_world.Cottages.Where(c=>_world.IsWorkplaceFoodStore(c)).ToArray();
            foreach(var source in sources){SelectBuilding(source.Id);await Frames();await UiClick(_productionPause);}
            await OpenMenu(2);await UiClick(_neighborhoodCommit);
            await Until(()=>_world.Neighborhood!.Arrived && _world.Food.Hunger>0,"Paused sources did not expose shortage");
            Check(!_world.Neighborhood!.Complete,"Food shutdown falsely completed welcome");await OpenMenu(2);await CaptureReviewBundle("inherited-shortage");
            foreach(var source in sources){SelectBuilding(source.Id);await Frames();await UiClick(_productionPause);}
            await Until(()=>_world.Neighborhood!.Complete,"Resumed inherited food did not recover welcome");
            await OpenMenu(2);await CaptureReviewBundle("inherited-complete");
            string end=_world.SaveJson();await UiClick(_staySettlement);await Frames();Check(!_watching && _hud.IsVisibleInTree() && _world.SaveJson()==end,"Keep building hid controls");
            await OpenMenu(2);await UiClick(_watchSettlement);await Frames();Check(_watching,"Watch did not open");await Press(Key.H);await Frames();
            await Press(Key.F5);string savedInlet=_world.SaveJson();await Press(Key.F9);await Frames();Check(_world.IsInheritedShoreline && _world.SaveJson()==savedInlet,"Inlet reload differs");
            GD.Print("PASS: inherited inlet entry, food view, bridge, west homes and venue, production shutdown/recovery, finite ending, independent keep-building/watch and exact reload (scripted)");return;
        }
        if(!_world.Neighborhood!.FoodLandChallenge)
        {
            await OpenMenu(2);await Frames();Check(_journeyAction.IsVisibleInTree() && _journeyStep==0,"Opening lacks crossing suggestion");
            await CaptureReviewBundle("guided-opening");await UiClick(_journeyAction);await Frames();
            Check(_placing && _buildKind==BuildingKind.Bridge,"Suggested crossing action failed");await Press(Key.Escape);
        }
        await Build(BuildingKind.Bridge,new(5,2),1);
        bool challenge=_world.Neighborhood!.FoodLandChallenge;
        foreach(var location in new[]{new Cell(8,2),new(10,4),new(17,1),new(21,1)}.Take(_world.NeighborhoodArrivals/2))
            await Build(BuildingKind.Cottage,location);
        var venue=await Build(BuildingKind.SeatingGarden,new(8,3));
        SelectBuilding(venue.Id);await Frames();await UiClick(_welcomeHere);await Frames();
        Check(_world.Neighborhood!.VenueId==venue.Id,"Journey welcome venue click failed");
        await OpenMenu(2);await UiClick(_neighborhoodCommit);await Frames();
        Check(_world.Neighborhood.CommittedAt!=null,"Journey arrival click failed");
        await Until(()=>_world.Neighborhood.Arrived && _world.Neighborhood.Welcomed.Count==_world.Population,"Journey welcome did not finish");
        if(!challenge) await Until(()=>_world.Neighborhood.Complete,"Welcome and housing did not complete opening");
        await CaptureReviewBundle();
        var hut=_world.Cottages.Single(c=>c.Kind==BuildingKind.ForagerHut);
        if(!challenge)
        {
            SelectBuilding(hut.Id);await Frames();
            await UiClick(_productionPause);await Frames();
            Check(hut.WorkPaused,"Journey producer pause click failed");
        }
        await Until(()=>_world.Food.Hunger>0,"Stopping the food source never caused a shortage");
        await CaptureReviewBundle();
        await Build(BuildingKind.Farm,new(17,3));await Build(BuildingKind.Farm,new(21,3));
        await Build(BuildingKind.Bakery,new(17,7));await Build(BuildingKind.Bakery,new(21,7));
        var pantry=await Build(BuildingKind.Pantry,new(14,5));
        SelectBuilding(pantry.Id);await Frames();
        while(pantry.PantryTarget<16){await UiClick(_pantryMore);await Frames();}
        await Until(()=>_world.Neighborhood.Complete && _world.Food.Hunger==0 && _world.EdibleStored>=(challenge?_world.NeighborhoodReserveTarget:12),"Player food-chain recovery failed");
        await OpenMenu(2);await Frames();Check(_nextSettlement.IsVisibleInTree() && _staySettlement.IsVisibleInTree() && !_journeyAction.Visible,"Finite ending actions missing");
        await CaptureReviewBundle("finite-settlement-complete");string finished=_world.SaveJson();await UiClick(_staySettlement);await Frames();
        Check(!_watching && _hud.IsVisibleInTree() && _world.SaveJson()==finished,"Keep building hid controls or changed settlement");
        await OpenMenu(2);await UiClick(_watchSettlement);await Frames();Check(_watching,"Watch action failed");await Press(Key.H);await Frames();Check(!_watching && _hud.IsVisibleInTree(),"Return from completed view failed");
        _world.Validate();await Press(Key.F5);string saved=_world.SaveJson();await Press(Key.F9);await Frames();
        Check(_world.SaveJson()==saved,"Journey save/load changed recovered state");
        _focus=OnGround(12,4);_camera.Size=29;UpdateCamera();ClearSelection();CloseDrawer();await Frames();await CaptureReviewBundle();
        GD.Print($"PASS: scripted placement/facing, venue/arrival, completed welcome, {(challenge?"scarce wild food and reserve objective":"producer pause")}, natural shortage, farm/bakery/pantry recovery and save/load through player controls; waits use simulation ticks");
    }
}

