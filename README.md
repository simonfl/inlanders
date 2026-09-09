# Inlanders

A personal Windows town-building game inspired by Outlanders, built with **Godot 4.6 and C# / .NET 8**. Visuals and sound effects are generated procedurally, with no downloaded art or audio assets.

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

## Main menu

Launching opens a quiet, paused village behind the title screen:

- **Continue** restores the last settlement saved or opened, whether campaign, original map, or Three clearings. It opens paused. On older installations without a Continue snapshot, the newest existing settlement/campaign save is used.
- **Campaign** starts, resumes, or replays either available level and shows completed levels. Replay retains the preceding village, recoverable from Goals.
- **Free play** starts or resumes either map. Starting anew retains a separate previous-village copy, accessible through **Restore previous**; ordinary saving does not overwrite that copy.
- **Settings** controls Effects, Nature, and mute; these are shared with in-game sound settings.
- **Quit** exits the game.

In game, use **Options → Return to main menu**. This saves the current settlement and updates `saves/continue.json` before returning; a failed save keeps the village open. F5 also updates Continue. Closing the window directly does not save changes since your last save. Free-play previous-village copies use `.before-new`; restoring one also keeps the replaced save as `.before-restore`.

## Campaign: four settlements

Choose **Campaign** on the title screen or **Goals [G]** in game. All buildings and tools remain available.

1. **A place to stay:** build a forager hut, deliver 24 fresh berries, and house eight villagers.
2. **Bread for the table:** add a farm and bakery; deliver 16 loaves. Meals do not erase progress.
3. **Room among the trees:** build a sawmill and lodge, house eight, and have loggers plant four trees. Marking spots alone does not count; maturity is not required.
4. **A place for everyone:** build a village square, house eight, stock 16 bread, then host supper from Goals. Everyone gathers near the square before the campaign finishes.

The square costs six logs and needs no staff. Leave eight walkable tiles within four tiles of its entrance for guests. Contextual hints can be dismissed, disabled, or reopened. Finishing a settlement lets you keep playing, continue, or replay; replay retains the previous village for restoration.

Save compatibility is not guaranteed during prototyping; use a fresh campaign for this revised sequence. If an old campaign cannot load, the Campaign menu offers **Start fresh campaign**.

Campaign saves live in `saves/campaign.json`, with a `.bak` of the previous write. Switching settlements, replaying, completing a level, and F5 save campaign progress; F9 restores the latest campaign save while in campaign mode. Use Continue or Campaign on the title screen to resume, paused. Save with F5 or return to the main menu before closing if you want to retain changes since the last save. This is separate from the standalone manual save.

## A larger map: Three clearings

Open **Options [O] → Explore larger map** to start or resume a separate 32×32 landscape with an irregular outline, open building areas, 20 harvestable trees, and six berry patches. Eight villagers arrive with 64 berries. All buildings are available; the existing supper objective can give you a goal while you explore.

**Home** frames the whole map. WASD pans across its full extent, and the wheel zooms between building detail and a wide overview. Land ends at the visible stepped edge: missing cells cannot be built on, planted, or crossed. Tree/stump clearing is available; water, bridges, decorative landscaping, and elevation remain future features.

F5/F9 use `saves/three-clearings.json` on this map. Entering it saves the village you leave; **Return to original map** in Options saves the larger village and restores the original standalone save. Campaign levels remain available through Goals. To resume the large map after relaunching, use Continue or Free play on the title screen. Existing saves retain their original terrain rather than expanding automatically.

## The first village supper

Eight villagers arrive with 24 berries. The objective is to **house all eight people and stock 16 loaves**, then click **Host supper** to gather everyone. The game continues after the celebration.

1. Open **Build** (button or **B**), select a **Forager hut**, then click a clear site. Start this early to replenish the initial food supply.
2. Build a **Farm** and a **Bakery**. A farmer sows grain, waits for it to ripen, harvests it, and hauls it to the pantry. A baker collects grain, bakes it, and carries bread back.
3. Build four **Cottages**, each housing two villagers. Cottages and production buildings cost six logs; the six initial harvestable alders provide 48 logs. Plant more alders when you want to expand. A **Lodge** is an alternative with four beds, costing eight planks from a sawmill.
4. When everyone has shelter and 16 loaves are in the pantry, open **Goals** and host the supper. Villagers return carried goods, gather, and celebrate before resuming their jobs. The Goals button shows **Ready** when you qualify.

