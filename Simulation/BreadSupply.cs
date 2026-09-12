using System;
using System.Linq;

namespace Inlanders.Simulation;

public sealed partial class World
{
    public string BreadSupplySummary()
    {
        float cutoff=Math.Max(0,Food.Time-FoodFlowWindow);
        int delivered=RecentFood.Where(e=>e.Time>cutoff).Sum(e=>e.Bread);
        int eaten=Food.MealConsumptions.Count(e=>e.Time>cutoff && e.Kind==Resource.Bread);
        int central=CentralFoodAvailable(Resource.Bread);
        int local=StoredFood(Resource.Bread)-Food.Bread;
        string text=$"Last {Math.Min(Food.Time,FoodFlowWindow):0}s: {delivered} bread delivered · {eaten} eaten in meals\n"+
            "Deliveries to all pantries; transfers excluded. Delivered bread may already be eaten.\n\n"+
            $"Central: {Food.Bread} stored · {FoodReservedAt(null,Resource.Bread)} reserved · {central} available\n"+
            $"Local pantries: {local} stored\n"+
            $"At bakeries: {Cottages.Sum(c=>c.OutputBread)} · carried: {People.Where(p=>p.Cargo==Resource.Bread).Sum(p=>p.Carried)}\n";
        if(!Creative && !Food.SupperComplete && !Food.Celebrating)
            text+=$"Supper needs {SupperCost} available centrally; {Math.Max(0,SupperCost-central)} more needed.\n";
        text+="\nMeals use bread too. Inspect baker staffing, grain, targets and travel; add baking capacity if needed.\n";
        if(local>0)
            text+="Return local surplus: lower its pantry target and assign a hauler. Haulers return surplus food, not bread specifically; meals can claim it first.";
        else text+="Supper also needs homes and clear gathering space.";
        return text;
    }
}
