# Feature roadmap

A living plan for a small, peaceful settlement that is satisfying to arrange, manage, and watch. Pick one playable chunk at a time; details and order can change as we learn.

Windows, local play, Godot, and C# remain the foundation. Save backward compatibility is not required during prototyping. Incompatible or invalid development saves may be discarded; do not add migration or repair work to preserve them. Fresh saves must still roundtrip correctly. Feature IDs stay stable even when entries are removed or reordered.

## Second phase — make the village worth watching and improving

The original ordered roadmap shipped its first versions. The next phase responds to the design review and the user's feedback that the buildings look flat and the village is not visually compelling.

**F23a is implemented and awaiting visual feedback.** Review the [matched before/after scene](ART_REVIEW_F23A.md) before expanding the style. **Next feature: F11b / F18b — Across the river.** F14b now makes meal variety reflect actual portions eaten, supporting the [resident-needs and campaign plan](CAMPAIGN_SYSTEMS.md). F19b provides session recovery and F16b now allows physical demolition in normal play. The first substantial river campaign proof can now test layout mistakes and recovery. F23b follows visual acceptance or another focused art iteration.

See [the comprehensive design review](DESIGN_REVIEW.md) for the complete building/cost audit, visual direction, enjoyment assessment, confirmed defects, and playtest questions. The baseline review contains earlier candidate costs; the F24a/F24b reviews record adopted values, measured tradeoffs and remaining playtest limits. **UI remains a major priority.**

## Suggested order

This is a recommendation, not a dependency chain. Work one playable chunk at a time; revise the order after seeing the result.

| Order | ID | Playable chunk | What success looks like |
| --- | --- | --- | --- |
| Review | F23a | **Visual identity slice — implemented, aesthetic acceptance pending.** Cottage porch/window depth, thick roofs and stone feet; bakery oven mass and recessed shop; open braced sawmill with progress-driven saw. Workshop displays follow real buffers. Costs and footprints unchanged. | Review the scene at the actual camera, without labels, at 960/1440 and four directions. The user finds the direction compelling before F23b proceeds; another focused iteration remains possible. |
| After art review | F23b | **The complete building family.** Apply the visual language to the other seven types: a visibly larger lodge, woodland shelter, distinct field/garden, civic square, storage bay and bridge. Restrain ground contrast; cap yard stock visuals. | A populated village has distinct forms, clear entrances and consistent materials. Fields and open spaces retain their intended low profiles; buildings stop looking like isolated objects on trays. |
| 1 | F11b / F18b | **A campaign beyond the tutorial.** Build one substantial River settlement as a proof of sustained planning, then a sequence of harder authored settlements. Keep the current lessons as onboarding; introduce situations as well as buildings. See the campaign expansion below. | A first-time player makes consequential choices throughout a roughly 20–30-minute River settlement, with multiple viable plans and a visible result. Later scenarios aim for 30–60 minutes. Duration is a playtest target, never a completion timer. All buildings remain available for now. |
| 2 | F25a | **Home, rest and recreation.** Deepen existing needs with home destinations, occasional rest and legible square/service participation. Use existing housing and civic buildings first. | Residents visibly use the village; unmet rest or recreation has a clear reason. Start with satisfaction/activity, without cascading fatigue penalties or a required day/night system. |
| 3 | F26a | **Life by the lake.** One fishing dock/boat, connected water habitat and delivered fish, paired with an authored campaign settlement. | Shore choice, shared replenishing fish stocks and worker time create a useful alternative to cultivation. Boats visibly leave, return and unload; several food mixes work. |
| 4 | F02b | **Managed woodland.** Preserve chosen trees and maintain a small replanting zone using existing loggers. Clearing and building orders take explicit precedence. | Several harvest/regrowth cycles without repetitive individual planting; the player shapes a productive grove and open village land. |
| 5 | F07b | **Useful neighborhood logistics.** Show supply routes and explain local harvest versus hauler redistribution first. Prototype local food/plank storage only where a settlement demonstrates useful payoff beyond the larger food loads. Clarify whether the forager hut becomes a real collection point or remains a worker permit. | A local arrangement measurably reduces unnecessary trips and is understandable to the player. Add only storage behavior that creates useful layout decisions. |
| 6 | F03b / F04b | **Believable work and village life.** Improve the most visible tool contact, tree felling, deliveries and harvesting. Present the F25 home/rest visits and a small set of social moments with recognizable residents; resident-need rules belong to F25, not this animation pass. | An observer understands work without labels and enjoys breaks and inhabited spaces. Pausing and interruptions preserve convincing animation; no constant visual noise. |

