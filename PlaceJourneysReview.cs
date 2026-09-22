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
        Cottage? field=null;
        for(int i=0;i<6000 && field==null;i++)
        {
            field=_world.Cottages.FirstOrDefault(c=>_world.ReadPlaceJourneys(c.Id).Any(t=>t.Relation=="Meal from this place" && t.Amount>0));
            if(field==null)_world.Tick(.1f);
        }
        Check(field!=null,"No actual carried meal from any workplace in observation interval");
        RenderActors(0);ShowWorkplaceCard(field!.Id);await Frames();string saved=_world.SaveJson();
        await UiClick(_workCardTrips);await Frames();
        for(int i=0;i<_world.Population && (_placeJourney?.Relation!="Meal from this place" || _placeJourney.Amount==0);i++){await UiClick(_placeTripNext);await Frames();}
        Check(_placeJourney is {Relation:"Meal from this place",Amount:>0},"No real meal trip visible");
        int person=_placeJourney!.Person;await UiClick(_placeTripFrame);await Frames();
        Check(saved==_world.SaveJson() && _placeTripLine.Points.Length>1,"Journey inspection changed world or lacks committed route");
        Check(_workCard.GetGlobalRect().End.Y<_hud.Size.Y-76,"Journey card overflows compact screen");
        await CaptureReviewBundle("place-meal-under-way");
        _speed=3;_paused=false;double timeout=Time.GetTicksMsec()+30000;
        while(_placeJourneyEnding==null && Time.GetTicksMsec()<timeout)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
        _paused=true;Check(_placeJourneyEnding?.Contains("reached the destination")==true,"Actual arrival was not observed");
        await CaptureReviewBundle("place-meal-arrived");saved=_world.SaveJson();await Press(Key.H);await Frames();Check(!_placeTripLine.IsVisibleInTree(),"Journey leaks into Watch");await Press(Key.Escape);await Frames();Check(saved==_world.SaveJson(),"Watch changed world");
        ClearSelection();Check(_placeJourneySite==-1,"Journey survived deselection");_world.Validate();
        GD.Print("PASS: actual meal route inspection/frame, pure controls, observed arrival, compact layout and Watch boundary.");
    }
}
