# Feature roadmap

A living menu of things we might build. Pick one chunk at a time; details, numbers, and order can change as we play. This is a feature plan, not a prototype cleanup backlog.

## Direction

A small, peaceful settlement that is satisfying to arrange and watch. Villagers carry real goods, buildings depend on one another, and short scenarios give the village a purpose. Keep Windows, local play, Godot, and C# as the starting point.

Current baseline: milestones 1–3 are playable, with eight workers, timber and construction, foraging/farming/baking, housing, a village supper, and save/load.

## Feature menu

Everything below is **Idea** unless marked otherwise. IDs stay stable so we can say “let's do F03.” Dependencies are provisional.

| ID | Feature | First playable chunk | Depends on |
| --- | --- | --- | --- |
| F01 | Paths and village layout — Done (first chunk) | Paint/remove connected dirt paths; villagers choose faster routes and gain a 25% walking bonus toward paved tiles. | — |
| F02 | Renewable woodland — Done (first chunk) | Plant trees that grow into harvestable timber, making continued building possible. | — |
| F03 | A village that feels alive — Done (first chunk) | Distinct work animations, recognizable carried goods, and a few idle actions. | — |
| F04 | Gathering places | Build a village square with benches/table; villagers visit during a short leisure period and gather there for supper. | — |
| F05 | New food choices | Add one alternative food chain, such as vegetables or an orchard, with its own building and visible harvest. Exact choice TBD. | — |
| F06 | More villagers | A small group arrives when spare housing is available; grow beyond the original eight. | — |
| F07 | Local storage and hauling | Place another stockpile and assign haulers so a distant work area can function efficiently. | — |
| F08 | More construction materials — Done (first chunk) | Add a sawmill and planks, then one building that uses them. | F02 suggested |
| F09 | Village character | Place gardens, fences, flowers, and decorative trees; give cottages a few visual variants. Coordinate outdoor decoration with F12c landscaping. | — |
| F10 | Sound effects — Done (first chunk) | Work, construction, hauling, UI, and ambient village/nature sounds. Start with a few recognizable actions and a volume control. | — |
| F11 | Campaign mode and objectives — First two levels done | Five authored settlements planned; the opening pair, saved progress, resume/replay, and level picker are playable. See the joint F11/F18 plan below. | — |
| F12 | Map expansion, landscape, and exploration — F12a and clearing done | Larger maps, irregular boundaries, and tree/stump clearing are playable. Water/crossings, decorative landscaping, and elevation remain planned; see chunks below. | — |
| F13 | Seasons | A visible seasonal cycle that changes one food source, giving stored food a purpose. | F05 suggested |
| F14 | Village happiness | A simple satisfaction measure driven by food variety and leisure, with visible villager reactions. Effects TBD. | F04, F05 |
| F15 | Small events and choices | Occasional visitors or requests with a modest reward or tradeoff. Start with one event. | F11 suggested |
| F16 | Free-build mode | An open-ended scenario with optional objectives and enough renewable resources to keep expanding. | F02, F06 suggested |
| F17 | Music | A gentle background soundtrack, with independent volume/mute controls. Tracks and transitions TBD. | — |
| F18 | Campaign tutorial — Opening pair done | Optional contextual guidance ships with levels 1–2. Continue teaching through later campaign settlements, introducing a building or building group per level. | F11 |
| F19 | Main menu | A title screen with Continue, New campaign, and settings for sound/music; add scenario/free-build selection as those modes arrive. | Campaign entry depends on F11 |
| F20 | Lighting and atmosphere | Warmer lighting, a cohesive palette, and subtle foliage movement. Day/night changes TBD. | — |
| F21 | UI and interaction — High priority; F21a/b done | Redesign the in-game HUD around a clear village view, contextual controls, and readable information. Continue in the chunks below. | — |
| F22 | Construction and growth presentation | Give each building recognizable construction stages, and make crop growth and harvesting more expressive. Start with one building or crop. | — |

## Original milestone 4 — Make it enjoyable to watch

This is a presentation milestone spanning several features; **F04 is the separate gathering-places feature**.

