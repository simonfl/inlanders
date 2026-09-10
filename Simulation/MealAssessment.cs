using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed record MealAssessment(int ResidentsWithHistory, int Residents, int Closed, int Missed, int Skipped,
    int Eaten, int NonDominant, int Delivered)
{
    public bool Reliable => ResidentsWithHistory==Residents && Missed==0 && Skipped==0;
    public bool Varied => Eaten>0 && NonDominant>=(Eaten+3)/4;
    public bool FreshSupply => Delivered>=Math.Max(Closed+Skipped,Eaten);
    public string Summary => $"Residents with two closed meal requests: {ResidentsWithHistory}/{Residents}\nMissed: {Missed} · skipped: {Skipped}\nActually eaten: {Eaten} · outside dominant food: {NonDominant}\nFresh deliveries: {Delivered}/{Math.Max(Closed+Skipped,Eaten)}";
    public string? Problem => Missed>0 || Skipped>0 ? "Residents missed meals. Inspect their trips: shorten travel to recreation or homes, or supply a pantry near distant destinations. The last three minutes must have no missed or skipped meals." :
        ResidentsWithHistory<Residents ? "Waiting for two closed meal requests per resident; a pending deadline does not prove reliable service." :
        !Varied ? "At least a quarter of food actually eaten must be outside the dominant food." :
        !FreshSupply ? "Fresh deliveries must cover actual eating and closed or skipped meal demand." : null;
}

public sealed partial class World
{
    // Closed service, actual eating and fresh production are deliberately separate observations.
    public MealAssessment ReadMealAssessment(float since=0)
    {
        float cutoff=Math.Max(since,Food.Time-FoodFlowWindow);
        var outcomes=Food.MealOutcomes.Where(m=>m.Time>cutoff).ToArray();
        var eaten=Food.MealConsumptions.Where(m=>m.Time>cutoff).ToArray();
        int served=People.Count(p=>outcomes.Count(m=>m.Person==p.Id && !m.Skipped)>=2);
        int dominant=eaten.GroupBy(m=>m.Kind).Select(g=>g.Count()).DefaultIfEmpty(0).Max();
        int deliveries=RecentFood.Where(e=>e.Time>cutoff).Sum(e=>e.Berries+e.Vegetables+e.Bread+e.Fish+e.Game);
        return new(served,Population,outcomes.Count(m=>!m.Skipped),outcomes.Count(m=>!m.Skipped && !m.Timely),
            outcomes.Count(m=>m.Skipped),eaten.Length,eaten.Length-dominant,deliveries);
    }
}
