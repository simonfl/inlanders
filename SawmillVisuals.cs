using Godot;
using Inlanders.Simulation;

public partial class Game
{
    private void Plank(Node3D parent, Vector3 at)
    {
        Box(parent, at, new(0.85f, 0.10f, 0.21f), new("d4ac70"));
        Box(parent, at + new Vector3(0, 0.051f, 0), new(0.72f, 0.005f, 0.015f), new("ae8554"));
    }
    private void MakeSawmill(Node3D parent, Cottage site, int stage)
    {
        // Open structural bays keep the cutting line visible beneath a steep half roof.
        foreach (float x in new[] { -1.18f, 1.18f }) foreach (float z in new[] { -.73f, .30f })
            StoneFoot(parent, new(x, .12f, z), new(.38f, .22f, .38f));
        if (stage == 0) return;
        foreach (float x in new[] { -1.18f, 1.18f })
        {
            Box(parent, new(x, 1.03f, -.73f), new(.23f, 1.90f, .23f), _frameTimber);
            Box(parent, new(x, 1.03f, .30f), new(.23f, 1.90f, .23f), _frameTimber);
            TimberBeam(parent, new(x, 1.35f, .30f), new(x, 1.95f, -.26f), .17f, _frameTimber);
            Box(parent, new(x, 1.99f, -.22f), new(.24f, .22f, 1.45f), _frameTimber);
        }
        Box(parent, new(0, 1.99f, -.73f), new(2.6f, .22f, .23f), _frameTimber);
        foreach (float side in new[] { -1f, 1f })
            TimberBeam(parent, new(side * 1.18f, 1.3f, -.73f), new(side * .57f, 1.99f, -.73f), .17f, _frameTimber);
        if (stage == 1) return;
        Box(parent, new(0, .65f, .56f), new(2.27f, .22f, .58f), new("96734d"));
        foreach (float x in new[] { -.85f, .85f })
        {
            Box(parent, new(x, .35f, .56f), new(.22f, .66f, .52f), _frameTimber);
            Box(parent, new(x, .11f, .56f), new(.52f, .17f, .67f), _frameTimber);
        }
        // Tall reciprocating saw frame on the front edge; the worker stands just beyond it.
        foreach (float x in new[] { -.30f, .30f }) Box(parent, new(x, 1.06f, .58f), new(.14f, 1.14f, .14f), _frameTimber);
        Box(parent, new(0, 1.67f, .58f), new(.83f, .18f, .20f), _frameTimber);
        if (stage < 3) return;
        VillageRoof(parent, new(0, 2.03f, -.30f), 2.87f, 1.30f, .65f, new("75878a"), false);
        foreach (float x in new[] { -1.18f, 1.18f })
            TimberBeam(parent, new(x, 2.02f, -.30f), new(x, 2.65f, -.30f), .16f, _frameTimber);
        var blade = new Node3D { Name = "SawBlade" }; parent.AddChild(blade);
        Box(blade, new(0, 1.11f, .58f), new(.07f, .73f, .23f), new("b0b7ad"));
        Box(blade, new(0, 1.47f, .58f), new(.43f, .09f, .15f), _frameTimber);
        for (int i = 0; i < site.InputLogs; i++)
        {
            var log = new Node3D { Name = "SawLog" + i }; parent.AddChild(log);
            Log(log, new(-.52f, .88f + i * .25f, .54f), .95f);
        }
        for (int i = 0; i < site.OutputPlanks; i++)
        {
            var plank = new Node3D { Name = "SawPlank" + i }; parent.AddChild(plank);
            Plank(plank, new(.77f, .86f + i * .115f, .53f));
        }
        FoodSign(parent, "SAWMILL", 3.02f);
    }
    private void MakeLodge(Node3D parent, int stage)
    {
        MakeLegacyCottage(parent, stage);
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
