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
| F04 | Gathering places — Supper square done | Build a village square with benches/table; villagers visit during a short leisure period and gather there for supper. | — |
| F05 | New food choices | Add one alternative food chain, such as vegetables or an orchard, with its own building and visible harvest. Exact choice TBD. | — |
| F06 | More villagers | A small group arrives when spare housing is available; grow beyond the original eight. | — |
| F07 | Local storage and hauling | Place another stockpile and assign haulers so a distant work area can function efficiently. | — |
| F08 | More construction materials — Done (first chunk) | Add a sawmill and planks, then one building that uses them. | F02 suggested |
| F09 | Village character | Place gardens, fences, flowers, and decorative trees; give cottages a few visual variants. Coordinate outdoor decoration with F12c landscaping. | — |
| F10 | Sound effects — Done (first chunk) | Work, construction, hauling, UI, and ambient village/nature sounds. Start with a few recognizable actions and a volume control. | — |
| F11 | Campaign mode and objectives — First campaign done | Four authored settlements, saved progress, resume/replay, and level picker are playable. See the joint F11/F18 plan below. | — |
| F12 | Map expansion, landscape, and exploration — F12a/b and clearing done | Larger maps, irregular boundaries, and tree/stump clearing are playable. Water/crossings are playable; decorative landscaping and elevation remain planned; see chunks below. | — |
| F13 | Seasons | A visible seasonal cycle that changes one food source, giving stored food a purpose. | F05 suggested |
| F14 | Village happiness | A simple satisfaction measure driven by food variety and leisure, with visible villager reactions. Effects TBD. | F04, F05 |
| F15 | Small events and choices | Occasional visitors or requests with a modest reward or tradeoff. Start with one event. | F11 suggested |
| F16 | Free-build mode | An open-ended scenario with optional objectives and enough renewable resources to keep expanding. | F02, F06 suggested |
| F17 | Music | A gentle background soundtrack, with independent volume/mute controls. Tracks and transitions TBD. | — |
| F18 | Campaign tutorial — First campaign done | Optional contextual guidance ships with all four levels, introducing a building or connected group per settlement. | F11 |
| F19 | Main menu — Done (first chunk) | Title screen with Continue, Campaign start/resume/replay, both Free play maps, sound settings, Quit, and save-on-return from gameplay. | F11 |
| F20 | Lighting and atmosphere — Done (first chunk) | Soft daylight/golden-hour light, muted grass variation, and subtle pause-aware foliage motion with saved visual settings. Day/night changes TBD. | — |
| F21 | UI and interaction — High priority; F21a–e first chunks done | Redesign the in-game HUD around a clear village view, contextual controls, and readable information. Continue in the chunks below. | — |
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
| Lighting | F20 | Soft daylight and golden hour are playable, with subtle foliage motion. |
| Restrained, useful UI | F21 | A major priority with several selectable chunks, detailed below. |

### F21 — UI and interaction

UI improvement is a substantial part of the game roadmap. Make building and managing the settlement intuitive while leaving room to watch village life. A watch mode is only one part of this work.

These are provisional chunks, **Idea** unless marked otherwise. Pick one and refine its details when we start.

| Chunk | Player experience | Initial scope |
| --- | --- | --- |
| F21a — HUD and layout — Done | See the village clearly and find essential controls quickly. | Compact top/bottom bars; Build/People/Goals/Options menus; one contextual inspector; scrolling and responsive layouts down to 960×640. |
| F21b — Building and placement — Done | Browse buildings and place them with confidence. | Building descriptions, staffing/recipes, available materials, translucent building/tree previews, entrance arrows, rotation feedback, and specific rejection explanations. |
| F21c — Selection and management — Done (first chunk) | Click something and immediately understand it. | Direct job picker, scrolling inspector, moving villager marker, camera follow, workplace staffing and worker links, plus construction priority/cancel controls. |
| F21d — Economy and feedback — Done (first chunk) | Understand shortages and know what needs attention. | Economy panel [I], available/reserved/carried/workplace inventory, food coverage, actionable shortage links, idle-worker inspection, and a quiet issue-count badge. |
| F21e — Watching the village — Done (first chunk) | Set management aside and enjoy the settlement. | H / Options enters watch mode: hidden HUD, compact pause/speed/frame/return bar, preserved selection and camera follow, safe placement cancellation, and easy management return. |

Style, layout, icons, and interaction details remain open. Start with a playable UI pass and adjust after using it; the main menu stays under F19 and tutorial guidance under F18.

## Where to start

**F12a — More room and varied map shapes** is playable through Options → Explore larger map. F12c's first clearing pass lets loggers reclaim trees and stumps for building. Try these before authoring later campaign layouts. F12b now adds water and bridges; F12c's decorative landscaping follow-up or F12e navigation can be selected next.

