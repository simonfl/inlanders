using Godot;
using Inlanders.Simulation;
using System;
using System.Linq;
using Resource = Inlanders.Simulation.Resource;

public partial class Game
{
    private sealed class PersonView
    {
        public Node3D Body = new(), Rig = new(), Torso = new(), Head = new(), Arm = new(), LeftArm = new(),
            LeftLeg = new(), RightLeg = new(), Carry = new(), Marker = null!, Axe = new(), Hammer = new(), Spade = new(), Peel = new();
        public Resource Cargo;
        public int Count = -1;
    }

    private PersonView MakeVillager(int id)
    {
        var v = new PersonView();
        v.Body.AddChild(v.Rig); v.Rig.AddChild(v.Torso); v.Torso.Position = new(0, 0.43f, 0);
        var shirts = new[] { "bf714d", "5c8390", "a19358", "7c6c92", "7a9161", "bd8a68", "af6971", "588c83" };
        Cylinder(v.Torso, new(0, 0.21f, 0), 0.26f, 0.48f, new(shirts[id]), 0.21f);
        v.Torso.AddChild(v.Head); v.Head.Position = new(0, 0.58f, 0);
        Mesh(v.Head, new SphereMesh { Radius = 0.20f, Height = 0.4f, RadialSegments = 8, Rings = 4 }, Vector3.Zero, new("e7bd8e"));
        Cylinder(v.Head, new(0, 0.19f, 0), 0.30f, 0.07f, new("dbc28c"));
        Cylinder(v.Head, new(0, 0.26f, 0), 0.19f, 0.15f, new("dbc28c"), 0.15f);
        foreach (var (leg, x) in new[] { (v.LeftLeg, -0.13f), (v.RightLeg, 0.13f) })
        {
            v.Rig.AddChild(leg); leg.Position = new(x, 0.4f, 0);
            Box(leg, new(0, -0.18f, 0), new(0.16f, 0.36f, 0.18f), new("494d45"));
            Box(leg, new(0, -0.35f, -0.05f), new(0.18f, 0.10f, 0.27f), new("614c3b"));
        }
        foreach (var (arm, x) in new[] { (v.LeftArm, -0.28f), (v.Arm, 0.28f) })
        {
            v.Torso.AddChild(arm); arm.Position = new(x, 0.36f, 0);
            Box(arm, new(0, -0.17f, 0), new(0.13f, 0.4f, 0.14f), new("e7bd8e"));
        }
        foreach (var tool in new[] { v.Axe, v.Hammer, v.Spade, v.Peel })
        {
            v.Arm.AddChild(tool); tool.Position = new(0, -0.32f, 0); tool.Visible = false;
            Box(tool, new(0, 0.1f, -0.15f), new(0.055f, 0.055f, 0.55f), _wood);
        }
        Box(v.Axe, new(-0.06f, 0.1f, -0.39f), new(0.25f, 0.10f, 0.18f), new("9ca8a4"));
        Box(v.Hammer, new(0, 0.1f, -0.37f), new(0.24f, 0.14f, 0.14f), new("737d7b"));
        Box(v.Spade, new(0, 0.1f, -0.43f), new(0.21f, 0.05f, 0.25f), new("89938a"));
        Box(v.Peel, new(0, 0.1f, -0.48f), new(0.32f, 0.04f, 0.34f), new("cba36d"));
        v.Torso.AddChild(v.Carry); v.Carry.Position = new(0, 0.12f, -0.43f);
        v.Marker = Cylinder(v.Body, new(0, 0.02f, 0), 0.36f, 0.02f, new("efd49c"));
        return v;
    }

    private void RefreshCargo(PersonView view, Villager worker)
    {
        view.Carry.Visible = worker.Carried > 0;
        if (view.Count == worker.Carried && view.Cargo == worker.Cargo) return;
        view.Count = worker.Carried; view.Cargo = worker.Cargo; Clear(view.Carry);
        if (worker.Carried == 0) return;
        if (worker.Cargo == Resource.Logs)
        {
            for (int i = 0; i < worker.Carried; i++) Log(view.Carry, new(0, i * 0.22f, 0), 0.72f);
            return;
        }
        // Shallow baskets frame their contents; each mound/sheaf/loaf represents one carried unit.
        Box(view.Carry, new(0, -0.05f, 0), new(0.67f, 0.08f, 0.40f), new("b18a53"));
        foreach (float z in new[] { -0.21f, 0.21f }) Box(view.Carry, new(0, 0.02f, z), new(0.71f, 0.16f, 0.04f), new("c5a16d"));
        foreach (float x in new[] { -0.34f, 0.34f }) Box(view.Carry, new(x, 0.02f, 0), new(0.04f, 0.16f, 0.42f), new("c5a16d"));
        for (int i = 0; i < worker.Carried; i++)
        {
            float x = worker.Carried == 1 ? 0 : -0.17f + i * 0.34f;
            if (worker.Cargo == Resource.Berries)
                for (int b = 0; b < 5; b++) Mesh(view.Carry, new SphereMesh { Radius = 0.075f, Height = 0.15f, RadialSegments = 6, Rings = 3 },
                    new(x + (b % 2 - 0.5f) * 0.12f, 0.10f + b / 4 * 0.10f, (b / 2 % 2 - 0.5f) * 0.14f), new("a74268"));
            else if (worker.Cargo == Resource.Grain)
                for (int s = 0; s < 4; s++)
                {
                    var stalk = Cylinder(view.Carry, new(x + (s - 1.5f) * 0.045f, 0.18f, 0), 0.018f, 0.4f, new("d5b55e"));
                    stalk.RotationDegrees = new(0, 0, (s - 1.5f) * 12);
                    Mesh(view.Carry, new SphereMesh { Radius = 0.048f, Height = 0.15f, RadialSegments = 5, Rings = 3 }, new(x + (s - 1.5f) * 0.075f, 0.39f, 0), new("e4c87a"));
                }
            else
            {
                var loaf = Mesh(view.Carry, new SphereMesh { Radius = 0.14f, Height = 0.23f, RadialSegments = 8, Rings = 4 }, new(x, 0.11f, 0), new("cb8844"));
                loaf.Scale = new(1, 1, 1.45f);
                for (int cut = 0; cut < 3; cut++) Box(view.Carry, new(x, 0.22f, -0.09f + cut * 0.09f), new(0.13f, 0.015f, 0.025f), new("f0ce8c"));
            }
        }
    }