| Part of the original milestone | Track here | Current state / next opportunity |
| --- | --- | --- |
| Carry poses and tools | F03 | First pass done; smoother transitions and more reactions remain possible. |
| Construction stages | F22 | Basic stages exist; make them specific to each building. |
| Growing crops and trees | F22, F02 | Both grow visibly; crop variety and harvest feedback can develop further. |
| Sound effects and ambience | F10 | First synthesized pass done: positional work sounds, UI cues, wind/birds, volume and mute controls. |
| Music | F17 | Planned separately from sound effects. |
| Lighting | F20 | Planned; develop the village's visual mood. |
| Restrained, useful UI | F21 | A major priority with several selectable chunks, detailed below. |

### F21 — UI and interaction

UI improvement is a substantial part of the game roadmap. Make building and managing the settlement intuitive while leaving room to watch village life. A watch mode is only one part of this work.

These are provisional chunks, **Idea** unless marked otherwise. Pick one and refine its details when we start.

| Chunk | Player experience | Initial scope |
| --- | --- | --- |
| F21a — HUD and layout — Done | See the village clearly and find essential controls quickly. | Compact top/bottom bars; Build/People/Goals/Options menus; one contextual inspector; scrolling and responsive layouts down to 960×640. |
| F21b — Building and placement — Done | Browse buildings and place them with confidence. | Building descriptions, staffing/recipes, available materials, translucent building/tree previews, entrance arrows, rotation feedback, and specific rejection explanations. |
| F21c — Selection and management | Click something and immediately understand it. | Contextual villager/building details, clear selection feedback, convenient job assignments, and accessible construction priorities. |
| F21d — Economy and feedback | Understand shortages and know what needs attention. | Useful inventory/production information, actionable waiting reasons, and unobtrusive notifications. Exact metrics TBD. |
| F21e — Watching the village | Set management aside and enjoy the settlement. | Collapse panels or hide the HUD, keep pause/speed easy to reach, and restore the management view easily. Camera conveniences TBD. |

Style, layout, icons, and interaction details remain open. Start with a playable UI pass and adjust after using it; the main menu stays under F19 and tutorial guidance under F18.

## Where to start

**F12a — More room and varied map shapes** is playable through Options → Explore larger map. F12c's first clearing pass lets loggers reclaim trees and stumps for building. Try these before authoring later campaign layouts. F12b water/crossings or F12c's decorative landscaping follow-up can be selected independently next.

**F11a + F18a — Campaign foundation and the first two guided levels** is playable. Try the opening pair before tuning the rest; **F11b + F18b — Production lessons** (levels 3–4) is the next campaign slice after map expansion. The finale and its square remain planned.

F21a and F21b establish the UI direction and clearer building placement. **F21c — Selection and management** can build on the contextual inspector independently. UI remains a major priority; further refinements should follow play feedback.

**F20 — Lighting and atmosphere** is another strong presentation pick now that F10 has its first sound pass. F17 music and F22 construction/growth presentation can be chosen independently. F04 gathering places remains available as a gameplay feature; it is not required to complete the original presentation milestone.

## Selected chunks

### F01 — Paths and village layout

Status: Done (first chunk).

First version: Build menu / P paints free paths on clear land; Shift+P removes them. Click or drag, with intervening tiles filled and Esc to finish. Adjacent tiles join visually. Entrances and resource access points can be paved. Weighted routing compares travel time rather than always choosing the fewest cells, with a 25% speed bonus toward paved tiles. Live edits replan travel while preserving job claims and cargo. Building footprints and new planting replace covered paths. Paths persist with each map; old saves load with none.

Verification: Simulation checks cover blocked targets, path preference and faster travel, route edits without lost claims, exact save continuation, building/planting replacement, and old saves. Rendered HUD checks exercise dragging, joined geometry, erasing, saved-path rendering, and switching tools. Full gameplay regression checked. Starting speed and free immediate placement remain tuning choices.

Later / TBD: More path styles, worker-built roads and costs, stronger route/traffic feedback, and visual blending with future landscaping. Try paths in larger settlements before tuning the bonus.

### F12 — Map expansion and landscaping

Status: F12a done; F12c clearing done as a first chunk. Other landscape work remains planned; terrain tools and visual treatment remain provisional.

Want to play: Build a village that can spread into groves, clearings, and distinct neighborhoods, on land that feels like a place rather than a small square board. Arrange the surrounding landscape as well as the buildings. Larger maps should offer interesting choices and useful space, not just longer walks across empty grass.

