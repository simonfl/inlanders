using Godot;
using Inlanders.Simulation;

public partial class Game
{
    private void MakeFarm(Node3D parent, int stage)
    {
        Box(parent, new(0,0.04f,0),new(2.8f,0.08f,1.8f),new("74654b"));
        foreach(float x in new[]{-1.35f,1.35f}) foreach(float z in new[]{-0.85f,0.85f})
            Box(parent,new(x,0.18f,z),new(0.055f,0.36f,0.055f),_wood);
        if(stage<1) return;
        // Beds are dug before the final timber edging is fitted.
        for(int row=0;row<(stage<2?2:3);row++)
            Box(parent,new(0,0.10f,-0.56f+row*0.56f),new(2.55f,0.1f,0.38f),new("594f37"));
        if(stage<2) return;
        foreach(float z in new[]{-0.88f,0.88f})
            Box(parent,new(0,0.16f,z),new(2.8f,0.12f,0.07f),new("b99b6f"));
        if(stage<3) return;
        foreach(float x in new[]{-1.38f,1.38f})
            Box(parent,new(x,0.16f,0),new(0.07f,0.12f,1.8f),new("b99b6f"));
        FoodSign(parent,"FARM",1.65f);
    }
    private void MakeCrops(Node3D root,Cottage farm,int stage)
    {
        if(stage==0) return;
        // Each of six columns represents one grain still in the field.
        for(int x=0;x<6;x++) for(int z=0;z<3;z++)
        {
            bool cut=stage==4 && x>=farm.Harvest;
            var tuft=new Node3D { Name=$"Crop{x}_{z}",Position=new(-1.05f+x*0.42f,0.15f,-0.56f+z*0.56f) };root.AddChild(tuft);
            tuft.SetMeta("standing",!cut);
            float height=cut?0.08f:stage switch { 1=>0.18f,2=>0.4f,3=>0.62f,_=>0.78f };
            var color=cut?new Color("c2a16b"):stage switch { 1=>new Color("8baf69"),2=>new Color("71984e"),3=>new Color("b9ad58"),_=>new Color("dfbb64") };
            foreach(float offset in new[]{-0.075f,0.075f})
            {
                Cylinder(tuft,new(offset,height/2,0),0.022f,height,color,0.012f);
                if(cut) continue;
                var leaf=Box(tuft,new(offset,height*0.5f,0.055f),new(0.055f,0.025f,height*0.55f),color);
                leaf.RotationDegrees=new(-28,offset<0?-35:35,0);
                if(stage>=3)
                    Mesh(tuft,new SphereMesh { Radius=0.048f,Height=0.18f,RadialSegments=6,Rings=3 },new(offset,height,0),color.Lightened(0.08f));
            }
        }
    }
}
