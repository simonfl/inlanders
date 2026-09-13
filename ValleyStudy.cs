using Godot;
using Inlanders.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Game
{
    private Node3D? _studyBoundary,_studyGround;
    private int _studyGroundKey;
    private void StorybookLandscapeContext()
    {
        var map=_world.Map;var land=map.Land.ToArray();
        using var surface=new SurfaceTool();surface.Begin(Godot.Mesh.PrimitiveType.Triangles);
        var corners=new Dictionary<Vector2,Vector3>();
        Vector3 Corner(float x,float z)
        {
            var key=new Vector2(x,z);if(corners.TryGetValue(key,out var cached))return cached;
            float distance=Math.Max(0,MathF.Sqrt(land.Min(c=>(c.X-x)*(c.X-x)+(c.Z-z)*(c.Z-z)))-.71f);
            float shoulder=Math.Min(1,distance/2.5f);
            float hills=(MathF.Sin(x*.24f+z*.11f)+MathF.Cos(z*.30f-x*.09f)+2)*.30f;
            float height=.01f-shoulder*.06f+hills*Math.Clamp((distance-2)/5,0,1);
            // Continue the stream beyond the playable crossings; no extra navigable water is implied.
            float stream=5+MathF.Sin(z*.13f)*Math.Clamp((MathF.Abs(z-1)-8)/5,0,1)*1.4f;
            if(z<-6 || z>8)
            {
                float width=.5f+.30f*Math.Clamp((MathF.Abs(z-1)-8)/4,0,1);
                height=Math.Min(height,-.11f+(MathF.Abs(x-stream)-width)*.30f);
            }
            return corners[key]=new(x,height,z);
        }
        for(int x=map.MinX-7;x<=map.MaxX+7;x++)for(int z=map.MinZ-7;z<=map.MaxZ+7;z++)
        {
            if(map.Contains(new(x,z)))continue;
            var a=Corner(x-.5f,z-.5f);var b=Corner(x+.5f,z-.5f);var c=Corner(x+.5f,z+.5f);var d=Corner(x-.5f,z+.5f);
            GroundColorTriangle(surface,a,d,c);GroundColorTriangle(surface,a,c,b);
        }
        SurfaceMesh(_landscape,surface).Name="ValleyContext";
        using var streamSurface=new SurfaceTool();streamSurface.Begin(Godot.Mesh.PrimitiveType.Triangles);
        for(int z=map.MinZ-7;z<=map.MaxZ+7;z++)
        {
            if(z>=-6 && z<=8)continue;
            float Center(float at)=>5+MathF.Sin(at*.13f)*Math.Clamp((MathF.Abs(at-1)-8)/5,0,1)*1.4f;
            float Width(float at)=>.5f+.30f*Math.Clamp((MathF.Abs(at-1)-8)/4,0,1);
            float p=z-.5f,q=z+.5f;
            var a=new Vector3(Center(p)-Width(p),-.11f,p);var b=new Vector3(Center(p)+Width(p),-.11f,p);
            var c=new Vector3(Center(q)+Width(q),-.11f,q);var d=new Vector3(Center(q)-Width(q),-.11f,q);
            Triangle(streamSurface,a,d,c,new("668e96"));Triangle(streamSurface,a,c,b,new("668e96"));
        }
        SurfaceMesh(_landscape,streamSurface).Name="DistantStream";
        // Broad distant woodland masses frame the open settlement; these are outside playable land.
        for(int x=map.MinX-5;x<=map.MaxX+5;x+=3)for(int z=map.MinZ-5;z<=map.MaxZ+5;z+=3)
        {
            if(z>map.MinZ+5 && x<map.MaxX-3)continue;
            float distance=land.Min(c=>(c.X-x)*(c.X-x)+(c.Z-z)*(c.Z-z));
            int hash=Math.Abs(x*31+z*17);
            if(distance<12 || distance>60 || hash%5>2 || Math.Abs(x-5)<3)continue;
            foreach(var offset in new[]{new Vector2(-.85f,-.55f),new(.55f,-.65f),new(-.3f,.7f),new(.9f,.65f)})
            {
                var tree=MakeTree(Corner(x+offset.X,z+offset.Y),.95f+(hash%3)*.12f,new("587752"));
                tree.Reparent(_landscape);
            }
        }
        _studyBoundary=new Node3D{Name="StudyBuildableBoundary",Visible=false};_landscape.AddChild(_studyBoundary);
        foreach(var edge in _storybookEdges)
        {
            var mid=(edge.A+edge.B)/2;var size=edge.A.X==edge.B.X?new Vector3(.035f,.015f,1):new Vector3(1,.015f,.035f);
            Box(_studyBoundary,mid+Vector3.Up*.025f,size,new("eed39a"));
        }
        BatchStaticGeometry(_studyBoundary);
    }
    private void UpdateStorybookSpaces()
    {
        if(_studyBoundary!=null && GodotObject.IsInstanceValid(_studyBoundary))_studyBoundary.Visible=_storybookScene && (_placing || _plantingTrees || _decorating);
        if(!_storybookScene)return;
        ApplyWorldLabels();
        var sites=_world.Cottages.Where(c=>c.Complete && !c.DemolitionRequested && c.Kind is BuildingKind.Cottage or BuildingKind.ForagerHut or BuildingKind.SeatingGarden or BuildingKind.Square).ToArray();
        int key=17;foreach(var site in sites)key=HashCode.Combine(key,site.Id,site.Cell,site.Rotation);
        if(_studyGround!=null && GodotObject.IsInstanceValid(_studyGround) && !_studyGround.IsQueuedForDeletion() && key==_studyGroundKey)return;
        if(_studyGround!=null && GodotObject.IsInstanceValid(_studyGround) && !_studyGround.IsQueuedForDeletion())_studyGround.QueueFree();
        _studyGround=new Node3D{Name="OutdoorGroundStudy"};_dynamic.AddChild(_studyGround);_studyGroundKey=key;
        if(sites.Length==0)return;
        (Color Color,float Strength) Tint(Vector3 at)
        {
            float strongest=0;Color earth=new("aa9068");
            foreach(var site in sites)
            {
                float radius=site.Kind is BuildingKind.SeatingGarden or BuildingKind.Square?2.5f:1.8f;
                float distance=new Vector2(at.X-site.Entrance.X,at.Z-site.Entrance.Z).Length();
                float amount=Math.Clamp((radius-distance)/(radius*.65f),0,1);
                if(amount<=strongest)continue;strongest=amount;earth=site.Kind==BuildingKind.ForagerHut?new("79674d"):new("938260");
            }
            return (ContinuousGroundTint(at).Lerp(earth,strongest*.78f),strongest);
        }
        using var surface=new SurfaceTool();surface.Begin(Godot.Mesh.PrimitiveType.Triangles);int count=0;
        foreach(var cell in _world.Map.Land)for(int ix=0;ix<2;ix++)for(int iz=0;iz<2;iz++)
        {
            float x=cell.X-.5f+ix*.5f,z=cell.Z-.5f+iz*.5f;
            var a=OnGround(x,z,.018f);var b=OnGround(x+.5f,z,.018f);var c=OnGround(x+.5f,z+.5f,.018f);var d=OnGround(x,z+.5f,.018f);
            if(new[]{a,b,c,d}.All(p=>Tint(p).Strength==0))continue;
            foreach(var tri in new[]{new[]{a,d,c},new[]{a,c,b}})
            {
                surface.SetNormal((tri[1]-tri[0]).Cross(tri[2]-tri[0]).Normalized());
                foreach(var point in new[]{tri[0],tri[2],tri[1]}){surface.SetColor(Tint(point).Color);surface.AddVertex(point);count++;}
            }
        }
        if(count>0)SurfaceMesh(_studyGround,surface);
    }
}
