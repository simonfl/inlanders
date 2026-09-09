# Feature roadmap

A living plan for a small, peaceful settlement that is satisfying to arrange, manage, and watch. Pick one playable chunk at a time; details and order can change as we learn.

Windows, local play, Godot, and C# remain the foundation. Save backward compatibility is not required during prototyping. Feature IDs stay stable even when entries are removed or reordered.

## Suggested order

**Next: F09 + F12c — Village character and decorative landscaping.** Everyday square visits now give villagers a recurring break between jobs. Add a small shared palette for shaping neighborhoods.

The order below is a recommendation, not a dependency chain. **UI remains a major priority:** include the controls and feedback each feature needs, and promote further F21 improvements when play reveals a concrete need.

| Order | ID | Next playable chunk | Why here |
| --- | --- | --- | --- |
| 1 | F09 + F12c | **Village character and decorative landscaping.** One shared palette for flowers, shrubs, fences, decorative trees, and ground treatment. Start with a few objects; cottage variants can follow. | Let the player shape neighborhoods as well as production layouts. |
| 2 | F17 | **Music.** A gentle soundtrack with independent volume/mute controls in game and on the main menu. Source, tracks, and transitions TBD. | Complete another part of the original “enjoyable to watch” milestone. |
| 3 | F14 | **Village happiness.** A small satisfaction system using food variety and leisure, with readable villager reactions. Effects TBD. | Build on F05 and F04 once their basic loops are fun. |
| 4 | F12e | **Navigating a larger settlement.** Useful location jumps, camera bookmarks, or a small overview map. Pick what actual play needs. | Help players move between established neighborhoods; coordinate with F21. |
| 5 | F15 | **Small events and choices.** One visitor or request with a modest reward or tradeoff. | Add occasional variety to the established village loop. |
| 6 | F11 + F18 | **Campaign expansion.** Playtest the current four levels, then add a level introducing one new building or connected feature group. | Teach features after their standalone versions have settled. |
| 7 | F16 | **Free-build mode.** Review what a distinct sandbox should add beyond existing free play: optional objectives, setup choices, or relaxed building rules. Scope TBD. | Avoid duplicating the playable open-ended maps. |
| 8 | F12d | **Hills and terrain shaping.** Start with authored gentle elevation, readable slopes, and clear building rules. Player terrain tools can follow. | A larger terrain change; revisit once flat-map expansion feels good. |

Music and decorative work can move earlier when we want a presentation-focused chunk. Campaign pacing and UI feedback can be addressed at any time.

## Already playable

“Done” means the first useful version is shipped, not that the feature can never grow. This is the single summary of completed work; the README covers how to play, and Git history retains implementation and verification details.

| ID | Shipped |
| --- | --- |
| Milestones 1–3 | Timber harvesting and hauling, construction, worker roles and priorities, berries → grain → bread, housing, meals, village supper, and save/load. |
| F01 | Paint/remove connected dirt paths; villagers choose faster routes and walk 25% faster on paths. |
| F02 | Logger planting, visible sapling growth, renewable timber, and stump replanting. |
| F03 | Stepping feet, work poses and tools, recognizable cargo, and idle gestures. |
| F04 | Village square hosts recurring six-second breaks between jobs, up to four visitors at distinct nearby spots, with a minute cooldown per villager. Social poses, live visitor counts, saved visits, safe reassignment, and supper priority. |
| F05 | Six-log vegetable gardens, shared farmer jobs, 60-second growth, eight directly edible vegetables per crop, visible harvest/cargo, meal and newcomer coverage, and saves. Supper remains bread-based. |
| F06 | Optional newcomer pairs with spare beds and food reserves. Dynamic population, meals, supper, staffing, visuals, audio, and saves. |
| F07 | Six-log stockpile with capacity 12 and targets 0–12; local logger deposits, builder/sawyer pickups, shared haulers, source/space reservations, physical transfers, per-location economy, and saves. Logs only in this first chunk. |
| F08 | Sawmill, sawyer role, planks, and four-bed lodge. |
| F10 | Procedural work/UI sounds, positional playback, wind/birds, and persistent Effects/Nature/mute controls. |
| F11 + F18 | Four authored campaign settlements with optional contextual guidance, progress, saves, resume, replay, and level selection. |
| F12a | Irregular 32×32 Three clearings map, distant resources, scalable camera bounds, and Home overview. |
| F12b | Water and one-tile bridges, construction from a reachable bank, and access to the far side. |
| F12c | Cancelable tree/stump clearing, physical timber recovery, root removal, and reusable building ground. Decorative landscaping remains above. |
| F19 | Main menu with Continue, Campaign, Free play, Settings, Quit, and save-on-return. |
| F20 | Soft daylight/golden-hour presets, grass variation, subtle pause-aware foliage, and persistent visual preferences. |
| F21a | Compact HUD, responsive menus, scrolling, and a contextual inspector down to 960×640. |
| F21b | Building descriptions, staffing/recipes, supply guidance, recognizable previews, entrances, rotation, and placement explanations. |
| F21c | Direct job assignment, worker/workplace links, staffing controls, selection marker, and camera follow. |
| F21d | Economy inventory, current-population food coverage, actionable shortages, and idle-worker links. |
| F21e | Watch mode with a small playback/camera bar, preserved selection/follow, and easy return to management. |
| F21f | Role/idle roster filters and role labels; building categories, construction-state filters and live site summaries; clickable storage locations with camera/inspector jumps. Invitations sit above the growing roster. Filters reset when switching settlements. |
| F22 | Distinct farm/forager construction stages and wheat growth through progressively harvested rows and stubble. Bakery and other buildings also have distinct procedural models. |

