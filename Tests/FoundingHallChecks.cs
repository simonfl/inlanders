using Inlanders.Simulation;
using System.Numerics;
using System.Text.Json;
static class FoundingHallChecks
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static World Ready()
    {
        var w=World.NewFoundingSettlement();
        Check(!w.BeginFoundingHall() && !w.FinishFoundingHall(),"Opening skips founding");
        foreach(var at in new[]{new Cell(-3,0),new(0,0),new(0,6),new(-3,5),new(-6,6)})Place(w,BuildingKind.Cottage,at);
        for(int i=0;i<12000 && w.FinishFoundingProblem()!=null;i++)
        {w.Tick(.1f);if(w.Population<12 && w.InvitationProblem()==null)Check(w.InviteNewcomers(),"Invitation failed");}
        Check(w.FinishFounding(),"Founding stalled");w.Validate();return w;
    }
    static Cottage Place(World w,BuildingKind kind,Cell at)
    {
        var p=ReviewPlacement.Find(w,kind,at,(c,r)=>true,"hall project")??throw new Exception("No legal "+kind);
        return w.Place(p.Actual,p.Rotation,kind)??throw new Exception("Order rejected");
    }
    public static void Run(bool support=false)
    {
        string dir=support?"artifacts/remote-support":"artifacts/founding-hall";Directory.CreateDirectory(dir);
        var baseline=Ready();baseline.SaveFile(dir+"/initial.json");
        foreach(string arm in (support?new[]{"home-side","stone-side","stone-depot","depots-pantry"}:new[]{"home-side","stone-side","awkward"}))
        {
            var w=World.LoadJson(baseline.SaveJson());Check(w.BeginFoundingHall() && !w.BeginFoundingHall(),"Begin state");
            Check(!w.FinishFoundingHall(),"Empty project finished");
            Place(w,BuildingKind.Quarry,new(15,1));Place(w,BuildingKind.Sawmill,new(-6,5));
            var hall=Place(w,BuildingKind.GatheringHall,arm=="home-side"?new(-2,3):arm!="awkward"?new(15,5):new(8,-10));
            var depots=new List<Cottage>();
            if(arm is "stone-depot" or "depots-pantry")
            {
                var depot=Place(w,BuildingKind.Stockpile,new(15,3));
                Check(w.SetStorageMaterial(depot.Id,Resource.Stone),"Stone configuration");depots.Add(depot);
                if(arm=="depots-pantry")
                {
                    depot=Place(w,BuildingKind.Stockpile,new(12,5));
                    Check(w.SetStorageMaterial(depot.Id,Resource.Planks),"Plank configuration");depots.Add(depot);
                    var pantry=Place(w,BuildingKind.Pantry,new(16,7));Check(w.SetPantryTarget(pantry.Id,12),"Pantry target");
                }
            }
            w.SaveFile(dir+"/"+arm+"-start.json");
            float? built=null,used=null;double materialTravel=0,leisureTravel=0,hungry=0;int visits=0;var lastVisits=w.People.Select(p=>p.LeisureVisits).ToArray();
            for(int i=0;i<9000;i++)
            {
                var positions=w.People.Select(p=>p.Position).ToArray();var material=w.People.Select(p=>p.Carried>0 && p.Cargo is Resource.Planks or Resource.Stone).ToArray();
                var walking=w.People.Select(p=>p.LeisureSiteId==hall.Id && p.Task==Work.ToLeisure).ToArray();
                foreach(var depot in depots.Where(d=>d.Complete && d.StorageTarget==0))
                    Check(w.SetStorageTarget(depot.Id,depot.StorageMaterial==Resource.Stone?12:8),"Depot target");
                w.Tick(.1f);
                for(int n=0;n<w.Population;n++)
                {double d=Vector2.Distance(positions[n],w.People[n].Position);if(material[n])materialTravel+=d;if(walking[n])leisureTravel+=d;if(w.People[n].LeisureVisits>lastVisits[n] && w.People[n].LastLeisureSiteId==hall.Id)visits++;lastVisits[n]=w.People[n].LeisureVisits;}
                hungry+=w.People.Count(p=>!p.Fed)*.1;
                if(built==null && hall.Complete){built=w.Food.Time-baseline.Food.Time;Check(!w.FinishFoundingHall(),"Construction alone finished project");}
                if(used==null && w.FinishFoundingHallProblem()==null){used=w.Food.Time-baseline.Food.Time;Check(w.FinishFoundingHall() && !w.FinishFoundingHall(),"Finish state");}
                if(i%100==0)w.Validate();
                if(i==1199)w.SaveFile(dir+"/"+arm+"-early.json");

            }
            Check(used!=null,"Hall never used: "+arm);w.Validate();string json=w.SaveJson();Check(World.LoadJson(json).SaveJson()==json,"Save mismatch");w.SaveFile(dir+"/"+arm+"-late.json");
            var result=new{arm,built,used,materialTravel,leisureTravel,visits,hungry,cell=hall.Cell,rotation=hall.Rotation,remainingStone=w.Map.StoneDeposits.Single().Remaining};
            File.WriteAllText(dir+"/"+arm+"-report.json",JsonSerializer.Serialize(result,new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine(JsonSerializer.Serialize(result));
        }
        if(support){Console.WriteLine("PASS: remote support comparison, actual use and current saves");return;}
        // Recover the awkward layout through ordinary dismantling and recovered materials.
        var repair=World.LoadFile(dir+"/awkward-late.json");var old=repair.FoundingHall!;
        Check(repair.RequestDemolition(old.Id),"Recovery demolition refused");
        var replacement=Place(repair,BuildingKind.GatheringHall,new(-2,3));
        for(int i=0;i<18000 && (!replacement.Complete || repair.Cottages.Any(c=>c.Id==old.Id));i++)repair.Tick(.1f);
        Check(replacement.Complete && repair.Cottages.All(c=>c.Id!=old.Id),"Recovery stalled");for(int i=0;i<6000 && repair.People.All(p=>p.LastLeisureSiteId!=replacement.Id);i++)repair.Tick(.1f);
        Check(repair.People.Any(p=>p.LastLeisureSiteId==replacement.Id),"Replacement hall never used");repair.Validate();repair.SaveFile(dir+"/recovered.json");
        Console.WriteLine("PASS: two sites, awkward site, actual use, current saves and ordinary demolition/rebuild recovery");
    }
}
