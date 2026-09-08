using Godot;
using System;

public partial class Game : Node3D
{
    private StandardMaterial3D Material(Color color) => new() { AlbedoColor = color, Roughness = 0.95f };
    private MeshInstance3D Mesh(Node3D parent, Mesh mesh, Vector3 at, Color color)
    {
        var result = new MeshInstance3D { Mesh = mesh, Position = at, MaterialOverride = Material(color) };
        parent.AddChild(result); return result;
    }
    private MeshInstance3D Box(Node3D parent, Vector3 at, Vector3 size, Color color) => Mesh(parent, new BoxMesh { Size = size }, at, color);
    private MeshInstance3D Cylinder(Node3D parent, Vector3 at, float radius, float height, Color color, float? top = null) =>
        Mesh(parent, new CylinderMesh { BottomRadius = radius, TopRadius = top ?? radius, Height = height, RadialSegments = 8 }, at, color);
    private void Clear(Node3D parent) { foreach (var child in parent.GetChildren()) { parent.RemoveChild(child); child.QueueFree(); } }

    private void MakeLandscape()
    {
        var environment = new Godot.Environment
        {
            BackgroundMode = Godot.Environment.BGMode.Color, BackgroundColor = new("afc5bf"),
            AmbientLightSource = Godot.Environment.AmbientSource.Color, AmbientLightColor = Colors.White, AmbientLightEnergy = 0.3f,
            TonemapMode = Godot.Environment.ToneMapper.Linear
        };
        AddChild(new WorldEnvironment { Environment = environment });
        AddChild(new DirectionalLight3D { RotationDegrees = new(-52, -30, 0), LightColor = new("fffaf1"), LightEnergy = 0.65f, ShadowEnabled = true, DirectionalShadowMaxDistance = 70 });
        _camera = new Camera3D { Projection = Camera3D.ProjectionType.Orthogonal, Size = 23, Far = 120, Current = true };
        AddChild(_camera);
        Box(this, new(0, -1.4f, 0), new(20, 2.3f, 18), new("877d62"));
        Box(this, new(0, -0.22f, 0), new(20, 0.35f, 18), new("a5ac75"));
        var backdrop = Box(this, new(0, -2.65f, 0), new(200, 0.1f, 200), new("8caaa6"));
        ((StandardMaterial3D)backdrop.MaterialOverride).ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
        var random = new Random(17);
        for (int x = -9; x <= 9; x++) for (int z = -8; z <= 8; z++)
        {
            float tint = (float)random.NextDouble() * 0.045f;
            Box(this, new(x, -0.045f, z), new(1.0f, 0.07f, 1.0f), new Color(0.42f + tint, 0.51f + tint, 0.30f + tint));
        }
        foreach (var p in new[] { new Vector3(-9,0,-8), new(-6,0,-8), new(-9,0,-4), new(9,0,-7), new(9,0,-3), new(6,0,-8), new(-9,0,8), new(9,0,8) })
            MakeTree(p, 0.8f + (float)random.NextDouble() * 0.35f, new("698458"));
        for (int i = 0; i < 65; i++)
        {
            float x = (float)random.NextDouble() * 18 - 9, z = (float)random.NextDouble() * 16 - 8;
            if (Math.Abs(x) < 7.5f && Math.Abs(z) < 6.5f) continue;
            var rock = Mesh(this, new SphereMesh { Radius = 0.22f, Height = 0.36f, RadialSegments = 5, Rings = 3 }, new(x, 0.04f, z), new("b7b299"));
            rock.Scale = new(1.5f, 0.8f, 1);
        }
        Box(this, new(-3, 0.04f, 3), new(1.5f, 0.08f, 1.5f), new("bda47c"));
        for (int i = 0; i < 4; i++) Box(this, new(-3.65f + i * 0.43f, 0.11f, 3), new(0.10f, 0.12f, 1.5f), _wood);
        Sign(new(-3, 0, 4), "TIMBER YARD");
    }
    private void Sign(Vector3 at, string text)
    {
        Box(this, at + new Vector3(0, 0.35f, 0), new(0.07f, 0.7f, 0.07f), _wood);
        var label = new Label3D { Text = text, Position = at + new Vector3(0, 0.85f, 0), FontSize = 32, PixelSize = 0.008f,
            Billboard = BaseMaterial3D.BillboardModeEnum.Enabled, Modulate = _cream, OutlineSize = 5 };
        AddChild(label);
    }
    private Node3D MakeTree(Vector3 at, float scale, Color leaves)
    {
        var tree = new Node3D { Position = at, Scale = Vector3.One * scale }; AddChild(tree);
        Cylinder(tree, new(0, 0.8f, 0), 0.18f, 1.6f, _wood, 0.11f);
        Mesh(tree, new SphereMesh { Radius = 0.9f, Height = 1.9f, RadialSegments = 7, Rings = 4 }, new(0, 1.95f, 0), leaves);
        Mesh(tree, new SphereMesh { Radius = 0.65f, Height = 1.3f, RadialSegments = 6, Rings = 3 }, new(0.5f, 1.7f, 0.1f), leaves.Lightened(0.05f));
        return tree;
    }
    private void Log(Node3D parent, Vector3 at, float length = 0.85f)
    {
        var log = Cylinder(parent, at, 0.13f, length, _wood); log.RotationDegrees = new(0, 0, 90);
        var end = Cylinder(parent, at + new Vector3(length / 2 + 0.002f, 0, 0), 0.115f, 0.015f, new("d4b17e")); end.RotationDegrees = new(0, 0, 90);
    }
    private void MakeCottage(Node3D parent, int stage)
    {
        Box(parent, new(0, 0.08f, 0), new(2.9f, 0.16f, 1.9f), new("b9b099"));
        if (stage < 1) return;
        foreach (float x in new[] { -1.3f, 1.3f }) foreach (float z in new[] { -0.8f, 0.8f })
            Box(parent, new(x, 0.83f, z), new(0.12f, 1.5f, 0.12f), _wood);
        Box(parent, new(0, 1.55f, 0.8f), new(2.8f, 0.14f, 0.14f), _wood);
        if (stage < 2) return;
        Box(parent, new(0, 0.87f, 0), new(2.65f, 1.4f, 1.65f), _cream);
        Box(parent, new(-0.48f, 0.63f, 0.84f), new(0.57f, 1.05f, 0.05f), _wood);
        Box(parent, new(0.65f, 1.05f, 0.85f), new(0.65f, 0.55f, 0.06f), _wood);
        Box(parent, new(0.65f, 1.05f, 0.89f), new(0.47f, 0.38f, 0.03f), new("738a89"));
        Box(parent, new(0.65f, 1.05f, 0.92f), new(0.05f, 0.40f, 0.03f), _cream);
        if (stage < 3) return;
        foreach (float side in new[] { -1f, 1f })
        {
            var roof = Box(parent, new(0, 1.95f, side * 0.53f), new(3.15f, 0.16f, 1.35f), _roof);
            roof.RotationDegrees = new(side * 32, 0, 0);
        }
        var gable = new SurfaceTool(); gable.Begin(Godot.Mesh.PrimitiveType.Triangles);
        foreach (float x in new[] { -1.325f, 1.325f })
        {
            var a = new Vector3(x, 1.57f, -0.825f); var b = new Vector3(x, 2.2f, 0); var c = new Vector3(x, 1.57f, 0.825f);
            gable.AddVertex(x < 0 ? c : a); gable.AddVertex(b); gable.AddVertex(x < 0 ? a : c);
        }
        gable.GenerateNormals(); Mesh(parent, gable.Commit(), Vector3.Zero, _cream);
        Box(parent, new(0.90f, 2.0f, -0.2f), new(0.30f, 1.0f, 0.35f), new("897e6c"));
    }
}
