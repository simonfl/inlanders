using System;
using Godot;
using Inlanders.Simulation;

public partial class Game
{
    private void MakeHabitantHome(Node3D root,int stage,int variant,CottageFinish finish)
    {
        // A small family of one-room timber houses, including an expanded room.
        // Stable building-id variants change massing, not beds, cost or footprint.
        bool compact=variant==1,expanded=variant==2;
        float width=compact?1.94f:expanded?1.82f:2.56f,depth=compact?1.63f:1.36f;
        float cx=expanded?-.37f:0,cz=-.14f,wall=compact?1.45f:1.19f,rise=compact?1.08f:.79f;
        float front=cz+depth/2,back=cz-depth/2;
        var palette=CottagePalette(finish==CottageFinish.Automatic?(CottageFinish)(1+variant):finish);
        if(finish==CottageFinish.Automatic)palette=(new Color(compact?"93866a":expanded?"756e58":"797965"),new Color(compact?"c6b28e":"a99471"));
        var timber=new Color("786346");
        StoneFoot(root,new(cx,.10f,cz),new(width+.12f,.18f,depth+.10f));
        StoneFoot(root,new(-.38f,.07f,front+.18f),new(.72f,.12f,.34f));
        if(expanded)StoneFoot(root,new(.96f,.10f,cz),new(.72f,.18f,1.43f));
        if(stage<1)return;
        foreach(float x in new[]{cx-width/2,cx+width/2})foreach(float z in new[]{back,front})
            Box(root,new(x,wall/2+.12f,z),new(.13f,wall,.13f),_frameTimber);
        foreach(float z in new[]{back,front})Box(root,new(cx,wall+.10f,z),new(width+.14f,.14f,.13f),_frameTimber);
        if(expanded)foreach(float z in new[]{back,front})Box(root,new(1.27f,.60f,z),new(.12f,1.06f,.12f),_frameTimber);
        if(stage<2)return;
        Box(root,new(cx,wall/2+.12f,cz),new(width,wall,depth),palette.Wall.Darkened(.09f));
        if(compact)
        {
            foreach(float x in new[]{-.91f,-.70f,-.12f,.26f,.91f})foreach(float z in new[]{back-.02f,front+.02f})
                Box(root,new(x,wall/2+.12f,z),new(.095f,wall,.08f),timber);
        }
        else for(int i=0;i<6;i++)foreach(float z in new[]{back-.025f,front+.025f})
            Box(root,new(cx,.24f+i*.18f,z),new(width+.10f,.065f,.075f),i%2==0?timber:timber.Lightened(.10f));
        Box(root,new(-.38f,.63f,front+.035f),new(.55f,1.02f,.075f),_recess);
        Box(root,new(-.38f,.60f,front+.08f),new(.41f,.90f,.04f),_frameTimber);
        Box(root,new(-.38f,1.18f,front+.06f),new(.69f,.12f,.12f),timber);
        CottageWindow(root,new(compact?.49f:.40f,.79f,front+.035f));
        CottageWindow(root,new(cx-width/2-.025f,.80f,cz),-90,false);
        CottageWindow(root,new(cx+.30f,.79f,back-.025f),180,false);
        if(expanded)
        {
            Box(root,new(.97f,.63f,cz),new(.66f,1.08f,1.36f),palette.Wall.Darkened(.2f));
            for(int i=0;i<6;i++)Box(root,new(.97f,.22f+i*.18f,front+.025f),new(.70f,.06f,.075f),timber);
        }
        if(stage<3)return;
        VillageRoof(root,new(cx,wall+.17f,cz),width+.30f,depth+.34f,rise,palette.Roof,true,compact?palette.Wall:timber);
        if(expanded)VillageRoof(root,new(.98f,1.23f,cz),.91f,1.70f,.40f,palette.Roof.Darkened(.08f),true,timber);
        float chimneyX=compact?.61f:cx-width*.36f,chimneyZ=cz-.21f,top=wall+rise+.43f;
        Box(root,new(chimneyX,top-.43f,chimneyZ),new(.36f,.93f,.38f),_stone.Darkened(.08f));
        Box(root,new(chimneyX,top+.06f,chimneyZ),new(.44f,.12f,.46f),_stone);
        Box(root,new(chimneyX,top+.125f,chimneyZ),new(.22f,.015f,.24f),_recess);
        // Restrained roof courses read as a material surface, not painted stripes.
        for(int i=1;i<5;i++)foreach(float side in new[]{-1f,1f})
        {
            float t=i/5f;Box(root,new(cx,wall+.18f+rise*(1-t),cz+side*(depth+.34f)/2*t),new(width+.30f,.022f,.025f),palette.Roof.Darkened(.08f));
        }
    }

    private bool FarmsteadWater(float x,float z)
    {
        if(_world.Founding?.RiverFarmstead!=true)return false;
        var map=_world.Map;int row=Math.Clamp((int)MathF.Round(z),map.MinZ,map.MaxZ),edge=map.MaxX+1;
        while(edge>map.MinX && map.Water.Contains(new(edge-1,row)))edge--;
        return x>=edge-.5f;
    }

    private void MakeFarmsteadRiverContext(int margin)
    {
        if(_world.Founding?.RiverFarmstead!=true)return;
        MakeRiverBank();
        var map=_world.Map;
        // Low shingle and silt soften the waterline without creating pretend obstacles/resources.
        if(_world.PublicPlace==null || _plainFarmstead)foreach(var bank in map.Land)
        {
            if(!map.Water.Contains(new(bank.X+1,bank.Z)))continue;
            Box(_landscape,OnGround(bank.X+.38f,bank.Z,.016f),new(.23f,.026f,.94f),new("a69b74"));
            if((bank.Z+30)%3==0)foreach(float z in new[]{-.25f,.18f})
                Mesh(_landscape,new SphereMesh{Radius=.10f,Height=.065f,RadialSegments=5,Rings=2},OnGround(bank.X+.37f,bank.Z+z,.055f),new("989583"));
        }
        using var water=new SurfaceTool();water.Begin(Godot.Mesh.PrimitiveType.Triangles);
        for(int z=map.MinZ-margin;z<=map.MaxZ+margin;z++)
            for(int x=map.MinX;x<=map.MaxX+margin;x++)
            {
                if(map.Contains(new(x,z)) || !FarmsteadWater(x,z))continue;
                var a=new Vector3(x-.5f,-.08f,z-.5f);var b=new Vector3(x+.5f,-.08f,z-.5f);
                var c=new Vector3(x+.5f,-.08f,z+.5f);var d=new Vector3(x-.5f,-.08f,z+.5f);
                if(_world.PublicPlace!=null){RiverWaterTriangle(water,a,d,c);RiverWaterTriangle(water,a,c,b);}
                else {Triangle(water,a,d,c,new("668e96"));Triangle(water,a,c,b,new("668e96"));}
            }
        SurfaceMesh(_landscape,water).Name="RiverBeyondSettlement";
    }
}
