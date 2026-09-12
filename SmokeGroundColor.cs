using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

public partial class Game
{
    private async Task CheckGroundColorPreview(string folder,string scene)
    {
        string saved=_world.SaveJson();
        // Adjacent triangles must use identical colors at their shared positions.
        var mesh=_landscape.GetNode<MeshInstance3D>(_world.Map.OriginalOutline?"OriginalGrass":"TerrainSurface").Mesh;
        var arrays=mesh.SurfaceGetArrays(0);var vertices=arrays[(int)Godot.Mesh.ArrayType.Vertex].AsVector3Array();
        var colors=arrays[(int)Godot.Mesh.ArrayType.Color].AsColorArray();var seen=new Dictionary<Vector3,Color>();
        for(int i=0;i<vertices.Length;i++)
        {
            if(seen.TryGetValue(vertices[i],out var previous) && !previous.IsEqualApprox(colors[i]))throw new Exception("Ground color seam at shared vertex");
            seen[vertices[i]]=colors[i];
        }
        async Task Frames(){for(int i=0;i<5;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        var valid=_world.Map.Land.First(c=>_world.PlacementProblem(c,0,BuildingKind.Cottage)==null);
        var invalid=_world.Map.Land.First(c=>_world.PlacementProblem(c,0,BuildingKind.Cottage)!=null);
        foreach(int width in new[]{960,1440})foreach(bool legal in new[]{true,false})foreach(int angle in new[]{0,1})
        {
            var cell=legal?valid:invalid;GetWindow().Size=new(width,width==960?640:900);
            _focus=OnGround(cell.X,cell.Z);_camera.Size=12;_angle=.72f+angle*Mathf.Pi/2;UpdateCamera();
            BeginPlacement(BuildingKind.Cottage);_rotation=0;await Frames();
            var pointer=_camera.UnprojectPosition(OnGround(cell.X,cell.Z));
            Input.ParseInputEvent(new InputEventMouseMotion{Position=pointer,GlobalPosition=pointer});await Frames();
            if(_hover!=cell || _ghostValid!=legal || !_ghost.Visible)throw new Exception("Ground color changed placement/hit feedback");
            await Capture($"{folder}/{scene}-preview-{legal}-{width}-{angle}.png");await Press(Key.Escape);
        }
        if(saved!=_world.SaveJson())throw new Exception("Ground preview changed simulation");
    }
}
