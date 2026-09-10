# Feature roadmap

A living plan for a small, peaceful settlement that is satisfying to arrange, manage, and watch. Pick one playable chunk at a time; details and order can change as we learn.

Windows, local play, Godot, and C# remain the foundation. Save backward compatibility is not required during prototyping. Feature IDs stay stable even when entries are removed or reordered.

## Second phase — make the village worth watching and improving

The original ordered roadmap shipped its first versions. The next phase responds to the design review and the user's feedback that the buildings look flat and the village is not visually compelling.

**F23a is implemented and awaiting visual feedback.** Review the [matched before/after scene](ART_REVIEW_F23A.md) before expanding the style across the catalog. **Next independent chunk: F21g — clearer building and management UI.** It can proceed while the visual direction is being judged; F23b follows acceptance or another focused art iteration. The bounded post-supper save repair has shipped.

See [the comprehensive design review](DESIGN_REVIEW.md) for the complete building/cost audit, visual direction, enjoyment assessment, confirmed defects, and playtest questions. Candidate costs there are experiments, not adopted balance changes. **UI remains a major priority.**

## Suggested order

This is a recommendation, not a dependency chain. Work one playable chunk at a time; revise the order after seeing the result.

| Order | ID | Playable chunk | What success looks like |
| --- | --- | --- | --- |
| Review | F23a | **Visual identity slice — implemented, aesthetic acceptance pending.** Cottage porch/window depth, thick roofs and stone feet; bakery oven mass and recessed shop; open braced sawmill with progress-driven saw. Workshop displays follow real buffers. Costs and footprints unchanged. | Review the scene at the actual camera, without labels, at 960/1440 and four directions. The user finds the direction compelling before F23b proceeds; another focused iteration remains possible. |
| 2 | F23b | **The complete building family.** Apply the visual language to the other seven types: a visibly larger lodge, woodland shelter, distinct field/garden, civic square, storage bay and bridge. Restrain ground contrast; cap yard stock visuals. | A populated village has distinct forms, clear entrances and consistent materials. Fields and open spaces retain their intended low profiles; buildings stop looking like isolated objects on trays. |
| 3 | F21g | **A clear building and management interface.** Visual building cards with purpose/cost/staff; separate Place, Landscape and Existing buildings. Selected-tool guidance stays nearby. Fix text-entry camera movement and stale activity labels; make world-label visibility controllable. | A player can choose the right building and understand the next step without reading a long manual. Typing never pans the camera. A quiet Watch view works without floating-label clutter. |
| 4 | F24 | **Building roles and investment.** Test the full catalog's material, land, labor, startup and payoff tradeoffs. Compare cottage/lodge efficiency, food alternatives and stockpile usefulness. Centralize definitions when tuning. | Multiple defensible building choices; prices match purpose and do not merely add waiting. All UI and campaign goals agree with adopted values. Initial candidate ranges are in the review, with final values TBD. |
| 5 | F21h | **Understand and direct work.** First add precise workplace states, relevant source/worker links and recent food production/consumption. Then add workplace pause and simple output targets. Explain global role transfers; preferred workplace assignment remains a decision to test. | Distinguish missing staff/input, travel, collection, pause and target met. Deliberately keep a garden running while pausing a grain field, without lost cargo or surprise staffing changes. |
| 6 | F16b | **Rearrange a village safely.** Bring deliberate completed-building demolition to normal play, with goods evacuation, stated material recovery and housing/access effects. Creative undo and moving active buildings remain separate follow-up candidates. | Recover from an awkward layout without restarting. No stranded workers, lost carried goods, or silently disconnected bridge access. Recovery timing and costs are visible before the action. |
| 7 | F14b | **Meals that make variety meaningful.** Base the benefit and level-5 lesson on a clearly explained consumption rule, with last-meal feedback. Choose village-level participation versus proportion served before implementation. | One untouched vegetable cannot imply everyone ate a varied meal. Scarcity still feeds as many residents as possible; one garden plus foraging remains a viable lesson. |
| 8 | F11b / F18b | **A campaign beyond the tutorial.** Build one substantial River settlement as a proof of sustained planning, then a sequence of harder authored settlements. Keep the current lessons as onboarding; introduce situations as well as buildings. See the campaign expansion below. | A first-time player makes consequential choices throughout a roughly 20–30-minute River settlement, with multiple viable plans and a visible result. Later scenarios aim for 30–60 minutes. Duration is a playtest target, never a completion timer. All buildings remain available for now. |
| 9 | F02b | **Managed woodland.** Preserve chosen trees and maintain a small replanting zone using existing loggers. Clearing and building orders take explicit precedence. | Several harvest/regrowth cycles without repetitive individual planting; the player shapes a productive grove and open village land. |
| 10 | F07b | **Useful neighborhood logistics.** Prototype local food/plank storage and visible supply routes in one settlement. Clarify whether the forager hut becomes a real collection point or remains a worker permit. | A local arrangement measurably reduces unnecessary trips and is understandable to the player. Add only storage behavior that creates useful layout decisions. |
| 11 | F03b / F04b | **Believable work and village life.** Improve the most visible tool contact, tree felling, deliveries and harvesting. Add a small set of social/home-related moments with recognizable residents, without a new household-needs simulation. | An observer understands work without labels and enjoys breaks and inhabited spaces. Pausing and interruptions preserve convincing animation; no constant visual noise. |

