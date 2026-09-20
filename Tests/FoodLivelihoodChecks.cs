using Inlanders.Simulation;
using System.Text.Json;
static class FoodLivelihoodChecks
{
    public static void Run(string? chosen=null)
    {
        Directory.CreateDirectory("artifacts/food-livelihood");var rows=new List<object>();
        foreach(string arm in new[]{"grain","grain-close","garden","shore","grain-grow","garden-grow","shore-grow"})
        {
            if(chosen!=null && arm!=chosen)continue;
            var w=World.NewRiverFarmstead();
            void Build(BuildingKind kind,Cell at){var p=ReviewPlacement.Find(w,kind,at,(c,r)=>true,"livelihood "+arm)!;if(w.Place(p.Actual,p.Rotation,kind)==null)throw new Exception("Rejected food placement");}
            foreach(var cell in new[]{new Cell(-3,6),new(1,0),new(1,6)})Build(BuildingKind.Cottage,cell);
            if(arm.StartsWith("grain")){Build(BuildingKind.Farm,new(3,3));Build(BuildingKind.Bakery,arm=="grain-close"?new(5,4):new(0,-3));}
            else if(arm.StartsWith("garden")){Build(BuildingKind.VegetableGarden,new(3,3));Build(BuildingKind.VegetableGarden,new(4,7));}
            else Build(BuildingKind.FishingDock,new(8,3));
            double quiet=0,work=0,hungry=0,mealWalk=0;
            for(int i=0;i<(arm.EndsWith("grow")?36000:18000);i++)
            {
                if(arm.EndsWith("grow") && i==6000)foreach(var cell in new[]{new Cell(-6,6),new(-6,9),new(-2,10),new(4,-5)})Build(BuildingKind.Cottage,cell);
                if(arm.EndsWith("grow") && i>6000 && w.Population<16 && w.InvitationProblem()==null)w.InviteNewcomers();
                w.Tick(.1f);if(i%1000==0)w.Validate();
                if(i<(arm.EndsWith("grow")?24000:6000))continue;
                quiet+=w.People.Count(p=>p.Task==Work.Waiting)*.1;
                work+=w.People.Count(p=>p.Role is Role.Farmer or Role.Baker or Role.Fisher)*.1;
                hungry+=w.People.Count(p=>!p.Fed)*.1;
                mealWalk+=w.People.Count(p=>p.Task is Work.ToMealSupply or Work.ToMealSeat)*.1;
            }
            if(arm=="grain-grow" && (w.Population!=16 || hungry>0))throw new Exception("Grain investment lost growth capacity");
            if(!arm.EndsWith("grow") && hungry>0)throw new Exception("Small livelihood stopped feeding residents");
            w.SaveFile($"artifacts/food-livelihood/{arm}.json");
            var producers=w.Cottages.Where(c=>World.ProductionOutput(c.Kind)!=null).ToArray();
            rows.Add(new {arm,w.Population,quiet,work,hungry,mealWalk,food=w.EdibleStored,land=producers.Sum(c=>World.Footprint(c.Cell,c.Rotation,c.Kind).Count()),logs=producers.Sum(c=>c.Required)});
        }
        string json=JsonSerializer.Serialize(rows,new JsonSerializerOptions{WriteIndented=true});Console.WriteLine(json);
        string stem=chosen==null?"report":"report-"+chosen;
        File.WriteAllText($"artifacts/food-livelihood/{stem}.json",json);
        File.WriteAllText($"artifacts/food-livelihood/{stem}.manifest.json",JsonSerializer.Serialize(new {assembly=typeof(FoodLivelihoodChecks).Assembly.ManifestModule.ModuleVersionId,utc=DateTime.UtcNow,scope=chosen??"all seven arms",producerExpansion=false},new JsonSerializerOptions{WriteIndented=true}));
    }
}