| Chunk | Player experience | First scope |
| --- | --- | --- |
| F12a — More room and varied map shapes — Done | Pan across a larger settlement and choose between several building areas. | Saved map dimensions and land exclusions support rectangular/irregular layouts. Three clearings is a flat 32×32 authored map with 20 trees, six berry patches, and eight villagers. Options starts/resumes it separately; camera limits scale with terrain and Home frames the map. |
| F12b — Water and crossings | Build beside a pond or stream, then connect another useful area. | Authored water tiles, visible banks, and a simple bridge across a narrow crossing. Water blocks ordinary walking/building; bridges provide routes. Keep initial shores and crossing positions simple. Exact bridge cost, width, and construction rules TBD. |
| F12c — Player landscaping — Clearing done | Reclaim woodland for building, then shape the outdoor spaces. | First chunk: C / Build clearing tool, cancelable orders, logger priority, physical timber recovery, four-second root removal, and reusable land. Later: grass/earth painting, flowers, shrubs, and decorative trees coordinated with F09. |
| F12d — Hills and terrain shaping | Settle a valley or hillside and make room for a building. | Start with authored gentle elevation, readable slopes, and clear building rules. Later consider player raise/lower/level tools, terraces, ramps, and retaining walls. Decide height steps, accessibility, and construction costs when this chunk begins. |
| F12e — Exploring a larger settlement | Find workers and understand distant work areas without losing the village. | Better camera framing, useful location jumps, and potentially a small overview map. Fog of war or hidden discoveries are optional later ideas, not requirements for using a larger map. |

F12a should make map bounds, terrain occupancy, and authored resource placement part of the scenario/map definition, shared by rendering, placement, pathfinding, and saves. Avoid simply enlarging the visible ground while leaving simulation or camera bounds fixed. An irregular outline can still use the existing grid: cells outside the land are unavailable rather than forcing the player to see a rectangular board. Exact data representation and edge rendering are implementation choices.

The first larger map needs enough timber and food near its starting yard to get established, plus worthwhile space farther away. Size and population are separate choices: start with eight villagers, without requiring F06 arrivals. Retain the current maps for old saves and introductory lessons; save new map layouts with their settlements so loading cannot shift buildings or resources onto different terrain.

Coordinate with existing features:

- **F01 paths:** connect neighborhoods and make the longer trips satisfying. Paint/remove paths separately from cosmetic ground cover.
- **F07 storage and hauling:** add local stockpiles when distance becomes a useful logistical choice. Do not stretch early objectives into long waits before this exists.
- **F09 village character:** supplies decorations for landscaping; avoid two separate palettes for the same objects.
- **F02 woodland:** productive planting and regrowth remain simulation features. Clearing should respect workers, carried timber, and resource accounting; moving mature productive trees is a later decision.
- **F11/F18 campaign:** use authored geography to distinguish later settlements. Revisit levels 3–5 layouts after F12a; rivers, bridges, and hills can become later lessons without making every terrain feature a prerequisite for those levels.

Playable when, for F12a: start a larger authored settlement, pan and zoom across its full extent, build and harvest in separated clearings, and save/load without changing the map. Placement and routes respect irregular boundaries; all resource access remains usable; the village remains responsive at normal and fast speed. Review how the landscape looks at its edges as well as how much room it provides.

F12a verification: simulation checks complete construction in three distant clearings, harvest every outer grove, preserve exact map/worker saves, support a larger rectangle, and reject malformed terrain or blocked access. Legacy saves load with the original outline. Rendered checks cover instanced terrain, full-map framing at 1440×900 and 960×640, camera limits, distant previews/building, 6× simulation, separate save files, and switching back to the original village. Placement connectivity uses a single reachability pass. Overview and distant-preview screenshots reviewed; natural-looking edges, resource density, and travel pacing remain open to play feedback.

F12c first chunk: click trees, saplings, planting markers, or exhausted stumps to mark clearing orders; click again to cancel. Amber crosses persist until cancellation or completion. Loggers take clearing orders before planting and ordinary harvesting, while finishing committed jobs/deliveries. Existing timber remains physical cargo; roots take four work seconds to remove, with a digging motion and rustling sound. Land stays blocked until removal. Young trees yield no timber and stop growing while marked. Cancellation releases root/planting work but does not undo logging; salvage piles and berry bushes are excluded. No material cost. Clearing orders and active work persist in saves, including old-map settlements.