**F11/F18 — First campaign** is complete: combined opening lesson, bread production, woodland, and village-square finale. Playtest pacing before expanding the campaign.

F21a and F21b establish the UI direction and clearer building placement. **F21c — Selection and management** now adds direct jobs, workplace links/staffing, and camera follow. F21d economy feedback is playable too; F21e watch mode completes the first UI pass. Further UI work should follow play feedback. UI remains a major priority; further refinements should follow play feedback.

**F20 — Lighting and atmosphere** now has a first pass: two light moods and gentle foliage movement. F17 music and F22 construction/growth presentation can be chosen independently. F04 gathering places remains available as a gameplay feature; it is not required to complete the original presentation milestone.

## Selected chunks

### F19 — Main menu and settlement entry

Status: Done (first chunk).

First version: Launch into a title screen over a paused village, with Continue, Campaign, Free play, Settings, and Quit. Continue restores the last opened/saved settlement from its own snapshot; pre-menu installations fall back to the newest existing save. Campaign exposes the opening levels with completion records and replay, retaining previous villages. Free play starts/resumes either map and keeps a recoverable previous-village copy when starting anew. Settings share Effects/Nature/mute with gameplay. Options → Return to main menu saves first and keeps the village open if saving fails. Entering gameplay is paused. Menu input cannot edit the world or advance simulation.

Verification: Rendered checks cover 1440×900 and 960×640 layouts, fresh/missing Continue, all settlement types, menu input isolation, sound settings, replay and previous-village recovery, corrupted Continue recovery through separate saves, older-save fallback, and failure to save on return. Existing gameplay/HUD checks pass. Menu screenshot reviewed.

Later / TBD: Music controls with F17, richer title artwork or ambient scene motion, expanded save-slot browsing, and keyboard/controller navigation polish. All four campaign levels are playable.

### F01 — Paths and village layout

Status: Done (first chunk).

First version: Build menu / P paints free paths on clear land; Shift+P removes them. Click or drag, with intervening tiles filled and Esc to finish. Adjacent tiles join visually. Entrances and resource access points can be paved. Weighted routing compares travel time rather than always choosing the fewest cells, with a 25% speed bonus toward paved tiles. Live edits replan travel while preserving job claims and cargo. Building footprints and new planting replace covered paths. Paths persist with each map; old saves load with none.

Verification: Simulation checks cover blocked targets, path preference and faster travel, route edits without lost claims, exact save continuation, building/planting replacement, and old saves. Rendered HUD checks exercise dragging, joined geometry, erasing, saved-path rendering, and switching tools. Full gameplay regression checked. Starting speed and free immediate placement remain tuning choices.

Later / TBD: More path styles, worker-built roads and costs, stronger route/traffic feedback, and visual blending with future landscaping. Try paths in larger settlements before tuning the bonus.

### F12 — Map expansion and landscaping

Status: F12a/b done; F12c clearing done as a first chunk. Other landscape work remains planned; terrain tools and visual treatment remain provisional.

Want to play: Build a village that can spread into groves, clearings, and distinct neighborhoods, on land that feels like a place rather than a small square board. Arrange the surrounding landscape as well as the buildings. Larger maps should offer interesting choices and useful space, not just longer walks across empty grass.

| Chunk | Player experience | First scope |
| --- | --- | --- |
| F12a — More room and varied map shapes — Done | Pan across a larger settlement and choose between several building areas. | Saved map dimensions and land exclusions support rectangular/irregular layouts. Three clearings is a flat 32×32 authored map with 20 trees, six berry patches, and eight villagers. Options starts/resumes it separately; camera limits scale with terrain and Home frames the map. |
| F12b — Water and crossings — Done (first chunk) | Build beside a pond or stream, then connect another useful area. | Saved water tiles, visible banks, and a stream on new Three clearings maps. Six-log bridges cross one water tile between clear dry banks; builders work from an accessible bank. Completed decks open routes; unfinished bridges can be cancelled with physical salvage on dry land. |
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
- **F11/F18 campaign:** use authored geography to distinguish later settlements. Revisit later campaign layouts after F12a; rivers, bridges, and hills can become later lessons without making every terrain feature a prerequisite for those levels.

Playable when, for F12a: start a larger authored settlement, pan and zoom across its full extent, build and harvest in separated clearings, and save/load without changing the map. Placement and routes respect irregular boundaries; all resource access remains usable; the village remains responsive at normal and fast speed. Review how the landscape looks at its edges as well as how much room it provides.

F12a verification: simulation checks complete construction in three distant clearings, harvest every outer grove, preserve exact map/worker saves, support a larger rectangle, and reject malformed terrain or blocked access. Legacy saves load with the original outline. Rendered checks cover instanced terrain, full-map framing at 1440×900 and 960×640, camera limits, distant previews/building, 6× simulation, separate save files, and switching back to the original village. Placement connectivity uses a single reachability pass. Overview and distant-preview screenshots reviewed; natural-looking edges, resource density, and travel pacing remain open to play feedback.

