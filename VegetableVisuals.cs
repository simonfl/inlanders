using Godot;
using Inlanders.Simulation;

public partial class Game
{
    private void MakeVegetableGarden(Node3D root, int stage)
    {
        Box(root,new(0,.04f,0),new(2.8f,.08f,1.8f),new("a18c63"));
        foreach(float x in new[]{-1.3f,1.3f}) foreach(float z in new[]{-.82f,.82f})
            Box(root,new(x,.2f,z),new(.06f,.4f,.06f),_wood);
        if(stage<1) return;
        foreach(float z in new[]{-.45f,.45f})
        {
            Box(root,new(0,.14f,z),new(2.55f,.2f,.66f),new("66523b"));
            foreach(float edge in new[]{-.34f,.34f})
                Box(root,new(0,.22f,z+edge),new(2.65f,.16f,.07f),new("b08a57"));
        }
        if(stage<2) return;
        foreach(float x in new[]{-1.3f,1.3f}) foreach(float z in new[]{-.45f,.45f})
            Box(root,new(x,.22f,z),new(.07f,.16f,.72f),new("b08a57"));
        if(stage<3) return;
        Cylinder(root,new(-1.1f,.43f,.75f),.12f,.3f,new("738d89"));
        FoodSign(root,"VEGETABLES",1.35f);
    }
    private void MakeSquash(Node3D root,Vector3 at,float radius)
    {
        Mesh(root,new SphereMesh { Radius=radius,Height=radius*1.5f,RadialSegments=8,Rings=4 },at,new("d98b3d"));
        Cylinder(root,at+new Vector3(0,radius*.83f,0),radius*.13f,radius*.35f,new("647341"));
    }
    private void MakeVegetables(Node3D root,Cottage garden,int stage)
    {
        if(stage==0) return;
        for(int i=0;i<8;i++)
        {
            bool standing=stage<4 || i<garden.Harvest;
            var plant=new Node3D { Name=$"Vegetable{i}",Position=new(-.96f+i%4*.64f,.25f,i/4==0?-.45f:.45f) };
            root.AddChild(plant); plant.SetMeta("standing",standing);
            if(!standing) { Box(plant,new(0,.02f,0),new(.17f,.035f,.08f),new("8c8657")); continue; }
            float leafSize=stage==1?.12f:stage==2?.21f:.27f;
            for(int leaf=0;leaf<3;leaf++)
            {
                var blade=Mesh(plant,new SphereMesh { Radius=leafSize,Height=.055f,RadialSegments=7,Rings=3 },
                    new((leaf-1)*.07f,.06f,leaf%2*.09f-.045f),new(stage==1?"8ba85a":"62814c"));
                blade.Scale=new(1,.8f,.55f); blade.RotationDegrees=new(0,leaf*60,leaf*10-10);
            }
            if(stage>=3) MakeSquash(plant,new(.08f,.1f,.03f),stage==3?.12f:.19f);
        }
    }
}