## Current campaign reference — F11 / F18

The first campaign is complete. Each level starts with eight villagers and introduces a building or connected group. All buildings and tools remain available on every level. **Per-level availability and progressive unlocks are a later addition**, once the game is more polished; an unrestricted replay option remains worth considering.

| Level | Introduces | Starting village | Required goals |
| --- | --- | --- | --- |
| 1. A place to stay | Forager hut, cottages, logging/building, everyday meals | Original clearing, 96 berries, two loggers/builders/foragers and two spare workers | Finish a hut, deliver 24 fresh berries, house eight |
| 2. Bread for the table | Farm and bakery | Four cottages and a staffed hut, 96 berries | Finish a farm and bakery; deliver 16 loaves cumulatively |
| 3. Room among the trees | Sawmill, lodge, renewable woodland | Larger map, two cottages and a staffed hut, 96 berries | Finish a mill and lodge, house eight, have loggers plant four trees |
| 4. A place for everyone | Village square and shared supper | Larger map, two cottages and a staffed hut, 96 berries | Finish a square, meet the housing goal, and complete supper |

Invitations are optional and are not required by campaign goals. Supper requires everyone housed, two loaves per current villager, and one reachable gathering tile per person near the square. That is 16 loaves for the original eight; choosing to grow increases the requirements.

Guidance is contextual and can be dismissed, disabled, or reopened. Meals do not erase delivery progress. Planting goals count completed planting work, not markers or tree maturity. Completion offers continued play, the next settlement, or replay. No deadlines, deaths, forced failure, or automatic transitions.

Later levels should introduce the newer gameplay systems rather than repeat the opening housing/foraging lesson. Narrative, optional objectives, exact maps, and level count remain TBD.

## Optional follow-ups

These are possibilities within existing features, not additional commitments or a second priority list. Promote one into the ordered list when we want to tackle it.

| Area | Ideas to revisit |
| --- | --- |
| F01 / F02 — Paths and woodland | More path styles, worker-built roads/costs, traffic feedback, replanting zones, a forester, tree species and growth/yield choices. |
| F03 / F22 — Work and construction presentation | Smoother pose transitions, tree falling, particles, character variation, material piles/scaffolding, smoother crop growth, and richer harvest motions. |
| F05 — Food choices | More crops or orchards, recipe variety, garden/grain balance, and meaningful food-variety effects with F14. |
| F06 — Population | Arrival journeys/timing, larger-population balancing and performance, population preferences, families, and more names/appearances. |
| F07 — Storage and hauling | Food and plank storage, resource filters, delivery priorities, capacities, broader logistics controls, and demolition/relocation. |
| F08 — Materials | More plank buildings, mixed-material recipes, adjustable stock targets, upgrades, and other materials. |
| F10 / F17 — Audio | More organic sounds, extra variations, mixing by zoom, tighter impact timing, and music transitions. |
| F12a / F12b — Maps and water | More authored geography, richer map edges and shores, wider bridges, islands, water animation, bridge variants, and demolition rules. |
| F12c / F12d — Landscaping | Area selection, clearing-time tuning, grass/earth painting, terraces, ramps, retaining walls, and raise/lower/level tools. Decorative objects belong in the shared F09 palette. |
| F19 — Main menu | Title artwork, save-slot browsing, and keyboard/controller navigation. Music settings belong with F17. |
| F20 — Atmosphere | Day/night progression, weather, water highlights, atmospheric particles, and richer wind animation. |
| F21 — Management | Permanent workplace assignments, route overlays, resource filters, production/consumption history, rates, richer workplace diagnostics, and configurable alerts. |
| F21e — Watching | Fully clean screenshots, optional label hiding, slow camera orbit, and scenic camera bookmarks shared with F12e. |

The original **milestone 4 — “Make it enjoyable to watch”** spans F03, F10, F17, F20, F21, and F22. Most have a first pass; music remains unstarted. **F04 is the separate gathering-places feature.**

## How we take a chunk

1. Pick an entry from the ordered list; mark it **In progress**.
2. Write the player experience, smallest useful version, and a short playable check. Leave unresolved choices TBD.
3. Implement it with current-format saves and relevant verification.
4. Record what shipped in the completed table, move remaining ideas to follow-ups, and update the next recommendation.

Keep supporting UI inside the feature that needs it. Avoid turning this roadmap into a test log or a list of prototype cleanup tasks.

## Parking lot

Fishing, trade, household routines, procedural maps, and hidden discoveries remain possibilities without a commitment. Infinite terrain or purchased land is not required for map expansion.

Combat, multiplayer, a large technology tree, and a full life simulation are outside the current direction.