F12b first chunk: new Three clearings maps have a narrow north–south stream separating the eastern grove from the central village. Walking around its ends remains possible; bridges create direct routes. Original and campaign layouts retain their authored dry terrain. Build → Bridge, place on water, and R rotates the span. The marked entrance automatically uses a reachable bank. Six logs and ordinary builder work complete the deck; villagers cannot cross an unfinished bridge. Both banks remain protected from building and planting. Paths stay on land; bridges need no paving.

Water, bridge progress, bank choice, and routes persist in settlement saves. Resources on a disconnected bank wait without claiming workers; after bridging, normal work resumes. Unfinished cancellation releases workers and leaves delivered logs as salvage on nearby legal dry land. Completed bridges cannot be demolished in this first version, matching other completed buildings.

F12b verification: simulation tests cover dry-land tool rejection, orientation, inaccessible resources, construction from the near bank, exact in-progress saves, far-bank construction and hauling, cancellation/salvage, and malformed water. Rendered map checks cover water/banks, invalid and rotated valid bridge previews, building stages, finished alignment, and saved map switching.

Later water work: wider spans, longer rivers and islands, richer shore shapes and animation, demolition rules, bridge materials/variants, and campaign geography. These remain separate from the first crossing.

F12c first chunk: click trees, saplings, planting markers, or exhausted stumps to mark clearing orders; click again to cancel. Amber crosses persist until cancellation or completion. Loggers take clearing orders before planting and ordinary harvesting, while finishing committed jobs/deliveries. Existing timber remains physical cargo; roots take four work seconds to remove, with a digging motion and rustling sound. Land stays blocked until removal. Young trees yield no timber and stop growing while marked. Cancellation releases root/planting work but does not undo logging; salvage piles and berry bushes are excluded. No material cost. Clearing orders and active work persist in saves, including old-map settlements.

F12c verification: simulation checks cover timber conservation, clearing priority, all five saved/interrupted phases, cancellation and replanting, immature trees, several loggers on the larger map, and old saves. Rendered checks cover C/Esc and Build controls, repeated marking/canceling, invalid-target guidance, saved markers, visible root work, and building on reclaimed land at 960×640. Screenshots reviewed. Decorative landscaping, drag/area selection, and clearing-time tuning remain open.

Later / TBD: Exact map sizes, terrain art, procedural generation and seeds, map editor, water reshaping, terraforming costs, undo rules for landscaping, and map expansion during an existing game. Start with larger maps selected at scenario creation; an infinite world or dynamically purchased land is not required.

### F11 + F18 — First campaign and integrated tutorial

Status: **F11/F18 first campaign complete.** Four guided settlements; quantities and pacing remain open to playtesting.

The opening housing and berry lessons are combined. Every building and tool remains available in every level. Later, once polished, consider per-level availability and progressive unlocks, with an unrestricted replay option.

| Level | Introduces | Starting village | Required goals |
| --- | --- | --- | --- |
| 1. A place to stay | Forager hut, cottages, logging/building, everyday meals | Empty original clearing, 96 berries, two loggers/builders/foragers, two spare workers | Finish a hut, deliver 24 fresh berries, house eight |
| 2. Bread for the table | Farm and bakery | Four cottages and a staffed hut, 96 berries | Finish a farm and bakery; deliver 16 loaves cumulatively |
| 3. Room among the trees | Sawmill, lodge, renewable woodland | Larger irregular map, two cottages and staffed hut, 96 berries | Finish a mill and lodge, house eight, have loggers actually plant four trees |
| 4. A place for everyone | Village square and shared supper | Larger irregular map, two cottages and staffed hut, 96 berries | Finish a square, house eight, stock 16 bread and host a completed supper |

Each settlement starts fresh, paused, with eight villagers. No deadlines, deaths, medals, forced failure, or automatic transition. Goals recognize building ahead and alternative housing. Deliveries count only once goods reach storage; meals do not erase earned delivery progress. Planting marks do not count until a logger finishes planting; maturity is not required.

F18 guidance covers camera/pause/building, staffing and waiting, food production, planks, planting, and gathering. One contextual hint appears at a time; dismiss, disable, or reopen guidance without affecting objectives. Earlier food lessons provide working support buildings in later levels.

**F04a — Village square: implemented for the finale.** Costs six logs, standard 3×2 footprint, no staff. A table and benches provide the supper destination. Hosting requires eight reachable gathering cells within four tiles of its entrance. All eight walk there, 16 loaves are consumed once, and the celebration finishes after everyone arrives. Daily leisure and happiness effects remain future F04 work.

