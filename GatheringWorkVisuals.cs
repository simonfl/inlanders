using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;

public partial class Game
{
    private static Vector3 BerryPosition(int index)
    {
        float angle=index*Mathf.Tau/8;
        return new(MathF.Cos(angle)*.34f,.47f,MathF.Sin(angle)*.34f);
    }
    private Vector3 BerryWorkTarget(Villager worker)
    {
        var bush=_world.Bushes.Single(b=>b.Id==worker.BushId);
        int picked=Math.Min(1,(int)worker.Timer);
        return OnGround(bush.Cell.X,bush.Cell.Z)+BerryPosition(Math.Max(0,bush.Ripe-1-picked));
    }
    private void AnimateBerryPicking(PersonView view,Villager worker)
    {
        if(!_world.Bushes.Any(b=>b.Id==worker.BushId))return;
        var target=BerryWorkTarget(worker);
        var direction=target-view.Body.Position;
        view.Body.Rotation=new(0,MathF.Atan2(-direction.X,-direction.Z),0);
        float phase=worker.Timer%1;
        float reach=Mathf.SmoothStep(0,1,phase/.25f)*(1-Mathf.SmoothStep(0,1,(phase-.7f)/.3f));
        view.Torso.Rotation=new(-.65f*reach,0,0);
        view.Head.Rotation=new(.2f*reach,0,0);
        view.Arm.Rotation=new(.8f*reach,0,0);
        view.LeftArm.Rotation=new(.65f*reach,0,.16f*reach);
        view.LeftLeg.Rotation=new(.2f*reach,0,-.06f*reach);
        view.RightLeg.Rotation=new(-.2f*reach,0,.06f*reach);
        var contact=new Vector3(0,.43f,0)+Basis.FromEuler(new(-.65f,0,0))*(new Vector3(.28f,.36f,0)+Basis.FromEuler(new(.8f,0,0))*new Vector3(0,-.36f,0));
        view.Rig.Position=(view.Body.ToLocal(target)-contact)*reach;
    }
}
