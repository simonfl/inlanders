using Godot;
using Inlanders.Simulation;

public partial class Game
{
    private void MakeHabitantHome(Node3D root,int stage,int variant,CottageFinish finish)
    {
        // A low, broad dwelling leaves people and worked ground visible beside it.
        var palette=CottagePalette(finish==CottageFinish.Automatic?(CottageFinish)(1+variant):finish);
        if(finish==CottageFinish.Automatic)palette=(new Color("827f6b").Lightened(variant*.04f),new Color("cabb98"));
        var timber=new Color("8b7658");
        StoneFoot(root,new(0,.10f,-.14f),new(2.45f,.18f,1.46f));
        StoneFoot(root,new(-.48f,.07f,.72f),new(.7f,.12f,.42f));
        if(stage<1)return;
        foreach(float x in new[]{-1.14f,1.14f})foreach(float z in new[]{-.80f,.53f})
            Box(root,new(x,.73f,z),new(.14f,1.30f,.14f),_frameTimber);
        foreach(float z in new[]{-.80f,.53f})Box(root,new(0,1.34f,z),new(2.44f,.14f,.14f),_frameTimber);
        if(stage<2)return;
        Box(root,new(0,.73f,-.14f),new(2.25f,1.16f,1.30f),palette.Wall.Darkened(.16f));
        // Shallow timber courses give the walls depth without an oversized porch.
        for(int i=0;i<5;i++)foreach(float z in new[]{-.805f,.525f})
            Box(root,new(0,.25f+i*.23f,z),new(2.25f,.045f,.045f),timber);
        Box(root,new(-.48f,.63f,.57f),new(.57f,1.02f,.075f),_recess);
        Box(root,new(-.48f,.60f,.615f),new(.43f,.90f,.04f),_frameTimber);
        Box(root,new(-.48f,1.18f,.60f),new(.76f,.14f,.14f),timber);
        CottageWindow(root,new(.62f,.81f,.55f));
        CottageWindow(root,new(-1.15f,.82f,-.18f),-90,false);
        CottageWindow(root,new(1.15f,.82f,-.18f),90,false);
        CottageWindow(root,new(.40f,.81f,-.82f),180,false);
        if(stage<3)return;
        VillageRoof(root,new(0,1.41f,-.14f),2.70f,1.89f,.76f,palette.Roof.Darkened(.06f),true,timber);
        // A masonry chimney and roof battens remain recognizable at village scale.
        Box(root,new(.74f,1.95f,-.38f),new(.37f,1.05f,.39f),_stone.Darkened(.08f));
        Box(root,new(.74f,2.50f,-.38f),new(.47f,.12f,.49f),_stone);
        Box(root,new(.74f,2.565f,-.38f),new(.23f,.015f,.25f),_recess);
        foreach(float x in new[]{-.95f,-.45f,.05f,.55f,1.05f})
            foreach(float side in new[]{-1f,1f})
                TimberBeam(root,new(x,1.44f,-.14f+side*.94f),new(x,2.19f,-.14f),.035f,palette.Roof.Lightened(.13f));
    }

    private bool FarmsteadWater(float x,float z)=>_world.Founding?.RiverFarmstead==true &&
        x>=8.5f+(_world.Founding?.TransformationHamlet==true?0:z< -5.5f?1:z>6.5f?-1:0);

    private void MakeFarmsteadRiverContext(int margin)
    {
        if(_world.Founding?.RiverFarmstead!=true)return;
        var map=_world.Map;
        using var water=new SurfaceTool();water.Begin(Godot.Mesh.PrimitiveType.Triangles);
        for(int z=map.MinZ-margin;z<=map.MaxZ+margin;z++)
            for(int x=map.MinX;x<=map.MaxX+margin;x++)
            {
                if(map.Contains(new(x,z)) || !FarmsteadWater(x,z))continue;
                var a=new Vector3(x-.5f,-.08f,z-.5f);var b=new Vector3(x+.5f,-.08f,z-.5f);
                var c=new Vector3(x+.5f,-.08f,z+.5f);var d=new Vector3(x-.5f,-.08f,z+.5f);
                Triangle(water,a,d,c,new("668e96"));Triangle(water,a,c,b,new("668e96"));
            }
        SurfaceMesh(_landscape,water).Name="RiverBeyondSettlement";
    }
}