F12c verification: simulation checks cover timber conservation, clearing priority, all five saved/interrupted phases, cancellation and replanting, immature trees, several loggers on the larger map, and old saves. Rendered checks cover C/Esc and Build controls, repeated marking/canceling, invalid-target guidance, saved markers, visible root work, and building on reclaimed land at 960×640. Screenshots reviewed. Decorative landscaping, drag/area selection, and clearing-time tuning remain open.

Later / TBD: Exact map sizes, terrain art, procedural generation and seeds, map editor, water reshaping, terraforming costs, undo rules for landscaping, and map expansion during an existing game. Start with larger maps selected at scenario creation; an infinite world or dynamically purchased land is not required.

### F11 + F18 — First campaign and integrated tutorial

Status: F11a + F18a done; levels 1–2 playable. Levels 3–5 remain planned. Quantities, layouts, and pacing are provisional.

Want to play: Help five small settlements take shape, learning one new building or connected group at a time. Each has a modest local purpose and a warm closing moment. Aim for roughly 10–20 minutes per level, with a shorter opening; tune after playing rather than adding timers to enforce this.

#### Shared campaign rules

- Each level starts a fresh authored settlement with eight villagers, its own layout, starting buildings, supplies, and jobs. Campaign completion carries forward; villagers and inventories do not. Keep population growth outside this first campaign.
- **All implemented buildings and features remain available on every level.** An introduction is a recommendation and tutorial focus, not an unlock. Later, when the game is more polished, consider per-level building availability, progressive unlocks, and a replay option with everything available.
- Keep the mood patient: no deadlines, deaths, medals, or forced failure. Supply generous starting food and accessible mature timber. Earlier lessons provide functioning support buildings; later ones ask the player to manage more of the economy. Resource budgets must support the intended route without depending on three-day regrowth to rescue an early mistake.
- Show a short arrival note, a few measurable goals, and one optional tutorial hint at a time. Pause/speed, camera movement, and experimentation remain available. Finishing offers Continue playing, Next settlement, or Replay; never replace the village automatically.
- Use completed buildings and delivered goods for objectives, not placed plans or goods still being carried. Track cumulative deliveries for production lessons so daily meals and construction cannot erase progress. Final stock requirements are explicitly labeled as current stock.
- Recognize actions done ahead of a prompt, including valid alternative housing. Tutorial hints can be dismissed, disabled, or reopened; dismissing them does not complete campaign objectives. Camera and menu practice are suggestions, never mandatory victory checks.

#### The first five levels

**1. A place to stay — Cottages, logging, and construction**

A sheltered clearing beside a working berry camp. The eight arrivals need homes before this feels like a village.

- Start with the timber yard, a completed forager hut and two assigned foragers, a generous food reserve, and nearby trees. Assign a couple of loggers and builders; leave the remaining workers available. No housing yet. Food support runs quietly while the player learns construction.
- Introduce looking around, pause/speed, Build, footprint/entrance previews, rotation, and watching logs travel from tree to yard to building. Use a selected worker to explain the difference between a job and their current task.
- Required goal: provide completed housing for all eight villagers. Suggest four cottages, but lodges also qualify if the player wants to experiment. A first completed home is the intermediate milestone.
- End with a brief welcome-home message and time to watch the village. Optional experiment: change a construction priority and see which uncommitted delivery goes next. Do not require a particular layout or rotation.

**2. The berry clearing — Forager hut and everyday meals**

A settled hamlet has homes but needs a dependable food supply. Berry patches sit at different distances from the yard, making placement worth considering.

- Start with enough completed cottages for eight, the yard, mature timber, and a few days of berries; no food workplace. Keep existing logging/building jobs and guide the player to reassign available villagers to foraging.
- Introduce building and staffing a forager hut, its two-worker capacity, physical berry deliveries, daily meals, bush regrowth, and waiting reasons. Explain that a finished workplace still needs workers.
- Required goals: complete a forager hut and deliver 24 newly gathered berries to storage during this level. Starting berries do not count. Show progress as “Berries gathered and delivered: X / 24,” independent of food already eaten.
- A normal daily meal triggers a short explanation rather than a timing challenge. If food runs short, offer a recovery hint; hunger never forces a restart. Optional experiment: compare a one-forager and two-forager crew.

**3. Bread for the table — Farm and bakery**

A berry-fed village wants its first batch of fresh bread. Leave a broad open patch for fields and a compact workshop area nearby.

