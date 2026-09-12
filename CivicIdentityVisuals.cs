using Godot;
using Inlanders.Simulation;
using System;

public partial class Game
{
    private void MakeCivicVenue(Node3D parent,int stage,CivicIdentity identity)
    {
        parent.SetMeta("civic_identity",(int)identity);
        if(identity==CivicIdentity.Hall){MakeGatheringHall(parent,stage);return;}
        if(identity==CivicIdentity.Chapel)MakeChapel(parent,stage);else MakePlantedCourt(parent,stage);
    }
    private void ChapelArch(Node3D parent,Vector3 at,float width,float height)
    {
        Box(parent,at+new Vector3(0,height*.42f,0),new(width*.72f,height*.84f,.055f),_recess);
        var round=Mesh(parent,new SphereMesh{Radius=width*.36f,Height=width*.72f,RadialSegments=12,Rings=6},at+new Vector3(0,height*.84f,0),_recess);round.Scale=new(1,1,.13f);
        foreach(float x in new[]{-width*.48f,width*.48f})Box(parent,at+new Vector3(x,height*.42f,.025f),new(width*.17f,height*.84f,.16f),_stone.Lightened(.18f));
        for(int i=0;i<9;i++)
        {
            float a=i*Mathf.Pi/8;
            var piece=Box(parent,at+new Vector3(MathF.Cos(a)*width*.48f,height*.84f+MathF.Sin(a)*width*.48f,.025f),new(width*.22f,width*.18f,.16f),_stone.Lightened(i%2==0?.12f:.22f));piece.RotationDegrees=new(0,0,a*180/Mathf.Pi);
        }
    }
    private void MakeChapel(Node3D parent,int stage)
    {
        foreach(float x in new[]{-1.02f,1.02f})foreach(float z in new[]{-.70f,.65f})StoneFoot(parent,new(x,.16f,z),new(.46f,.30f,.46f));
        StoneFoot(parent,new(0,.075f,.86f),new(.98f,.12f,.38f));
        if(stage==0)return;
        foreach(float x in new[]{-1.02f,1.02f})
        {
            foreach(float z in new[]{-.70f,.65f})Box(parent,new(x,1.05f,z),new(.16f,1.85f,.16f),_frameTimber);
            Box(parent,new(x,1.97f,-.03f),new(.18f,.18f,1.70f),_frameTimber);
        }
        foreach(float z in new[]{-.70f,.65f})Box(parent,new(0,1.97f,z),new(2.22f,.18f,.18f),_frameTimber);
        if(stage==1)return;
        var plaster=new Color("e5d4b5");
        foreach(float x in new[]{-1.02f,1.02f})Box(parent,new(x,1.02f,-.03f),new(.18f,1.74f,1.48f),plaster);
        foreach(float z in new[]{-.70f,.65f})Box(parent,new(0,1.02f,z),new(1.98f,1.74f,.18f),plaster);
        foreach(float x in new[]{-1.07f,1.07f})foreach(float z in new[]{-.70f,.65f})
        {StoneFoot(parent,new(x,.47f,z),new(.33f,.86f,.35f));Box(parent,new(x,.96f,z),new(.25f,.16f,.27f),_stone.Lightened(.16f));}
        ChapelArch(parent,new(0,.13f,.765f),.87f,1.25f);
        foreach(float side in new[]{-1f,1f})
        {
            var window=new Node3D{Position=new(side*1.13f,.75f,-.05f),RotationDegrees=new(0,side*90,0)};parent.AddChild(window);ChapelArch(window,Vector3.Zero,.48f,.63f);
            Box(window,new(0,.34f,.04f),new(.035f,.53f,.06f),new("bba46f"));
        }
        CottageWindow(parent,new(0,1.02f,-.82f),180,false);
        if(stage==2)return;
        var roof=new Node3D{Position=new(0,2.02f,-.03f),RotationDegrees=new(0,90,0)};parent.AddChild(roof);
        VillageRoof(roof,Vector3.Zero,2.02f,2.73f,.93f,new("9b6655"),true,plaster);
        foreach(float x in new[]{-.25f,.25f})Box(parent,new(x,3.32f,.49f),new(.14f,.90f,.24f),_stone.Lightened(.2f));
        Box(parent,new(0,3.77f,.49f),new(.65f,.13f,.30f),_stone.Lightened(.22f));
        Cylinder(parent,new(0,3.42f,.49f),.15f,.18f,new("bca16a"),.065f);
        Box(parent,new(0,3.60f,.49f),new(.04f,.20f,.04f),_frameTimber);
        VillageRoof(parent,new(0,3.83f,.49f),.80f,.59f,.30f,new("9b6655"),false);
        FoodSign(parent,"CHAPEL · GATHERING HALL",4.45f);
    }
    private void MakePlantedCourt(Node3D parent,int stage)
    {
        foreach(float x in new[]{-1.16f,1.16f})StoneFoot(parent,new(x,.12f,-.03f),new(.30f,.20f,1.60f));
        StoneFoot(parent,new(0,.12f,-.77f),new(2.58f,.20f,.30f));
        foreach(float x in new[]{-.30f,.30f})StoneFoot(parent,new(x,.045f,.80f),new(.49f,.06f,.30f));
        if(stage==0)return;
        foreach(float x in new[]{-1.16f,1.16f})
        {
            Box(parent,new(x,.35f,-.03f),new(.28f,.35f,1.60f),_stone.Lightened(.13f));
            foreach(float z in new[]{-.72f,.65f})StoneFoot(parent,new(x,.44f,z),new(.40f,.65f,.40f));
        }
        Box(parent,new(0,.35f,-.77f),new(2.24f,.35f,.28f),_stone.Lightened(.13f));
        foreach(float x in new[]{.28f,1.02f})foreach(float z in new[]{-.60f,.44f})Box(parent,new(x,.93f,z),new(.10f,1.66f,.10f),_frameTimber);
        if(stage==1)return;
        foreach(float x in new[]{-1.16f,1.16f})Box(parent,new(x,.56f,-.03f),new(.36f,.10f,1.64f),new("c8bda1"));
        Box(parent,new(0,.56f,-.77f),new(2.60f,.10f,.35f),new("c8bda1"));
        // Empty beds before the final planting stage; no permanent visitor props.
        Box(parent,new(-.63f,.13f,-.14f),new(.75f,.17f,.85f),new("766247"));
        foreach(float z in new[]{-.65f,.49f})Box(parent,new(.65f,1.78f,z),new(1.0f,.14f,.14f),_frameTimber);
        if(stage==2)return;
        for(int i=0;i<7;i++)Box(parent,new(.65f,1.86f,-.69f+i*.20f),new(1.13f,.09f,.10f),new("b79968"));
        Cylinder(parent,new(-.63f,.77f,-.14f),.07f,1.22f,_wood,.04f);
        foreach(var at in new[]{new Vector3(-.63f,1.63f,-.14f),new(-.95f,1.43f,-.12f),new(-.35f,1.46f,-.24f)})
            Mesh(parent,new SphereMesh{Radius=.40f,Height=.64f,RadialSegments=8,Rings=5},at,new("8aa27b"));
        for(int i=0;i<5;i++)
        {
            var at=new Vector3(-.95f+i*.18f,.25f,-.53f);
            Mesh(parent,new SphereMesh{Radius=.11f,Height=.17f,RadialSegments=6,Rings=3},at,new(i%2==0?"cba8aa":"88a178"));
        }
        FoodSign(parent,"PLANTED COURT · GATHERING HALL",2.35f);
    }
}
