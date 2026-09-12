using Godot;
using System;

public partial class Game
{
    private void MakeGatheringHall(Node3D parent,int stage)
    {
        // A public gable and deep entrance, with no assigned seats inside blocked land.
        foreach(float x in new[]{-1.16f,1.16f})
        {
            StoneFoot(parent,new(x,.16f,-.08f),new(.40f,.27f,1.55f));
            foreach(float z in new[]{-.72f,.65f})StoneFoot(parent,new(x,.20f,z),new(.52f,.34f,.46f));
        }
        foreach(float x in new[]{-.46f,.12f,.70f})
            StoneFoot(parent,new(x,.075f,.80f),new(.51f,.10f,.38f));
        if(stage==0)return;
        foreach(float x in new[]{-1.16f,1.16f})
        {
            Box(parent,new(x,.79f,-.08f),new(.32f,1.16f,1.48f),_stone.Lightened(.06f));
            foreach(float z in new[]{-.72f,.65f})
            {
                Box(parent,new(x,1.21f,z),new(.24f,1.90f,.24f),_frameTimber);
                StoneFoot(parent,new(x,.51f,z),new(.42f,.75f,.39f));
            }
            Box(parent,new(x,2.12f,-.06f),new(.26f,.24f,1.88f),_frameTimber);
            TimberBeam(parent,new(x,1.63f,.65f),new(x,2.12f,.14f),.16f,_frameTimber);
        }
        Box(parent,new(0,2.12f,.66f),new(2.60f,.25f,.27f),_frameTimber);
        Box(parent,new(0,2.12f,-.75f),new(2.60f,.25f,.27f),_frameTimber);
        if(stage==1)return;
        Box(parent,new(0,1.22f,-.75f),new(2.14f,1.75f,.18f),new("c8bda0"));
        // Recessed side lights and exposed sills give the walls depth on every rotation.
        foreach(float side in new[]{-1f,1f})
        {
            var light=new Node3D{Position=new(side*1.34f,1.54f,-.05f),RotationDegrees=new(0,side*90,0)};parent.AddChild(light);
            Box(light,Vector3.Zero,new(.73f,.60f,.07f),_recess);
            foreach(float x in new[]{-.4f,.4f})Box(light,new(x,0,.03f),new(.12f,.79f,.18f),_frameTimber);
            Box(light,new(0,-.36f,.09f),new(.96f,.14f,.28f),_stone.Lightened(.15f));
            foreach(float x in new[]{-.20f,.20f})Box(light,new(x,0,.055f),new(.055f,.60f,.08f),new("c7ad74"));
        }
        CottageWindow(parent,new(0,1.37f,-.87f),180,false);
        foreach(float x in new[]{-.91f,.91f})
        {
            Box(parent,new(x,1.10f,.69f),new(.24f,1.36f,.28f),_stone.Lightened(.08f));
            Box(parent,new(x,1.94f,.84f),new(.24f,.55f,.055f),new("bd8053"));
            Box(parent,new(x,2.21f,.84f),new(.32f,.075f,.09f),_frameTimber);
        }
        for(int i=0;i<9;i++)
        {
            float angle=i*Mathf.Pi/8;
            var stone=Box(parent,new(MathF.Cos(angle)*.78f,1.35f+MathF.Sin(angle)*.58f,.72f),new(.30f,.23f,.30f),_stone.Lightened(i%2*.08f));
            stone.Rotation=new(0,0,angle);
        }
        if(stage==2)return;
        var roof=new Node3D{Position=new(0,2.25f,-.02f),RotationDegrees=new(0,90,0)};parent.AddChild(roof);
        VillageRoof(roof,Vector3.Zero,2.22f,3.03f,.92f,new("537779"),false);
        // Front and rear gables remain distinct from the cottage's side-facing ridge.
        foreach(float side in new[]{-1f,1f})
        {
            using var gable=new SurfaceTool();gable.Begin(Godot.Mesh.PrimitiveType.Triangles);
            gable.AddVertex(new(-1.3f,2.19f,side*.88f));gable.AddVertex(new(1.3f,2.19f,side*.88f));gable.AddVertex(new(0,3.05f,side*.88f));
            gable.GenerateNormals();var face=Mesh(parent,gable.Commit(),Vector3.Zero,new("d0c5a3"));
            ((StandardMaterial3D)face.MaterialOverride).CullMode=BaseMaterial3D.CullModeEnum.Disabled;
            TimberBeam(parent,new(0,2.21f,side*.94f),new(0,3.05f,side*.94f),.14f,_frameTimber);
            foreach(float x in new[]{-.8f,.8f})TimberBeam(parent,new(x,2.22f,side*.94f),new(0,2.89f,side*.94f),.10f,_frameTimber);
        }
        // Low roof lantern: a civic silhouette, without implying bells or a new service.
        Box(parent,new(0,3.23f,-.12f),new(.62f,.40f,.68f),_recess);
        foreach(float x in new[]{-.33f,.33f})foreach(float z in new[]{-.48f,.24f})
            Box(parent,new(x,3.24f,z),new(.10f,.44f,.10f),_frameTimber);
        foreach(float y in new[]{3.15f,3.27f})Box(parent,new(0,y,.25f),new(.60f,.045f,.06f),new("c6ad7c"));
        VillageRoof(parent,new(0,3.46f,-.12f),.91f,1.04f,.24f,new("537779"),false);
        FoodSign(parent,"GATHERING HALL",4.05f);
    }
}
