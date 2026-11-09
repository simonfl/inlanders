using Godot;
using System;

public partial class Game
{
    private void MakeVillagerClothes(PersonView view,int id,Color cloth)
    {
        var clothes=new Node3D();view.Torso.AddChild(clothes);
        Cylinder(clothes,new(0,.21f,0),.26f,.48f,cloth,.21f);
        // Broad patches stay readable from the village camera. No job/color coding.
        if(id%2==0)
        {
            var apron=new Color(new[]{"bbaa8d","c7bba1","bda77f","b8ac97"}[id/2%4]);
            Box(clothes,new(0,.13f,-.235f),new(.29f,.31f,.035f),apron);
            Box(clothes,new(0,.34f,-.201f),new(.20f,.15f,.035f),apron.Lightened(.04f));
        }
        else
        {
            Box(clothes,new(0,.25f,-.215f),new(.09f,.36f,.035f),new("cfc2a6"));
            foreach(float x in new[]{-.075f,.075f})
                Box(clothes,new(x,.32f,-.225f),new(.055f,.24f,.045f),cloth.Darkened(.15f));
        }
        Cylinder(clothes,new(0,.035f,0),.263f,.055f,new("594939"),.257f);
        Box(clothes,new(.03f,.035f,-.268f),new(.07f,.05f,.02f),new("b5a27a"));
        foreach(float x in new[]{-.07f,.07f})
        {
            var collar=Box(clothes,new(x,.425f,-.14f),new(.105f,.075f,.095f),new("d3c6ab"));
            collar.Rotation=new(0,0,x*2);
        }
        BatchStaticGeometry(clothes);
    }

    private void MakeVillagerFace(PersonView view,int id,Color skin,Color cloth)
    {
        var hair=new Color(new[]{"4c3729","6a4934","b39e7c","3f3831","8c6544","aaa497"}[id%6]);
        Cylinder(view.Head,new(0,.145f,.015f),.205f,.13f,hair,.14f);
        Box(view.Head,new(0,.03f,.15f),new(.27f,.23f,.09f),hair);
        foreach(float x in new[]{-.175f,.175f})
        {
            Box(view.Head,new(x,.015f,.025f),new(.06f,.17f,.17f),hair);
            Box(view.Head,new(x,-.015f,-.035f),new(.055f,.075f,.07f),skin);
        }
        foreach(float x in new[]{-.069f,.069f})
            Box(view.Head,new(x,.018f,-.18f),new(.026f,.029f,.021f),new("3e352b"));
        Box(view.Head,new(0,-.023f,-.197f),new(.048f,.06f,.065f),skin.Darkened(.05f));
        Box(view.Head,new(0,-.080f,-.175f),new(.046f,.013f,.024f),new("97664e"));
        if(id%4==1)
        {
            // Soft wool cap with a folded rim and a gently sloping crown.
            Cylinder(view.Hat,new(0,.19f,0),.22f,.09f,cloth.Darkened(.15f));
            var crown=Cylinder(view.Hat,new(.025f,.28f,.015f),.205f,.16f,cloth,.12f);
            crown.Rotation=new(.08f,0,-.12f);
        }
        else if(id%4==0 || id%4==2)
        {
            // Linen head covering: light crown, side folds and a small nape knot.
            Cylinder(view.Hat,new(0,.21f,.025f),.215f,.14f,new("c8bea4"),.17f);
            foreach(float x in new[]{-.19f,.19f})Box(view.Hat,new(x,.065f,.04f),new(.05f,.20f,.20f),new("b8ab90"));
            Box(view.Hat,new(0,.06f,.225f),new(.13f,.09f,.08f),new("9f947f"));
        }
        else
        {
            Cylinder(view.Hat,new(0,.18f,0),.30f,.045f,new("746045"));
            Cylinder(view.Hat,new(0,.27f,0),.19f,.17f,new("867255"),.16f);
            Cylinder(view.Hat,new(0,.21f,0),.195f,.045f,new("514338"),.19f);
        }
    }

    private static void FinishVillagerLegs(PersonView view)
    {
        // Existing seated poses control the hips. Counter-rotate the lower legs
        // so feet fall below the knees instead of pointing rigidly into the air.
        if(view.LeftLeg.Rotation.X>0 && view.RightLeg.Rotation.X>0)
        {
            view.LeftKnee.Rotation=new(-view.LeftLeg.Rotation.X,0,0);
            view.RightKnee.Rotation=new(-view.RightLeg.Rotation.X,0,0);
        }
    }
}
