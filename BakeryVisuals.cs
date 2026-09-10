using Godot;
using Inlanders.Simulation;

public partial class Game
{
    private void MakeBakery(Node3D parent, Cottage site, int stage)
    {
        var masonry = new Color("bc9572");
        var brick = new Color("997353");
        StoneFoot(parent, new(-.55f, .13f, -.22f), new(1.65f, .23f, 1.27f));
        StoneFoot(parent, new(.83f, .20f, .04f), new(1.08f, .36f, 1.52f));
        StoneFoot(parent, new(-.50f, .07f, .70f), new(1.40f, .10f, .44f));
        if (stage < 1) return;
        foreach (float x in new[] { -1.27f, .20f }) foreach (float z in new[] { -.76f, .45f })
            Box(parent, new(x, .85f, z), new(.16f, 1.55f, .16f), _frameTimber);
        Box(parent, new(-.53f, 1.59f, .45f), new(1.7f, .18f, .19f), _frameTimber);
        Box(parent, new(.86f, .86f, -.40f), new(.62f, 1.38f, .68f), brick);
        if (stage < 2) return;
        // The serving recess is empty space between wall piers, not a painted window.
        Box(parent, new(-.54f, .90f, -.42f), new(1.42f, 1.32f, .60f), new("dbc8a4"));
        foreach (float x in new[] { -1.20f, .12f })
            Box(parent, new(x, .91f, .16f), new(.19f, 1.32f, .58f), new("dbc8a4"));
        Box(parent, new(-.54f, 1.03f, -.10f), new(1.12f, .70f, .035f), _recess);
        Box(parent, new(-.54f, .52f, .28f), new(1.20f, .51f, .30f), brick);
        Box(parent, new(-.54f, .81f, .36f), new(1.43f, .14f, .51f), new("b5976b"));
        // A broad oven shoulder merges into a stout chimney above the lower shop roof.
        Box(parent, new(.83f, .56f, .05f), new(.98f, .52f, 1.24f), brick);
        Mesh(parent, new SphereMesh { Radius = .62f, Height = 1.24f, RadialSegments = 8, Rings = 4 }, new(.80f, .90f, .03f), masonry);
        Box(parent, new(.84f, 1.81f, -.40f), new(.59f, 2.04f, .65f), masonry);
        Box(parent, new(.84f, 2.53f, -.40f), new(.63f, .15f, .69f), brick);
        Box(parent, new(.84f, 2.87f, -.40f), new(.77f, .18f, .82f), brick);
        Box(parent, new(.84f, 2.965f, -.40f), new(.39f, .02f, .44f), _recess);
        Box(parent, new(.80f, .79f, .64f), new(.49f, .52f, .06f), _recess);
        foreach (float x in new[] { .48f, 1.12f }) Box(parent, new(x, .76f, .66f), new(.17f, .65f, .18f), masonry.Lightened(.12f));
        Box(parent, new(.80f, 1.10f, .63f), new(.80f, .18f, .24f), masonry.Lightened(.12f));
        StoneFoot(parent, new(.80f, .44f, .74f), new(.88f, .14f, .43f));
        if (stage < 3) return;
        VillageRoof(parent, new(-.54f, 1.63f, -.26f), 1.85f, 1.29f, .38f, new("687d70"), true);
        var canopy = Box(parent, new(-.54f, 1.49f, .62f), new(1.77f, .12f, .60f), new("bc9b63"));
        canopy.RotationDegrees = new(13, 0, 0);
        Box(parent, new(-.54f, 1.37f, .91f), new(1.77f, .16f, .08f), new("ae784f"));
        foreach (float x in new[] { -1.27f, .20f })
            TimberBeam(parent, new(x, 1.11f, .42f), new(x, 1.47f, .84f), .10f, _frameTimber);
        for (int i = 0; i < site.InputGrain; i++)
        {
            var sack = Mesh(parent, new SphereMesh { Radius = .18f, Height = .40f, RadialSegments = 7, Rings = 4 }, new(-1.18f + i * .36f, .28f, .73f), new("c9b68a"));
            sack.Name = "GrainSack" + i;
        }
        for (int i = 0; i < site.OutputBread; i++)
        {
            var loaf = Mesh(parent, new SphereMesh { Radius = .16f, Height = .21f, RadialSegments = 8, Rings = 4 }, new(-.99f + i * .30f, .98f, .39f), new("dfa653"));
            loaf.Scale = new(1, 1, .8f); loaf.Name = "BreadLoaf" + i;
        }
        var glow = Box(parent, new(.80f, .66f, .68f), new(.39f, .12f, .025f), new("eb9f50"));
        glow.Name = "OvenGlow"; glow.Visible = false;
        var glowMaterial = (StandardMaterial3D)glow.MaterialOverride;
        glowMaterial.EmissionEnabled = true; glowMaterial.Emission = new("bf6a2d");
        FoodSign(parent, "BAKERY", 3.25f);
    }
}
