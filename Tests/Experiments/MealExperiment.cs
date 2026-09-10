using System.Text.Json;

namespace MealPrototype;

// Deliberately test-only: discrete 0.1s service/accounting experiment, not a second
// live World scheduler. Route durations are supplied by fixtures.
public enum Phase { Waiting, Walking, Eating, Returning, Satisfied }
public sealed class Request
{
    public int Id { get; set; }
    public int Due { get; set; }
    public int Kind { get; set; } = -1;
    public Phase Phase { get; set; }
    public int Progress { get; set; }
    public bool Closed { get; set; }
}
public sealed class Resident
{
    public int Id { get; set; }
    public int NextDue { get; set; }
    public int RouteTicks { get; set; }
    public int BusyUntil { get; set; }
    public int HoldUntil { get; set; }
    public int Travel { get; set; }
    public bool Fed { get; set; } = true;
    public Request? Request { get; set; }
}
public sealed record Outcome(int Request, int Resident, int Deadline, bool Timely);
public sealed record Consumption(int Request, int Resident, int Time, int Kind, bool Late);
public sealed record Skipped(int Resident,int Due,int Time);
public sealed class Experiment
{
    public int Time { get; set; }
    public int NextId { get; set; } = 1;
    public int EatTicks { get; set; } = 40;
    public int[] Stock { get; set; } = new int[5];
    public int[] Supplied { get; set; } = new int[5];
    public int[] Eaten { get; set; } = new int[5];
    public List<Resident> People { get; set; } = new();
    public List<Outcome> Outcomes { get; set; } = new();
    public List<Consumption> Meals { get; set; } = new();
    public List<Skipped> Skipped { get; set; } = new();
    public float Hunger => People.Count==0?0:People.Count(p=>!p.Fed)/(float)People.Count;
    public void Supply(int kind,int count) { Stock[kind]+=count; Supplied[kind]+=count; }
    public int Available(int kind) => Stock[kind]-People.Count(p=>p.Request is { Phase:Phase.Walking } r && r.Kind==kind);
    public string Save() => JsonSerializer.Serialize(this);
    public static Experiment Load(string data) { var e=JsonSerializer.Deserialize<Experiment>(data)!; e.Validate(); return e; }
    void Retire(Resident p)
    {
        p.NextDue=p.Request!.Due+600;
        while(p.NextDue<Time) { Skipped.Add(new(p.Id,p.NextDue,Time)); p.NextDue+=600; }
        p.Request=null;
    }
    public void Interrupt(int id)
    {
        var r=People[id].Request; if(r==null || r.Phase==Phase.Satisfied) return;
        if(r.Phase==Phase.Eating) { r.Phase=Phase.Returning; r.Progress=0; }
        else if(r.Phase==Phase.Walking) { r.Phase=Phase.Waiting; r.Kind=-1; r.Progress=0; }
    }
    public void Step(int count=1)
    {
        for(int t=0;t<count;t++,Time++)
        {
            // Rotate ties once per demand round. Staggering normally avoids them.
            int start=People.Count==0?0:(Time/600)%People.Count;
            foreach(var p in People.OrderBy(p=>(p.Id-start+People.Count)%People.Count))
            {
                if(p.Request==null && Time>=p.NextDue)
                    p.Request=new Request { Id=NextId++, Due=p.NextDue };
                var r=p.Request; if(r==null) continue;
                if(Time>=r.Due+600 && !r.Closed)
                {
                    r.Closed=true;
                    bool timely=r.Phase==Phase.Satisfied;
                    Outcomes.Add(new(r.Id,p.Id,r.Due+600,timely)); p.Fed=timely;
                    if(r.Phase is Phase.Waiting or Phase.Walking or Phase.Satisfied) { Retire(p); continue; }
                }
                if(r.Phase==Phase.Eating && Time>=r.Due+900)
                { r.Phase=Phase.Returning; r.Progress=0; }
                if(Time<p.HoldUntil) continue;
                switch(r.Phase)
                {
                    case Phase.Waiting:
                        if(Time<p.BusyUntil) break;
                        int kind=Enumerable.Range(0,5).Where(k=>Available(k)>0)
                            .OrderBy(k=>Meals.Count(m=>m.Kind==k && m.Time>Time-1800)).ThenBy(k=>k).DefaultIfEmpty(-1).First();
                        if(kind<0) break;
                        r.Kind=kind; r.Phase=Phase.Walking; r.Progress=0; break;
                    case Phase.Walking:
                        if(p.RouteTicks>0) p.Travel++;
                        if(++r.Progress<p.RouteTicks) break;
                        Stock[r.Kind]--; r.Phase=Phase.Eating; r.Progress=0; break;
                    case Phase.Eating:
                        if(++r.Progress<EatTicks) break;
                        Eaten[r.Kind]++; Meals.Add(new(r.Id,p.Id,Time,r.Kind,r.Closed)); p.Fed=true;
                        r.Phase=Phase.Satisfied;
                        if(r.Closed) Retire(p);
                        break;
                    case Phase.Returning:
                        if(p.RouteTicks>0) p.Travel++;
                        if(++r.Progress<p.RouteTicks) break;
                        Stock[r.Kind]++; r.Kind=-1; r.Progress=0;
                        if(r.Closed) Retire(p); else r.Phase=Phase.Waiting;
                        break;
                }
            }
            Validate();
        }
    }
    public void Validate()
    {
        if(People.Select(p=>p.Id).Where((id,index)=>id!=index).Any() || Outcomes.Select(o=>o.Request).Distinct().Count()!=Outcomes.Count || Meals.Select(m=>m.Request).Distinct().Count()!=Meals.Count)
            throw new Exception("Duplicate identity or accounting event");
        for(int k=0;k<5;k++)
        {
            int cargo=People.Count(p=>p.Request is { Phase:Phase.Eating or Phase.Returning } r && r.Kind==k);
            if(Available(k)<0 || Stock[k]+cargo+Eaten[k]!=Supplied[k]) throw new Exception("Meal stock/claim conservation");
        }
        if(People.Any(p=>p.Request is {} r && (r.Id<=0 || r.Id>=NextId || r.Due>Time || r.Kind < -1 || r.Kind>4 || r.Progress<0))) throw new Exception("Invalid request");
    }
}
