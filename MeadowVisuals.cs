using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;

public partial class Game
{
    private void MakeOpenMeadow(Node3D root,Cell[] trees)
    {
        var occupied=_world.Cottages.SelectMany(World.Footprint)
            .Concat(_world.Cottages.SelectMany(c=>_world.HomeYardPlaces(c)))
            .Concat(_world.Paths).Concat(_world.SharedPlaces.SelectMany(c=>c.Places.Append(c.Center)))
            .Concat(_world.ReadGroundUse(true).Where(m=>m.Visits>=3).Select(m=>m.Cell)).ToHashSet();
        var homes=_world.Cottages.Where(c=>Buildings.Get(c.Kind).Beds>0).SelectMany(World.Footprint).ToArray();
        using var surface=new SurfaceTool();surface.Begin(Godot.Mesh.PrimitiveType.Triangles);int count=0;
        foreach(var cell in _world.Map.Land)
        {
            if(occupied.Contains(cell) || (cell.Point-_world.Stockpile.Point).LengthSquared()<5 ||
                trees.Any(t=>(t.Point-cell.Point).LengthSquared()<6) || homes.Any(h=>(h.Point-cell.Point).LengthSquared()<3))continue;
            uint seed=unchecked((uint)(cell.X*53471161 ^ cell.Z*9717529));
            float patch=(MathF.Sin(cell.X*.37f+MathF.Sin(cell.Z*.31f))+MathF.Cos(cell.Z*.51f-cell.X*.12f)+2)/4;
            if(seed%100>patch*70)continue;
            var at=OnGround(cell.X+MathF.Sin(seed%83)*.22f,cell.Z+MathF.Cos(seed%67)*.22f,.026f);
            var color=new Color("7c8953").Lightened((seed%5)*.025f);
            for(int blade=0;blade<7;blade++)
            {
                float angle=blade*Mathf.Tau/7+seed%19;
                var direction=new Vector3(MathF.Cos(angle),0,MathF.Sin(angle));
                var side=new Vector3(-direction.Z,0,direction.X)*.022f;
                float height=.13f+(seed%7)*.022f;
                var foot=at+direction*.04f;var tip=at+direction*.22f+Vector3.Up*height;
                Triangle(surface,foot-side,foot+side,tip,color);
                Triangle(surface,foot+side,foot-side,tip,color);
            }
            count++;
        }
        if(count>0)SurfaceMesh(root,surface).Name="OpenMeadowGrass";
    }
}
