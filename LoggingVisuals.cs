using Godot;
using Inlanders.Simulation;

public partial class Game
{
    private const float TreeFallSeconds=.9f;
    // Four strokes in the existing four work-second chop. The last stroke lands before felling.
    private static float AxeStroke(float timer)
    {
        float phase=timer%1;
        return phase<.55f ? Mathf.Lerp(.12f,1.9f,Mathf.SmoothStep(0,1,phase/.55f)) :
            phase<.75f ? Mathf.Lerp(1.9f,.12f,Mathf.SmoothStep(0,1,(phase-.55f)/.2f)) : .12f;
    }
    private static void AnimateAxeStroke(PersonView view,float timer)
    {
        view.Rig.Position=new(0,0,-.17f);
        view.Arm.Rotation=new(AxeStroke(timer),0,0);
        view.LeftArm.Rotation=new(.8f,0,.15f);
        view.Torso.Rotation=new(-.14f,0,0);
        view.Head.Rotation=new(.1f,0,0);
        view.LeftLeg.Rotation=new(-.12f,0,-.08f); view.RightLeg.Rotation=new(.1f,0,.08f);
    }
    private void AnimateTimberTree(TreeView view,TimberTree tree)
    {
        if(!tree.Felled) { view.ObservedStanding=!tree.NeedsPlanting && tree.Growth>=1; view.FallStarted=null; }
        else if(view.ObservedStanding && !tree.Salvage) { view.ObservedStanding=false; view.FallStarted=_world.Food.Time; }
        float fall=view.FallStarted is float started ? (_world.Food.Time-started)/TreeFallSeconds : 1;
        bool falling=tree.Felled && fall<1;
        view.Top.Visible=falling || !tree.Felled && !tree.NeedsPlanting && (tree.Logs>0 || tree.Growth<1);
        view.Top.Rotation=falling ? new(0,0,Mathf.Pi*.48f*fall*fall) : Vector3.Zero;
        // The last short collapse resolves the canopy into the ordinary timber pile.
        float collapse=falling ? 1-Mathf.SmoothStep(0,1,(fall-.7f)/.3f) : 1;
        view.Top.Scale=Vector3.One*.9f*(.2f+.8f*tree.Growth)*collapse;
        view.Pile.Visible=!falling;
    }
}
