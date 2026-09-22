using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Collections.Generic;
public partial class Game
{
    private bool _plainFarmstead;
    private Node3D? _workedLand;
    private World? _workedWorld;
    private string _workedKey="";
    private float _nextWorkedLand;
    private void UpdateWorkedLandscape(bool force=false)
    {
        if(!force && _uiTime<_nextWorkedLand)return;_nextWorkedLand=_uiTime+.25f;
        if(_workedLand==null){_workedLand=new(){Name="WorkedLandscape"};AddChild(_workedLand);}
        _workedLand.Visible=_world.PublicPlace!=null && !_plainFarmstead;
        if(!_workedLand.Visible)return;
        string key=_world.PathsRevision+":"+string.Join(';',_world.Cottages.Select(c=>$"{c.Id}:{c.Cell}:{c.Rotation}:{c.Complete}:{c.Improved}:{c.YardSide}"))+":"+
            string.Join(';',_world.Trees.Where(t=>!t.Felled && !t.NeedsPlanting).Select(t=>$"{t.Cell}:{t.Growth>=1}"))+":"+string.Join(';',_world.Commons?.Places??Array.Empty<Cell>());
        if(!force && _workedWorld==_world && key==_workedKey)return;
        _workedWorld=_world;_workedKey=key;Clear(_workedLand);
        var fields=_world.Cottages.Where(c=>c.Complete && (World.IsVegetablePlot(c.Kind) || c.Kind is BuildingKind.Farm or BuildingKind.Orchard))
            .SelectMany(c=>World.Footprint(c.Cell,c.Rotation,c.Kind)).ToHashSet();
        var homes=_world.Cottages.Where(c=>c.Complete && Buildings.Get(c.Kind).Beds>0).SelectMany(c=>World.Footprint(c.Cell,c.Rotation,c.Kind)).ToArray();
        var yards=_world.Cottages.SelectMany(c=>_world.HomeYardPlaces(c)).Concat(_world.Commons?.Places??Array.Empty<Cell>()).ToArray();
        var trees=_world.Trees.Where(t=>!t.Felled && !t.NeedsPlanting && t.Growth>=1).Select(t=>t.Cell).ToArray();
        var paths=_world.Paths.ToArray();var water=_world.Map.Water.ToHashSet();
        float Distance(float x,float z,IEnumerable<Cell> cells,float extent=0)
        {
            float nearest=1000;
            foreach(var c in cells){float dx=Math.Max(0,Math.Abs(x-c.X)-extent),dz=Math.Max(0,Math.Abs(z-c.Z)-extent);nearest=Math.Min(nearest,dx*dx+dz*dz);}
            return MathF.Sqrt(nearest);
        }
        var colors=new Dictionary<(int,int),Color>();
        Color Tint(float x,float z)
        {
            var sample=((int)MathF.Round(x*2),(int)MathF.Round(z*2));if(colors.TryGetValue(sample,out var saved))return saved;
            float mottling=MathF.Sin(x*1.37f+MathF.Sin(z*.72f))*.035f+MathF.Cos(z*1.91f-x*.34f)*.02f;
            Color color=new Color("818353").Lightened(mottling);
            float forest=Math.Clamp(1-Distance(x,z,trees)/2.8f,0,1);
            color=color.Lerp(new Color("555d3c").Lightened(mottling),forest*.78f);
            float domestic=Math.Clamp(1-Distance(x,z,homes,.5f)/1.7f,0,1);
            color=color.Lerp(new Color("968363").Lightened(mottling),domestic*.9f);
            float yard=Math.Clamp(1-Distance(x,z,yards,.35f)/.8f,0,1);
            color=color.Lerp(new("a38e69"),yard*.85f);
            // Shore treatment stays on actual land; no visual crossing or productive land is invented.
            float bank=Math.Clamp(1-Distance(x,z,water,.5f)/1.15f,0,1);
            color=color.Lerp(new Color("a59976").Lightened(mottling),bank*.8f);
            float field=Distance(x,z,fields,.5f);
            if(field<.01f)color=new Color("756448").Lightened(mottling);
            else color=color.Lerp(new Color("958464"),Math.Clamp(1-field/.55f,0,1)*.65f);
            float path=Math.Clamp(1-Distance(x,z,paths)/.8f,0,1);
            color=color.Lerp(new("9b8562"),path*.65f);
            colors[sample]=color;return color;
        }
        using var surface=new SurfaceTool();surface.Begin(Godot.Mesh.PrimitiveType.Triangles);
        void Tri(Vector3 a,Vector3 b,Vector3 c)
        {
            surface.SetNormal((b-a).Cross(c-a).Normalized());
            foreach(var at in new[]{a,c,b}){surface.SetColor(Tint(at.X,at.Z));surface.AddVertex(at);}
        }
        foreach(var cell in _world.Map.Land)for(int z=0;z<2;z++)for(int x=0;x<2;x++)
        {
            float left=cell.X-.5f+x*.5f,back=cell.Z-.5f+z*.5f;
            var a=OnGround(left,back,.014f);var b=OnGround(left+.5f,back,.014f);var c=OnGround(left+.5f,back+.5f,.014f);var d=OnGround(left,back+.5f,.014f);
            Tri(a,d,c);Tri(a,c,b);
        }
        SurfaceMesh(_workedLand,surface).Name="LandUseSurface";
    }
}