**Separate session follow-up — F19b:** unify recoverable restart, save-on-exit and mode-aware rolling autosave; keep manual F5/F9 restore semantics explicit. Schedule this as a small reliability chunk without delaying the F23a visual slice. No save-version migration work.

Before population, map size, or decoration density expands further, profile a decorated 16–24-person settlement and a prolonged paused placement preview. Optimize measured costs in the owning chunk; avoid a standalone engine-rewrite project.

**Art acceptance is a player judgment.** Screenshots, clips and side-by-side comparisons at the real camera are required for F23; successful smoke tests alone do not establish appeal. Keep larger terraforming tools, more resources, orchards/fishing, and additional visitors as later candidates until the existing village is attractive and understandable.

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
| F09 | Free flowers, shrubs, rotated low fences, ornamental trees, and walkable pebble cover; repeat placement/removal, protected access, live rerouting, saved layouts, and three stable cottage roof colours. |
| F10 | Procedural work/UI sounds, positional playback, wind/birds, and persistent Effects/Nature/mute controls. |
| F11 + F18 | Five authored campaign settlements with optional contextual guidance, progress, saves, resume, replay, and level selection. |
| F12a | Irregular 32×32 Three clearings map, distant resources, scalable camera bounds, and Home overview. |
| F12b | Water and one-tile bridges, construction from a reachable bank, and access to the far side. |
| F12c | Cancelable tree/stump clearing, physical timber recovery, root removal, and reusable building ground. Decorative landscaping shares the F09 palette. |
| F12d | Two raised meadows on new Three clearings maps, with walkable slopes and level hilltops. Whole footprints and entrances must be level; paths, planting and decorations follow slopes. Terrain, actors, crops, previews, picking, camera focus and positional sound share heights. Height data and uphill routes persist; original/campaign maps and riverbanks stay flat. |
| F12e | Three named camera views per settlement; saved focus, zoom and orbit, Options controls, 1–3 recall / Ctrl+1–3 set, clearing/overwrite, and recall during Watch mode. Existing workplace jumps and Home overview remain. |
| F14 | Per-villager satisfaction from meals, food choices recorded at meal time, housing coverage, and a completed square break within two minutes. Village average, expandable reasons below worker controls, saved history, and cheerful/unsettled idle reactions; no additional productivity penalty. |
| F15 | One gardener visit from day 3 with a finished hut; 8 berries unlock freely placeable sunflowers. Quiet yard marker/Goals card, projected food reserve, accept/decline, no deadline, saved outcomes, and preserved delivery milestones. |
| F16 | Creative on both maps: instant free completed buildings, no hunger or meals, all decorations, immediate clearing with timber recovery, and safe completed-building removal. Production/cargo stay physical; bridge removal protects access. Separate New/Resume/Restore saves and Continue, mode-aware UI, beds-only invitations, and neutral food satisfaction. |
| F17 | Original 96-second procedural soundtrack with soft plucks/chords and a gentle loop; independent Music volume/mute in Options and main-menu Settings, master mute, persisted preferences, and uninterrupted transport across pause and settlement changes. |
| F19 | Main menu with Continue, Campaign, Free play, Settings, Quit, and save-on-return. |
| F19a | Supper releases its gathering destinations on completion, so subsequent decoration/construction on vacated spots does not invalidate fresh saves or campaign snapshots. No migration or repair of old saves. |
| F20 | Soft daylight/golden-hour presets, grass variation, subtle pause-aware foliage, and persistent visual preferences. |
| F21a | Compact HUD, responsive menus, scrolling, and a contextual inspector down to 960×640. |
| F21b | Building descriptions, staffing/recipes, supply guidance, recognizable previews, entrances, rotation, and placement explanations. |
| F21c | Direct job assignment, worker/workplace links, staffing controls, selection marker, and camera follow. |
| F21d | Economy inventory, current-population food coverage, actionable shortages, and idle-worker links. |
| F21e | Watch mode with a small playback/camera bar, preserved selection/follow, and easy return to management. |
| F21f | Role/idle roster filters and role labels; building categories, construction-state filters and live site summaries; clickable storage locations with camera/inspector jumps. Invitations sit above the growing roster. Filters reset when switching settlements. |
| F22 | Distinct farm/forager construction stages and wheat growth through progressively harvested rows and stubble. Bakery and other buildings also have distinct procedural models. |
| F23a implementation | Revised cottage/bakery/sawmill models and construction stages; actual input/output stock displays, baking-only oven glow and pause-aware saw motion. Matched comparison and repeatable art scene available. Visual acceptance remains open above. |