Before population, map size, or decoration density expands further, profile a decorated 16–24-person settlement and a prolonged paused placement preview. Optimize measured costs in the owning chunk; avoid a standalone engine-rewrite project.

**Art acceptance is a player judgment.** Screenshots, clips and side-by-side comparisons at the real camera are required for F23; successful smoke tests alone do not establish appeal. Keep larger terraforming tools, orchards and additional visitors as later candidates. Fishing, stone and wildlife now have explicit campaign-oriented plans below; they follow the first substantial river scenario rather than becoming prerequisites for it.

## Already playable

“Done” means the first useful version is shipped, not that the feature can never grow. This is the single summary of completed work; the README covers how to play, and Git history retains implementation and verification details.

| ID | Shipped |
| --- | --- |
| Milestones 1–3 | Timber harvesting and hauling, construction, worker roles and priorities, berries → grain → bread, housing, meals, village supper, and save/load. |
| F01 | Paint/remove connected dirt paths; villagers choose faster routes and walk 25% faster on paths. |
| F02 | Logger planting, visible sapling growth, renewable timber, and stump replanting. |
| F03 | Stepping feet, work poses and tools, recognizable cargo, and idle gestures. |
| F04 | Village square hosts recurring six-second breaks between jobs, up to four visitors at distinct nearby spots, with a minute cooldown per villager. Social poses, live visitor counts, saved visits, safe reassignment, and supper priority. |
| F05 | Four-log vegetable gardens, shared farmer jobs, 60-second growth, eight directly edible vegetables per crop, visible harvest/cargo, meal and newcomer coverage, and saves. Supper remains bread-based. |
| F06 | Optional newcomer pairs with spare beds and food reserves. Dynamic population, meals, supper, staffing, visuals, audio, and saves. |
| F07 | Four-log stockpile with capacity 12 and targets 0–12; local logger deposits, builder/sawyer pickups, shared haulers, source/space reservations, physical transfers, per-location economy, and saves. Logs only in this first chunk. |
| F08 | Sawmill, sawyer role, planks, and four-bed lodge. |
| F09 | Free flowers, shrubs, rotated low fences, ornamental trees, and walkable pebble cover; repeat placement/removal, protected access, live rerouting, saved layouts, and three stable cottage roof colours. |
| F10 | Procedural work/UI sounds, positional playback, wind/birds, and persistent Effects/Nature/mute controls. |
| F11 + F18 | Five authored campaign settlements with optional contextual guidance, progress, saves, resume, replay, and level selection. |
| F12a | Irregular 32×32 Three clearings map, distant resources, scalable camera bounds, and Home overview. |
| F12b | Water and one-tile bridges, construction from a reachable bank, and access to the far side. |
| F12c | Cancelable tree/stump clearing, physical timber recovery, root removal, and reusable building ground. Decorative landscaping shares the F09 palette. |
| F12d | Two raised meadows on new Three clearings maps, with walkable slopes and level hilltops. Whole footprints and entrances must be level; paths, planting and decorations follow slopes. Terrain, actors, crops, previews, picking, camera focus and positional sound share heights. Height data and uphill routes persist; original/campaign maps and riverbanks stay flat. |
| F12e | Three named camera views per settlement; saved focus, zoom and orbit, Options controls, 1–3 recall / Ctrl+1–3 set, clearing/overwrite, and recall during Watch mode. Existing workplace jumps and Home overview remain. |
| F14 / F14b | Per-villager satisfaction from meals, village variety proportional to portions actually eaten, housing coverage, and a completed square break within two minutes. Village average, expandable reasons below worker controls, saved meal portions, and cheerful/unsettled idle reactions; no additional productivity penalty. Available foods are shared evenly, with last-meal feedback in Economy and the inspector. Level 5 requires two full meals with at least a quarter vegetables and a quarter other food. |
| F15 | One gardener visit from day 3 with a finished hut; 8 berries unlock freely placeable sunflowers. Quiet yard marker/Goals card, projected food reserve, accept/decline, no deadline, saved outcomes, and preserved delivery milestones. |
| F16b | Normal-play demolition: service/beds stop on order; builders evacuate buffers and crops, dismantle over 12 work seconds, then physically recover all construction materials. Cancel before dismantling; interrupted workers retain cargo and roles. Progressive building stages, work marker, inspector consequences and current saves. Bridge orders preserve alternative access, including other pending demolitions. |
| F16 | Creative on both maps: instant free completed buildings, no hunger or meals, all decorations, immediate clearing with timber recovery, and safe completed-building removal. Production/cargo stay physical; bridge removal protects access. Separate New/Resume/Restore saves and Continue, mode-aware UI, beds-only invitations, and neutral food satisfaction. |
| F17 | Original 96-second procedural soundtrack with soft plucks/chords and a gentle loop; independent Music volume/mute in Options and main-menu Settings, master mute, persisted preferences, and uninterrupted transport across pause and settlement changes. |
| F19 | Main menu with Continue, Campaign, Free play, Settings, Quit, and save-on-return. |
| F19b | Two rolling autosaves per sandbox map/mode and campaign level, every two real minutes including changed paused layouts; explicit restore and undo in Options. Autosaves update Continue but preserve F5/F9 checkpoints. Restart retains live progress and opens paused; window close saves the session and remains open on failure. |
| F19a | Supper releases its gathering destinations on completion, so subsequent decoration/construction on vacated spots does not invalidate fresh saves or campaign snapshots. No migration or repair of old saves. |
| F20 | Soft daylight/golden-hour presets, grass variation, subtle pause-aware foliage, and persistent visual preferences. |
| F21a | Compact HUD, responsive menus, scrolling, and a contextual inspector down to 960×640. |
| F21b | Building descriptions, staffing/recipes, supply guidance, recognizable previews, entrances, rotation, and placement explanations. |
| F21c | Direct job assignment, worker/workplace links, staffing controls, selection marker, and camera follow. |
| F21d | Economy inventory, current-population food coverage, actionable shortages, and idle-worker links. |
| F21e | Watch mode with a small playback/camera bar, preserved selection/follow, and easy return to management. |
| F21f | Role/idle roster filters and role labels; building categories, construction-state filters and live site summaries; clickable storage locations with camera/inspector jumps. Invitations sit above the growing roster. Filters reset when switching settlements. |
| F21g | Model-thumbnail cards with purpose/cost/staff; separate Place, Landscape and Existing sections, pinned tool guidance/cancel, and map hints that avoid drawer controls. Text entry stops camera shortcuts. World-label preference works in Options/Watch and survives scene rebuilds. Break, garden and storage guidance wording corrected. |
| F21h | Workplace input/work/travel/collection/paused/target states, source and worker links, explicit named global role transfers, pause after current work, and per-workplace 0–200/no-limit targets counting village-wide stock and committed production. Recent 180-second pantry arrivals versus meals, shortage-to-resume links, current-format saves and narrow/wide controls. Local food/plank logistics and permanent workplace assignment remain follow-ups. |
| F22 | Distinct farm/forager construction stages and wheat growth through progressively harvested rows and stubble. Bakery and other buildings also have distinct procedural models. |
| F24a | Shared building definitions; hut/farm/garden/stockpile 4 logs, bakery 8 logs, cottage/mill/square/bridge 6 logs, lodge 12 planks and matching mill target. Footprints/timing/recipes unchanged. Comparable campaign, housing, food and storage experiments documented; F24b completes the initial payoff review below. |
| F24b | Four-loaf bakery deliveries and four-grain harvest loads with visible cargo and safe interruption/saves. Matched food layouts show a modest bread payoff; a woodland stockpile repays setup over sustained construction, while haulers trade extra labor for speed. Guidance now calls haulers optional. Costs, growth and recipes unchanged; see the measured F24b review for limits. |
| F23a implementation | Revised cottage/bakery/sawmill models and construction stages; actual input/output stock displays, baking-only oven glow and pause-aware saw motion. Matched comparison and repeatable art scene available. Visual acceptance remains open above. |

