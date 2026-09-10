using Godot;
using Inlanders.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Game
{
    private readonly Dictionary<int, (Node3D Body, int Stage)> _cropViews = new();
    private readonly Dictionary<int, (Node3D Body, int Ripe)> _bushViews = new();
    private Node3D _pantry = null!;
    private string _pantryKey = "";
    private void CreateFoodViews()
    {
        _cropViews.Clear(); _bushViews.Clear(); _pantryKey = "";
        _pantry = new(); _dynamic.AddChild(_pantry);
    }
    private void FoodSign(Node3D parent, string text, float height)
    {
        parent.AddChild(new Label3D { Text = text, Position = new(0, height, 0), FontSize = 25, PixelSize = 0.009f,
            Billboard = BaseMaterial3D.BillboardModeEnum.Enabled, Modulate = _cream, OutlineSize = 5 });
    }
    private void MakeBuilding(Node3D parent, Cottage site, int stage)
    {
        MakeBuildingPieces(parent,site,stage);
        BatchStaticGeometry(parent);
    }
    private void MakeBuildingPieces(Node3D parent, Cottage site, int stage)
    {
        if (site.Kind == BuildingKind.FishingDock) { MakeFishingDock(parent,stage); return; }
        if (site.Kind == BuildingKind.Stockpile) { MakeStockpile(parent, site, stage); return; }
        if (site.Kind == BuildingKind.Bridge)
        {
            foreach (float x in new[] { -0.4f, 0.4f })
                Box(parent, new(x, -0.03f, 0), new(0.12f, 0.15f, 1.9f), _wood);
            if (stage >= 1)
                for (int i = 0; i < (stage >= 2 ? 9 : 4); i++)
                    Box(parent, new(0, 0.025f, -0.85f + i * 0.21f), new(0.95f, 0.06f, 0.18f), new("c4a77b"));
            if (stage >= 3)
                foreach (float x in new[] { -0.48f, 0.48f })
                {
                    foreach (float z in new[] { -0.8f, 0.8f })
                        Box(parent, new(x, 0.27f, z), new(0.08f, 0.55f, 0.08f), _wood);
                    Box(parent, new(x, 0.52f, 0), new(0.06f, 0.08f, 1.9f), _wood);
                }
            return;
        }
        if (site.Kind == BuildingKind.Square)
        {
            Box(parent, new(0, 0.04f, 0), new(2.8f, 0.08f, 1.8f), new("b9aa86"));
            if (stage >= 1)
                foreach (float x in new[] { -0.8f, 0.8f })
                    Box(parent, new(x, 0.35f, 0), new(0.15f, 0.65f, 0.6f), _wood);
            if (stage >= 2) Box(parent, new(0, 0.7f, 0), new(2.2f, 0.12f, 0.7f), _wood);
            if (stage >= 3)
            {
                foreach (float z in new[] { -0.65f, 0.65f })
                    Box(parent, new(0, 0.3f, z), new(2.2f, 0.15f, 0.25f), _wood);
                FoodSign(parent, "VILLAGE SQUARE", 1.7f);
            }
            return;
        }
        if (site.Kind == BuildingKind.Bakery) { MakeBakery(parent, site, stage); return; }
        if (site.Kind == BuildingKind.Sawmill) { MakeSawmill(parent, site, stage); return; }
        if (site.Kind == BuildingKind.Lodge) { MakeLodge(parent, stage); return; }
        if (site.Kind == BuildingKind.VegetableGarden) { MakeVegetableGarden(parent, stage); return; }
        if (site.Kind == BuildingKind.Farm) { MakeFarm(parent, stage); return; }
        if (site.Kind == BuildingKind.ForagerHut) { MakeForagerHut(parent, stage); return; }
        MakeCottage(parent, stage, site.Id % 3);
    }
    private void RenderFoodViews()
    {
        RenderSupplyRoutes();
        RenderManagedWoodland();
        RenderFishingGrounds();
        foreach (var bush in _world.Bushes)
        {
            if (_bushViews.TryGetValue(bush.Id, out var old) && old.Ripe == bush.Ripe) continue;
            if (old.Body != null) old.Body.QueueFree();
            var body = new Node3D { Position = OnGround(bush.Cell.X, bush.Cell.Z) }; _dynamic.AddChild(body);
            Mesh(body, new SphereMesh { Radius = 0.45f, Height = 0.75f, RadialSegments = 7, Rings = 4 }, new(0, 0.3f, 0), new("496d48"));
            for (int i = 0; i < bush.Ripe; i++)
            {
                float angle = i * Mathf.Tau / 8;
                Mesh(body, new SphereMesh { Radius = 0.07f, Height = 0.14f, RadialSegments = 5, Rings = 3 }, new(MathF.Cos(angle) * 0.34f, 0.47f, MathF.Sin(angle) * 0.34f), new("b85877"));
            }
            _bushViews[bush.Id] = (body, bush.Ripe);
        }
        foreach (int id in _cropViews.Keys.Where(id => !_world.Cottages.Any(c => c.Id == id)).ToArray()) { _cropViews[id].Body.QueueFree(); _cropViews.Remove(id); }
        foreach (int id in _cropViews.Keys.Where(id => !_world.Cottages.Any(c => c.Id == id)).ToArray()) { _cropViews[id].Body.QueueFree(); _cropViews.Remove(id); }
        foreach (var farm in _world.Cottages.Where(c => (c.Kind is BuildingKind.Farm or BuildingKind.VegetableGarden) && c.Complete))
        {
            int stage = farm.Harvest > 0 ? 4 : farm.Planted ? 1 + (int)(farm.Growth * 2.9f) : 0;
            int viewKey=stage*10+farm.Harvest;
            if (_cropViews.TryGetValue(farm.Id, out var old) && old.Stage == viewKey) continue;
            if (old.Body != null) old.Body.QueueFree();
            var root = new Node3D { Position = OnGround(farm.Cell.X + (farm.Rotated ? -0.5f : 0), farm.Cell.Z + (farm.Rotated ? 0 : -0.5f)), RotationDegrees = new(0, farm.Rotated ? 90 : 0, 0) };
            _dynamic.AddChild(root);
            if (farm.Kind == BuildingKind.VegetableGarden) MakeVegetables(root, farm, stage);
            else MakeCrops(root, farm, stage);
            _cropViews[farm.Id] = (root, viewKey);
        }
        string key = $"{_world.Food.Berries}/{_world.Food.Grain}/{_world.Food.Bread}/{_world.Food.Vegetables}/{_world.Food.Fish}";
        if (_pantryKey == key) return;
        _pantryKey = key; Clear(_pantry);
        if(_world.Map.FishingGrounds.Count>0 || _world.Food.CaughtFish>0)
        {
            Cylinder(_pantry,new(-1.2f,.24f,3.85f),.22f,.35f,new("a58256"));
            if(_world.Food.Fish>0) MakeFish(_pantry,new(-1.2f,.43f,3.85f));
        }
        // The pantry shares the timber yard; displayed baskets summarize its inventories.
        foreach (var (amount, color, x) in new[] { (_world.Food.Berries, new Color("a95172"), -3.6f), (_world.Food.Grain, new Color("dabb69"), -3.0f), (_world.Food.Bread, new Color("cf914e"), -2.4f), (_world.Food.Vegetables, new Color("d88739"), -1.8f) })
        {
            Cylinder(_pantry, new(x, 0.24f, 3.85f), 0.22f, 0.35f, new("a58256"));
            if (amount > 0) Mesh(_pantry, new SphereMesh { Radius = 0.19f, Height = 0.22f, RadialSegments = 7, Rings = 3 }, new(x, 0.43f, 3.85f), color);
        }
    }
}
