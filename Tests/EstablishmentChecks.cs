using Inlanders.Simulation;
using System.Text.Json;
static class EstablishmentChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/establishment");var rows=new List<object>();
        foreach(bool relaxed in new[]{false,true})foreach(string livelihood in new[]{"gardens","bread","fish"})foreach(bool control in new[]{true,false})
        {
            var w=World.NewPlayerFounded(relaxed,control);
            foreach(var at in new[]{new Cell(-3,7),new(1,7),new(-3,11),new(1,11)})Check(w.Place(at,0,BuildingKind.Cottage)!=null,"Home rejected");
            if(livelihood=="gardens")foreach(var at in new[]{new Cell(4,3),new(4,7)})Check(w.Place(at,0,BuildingKind.VegetableGarden)!=null,"Garden rejected");
            if(livelihood=="bread"){Check(w.Place(new(1,-5),0,BuildingKind.Farm)!=null,"Field rejected");Check(w.Place(new(5,-3),0,BuildingKind.Bakery)!=null,"Oven rejected");}
            if(livelihood=="fish")Check(w.Place(new(8,8),1,BuildingKind.FishingDock)!=null,"Landing rejected");
            int Produced()=>livelihood=="gardens"?w.Food.GrownVegetables:livelihood=="bread"?w.Food.BakedBread:w.Food.CaughtFish;
            int Eaten()=>livelihood=="gardens"?w.Food.EatenVegetables:livelihood=="bread"?w.Food.EatenBread:w.Food.EatenFish;
            var sites=w.Cottages.Where(c=>World.ProductionOutput(c.Kind)!=null).ToArray();
            var siteEvents=sites.ToDictionary(c=>c.Id,c=>new Dictionary<string,float>());
            float? completed=null,work=null,output=null,meal=null;int hungry=0;bool savedActive=false;
            for(int i=0;i<12000;i++)
            {
                w.Tick(.1f);hungry+=w.People.Count(p=>!p.Fed);
                foreach(var site in sites)
                {
                    var events=siteEvents[site.Id];
                    if(site.Complete)events.TryAdd("completed",w.Food.Time);
                    if(w.People.Any(p=>p.WorkplaceId==site.Id))events.TryAdd("firstWork",w.Food.Time);
                    if(site.Harvest>0 || site.OutputBread>0 || site.Boat?.Fish>0)events.TryAdd("firstOutput",w.Food.Time);
                    if(!control && !site.EstablishmentPending)events.TryAdd("cycleClosed",w.Food.Time);
                }
                if(completed==null && sites.All(c=>c.Complete))completed=w.Food.Time;
                if(work==null && w.People.Any(p=>p.WorkplaceId is int id && sites.Any(c=>c.Id==id)))work=w.Food.Time;
                if(output==null && Produced()>0){output=w.Food.Time;w.SaveFile($"artifacts/establishment/{relaxed}-{livelihood}-{control}-output.json");}
                if(meal==null && Eaten()>0)meal=w.Food.Time;
                if(!savedActive && sites.Any(c=>c.EstablishmentPending) && work!=null)
                {
                    var copy=World.LoadJson(w.SaveJson());Check(copy.SaveJson()==w.SaveJson(),"Active first cycle lost in save");var twin=World.LoadJson(copy.SaveJson());for(int n=0;n<100;n++){copy.Tick(.1f);twin.Tick(.1f);}Check(copy.SaveJson()==twin.SaveJson(),"Active continuation differs");savedActive=true;
                }
                if(i%100==0)w.Validate();
            }
            Check(work!=null && output!=null && meal!=null && hungry==0,"Livelihood did not feed safely");
            if(!control)Check(work<300 && output<450 && savedActive && sites.All(c=>!c.EstablishmentPending),"First productive cycle delayed or never closed");
            rows.Add(new{relaxed,livelihood,control,allWorkplacesBuilt=completed,firstWork=work,firstOutput=output,firstMeal=meal,siteEvents,hungry,food=w.EdibleStored});
            Console.WriteLine($"PASS establishment {relaxed}/{livelihood}/control={control}: built {completed:0.0}s, work {work:0.0}s, output {output:0.0}s, meal {meal:0.0}s");
        }
        // A completed first cycle cannot be manufactured again by relocating or loading.
        var move=World.NewPlayerFounded(true);var garden=move.Place(new(4,3),0,BuildingKind.VegetableGarden)!;
        move.SetWorkplacePaused(garden.Id,true);for(int i=0;i<300;i++)move.Tick(.1f);
        Check(garden.EstablishmentPending && move.Food.GrownVegetables==0,"Paused first cycle started");
        Check(move.MoveBuilding(garden.Id,new(4,7),0),"Paused first garden did not move");move.SetWorkplacePaused(garden.Id,false);
        for(int i=0;i<3000 && garden.EstablishmentPending;i++)move.Tick(.1f);
        Check(!garden.EstablishmentPending && move.Food.GrownVegetables==8,"First crop not bounded");move.SetWorkplacePaused(garden.Id,true);
        Check(move.MoveBuilding(garden.Id,new(4,3),0),"Completed first garden did not move");
        move=World.LoadJson(move.SaveJson());Check(!move.Cottages.Single(c=>c.Id==garden.Id).EstablishmentPending,"Move/load restarted establishment");
        File.WriteAllText("artifacts/establishment/timeline.json",JsonSerializer.Serialize(rows,new JsonSerializerOptions{WriteIndented=true}));
    }
}
