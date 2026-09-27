using Inlanders.Simulation;
using System.Text.Json;
static class MatureCultivationChecks
{
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/mature-cultivation");var report=new List<object>();
        foreach(bool relaxed in new[]{false,true})foreach(bool split in new[]{false,true})foreach(bool remote in split?new[]{false}:new[]{false,true})
        {
            var w=World.NewPlayerFounded(relaxed);foreach(var c in new[]{new Cell(-3,7),new(1,7),new(-3,11),new(1,11)})if(w.Place(c)==null)throw new Exception("Mature home rejected");
            var field=w.Place(split?new Cell(4,3):remote?new Cell(1,-5):new Cell(4,7),0,BuildingKind.VegetableField,split?3:6)??throw new Exception("Mature strip rejected");
            if(split && w.Place(new(4,7),0,BuildingKind.VegetableField,3)==null)throw new Exception("Second strip rejected");
            int hungry=0,quiet=0;bool revised=false;var samples=new List<object>();
            for(int i=0;i<18000;i++)
            {
                w.Tick(.1f);hungry+=w.People.Count(p=>!p.Fed);if(!w.FoodWorkNeeded && !field.EstablishmentPending)quiet++;
                if(!revised && w.Food.Time>900 && field.Harvest==0)
                {w.SetWorkplacePaused(field.Id,true);revised=w.ReshapePlot(field.Id,field.Depth-1);w.SetWorkplacePaused(field.Id,false);}
                if(i%600==0){w.Validate();samples.Add(new{time=w.Food.Time,food=w.EdibleStored,grown=w.Food.GrownVegetables,eaten=w.Food.EatenVegetables,housed=w.Housed,foodNeeded=w.FoodWorkNeeded,revised,working=w.People.Count(p=>p.Task!=Work.Waiting),home=w.People.Count(w.AvailableAtHome)});}
            }
            w.Validate();if(w.Housed!=8 || !revised || w.Food.GrownVegetables<=24)throw new Exception("Mature livelihood/revision never realized");
            w.SaveFile($"artifacts/mature-cultivation/{relaxed}-{split}-{remote}.json");report.Add(new{relaxed,split,remote,seconds=w.Food.Time,hungryTicks=hungry,quietTicks=quiet,food=w.EdibleStored,grown=w.Food.GrownVegetables,eaten=w.Food.EatenVegetables,revised,samples});
            Console.WriteLine($"Mature cultivation relaxed={relaxed}, split={split}, remote={remote}: food={w.EdibleStored}, grown={w.Food.GrownVegetables}, hungryTicks={hungry}, quietTicks={quiet}, revised={revised}. Scripted comparison, not preference.");
        }
        File.WriteAllText("artifacts/mature-cultivation/report.json",JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true}));
    }
}
