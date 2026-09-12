using Godot;

public partial class Game
{
    private void MakeVillageSquare(Node3D parent,int stage)
    {
        // A civic frontage, not seating inside an inaccessible building footprint.
        foreach(float x in new[]{-.92f,0f,.92f})
        {
            var stone=Box(parent,new(x,.035f,.50f),new(.79f,.065f,.52f),new("99977f"));
            stone.RotationDegrees=new(0,x*4,0);
        }
        foreach(float x in new[]{-1.06f,1.06f}) StoneFoot(parent,new(x,.11f,-.55f),new(.39f,.20f,.40f));
        if(stage<1)return;
        foreach(float x in new[]{-1.06f,1.06f})
        {
            Box(parent,new(x,1.12f,-.55f),new(.18f,2.04f,.18f),_frameTimber);
            TimberBeam(parent,new(x,1.55f,-.55f),new(x*.59f,2.18f,-.55f),.13f,_frameTimber);
        }
        Box(parent,new(0,2.18f,-.55f),new(2.61f,.21f,.23f),_frameTimber);
        if(stage<2)return;
        // Shared serving table sits behind the open approach; no permanently stocked food.
        foreach(float x in new[]{-.65f,.65f})
        {
            Box(parent,new(x,.37f,-.35f),new(.17f,.67f,.44f),_frameTimber);
            Box(parent,new(x,.12f,-.35f),new(.40f,.17f,.62f),_wood);
        }
        foreach(float z in new[]{-.51f,-.25f}) Box(parent,new(0,.73f,z),new(1.88f,.16f,.24f),new("b39266"));
        if(stage<3)return;
        // Open arbor and cloth pennants mark the center without hiding nearby visitors.
        foreach(float x in new[]{-1.12f,-.56f,0f,.56f,1.12f})
            Box(parent,new(x,2.31f,-.55f),new(.12f,.14f,.77f),new("8d7555"));
        for(int i=0;i<5;i++)
        {
            float x=-.8f+i*.4f;
            Box(parent,new(x,1.94f,-.41f),new(.25f,.30f+(i%2)*.08f,.035f),new(i%2==0?"ae795e":"799087"));
        }
        foreach(float side in new[]{-1f,1f})
        {
            Cylinder(parent,new(side*1.14f,.20f,-.57f),.20f,.34f,new("987756"),.24f);
            Mesh(parent,new SphereMesh {Radius=.27f,Height=.38f,RadialSegments=7,Rings=3},new(side*1.14f,.49f,-.57f),new("6f8056"));
        }
        FoodSign(parent,"VILLAGE SQUARE",2.73f);
    }
}
