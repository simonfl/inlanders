# Feature roadmap

A living menu of things we might build. Pick one chunk at a time; details, numbers, and order can change as we play. This is a feature plan, not a prototype cleanup backlog.

## Direction

A small, peaceful settlement that is satisfying to arrange and watch. Villagers carry real goods, buildings depend on one another, and short scenarios give the village a purpose. Keep Windows, local play, Godot, and C# as the starting point.

Current baseline: milestones 1–3 are playable, with eight workers, timber and construction, foraging/farming/baking, housing, a village supper, and save/load.

## Feature menu

Everything below is **Idea** unless marked otherwise. IDs stay stable so we can say “let's do F03.” Dependencies are provisional.

| ID | Feature | First playable chunk | Depends on |
| --- | --- | --- | --- |
| F01 | Paths and village layout | Paint/remove simple paths; villagers prefer them and walk faster on them. | — |
| F02 | Renewable woodland — Done (first chunk) | Plant trees that grow into harvestable timber, making continued building possible. | — |
| F03 | A village that feels alive — Done (first chunk) | Distinct work animations, recognizable carried goods, and a few idle actions. | — |
| F04 | Gathering places | Build a village square with benches/table; villagers visit during a short leisure period and gather there for supper. | — |
| F05 | New food choices | Add one alternative food chain, such as vegetables or an orchard, with its own building and visible harvest. Exact choice TBD. | — |
| F06 | More villagers | A small group arrives when spare housing is available; grow beyond the original eight. | — |
| F07 | Local storage and hauling | Place another stockpile and assign haulers so a distant work area can function efficiently. | — |
| F08 | More construction materials — Done (first chunk) | Add a sawmill and planks, then one building that uses them. | F02 suggested |
| F09 | Village character | Place gardens, fences, flowers, and decorative trees; give cottages a few visual variants. | — |
| F10 | Sound effects | Work, construction, hauling, UI, and ambient village/nature sounds. Start with a few recognizable actions and a volume control. | — |
| F11 | Campaign mode and objectives | A sequence of authored settlements with distinct goals and saved progress. Start with two linked scenarios, including the current supper. | — |
| F12 | Landscape and exploration | A larger authored map with water, a bridge, and another useful area to settle. Terrain height TBD. | F11 suggested |
| F13 | Seasons | A visible seasonal cycle that changes one food source, giving stored food a purpose. | F05 suggested |
| F14 | Village happiness | A simple satisfaction measure driven by food variety and leisure, with visible villager reactions. Effects TBD. | F04, F05 |
| F15 | Small events and choices | Occasional visitors or requests with a modest reward or tradeoff. Start with one event. | F11 suggested |
| F16 | Free-build mode | An open-ended scenario with optional objectives and enough renewable resources to keep expanding. | F02, F06 suggested |
| F17 | Music | A gentle background soundtrack, with independent volume/mute controls. Tracks and transitions TBD. | — |
| F18 | Campaign tutorial | Make the opening campaign settlement teach camera controls, building, jobs, and food through small objectives that advance as the player acts. | F11 |
| F19 | Main menu | A title screen with Continue, New campaign, and settings for sound/music; add scenario/free-build selection as those modes arrive. | Campaign entry depends on F11 |
| F20 | Lighting and atmosphere | Warmer lighting, a cohesive palette, and subtle foliage movement. Day/night changes TBD. | — |
| F21 | UI and interaction — High priority | Redesign the in-game HUD around a clear village view, contextual controls, and readable information. Start with F21a below. | — |
| F22 | Construction and growth presentation | Give each building recognizable construction stages, and make crop growth and harvesting more expressive. Start with one building or crop. | — |

## Original milestone 4 — Make it enjoyable to watch

This is a presentation milestone spanning several features; **F04 is the separate gathering-places feature**.

| Part of the original milestone | Track here | Current state / next opportunity |
| --- | --- | --- |
| Carry poses and tools | F03 | First pass done; smoother transitions and more reactions remain possible. |
| Construction stages | F22 | Basic stages exist; make them specific to each building. |
| Growing crops and trees | F22, F02 | Both grow visibly; crop variety and harvest feedback can develop further. |
| Sound effects and ambience | F10 | Planned; add recognizable work sounds and quiet outdoor ambience. |
| Music | F17 | Planned separately from sound effects. |
| Lighting | F20 | Planned; develop the village's visual mood. |
| Restrained, useful UI | F21 | A major priority with several selectable chunks, detailed below. |

### F21 — UI and interaction

UI improvement is a substantial part of the game roadmap. Make building and managing the settlement intuitive while leaving room to watch village life. A watch mode is only one part of this work.

These are provisional chunks, all **Idea**. Pick one and refine its details when we start.

| Chunk | Player experience | Initial scope |
| --- | --- | --- |
| F21a — HUD and layout | See the village clearly and find essential controls quickly. | Compact resource/objective/time controls, a consistent visual hierarchy, and panels that open when needed. Support different window sizes and readable text. |
| F21b — Building and placement | Browse buildings and place them with confidence. | Clear building choices, costs and descriptions, recognizable previews, rotation feedback, and specific explanations for rejected placements. |
| F21c — Selection and management | Click something and immediately understand it. | Contextual villager/building details, clear selection feedback, convenient job assignments, and accessible construction priorities. |
| F21d — Economy and feedback | Understand shortages and know what needs attention. | Useful inventory/production information, actionable waiting reasons, and unobtrusive notifications. Exact metrics TBD. |
| F21e — Watching the village | Set management aside and enjoy the settlement. | Collapse panels or hide the HUD, keep pause/speed easy to reach, and restore the management view easily. Camera conveniences TBD. |