The initial workforce is two loggers, two builders, two foragers, one farmer, and one baker. Change allocations with **+ / −** in **People** (button or **V**). Minus unassigns a worker. Plus uses an unassigned worker first, then transfers someone from another job. Select a villager on the map or in People to inspect their task, waiting reason, and cargo; the assignment button cycles that individual's role.

### Food and work

- One game day lasts 60 simulation seconds. Eight food units are consumed each day, using berries before bread. Raw grain is not edible.
- Berry bushes regenerate. A forager hut supports two foragers; each farm and bakery supports one active worker at a time.
- A planted crop takes 45 seconds to ripen and yields six grain. Two grain bake into four loaves in ten seconds.
- Missed meals reduce movement and work speed, down to 50% when everyone goes hungry. Nobody dies; food production can recover the settlement.
- Food physically travels from source to storage and from storage to production buildings. Goods in transit or still inside a bakery cannot be eaten or used for the supper.

### Construction management

Select a building on the map or in the **Build** menu's building list to open its inspector and change **Low / Normal / High** construction priority. New job claims favor higher priorities, then older plans. Already committed deliveries finish before workers choose another job.

Cancel an unfinished plan to release its claims. Carried timber returns to the yard; delivered timber remains as a salvage pile for loggers to collect. After collection, the site can be reused. Completed buildings cannot be cancelled.

The Build menu explains each building's purpose, staffing, recipes, and available materials. You can place plans before you have enough supplies; builders wait for materials. Translucent building previews turn green on legal spots and red on blocked ones, with a specific explanation below. An arrow marks the entrance; R rotates the model and footprint together. Tree planting uses a sapling preview and stays active for repeated planting. Placement protects workers, entrances, and resource access, and recalculates routes around new plans. Border trees are decorative. Villagers can pass through one another.

### Renewable woodland

In **Build**, choose **Plant alders**, or press **T**. Click open ground or a fully harvested stump to mark planting spots; press **Esc** when finished. Planting is free and protects the same worker routes and entrances as construction.

Loggers take clearing orders first, then plant marked spots before ordinary harvesting jobs. Already committed work and deliveries finish first. Each planting takes four work seconds, then the sapling grows over **three game days** into an alder yielding **eight logs**. Growth continues independently of staffing and hunger, but pauses with the game or while marked for clearing. Saplings visibly grow; hover over Logs in the top bar for clearing, planting, growth, and reservation counts.

Once all logs have been collected, you can mark the stump again for another cycle. Replanting is manual; there is no automatic forestry zone. You can remove an unwanted planting marker with a clearing order. Planting jobs, growth, and new timber are saved, and saves from before this feature still load.

### Clear trees and stumps

Open **Build → Clear trees & stumps**, or press **C**. Click a tree, sapling, planting marker, or exhausted stump to queue clearing. Amber crosses mark orders. Click a marked target again to cancel its order; **Esc** finishes using the tool without canceling queued work.

Assigned loggers prioritize these orders, harvest and physically haul existing timber, then spend four work seconds removing roots. The cell stays blocked until root work finishes; afterward it can be built on or replanted, subject to normal placement rules. Timber already being carried still travels to storage normally. Saplings and empty planting markers produce no timber. Clearing costs worker time, with no material charge.

Cancellation stops root removal or conflicting planting work, but does not undo cutting or an active timber delivery. Reassignment and save/load preserve orders and physical goods. Berry bushes, decorative border trees, buildings, and salvage piles are outside this tool; loggers already collect salvage automatically. Clearing is available on both original and larger maps.

### Sawmill and lodges

Build a **Sawmill** for six logs, then assign a **Sawyer** in People. Each mill supports one sawyer, who fetches two unreserved logs, saws them into four planks over ten work seconds, and hauls the planks back to the timber yard in loads of two. Builders and sawyers share log reservations, so they cannot claim the same timber.

Mills aim for a shared stock of eight planks, counting batches and shipments already on the way. They start another four-plank batch when that total falls to four or less. Reassign the sawyer when you want to stop production; carried materials return to storage and unfinished batches remain at the mill.

A **Lodge** costs eight planks and houses four villagers on the same footprint as a cottage. Builders reserve and deliver planks before construction starts. Lodges count toward the supper's housing objective. Cancelling an unfinished lodge leaves delivered planks as salvage for loggers to recover; this does not turn them back into logs.

Plank inventories, shipments, reservations, and sawmill batches survive save/load. Older saves load with no planks or sawmill production.

### Paths

Choose **Build → Paint paths**, or press **P**, then click or drag across clear land. **Shift+P** selects removal; **Esc** finishes. Paths are free and appear immediately, with visible connections between neighboring tiles.

