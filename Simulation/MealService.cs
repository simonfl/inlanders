using System;
using System.Collections.Generic;
using System.Linq;

namespace Inlanders.Simulation;

public sealed class MealRequest
{
    public int? SourceId { get; set; }
    public int Id { get; set; }
    public float Due { get; set; }
    public Resource Kind { get; set; }
    public bool Reserved { get; set; }
    public bool Carrying { get; set; }
    public bool Eaten { get; set; }
    public bool Closed { get; set; }
    public Cell Seat { get; set; }
}
public sealed record MealOutcome(int Request,int Person,float Time,bool Timely,bool Skipped=false);
public sealed record MealConsumption(int Request,int Person,float Time,Resource Kind,bool Late);

public sealed partial class World
{
    public static readonly Resource[] EdibleKinds={Resource.Berries,Resource.Vegetables,Resource.Bread,Resource.Fish,Resource.Game,Resource.Fruit};
    public int CentralFood(Resource kind) => kind switch
    {
        Resource.Berries=>Food.Berries, Resource.Vegetables=>Food.Vegetables, Resource.Bread=>Food.Bread,
        Resource.Fish=>Food.Fish, Resource.Game=>Food.Game, Resource.Fruit=>Food.Fruit, _=>0
    };
    private void ChangeCentralFood(Resource kind,int amount)
    {
        switch(kind)
        {
            case Resource.Berries: Food.Berries+=amount; break;
            case Resource.Vegetables: Food.Vegetables+=amount; break;
            case Resource.Bread: Food.Bread+=amount; break;
            case Resource.Fish: Food.Fish+=amount; break;
            case Resource.Game: Food.Game+=amount; break;
            case Resource.Fruit: Food.Fruit+=amount; break;
            default: throw new InvalidOperationException("Not an edible food");
        }
    }
    public int MealReserved(Resource kind) => People.Count(p=>p.Meal is {Reserved:true} r && r.Kind==kind);
    public int CentralFoodAvailable(Resource kind) => FoodAvailableAt(null,kind);
    // Goods remain delivered while carried out of storage, but a producer's first trip is not a delivery yet.
    private int DeliveredFood(Resource kind)
    {
        int moving=People.Where(p=>p.Cargo==kind && (p.FoodTransfer || p.Meal is {Carrying:true})).Sum(p=>p.Carried);
        return StoredFood(kind)+moving-(int)CreativeNet(kind)+(kind switch
        {
            Resource.Berries=>Food.EatenBerries+Food.TradedBerries-Food.InitialBerries,
            Resource.Vegetables=>Food.EatenVegetables,
            Resource.Bread=>Food.EatenBread+Food.SupperBread,
            Resource.Fish=>Food.EatenFish,
            Resource.Game=>Food.EatenGame,
            Resource.Fruit=>Food.EatenFruit,
            _=>throw new InvalidOperationException("Not an edible delivery")
        });
    }
    private static bool MealWork(Work task) => task is Work.ToMealSupply or Work.ToMealSeat or Work.EatingMeal or Work.ReturnMeal;
    private bool MealSpotReserved(Cell cell) => People.Any(p=>p.Meal is {} r && (r.Reserved || r.Carrying) && r.Seat==cell);