Style, layout, icons, and interaction details remain open. Start with a playable UI pass and adjust after using it; the main menu stays under F19 and tutorial guidance under F18.

## Where to start

Suggested next pick: **F21a — HUD and layout**, establishing the UI direction and giving the village more screen space. UI is a major priority, not just a final polish pass.

**F10 — Sound effects** is the next strong presentation pick, followed by **F20 — Lighting and atmosphere**. F17 music and F22 construction/growth presentation can be chosen independently. F04 gathering places remains available as a gameplay feature; it is not required to complete the original presentation milestone.

## Selected chunks

### F02 — Renewable woodland / planting and regrowth

Status: Done

Want to play: Keep expanding after the original woodland runs out, and shape a new grove around the village.

First version: Mark open ground or exhausted stumps for free planting. Loggers plant before taking new harvest jobs; saplings grow over three game days and yield eight logs. Show planting markers, growing trees, and job/growth counts. Save the entire cycle.

Later / TBD: Automatic replanting zones, cancelling planting spots, a dedicated forester, tree species, and different growth/yield tradeoffs.

Playable when: Mark a tree, watch a logger plant it, save/load during growth, harvest and haul its timber into construction, then replant its stump.

Verification: Simulation checks cover repeated growth/harvest cycles, using renewed logs in construction, job interruption, dense placement, and save compatibility. Rendered checks cover controls, sapling growth, save/load, harvesting, and stump replanting. Three days and eight logs remain starting values; player feedback TBD.

### F03 — Villager motions and cargo

Status: Done

Want to play: Understand what villagers are doing by watching them, and enjoy a little more life in the settlement.

First version: Stepping legs, carrying poses, distinct logging/building/planting/foraging/baking motions and tools, berry baskets/grain sheaves/loaves, and idle head turns and hat adjustments. Workers face their work, and animation follows pause and speed.

Later / TBD: Tree-falling animation, particles, smoother transitions between poses, social interactions, and more character variation.

Playable when: Watch workers chop, build, farm, forage, bake, and haul recognizable goods; pause freezes their poses and loading restores the appropriate tools/cargo.

Verification: Build and rendered scenario/woodland checks pass, including restored baker tools and frozen poses while paused. Reviewed normal village and closer baking views. Player feedback TBD; this is a first procedural animation pass.

### F08 — Sawmill, planks, and lodge

Status: Done

Want to play: Turn renewable timber into a new building material and use it for better housing.

First version: A six-log sawmill with one sawyer slot; two logs become four planks in ten work seconds. Haul both input and output physically, with a shared stock target of eight planks. Build an eight-plank lodge with four beds. Preserve reservations, batches, and plank salvage through interruption and save/load.

Later / TBD: Mixed-material recipes, more plank buildings, adjustable stock targets, upgrades, and other materials.

Playable when: Build and staff the mill, watch logs become planks, finish a lodge, and restore an active batch from a save.

Verification: Simulation checks cover the production chain, stock target, reservations, interruption, cancellation with plank salvage, and save compatibility. Rendered checks cover placement/staffing, the saw tool and plank cargo, saved batches, and lodge completion. Player feedback TBD; stock target, recipe, and housing costs are initial values.

## How we take a chunk

1. Pick an ID and mark it **Next**, then **In progress** when work starts.
2. Add a short entry below: what we want to play, the smallest useful version, and any decisions needed now. Leave the rest TBD.
3. Implement enough to play it, including save/load for new persistent state and relevant checks.
4. Play it, record what we learned, and mark it **Done** or split out a follow-up.

Keep refactors and supporting UI inside the feature that needs them. A broad feature can have several small chunks; finishing its first chunk does not commit us to every possible extension.

### Chunk template

Copy this when we choose a feature; no need to fill it out for every idea upfront.

```markdown
### Fxx — Feature name / chunk name

Status: Next | In progress | Done | Parked

Want to play: One sentence describing the new player experience.
First version: A few concrete things we will add.
Later / TBD: Anything deliberately left open.
Playable when: A short in-game check that demonstrates the feature.
After playing: Notes, follow-ups, and implementation commit if useful.
```

## Parking lot

Possibilities without a commitment: fishing, trade, building upgrades, professions tied to individual workplaces, household routines, weather, procedural maps, and a photo mode.

Combat, multiplayer, a large technology tree, and a full life simulation are outside the current direction. Revisit only if they sound fun later.

## Completed chunks

- Milestone 1: harvesting, hauling, and the first cottage.
- Milestone 2: eight villagers, work assignments, reservations, and construction priorities.
- Milestone 3: food production, meals, the village supper, and save/load.
- F02, first chunk: logger planting, visible tree growth, renewable timber, and stump replanting.
- F03, first chunk: stepping feet, work tools and motions, recognizable cargo, and idle gestures.
- F08, first chunk: sawmill, sawyer role, planks, and four-bed lodge.
