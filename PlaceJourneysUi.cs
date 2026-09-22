using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
public partial class Game
{
    private Button _workCardTrips=null!,_placeTripNext=null!,_placeTripFollow=null!,_placeTripFrame=null!;
    private VBoxContainer _placeTripsPanel=null!;
    private Label _placeTripText=null!;
    private Line2D _placeTripLine=null!;
    private int _placeJourneySite=-1,_placeJourneyPerson=-1;
    private PlaceJourney? _placeJourney;
    private string? _placeJourneyEnding;
    private void MakePlaceJourneys(VBoxContainer column)
    {
        _workCardTrips=Button("Journeys here",()=>{_placeJourneySite=_placeJourneySite==_workCardSite?-1:_workCardSite;_placeJourneyPerson=-1;_placeJourney=null;_placeJourneyEnding=null;});column.AddChild(_workCardTrips);
        _placeTripsPanel=new();column.AddChild(_placeTripsPanel);_placeTripText=Text("",14,true);_placeTripsPanel.AddChild(_placeTripText);
        var row=new HBoxContainer();_placeTripsPanel.AddChild(row);
        _placeTripNext=Button("Next trip",()=>{
            _placeJourneyEnding=null;_placeJourney=null;var trips=_world.ReadPlaceJourneys(_placeJourneySite);int at=Array.FindIndex(trips,t=>t.Person==_placeJourneyPerson);
            _placeJourneyPerson=trips.Length>0?trips[(at+1)%trips.Length].Person:-1;
        });row.AddChild(_placeTripNext);
        _placeTripFrame=Button("Frame trip",()=>{
            if(_placeJourney is not {} trip)return;
            var points=trip.Steps.Select(c=>c.Point).Append(trip.Position).ToArray();
            float x=(points.Min(p=>p.X)+points.Max(p=>p.X))/2,z=(points.Min(p=>p.Y)+points.Max(p=>p.Y))/2;
            _focus=OnGround(x,z);_camera.Size=Math.Clamp(Math.Max(points.Max(p=>p.X)-points.Min(p=>p.X),points.Max(p=>p.Y)-points.Min(p=>p.Y))*1.6f+9,12,MaximumZoom);_followPerson=false;_watchOrbit=false;UpdateCamera();
        });row.AddChild(_placeTripFrame);
        _placeTripFollow=Button("Follow",()=>{if(_placeJourney is {} trip){ShowDailyLife(trip.Person);_followPerson=true;}});row.AddChild(_placeTripFollow);
        _placeTripLine=new(){Width=3,Antialiased=true,ZIndex=-1};_hud.AddChild(_placeTripLine);_placeTripsPanel.Hide();
    }
    private void RenderPlaceJourneys()
    {
        _workCardTrips.Visible=_world.PublicPlace!=null && _workCard.Visible && _yardPreviewSide<0;
        bool show=_workCardTrips.Visible && _placeJourneySite==_workCardSite;
        _placeTripsPanel.Visible=show;_placeTripLine.Visible=show;
        _workCardTrips.Text=show?"Hide journeys":"Journeys here";
        if(!show){_placeJourney=null;return;}
        var trips=_world.ReadPlaceJourneys(_placeJourneySite);
        var next=trips.FirstOrDefault(t=>t.Person==_placeJourneyPerson);
        if(_placeJourney is {} prior && (next==null || next.Activity!=prior.Activity || next.Steps[^1]!=prior.Steps[^1]))
        {
            var person=_world.People.FirstOrDefault(p=>p.Id==prior.Person);
            bool reached=person!=null && (person.Position-prior.Steps[^1].Point).LengthSquared()<.04f;
            _placeJourneyEnding=reached?$"{prior.Name} reached the destination"+(prior.Amount>0?$" carrying {prior.Amount} {prior.Cargo!.Value.ToString().ToLowerInvariant()}.":"."):$"{prior.Name}'s trip changed before reaching this destination.";
            _placeJourney=null;
        }
        if(_placeJourneyEnding==null)_placeJourney=next??trips.FirstOrDefault();
        _placeJourneyPerson=_placeJourney?.Person??_placeJourneyPerson;
        _placeTripNext.Disabled=trips.Length<(_placeJourneyEnding==null?2:1);_placeTripFrame.Disabled=_placeTripFollow.Disabled=_placeJourney==null;
        if(_placeJourney is not {} trip){_placeTripText.Text=_placeJourneyEnding??"No trip is under way here. Let daily life continue; no journey is invented.";_placeTripLine.ClearPoints();return;}
        _placeTripText.Text=$"{trip.Name} · {trip.Relation}\n{trip.Activity}\n"+(trip.Amount>0?$"Carrying {trip.Amount} {trip.Cargo!.Value.ToString().ToLowerInvariant()}":"Hands free")+$" · {trips.Length} current trips";
        _placeTripLine.DefaultColor=new(trip.Amount>0?"edc57c":"89c7cd");
        _placeTripLine.Points=new[]{_camera.UnprojectPosition(PresentedPerson(trip.Person)+Vector3.Up*.25f)}.Concat(trip.Steps.Select(c=>_camera.UnprojectPosition(OnGround(c.X,c.Z,.25f)))).ToArray();
    }
}
