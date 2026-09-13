using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public static bool SupportsWorkplaceAssignment(Role role) => role is Role.Forager or Role.Farmer or Role.Baker or Role.Sawyer or Role.Fisher or Role.Quarrier or Role.Hunter or Role.Carpenter;
    public int AssignedWorkers(int siteId) => People.Count(p=>p.AssignedWorkplaceId==siteId);
    public string? WorkplaceAssignmentProblem(int personId,int? siteId)
    {
        var person=People.FirstOrDefault(p=>p.Id==personId);
        if(person==null)return "Choose a resident.";
        if(Food.Celebrating)return "Wait until supper finishes.";
        if(siteId==null)return null;
        var site=Cottages.FirstOrDefault(c=>c.Id==siteId);
        if(!SupportsWorkplaceAssignment(person.Role))return "This role works across the village.";
        if(site==null || !site.Complete || site.DemolitionRequested)return "Choose a finished workplace that is not being demolished.";
        var definition=Buildings.Get(site.Kind);
        if(definition.Worker!=person.Role || definition.Slots==0)return "Choose a workplace for this resident's current role.";
        if(People.Count(p=>p.Id!=personId && p.AssignedWorkplaceId==siteId)>=definition.Slots)return $"Fully assigned ({definition.Slots}/{definition.Slots}). Return an assigned worker to Automatic first.";
        return null;
    }
    public bool SetWorkplaceAssignment(int personId,int? siteId)
    {
        if(WorkplaceAssignmentProblem(personId,siteId)!=null)return false;
        var person=People.Single(p=>p.Id==personId);
        if(person.AssignedWorkplaceId==siteId)return true;
        person.AssignedWorkplaceId=siteId;
        if(siteId!=null)person.SharedWorker=false;
        History.Add($"{person.Name}: "+(siteId is int id?$"assigned to {Buildings.Get(Cottages.Single(c=>c.Id==id).Kind).Name} {id}":"workplace set to Automatic")+" after the current job.");
        _retry=0;return true;
    }
    // Named slots are reserved, even during meals/rest. Existing automatic work is never interrupted.
    private bool CanClaimWorkplace(Villager person,Cottage site)
    {
        if(person.AssignedWorkplaceId is int assigned)return assigned==site.Id;
        return AssignedWorkers(site.Id)+People.Count(p=>p.WorkplaceId==site.Id && p.AssignedWorkplaceId!=site.Id)<Buildings.Get(site.Kind).Slots;
    }
    private void ClearWorkplaceAssignments(int siteId)
    {
        foreach(var person in People.Where(p=>p.AssignedWorkplaceId==siteId))
        {person.AssignedWorkplaceId=null;History.Add($"{person.Name}: workplace removed; returned to Automatic.");}
    }
    public string ReadWorkplaceAssignment(Villager person)
    {
        if(person.SharedWorker)return "Shared worker — takes available work across professions.";
        if(person.AssignedWorkplaceId is not int id)return SupportsWorkplaceAssignment(person.Role)?"Automatic — chooses available workplaces.":"Automatic — works across the village.";
        var site=Cottages.Single(c=>c.Id==id);
        string name=$"{Buildings.Get(site.Kind).Name} {id} ({site.Cell.X}, {site.Cell.Z})";
        if(person.WorkplaceId!=null && person.WorkplaceId!=id)return name+" — after the current job and delivery.";
        if(person.WorkplaceId==id)return name+" — "+person.Status;
        if(!FreeStation(site))return name+" — waiting for the current worker to finish.";
        return name+" — "+ReadWorkplace(site).State+". Works only here; meals and breaks continue.";
    }
    private void ValidateWorkplaceAssignments()
    {
        if(People.Any(p=>p.SharedWorker && (!SharedWork || p.AssignedWorkplaceId!=null)))throw new InvalidOperationException("Invalid shared worker");
        foreach(var person in People.Where(p=>p.AssignedWorkplaceId!=null))
        {
            var site=Cottages.FirstOrDefault(c=>c.Id==person.AssignedWorkplaceId);
            if(site==null || !site.Complete || site.DemolitionRequested || !SupportsWorkplaceAssignment(person.Role) || Buildings.Get(site.Kind).Worker!=person.Role || Buildings.Get(site.Kind).Slots==0)
                throw new InvalidOperationException("Invalid assigned workplace");
        }
        foreach(var site in Cottages)
            if(AssignedWorkers(site.Id)>Buildings.Get(site.Kind).Slots)throw new InvalidOperationException("Workplace assignments exceed capacity");
    }
}