Campaign completion offers continued play, next settlement (except the finale), or replay. Main menu and Goals list all four levels. Campaign saves include snapshots, completed levels, dismissed guidance, delivery/planting progress, and in-flight supper state. Replay keeps the preceding village with a restore/swap control. Save compatibility is not a project requirement at this stage; start fresh when formats change.

Verification covers complete simulated playthroughs of all four authored setups, actual planting, delivery progress after meals, exact save continuation (including gathering), one-time supper cost, and replay records. Rendered checks cover the four-level flow, tutorial controls, save/load, finale hosting, next/replay/restore, and 960×640 layout.

Later / TBD: pacing and distinct scenery for later maps, more narrative closing moments, optional objectives, more levels, progressive availability, daily square activity, and richer tutorial presentation.

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

### F21c — Selection and management

Status: Done (first chunk).

Select a villager to choose any role directly, including Unassigned, then apply it. The moving ground marker makes the selection visible. Follow keeps the camera on that villager; manual WASD pan, Home, closing selection, or switching to a building stops following. Inspect workplace jumps to the current production or construction site.

Production inspectors show active workers and village-wide job counts, offer +/− staffing, and link to workers currently using that site. Jobs remain shared across workplaces: these controls adjust the village workforce, not permanent per-building assignments. Construction inspectors retain delivered/incoming material counts, priorities, cancellation, and now links to committed builders.

The inspector scrolls within the available screen height. These controls are view state; ordinary assignment commands preserve the existing cargo/claim behavior. Verification covers direct assignment/unassignment, staffing, worker/workplace links, follow/manual-pan cancellation, selection clearing, and 960×640 inspection.

Later / TBD: permanent workplace assignments, richer production diagnostics (F21d), resource inspectors, and route overlays.

### F21d — Economy and feedback

Status: Done (first chunk).

Open Economy with I, its bottom-bar button, or any top-bar resource. The inventory separates available, stored, reserved, carried, and workplace goods. Construction demand excludes material already delivered or committed to incoming shipments. Food coverage counts full eight-person meals using berries and bread in storage, with time until the next meal; it explicitly assumes no new deliveries.

A quiet issue-count badge replaces repeated shortage pop-ups. Messages diagnose low food, missing production buildings, unstaffed workplaces, missing builders/loggers, and exhausted timber. Selecting one opens building placement, the relevant People allocation, or tree planting. Warnings refresh from current state and disappear when resolved. Idle workers link to their inspector and show actual waiting reasons; growing crops, regrowth, and full stock targets are explained as normal waits.

Verification: read-only snapshots, stored versus reserved/carried material, committed construction demand, food coverage, staffing/low-food detection and resolution; rendered checks cover resource-bar and keyboard entry, live build/staff actions, and inventory scrolling at 960×640.

Later / TBD: production/consumption history, rolling rates, richer per-workplace diagnostics, resource filters, and configurable alerts. Forecasting future harvests is outside this first coverage estimate.

### F21e — Watching the village

Status: Done (first chunk).

Press H or choose Options → Watch village. The HUD and selection marker disappear, leaving a small pause/speed/frame/return bar. Simulation and sound continue at the current playback setting. WASD, zoom, orbit, and existing villager-follow remain usable.

H, Esc, or Manage restores the normal HUD and retained selection/drawer. Management/tool shortcuts exit watch mode before opening their controls. Entering watch cancels active placement and path strokes; clicking the village while watching does not select or place anything. World loading, switching, and main-menu entry leave watch mode. It is a local view choice, not saved settlement state.

Verification: rendered checks cover 1440×900 and 960×640 layout, read-only entry, retained selection, map clicks, playback controls, continued simulation, cancelled placement, and keyboard/button return. The broader HUD checks also pass.

Later / TBD: fully clean screenshots, optional world-label hiding, slow camera orbit, and scenic camera bookmarks.

### F20 — Lighting and atmosphere

Status: Done (first chunk).

The village uses warmer direct light, cooler ambient fill, and gently varying muted grass colors shared by both map renderers. Options → Atmosphere switches between soft daylight and golden hour. Golden hour lowers the sun for longer shadows while keeping the village readable.

Tree crowns sway slightly at different phases while their trunks and physical cells stay fixed. Motion follows saved village time, pauses with the game, and responds to playback speed. Placement previews remain still. Foliage motion can be switched off. Preferences live separately in saves/atmosphere.cfg and apply to all settlements; these choices do not change production, paths, or objectives.

Verification: rendered preset comparisons, unchanged settlement state, paused/moving foliage, motion toggle, static planting previews, preference persistence, and controls at 960×640. Existing HUD and watch-mode checks pass.

Later / TBD: day/night progression, weather, water highlights, atmospheric particles, and richer wind animation. Light mood is a player-controlled visual preset in this first pass.

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
- F19, first chunk: main menu, last-settlement Continue, campaign/free-play entry and recovery, sound settings, and saving when returning to the menu.