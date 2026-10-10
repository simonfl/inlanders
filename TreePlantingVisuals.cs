using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
public partial class Game
{
    private void AnimateTreePlanting(PersonView view,Villager worker)
    {
        var tree=_world.Trees.FirstOrDefault(t=>t.Id==worker.TreeId);if(tree==null)return;
        bool digging=worker.Timer<2;
        var target=OnGround(tree.Cell.X,tree.Cell.Z,.06f);
        var direction=target-view.Body.Position;
        view.Body.Rotation=new(0,MathF.Atan2(-direction.X,-direction.Z),0);
        float reach=Mathf.SmoothStep(0,1,worker.Timer/.35f)*(1-Mathf.SmoothStep(0,1,(worker.Timer-3.65f)/.35f));
        PoseVisible(view,view.Spade,digging);PoseVisible(view,view.Sapling,!digging);
        float stroke=digging?MathF.Sin(worker.Timer*Mathf.Tau)*.22f:0;
        view.Torso.Rotation=new(-.75f*reach,0,0);view.Head.Rotation=new(.15f*reach,0,0);
        view.Arm.Rotation=new((.8f+stroke)*reach,0,0);view.LeftArm.Rotation=new(.8f*reach,0,0);
        view.LeftLeg.Rotation=new(.3f*reach,0,-.08f);view.RightLeg.Rotation=new(-.25f*reach,0,.08f);
        var contact=new Vector3(0,.43f,0)+Basis.FromEuler(new(-.75f,0,0))*(new Vector3(digging?.28f:-.28f,.36f,0)+Basis.FromEuler(new(.8f,0,0))*(digging?new Vector3(0,-.22f,-.55f):new Vector3(0,-.36f,0)));
        view.Rig.Position=(view.Body.ToLocal(target)-contact)*reach;
    }
}
