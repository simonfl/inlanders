using Godot;
using Inlanders.Simulation;
using System.Linq;

public partial class Game
{
    private bool HasHammerWork(Villager worker) => worker.Task==Work.Building || worker.Task==Work.Demolishing &&
        _world.Cottages.Any(c=>c.Id==worker.SiteId && c.DemolitionProgress<1 && c.StoredLogs+c.StoredPlanks+c.InputLogs+c.OutputPlanks+c.InputGrain+c.OutputBread+c.Harvest==0);
    private static bool DeliveryTask(Work task) => task is Work.ToStockpile or Work.ToHaulDrop or Work.ToPantry or Work.ToCottage or Work.ToOven or Work.ToSawmill;
    private static bool PickupTask(Work task) => task is Work.ToMaterials or Work.ToGrain or Work.ToSawLogs or Work.ToBread or Work.ToPlanks or Work.ToHaulPickup;
    private static float ArrivalReach(Villager worker)
    {
        if(worker.Route.Count>1) return 0;
        float distance=System.Numerics.Vector2.Distance(worker.Position,worker.Destination.Point);
        return 1-Mathf.SmoothStep(0,1,distance/.85f);
    }
    private void AnimateCargoHandoff(PersonView view,Villager worker)
    {
        float lift=view.PickupStarted is float start ? 1-Mathf.SmoothStep(0,1,(_world.Food.Time-start)/.4f) : 0;
        float lower=DeliveryTask(worker.Task)?ArrivalReach(worker):0;
        float reach=Mathf.Max(lift,lower);
        view.Carry.Position=new(0,.12f-.18f*reach,-.43f-.08f*reach);
        view.Arm.Rotation=view.LeftArm.Rotation=new(1.05f-.35f*reach,0,0);
        view.Torso.Rotation=new(-.08f-.18f*reach,0,0);
        view.Head.Rotation=new(.12f*reach,0,0);
    }
    private static bool AnimatePickupReach(PersonView view,Villager worker)
    {
        if(!PickupTask(worker.Task)) return false;
        float reach=ArrivalReach(worker); if(reach<=0) return false;
        view.Arm.Rotation=view.LeftArm.Rotation=new(.7f*reach,0,0);
        view.Torso.Rotation=new(-.26f*reach,0,0);
        view.Head.Rotation=new(.12f*reach,0,0);
        return true;
    }
}