## Current campaign reference — F11 / F18

The first five-level campaign is complete. Each level starts with eight villagers and introduces a building or connected group. All buildings and tools remain available on every level. **Per-level availability and progressive unlocks are a later addition**, once the game is more polished; an unrestricted replay option remains worth considering.

| Level | Introduces | Starting village | Required goals |
| --- | --- | --- | --- |
| 1. A place to stay | Forager hut, cottages, logging/building, everyday meals | Original clearing, 96 berries, two loggers/builders/foragers and two spare workers | Finish a hut, deliver 24 fresh berries, house eight |
| 2. Bread for the table | Farm and bakery | Four cottages and a staffed hut, 96 berries | Finish a farm and bakery; deliver 16 loaves cumulatively |
| 3. Room among the trees | Sawmill, lodge, renewable woodland | Larger map, two cottages and a staffed hut, 96 berries | Finish a mill and lodge, house eight, have loggers plant four trees |
| 4. A place for everyone | Village square and shared supper | Larger dry map, four cottages, staffed hut/farm/bakery, 96 berries | Finish a square and complete supper with everyone housed |
| 5. More for the table | Vegetable garden and food choice | Larger dry map, four cottages, staffed hut, one farmer, 48 berries | Finish a garden, deliver 16 vegetables, and serve two full meals with at least a quarter vegetable portions and a quarter other-food portions |