- Start with housing, a staffed forager hut, the yard, and adequate berries/timber. The player supplies the farm and bakery and reallocates workers without abandoning basic food gathering.
- Introduce sowing, visible crop growth, harvesting, grain storage, baking, and bread delivery. Explain that grain cannot be eaten and that the baker may legitimately wait for the first harvest.
- Required goals: complete a farm and bakery, and deliver 16 freshly baked loaves to storage over the level. Grain/bread in transit or workplace buffers do not count yet; eating delivered bread does not undo progress.
- End with a small narrative thank-you. Save the physical campaign celebration for level 5. The current standalone supper scenario remains playable and supplies the tested food-chain behavior for this level; no need to remove or rewrite it first.

**4. Room among the trees — Sawmill, lodge, and renewable woodland**

A woodland hamlet wants a larger home and a grove that will outlast the first building rush.

- Start with functioning food support, six cottage beds, a yard, and enough mature timber for the mill and lodge chain with a comfortable margin. The temporary housing shortfall is the motivation, not a penalty. Provide clear planting ground and explain that exhausted stumps can also be reused.
- Introduce the sawyer, logs becoming planks, the eight-plank stock target, shared log demand between builders and the mill, and the lodge's four beds. Introduce logger planting alongside the mill so saplings grow while construction proceeds.
- Required goals: complete a sawmill, complete a lodge, house all eight, and have loggers actually plant four new trees. Merely marking planting spots does not count. Existing mature trees do not count either.
- Show the three-day growth cycle, but do not require waiting for maturity to win. Optional follow-up while continuing to play: harvest a player-planted tree and replant its stump. No new forester building or automatic forestry system is needed.

**5. A place for everyone — Village square and a shared supper**

A small established settlement has the essentials but no shared center. Choose where the square belongs, expand the food chain, and bring everyone together.

- Start with housing, a staffed forager hut, the yard, and generous timber/food. Leave farm, bakery, and square placement to the player; all earlier buildings remain available. An open central clearing invites a gathering place without prescribing its position.
- Introduce one new building: the **Village square (F04)**, a modest table-and-benches space with no permanent staff. For this campaign slice it supplies a gathering destination; broader daily leisure and happiness effects can wait. Cost and footprint TBD; prefer logs so the finale does not require rebuilding every prior production chain.
- Required goals: complete the square, house all eight, hold 16 loaves in shared storage, then host and finish the supper. This stock goal is deliberately different from level 3's cumulative production lesson. Explain that hosting consumes the 16 loaves once.
- Reuse the existing physical gathering and celebration, directing villagers to the square instead of the yard. Hosting needs reachable gathering positions for all eight. End with everyone at the table, a campaign-complete note, and continued free play in the settlement.

#### Buildable chunks and supporting work

| Chunk | Scope | Playable when |
| --- | --- | --- |
| F11a + F18a — Opening pair — Done | Authored level setups, reusable objectives, saved campaign progress, replay/next/continue controls, contextual hints, and levels 1–2. Entry/level picker lives in Goals; the polished title screen stays in F19. | Complete housing, move to a fresh berry settlement, save/load midway through its delivery goal, and finish it with guidance on or off. |
| F11b + F18b — Production lessons | Levels 3–4, cumulative production/planting milestones, and contextual food, plank, and regrowth hints. | Both levels work when players follow the suggested order or build ahead; consuming goods does not lose earned progress. |
| F04a + F11c + F18c — Campaign finale | Build/place the square, validate gathering space, adapt supper to its destination, author level 5, and show campaign completion. | All eight reach the square, supper is consumed once, the celebration completes, and play can continue. |

F11 owns scenario setup, objectives, transitions, and saved campaign progress. F18 owns the teaching sequence and contextual hints; ship each level's guidance with its gameplay. Keep level definitions and objective types reusable instead of adding a separate hard-coded victory flow for each map. Exact data format is an implementation decision.

Implemented persistence: campaign.json holds the active level, full settlement snapshots, tutorial state, and completion record. Berry delivery progress derives from conserved inventory plus consumption minus starting supply. Transitions preserve a snapshot per settlement; replay additionally retains the previous village, with a restore/swap control. F5 saves, F9 loads, completion and transitions save automatically, and launch resumes an active saved campaign paused. Existing standalone saves remain standalone; broader save-slot UI is TBD.