Villagers choose routes by travel cost and walk 25% faster toward paved tiles. Editing a path updates active routes without canceling jobs or changing cargo. Paths can cover entrances and collection points; they cannot cover trees, bushes, buildings, or missing land. Building or planting on a path replaces the covered tiles. Paths are saved with each settlement; older saves start without paths.

## Controls and saves

The compact top bar shows stored resources, housing, day, hunger, pause, and speed. The bottom bar opens **Build**, **People**, **Goals**, and **Options**; click the active menu again or press **Esc** to close it. Selecting a villager or building opens a single contextual inspector with the relevant actions. **Move camera here** centers the selected entity.

The default view has no open side panels. At widths below 1100 pixels, opening a menu replaces the inspector and selecting an entity replaces the menu. Menus scroll when needed. The interface keeps its text size as the window resizes, with a minimum window size of 960×640; layouts are checked at 960×640, 1280×720, and 1440×900.

Villagers have stepping feet, distinct work motions and tools, and occasional idle gestures. Carried timber appears as logs; berries, grain, and bread use baskets with visible contents. These animations follow pause and game speed.

In **Options**, the **Effects** slider controls footsteps, work sounds, hauling, construction completion, and UI cues. **Nature** controls quiet wind and occasional birds. Press **M** or click **Mute sound** to mute both, retaining their volume settings. Work sounds stop while paused; nature ambience continues. Sounds use a limited number of voices and real-time repetition limits at faster game speeds.

Audio preferences persist in `saves/audio.cfg`, independently of settlement saves, resets, and loads. This first audio pass uses synthesized effects; music remains future work (F17).

| Control | Action |
| --- | --- |
| Left click | Place a plan or select a villager/building |
| B | Open/close Build |
| V | Open/close People and workforce assignments |
| G | Open/close Goals and the supper objective |
| O | Open/close Options: save, load, restart, audio, and controls |
| T | Toggle repeat tree planting on open ground or exhausted stumps |
| C | Toggle clearing orders; click trees/stumps to mark or cancel |
| P / Shift+P | Paint / remove paths by clicking or dragging |
| R | Rotate the unplaced building |
| Esc | Cancel preview first; otherwise close the menu or inspector |
| WASD | Pan |
| Q / E | Orbit in quarter turns |
| Mouse wheel | Zoom |
| Space / Pause | Pause or resume |
| M | Mute/unmute effects and nature ambience |
| Speed | Cycle 1×, 3×, 6× |
| F5 / Save | Save the current settlement |
| Home | Frame the full map |
| F9 / Load | Restore the saved settlement, paused |
| Start again | Restart standalone play; in a campaign, replay the current level with its previous village retained |

The original standalone manual save is `saves/settlement.json`; Three clearings uses `saves/three-clearings.json`. Previous saves are retained as `.bak`. Saves preserve terrain layout, simulation time, hunger, food inventories, crop growth, bakery batches, workers' positions/routes/tasks, reservations, construction, and supper progress. Loading validates the save before replacing the live game. Camera position and playback speed remain local view settings. Standalone play has no periodic autosave; map switches and campaign transitions/completion save as described above. Older standalone saves remain supported.

## Development

See the [feature roadmap](docs/ROADMAP.md) for future ideas and selectable work chunks. Pick a feature ID; flesh out its first playable version when we start it.