## Current campaign reference — F11 / F18

The first five-level campaign is complete. Each level starts with eight villagers and introduces a building or connected group. All buildings and tools remain available on every level. **Per-level availability and progressive unlocks are a later addition**, once the game is more polished; an unrestricted replay option remains worth considering.

| Level | Introduces | Starting village | Required goals |
| --- | --- | --- | --- |
| 1. A place to stay | Forager hut, cottages, logging/building, everyday meals | Original clearing, 96 berries, two loggers/builders/foragers and two spare workers | Finish a hut, deliver 24 fresh berries, house eight |
| 2. Bread for the table | Farm and bakery | Four cottages and a staffed hut, 96 berries | Finish a farm and bakery; deliver 16 loaves cumulatively |
| 3. Room among the trees | Sawmill, lodge, renewable woodland | Larger map, two cottages and a staffed hut, 96 berries | Finish a mill and lodge, house eight, have loggers plant four trees |
| 4. A place for everyone | Village square and shared supper | Larger dry map, four cottages, staffed hut/farm/bakery, 96 berries | Finish a square and complete supper with everyone housed |
| 5. More for the table | Vegetable garden and food choice | Larger dry map, four cottages, staffed hut, one farmer, 48 berries | Finish a garden, deliver 16 vegetables, and have vegetables plus another food available at two meal times |

Invitations are optional and are not required by campaign goals. Supper requires everyone housed, two loaves per current villager, and one reachable gathering tile per person near the square. That is 16 loaves for the original eight; choosing to grow increases the requirements.

Guidance is contextual and can be dismissed, disabled, or reopened. Meals do not erase delivery progress. Planting goals count completed planting work, not markers or tree maturity. Completion offers continued play, the next settlement, or replay. No deadlines, deaths, forced failure, or automatic transitions.

Vegetable deliveries and qualifying meal times are cumulative; carried or still-growing crops do not count. Level 5 needs one garden and no bakery, visitor trade, happiness threshold, invitation, or supper. Scripted normal-speed runs finish the current lessons in roughly two to five minutes; human play can take longer. Further narrative and optional objectives remain TBD.

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

**First chunk:** author Across the river and the minimum objective support it actually needs. Test opening → crossing → prepared growth → supported neighborhood before producing the other maps. Geography should make the crossing useful naturally; objective wording should explain the settlement's purpose. Compare at least two plausible approaches (for example early bridging versus building a stronger starting food supply). Existing global staffing, log-only stockpiles and movement rules must support the intended solution; defer goals that require unimplemented local services or per-workplace assignments.