    private void RetireMeal(Villager person,bool skipCurrentWindow=false)
    {
        person.NextMealTime=person.Meal!.Due+60;
        while(person.NextMealTime+(skipCurrentWindow?0:60)<Food.Time)
        {
            Food.MealOutcomes.Add(new(Food.NextMealId++,person.Id,person.NextMealTime,false,true));
            RecentFood.Add(new(Food.Time,Required:1));
            person.NextMealTime+=60;
        }
        person.Meal=null;
    }
    private void AdvanceMealRequests()
    {
        Food.MealOutcomes.RemoveAll(m=>m.Time<Food.Time-180);
        Food.MealConsumptions.RemoveAll(m=>m.Time<Food.Time-180);
        foreach(var p in People)
        {
            if(p.Meal==null && Food.Time>=p.NextMealTime)
                p.Meal=new() {Id=Food.NextMealId++,Due=p.NextMealTime};
            var r=p.Meal; if(r==null) continue;
            if(Food.Time>=r.Due+60 && !r.Closed)
            {
                r.Closed=true; p.Fed=r.Eaten;
                Food.MealOutcomes.Add(new(r.Id,p.Id,r.Due+60,r.Eaten));
                RecentFood.Add(new(Food.Time,Required:1));
                if(!r.Carrying)
                {
                    if(MealWork(p.Task)) { p.Route.Clear(); p.Task=Work.Waiting; p.Timer=0; }
                    RetireMeal(p); continue;
                }
            }
            if(r.Carrying && Food.Time>=r.Due+90 && p.Task!=Work.ReturnMeal)
                Go(p,YardAccess,Work.ReturnMeal,"Returning an uneaten late meal");
        }
        Food.Hunger=People.Count(p=>!p.Fed)/(float)Population;
    }
    private bool ClaimMeal(Villager person)
    {
        var r=person.Meal;
        if(Creative || r==null || r.Eaten || r.Closed || person.Carried>0) return false;
        var occupied=People.Where(p=>p.LeisureSiteId!=null || p.Task is Work.ToRest or Work.Resting).Select(p=>p.Destination).ToHashSet();
        foreach(var source in FoodStores().OrderBy(id=>TravelCost(At(person),FoodAccess(id))).ThenBy(id=>id??0))
        {
            var access=FoodAccess(source);
            var kinds=EdibleKinds.Where(k=>FoodAvailableAt(source,k)>0).OrderBy(k=>Food.MealConsumptions.Count(m=>m.Kind==k)+
                People.Count(p=>p.Meal is {} meal && (meal.Reserved || meal.Carrying) && meal.Kind==k)).ToArray();
            if(kinds.Length==0 || FindPath(At(person),access,Blocked)==null) continue;
            var seat=Map.Land.Where(c=>(c.Point-access.Point).LengthSquared()<=4 && !Blocked(c) && !MealSpotReserved(c) && !ComfortSpotReserved(c) && !occupied.Contains(c))
                .OrderBy(c=>(c.Point-access.Point).LengthSquared()).ThenBy(c=>c.Z).ThenBy(c=>c.X)
                .Cast<Cell?>().FirstOrDefault(c=>FindPath(access,c!.Value,Blocked)!=null);
            if(seat==null) continue;
            r.SourceId=source; r.Kind=kinds[0]; r.Reserved=true; r.Seat=seat.Value;
            Go(person,access,Work.ToMealSupply,source is int id?$"Collecting a meal at pantry {id}":"Collecting a meal at the central pantry"); return true;
        }
        return false;
    }
    private bool InterruptMeal(Villager p)
    {
        if(!MealWork(p.Task)) return false;
        var r=p.Meal ?? throw new InvalidOperationException("Meal work without request");
        r.Reserved=false;
        if(r.Carrying) Go(p,YardAccess,Work.ReturnMeal,"Returning an uneaten meal before changing jobs");
        else {p.Route.Clear(); p.Task=Work.Waiting; p.Timer=0;}
        return true;
    }
    private void TickMeal(Villager p)
    {
        var r=p.Meal ?? throw new InvalidOperationException("Missing meal request");
        switch(p.Task)
        {
            case Work.ToMealSupply:
                ChangeFoodAt(r.SourceId,r.Kind,-1); r.Reserved=false; r.Carrying=true;
                p.Cargo=r.Kind; p.Carried=1;
                Go(p,r.Seat,Work.ToMealSeat,"Carrying a meal to a nearby seat"); break;
            case Work.ToMealSeat:
                p.Task=Work.EatingMeal; p.Timer=0; p.Status="Eating a meal"; break;
            case Work.EatingMeal:
                if(p.Timer<4) return;
                switch(r.Kind)
                {
                    case Resource.Berries: Food.EatenBerries++; break;
                    case Resource.Vegetables: Food.EatenVegetables++; break;
                    case Resource.Bread: Food.EatenBread++; break;
                    case Resource.Fish: Food.EatenFish++; break;
                    case Resource.Game: Food.EatenGame++; break;
                    case Resource.Fruit: Food.EatenFruit++; break;
                }
                p.Carried=0; r.Carrying=false; r.Eaten=true; p.Fed=true;
                Food.MealConsumptions.Add(new(r.Id,p.Id,Food.Time,r.Kind,r.Closed));
                RecentFood.Add(new(Food.Time,Eaten:1));
                if(r.Closed) RetireMeal(p,true);
                Finish(p); break;
            case Work.ReturnMeal:
                ChangeCentralFood(r.Kind,p.Carried); p.Carried=0; r.Carrying=false;
                if(r.Closed) RetireMeal(p,true);
                Finish(p); break;
        }
    }
    private void SnapshotMeals()
    {
        var meals=Food.MealConsumptions.Where(m=>m.Time>Food.Time-60).GroupBy(m=>m.Person).Select(g=>g.Last()).ToArray();
        int Count(Resource kind)=>meals.Count(m=>m.Kind==kind);
        Food.LastMealBerries=Count(Resource.Berries); Food.LastMealVegetables=Count(Resource.Vegetables);
        Food.LastMealBread=Count(Resource.Bread); Food.LastMealFish=Count(Resource.Fish); Food.LastMealGame=Count(Resource.Game);
        Food.LastMealFruit=Count(Resource.Fruit);
        Food.LastMealRequired=Population; Food.LastMealChoices=meals.Select(m=>m.Kind).Distinct().Count();
        int quarter=(Population+3)/4;
        if(meals.Length==Population && Food.LastMealVegetables>=quarter && meals.Length-Food.LastMealVegetables>=quarter) Food.VegetableChoiceMeals++;
        RecordRiverMeal(); RecordLakeMeal(); RecordQuarryMeal(); RecordWoodsMeal();
    }
    private void ValidateMealService()
    {
        if(Food.MealOutcomes==null || Food.MealConsumptions==null || Food.MealOutcomes.Any(m=>m.Request<1 || m.Request>=Food.NextMealId || m.Person<0 || m.Person>=Population || !float.IsFinite(m.Time) || m.Time<0 || m.Time>Food.Time) ||
            Food.MealConsumptions.Any(m=>m.Request<1 || m.Request>=Food.NextMealId || m.Person<0 || m.Person>=Population || !float.IsFinite(m.Time) || m.Time<0 || m.Time>Food.Time || !EdibleKinds.Contains(m.Kind)))
            throw new InvalidOperationException("Invalid meal history");
        if(Food.NextMealId<1 || Food.MealOutcomes.Select(m=>m.Request).Distinct().Count()!=Food.MealOutcomes.Count || Food.MealConsumptions.Select(m=>m.Request).Distinct().Count()!=Food.MealConsumptions.Count)
            throw new InvalidOperationException("Duplicate meal accounting");
        var active=People.Where(p=>p.Meal!=null).Select(p=>p.Meal!).ToArray();
        var seats=active.Where(r=>r.Reserved || r.Carrying).Select(r=>r.Seat).ToArray();
        if(active.Select(r=>r.Id).Distinct().Count()!=active.Length || seats.Distinct().Count()!=seats.Length || seats.Any(Blocked)) throw new InvalidOperationException("Overlapping meal reservations");
        foreach(var kind in EdibleKinds) if(CentralFoodAvailable(kind)<0) throw new InvalidOperationException("Overreserved meals");
        foreach(var p in People)
        {
            if(!float.IsFinite(p.NextMealTime) || p.NextMealTime<0) throw new InvalidOperationException("Invalid meal schedule");
            var r=p.Meal;
            if(r==null) {if(MealWork(p.Task)) throw new InvalidOperationException("Orphan meal work"); continue;}
            if(r.Reserved && r.SourceId is int source && !Cottages.Any(c=>c.Id==source && c.Kind==BuildingKind.Pantry && c.Complete && !c.DemolitionRequested && !c.WorkPaused))
                throw new InvalidOperationException("Missing meal supply source");
            if(r.Id<1 || r.Id>=Food.NextMealId || !float.IsFinite(r.Due) || r.Due<0 || r.Due>Food.Time ||
                r.Reserved!=(p.Task==Work.ToMealSupply) || r.Carrying!=(p.Task is Work.ToMealSeat or Work.EatingMeal or Work.ReturnMeal) ||
                ((r.Reserved || r.Carrying || r.Eaten) && !EdibleKinds.Contains(r.Kind)) ||
                (r.Carrying && (p.Carried!=1 || p.Cargo!=r.Kind)) || (r.Reserved && r.Carrying) || (r.Eaten && (r.Reserved || r.Carrying)))
                throw new InvalidOperationException("Invalid meal ownership");
        }
    }
}
