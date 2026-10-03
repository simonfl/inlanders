using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Collections.Generic;
public partial class Game
{
    private World? _riverTintWorld;
    private Cell[] _riverBanks=Array.Empty<Cell>();
    private readonly Dictionary<Vector2,Color> _riverTints=new();
    private Color RiverWaterTint(Vector3 at)
    {
        if(_riverTintWorld!=_world)
        {
            _riverTintWorld=_world;_riverTints.Clear();
            var steps=new[]{new Cell(1,0),new Cell(-1,0),new Cell(0,1),new Cell(0,-1)};
            _riverBanks=_world.Map.Land.Where(c=>steps.Any(d=>_world.Map.Water.Contains(new(c.X+d.X,c.Z+d.Z)))).ToArray();
        }
        var key=new Vector2(at.X,at.Z);if(_riverTints.TryGetValue(key,out var saved))return saved;
        // Visual depth only: boats and placement still use exact saved water cells.
        float distance=_riverBanks.Length==0?5:_riverBanks.Min(c=>MathF.Sqrt(MathF.Pow(Math.Max(0,Math.Abs(at.X-c.X)-.5f),2)+MathF.Pow(Math.Max(0,Math.Abs(at.Z-c.Z)-.5f),2)));
        float deep=Math.Clamp(distance/4,0,1);float flow=MathF.Sin(at.Z*.35f+at.X*.18f)*.012f;
        var color=new Color("8b9c8b").Lerp(new("4f727a"),deep).Lightened(flow);_riverTints[key]=color;return color;
    }
    private void RiverWaterTriangle(SurfaceTool surface,Vector3 a,Vector3 b,Vector3 c)
    {
        surface.SetNormal(Vector3.Up);foreach(var at in new[]{a,c,b}){surface.SetColor(RiverWaterTint(at));surface.AddVertex(at);}
    }
}
