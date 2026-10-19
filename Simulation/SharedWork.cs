using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public bool SharedWork { get; private set; }
    public void UseAutomaticVillageWork()
    {
        if(!SharedWork)return;
        foreach(var person in People)
        {
            person.AssignedWorkplaceId=null;person.SharedWorker=true;
            if(person.Task==Work.Waiting)person.Role=Role.Unassigned;
        }
        _retry=0;
    }

    public Villager? SharedStaffCandidate(int siteId)=>People.Where(p=>p.SharedWorker)
        .OrderByDescending(p=>p.WorkplaceId==siteId).ThenByDescending(p=>p.Task==Work.Waiting).ThenBy(p=>p.Id).FirstOrDefault();
    public string? DedicateWorkerProblem(int siteId)
    {
        if(!SharedWork)return "Shared staffing is only available in the neighborhood experiment.";
        if(Food.Celebrating)return "Wait until supper finishes.";
        var site=Cottages.FirstOrDefault(c=>c.Id==siteId);
        if(site==null || !site.Complete || site.DemolitionRequested)return "Choose a finished workplace.";
        var definition=Buildings.Get(site.Kind);
        if(definition.Worker is not Role role || !SupportsWorkplaceAssignment(role) || definition.Slots==0)return "This building uses visiting workers.";
        if(AssignedWorkers(siteId)>=definition.Slots)return "All slots are dedicated. Release a worker to make room.";
        if(SharedStaffCandidate(siteId)==null)return "No shared workers remain. Release a dedicated worker first.";
        return null;
    }
    public bool DedicateWorker(int siteId)
    {
        if(DedicateWorkerProblem(siteId)!=null)return false;
        var person=SharedStaffCandidate(siteId)!;
        var role=Buildings.Get(Cottages.Single(c=>c.Id==siteId).Kind).Worker!.Value;
        // Keeping a worker already in this profession changes future staffing only.
        // In particular, do not recall a boat or return a half-finished delivery.
        if(person.Role!=role)Assign(person.Id,role);
        else person.SharedWorker=false;
        return SetWorkplaceAssignment(person.Id,siteId);
    }
    public bool ReleaseDedicatedWorker(int siteId)
    {
        if(!SharedWork || Food.Celebrating)return false;
        var person=People.LastOrDefault(p=>p.AssignedWorkplaceId==siteId);
        if(person==null)return false;
        person.AssignedWorkplaceId=null;person.SharedWorker=true;
        History.Add(person.Name+": returns to shared work after the current job and delivery.");
        _retry=0;return true;
    }
    public static World NewSharedWorkExperiment()
    {
        var world=NewLocalSupplyExperiment();world.SharedWork=true;
        foreach(var person in world.People)world.Assign(person.Id,Role.Unassigned);
        return world;
    }
    private static bool FoodRole(Role role)=>role is Role.Forager or Role.Farmer or Role.Baker or Role.Fisher or Role.Hunter;
    public bool ProvisionedLife => Founding?.ProvisionedLife==true;
    public bool FoodWorkNeeded => !ProvisionedLife || EdibleStored<Population*4 || People.Any(p=>!p.Fed);
    private bool SharedTimberNeeded => Stored<Math.Min(24,8+Cottages.Where(c=>!c.Complete && !c.ConstructionPaused && c.Material==Resource.Logs).Sum(c=>c.Remaining(Resource.Logs)));
    // Development-only observations of the actual dispatch path; never saved or used to choose work.
    public bool TraceSharedWork { get; set; }
    public System.Collections.Generic.Dictionary<int,string[]> WorkDecisions { get; }=new();
    public string WorkforceSummary()
    {
        int available=People.Count(p=>p.Task==Work.Waiting);
        int personal=People.Count(p=>p.Task is Work.ToRest or Work.Resting or Work.ToLeisure or Work.Leisure or Work.ToMealSupply or Work.ToMealSeat or Work.EatingMeal or Work.ReturnMeal);
        return $"{Population-available-personal} working or travelling · {personal} eating or resting · {available} available\n"+
            (!FoodWorkNeeded?$"Food reserves sufficient: {EdibleStored} portions. Food work resumes below {Population*4} or when someone needs a meal.":"Food production is needed. Residents take reachable work with free positions and supplies.")+"\nInspect an unfinished plan or workplace for its current progress or blocker.";
    }
    private void ClaimSharedWork(Villager person)
    {
        var decisions=TraceSharedWork?new System.Collections.Generic.List<string>():null;
        if(decisions!=null)WorkDecisions[person.Id]=System.Array.Empty<string>();
        void Record(string reason){if(decisions!=null){decisions.Add(reason);WorkDecisions[person.Id]=decisions.ToArray();}}
        person.Role=Role.Unassigned;
        int capacity=Math.Max(1,(Population+2)/3);
        bool Try(Role role)
        {
            if(People.Count(p=>p.Id!=person.Id && p.Role==role)>=capacity){Record(role+": profession capacity claimed");return false;}
            person.Role=role;ClaimProfessionWork(person);
            if(person.Task!=Work.Waiting){Record(role+": "+person.Status);return true;}
            Record(role+": "+person.Status);
            person.Role=Role.Unassigned;return false;
        }
        bool FoodWork()
        {
            if(!FoodWorkNeeded && !Cottages.Any(c=>c.Complete && !c.WorkPaused && !c.DemolitionRequested && EstablishmentWork(c))){Record("Food: reserve sufficient");return false;}
            if(People.Count(p=>FoodRole(p.Role))>=Math.Max(1,(Population+1)/2)){Record("Food: worker capacity claimed");return false;}
            foreach(var role in new[]{Role.Baker,Role.Forager,Role.Farmer,Role.Fisher,Role.Hunter}
                .OrderBy(r=>People.Count(p=>p.Role==r)))if(Try(role))return true;
            return false;
        }
        if(EdibleStored<Population*3 && FoodWork())return;
        if(Try(Role.Builder))return;
        // Keep a modest working reserve; shared labor must not strip the map while idle.
        if((SharedTimberNeeded || Trees.Any(t=>t.ClearRequested || t.NeedsPlanting) || ManagedWoodland.Any(GroveNeedsPlanting)) && Try(Role.Logger))return;
        if(ClaimWelcomeDelivery(person)){person.Role=Role.Hauler;return;}
        int plankNeed=8+Cottages.Where(c=>!c.Complete && !c.ConstructionPaused && c.Material==Resource.Planks).Sum(c=>c.Remaining(Resource.Planks))
            +Cottages.Where(c=>c.ImprovementRequested).Sum(c=>Math.Max(0,ComfortCost(c)-c.ImprovementPlanks-ComfortIncoming(c)));
        int stoneNeed=6+Cottages.Where(c=>!c.Complete && !c.ConstructionPaused).Sum(c=>c.Remaining(Resource.Stone));
        if(Try(Role.Carpenter) || ((!ProvisionedLife || PendingPlanks<plankNeed) && Try(Role.Sawyer)) ||
            ((!ProvisionedLife || ProductionCommitted(Resource.Stone)<stoneNeed) && Try(Role.Quarrier)) || Try(Role.Hauler) || FoodWork())return;
        person.Status="Shared worker — waiting for available work";
        WaitNearHome(person);
    }
}
