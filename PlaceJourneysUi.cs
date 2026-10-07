using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
public partial class Game
{
    private Button _workCardTrips=null!,_placeTripNext=null!,_placeTripFollow=null!,_placeTripFrame=null!,_placeTripPath=null!;
    private HBoxContainer _tripPathActions=null!;
    private Button _tripPathApply=null!;
    private (Cell Start,Cell End)? _tripPathProposal;
    private VBoxContainer _placeTripsPanel=null!;
    private Label _placeTripText=null!;
    private Line2D _placeTripLine=null!;
    private int _placeJourneySite=-1,_placeJourneyPerson=-1;
    private PlaceJourney? _placeJourney;
    private string? _placeJourneyEnding;
    private void MakePlaceJourneys(VBoxContainer column)
    {
        _workCardTrips=Button("People & trips",()=>{_placeJourneySite=_placeJourneySite==_workCardSite?-1:_workCardSite;_placeJourneyPerson=-1;_placeJourney=null;_placeJourneyEnding=null;_nextWorkCard=0;});column.AddChild(_workCardTrips);
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
        _placeTripFollow=Button("Follow",()=>{if(_placeJourney is {} trip){ShowDailyLife(trip.Person,_placeJourneySite);_followPerson=true;}});row.AddChild(_placeTripFollow);
        _placeTripPath=Button("Preview path",()=>{
            if(_placeJourney is not {} trip)return;
            _tripPathProposal=(new Cell((int)MathF.Round(trip.Position.X),(int)MathF.Round(trip.Position.Y)),trip.Steps[^1]);
        });row.AddChild(_placeTripPath);
        _placeTripPath.TooltipText="Preview a walking path from this resident's current ground to the actual destination. No ground changes until you apply.";
        _tripPathActions=new();_placeTripsPanel.AddChild(_tripPathActions);
        _tripPathApply=Button("Lay this path",()=>{
            if(_tripPathProposal is {} proposal && _world.ConnectPaths(proposal.Start,proposal.End)){_tripPathProposal=null;Notice("Walking path laid. Residents choose routes using the new ground.");}
        });_tripPathActions.AddChild(_tripPathApply);
        _tripPathActions.AddChild(Button("Cancel path",()=>_tripPathProposal=null));_tripPathActions.Hide();
        _placeTripLine=new(){Width=3,Antialiased=true,ZIndex=-1};_hud.AddChild(_placeTripLine);_placeTripsPanel.Hide();
    }
    private void RenderPlaceJourneys()
    {
        _workCardTrips.Visible=_reshapingPlot<0 && _world.PublicPlace!=null && _world.Cottages.Any(c=>c.Id==_workCardSite && (Buildings.Get(c.Kind).Beds==0 || !_homeOptions)) && _workCard.Visible && _yardPreviewSide<0;
        bool show=_workCardTrips.Visible && _placeJourneySite==_workCardSite;
        _placeTripsPanel.Visible=show;_placeTripLine.Visible=show;
        bool household=_world.Cottages.Any(c=>c.Id==_workCardSite && Buildings.Get(c.Kind).Beds>0);
        _workCardTrips.Text=show?"Back to place":household?"Household journeys":"People & trips";
        _workCardTrips.TooltipText=household?"Actual trips by the people who live here. They may be working or eating elsewhere; these are not all journeys to this house.":"Inspect actual journeys related to this place.";
        if(!show){_placeJourney=null;_tripPathProposal=null;return;}
        _tripPathActions.Visible=_tripPathProposal!=null;
        if(_tripPathProposal is {} proposal)
        {
            string? problem=_world.PathConnection(proposal.Start,proposal.End,out var route);
            _tripPathApply.Disabled=problem!=null;
            _placeTripNext.Disabled=_placeTripFrame.Disabled=_placeTripFollow.Disabled=_placeTripPath.Disabled=true;
            _placeTripText.Text=problem??$"Walking path · {route.Count} tiles\nFrom the observed position to the trip’s destination. Village life continues; this proposal stays in place.";
            _placeTripLine.DefaultColor=new(problem==null?"8fd3d1":"e38673");
            _placeTripLine.Points=route.Select(c=>_camera.UnprojectPosition(OnGround(c.X,c.Z,.25f))).ToArray();return;
        }
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
        _placeTripNext.Disabled=trips.Length<(_placeJourneyEnding==null?2:1);_placeTripFrame.Disabled=_placeTripFollow.Disabled=_placeTripPath.Disabled=_placeJourney==null;
        if(_placeJourney is not {} trip){_placeTripText.Text=_placeJourneyEnding??"No trip is under way here. Let daily life continue; no journey is invented.";_placeTripLine.ClearPoints();return;}
        _placeTripText.Text=$"{trip.Name} · {trip.Relation}\n{trip.Activity}\n"+(trip.Amount>0?$"Carrying {trip.Amount} {trip.Cargo!.Value.ToString().ToLowerInvariant()}":"Hands free")+$" · {trips.Length} current trips";
        _placeTripLine.DefaultColor=new(trip.Amount>0?"edc57c":"89c7cd");
        _placeTripLine.Points=new[]{_camera.UnprojectPosition(PresentedPerson(trip.Person)+Vector3.Up*.25f)}.Concat(trip.Steps.Select(c=>_camera.UnprojectPosition(OnGround(c.X,c.Z,.25f)))).ToArray();
    }
}
