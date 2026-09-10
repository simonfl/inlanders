using Godot;
using System;

public partial class Game
{
    private readonly Color _frameTimber = new("694f3d"), _stone = new("999582"), _recess = new("393f39");

    private void TimberBeam(Node3D parent, Vector3 from, Vector3 to, float width, Color color)
    {
        var beam = Box(parent, (from + to) / 2, new(width, from.DistanceTo(to), width), color);
        beam.Quaternion = new Quaternion(Vector3.Up, (to - from).Normalized());
    }

    // A thick pitched roof with exposed end grain; ridge and eaves read at town scale.
    private void VillageRoof(Node3D parent, Vector3 center, float width, float depth, float rise, Color color, bool enclosed)
    {
        float half = depth / 2;
        float slope = MathF.Atan2(rise, half);
        foreach (float side in new[] { -1f, 1f })
        {
            var panel = Box(parent, center + new Vector3(0, rise / 2, side * half / 2),
                new(width, .19f, MathF.Sqrt(half * half + rise * rise) + .06f), color);
            panel.Rotation = new(side * slope, 0, 0);
            Box(parent, center + new Vector3(0, -.03f, side * half), new(width, .20f, .16f), _frameTimber);
            foreach (float x in new[] { -width / 2 + .07f, width / 2 - .07f })
                TimberBeam(parent, center + new Vector3(x, 0, side * half), center + new Vector3(x, rise, 0), .16f, _frameTimber);
        }
        Box(parent, center + new Vector3(0, rise + .045f, 0), new(width + .04f, .18f, .18f), color.Lightened(.07f));
        if (!enclosed) return;
        using var surface = new SurfaceTool(); surface.Begin(Godot.Mesh.PrimitiveType.Triangles);
        foreach (float side in new[] { -1f, 1f })
        {
            float x = side * (width / 2 - .18f);
            surface.AddVertex(center + new Vector3(x, -.08f, -half + .12f));
            surface.AddVertex(center + new Vector3(x, rise - .07f, 0));
            surface.AddVertex(center + new Vector3(x, -.08f, half - .12f));
        }
        surface.GenerateNormals();
        var gable = Mesh(parent, surface.Commit(), Vector3.Zero, new("d5c4a0"));
        ((StandardMaterial3D)gable.MaterialOverride).CullMode = BaseMaterial3D.CullModeEnum.Disabled;
    }

    private void StoneFoot(Node3D parent, Vector3 at, Vector3 size)
    {
        Box(parent, at, size, _stone);
        Box(parent, at + new Vector3(.015f, size.Y * .45f, 0), new(size.X * .94f, .06f, size.Z * .95f), _stone.Lightened(.10f));
    }

    private void CottageWindow(Node3D parent, Vector3 at, float yaw = 0)
    {
        var opening = new Node3D { Position = at, RotationDegrees = new(0, yaw, 0) }; parent.AddChild(opening);
        Box(opening, Vector3.Zero, new(.54f, .56f, .035f), _recess);
        foreach (float x in new[] { -.30f, .30f }) Box(opening, new(x, 0, .07f), new(.10f, .72f, .16f), _frameTimber);
        Box(opening, new(0, .31f, .07f), new(.70f, .10f, .16f), _frameTimber);
        Box(opening, new(0, -.32f, .12f), new(.76f, .13f, .28f), _stone.Lightened(.12f));
        Box(opening, new(0, 0, .025f), new(.045f, .53f, .055f), new("d5bb89"));
        Box(opening, new(0, 0, .025f), new(.53f, .045f, .055f), new("d5bb89"));
        Box(opening, new(.47f, 0, .045f), new(.19f, .57f, .07f), new("647b6b"));
    }

    private void MakeCottage(Node3D parent, int stage, int variant = 0)
    {
        // Individual wall feet and a doorstep replace the full rectangular display plinth.
        StoneFoot(parent, new(0, .16f, -.23f), new(2.4f, .28f, 1.22f));
        StoneFoot(parent, new(.62f, .15f, .55f), new(1.16f, .26f, .42f));
        StoneFoot(parent, new(-.58f, .09f, .68f), new(.85f, .13f, .51f));
        if (stage < 1) return;
        foreach (float x in new[] { -1.12f, 1.12f }) foreach (float z in new[] { -.74f, .66f })
        {
            StoneFoot(parent, new(x, .14f, z), new(.30f, .22f, .30f));
            Box(parent, new(x, 1.02f, z), new(.17f, 1.68f, .17f), _frameTimber);
        }
        foreach (float z in new[] { -.74f, .66f }) Box(parent, new(0, 1.83f, z), new(2.48f, .20f, .18f), _frameTimber);
        if (stage < 2) return;
        var plaster = new Color("e2cfaa");
        Box(parent, new(0, 1.02f, -.25f), new(2.22f, 1.48f, 1.02f), plaster);
        Box(parent, new(.63f, 1.02f, .46f), new(.96f, 1.48f, .41f), plaster);
        // The door sits behind the porch posts, giving a real shaded entry corner.
        Box(parent, new(-.57f, .85f, .27f), new(.67f, 1.16f, .05f), _recess);
        Box(parent, new(-.57f, .78f, .30f), new(.49f, 1.02f, .045f), new("80684b"));
        Box(parent, new(-.57f, 1.48f, .35f), new(.85f, .17f, .23f), _frameTimber);
        Box(parent, new(-.32f, .8f, .34f), new(.06f, .08f, .05f), new("c1a371"));
        CottageWindow(parent, new(.62f, 1.13f, .69f));
        CottageWindow(parent, new(1.13f, 1.10f, -.20f), 90);
        CottageWindow(parent, new(-1.13f, 1.10f, -.20f), -90);
        CottageWindow(parent, new(-.20f, 1.10f, -.77f), 180);
        if (stage < 3) return;
        var roofColor = variant == 1 ? new Color("768478") : variant == 2 ? new Color("a28d63") : new Color("ae7156");
        VillageRoof(parent, new(0, 1.87f, -.02f), 2.78f, 1.95f, .84f, roofColor, true);
        Box(parent, new(.75f, 2.49f, -.37f), new(.39f, 1.18f, .42f), _stone.Darkened(.10f));
        Box(parent, new(.75f, 3.10f, -.37f), new(.52f, .16f, .55f), _stone);
        Box(parent, new(.75f, 3.185f, -.37f), new(.25f, .015f, .28f), _recess);
    }
}