Invitations are optional and are not required by campaign goals. Supper requires everyone housed, two loaves per current villager, and one reachable gathering tile per person near the square. That is 16 loaves for the original eight; choosing to grow increases the requirements.

Guidance is contextual and can be dismissed, disabled, or reopened. Meals do not erase delivery progress. Planting goals count completed planting work, not markers or tree maturity. Completion offers continued play, the next settlement, or replay. No deadlines, deaths, forced failure, or automatic transitions.

Vegetable deliveries and qualifying meal times are cumulative; carried or still-growing crops do not count. Level 5 needs one garden and no bakery, visitor trade, happiness threshold, invitation, or supper. Scripted normal-speed runs finish the current lessons in roughly two to eight minutes; human play can take longer. Further narrative and optional objectives remain TBD.

## Campaign expansion — F11b / F18b

**The current five settlements are an introductory chapter, not the desired scope of the campaign.** The campaign should teach and then test mastery: planning construction order, allocating scarce labor, choosing food production, reading geography, and expanding without exhausting the economy that supports expansion. A later level can introduce a new problem using familiar buildings; it does not need a new building to justify its existence.

Keep the existing lessons available while prototyping the first substantial scenario. Later, review whether to combine or make some lessons optional. Final chapter names, numbering, populations, starting stocks and thresholds remain TBD. Do not promise a duration by multiplying delivery goals or slowing production.

| Proposed settlement | First-play duration target | Central decisions and progression | Completion direction |
| --- | --- | --- | --- |
| **Across the river** — first implementation slice | 20–30 minutes | Begin with a small functioning village and modest reserves. Nearby land can feed the initial population but offers limited room for growth; the far bank provides alternative field sites and woodland. Choose bridge position, construction order and when to shift labor from food into expansion. Prepare housing and supply before inviting a second group. | Establish an inhabited far-bank neighborhood, house the expanded population, and demonstrate that the expanded village can feed itself across a short, clearly explained meal window. Food may travel across the river; do not imply local food storage already exists. |
| **The long haul** | 30–40 minutes | Productive clearings are separated by distance. Decide between compact housing and production closer to resources; invest in paths and log stockpiles, then reconsider routes as the village grows. | Complete and support two occupied areas with continuing food and timber work. Judge viable operation, not a mandatory stockpile layout or an arbitrary delivery quota. |
| **Room to grow** | 30–45 minutes | Limited level building land creates competition between fields, cottages and more efficient lodges. Expand in player-triggered stages, balancing milling, construction and food labor. | Support a larger village within the authored terrain and finish a recognizable village center. Several housing/food mixes should work; exact population follows balance and performance checks. |
| **A lasting village** — chapter finale | 40–60 minutes | Combine a crossing, distant timber and limited prime land. Plan growth and replenishment, then redirect workers toward a shared gathering without draining everyday supplies. | A housed, reliably fed settlement with replenished woodland and a completed celebration. Assess several systems working together; avoid mandatory ownership of every building. |

These are first-time human play targets with ordinary use of pause and speed controls, not minimum times, deadlines, or measured current runtimes. Skilled play should finish faster. Quiet watching belongs in the pacing, but the player should have worthwhile decisions beyond the opening placement burst.

**Recovery design:** F16b returns all construction materials through worker trips. Start the river proof with this forgiving rule; judge whether lost worker time and temporary service/housing disruption provide enough consequence before considering material loss. Do not make a salvage penalty or relocation tool a prerequisite for the scenario.

**First chunk:** author Across the river and the minimum objective support it actually needs. Test opening → crossing → prepared growth → supported neighborhood before producing the other maps. Geography should make the crossing useful naturally; objective wording should explain the settlement's purpose. Compare at least two plausible approaches (for example early bridging versus building a stronger starting food supply). Existing global staffing, log-only stockpiles and movement rules must support the intended solution; defer goals that require unimplemented local services or per-workplace assignments.

