using Godot;
using Inlanders.Simulation;

public partial class Game
{
    private void MakeCarpenter(Node3D parent,int stage)
    {
        StoneFoot(parent,new(0,.1f,-.2f),new(2.4f,.2f,1.3f));
        if(stage<1)return;
        foreach(float x in new[]{-1.1f,1.1f}) foreach(float z in new[]{-.7f,.65f})
            Box(parent,new(x,.95f,z),new(.16f,1.7f,.16f),_wood);
        if(stage<2)return;
        Box(parent,new(0,1,-.73f),new(2.1f,1.6f,.12f),new("c6ae84"));
        Box(parent,new(.1f,.82f,.1f),new(1.65f,.17f,.65f),new("c49361"));
        foreach(float x in new[]{-.55f,.75f}) Box(parent,new(x,.42f,.1f),new(.14f,.8f,.5f),_wood);
        Box(parent,new(-.4f,.98f,.12f),new(.36f,.12f,.23f),new("596d72"));
        for(int i=0;i<3;i++) Box(parent,new(-.6f+i*.6f,1.2f,-.63f),new(.08f,.4f,.1f),_wood);
        if(stage<3)return;
        VillageRoof(parent,new(0,1.85f,-.02f),2.7f,1.9f,.65f,new("758991"),true);
        FoodSign(parent,"CARPENTER",2.7f);
    }
    private void MakeHomeComfort(Node3D parent,Cottage home,int stage)
    {
        if(stage<3 || Buildings.Get(home.Kind).Beds==0)return;
        var detail=new Node3D {Name="HomeComfort"};parent.AddChild(detail);
        if(home.Improved)
        {
            float z=home.Kind==BuildingKind.Cottage?.74f:.98f;
            foreach(float x in new[]{.15f,1.10f})
            {
                Box(detail,new(x,1.1f,z),new(.22f,.64f,.09f),new("57766d"));
                for(int i=0;i<3;i++) Box(detail,new(x,.91f+i*.18f,z+.05f),new(.25f,.04f,.04f),new("93a48a"));
            }
            Box(detail,new(.62f,.72f,z),new(.9f,.1f,.23f),_wood);
        }
        else if(home.ImprovementPlanks>0)
            for(int i=0;i<home.ImprovementPlanks;i++) Plank(detail,new(.78f,.12f+i*.055f,.5f));
    }
}
