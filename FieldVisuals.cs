using Godot;
using Inlanders.Simulation;

public partial class Game
{
    private void SoilBed(Node3D parent,Vector3 at,float width,float depth,float height,Color color)
    {
        // Tapered, faceted earth meets the grass without a full rectangular plinth.
        var bed=Mesh(parent,new CylinderMesh {BottomRadius=1,TopRadius=.93f,Height=height,RadialSegments=12},at+new Vector3(0,height/2,0),color);
        bed.Scale=new(width/2,1,depth/2);
    }
    private void MakeFarm(Node3D parent, int stage)
    {
        foreach(float x in new[]{-1.35f,1.35f}) foreach(float z in new[]{-0.85f,0.85f})
            Box(parent,new(x,0.11f,z),new(0.055f,0.22f,0.055f),_wood);
        if(stage<1) return;
        // Two worked strips become three; crop origins remain at the soil surface.
        for(int row=0;row<(stage<2?2:3);row++)
            SoilBed(parent,new(0,0,-.56f+row*.56f),2.72f,.46f,.15f,new Color("746348").Lightened(row*.018f));
        if(stage<3) return;
        // Short end markers leave all three rows open, rather than boxing in the field.
        foreach(float z in new[]{-.56f,0f,.56f})
            Box(parent,new(-1.34f,.07f,z),new(.07f,.10f,.24f),new("998060"));
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
