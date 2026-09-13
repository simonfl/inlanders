# Feature roadmap

A living plan for a peaceful settlement that is satisfying to arrange, manage, and watch. Pick one bounded chunk at a time and revise the order after seeing the result.

Windows, local play, Godot and C# remain the foundation. Save migration is not required during prototyping; fresh saves must roundtrip. Seasons remain removed. All buildings remain available in every campaign level. Feature IDs stay stable.

## Current direction

**Next: immediate critical design review at checkpoint 8. Routine feature delivery is paused.** The user has empowered reconsidering the entire game and substantial redesign. Follow the [design review mandate](DESIGN_REVIEW_MANDATE.md): judge the core experience, challenge existing systems and visual direction, compare alternatives, then replace this queue with a coherent direction. The existing roadmap records prior decisions; it is not a commitment to preserve them.

**Environment is part of the building design.** Each new producer ships with readable placement prerequisites, a landscape where it is useful, a visible output/use and an alternative viable plan. Not every map needs every resource. Start with geometry, access and shared habitat/deposit stocks; fertility, drinking water and deeper resource chains remain TBD. Campaign success measures residents supported and projects used, rather than ownership of every new building.

**Future building priorities:** use the existing fishery, quarry/hall and wildlife prototypes in contrasting campaign landscapes first. Small recreation gardens (F25c) are now implemented. Review the implemented **neighborhood food service (F07c)** through actual meal journeys; **carpenter/home improvements (F25b)** now have a working comparison and remain optional. No food-output gain was demonstrated; final family art still follows the visual-direction review. Orchards shipped in F05c; pasture/herder shelters and a river mill remain later alternatives; education and chapel/reflection projects remain exploratory. The [building-and-place briefs](CAMPAIGN_SYSTEMS.md#choose-the-next-buildings-through-contrasting-places) now give each candidate a terrain or neighborhood interaction, a campaign situation, competing approaches and recovery. They are options for future chunks, not a commitment to ship every building.

**Resident progression:** food → shelter and rest → recreation → comfort and chosen civic ambitions. Food, home rest and recreation already exist; improve their visible use and player feedback rather than restart a needs system. Sleep schedules, thirst, formal education and a separate spiritual need stay TBD. A religious or secular landmark can initially use recreation/ceremony rules. Longer levels should start from imperfect working villages and introduce staged land/labor decisions; avoid adding duration through quotas or requiring every food and institution.

- **Food, shelter/rest and company now have a first shared loop.** F14b measures actual meals; F25a adds assigned homes, real rest visits and recreation explanations using cottages/lodges and squares. See [the home-life review](HOME_LIFE_F25A.md). Comfort can later motivate visible home improvements.
- **F26a, implemented: water → fishing dock/boat → food.** Shared fish habitat, shoreline access and complete water/pantry trips distinguish fishing from gardens. Life by the lake introduces the system; F11b2 now adds constrained sites and meaningful service-placement choices.
- **F26b1, implemented: rock outcrop → quarry → gathering hall.** Finite stone and mixed construction/recovery now work. Hall visits trade a larger investment and longer attendance for fewer repeat journeys. F26b2 is implemented as level eight; no iron/tool-wear chain.
- **F26c, prototype implemented: retained woodland → wildlife habitat → hunting lodge/game.** Shared stock and recovery depend on mature trees. Pause hunting for stock recovery; regrow woodland for capacity. Living woods is implemented as level nine with both recovery paths; no full ecosystem simulation.
- **F25b — Comfort at home, prototype/comparison complete:** [F25b2 results](HOME_COMFORT_COMPARISON.md) show reduced homeward travel, ordinary-home viability, and no food-output gain over twenty minutes. Full and partially occupied lodges were compared with cottages, relocation and food investment. Keep costs/intervals provisional and scenario adoption behind F25b3 player review.
- **F25c — Recreation alternatives, implemented:** one-tile seating gardens, squares and halls offer different footprint/capacity/investment choices with actual visits. Compare their usefulness in normal play before adding more venues.
- **F25d — Learning/shared projects, exploratory:** the attendance gate was rejected and F25d2 found no distinct staged payoff from restoring a crossing. Do not integrate F25d3 or add a workshop requirement. A future productive restoration needs a new functional brief; no school-age simulation or generic research tree.
- **F25e — Reflection and village identity, implemented:** Hall/Chapel/Planted court choices and actual quiet visits share hall recreation. No separate need, compulsory religion or requirement to own every civic building. Further ceremonies remain exploratory.

**Building shortlist and design gates:** the gathering hall is now the first stone consumer. The campaign systems plan retains carpenter-funded home improvements, the implemented small recreation gardens, and later learning/reflection venues. Choose between candidates through a playable purpose, not a larger construction menu. F25a now explains actual meals, home/rest and recreation participation; F26 must show habitat/deposit quality and access before investment. Balance new producers against construction, labor, transport and land use on contrasting maps. Costs and detailed need schedules remain TBD; later needs can be merged or cut if they duplicate recreation.

**Next campaign-system review:** [F26b1 findings](QUARRY_HALL_F26B1.md) show a real difference between nearby/distant stone and a conditional travel benefit from halls. F21i service coverage now exposes current reasons and destination links; review whether those explanations help players improve a dispersed village. F26b2 has shipped; its short competent routes leave human pacing and deeper challenge open. F26c now tests retained mature woodland against hunting and clearing, with separate recoverable consequences. F21j brings shared-source feedback into direct map inspection; see the [scenario briefs](CAMPAIGN_SYSTEMS.md#next-design-decisions-make-each-addition-earn-its-place).

The expanded candidate palette includes a river-powered mill, pasture/herder shelter, a civic well, a learning workshop/restoration project, and chapel/reflection-garden alternatives. These are exploratory, not new commitments or prerequisites: each needs a distinct land, labor or resident-service choice. No automatic thirst meter, mandatory flour chain, school-age population or separate religion gauge. Food, actual home rest and recreation remain the core; comfort and chosen civic ambitions provide later progression.

**Scope of the next needs pass:** food, home/rest and recreation are the core; comfort is an improvement, with learning and reflection optional later ambitions. The [systems plan](CAMPAIGN_SYSTEMS.md#later-needs-small-independently-reviewable-chunks) gives F25b–e individual experiments and cut criteria. Follow each resource all the way to its resident benefit: fish/game into actual meals, stone into an attended civic place, planks into occupied home improvements. New foods must not make every existing diet inadequate. Campaign difficulty comes from competing land, access and worker time, with visible recovery options—not accumulating mandatory needs or construction checklists.

Keep Across the river as the first longer level using existing systems. Life by the lake is now implemented as a resource introduction. Built to last is level eight and The living woods is level nine. A lasting village is implemented as level ten. **A place to call home** remains an optional campaign concept. The long haul and Room to grow can become challenges within these maps or optional scenarios, rather than padding the sequence with overlapping levels. The optional concepts remain planned, not implemented. All-building availability and the removal of seasons remain unchanged.

## Optional follow-ups

These are remaining possibilities, not a second priority list. Items promoted into the current delivery queue take precedence; the notes here describe possible extensions beyond those first chunks.

| Area | Ideas to revisit |
| --- | --- |
| F01 / F02 — Paths and woodland | More path styles, worker-built roads/costs, traffic feedback, larger grove brushes, a forester, tree species and growth/yield choices. |
| F03 / F22 — Work and construction presentation | Smoother interruption/stance transitions, character variation, material piles/scaffolding and smoother crop growth. Logging falls and distinct field harvesting have shipped; judge their readability in ordinary village play before adding particles or more motions. |
| F05 — Food choices | Orchard comparison and playable integration delivered in F05b/c. Orchard campaign use remains behind feedback; recipe variety, garden/grain balance and further diet choices remain candidates. |
| F06 — Population | Arrival journeys/timing, larger-population balancing and performance, population preferences, families, and more names/appearances. |
| F16 — Creative | Relocation, area removal and central resource setup delivered in F16c/d/e; bush movement delivered in F16f; population preferences remain a candidate. Keep normal economy saves separate. |
| F07 — Storage and hauling | Yard visuals shipped in F23b5; loose-source stacks are F23c1. Review of neighborhood food service (F07c3), resource filters, delivery priorities, capacities, broader logistics controls, and relocation. Normal-play demolition shipped in F16b; retain its physical goods recovery. |
| F08 — Materials | More plank/stone consumers and upgrades. Mixed plank/stone construction shipped with the hall; local stone storage is delivered in F07d. Adjustable workplace stock targets shipped in F21h. |
| F15 — Visitors | More encounters and rewards after playtesting the first offer; no seed inventory, repeat-trade economy, or production bonus in the first version. |
| F14 — Happiness | Playtest thresholds and break duration, richer reactions, and additional reasons only when they create useful decisions. |
| F25 — Home routines | Playtest rest duration and commute cost; consider household swaps and grouping advanced home controls if needed. Keep comfort, learning and reflection behind a distinct resident benefit. No synchronized nightly sleep or fatigue penalty yet. |
| F09 — Village character | Cottage finishes, connected fences and decoration brush strokes shipped in F09b/c/d. Gateways shipped in F09e. More cottage details and richer ornamental planting remain candidates. |
| F10 / F17 — Audio | Spatial work sounds, three musical candidates and quiet transitions are implemented. Listening acceptance, supported mix/variation changes and tighter impact timing remain open. |
| F12a / F12b — Maps and water | More authored geography, richer map edges and shores (including softer bright rims/water marks noted in F23b8), wider bridges, islands, water animation, and bridge variants. Normal-play demolition and protected access shipped in F16b. |
| F12c / F12d — Landscaping | Creative rectangular terrace shaping and session Undo shipped in F12h; broader sculpting, normal-play costs, clearing-time tuning, grass/earth painting, ramps and retaining walls remain candidates. Decorative objects belong in the shared F09 palette. |
| F19 — Main menu | Keyboard navigation shipped in F19c and composed title artwork in F19d. Save-slot browsing and controller navigation remain separate follow-ups. Music settings belong with F17. |
| F20 — Atmosphere | Day/night progression, weather, water highlights, atmospheric particles, and richer wind animation. |
| F21 — Management | Construction-catalog keyboard focus shipped in F21o; People/Economy/Goals/survey keyboard navigation and named workplace assignments are delivered. Supply-route emphasis, resource filters, longer productivity history and configurable alerts remain candidates. Basic workplace diagnostics and recent food rates shipped in F21h; assess their clarity before adding more metrics. Add catalog search only if categories stop being sufficient. |
| F21e — Watching | Clean view, optional slow orbit and drag-to-pan have shipped. Review their use before adding camera paths, direction/speed settings or more viewing controls. Keep manual takeover immediate. |

The original **milestone 4 — “Make it enjoyable to watch”** spans F03, F10, F17, F20, F21, and F22. All now have a first pass. **F04 is the separate gathering-places feature.**

The accepted [periodic reviewer policy](REVIEW_CADENCE_PROPOSAL.md) runs game design, UX/onboarding, playtest and development-lead reviews every five playable checkpoints, adding visual/audio review every ten or after substantial presentation changes. Track completed outcomes and the next review in [the checkpoint ledger](CHECKPOINTS.md).

## How we take a chunk

1. Pick an entry from the ordered list; mark it **In progress**.
2. Write the player experience, smallest useful version, and a short playable check. For substantial features, get a short game-designer review before implementation: meaningful choice, fit, overlap, scope, and playable checks. Use UX or simulation reviewers when the change calls for them. Leave unresolved choices TBD.
3. Implement it with current-format saves and relevant verification.
4. Record what shipped in the completed table and move remaining ideas to follow-ups.
5. Reevaluate the roadmap after every chunk using what implementation and play revealed. Reorder, clarify, combine, cut, expand, or add features when that improves the game; the current list is not a fixed commitment. Keep feature IDs stable, UI prominent, and uncertain details TBD. Do not restore explicitly removed features without a new reason and user agreement.
6. Update the next recommendation and briefly explain meaningful scope or priority changes. Commit and push the chunk with its reviewed roadmap.

Keep supporting UI inside the feature that needs it. Avoid turning this roadmap into a test log or a list of prototype cleanup tasks.

## Parking lot

Orchards shipped in F05c; hidden discoveries remain candidates. Neighborhood pantries have shipped in F07c2; a market needs a distinct purpose before expansion. Carpenter/home improvements and limited rest visits are described in F25. Fishing, stone and woodland wildlife have moved into the F26 campaign plan. Trade, broader household routines and procedural maps remain possibilities without a commitment. Infinite terrain or purchased land is not required for map expansion.

Combat, multiplayer, a large technology tree, and a full life simulation are outside the current direction.
