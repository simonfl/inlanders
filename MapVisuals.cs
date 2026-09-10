using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;

public partial class Game
{
    private float MaximumZoom => _world.MaximumViewZoom;
    private void FrameMap()
    {
        _followPerson = false;
        var map = _world.Map;
        _focus = new((map.MinX + map.MaxX) / 2f, 0, (map.MinZ + map.MaxZ) / 2f);
        _camera.Size = map.OriginalOutline ? 23 : Math.Min(MaximumZoom, (map.Width + map.Depth) * 0.76f);
        UpdateCamera();
    }
    private void MakeExpandedLandscape()
    {
        var land = _world.Map.Land.ToArray();
        // Two instanced layers keep a larger board inexpensive to draw, including its stepped edge.
        void Layer(float height, float y, bool grass)
        {
            var mesh = new MultiMesh { TransformFormat = MultiMesh.TransformFormatEnum.Transform3D, UseColors = true,
                Mesh = new BoxMesh { Size = new(1, height, 1) }, InstanceCount = land.Length };
            var random = new Random(17);
            for (int i = 0; i < land.Length; i++)
            {
                float tint = (float)random.NextDouble() * 0.045f;
                mesh.SetInstanceTransform(i, new Transform3D(Basis.Identity, new Vector3(land[i].X, y, land[i].Z)));
                mesh.SetInstanceColor(i, grass ? GroundTint(land[i].X, land[i].Z) : new Color("877d62"));
            }
            _landscape.AddChild(new MultiMeshInstance3D { Multimesh = mesh, MaterialOverride = new StandardMaterial3D { VertexColorUseAsAlbedo = true, Roughness = 1 } });
        }
        if (_world.Map.Heights.Length > 0) MakeTerrainSurface();
        else { Layer(1.6f, -0.87f, false); Layer(0.08f, -0.03f, true); }
        foreach (var cell in _world.Map.Water)
        {
            Box(_landscape, new(cell.X, -0.5f, cell.Z), new(1, 0.8f, 1), new("687d73"));
            Box(_landscape, new(cell.X, -0.11f, cell.Z), new(1, 0.06f, 1), new("639baf"));
            Box(_landscape, new(cell.X - 0.12f, -0.075f, cell.Z + 0.15f), new(0.4f, 0.01f, 0.025f), new("9ac3ca"));
            foreach (var d in new[] { new Cell(1,0), new(-1,0), new(0,1), new(0,-1) })
            {
                var bank = new Cell(cell.X+d.X, cell.Z+d.Z);
                if (!_world.Map.Contains(bank) || _world.Map.Water.Contains(bank)) continue;
                Box(_landscape, new(cell.X+d.X*0.48f, -0.02f, cell.Z+d.Z*0.48f),
                    new(d.X == 0 ? 1 : 0.12f, 0.12f, d.Z == 0 ? 1 : 0.12f), new("b9ae85"));
            }
        }
        var backdrop = Box(_landscape, new(0, -1.75f, 0), new(400, 0.1f, 400), new("8caaa6"));
        ((StandardMaterial3D)backdrop.MaterialOverride).ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
    }
    private string _largeSavePath = "saves/three-clearings.json";
    private string _creativeSavePath = "saves/creative.json", _creativeLargeSavePath = "saves/creative-three-clearings.json";
    private string SandboxSavePath(bool large, bool creative) => creative ? (large ? _creativeLargeSavePath : _creativeSavePath) : (large ? _largeSavePath : _savePath);
    private string CurrentSavePath => SandboxSavePath(_world.Map.Name == "Three clearings", _world.Creative);
    private void OpenLargeMap()
    {
        try
        {
            if (_world.Map.Name == "Three clearings") { FrameMap(); CloseDrawer(); return; }
            var path = SandboxSavePath(true, _world.Creative);
            var next = System.IO.File.Exists(path) ? World.LoadFile(path) : _world.Creative ? World.NewCreative(true) : World.NewLargeMap();
            if (_world.Campaign != null) SaveCampaign(); else _world.SaveFile(CurrentSavePath);
            AdoptWorld(next); FrameMap();
            Notice(_world.Creative ? "Creative · Three clearings. Home frames the map; F5 saves." : "Three clearings · a larger village to explore. Home frames the map; F5 saves. The supper goal is optional.");
        }
        catch (Exception e) { Notice("Could not open larger map; current village kept. " + e.Message); }
    }
    private void OpenOriginalMap()
    {
        try
        {
            if (_world.Campaign == null && _world.Map.OriginalOutline) { FrameMap(); CloseDrawer(); return; }
            var path = SandboxSavePath(false, _world.Creative);
            var next = System.IO.File.Exists(path) ? World.LoadFile(path) : _world.Creative ? World.NewCreative() : World.NewScenario();
            if (_world.Campaign != null) SaveCampaign(); else _world.SaveFile(CurrentSavePath);
            AdoptWorld(next); FrameMap(); Notice("Original settlement restored and paused. Press Space to continue.");
        }
        catch (Exception e) { Notice("Could not open original settlement; current village kept. " + e.Message); }
    }
}
