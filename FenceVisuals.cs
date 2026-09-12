using Godot;
using Inlanders.Simulation;
using System.Collections.Generic;
using System.Linq;

public partial class Game
{
    private static readonly Cell[] FenceDirections = { new(1,0), new(0,1), new(-1,0), new(0,-1) };
    private readonly Dictionary<Cell,Node3D> _fenceBodies = new();
    private readonly HashSet<Cell> _fencePreviewCells = new();
    private World? _fencePreviewWorld;

    // Connections are derived presentation: no new saved state or navigation rules.
    private int FenceConnections(Cell cell, Cell? added=null, Cell? removed=null)
    {
        int mask=0;
        for(int i=0;i<4;i++)
        {
            var d=FenceDirections[i];var neighbor=new Cell(cell.X+d.X,cell.Z+d.Z);
            if(neighbor!=removed && (neighbor==added || _world.Decorations.Any(f=>f.Kind==DecorationKind.Fence && f.Cell==neighbor))) mask|=1<<i;
        }
        return mask;
    }
    private void MakeFence(Node3D root, Cell cell, bool rotated, int connections)
    {
        root.SetMeta("connections",connections);
        Vector3 Ground(float x,float z,float lift) => OnGround(cell.X+x,cell.Z+z,lift)-OnGround(cell.X,cell.Z);
        void Post(float x,float z) => Box(root,Ground(x,z,.31f),new(.09f,.62f,.09f),new("bfa37a"));
        void Rail(float x,float z)
        {
            foreach(float y in new[]{.22f,.48f}) TimberBeam(root,Ground(0,0,y),Ground(x,z,y),.075f,new("d1bb91"));
        }
        Post(0,0);
        if(connections==0)
        {
            foreach(float end in new[]{-.42f,.42f}) { float x=rotated?0:end,z=rotated?end:0;Post(x,z);Rail(x,z); }
        }
        else
        {
            for(int i=0;i<4;i++) if((connections&(1<<i))!=0)
            {
                var d=FenceDirections[i];Rail(d.X*.5f,d.Z*.5f);
                // A terminal extends to the far side of its tile, independent of old rotation.
                if((connections&(connections-1))==0) { Post(-d.X*.42f,-d.Z*.42f);Rail(-d.X*.42f,-d.Z*.42f); }
            }
        }
        BatchStaticGeometry(root);
    }
    private void MakeFencePreview()
    {
        _fencePreviewCells.Clear();_fencePreviewWorld=_world;
        Cell? added=_ghostValid && !_removeDecoration?_hover:null;
        Cell? removed=_ghostValid && _removeDecoration?_hover:null;
        if(!_removeDecoration) MakeFence(_ghostModel,_hover,_rotation%2!=0,FenceConnections(_hover,added,removed));
        if(!_ghostValid)return;
        if(_removeDecoration)_fencePreviewCells.Add(_hover);
        foreach(var d in FenceDirections)
        {
            var cell=new Cell(_hover.X+d.X,_hover.Z+d.Z);
            var neighbor=_world.Decorations.FirstOrDefault(f=>f.Kind==DecorationKind.Fence && f.Cell==cell);
            if(neighbor==null)continue;
            _fencePreviewCells.Add(cell);
            var body=new Node3D { Position=OnGround(cell.X,cell.Z)-OnGround(_hover.X,_hover.Z) };
            _ghostModel.AddChild(body);MakeFence(body,cell,neighbor.Rotated,FenceConnections(cell,added,removed));
        }
    }
    private void UpdateFencePreviewVisibility()
    {
        bool preview=_placing && _decorating && _ghostValid && _ghost.Visible && _fencePreviewWorld==_world;
        foreach(var (cell,body) in _fenceBodies) body.Visible=!(preview && _fencePreviewCells.Contains(cell));
    }
}