| File | Responsibility |
| --- | --- |
| `Simulation/Settlement.cs` | Fixed-step simulation, grid A*, placement, logging, construction, reservations |
| `Simulation/Food.cs` | Foraging, farming, baking, meals, hunger, supper |
| `Simulation/Woodland.cs` | Planting sites, sapling growth, renewable timber accounting |
| `Simulation/Clearing.cs`, `ClearingUi.cs` | Logger clearing orders, cancellation, root work, and clearing previews |
| `Simulation/Sawmill.cs` | Sawyers, log-to-plank production, stock target, plank accounting |
| `Simulation/Saving.cs` | Versioned JSON saves, validation, file replacement/backup |
| `Simulation/Campaign.cs`, `CampaignUi.cs` | Authored campaign setups, objective definitions, tutorial hints, progress and resumable villages |
| `Simulation/Maps.cs`, `MapVisuals.cs` | Saved map dimensions/land cells, larger authored map, terrain instancing, camera overview and map switching |
| `Game.cs` | Input, actor views, scene lifecycle, simulation/render coordination |
| `Visuals.cs`, `FoodVisuals.cs` | Procedural geometry, lighting, crops, pantry |
| `VillagerVisuals.cs` | Villager bodies, work tools, walking/idle poses, and cargo geometry |
| `SawmillVisuals.cs` | Sawmill, lodge, and plank geometry |
| `VillageAudio.cs`, `SoundSynthesis.cs`, `AudioUi.cs` | Procedural sounds, positional playback, ambience, volume controls, and preferences |
| `Hud.cs`, `HudLayout.cs`, `PersistenceUi.cs` | Compact HUD, menus, responsive layout, contextual inspector, save/load feedback |
| `MainMenu.cs`, `SmokeMainMenu.cs` | Title screen, mode selection, last-settlement resume, sound settings, and transition checks |
| `Smoke.cs`, `Smoke3.cs`, `SmokeWoodland.cs`, `SmokeSawmill.cs` | Rendered interaction checks |
| `SmokeAudio.cs` | Live mixer, mute, volume persistence, PCM, and audio lifecycle checks |
| `SmokeHud.cs` | Window resizing, menu/inspector flows, scrolling, and input isolation |
| `Tests/Checks.cs`, `Tests/FoodChecks.cs`, `Tests/WoodlandChecks.cs`, `Tests/SawmillChecks.cs` | Simulation and persistence tests |

Simulation advances in fixed 0.1-second steps on one thread. Job claims reserve resources and destination capacity together. Harvesting, construction, and food production have explicit ownership/worker limits. Reassignment releases claims and returns cargo physically. `World.Validate()` checks resource accounting, ownership, capacity, live targets, and routes. The C# simulation has no Godot dependencies.

`NuGet.Config` uses packages bundled with the portable Godot download. The launch/test scripts scope .NET and app-data settings to their process. To work in the Godot editor, use the .NET executable in `.tools/godot/` with the environment configured in `Play.ps1`, and open `project.godot`. Scene content is generated in C# at runtime.

## Verification

```powershell
# Simulation, resource reservations, and save/load tests
powershell -NoProfile -ExecutionPolicy Bypass -File Test.ps1

# Also compile and exercise the actual rendered game
powershell -NoProfile -ExecutionPolicy Bypass -File Test.ps1 -Rendered

# Only build and run the focused audio checks
powershell -NoProfile -ExecutionPolicy Bypass -File Play.ps1 -AudioSmokeTest

# Only build and run the responsive HUD checks
powershell -NoProfile -ExecutionPolicy Bypass -File Play.ps1 -HudSmokeTest
```

Tests cover legal placements, competing workers, scarce timber, priorities, reassignment, cancellation/salvage, seeded stress runs, food conservation, hunger recovery, supper completion, exact save/load continuation through every food-production phase, corrupted saves, and disk backups. The rendered check exercises the UI, all building types, active-batch save/load, the supper gathering, and restoration of a completed scenario. Its saves and screenshots go to `artifacts/`, separate from player saves.

Audio checks inspect live Godot mixer output, muted silence, volume persistence, pause suppression, voice/cadence limits, PCM bounds, and the wind loop seam. They export WAV previews to `artifacts/f10-audio/` and use an isolated preferences file in `artifacts/`.

Campaign checks complete all four levels and verify planting, physical gathering, exact saves, persistent delivery milestones, and replay. Run the focused rendered check with `powershell -ExecutionPolicy Bypass -File Play.ps1 -CampaignSmokeTest`; `Test.ps1 -Rendered` includes it. Screenshots go to `artifacts/f11-*.png`; campaign smoke saves use isolated temporary files.

Map checks build in three distant clearings, harvest the outer groves, preserve exact saves, and reject invalid terrain. Run `powershell -ExecutionPolicy Bypass -File Play.ps1 -MapSmokeTest` for overview/camera, distant placement, fast simulation, and map-switching checks at 1440×900 and 960×640. This is also included in `Test.ps1 -Rendered`.

Clearing checks cover five saved/interrupted work phases, timber conservation, cancellation/replanting, saplings, concurrent workers on the larger map, and legacy saves. Run `powershell -ExecutionPolicy Bypass -File Play.ps1 -ClearingSmokeTest` for tool controls, order markers, hauling, root-work animation, and construction on reclaimed land. `Test.ps1 -Rendered` includes it.

Milestones 1–3 are implemented: the first cottage, eight competing workers, and a complete food/supper scenario with persistence. Renewable woodland, villager animation/cargo, sawmills/lodges, and a first sound pass are also playable. See the roadmap for future features and presentation work.