Opening-pair verification: simulation playthroughs complete both levels with action-aware or disabled guidance; cottages and lodges both satisfy level 1. Checks cover interrupted berry deliveries, meals, exact save continuation, replay records, legacy saves, and disk backups. Rendered checks exercise campaign entry, both levels, hints, next/replay/restore, startup resume, invalid-save recovery, standalone return, and a 960×640 layout. Screenshots reviewed. Initial supplies are 64 berries and 48 harvestable logs per level, plus prebuilt infrastructure; pacing and player feedback remain TBD.

Relevant checks when implementing: save/load mid-objective and mid-celebration; goals already satisfied before hints appear; alternative housing; consumed goods versus cumulative deliveries; no double-counting carried or redelivered goods; replay without losing progress; all tools available in every level; and a complete playthrough of each authored starting setup. Tune food buffers, tree placement, walking distances, and objective quantities from those playthroughs.

Later / TBD: Per-level building restrictions/unlocks; richer stories and level art; optional challenge goals; more campaign chapters; branching progression; F19 title-screen presentation. Paths (F01), extra food chains (F05), new arrivals (F06), and happiness (F14) are good candidates for later lessons, not dependencies of these first five levels.

### F02 — Renewable woodland / planting and regrowth

Status: Done

Want to play: Keep expanding after the original woodland runs out, and shape a new grove around the village.

First version: Mark open ground or exhausted stumps for free planting. Loggers plant before taking new harvest jobs (after explicit clearing orders added in F12c); saplings grow over three game days and yield eight logs. Show planting markers, growing trees, and job/growth counts. Save the entire cycle.

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

### F10 — Village sounds and audio controls

Status: Done

Want to play: Hear the village working, with a quiet outdoor background and simple sound controls.

First version: Original synthesized footsteps, chopping, hammering, crop/plant rustles, sawing, oven crackle, cargo drops, placement/UI feedback, and completion cues. Positional work sounds with bounded voices and repetition; wind and occasional birds. Separate Effects/Nature volumes and M to mute, with preferences saved independently of the village. Work stops sounding when paused; nature continues.

Later / TBD: More organic recordings or richer synthesis, extra variations, mixing by zoom level, tighter motion/impact synchronization, and ambient details. Music stays in F17.

Verification: Build and rendered gameplay checks pass. Focused audio checks verify live mixer output, muted silence, settings persistence, pause suppression, bounded playback, simulation independence, PCM levels, and wind loop continuity. Subjective sound/mix feedback TBD.

### F21a — Compact HUD and contextual panels

Status: Done

Want to play: Keep the village visible while making management controls easy to find.

First version: Compact resource/housing/day/pause/speed bar; bottom navigation for Build, People, Goals, and Options; one inspector for the selected villager or building. Building costs on cards, scrollable menus, contextual construction/job controls, and a camera-focus action. Menu shortcuts B/V/G/O and layered Esc handling. Narrow windows show either a menu or inspector, keeping text readable instead of scaling the whole HUD down.

Follow-up: Building previews and placement explanations delivered in F21b. Deeper management controls (F21c), economy feedback (F21d), and full HUD hiding (F21e) remain open.

Verification: Rendered gameplay, woodland, sawmill/lodge, save/load, and audio checks pass through the new menus. Focused HUD checks cover 1440×900, 1280×720, and 960×640, menu/inspector switching, scrolling, keyboard controls, and preventing UI clicks from placing buildings. Player feedback TBD.

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
- F10, first chunk: synthesized village sounds, wind/birds, positional playback, and persistent audio controls.
- F21a: compact HUD, responsive menus, and contextual inspector.
- F21b: building descriptions and supply guidance, recognizable placement previews, entrance/rotation feedback, and specific placement rejection reasons. Verified with simulation checks and rendered HUD checks, including repeat tree planting and preview state isolation. Visual style and description wording remain open to play feedback.
- F11a + F18a: the first two campaign settlements, integrated optional guidance, reusable goals, campaign saves/resume/replay, and preserved standalone play.
- F12a: configurable saved map layouts, Three clearings with an irregular 32×32 outline, distant resource access, scalable camera bounds, Home overview, and separate map saves.
- F12c, first chunk: cancelable tree/stump clearing orders, logger timber recovery and root removal, visible markers/work, and reusable building ground.
