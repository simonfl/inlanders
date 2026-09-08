# Inlanders

A personal Windows town-building game inspired by Outlanders, built with **Godot 4.6 and C# / .NET 8**. All visuals are original procedural geometry.

![A settlement after its first village supper](docs/images/settlement.png)

## Run from a fresh clone

In PowerShell:

```powershell
git clone https://github.com/simonfl/inlanders.git
cd inlanders
powershell -NoProfile -ExecutionPolicy Bypass -File Setup.ps1
.\Play.cmd
```

Setup downloads portable Godot 4.6 and .NET SDK 8.0.424 from their official distributions into `.tools`. It requires network access once. Afterwards, double-click **Play.cmd** to compile and play offline. No global Godot/.NET installation or PATH changes are required. On the original development machine the tools are already installed.

The launcher runs the Godot project directly; this repository does not contain an exported standalone executable. Downloaded tools, generated build files, test artifacts, and player saves are excluded from Git.

## The first village supper

Eight villagers arrive with 24 berries. The objective is to **house all eight people and stock 16 loaves**, then click **Host supper** to gather everyone. The game continues after the celebration.

1. In the **Build** tab, select a **Forager hut**, then click a clear site. Start this early to replenish the initial food supply.
2. Build a **Farm** and a **Bakery**. A farmer sows grain, waits for it to ripen, harvests it, and hauls it to the pantry. A baker collects grain, bakes it, and carries bread back.
3. Build four **Cottages**, each housing two villagers. Every building costs six logs; the six initial harvestable alders provide 48 logs. Plant more alders when you want to expand.
4. When everyone has shelter and 16 loaves are in the pantry, host the supper. Villagers return carried goods, gather, and celebrate before resuming their jobs.

The initial workforce is two loggers, two builders, two foragers, one farmer, and one baker. Change allocations with **+ / −** in the **Workforce** tab. Minus unassigns a worker. Plus uses an unassigned worker first, then transfers someone from another job. Select a villager to inspect their task, waiting reason, cargo, and claims; the assignment button cycles that individual's role.

### Food and work

- One game day lasts 60 simulation seconds. Eight food units are consumed each day, using berries before bread. Raw grain is not edible.
- Berry bushes regenerate. A forager hut supports two foragers; each farm and bakery supports one active worker at a time.
- A planted crop takes 45 seconds to ripen and yields six grain. Two grain bake into four loaves in ten seconds.
- Missed meals reduce movement and work speed, down to 50% when everyone goes hungry. Nobody dies; food production can recover the settlement.
- Food physically travels from source to storage and from storage to production buildings. Goods in transit or still inside a bakery cannot be eaten or used for the supper.

### Construction management

Select a building in the queue to change its **Low / Normal / High** construction priority. New job claims favor higher priorities, then older plans. Already committed deliveries finish before workers choose another job.

Cancel an unfinished plan to release its claims. Carried timber returns to the yard; delivered timber remains as a salvage pile for loggers to collect. After collection, the site can be reused. Completed buildings cannot be cancelled.

Pale placement cells are legal; red cells are blocked. The separate small square marks the entrance. Placement protects workers, entrances, and resource access, and recalculates routes around new plans. Border trees are decorative. Villagers can pass through one another.

### Renewable woodland

In the **Build** tab, choose **Plant alders**, or press **T**. Click open ground or a fully harvested stump to mark planting spots; press **Esc** when finished. Planting is free and protects the same worker routes and entrances as construction.

Loggers plant marked spots before taking new harvesting jobs. Each planting takes four work seconds, then the sapling grows over **three game days** into an alder yielding **eight logs**. Growth continues independently of staffing and hunger, but pauses with the game. Saplings visibly grow, and the resource header counts waiting planting jobs and growing trees.

Once all logs have been collected, you can mark the stump again for another cycle. Replanting is manual; there is no automatic forestry zone or planting cancellation yet. Planting jobs, growth, and new timber are saved, and saves from before this feature still load.

## Controls and saves

| Control | Action |
| --- | --- |
| Left click | Place a plan or select a villager/building |
| B | Toggle placement for the selected building type |
| T | Toggle repeat tree planting on open ground or exhausted stumps |
| R | Rotate the unplaced building |
| Esc | Cancel placement preview |
| WASD | Pan |
| Q / E | Orbit in quarter turns |
| Mouse wheel | Zoom |
| Space / Pause | Pause or resume |
| Speed | Cycle 1×, 3×, 6× |
| F5 / Save | Save the current settlement |
| F9 / Load | Restore the saved settlement, paused |
| Start again | Start a new settlement without deleting the save |

The manual save is `saves/settlement.json`; the previous save is retained as `.bak`. Saves preserve simulation time, hunger, food inventories, crop growth, bakery batches, workers' positions/routes/tasks, reservations, construction, and supper progress. Loading validates the save before replacing the live game. Camera position and playback speed remain local view settings. There is no autosave.

## Development

See the [feature roadmap](docs/ROADMAP.md) for future ideas and selectable work chunks. Pick a feature ID; flesh out its first playable version when we start it.

| File | Responsibility |
| --- | --- |
| `Simulation/Settlement.cs` | Fixed-step simulation, grid A*, placement, logging, construction, reservations |
| `Simulation/Food.cs` | Foraging, farming, baking, meals, hunger, supper |
| `Simulation/Woodland.cs` | Planting sites, sapling growth, renewable timber accounting |
| `Simulation/Saving.cs` | Versioned JSON saves, validation, file replacement/backup |
| `Game.cs` | Input, actor views, scene lifecycle, simulation/render coordination |
| `Visuals.cs`, `FoodVisuals.cs` | Procedural geometry, lighting, crops, pantry |
| `Hud.cs`, `PersistenceUi.cs` | Workforce, construction queue, inspectors, save/load feedback |
| `Smoke.cs`, `Smoke3.cs`, `SmokeWoodland.cs` | Rendered interaction checks |
| `Tests/Checks.cs`, `Tests/FoodChecks.cs`, `Tests/WoodlandChecks.cs` | Simulation and persistence tests |

Simulation advances in fixed 0.1-second steps on one thread. Job claims reserve resources and destination capacity together. Harvesting, construction, and food production have explicit ownership/worker limits. Reassignment releases claims and returns cargo physically. `World.Validate()` checks resource accounting, ownership, capacity, live targets, and routes. The C# simulation has no Godot dependencies.

`NuGet.Config` uses packages bundled with the portable Godot download. The launch/test scripts scope .NET and app-data settings to their process. To work in the Godot editor, use the .NET executable in `.tools/godot/` with the environment configured in `Play.ps1`, and open `project.godot`. Scene content is generated in C# at runtime.

## Verification

```powershell
# Simulation, resource reservations, and save/load tests
powershell -NoProfile -ExecutionPolicy Bypass -File Test.ps1

# Also compile and exercise the actual rendered game
powershell -NoProfile -ExecutionPolicy Bypass -File Test.ps1 -Rendered
```

Tests cover legal placements, competing workers, scarce timber, priorities, reassignment, cancellation/salvage, seeded stress runs, food conservation, hunger recovery, supper completion, exact save/load continuation through every food-production phase, corrupted saves, and disk backups. The rendered check exercises the UI, all building types, active-batch save/load, the supper gathering, and restoration of a completed scenario. Its saves and screenshots go to `artifacts/`, separate from player saves.

Milestones 1–3 are implemented: the first cottage, eight competing workers, and a complete food/supper scenario with persistence. Population growth, seasons, sound, and further animation/presentation polish remain future work.
