using Inlanders.Simulation;
using System.Text.Json;
static class TransformationChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        const string folder="artifacts/transformation";Directory.CreateDirectory(folder);var rows=new List<object>();
        foreach(string arm in new[]{"unchanged","crossing","gardens","mixed","grain","recovered"})
        {
            var w=World.NewTransformationHamlet();Check(w.Population==12 && w.Housed==12,"Inhabited start missing");
            void Build(BuildingKind kind,Cell near){var p=ReviewPlacement.Find(w,kind,near,(c,r)=>true,"hamlet "+arm)!;Check(w.Place(p.Actual,p.Rotation,kind)!=null,"Plan rejected");}
            if(arm is "crossing" or "recovered")Check(w.Place(new(2,0),0,BuildingKind.Bridge)!=null,"Crossing rejected");
            if(arm=="gardens")foreach(var c in new[]{new Cell(4,3),new(4,-9),new(0,-9)})Build(BuildingKind.VegetableGarden,c);
            if(arm=="mixed"){Build(BuildingKind.FishingDock,new(8,5));Build(BuildingKind.VegetableGarden,new(4,3));}
            if(arm=="grain"){Build(BuildingKind.Farm,new(2,-8));Build(BuildingKind.Bakery,new(4,3));}
            int addedCost=w.Cottages.Skip(9).Sum(c=>c.Required);if(arm is "gardens" or "mixed" or "grain")Check(addedCost==12,"Food comparison investment differs");
            double hungry=0,walking=0;var trace=new List<string>{$"0s: {arm}; twelve residents; extra food investment is twelve logs in gardens/mixed/grain"};
            trace.AddRange(w.Cottages.Skip(9).Select(c=>$"Planned {c.Kind} at {c.Cell}, facing {c.Rotation}"));
            for(int i=0;i<12000;i++)
            {
                w.Tick(.1f);if(i>=6000){hungry+=w.People.Count(p=>!p.Fed)*.1;walking+=w.People.Count(p=>p.Task is Work.ToMealSupply or Work.ToMealSeat)*.1;}
                if(i==5999){string json=w.SaveJson();var clone=World.LoadJson(json);for(int j=0;j<10;j++){w.Tick(.1f);clone.Tick(.1f);}Check(w.SaveJson()==clone.SaveJson(),"Continuation differs");trace.Add($"600s: {w.EdibleStored} stored; {w.People.Count(p=>!p.Fed)} hungry; inspect before further investment");}
                if(arm=="recovered" && i==6000 && w.EdibleStored<w.Population*3){trace.Add($"{w.Food.Time:0}s: reserve below three meals after crossing; add a nearby garden with remaining timber");Build(BuildingKind.VegetableGarden,new(4,3));}
                if(i%500==0)w.Validate();
            }
            w.SaveFile(folder+"/"+arm+".json");rows.Add(new{arm,hungry,walking,food=w.EdibleStored,buildings=w.Cottages.Count,addedTimber=w.Cottages.Skip(9).Sum(c=>c.Required),trace});
        }
        string report=JsonSerializer.Serialize(rows,new JsonSerializerOptions{WriteIndented=true});File.WriteAllText(folder+"/report.json",report);Console.WriteLine(report);
        File.WriteAllText(folder+"/manifest.json",JsonSerializer.Serialize(new{assembly=typeof(TransformationChecks).Assembly.ManifestModule.ModuleVersionId,utc=DateTime.UtcNow,scope="six current hamlet arms; one conditional recovery",humanPlay=false},new JsonSerializerOptions{WriteIndented=true}));
        Console.WriteLine("PASS: six ordinary hamlet approaches, fixed population, equal added food investment and exact active saves. Scripted plans, not human enjoyment.");
    }
}
