using Godot;
using System;

public partial class Game
{
    private void ShorePile(Node3D parent,float x,float z,float top)
    {
        Cylinder(parent,new(x,(top-.42f)/2,z),.085f,top+.42f,new("796951"));
        Cylinder(parent,new(x,top-.055f,z),.098f,.06f,new("c4ad7c"));
    }
    private void MakeFishingDock(Node3D parent,int stage)
    {
        // Three actual shore cells: covered gear bay, clear gangway, catch-handling bay.
        foreach(float x in new[]{-1.34f,-.36f,.36f,1.34f})foreach(float z in new[]{-.36f,.36f})ShorePile(parent,x,z,.22f);
        foreach(float x in new[]{-.36f,.36f})ShorePile(parent,x,-1.05f,.24f);
        if(stage==0)return;
        for(int i=0;i<(stage==1?4:8);i++)
            Box(parent,new(0,.19f,-.35f+i*.10f),new(2.87f,.11f,.092f),new Color("9f8561").Lightened(i%3*.035f));
        for(int i=0;i<5;i++)Box(parent,new(0,.19f,-.44f-i*.15f),new(.85f,.11f,.14f),new("b69a70"));
        if(stage==1)return;
        foreach(float x in new[]{-1.32f,-.50f})foreach(float z in new[]{-.32f,.32f})
            Box(parent,new(x,.84f,z),new(.11f,1.35f,.11f),_frameTimber);
        Box(parent,new(-.91f,1.49f,0),new(1.03f,.14f,.91f),_frameTimber);
        // The right-hand bay holds only actual stored catch (rendered separately).
        Box(parent,new(.96f,.42f,0),new(.76f,.40f,.64f),new("776448"));
        Box(parent,new(.96f,.65f,0),new(.88f,.09f,.72f),new("b6a17a"));
        if(stage==2)return;
        var roof=Box(parent,new(-.91f,1.60f,-.02f),new(1.15f,.13f,1.10f),new("647566"));roof.RotationDegrees=new(-14,0,0);
        for(int i=0;i<5;i++)Box(parent,new(-1.30f,.80f,-.26f+i*.13f),new(.025f,.63f,.022f),new("bdb18e"));
        for(int i=0;i<5;i++)Box(parent,new(-1.30f,.51f+i*.15f,0),new(.024f,.024f,.55f),new("bdb18e"));
        foreach(float z in new[]{-.22f,.22f})
            TimberBeam(parent,new(-1.13f,.28f,z),new(-.66f,1.18f,z),.055f,new("b69a70"));
        foreach(float x in new[]{-.38f,.38f})Box(parent,new(x,.29f,-.78f),new(.14f,.10f,.08f),new("c4ad7c"));
    }

    private void MakeBridge(Node3D parent,int stage)
    {
        foreach(float x in new[]{-.43f,.43f})
        {
            foreach(float z in new[]{-.79f,.79f})ShorePile(parent,x,z,.15f);
            Box(parent,new(x,-.065f,0),new(.17f,.21f,1.93f),_frameTimber);
        }
        if(stage==0)return;
        for(int i=0;i<(stage==1?5:10);i++)
            Box(parent,new(0,.04f,-.86f+i*.19f),new(1.01f,.095f,.177f),new Color("c0a37a").Lightened((i%3)*.027f));
        if(stage==1)return;
        foreach(float x in new[]{-.46f,.46f})
        {
            foreach(float z in new[]{-.81f,0,.81f})Box(parent,new(x,.31f,z),new(.11f,.57f,.11f),_frameTimber);
            Box(parent,new(x,.57f,0),new(.13f,.12f,1.96f),new("a88b63"));
            if(stage<3)continue;
            foreach(float z in new[]{-.8f,.02f})
            {
                TimberBeam(parent,new(x,.14f,z),new(x,.51f,z+.78f),.055f,_wood);
                TimberBeam(parent,new(x,.51f,z),new(x,.14f,z+.78f),.055f,_wood);
            }
            foreach(float z in new[]{-.81f,0,.81f})Box(parent,new(x,.66f,z),new(.16f,.065f,.16f),new("cab38b"));
        }
    }
}
