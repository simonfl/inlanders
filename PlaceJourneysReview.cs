using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
public partial class Game
{
    private async Task ProbePlaceJourneys()
    {
        void Check(bool ok,string why){if(!ok)throw new Exception(why);}
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        Check(_placeFood.IsVisibleInTree(),"Public food summary missing");
        await Click(_placeFood.GetGlobalRect().GetCenter());await Frames();
        Check(_drawer.Visible && _tabs.CurrentTab==4,"Food summary does not open full Economy");
        await Press(Key.Escape);await Frames();
        Check(_topBar.GetGlobalRect().End.X<=_hud.Size.X,"Public status overflows viewport");
        Cottage? field=null;
        for(int i=0;i<6000 && field==null;i++)
        {
            field=_world.Cottages.FirstOrDefault(c=>_world.ReadPlaceJourneys(c.Id).Any(t=>t.Relation=="Meal from this place" && t.Amount>0));
            if(field==null)_world.Tick(.1f);
        }
        Check(field!=null,"No actual carried meal from any workplace in observation interval");
        RenderActors(0);ShowWorkplaceCard(field!.Id);await Frames();string saved=_world.SaveJson();
        Check(!_workCardWorker.Visible && !_workCardDiner.Visible,"Default card did not consolidate following controls");
        await CaptureReviewBundle("place-default-card");
        await UiClick(_workCardWatchPlace);await Frames();
        Check(_watching && !_followPerson && World.Footprint(field).All(c=>new Rect2(Vector2.Zero,_hud.Size).HasPoint(_camera.UnprojectPosition(OnGround(c.X,c.Z)))) && saved==_world.SaveJson(),"Watch place failed to frame workplace purely");
        await Press(Key.Escape);await Frames();ShowWorkplaceCard(field.Id);await Frames();
        await UiClick(_workCardTrips);await Frames();
        Check(!_workCardWorker.Visible && !_workCardDiner.Visible,"Journey view duplicates follow controls");
        for(int i=0;i<_world.Population && (_placeJourney?.Relation!="Meal from this place" || _placeJourney.Amount==0);i++){await UiClick(_placeTripNext);await Frames();}
        Check(_placeJourney is {Relation:"Meal from this place",Amount:>0},"No real meal trip visible");
        int householdPerson=_placeJourney!.Person;int homeId=_world.People[householdPerson].HomeId!.Value;
        ShowWorkplaceCard(homeId);await Frames();Check(_workCardTrips.IsVisibleInTree(),"Home journeys unavailable");
        await UiClick(_workCardTrips);await Frames();Check(!_householdPeople.Visible && _placeJourney is {Relation:"Household trip"},"Household journey not actual or stacked");
        Check(_workCard.GetGlobalRect().End.X<=_hud.Size.X && _placeTripPath.GetGlobalRect().End.X<=_hud.Size.X,"Right household card clips controls");await CaptureReviewBundle("household-journey");var originFocus=_focus;float originZoom=_camera.Size;
        await UiClick(_placeTripFollow);await Frames();Check(_dailyPerson>=0 && _householdOrigin==homeId,"Household follow lost origin");
        await UiClick(_dailyHomeBack);await Frames();Check(_workCardSite==homeId && _focus==originFocus && _camera.Size==originZoom && saved==_world.SaveJson(),"Household return changed world/view");
        ShowWorkplaceCard(field.Id);await Frames();await UiClick(_workCardTrips);await Frames();
        for(int i=0;i<_world.Population && (_placeJourney?.Relation!="Meal from this place" || _placeJourney.Amount==0);i++){await UiClick(_placeTripNext);await Frames();}
        int person=_placeJourney!.Person;await UiClick(_placeTripFrame);await Frames();
        Check(saved==_world.SaveJson() && _placeTripLine.Points.Length>1,"Journey inspection changed world or lacks committed route");
        Check(_workCard.GetGlobalRect().End.X<=_hud.Size.X && _placeTripPath.GetGlobalRect().End.X<=_hud.Size.X && _workCard.GetGlobalRect().End.Y<_hud.Size.Y-76,"Journey card overflows compact screen");
        await CaptureReviewBundle("place-meal-under-way");
        var endpoint=_placeJourney!.Steps[^1];await UiClick(_placeTripPath);await Frames();
        Check(_pathAnchor==field.Entrance && _pathDraftEnd!=null && !_pathProposalApply.Disabled && saved==_world.SaveJson(),"Unified trip path preview mutated or lost durable origin");
        await UiClick(_pathProposalFrame);await Frames();await CaptureReviewBundle("trip-world-path-proposal");
        await Press(Key.Escape);await Frames();Check(!_placing && _workCardSite==field.Id && saved==_world.SaveJson(),"Trip path cancellation lost origin");
        await UiClick(_workCardTrips);await Frames();await UiClick(_placeTripPath);await Frames();
        Check(_world.PathConnection(ConnectionStops(_pathDraftEnd!.Value),out var proposed)==null,"Proposal invalid");
        await UiClick(_pathProposalApply);await Frames();Check(!_placing && proposed.Where(c=>!_world.Map.Water.Contains(c)).All(_world.Paths.Contains),"Trip path not applied");
        _world.Validate();var restored=World.LoadJson(_world.SaveJson());Check(restored.SaveJson()==_world.SaveJson(),"Trip path save differs");
        await UiClick(_workCardTrips);await Frames();
        for(int i=0;i<_world.Population && _placeJourney?.Person!=person;i++){await UiClick(_placeTripNext);await Frames();}
        _speed=3;_paused=false;double timeout=Time.GetTicksMsec()+30000;
        while(_placeJourneyEnding==null && Time.GetTicksMsec()<timeout)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
        _paused=true;Check(_placeJourneyEnding?.Contains("reached the destination")==true,"Actual arrival was not observed");
        await CaptureReviewBundle("place-meal-arrived");saved=_world.SaveJson();await Press(Key.H);await Frames();Check(!_placeTripLine.IsVisibleInTree(),"Journey leaks into Watch");await Press(Key.Escape);await Frames();Check(saved==_world.SaveJson(),"Watch changed world");
        ClearSelection();Check(_placeJourneySite==-1,"Journey survived deselection");_world.Validate();
        GD.Print("PASS: actual meal route inspection/frame, pure controls, observed arrival, compact layout and Watch boundary.");
    }
}

