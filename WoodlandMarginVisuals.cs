using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;

public partial class Game
{
    private void MakeWoodlandMargin(Node3D root,Cell[] trees)
    {
        if(trees.Length==0)return;
        var occupied=_world.Cottages.SelectMany(World.Footprint)
            .Concat(_world.Cottages.SelectMany(c=>_world.HomeYardPlaces(c)))
            .Concat(_world.Paths).Concat(_world.SharedPlaces.SelectMany(c=>c.Places.Append(c.Center)))
            .Concat(_world.ReadGroundUse(true).Where(m=>m.Visits>=3).Select(m=>m.Cell)).ToHashSet();
        using var surface=new SurfaceTool();surface.Begin(Godot.Mesh.PrimitiveType.Triangles);
        int count=0;
        foreach(var cell in _world.Map.Land)
        {
            if(occupied.Contains(cell) || (cell.Point-_world.Stockpile.Point).LengthSquared()<5)continue;
            float distance=trees.Min(t=>(t.Point-cell.Point).LengthSquared());
            if(distance>5.5f)continue;
            uint seed=unchecked((uint)(cell.X*73856093 ^ cell.Z*19349663));
            if(seed%100>90-distance*13)continue;
            for(int plant=0;plant<2;plant++)
            {
                float x=cell.X+MathF.Sin(seed%127+plant*2.3f)*.13f;
                float z=cell.Z+MathF.Cos(seed%113+plant*3.7f)*.13f;
                float height=.14f+(seed%7)*.023f;
                var basePoint=OnGround(x,z,.026f);
                var color=new Color("647549").Lightened((seed%5)*.015f);
                for(int leaf=0;leaf<5;leaf++)
                {
                    float angle=leaf*Mathf.Tau/5+seed%11;
                    var direction=new Vector3(MathF.Cos(angle),0,MathF.Sin(angle));
                    var side=new Vector3(-direction.Z,0,direction.X)*.07f;
                    var middle=basePoint+direction*.15f+Vector3.Up*height;
                    var tip=basePoint+direction*.35f+Vector3.Up*height*.65f;
                    Triangle(surface,basePoint,middle+side,tip,color);
                    Triangle(surface,basePoint,tip,middle-side,color.Lightened(.05f));
                    Triangle(surface,basePoint,tip,middle+side,color);
                    Triangle(surface,basePoint,middle-side,tip,color.Lightened(.05f));
                }
                count++;
            }
        }
        if(count>0)SurfaceMesh(root,surface).Name="WoodlandUnderstory";
    }
}
