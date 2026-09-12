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
        foreach(float x in new[]{-.36f,.36f})foreach(float z in new[]{.29f,-.92f})ShorePile(parent,x,z,.36f);
        foreach(float x in new[]{-.36f,.36f})
        {
            Box(parent,new(x,.12f,-.30f),new(.15f,.18f,1.54f),_frameTimber);
            TimberBeam(parent,new(x,-.24f,-.90f),new(x,.13f,.25f),.09f,_frameTimber);
        }
        if(stage==0)return;
        for(int i=0;i<(stage==1?4:8);i++)
            Box(parent,new(0,.23f,.35f-i*.18f),new(.89f,.10f,.166f),new Color("b69a70").Lightened((i%3)*.035f));
        if(stage==1)return;
        foreach(float x in new[]{-.37f,.37f})
        {
            Box(parent,new(x,.88f,.24f),new(.10f,1.25f,.10f),_frameTimber);
            TimberBeam(parent,new(x,1.19f,.24f),new(x,1.46f,-.14f),.07f,_wood);
        }
        Box(parent,new(0,1.49f,.24f),new(.95f,.12f,.12f),_frameTimber);
        if(stage==2)return;
        // Canvas sits above the shore end; net and rope stay to the side of the landing.
        for(int panel=0;panel<5;panel++)
        {
            var canvas=Box(parent,new(-.408f+panel*.204f,1.53f,.05f),new(.205f,.065f,.80f),panel%2==0?new("6f958b"):new("bbc4a0"));
            canvas.RotationDegrees=new(-12,0,0);
        }
        Box(parent,new(0,1.37f,-.34f),new(1.03f,.13f,.06f),new("6f958b"));
        for(int line=0;line<5;line++)Box(parent,new(.425f,.90f,-.12f+line*.085f),new(.014f,.48f,.016f),new("c8bb91"));
        for(int line=0;line<5;line++)Box(parent,new(.425f,.68f+line*.11f,.05f),new(.016f,.014f,.40f),new("c8bb91"));
        foreach(float x in new[]{-.36f,.36f})Box(parent,new(x,.36f,-.48f),new(.15f,.07f,.07f),new("c4ad7c"));
        for(int i=0;i<10;i++)
        {
            float a=i*Mathf.Tau/10,b=(i+1)*Mathf.Tau/10;
            TimberBeam(parent,new(-.26f+MathF.Cos(a)*.105f,.30f,-.33f+MathF.Sin(a)*.12f),new(-.26f+MathF.Cos(b)*.105f,.30f,-.33f+MathF.Sin(b)*.12f),.025f,new("c8bb91"));
        }
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