    private void AnimateVillager(PersonView view, Villager v)
    {
        RefreshCargo(view, v); view.Marker.Visible = v.Id == _selectedPerson;
        bool walking = v.Route.Count > 0;
        float cycle = _clock * 8 + v.Id * 1.7f, swing = MathF.Sin(cycle);
        view.Rig.Position = new(0, walking ? MathF.Abs(swing) * 0.035f : 0, 0);
        view.Torso.Rotation = Vector3.Zero; view.Head.Rotation = Vector3.Zero;
        view.LeftLeg.Rotation = new(walking ? swing * 0.55f : 0, 0, 0);
        view.RightLeg.Rotation = -view.LeftLeg.Rotation;
        view.Arm.Rotation = new(walking ? -swing * 0.35f : 0, 0, 0);
        view.LeftArm.Rotation = -view.Arm.Rotation;
        view.Axe.Visible = view.Hammer.Visible = view.Spade.Visible = view.Peel.Visible = false;
        if (v.Carried > 0)
        {
            view.Arm.Rotation = view.LeftArm.Rotation = new(1.05f, 0, 0);
            view.Torso.Rotation = new(-0.08f, 0, 0); return;
        }
        if (walking) return;
        Cell? facing = v.TreeId is int tree ? _world.Trees.FirstOrDefault(t => t.Id == tree)?.Cell :
            v.BushId is int bush ? _world.Bushes.FirstOrDefault(b => b.Id == bush)?.Cell :
            (v.SiteId ?? v.WorkplaceId) is int site ? _world.Cottages.FirstOrDefault(c => c.Id == site)?.Cell : null;
        if (facing is Cell cell)
        {
            var direction = new Vector3(cell.X, 0, cell.Z) - view.Body.Position;
            if (direction.LengthSquared() > 0.01f) view.Body.Rotation = new(0, MathF.Atan2(-direction.X, -direction.Z), 0);
        }
        switch (v.Task)
        {
            case Work.Chopping:
                bool felling = _world.Trees.Any(t => t.Id == v.TreeId && !t.Felled);
                view.Axe.Visible = felling;
                view.Arm.Rotation = new(felling ? 1.0f + swing * 1.0f : 0.65f + swing * 0.25f, 0, 0);
                view.Torso.Rotation = new(felling ? -0.10f : -0.35f, swing * 0.08f, 0); break;
            case Work.Building:
                view.Hammer.Visible = true; view.Arm.Rotation = new(1.1f + MathF.Sin(cycle * 1.5f) * 0.55f, 0, 0); break;
            case Work.PlantingTree: case Work.Planting: case Work.Harvesting:
                view.Spade.Visible = true; view.Torso.Rotation = new(-0.4f - swing * 0.12f, 0, 0);
                view.Arm.Rotation = new(0.6f + swing * 0.35f, 0, 0); view.LeftArm.Rotation = new(0.5f, 0, 0); break;
            case Work.Foraging:
                view.Torso.Rotation = new(-0.18f, swing * 0.12f, 0);
                view.Arm.Rotation = new(1.05f + swing * 0.35f, 0, 0);
                view.LeftArm.Rotation = new(1.05f - swing * 0.35f, 0, 0); break;
            case Work.Baking:
                view.Peel.Visible = true; view.Arm.Rotation = new(0.8f + MathF.Sin(cycle * 0.5f) * 0.22f, 0, 0);
                view.Torso.Rotation = new(-0.1f - MathF.Sin(cycle * 0.5f) * 0.08f, 0, 0); break;
            case Work.Supper:
                view.Arm.Rotation = new(2.6f, 0, swing * 0.3f); view.LeftArm.Rotation = new(1.3f, 0, -swing * 0.2f); break;
            case Work.Waiting:
                float idle = (_clock + v.Id * 2.3f) % 14;
                view.Head.Rotation = new(0, MathF.Sin(_clock * 0.6f + v.Id) * 0.25f, 0);
                view.Torso.Rotation = new(0, 0, MathF.Sin(_clock * 0.8f + v.Id) * 0.025f);
                if (idle < 2) view.Arm.Rotation = new(2.7f, 0, -0.25f); // Adjust the brim of the hat.
                break;
        }
    }
}
