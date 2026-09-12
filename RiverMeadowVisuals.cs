using Godot;
using Inlanders.Simulation;
using System;

public partial class Game
{
    private Color MeadowGroundTint(float x,float z)
    {
        float patches=MathF.Sin(x*.39f+MathF.Sin(z*.32f))*MathF.Cos(z*.29f)*.035f;
        float shore=MathF.Exp(-MathF.Abs(x-6)*.55f)*.035f;
        return new(.42f+patches-shore,.50f+patches,.30f+patches-shore*.3f);
    }
    private void MeadowTriangle(SurfaceTool surface,Vector3 a,Vector3 b,Vector3 c)
    {
        surface.SetNormal((b-a).Cross(c-a).Normalized());
        foreach(var vertex in new[]{a,c,b}){surface.SetColor(MeadowGroundTint(vertex.X,vertex.Z));surface.AddVertex(vertex);}
    }
    private void MakeRiverMeadowWater()
    {
        var water=new Node3D{Name="RiverMeadowWater"};_landscape.AddChild(water);
        var directions=new[]{new Cell(1,0),new(-1,0),new(0,1),new(0,-1)};
        foreach(var cell in _world.Map.Water)
        {
            int hash=Math.Abs(cell.X*31+cell.Z*17);
            Box(water,new(cell.X,-.5f,cell.Z),new(1,.8f,1),new("687d73"));
            Box(water,new(cell.X,-.11f,cell.Z),new(1,.06f,1),new("668e96"));
            if(hash%3==0)
            {
                var ripple=Box(water,new(cell.X-.09f,-.075f,cell.Z+.1f),new(.24f+hash%4*.045f,.008f,.018f),new("a1babc"));
                ripple.RotationDegrees=new(0,12+hash%17,0);
            }
            foreach(var d in directions)
            {
                var bank=new Cell(cell.X+d.X,cell.Z+d.Z);
                if(!_world.Map.Contains(bank) || _world.Map.Water.Contains(bank))continue;
                // Short earth shelves interrupt the old continuous bright outline. All stay in water cells.
                float length=.60f+hash%3*.10f;
                Box(water,new(cell.X+d.X*.475f,-.035f,cell.Z+d.Z*.475f),new(d.X==0?length:.05f,.06f,d.Z==0?length:.05f),new("929272"));
                if(cell.X==6 || hash%3!=0)continue; // Keep the narrow crossing channel open visually.
                var at=new Vector3(cell.X+d.X*.32f,-.04f,cell.Z+d.Z*.32f);
                for(int i=0;i<3;i++)
                {
                    float height=.16f+i*.055f;
                    var reed=Box(water,at+new Vector3((i-1)*.055f,height/2,i*.035f),new(.024f,height,.03f),new(i==1?"7d8a55":"65784b"));
                    reed.RotationDegrees=new(i*6,hash%90,(i-1)*13);
                }
                var pebble=Mesh(water,new SphereMesh{Radius=.09f,Height=.10f,RadialSegments=5,Rings=2},at+new Vector3(.13f,0,-.08f),new("929082"));
                pebble.Scale=new(1.2f,.75f,.8f);
            }
        }
        BatchStaticGeometry(water);
    }
}
