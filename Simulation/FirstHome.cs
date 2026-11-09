using System;
using System.Collections.Generic;
using System.Linq;

namespace Inlanders.Simulation;

public sealed class FirstHomeProgress
{
    public int? NewHomeId { get; set; }
    public HashSet<int> Served { get; set; } = new();
    public bool Complete => Served.Contains(2) && Served.Contains(3);
}

public sealed record FirstHomeObjectives(string Home, string Food, string Meals, string Hint, string Action, int? Site, int Done);

public sealed partial class World
{
    public FirstHomeObjectives ReadFirstHomeObjectives()
    {
        var story = Founding!.FirstHome!;
        var homes = Cottages.Where(c => c.Kind == BuildingKind.Cottage && !c.DemolitionRequested &&
            !Founding.StartingBuildings.Any(s => s.Id == c.Id)).ToArray();
        bool housed = People.Where(p => p.Id >= 2).All(p => homes.Any(c => c.Id == p.HomeId && IsHome(c) && Accessible(c.Entrance)));
        var home = homes.OrderByDescending(c => c.Complete).FirstOrDefault();
        var foods = Cottages.Where(c => !c.DemolitionRequested && ProductionOutput(c.Kind) is {} k && EdibleKinds.Contains(k)).ToArray();
        var food = foods.OrderByDescending(c => WorkplaceFoodDelivered(c.Id)).ThenByDescending(c => c.Complete).FirstOrDefault();
        bool produced = DeliveredEdible > 0;
        if (story.Complete) { housed = true; produced = true; }
        string Stage(Cottage? c) => c == null ? "Not planned" : !Accessible(c.Entrance) ? "No route" :
            c.ConstructionPaused && !c.Complete ? "Construction paused" : c.Complete ? "Ready" :
            !c.MaterialsReady ? $"Timber {c.Delivered}/{c.Required}" : $"Building {c.Construction:P0}";
        string hint; string action; int? site = null;
        var pending = !housed ? home : !produced ? food : null;
        if (story.Complete) { hint = "They have a home and have both eaten fresh food. You can keep playing."; action = "done"; }
        else if (!housed && home == null) { hint = "Choose a spot for their cottage. The villagers do the building."; action = "home"; }
        else if (housed && food == null) { hint = "Choose one food source. Villagers work it automatically."; action = "food"; }
        else if (pending != null && (!Accessible(pending.Entrance) || pending.ConstructionPaused || pending.WorkPaused))
        { hint = "Inspect the blocked or paused building to resume progress."; action = "inspect"; site = pending.Id; }
        else if (pending is { Complete: false, MaterialsReady: false } && Available < pending.Remaining(Resource.Logs) &&
            !Trees.Any(t => !t.Preserved && t.Logs > 0 && Accessible(t.Access)))
        { hint = "Waiting for timber. Mark a grove to harvest and renew."; action = "grove"; }
        else if (pending != null)
        { hint = pending.Complete ? ReadWorkplace(pending).State + ". " + ReadWorkplace(pending).Detail : "Villagers collect the timber and build automatically."; action = "inspect"; site = pending.Id; }
        else { hint = "Jeanne and Pierre eat automatically when hungry. Starting provisions do not count as fresh food."; action = "watch"; }
        return new(housed ? "✓ Cottage ready" : "○ Cottage · " + Stage(home),
            produced ? "✓ Food produced" : "○ Food source · " + Stage(food),
            $"{(story.Complete ? "✓" : "○")} Fresh meals · {story.Served.Count}/2 residents",
            hint, action, site, (housed ? 1 : 0) + (produced ? 1 : 0) + (story.Complete ? 1 : 0));
    }

    public static World NewFirstHome()
    {
        var world = NewRiverFarmstead();
        world.People.Clear();
        var names = new[] { "Marie", "Louis", "Jeanne", "Pierre" };
        for (int id = 0; id < names.Length; id++)
        {
            world.People.Add(new Villager
            {
                Id = id,
                Name = names[id],
                Position = world.Cottages[0].Entrance.Point,
                NextMealTime = 25 + id * 5,
                Role = Role.Unassigned,
                SharedWorker = true
            });
        }
        world.Founding!.TransformationHamlet = true;
        world.Founding.FirstHome = new();
        world.Founding.StartingBuildings = world.Cottages.Select(home =>
            new StartingBuilding { Id = home.Id, Cell = home.Cell, Rotation = home.Rotation, Kind = home.Kind }).ToList();
        foreach (var tree in world.Trees) tree.Preserved = true;
        world._yardLogs = 0;
        world.InitialLogs = world.Trees.Sum(tree => tree.Logs) + world.Cottages.Sum(home => home.Delivered);
        world.Food.InitialBerries = world.Food.Berries = 20;
        world.Map.Name = "Une première maison · A first home";
        world.History.Clear();
        world.History.Add("Marie and Louis have offered Jeanne and Pierre a place to begin. Their stores will feed everyone for a while. Choose a little ground, take timber from a grove, and help the pair make a home and a living here.");
        world.ReconcileHomes();
        world.Validate();
        world.ValidateMapOccupancy();
        return world;
    }

    private void AdvanceFirstHome()
    {
        if (Founding?.FirstHome is not { NewHomeId: null } story) return;
        var home = Cottages.FirstOrDefault(site => IsHome(site) &&
            !Founding.StartingBuildings.Any(start => start.Id == site.Id) &&
            People.Any(person => person.Id >= 2 && person.HomeId == site.Id));
        if (home == null || !Accessible(home.Entrance)) return;
        story.NewHomeId = home.Id;
        foreach (var person in People.Where(person => person.Id >= 2 && person.HomeId == home.Id))
        {
            // A committed trip or load finishes normally. The next free resident visits their home.
            if (person.Task == Work.Waiting)
                Go(person, home.Entrance, Work.ToArrival, "Moving into our new home");
        }
        History.Add("Jeanne and Pierre have a home of their own. Their first meal from this land will make it a beginning.");
    }

    private void RecordFirstHomeMeal(Villager person, MealRequest meal)
    {
        if (Founding?.FirstHome is not {} story || person.Id < 2 || person.HomeId is not int homeId ||
            Founding.StartingBuildings.Any(start => start.Id == homeId) ||
            !Cottages.Any(home => home.Id == homeId && IsHome(home) && Accessible(home.Entrance)))
            return;
        // Only berries exist in the opening stores. Other foods remain fresh produce after hauling.
        bool fromProducer = meal.SourceId is int source && Cottages.Any(site => site.Id == source &&
            ProductionOutput(site.Kind) == meal.Kind);
        if (meal.Kind == Resource.Berries && !fromProducer && Food.EatenBerries <= Food.InitialBerries) return;
        if (story.Served.Add(person.Id))
            History.Add(person.Name + " has eaten a first meal from the new livelihood.");
    }

    private void ValidateFirstHome()
    {
        if (Founding?.FirstHome is not {} story) return;
        if (Population != 4 || Creative || Challenge != null || !Founding.TransformationHamlet ||
            story.Served == null || story.Served.Any(id => id is not (2 or 3)) ||
            story.NewHomeId is int id && (id < 1 || id >= _nextSite))
            throw new InvalidOperationException("Invalid first-home commission");
    }
}