**Objective rules:** use visible milestones and a small number of simultaneous outcome conditions. A brief operational check after growth distinguishes a sustainable settlement from a one-time stockpile, but must not become the main source of level length. Show what counts, progress and why a condition is unmet. Completed construction milestones stay completed; operational conditions may recover when missed. Stage transitions and arrivals are player-triggered, explained in advance, and saved. Failure to meet a condition invites adjustment, not restart. No deaths, forced failure, surprise disasters or deadlines.

**Playtest acceptance:** record elapsed time alongside decisions, idle stretches, bottlenecks and recoveries. A player should need to respond to the consequences of their earlier layout or staffing choices, be able to explain those consequences, and recover from an imperfect plan. If placing everything at the start and fast-forwarding wins comfortably, revise the scenario. If most time is spent waiting for counters, remove padding. Validate both a competent route and a recoverable inefficient route; no required perfect build order. The finished village should visibly reward the effort. New buildings remain optional design candidates, and availability restrictions remain deferred.

## Optional follow-ups

These are remaining possibilities, not a second priority list. Items promoted into the ordered second phase above take precedence; the notes here describe possible extensions beyond those first chunks.

| Area | Ideas to revisit |
| --- | --- |
| F01 / F02 — Paths and woodland | More path styles, worker-built roads/costs, traffic feedback, replanting zones, a forester, tree species and growth/yield choices. |
| F03 / F22 — Work and construction presentation | Smoother pose transitions, tree falling, particles, character variation, material piles/scaffolding, smoother crop growth, and richer harvest motions. |
| F05 — Food choices | More crops or orchards, recipe variety, garden/grain balance, and meaningful food-variety effects with F14. |
| F06 — Population | Arrival journeys/timing, larger-population balancing and performance, population preferences, families, and more names/appearances. |
| F16 — Creative | Relocation, multi-object removal, resource setup controls, population preferences, and bush rearrangement if arranging villages calls for them. Keep normal economy saves separate. |
| F07 — Storage and hauling | Compact yard visuals for large reserves (Creative clearing can produce tall stacks), food and plank storage, resource filters, delivery priorities, capacities, broader logistics controls, and demolition/relocation. |
| F08 — Materials | More plank buildings, mixed-material recipes, adjustable stock targets, upgrades, and other materials. |
| F15 — Visitors | More encounters and rewards after playtesting the first offer; no seed inventory, repeat-trade economy, or production bonus in the first version. |
| F14 — Happiness | Playtest thresholds and break duration, richer reactions, and additional reasons only when they create useful decisions. |
| F09 — Village character | More palettes and cottage details, player-selected house colours, connected fence runs, decoration brush strokes, and richer ornamental planting. |
| F10 / F17 — Audio | More organic sounds, extra variations, mixing by zoom, tighter impact timing, more musical themes, and music transitions. |
| F12a / F12b — Maps and water | More authored geography, richer map edges and shores, wider bridges, islands, water animation, bridge variants, and demolition rules. |
| F12c / F12d — Landscaping | Player terrain shaping, area selection, clearing-time tuning, grass/earth painting, constructed ramps, retaining walls, and raise/lower/level tools. Decorative objects belong in the shared F09 palette. |
| F19 — Main menu | Title artwork, save-slot browsing, and keyboard/controller navigation. Music settings belong with F17. |
| F20 — Atmosphere | Day/night progression, weather, water highlights, atmospheric particles, and richer wind animation. |
| F21 — Management | Permanent workplace assignments, route overlays, resource filters, production/consumption history, rates, richer workplace diagnostics, and configurable alerts. |
| F21e — Watching | Fully clean screenshots, optional label hiding, slow camera orbit, and scenic camera bookmarks shared with F12e. |

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

Orchards, a neighborhood pantry/market, and a carpenter that produces visible building improvements are candidate additions described in the design review; they need a distinct player decision before being promoted. Fishing, trade, household routines, procedural maps, and hidden discoveries also remain possibilities without a commitment. Infinite terrain or purchased land is not required for map expansion.

Combat, multiplayer, a large technology tree, and a full life simulation are outside the current direction.
