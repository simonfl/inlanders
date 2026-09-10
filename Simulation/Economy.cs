using System;
using System.Collections.Generic;
using System.Linq;

namespace Inlanders.Simulation;

public sealed record EconomyStock(Resource Resource, int Stored, int Reserved, int Carried, int AtWorkplaces, int ConstructionNeed)
{
    public int Available => Stored - Reserved;
}
public sealed record EconomyIssue(string Id, string Text, BuildingKind? Build = null, Role? Staff = null, bool Plant = false, int? Workplace = null);
public sealed record EconomyReport(EconomyStock[] Stocks, int Meals, float NextMealSeconds, EconomyIssue[] Issues);

public sealed partial class World
{
    // A read-only snapshot: buffers and shipments are not counted as available storage.
    public EconomyReport ReadEconomy()
    {
        int Need(Resource r) => Cottages.Where(c => !c.Complete && c.Material == r).Sum(c => Math.Max(0,c.Required-c.Delivered-c.Incoming));
        var stocks = Enum.GetValues<Resource>().Select(r => new EconomyStock(r,
            r switch { Resource.Logs => Stored, Resource.Planks => Planks, Resource.Berries => Food.Berries, Resource.Vegetables => Food.Vegetables, Resource.Grain => Food.Grain, _ => Food.Bread },
            r switch { Resource.Logs => ReservedStorage, Resource.Planks => ReservedPlanks, Resource.Grain => ReservedGrain, _ => 0 },
            People.Where(p=>p.Cargo==r).Sum(p=>p.Carried),
            r switch { Resource.Logs => Cottages.Sum(c=>c.InputLogs), Resource.Planks => Cottages.Sum(c=>c.OutputPlanks),
                Resource.Vegetables => Cottages.Where(c=>c.Kind==BuildingKind.VegetableGarden).Sum(c=>c.Harvest),
                Resource.Grain => Cottages.Where(c=>c.Kind==BuildingKind.Farm).Sum(c=>c.Harvest)+Cottages.Sum(c=>c.InputGrain), Resource.Bread => Cottages.Sum(c=>c.OutputBread), _ => 0 },
            Need(r))).ToArray();
        var issues = new List<EconomyIssue>();
        bool Staffed(Role role) => People.Any(p=>p.Role==role);
        bool Planned(BuildingKind kind) => Cottages.Any(c=>c.Kind==kind);
        void Workplace(BuildingKind kind, Role role, bool needed)
        {
            if(!needed) return;
            string label = kind == BuildingKind.VegetableGarden ? "vegetable garden" : kind == BuildingKind.ForagerHut ? "forager hut" : kind.ToString().ToLowerInvariant();
            if(!Planned(kind) && !issues.Any(i=>i.Build==kind)) issues.Add(new("build-"+kind, $"Missing {label}: build one to give {role.ToString().ToLowerInvariant()}s a workplace.", Build:kind));
            else if(Cottages.Any(c => c.Kind == kind && c.Complete && !c.WorkPaused && (BelowOutputTarget(c) || c.Harvest > 0 || c.InputGrain > 0 || c.OutputBread > 0 || c.InputLogs > 0 || c.OutputPlanks > 0)) && !Staffed(role) && !issues.Any(i=>i.Id=="staff-"+role)) issues.Add(new("staff-"+role,$"No {role.ToString().ToLowerInvariant()}s assigned. Staff the completed {label}.", Staff:role));
        }
        if(!Food.Celebrating)
        {
            if(!Creative && Food.EdibleStored<Population*2)
            {
                var foodSites = Cottages.Where(c => c.Complete && ProductionOutput(c.Kind) is Resource.Berries or Resource.Vegetables or Resource.Bread).ToArray();
                if (foodSites.Length > 0 && foodSites.All(c => c.WorkPaused))
                    issues.Add(new("food-paused", "Food is below two meals and edible-food workplaces are paused. Inspect a workplace to resume it.", Workplace: foodSites[0].Id));
                else
                issues.Add(new("food-low",$"Food reserve is below two meals. Villagers eat {Population} berries/vegetables/bread per day; grain must be baked.",
                    Build: !Planned(BuildingKind.ForagerHut) && !Planned(BuildingKind.VegetableGarden) ? BuildingKind.ForagerHut : null,
                    Staff: !Planned(BuildingKind.ForagerHut) && Planned(BuildingKind.VegetableGarden) ? (HasBuilding(BuildingKind.VegetableGarden) ? Role.Farmer : Role.Builder) : !Planned(BuildingKind.ForagerHut) ? null : HasForagerHut ? Role.Forager : Role.Builder));
            }
        }
        if(!Food.Celebrating)
        {
            if (Staffed(Role.Hauler) && !Planned(BuildingKind.Stockpile))
                issues.Add(new("stockpile", "Haulers need a stockpile with a log target.", Build: BuildingKind.Stockpile));
            bool building=Cottages.Any(c=>!c.Complete);
            if(building && !Staffed(Role.Builder)) issues.Add(new("builders","Construction has no builders. Assign someone to deliver materials and build.",Staff:Role.Builder));
            bool timberNeeded=Need(Resource.Logs)>Available || (Need(Resource.Planks)>AvailablePlanks && Available<2);
            if(timberNeeded)
            {
                if(!Staffed(Role.Logger)) issues.Add(new("loggers","Construction needs more timber. Assign loggers to supply the yard.",Staff:Role.Logger));
                else if(!Trees.Any(t=>t.Logs>0 || t.Growth<1 || t.NeedsPlanting)) issues.Add(new("plant","Harvestable timber is exhausted. Mark new alders for loggers to plant.",Plant:true));
            }
            Workplace(BuildingKind.ForagerHut,Role.Forager,Staffed(Role.Forager) || HasBuilding(BuildingKind.ForagerHut));
            bool hasGarden = Planned(BuildingKind.VegetableGarden);
            Workplace(BuildingKind.Farm,Role.Farmer,HasBuilding(BuildingKind.Farm) || (Staffed(Role.Farmer) && !hasGarden) || (HasBuilding(BuildingKind.Bakery) && Food.Grain==0));
            Workplace(BuildingKind.VegetableGarden,Role.Farmer,HasBuilding(BuildingKind.VegetableGarden));
            Workplace(BuildingKind.Bakery,Role.Baker,Staffed(Role.Baker) || HasBuilding(BuildingKind.Bakery));
            Workplace(BuildingKind.Sawmill,Role.Sawyer,Staffed(Role.Sawyer) || Need(Resource.Planks)>AvailablePlanks);
        }
        return new(stocks,(Food.EdibleStored)/Population,Math.Max(0,60-Food.MealClock),issues.ToArray());
    }
}
