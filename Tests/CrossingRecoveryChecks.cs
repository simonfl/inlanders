using Inlanders.Simulation;

static class CrossingRecoveryChecks
{
    public static void Run()
    {
        Directory.CreateDirectory("artifacts/crossing-recovery");
        foreach(var route in new[]{("repair",0),("garden",0),("repair",240)})
        {
            var (plan,delay)=route;
            var w=World.NewCrossingRecovery();
            var oldField=w.Cottages.Single(c=>c.Kind==BuildingKind.VegetableField);
            if(w.Housed!=4 || w.PublicPlace?.SaveName!="broken-crossing.json" || !w.ReadWorkplace(oldField).MissingAccess || w.ReadFirstHomeObjectives().Action!="recover")throw new Exception("Opening is not a housed recovery problem");
            w.SaveFile("artifacts/crossing-recovery/start.json");
            for(int i=0;i<delay*10;i++)w.Tick(.1f);
            if(delay>0 && (w.ReachableEdibleStored!=0 || w.People.All(p=>p.Fed) || oldField.Harvest!=20 || w.WorkplaceFoodDelivered(oldField.Id)!=0))throw new Exception("No-action control did not strand food or cause shortage");
            if(delay>0)w.SaveFile("artifacts/crossing-recovery/shortage.json");
            if(plan=="repair")
            {
                var proposal=w.ReadBridgeOpportunity(World.RecoveryCrossing,0) ?? throw new Exception("Crossing cannot be repaired");
                if(proposal.RipeFood!=20 || proposal.Workplaces!=1)throw new Exception("Crossing preview does not identify stranded crop");
                var cancelled=w.Place(World.RecoveryCrossing,0,BuildingKind.Bridge)!;
                if(!w.Cancel(cancelled.Id) || w.ReadFirstHomeObjectives().Action!="recover")throw new Exception("Cancel plan does not restore alternatives");
                w.Place(World.RecoveryCrossing,0,BuildingKind.Bridge);
                w.ApplyWoodlandCare(new(-11,6),new(-6,10),WoodlandIntent.Renew,false);
            }
            else
            {
                var placement=ReviewPlacement.Find(w,BuildingKind.VegetableGarden,new(0,5),(c,r)=>c.Z>0,"local recovery")!;
                w.Place(placement.Actual,placement.Rotation,BuildingKind.VegetableGarden);
            }
            float firstFood=0,access=0;int peakHungry=0;bool cropCrossed=false;
            for(int i=0;i<9000 && !w.Founding!.FirstHome!.Complete;i++)
            {
                w.Tick(.1f);peakHungry=Math.Max(peakHungry,w.People.Count(p=>!p.Fed));
                if(firstFood==0 && w.ReadFoodFlow().Delivered>0)firstFood=w.Food.Time;
                if(access==0 && !w.ReadWorkplace(oldField).MissingAccess)access=w.Food.Time;
                if(w.People.Any(p=>p.Cargo==Resource.Vegetables && p.Carried>0 && World.At(p)==World.RecoveryCrossing))cropCrossed=true;
                if(plan=="repair" && access>0 && w.Food.Time<access+.11f)w.SaveFile("artifacts/crossing-recovery/crossing-open.json");
            }
            if(!w.Founding!.FirstHome!.Complete || w.Founding.FirstHome.Served.Count!=4 || w.ReadFirstHomeObjectives().Done!=3 || w.Cottages.Count(c=>c.Kind==BuildingKind.Cottage)!=2)throw new Exception("Recovery did not feed all four without new housing");
            if(plan=="repair" && (w.ReadWorkplace(oldField).MissingAccess || w.Cottages.Count!=4 || !cropCrossed))throw new Exception("Repair needed a new food workplace or harvest never crossed");
            if(plan=="garden" && (!w.ReadWorkplace(oldField).MissingAccess || oldField.Harvest!=20 || w.Cottages.Any(c=>c.Kind==BuildingKind.Bridge)))throw new Exception("Local plan secretly used the crossing");
            w.Validate();var copy=World.LoadJson(w.SaveJson());
            for(int i=0;i<50;i++){w.Tick(.1f);copy.Tick(.1f);}
            if(w.SaveJson()!=copy.SaveJson())throw new Exception("Recovery save differs");
            w.SaveFile($"artifacts/crossing-recovery/{plan}-{delay}-complete.json");
            Console.WriteLine($"PASS {plan} delay{delay}: access {access:0}s firstfood {firstFood:0}s complete {w.Food.Time:0}s hungry {peakHungry}/4, no new homes, exact save");
        }
    }
}