**Objective rules:** use visible milestones and a small number of simultaneous outcome conditions. A brief operational check after growth distinguishes a sustainable settlement from a one-time stockpile, but must not become the main source of level length. Show what counts, progress and why a condition is unmet. Completed construction milestones stay completed; operational conditions may recover when missed. Stage transitions and arrivals are player-triggered, explained in advance, and saved. Failure to meet a condition invites adjustment, not restart. No deaths, forced failure, surprise disasters or deadlines.

**Playtest acceptance:** record elapsed time alongside decisions, idle stretches, bottlenecks and recoveries. A player should need to respond to the consequences of their earlier layout or staffing choices, be able to explain those consequences, and recover from an imperfect plan. If placing everything at the start and fast-forwarding wins comfortably, revise the scenario. If most time is spent waiting for counters, remove padding. Validate both a competent route and a recoverable inefficient route; no required perfect build order. The finished village should visibly reward the effort. The F25/F26 plan below proposes later buildings tied to needs and geography; the first river proof does not depend on them. Availability restrictions remain deferred.

## Needs, buildings and environmental opportunities — F25 / F26

See the [campaign systems plan](CAMPAIGN_SYSTEMS.md) for the need → building → landscape relationships, level sketches and open decisions. This is a design direction; prices, yields, service capacities and final sequence remain TBD.

**Campaign progression:** establish a home (food, reachable shelter and rest) → make a community (actual recreation and a useful diet) → improve life here (comfort and a chosen civic project). Introduce expectations through explained, player-triggered campaign stages, not surprise population thresholds. Education and religion/reflection remain later possibilities with a distinct purpose still to prove; they are not additional mandatory meters. The systems plan includes a concrete F25a home/rest experiment and an authored terrain shortlist: fishing waters, quarry outcrops, woodland habitat, competing meadow/terrace space and separated neighborhoods.

**Environment is part of the building design.** Each new producer ships with readable placement prerequisites, a landscape where it is useful, a visible output/use and an alternative viable plan. Not every map needs every resource. Start with geometry, access and shared habitat/deposit stocks; fertility, drinking water and deeper resource chains remain TBD. Campaign success measures residents supported and projects used, rather than ownership of every new building.

- **First deepen food, shelter/rest and company.** F14b measures actual meals. F25a gives existing cottages/lodges and squares a stronger resident-facing purpose before adding more meters. Comfort can later motivate visible home improvements.
- **F26a: water → fishing dock/boat → food.** Shared fish habitat, shoreline access and travel distinguish fishing from gardens. Pair the first implementation with Life by the lake.
- **F26b: rock outcrop → quarry → lasting project.** Add stone together with a meaningful consumer, such as a civic building or improved crossing. A deposit can run out; access and local transport matter. Mixed-material costs need support, but no iron/tool-wear chain yet.
- **F26c: retained woodland → wildlife habitat → hunting lodge/game.** Clearing competes with a replenishing food source. Start with habitat stock and visible animals; no full ecosystem simulation.
- **F25b, later/TBD: comfort, learning and reflection.** Carpenter/home improvements, adult workshop/library, chapel/shrine or secular reflection space are candidates. Define the resident benefit first. No compulsory religion, school-age simulation, or requirement to own every civic building.

**Building shortlist and design gates:** the campaign systems plan now sketches a gathering hall as a possible first stone consumer, carpenter-funded home improvements, small recreation gardens, and later learning/reflection venues. Choose between candidates through a playable purpose, not a larger construction menu. F25a must explain actual meals, home/rest and recreation participation; F26 must show habitat/deposit quality and access before investment. Balance new producers against construction, labor, transport and land use on contrasting maps. Costs and detailed need schedules remain TBD; later needs can be merged or cut if they duplicate recreation.

Keep Across the river as the first longer level using existing systems. Later campaign roles: **Life by the lake**, **A place to call home**, **Built to last**, **The living woods**, then an expanded **A lasting village**. The long haul and Room to grow can become challenges within these maps or optional scenarios, rather than padding the sequence with overlapping levels. These additions are planned, not implemented. All-building availability and the removal of seasons remain unchanged.

## Optional follow-ups

These are remaining possibilities, not a second priority list. Items promoted into the ordered second phase above take precedence; the notes here describe possible extensions beyond those first chunks.

