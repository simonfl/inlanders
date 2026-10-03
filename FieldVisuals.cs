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
    private bool ContinuousFields=>_world.PublicPlace!=null && !_plainFarmstead;
    private void MakeWorkedField(Node3D parent,int stage,bool grain,int plotRows=5)
    {
        foreach(float x in new[]{-1.43f,1.43f})foreach(float z in new[]{-plotRows/2f+.07f,plotRows/2f-.07f})
            Box(parent,new(x,.10f,z),new(.035f,.20f,.035f),_wood);
        if(stage<1)return;
        // One continuous tilled surface, entirely inside the real 3x5 reserved plot.
        float depth=stage==1?plotRows/2f-.04f:plotRows-.06f;
        Box(parent,new(0,.021f,stage==1?plotRows/4f:0),new(2.94f,.038f,depth),new("736044"));
        int rows=grain?6:plotRows;
        for(int i=0;i<rows;i++)
        {
            float z=grain?World.GrainRow(i+1):-(plotRows-1)/2f+i;
            if(stage==1 && z<0)continue;
            // Raised shoulders catch light while the recessed furrow stays dark.
            using var ridge=new SurfaceTool();ridge.Begin(Godot.Mesh.PrimitiveType.Triangles);
            for(int segment=0;segment<6;segment++)
            {
                float left=-1.42f+segment*.47f,right=left+.47f;
                float rise=.095f+(segment%3)*.008f;
                var a=new Vector3(left,.044f,z-.15f);var b=new Vector3(right,.044f,z-.15f);
                var c=new Vector3(right,rise,z);var d=new Vector3(left,rise,z);
                Triangle(ridge,a,d,c,new("92764e"));Triangle(ridge,a,c,b,new("92764e"));
                var e=new Vector3(left,.044f,z+.15f);var f=new Vector3(right,.044f,z+.15f);
                Triangle(ridge,d,e,f,new("685139"));Triangle(ridge,d,f,c,new("685139"));
            }
            SurfaceMesh(parent,ridge);
            Box(parent,new(0,.043f,z+.22f),new(2.85f,.012f,.10f),new("534631"));
        }
        if(stage==3)
        {
            // A low working edge reads as cultivated land; no floating field placard.
            Box(parent,new(0,.041f,plotRows/2f-.18f),new(2.83f,.025f,.24f),new("a18b62"));
            TimberBeam(parent,new(-1.25f,.06f,plotRows/2f-.25f),new(-1.19f,.59f,plotRows/2f-.39f),.035f,_wood);
            Box(parent,new(-1.25f,.07f,plotRows/2f-.25f),new(.20f,.035f,.10f),new("65645b"));
        }
    }
    private void MakeFarm(Node3D parent, int stage)
    {
        if(ContinuousFields){MakeWorkedField(parent,stage,true);return;}
        foreach(float x in new[]{-1.35f,1.35f}) foreach(float z in new[]{-2.35f,2.35f})
            Box(parent,new(x,0.11f,z),new(0.055f,0.22f,0.055f),_wood);
        if(stage<1) return;
        // Six harvest rows occupy actual reserved land; origins meet the soil.
        for(int row=0;row<(stage<2?3:6);row++)
            SoilBed(parent,new(0,0,World.GrainRow(row+1)),2.72f,.64f,.15f,new Color("746348").Lightened(row*.018f));
        if(stage<3) return;
        // Short end markers leave all three rows open, rather than boxing in the field.
        foreach(float z in new[]{-1.9f,-1.14f,-.38f,.38f,1.14f,1.9f})
            Box(parent,new(-1.34f,.07f,z),new(.07f,.10f,.24f),new("998060"));
        FoodSign(parent,"FARM",1.65f);
    }
    private void MakeCrops(Node3D root,Cottage farm,int stage)
    {
        if(stage==0) return;
        // Each of six rows represents one grain still in the field.
        for(int x=0;x<3;x++) for(int z=0;z<6;z++)
        {
            bool cut=stage==4 && z>=farm.Harvest;
            var tuft=new Node3D { Name=$"Crop{x}_{z}",Position=new(-.96f+x*.96f,ContinuousFields?.055f:.15f,World.GrainRow(z+1)) };root.AddChild(tuft);
            tuft.SetMeta("standing",!cut);
            float height=cut?0.08f:stage switch { 1=>0.18f,2=>0.4f,3=>0.62f,_=>0.78f };
            var color=cut?new Color("c2a16b"):stage switch { 1=>new Color("8baf69"),2=>new Color("71984e"),3=>new Color("b9ad58"),_=>new Color("dfbb64") };
            foreach(float offset in new[]{-.24f,-.12f,0f,.12f,.24f})
            {
                Cylinder(tuft,new(offset,height/2,0),0.022f,height,color,0.012f);
                if(cut) continue;
                var leaf=Box(tuft,new(offset,height*0.5f,0.055f),new(0.055f,0.025f,height*0.55f),color);
                leaf.RotationDegrees=new(-28,offset<0?-35:35,0);
                if(stage>=3)
                    Mesh(tuft,new SphereMesh { Radius=0.055f,Height=0.18f,RadialSegments=6,Rings=3 },new(offset,height,0),color.Lightened(0.08f));
            }
        }
    }
}
