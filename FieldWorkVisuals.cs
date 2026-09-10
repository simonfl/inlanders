using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;

public partial class Game
{
    // Match F22's remaining crop order, including the last partial harvest.
    private Vector3 FieldWorkTarget(Cottage field, bool sowing)
    {
        bool vegetables=field.Kind==BuildingKind.VegetableGarden;
        int plant=Math.Max(0,field.Harvest-1);
        Vector3 local=sowing ? new(0,.25f,.45f) : vegetables
            ? new(-.96f+plant%4*.64f+.08f,.35f,plant/4==0?-.42f:.48f)
            : new(-1.05f+Math.Min(2,plant)*.42f,.38f,.56f);
        return OnGround(field.Cell.X+(field.Rotated?-.5f:0),field.Cell.Z+(field.Rotated?0:-.5f))
            +local.Rotated(Vector3.Up,field.Rotated?Mathf.Pi/2:0);
    }

    private void AnimateFieldWork(PersonView view,Villager worker)
    {
        var field=_world.Cottages.FirstOrDefault(c=>c.Id==worker.WorkplaceId);
        if(field==null || !field.Complete || field.Kind is not (BuildingKind.Farm or BuildingKind.VegetableGarden)) return;
        bool sowing=worker.Task==Work.Planting, grain=field.Kind==BuildingKind.Farm;
        var target=FieldWorkTarget(field,sowing);
        var direction=target-view.Body.Position;
        view.Body.Rotation=new(0,MathF.Atan2(-direction.X,-direction.Z),0);
        float duration=sowing?4:2, progress=Math.Clamp(worker.Timer/duration,0,1);
        // Enter the bed, work, then return to the simulated entrance before pickup.
        float stance=Mathf.SmoothStep(0,1,progress/.2f)*(1-Mathf.SmoothStep(0,1,(progress-.8f)/.2f));
        float sweep=MathF.Sin((progress-.5f)*Mathf.Tau);
        view.Sickle.Visible=!sowing && grain;
        view.SeedPouch.Visible=sowing;
        view.Torso.Rotation=new((grain && !sowing?-.35f:-.65f)*stance,0,0);
        view.Head.Rotation=new(.2f*stance,0,0);
        view.Arm.Rotation=grain && !sowing ? new(0,sweep*.35f*stance,0) : new(.8f*stance,0,-sweep*.25f*stance);
        view.LeftArm.Rotation=sowing ? new(.9f*stance,0,.35f*stance) : new(.75f*stance,0,.12f*stance);
        view.LeftLeg.Rotation=new(.35f*stance,0,-.06f*stance);
        view.RightLeg.Rotation=new(-.3f*stance,0,.06f*stance);
        // A fixed contact stance lets the arm sweep through the crop instead of
        // dragging the whole body sideways to follow the animated tool.
        var torsoBasis=Basis.FromEuler(new(grain && !sowing?-.35f:-.65f,0,0));
        var armBasis=Basis.FromEuler(new(grain && !sowing?0:.8f,0,0));
        var tip=grain && !sowing ? new Vector3(.16f,-.32f,-.48f) : new Vector3(0,-.36f,0);
        var contact=new Vector3(0,.43f,0)+torsoBasis*(new Vector3(.28f,.36f,0)+armBasis*tip);
        view.Rig.Position=(view.Body.ToLocal(target)-contact)*stance;
    }
}
