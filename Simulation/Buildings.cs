using System;

namespace Inlanders.Simulation;

public sealed record BuildingDefinition(string Name, int Cost, Resource Material, int Beds = 0, Role? Worker = null, int Slots = 0)
{
    public float ConstructionSeconds => 12;
    public string CostText => $"{Cost} {Material.ToString().ToLowerInvariant()}";
}

public static class Buildings
{
    // Indexed by the stable BuildingKind enum. No mutable balancing state in a save.
    private static readonly BuildingDefinition[] Definitions =
    {
        new("Cottage", 6, Resource.Logs, Beds: 2),
        new("Forager hut", 4, Resource.Logs, Worker: Role.Forager, Slots: 2),
        new("Farm", 4, Resource.Logs, Worker: Role.Farmer, Slots: 1),
        new("Bakery", 8, Resource.Logs, Worker: Role.Baker, Slots: 1),
        new("Sawmill", 6, Resource.Logs, Worker: Role.Sawyer, Slots: 1),
        new("Lodge", 12, Resource.Planks, Beds: 4),
        new("Village square", 6, Resource.Logs),
        new("Bridge", 6, Resource.Logs),
        new("Stockpile", 4, Resource.Logs, Worker: Role.Hauler),
        new("Vegetable garden", 4, Resource.Logs, Worker: Role.Farmer, Slots: 1),
        new("Fishing dock", 8, Resource.Logs, Worker: Role.Fisher, Slots: 1)
    };
    public static BuildingDefinition Get(BuildingKind kind) => (uint)kind < Definitions.Length
        ? Definitions[(int)kind] : throw new ArgumentOutOfRangeException(nameof(kind));
}
