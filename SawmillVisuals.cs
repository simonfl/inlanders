using Godot;
using Inlanders.Simulation;

public partial class Game
{
    private void Plank(Node3D parent, Vector3 at)
    {
        Box(parent, at, new(0.85f, 0.10f, 0.21f), new("d4ac70"));
        Box(parent, at + new Vector3(0, 0.051f, 0), new(0.72f, 0.005f, 0.015f), new("ae8554"));
    }
    private void MakeSawmill(Node3D parent, int stage)
    {
        Box(parent, new(0, 0.06f, 0), new(2.9f, 0.12f, 1.9f), new("b49d78"));
        if (stage == 0) return;
        foreach (float x in new[] { -1.2f, 1.2f }) Box(parent, new(x, 0.85f, -0.6f), new(0.14f, 1.7f, 0.14f), _wood);
        if (stage == 1) return;
        Box(parent, new(0, 0.55f, 0.3f), new(2.3f, 0.14f, 0.6f), _wood);
        foreach (float x in new[] { -0.9f, 0.9f }) Box(parent, new(x, 0.28f, 0.3f), new(0.12f, 0.55f, 0.45f), _wood);
        if (stage < 3) return;
        var roof = Box(parent, new(0, 1.7f, -0.35f), new(2.9f, 0.14f, 1.3f), new("6b8390")); roof.RotationDegrees = new(-10,0,0);
        Log(parent, new(0, 0.75f, 0.3f), 1.8f);
        Box(parent, new(0.2f, 0.95f, 0.3f), new(0.05f, 0.35f, 0.6f), new("9da7a2"));
        for (int i = 0; i < 3; i++) Plank(parent, new(-0.4f, 0.17f + i * 0.12f, -0.5f));
        FoodSign(parent, "SAWMILL", 2.15f);
    }
    private void MakeLodge(Node3D parent, int stage)
    {
        MakeCottage(parent, stage);
        if (stage < 2) return;
        // Clapboard siding and a blue roof distinguish the four-bed lodge.
        for (int i = 0; i < 7; i++) Box(parent, new(0, 0.3f + i * 0.18f, -0.84f), new(2.68f, 0.14f, 0.04f), new("d4ac70"));
        if (stage < 3) return;
        foreach (float side in new[] { -1f, 1f })
        {
            var roof = Box(parent, new(0, 1.97f, side * 0.53f), new(3.18f, 0.17f, 1.37f), new("587684"));
            roof.RotationDegrees = new(side * 32, 0, 0);
        }
        Box(parent, new(0, 0.12f, 1.0f), new(2.8f, 0.2f, 0.4f), new("d4ac70"));
        FoodSign(parent, "LODGE · 4 BEDS", 2.65f);
    }
}
