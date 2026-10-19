using System;
using System.Collections.Generic;
using System.Linq;
namespace Inlanders.Simulation;
public sealed class NewcomerChallenge
{
    public bool Repair {get;set;}
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
    public static World NewNewcomerChallenge(bool relaxed=false,bool repair=false)
    {
        var w=NewTransformationHamlet(false,true,false,true);
        foreach(var person in w.People)w.Interrupt(person);
        if(!repair)foreach(var field in w.Cottages.Where(b=>b.Kind==BuildingKind.VegetableField).ToArray())w.Cottages.Remove(field);
        w.Founding!.Challenge=new(){Repair=repair};
        if(repair)
        {
            for(int x=-10;x<=-5;x++)w.Map.Water.Add(new(x,0));
            w.Paths.RemoveWhere(w.Map.Water.Contains);
            Array.Clear(w.Map.Heights);
        }
        w.Founding.StartingBuildings=w.Cottages.Select(b=>new StartingBuilding{Id=b.Id,Cell=b.Cell,Rotation=b.Rotation,Kind=b.Kind}).ToList();
        w.Map.Name="Une place pour les nouveaux · A place for newcomers";
        w._yardLogs=18;w.InitialLogs=w.SawnLogs+w.Trees.Sum(t=>t.Logs)+w.Cottages.Sum(b=>b.Delivered)+w._yardLogs;
        w.Food.InitialBerries=w.Food.Berries=48;
        w.History.Clear();w.History.Add("Four neighbors seek a home here. The south bank is crowded; a small kitchen garden cannot feed an enlarged village alone. Open northern ground, shorten the journey across the inlet, or make more room beside the existing homes. Prepare four beds and food, then invite them. They arrive thirty seconds after your invitation. Help them settle through ordinary meals; choose your own homes, food sources and paths.");
        if(repair){w.Map.Name="Les champs de l’autre rive · The far-bank fields";w.History.Clear();w.History.Add("The northern fields have been cut off by the river. Twelve residents still have a kitchen garden and provisions, but four newcomers need a lasting food supply. Reconnect the fields with a crossing, move cultivation onto the home bank, or choose another livelihood. No prescribed building is required. Invite the neighbors only when you are ready.");}
        w.Validate();w.ValidateMapOccupancy();return relaxed?RelaxedHamletFrom(w):w;
    }
    public int ChallengeDeliveries=>Challenge is {Arrived:true} c?Math.Max(0,DeliveredEdible-c.DeliveryBaseline):DeliveredEdible;
    public bool ChallengeNeedsHome(Villager person)=>person.HomeId is not int id || !Cottages.Any(b=>b.Id==id && IsHome(b) && Accessible(b.Entrance));
    public int ChallengeHoused=>People.Count(p=>!ChallengeNeedsHome(p));
    public string ChallengeStatus=>Challenge is not {} c?"Challenge":c.Complete?"Challenge · complete":c.InvitedAt==null?(ChallengeInvitationProblem()==null?"Ready to invite four":"Challenge · preparing"):!c.Arrived?"Neighbors on their way":
        ChallengeHoused<Population?$"Homes · {ChallengeHoused}/{Population}":People.Any(p=>!p.Fed)?"Meals need attention":ChallengeNeighborsFed<4?$"Settling · {ChallengeNeighborsFed}/4":ChallengeDeliveries<32?$"Fresh food · {ChallengeDeliveries}/32":"Settling";
    public string? ChallengeSettlingProblem()=>Challenge is not {Arrived:true}?"Wait for the newcomers to arrive.":
        ChallengeHoused<Population?"Some residents need an accessible finished home.":
        People.FirstOrDefault(p=>!p.Fed) is {} hungry?hungry.Name+" missed a meal. Restore their food access.":
        ChallengeNeighborsFed<4?"Let each newcomer collect and eat two ordinary meals.":
        ChallengeDeliveries<32?"Deliver 32 fresh portions after arrival. Starting stores do not count.":null;
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
        if(c.Arrived && ChallengeSettlingProblem()==null)
        {c.Complete=true;History.Add("The newcomers have settled: homes, two meals each and 32 fresh food deliveries since arrival. This challenge is complete.");}
    }
    private void RecordChallengeMeal(Villager person)
    {
        if(Challenge is {Arrived:true,Complete:false} c && person.Id>=12)c.Meals[person.Id]=Math.Min(2,c.Meals.GetValueOrDefault(person.Id)+1);
    }
    private void ValidateNewcomerChallenge()
    {
        if(Challenge is not {} c)return;
        if(PublicPlace==null || (c.Continuing?Population<16:Population!=(c.Arrived?16:12)) || c.InvitedAt is float at && (!float.IsFinite(at) || at<0 || at>Food.Time) ||
            c.Arrived && c.InvitedAt==null || c.DeliveryBaseline<0 || c.DeliveryBaseline>DeliveredEdible || c.Meals==null ||
            c.Meals.Any(p=>p.Key<12 || p.Key>=16 || p.Value is <1 or >2) || c.Complete && (!c.Arrived || ChallengeNeighborsFed!=4 || ChallengeDeliveries<32) || c.Continuing && !c.Complete)
            throw new InvalidOperationException("Invalid newcomer challenge");
    }
}
