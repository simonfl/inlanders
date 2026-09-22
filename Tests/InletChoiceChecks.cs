using Inlanders.Simulation;
static class InletChoiceChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/inlet-choice");
        foreach(bool relaxed in new[]{false,true})foreach(string choice in new[]{"keep","cross","reshape"})
        {
            var profile=new HamletProfile(relaxed,true,false,true);var w=profile.Create();
            var field=w.Cottages.First(c=>c.Kind==BuildingKind.VegetableField && c.Cell.X==5);
            var home=w.Cottages.Single(c=>c.Cell==new Cell(5,7));
            Check(w.Population==12 && w.Housed==12 && w.Stored==12 && w.Planks==4 && w.EdibleStored==72,"Opening inventory");
            Check(w.Cottages.Where(c=>World.IsVegetablePlot(c.Kind)).Sum(c=>World.VegetableYield(c.Kind))==48,"Matched crop capacity");
            if(choice=="cross")Check(w.Place(new(5,0),0,BuildingKind.Bridge)!=null,"Crossing: "+w.PlacementProblem(new(5,0),0,BuildingKind.Bridge));
            if(choice=="reshape")
            {
                Check(w.MoveBuilding(home.Id,new(1,-2),0),"Home move: "+w.RelocationProblem(home.Id,new(1,-2),0));
                Check(w.SetWorkplacePaused(field.Id,true),"Pause");
                for(int wait=0;wait<600 && w.RelocationProblem(field.Id,new(5,9),0)!=null;wait++)w.Tick(.1f);
                Check(w.MoveBuilding(field.Id,new(5,9),0),"Field move: "+w.RelocationProblem(field.Id,new(5,9),0));
                Check(!field.Planted && field.Growth==0,"Growing crop should need resowing");
                Check(w.SetWorkplacePaused(field.Id,false),"Resume");
                Check(w.ConnectPaths(w.YardAccess,field.Entrance) && w.ConnectPaths(w.YardAccess,home.Entrance),"Approaches");
            }
            w.SaveFile($"artifacts/inlet-choice/{profile.ModeName}-{choice}-start.json");
            float walked=0;int hungry=0;bool connected=false;for(int i=0;i<6000;i++){var positions=w.People.Select(p=>p.Position).ToArray();w.Tick(.1f);walked+=w.People.Select((p,i)=>(p.Position-positions[i]).Length()).Sum();if(choice=="cross" && !connected && w.Cottages.Any(c=>c.Kind==BuildingKind.Bridge && c.Complete)){Check(w.ConnectPaths(w.YardAccess,field.Entrance),"Crossing approach");connected=true;}if(w.People.Any(p=>!p.Fed))hungry++;if(i%100==0)w.Validate();}
            Check(w.FoundingHasNewFood,"No real harvest");
            if(choice=="cross")Check(w.Cottages.Any(c=>c.Kind==BuildingKind.Bridge && c.Complete),"Crossing never finished");
            if(choice=="keep")Check(hungry==0,"Leave-it opening causes hunger");
            var copy=World.LoadJson(w.SaveJson());for(int i=0;i<100;i++){w.Tick(.1f);copy.Tick(.1f);}
            Check(w.SaveJson()==copy.SaveJson() && copy.PublicPlace==profile && profile.Create().PublicPlace==profile,"Exact continuation/identity");
            w.SaveFile($"artifacts/inlet-choice/{profile.ModeName}-{choice}.json");
            Console.WriteLine($"PASS inlet {profile.ModeName} {choice}: food {w.EdibleStored}, hungry ticks {hungry}, logs {w.Stored}, walked {walked:F0} tiles. Scripted consequence, not preference.");
        }
    }
}
