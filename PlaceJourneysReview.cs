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
        int person=_placeJourney!.Person;await UiClick(_placeTripFrame);await Frames();
        Check(saved==_world.SaveJson() && _placeTripLine.Points.Length>1,"Journey inspection changed world or lacks committed route");
        Check(_workCard.GetGlobalRect().End.Y<_hud.Size.Y-76,"Journey card overflows compact screen");
        await CaptureReviewBundle("place-meal-under-way");
        await UiClick(_placeTripPath);await Frames();Check(_tripPathProposal!=null && !_tripPathApply.Disabled && saved==_world.SaveJson(),"Trip path preview mutated or rejected");
        Check(_workCard.GetGlobalRect().End.Y<_hud.Size.Y-76,"Path proposal overflows compact screen");
        await CaptureReviewBundle("trip-path-proposal");
        await UiClick(_tripPathActions.GetChild<Button>(1));await Frames();Check(_tripPathProposal==null && saved==_world.SaveJson(),"Trip path cancellation mutated");
        await UiClick(_placeTripPath);await Frames();var routeProposal=_tripPathProposal!.Value;
        Check(_world.PathConnection(routeProposal.Start,routeProposal.End,out var proposed)==null,"Proposal invalid");
        await UiClick(_tripPathApply);await Frames();Check(_tripPathProposal==null && proposed.Where(c=>!_world.Map.Water.Contains(c)).All(_world.Paths.Contains),"Trip path not applied");
        _world.Validate();var restored=World.LoadJson(_world.SaveJson());Check(restored.SaveJson()==_world.SaveJson(),"Trip path save differs");

        _speed=3;_paused=false;double timeout=Time.GetTicksMsec()+30000;
        while(_placeJourneyEnding==null && Time.GetTicksMsec()<timeout)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
        _paused=true;Check(_placeJourneyEnding?.Contains("reached the destination")==true,"Actual arrival was not observed");
        await CaptureReviewBundle("place-meal-arrived");saved=_world.SaveJson();await Press(Key.H);await Frames();Check(!_placeTripLine.IsVisibleInTree(),"Journey leaks into Watch");await Press(Key.Escape);await Frames();Check(saved==_world.SaveJson(),"Watch changed world");
        ClearSelection();Check(_placeJourneySite==-1,"Journey survived deselection");_world.Validate();
        GD.Print("PASS: actual meal route inspection/frame, pure controls, observed arrival, compact layout and Watch boundary.");
    }
}

