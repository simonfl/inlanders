using Godot;

public partial class Game
{
    private void MakeBakery(Node3D parent, int stage)
    {
        var brick = new Color("b47a58");
        var mortar = new Color("d0aa80");
        var plaster = new Color("eed7a6");
        var roofColor = new Color("537c78");

        // A low shop beside a round masonry oven, all inside the usual footprint.
        Box(parent, new(0, 0.07f, 0), new(2.85f, 0.14f, 1.85f), new("b6a18b"));
        Box(parent, new(0.85f, 0.28f, -0.13f), new(0.98f, 0.42f, 1.25f), brick);
        if (stage < 1) return;
        foreach (float x in new[] { -1.28f, 0.25f })
            foreach (float z in new[] { -0.73f, 0.18f })
                Box(parent, new(x, 0.76f, z), new(0.11f, 1.4f, 0.11f), _wood);
        Box(parent, new(-0.5f, 1.4f, -0.25f), new(1.7f, 0.1f, 1.05f), _wood);
        Box(parent, new(0.86f, 0.85f, -0.51f), new(0.43f, 1.35f, 0.43f), brick);
        if (stage < 2) return;

        Box(parent, new(-0.5f, 0.76f, -0.3f), new(1.62f, 1.28f, 0.94f), plaster);
        Box(parent, new(-0.5f, 0.24f, -0.3f), new(1.67f, 0.2f, 0.98f), brick);
        // Exposed dome and tall chimney make the oven recognizable from above.
        Mesh(parent, new SphereMesh { Radius = 0.56f, Height = 1.12f, RadialSegments = 10, Rings = 5 },
            new(0.83f, 0.65f, -0.08f), mortar);
        Box(parent, new(0.88f, 1.56f, -0.52f), new(0.45f, 2.02f, 0.46f), brick);
        for (int i = 0; i < 7; i++)
            Box(parent, new(0.88f, 1.13f + i * 0.21f, -0.52f), new(0.465f, 0.035f, 0.475f), mortar);
        Box(parent, new(0.88f, 2.6f, -0.52f), new(0.59f, 0.14f, 0.6f), new("926b54"));
        Box(parent, new(0.88f, 2.675f, -0.52f), new(0.33f, 0.015f, 0.34f), new("403b34"));
        var mouth = Mesh(parent, new SphereMesh { Radius = 0.27f, Height = 0.52f, RadialSegments = 10, Rings = 5 },
            new(0.83f, 0.55f, 0.43f), new("483c31"));
        mouth.Scale = new(1, 1, 0.16f);
        Box(parent, new(0.83f, 0.39f, 0.485f), new(0.4f, 0.1f, 0.03f), new("df9a4c"));
        Box(parent, new(0.83f, 0.29f, 0.53f), new(0.7f, 0.11f, 0.42f), new("938477"));
        if (stage < 3) return;

        var roof = Box(parent, new(-0.52f, 1.54f, -0.26f), new(1.88f, 0.15f, 1.22f), roofColor);
        roof.RotationDegrees = new(10, 0, 0);
        for (int i = 0; i < 5; i++)
        {
            var seam = Box(parent, new(-1.26f + i * 0.37f, 1.63f, -0.26f), new(0.035f, 0.025f, 1.22f), roofColor.Lightened(0.13f));
            seam.RotationDegrees = new(10, 0, 0);
        }
        // A shaded serving hatch and striped canvas distinguish the shopfront.
        Box(parent, new(-0.53f, 0.94f, 0.18f), new(1.19f, 0.65f, 0.06f), _wood);
        Box(parent, new(-0.53f, 0.94f, 0.22f), new(1.03f, 0.5f, 0.035f), new("625649"));
        foreach (float x in new[] { -1.3f, 0.22f })
            Box(parent, new(x, 0.67f, 0.78f), new(0.065f, 1.25f, 0.065f), _wood);
        for (int i = 0; i < 6; i++)
        {
            var color = i % 2 == 0 ? new Color("e9c98a") : new Color("b96544");
            var canvas = Box(parent, new(-1.16f + i * 0.26f, 1.3f, 0.52f), new(0.26f, 0.07f, 0.73f), color);
            canvas.RotationDegrees = new(14, 0, 0);
            Box(parent, new(-1.16f + i * 0.26f, 1.16f, 0.88f), new(0.26f, 0.15f, 0.05f), color);
        }
        Box(parent, new(-0.53f, 0.44f, 0.63f), new(1.4f, 0.68f, 0.44f), _wood);
        Box(parent, new(-0.53f, 0.8f, 0.64f), new(1.48f, 0.09f, 0.5f), new("d0a06b"));
        foreach (float x in new[] { -1.0f, -0.56f, -0.12f })
        {
            var loaf = Mesh(parent, new SphereMesh { Radius = 0.14f, Height = 0.22f, RadialSegments = 8, Rings = 4 },
                new(x, 0.91f, 0.66f), new("dfa64e"));
            loaf.Scale = new(1.25f, 1, 0.8f);
            for (int i = 0; i < 2; i++)
                Box(parent, new(x - 0.055f + i * 0.1f, 1.02f, 0.66f), new(0.025f, 0.012f, 0.13f), new("f4d796"));
        }
        FoodSign(parent, "BAKERY", 2.95f);
    }
}