| Area | Ideas to revisit |
| --- | --- |
| F01 / F02 — Paths and woodland | More path styles, worker-built roads/costs, traffic feedback, replanting zones, a forester, tree species and growth/yield choices. |
| F03 / F22 — Work and construction presentation | Smoother pose transitions, tree falling, particles, character variation, material piles/scaffolding, smoother crop growth, and richer harvest motions. |
| F05 — Food choices | More crops or orchards, recipe variety, garden/grain balance, and further diet choices building on the actual-meal rule shipped in F14b. |
| F06 — Population | Arrival journeys/timing, larger-population balancing and performance, population preferences, families, and more names/appearances. |
| F16 — Creative | Relocation, multi-object removal, resource setup controls, population preferences, and bush rearrangement if arranging villages calls for them. Keep normal economy saves separate. |
| F07 — Storage and hauling | Compact yard visuals for large reserves (Creative clearing can produce tall stacks), food and plank storage, resource filters, delivery priorities, capacities, broader logistics controls, and relocation. Normal-play demolition shipped in F16b; retain its physical goods recovery. |
| F08 — Materials | More plank buildings, mixed-material recipes, upgrades and other materials. Adjustable workplace stock targets shipped in F21h. |
| F15 — Visitors | More encounters and rewards after playtesting the first offer; no seed inventory, repeat-trade economy, or production bonus in the first version. |
| F14 — Happiness | Playtest thresholds and break duration, richer reactions, and additional reasons only when they create useful decisions. |
| F09 — Village character | More palettes and cottage details, player-selected house colours, connected fence runs, decoration brush strokes, and richer ornamental planting. |
| F10 / F17 — Audio | More organic sounds, extra variations, mixing by zoom, tighter impact timing, more musical themes, and music transitions. |
| F12a / F12b — Maps and water | More authored geography, richer map edges and shores, wider bridges, islands, water animation, and bridge variants. Normal-play demolition and protected access shipped in F16b. |
| F12c / F12d — Landscaping | Player terrain shaping, area selection, clearing-time tuning, grass/earth painting, constructed ramps, retaining walls, and raise/lower/level tools. Decorative objects belong in the shared F09 palette. |
| F19 — Main menu | Title artwork, save-slot browsing, and keyboard/controller navigation. Music settings belong with F17. |
| F20 — Atmosphere | Day/night progression, weather, water highlights, atmospheric particles, and richer wind animation. |
| F21 — Management | Keyboard focus navigation, permanent workplace assignments, route overlays, resource filters, longer or per-workplace productivity history, and configurable alerts. Basic workplace diagnostics and recent food rates shipped in F21h; assess their clarity before adding more metrics. Add catalog search only if categories stop being sufficient. |
| F21e — Watching | Hide the playback bar for fully clean screenshots and add a slow camera orbit. World-label hiding has shipped in F21g; scenic camera bookmarks already belong to F12e. |

The original **milestone 4 — “Make it enjoyable to watch”** spans F03, F10, F17, F20, F21, and F22. All now have a first pass. **F04 is the separate gathering-places feature.**

## How we take a chunk

1. Pick an entry from the ordered list; mark it **In progress**.
2. Write the player experience, smallest useful version, and a short playable check. For substantial features, get a short game-designer review before implementation: meaningful choice, fit, overlap, scope, and playable checks. Use UX or simulation reviewers when the change calls for them. Leave unresolved choices TBD.
3. Implement it with current-format saves and relevant verification.
4. Record what shipped in the completed table and move remaining ideas to follow-ups.
5. Reevaluate the roadmap after every chunk using what implementation and play revealed. Reorder, clarify, combine, cut, expand, or add features when that improves the game; the current list is not a fixed commitment. Keep feature IDs stable, UI prominent, and uncertain details TBD. Do not restore explicitly removed features without a new reason and user agreement.
6. Update the next recommendation and briefly explain meaningful scope or priority changes. Commit and push the chunk with its reviewed roadmap.

Keep supporting UI inside the feature that needs it. Avoid turning this roadmap into a test log or a list of prototype cleanup tasks.

## Parking lot

Orchards and hidden discoveries remain candidates. A neighborhood pantry/market stays with F07b; carpenter/home improvements and limited rest visits are described in F25. Fishing, stone and woodland wildlife have moved into the F26 campaign plan. Trade, broader household routines and procedural maps remain possibilities without a commitment. Infinite terrain or purchased land is not required for map expansion.

Combat, multiplayer, a large technology tree, and a full life simulation are outside the current direction.
