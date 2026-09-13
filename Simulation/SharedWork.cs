using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public bool SharedWork { get; private set; }
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
        Assign(person.Id,role);return SetWorkplaceAssignment(person.Id,siteId);
    }
    public bool ReleaseDedicatedWorker(int siteId)
    {
        if(!SharedWork || Food.Celebrating)return false;
        var person=People.LastOrDefault(p=>p.AssignedWorkplaceId==siteId);
        if(person==null)return false;
        Assign(person.Id,Role.Unassigned);return true;
    }
    public static World NewSharedWorkExperiment()
    {
        var world=NewLocalSupplyExperiment();world.SharedWork=true;
        foreach(var person in world.People)world.Assign(person.Id,Role.Unassigned);
        return world;
    }
    private static bool FoodRole(Role role)=>role is Role.Forager or Role.Farmer or Role.Baker or Role.Fisher or Role.Hunter;
    private void ClaimSharedWork(Villager person)
    {
        person.Role=Role.Unassigned;
        int capacity=Math.Max(1,(Population+2)/3);
        bool Try(Role role)
        {
            if(People.Count(p=>p.Id!=person.Id && p.Role==role)>=capacity)return false;
            person.Role=role;ClaimProfessionWork(person);
            if(person.Task!=Work.Waiting)return true;
            person.Role=Role.Unassigned;return false;
        }
        bool FoodWork()
        {
            if(People.Count(p=>FoodRole(p.Role))>=Math.Max(1,(Population+1)/2))return false;
            foreach(var role in new[]{Role.Baker,Role.Forager,Role.Farmer,Role.Fisher,Role.Hunter}
                .OrderBy(r=>People.Count(p=>p.Role==r)))if(Try(role))return true;
            return false;
        }
        if(EdibleStored<Population*3 && FoodWork())return;
        if(Try(Role.Builder))return;
        // Keep a modest working reserve; shared labor must not strip the map while idle.
        int logsNeeded=8+Cottages.Where(c=>!c.Complete && c.Material==Resource.Logs).Sum(c=>c.Remaining(Resource.Logs));
        if((Stored<Math.Min(24,logsNeeded) || Trees.Any(t=>t.ClearRequested || t.NeedsPlanting)) && Try(Role.Logger))return;
        if(ClaimWelcomeDelivery(person)){person.Role=Role.Hauler;return;}
        if(Try(Role.Carpenter) || Try(Role.Sawyer) || Try(Role.Quarrier) || Try(Role.Hauler) || FoodWork())return;
        person.Status="Shared worker — waiting for available work";
    }
}
