using System;
using System.Collections.Generic;
using System.Linq;
namespace Inlanders.Simulation;
public sealed class NewcomerChallenge
{
    public float? InvitedAt {get;set;}
    public bool Arrived {get;set;}
    public bool Complete {get;set;}
    public bool Continuing {get;set;}
    public int DeliveryBaseline {get;set;}
    public Dictionary<int,int> Meals {get;set;}=new();
}
public sealed partial class World
{
    public NewcomerChallenge? Challenge=>Founding?.Challenge;
    public static World NewNewcomerChallenge(bool relaxed=false)
    {
        var w=NewRiverLivelihood();w.Founding!.Challenge=new();
        w.Map.Name="Une place pour les nouveaux · A place for newcomers";
        w._yardLogs-=12;w.InitialLogs-=12;
        w.History.Clear();w.History.Add("Four neighbors seek a home here. Establish food, prepare four beds, then invite them. They arrive thirty seconds after your invitation. Help them settle through ordinary meals; choose your own homes, food sources and paths.");
        w.Validate();return relaxed?RelaxedHamletFrom(w):w;
    }
    public int ChallengeDeliveries=>Challenge is {Arrived:true} c?Math.Max(0,DeliveredEdible-c.DeliveryBaseline):DeliveredEdible;
    public int ChallengeNeighborsFed=>Challenge?.Meals.Count(p=>p.Value>=2)??0;
    public string? ChallengeInvitationProblem()=>Challenge is not {} c?"No newcomer challenge.":c.InvitedAt!=null?"The invitation is already sent.":
        SpareBeds<4?"Finish four spare beds for the newcomers.":DeliveredEdible<16?"Deliver 16 locally produced food portions; starting provisions do not count.":
        EdibleStored<32?"Keep 32 food portions ready for the larger village.":People.Any(p=>!p.Fed)?"Restore meals for current residents before inviting more.":
        ArrivalSpots().Length<2?"Clear the arrival path beside the yard.":null;
    public bool InviteChallengeNeighbors()
    {
        if(ChallengeInvitationProblem()!=null)return false;
        Challenge!.InvitedAt=Food.Time;History.Add("Four neighbors accepted the invitation. Arrival in 30 seconds.");return true;
    }
    private void AdvanceNewcomerChallenge()
    {
        if(Challenge is not {} c || c.Complete)return;
        if(c.InvitedAt is float at && !c.Arrived && Food.Time>=at+30)
        {
            var names=new[]{"Jeanne","Pierre","Marguerite","Louis"};
            foreach(var name in names)People.Add(new Villager{Id=Population,Name=name,Position=YardAccess.Point,Role=Role.Unassigned,SharedWorker=true,NextMealTime=Food.Time+15,Status="Arrived — settling into village life"});
            c.Arrived=true;c.DeliveryBaseline=DeliveredEdible;ReconcileHomes();
            foreach(var person in People.Where(p=>p.Id>=12))
                if(person.HomeId is int id && Accessible(Cottages.Single(b=>b.Id==id).Entrance))Go(person,Cottages.Single(b=>b.Id==id).Entrance,Work.ToArrival,"Walking to a new home");
            _retry=0;History.Add("Four neighbors arrived. Help them settle into their homes and ordinary meals.");
        }
        if(c.Arrived && Housed==Population && ChallengeNeighborsFed==4 && ChallengeDeliveries>=32 && People.All(p=>p.Fed))
        {c.Complete=true;History.Add("The newcomers have settled: homes, two meals each and 32 fresh food deliveries since arrival. This challenge is complete.");}
    }
    private void RecordChallengeMeal(Villager person)
    {
        if(Challenge is {Arrived:true,Complete:false} c && person.Id>=12)c.Meals[person.Id]=Math.Min(2,c.Meals.GetValueOrDefault(person.Id)+1);
    }
    private void ValidateNewcomerChallenge()
    {
        if(Challenge is not {} c)return;
        if(PublicPlace==null || Population!=(c.Arrived?16:12) || c.InvitedAt is float at && (!float.IsFinite(at) || at<0 || at>Food.Time) ||
            c.Arrived && c.InvitedAt==null || c.DeliveryBaseline<0 || c.DeliveryBaseline>DeliveredEdible || c.Meals==null ||
            c.Meals.Any(p=>p.Key<12 || p.Key>=Population || p.Value is <1 or >2) || c.Complete && (!c.Arrived || ChallengeNeighborsFed!=4) || c.Continuing && !c.Complete)
            throw new InvalidOperationException("Invalid newcomer challenge");
    }
}
