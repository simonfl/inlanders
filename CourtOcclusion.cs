using Godot;
using Inlanders.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Game
{
    private sealed class CourtOccluder
    {
        public Node3D Body=null!;
        public int Stage;
        public bool Veiled;
        public List<(MeshInstance3D Mesh,StandardMaterial3D Normal,StandardMaterial3D Veil)> Pieces=new();
    }
    private readonly Dictionary<int,CourtOccluder> _courtOccluders=new();
    private static bool BlocksView(Vector3 point,Vector3 camera,float height)
    {
        var direction=camera-point;float near=0,far=1;
        for(int axis=0;axis<3;axis++)
        {
            float low=axis==1?.15f:axis==0?-1.4f:-1.0f;
            float high=axis==1?height:axis==0?1.4f:1.0f;
            if(MathF.Abs(direction[axis])<.0001f){if(point[axis]<low || point[axis]>high)return false;continue;}
            float a=(low-point[axis])/direction[axis],b=(high-point[axis])/direction[axis];
            near=MathF.Max(near,MathF.Min(a,b));far=MathF.Min(far,MathF.Max(a,b));
            if(near>far)return false;
        }
        return near>.0001f && near<1;
    }
    private void RenderCourtOcclusion()
    {
        if(!_world.IsArrangementCourt)return;
        foreach(var home in _world.Cottages.Where(c=>c.Complete && Buildings.Get(c.Kind).Beds>0))
        {
            if(!_cottages.TryGetValue(home.Id,out var building))continue;
            if(!_courtOccluders.TryGetValue(home.Id,out var entry) || entry.Body!=building.Body || entry.Stage!=building.Stage)
            {
                entry=new(){Body=building.Body,Stage=building.Stage};_courtOccluders[home.Id]=entry;
                void Collect(Node node)
                {
                    foreach(var child in node.GetChildren())
                    {
                        if(child is MeshInstance3D mesh && mesh.MaterialOverride is StandardMaterial3D material)
                        {
                            var veil=(StandardMaterial3D)material.Duplicate();var tint=veil.AlbedoColor;tint.A=.20f;
                            veil.AlbedoColor=tint;veil.Transparency=BaseMaterial3D.TransparencyEnum.AlphaHash;
                            entry.Pieces.Add((mesh,material,veil));
                        }
                        Collect(child);
                    }
                }
                Collect(building.Body);
            }
            bool hidden=false;
            if(ReadableCourt)
            {
                var camera=entry.Body.ToLocal(_camera.GlobalPosition);
                foreach(var p in _world.People)
                {
                    // Reveal real stationary activity, not every passing or idle person.
                    if(p.Task is not (Work.Planting or Work.Harvesting or Work.EatingMeal or Work.Resting or Work.Leisure))continue;
                    var chest=_people[p.Id].Torso.GlobalPosition+new Vector3(0,.2f,0);
                    if(BlocksView(entry.Body.ToLocal(chest),camera,home.Kind==BuildingKind.Lodge?3.7f:3.0f)){hidden=true;break;}
                }
            }
            if(entry.Veiled==hidden)continue;
            entry.Veiled=hidden;
            foreach(var piece in entry.Pieces)piece.Mesh.MaterialOverride=hidden?piece.Veil:piece.Normal;
        }
    }
}
