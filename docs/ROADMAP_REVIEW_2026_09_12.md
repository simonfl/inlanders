# Roadmap review — September 12, 2026

Baseline: `e9e4f67`. This is a source/document review and a plan, not a new playtest or an implementation claim. Recent rendered comparisons and simulation reports are supporting evidence; human enjoyment remains unverified.

> Historical baseline: this sequence has been delivered. See [the current next chunks](NEXT_CHUNKS.md) for the updated assessment and order. Counts and unimplemented items below describe the baseline revision.

## Assessment

The prototype has enough systems to support a richer game. The priority is making the existing village understandable, attractive and worth improving, then using its resource/geography choices in authored scenarios. Repeatedly adding art details alone will not answer whether the campaign is enjoyable; repeatedly adding needs will make an already substantial interface harder to learn.

The roadmap's main problem was status and ordering: its top queue mostly listed completed work awaiting human review, while the next actionable art slice was buried in a growing preamble. It repeated the five completed goals chunks, described seven playable levels as a five-level campaign, and still called the implemented comfort prototype unimplemented. Some linked design documents contain deliberately historical prices and behavior. Those are useful history but poor entry points for choosing today's work.

The revised roadmap has one five-chunk queue, separate review questions, a shipped reference and an optional backlog. Player feedback can redirect it; lack of feedback does not require inventing another system or stopping all unrelated work.

## Current system audit

| Area | Evidence / present state | Recommendation |
| --- | --- | --- |
| Architecture | F23a and F23b1–3 changed homes, workshops, shelter and fields. Square, yard/storage, dock and bridge still have older presentation. | Finish a civic focal point and storage/ground coherence before another full family pass. |
| Work and life | Actual cargo, field contact, home visits, social pairs and paused poses are implemented. | Tie scenery to those actions. Do not add permanent food props or seats that imply unsupported behavior. |
| UI | F21h–l provide status, service/source inspection, goal evidence and tracking. | Review a player's route through these controls before adding alerts, dashboards or metrics. |
| Campaign | `Simulation/Campaign.cs` lists seven levels. River/lake have staged growth and operational proofs; lesson levels intentionally remain short. | Review decisions and recovery, then author the quarry scenario with existing buildings. Do not promise duration from scripted seconds. |
| Economy | Shared definitions cover 17 building kinds. Actual meals, local food/log/plank storage, finite stone and shared wildlife/fish sources exist. | Keep costs provisional but avoid speculative across-the-board rebalance. Test a particular tradeoff when a scenario exposes it. |
| Resident needs | `Homes.cs` assigns residents and actual rest; recreation and meals already operate. Carpenter comfort is implemented and optional. | No need to restart a needs system. Defer mandatory education, religion, thirst or fatigue. |
| Rendering | `Game.cs` still creates a yard prop for every stored log/plank. Earlier uncapped measurements changed the interpretation of frame timings. | Bound stock display geometry and measure its actual workload. Do not infer a general performance gain from old frame-sync comparisons. |
| Maps/landscaping | Irregular maps, water/crossings, raised land, clearing, decorations and four-way placement exist. | Use authored geography now. Larger terrain tools and population increases require a concrete scenario and representative performance checks. |
| Audio/menu | First versions are shipped, with polish options in the backlog. | Keep them visible as later polish rather than accidentally treating them as missing core systems. |

## Current building palette

Costs and capacities below come from `Simulation/Buildings.cs`, not the historical design review. All non-compact buildings occupy six tiles; bridge, dock and seating garden occupy one tile, with separate access/launch requirements. Construction also costs worker time and transport.

| Building | Cost | Role / payoff |
| --- | --- | --- |
| Cottage | 6 logs | 2 beds |
| Lodge | 12 planks | 4 beds; efficient use of housing land |
| Forager hut | 4 logs | 2 forager slots; equipment base, not local berry storage |
| Farm | 4 logs | 1 farmer slot; grain for baking |
| Vegetable garden | 4 logs | 1 farmer slot; directly edible produce |
| Bakery | 8 logs | 1 baker slot; grain into bread |
| Sawmill | 6 logs | 1 sawyer slot; logs into planks |
| Stockpile | 4 logs | Local logs or planks; optional hauling |
| Neighborhood pantry | 6 logs | Local edible food service; optional hauling |
| Village square | 6 logs | 4 recreation visitors; shared supper |
| Seating garden | 4 logs | 2 recreation visitors in a small plot |
| Gathering hall | 8 planks + 12 stone | 8 recreation visitors, longer/less frequent visits |
| Bridge | 6 logs | Reachable crossing |
| Fishing dock | 8 logs | 1 fisher/boat; water habitat into food |
| Quarry camp | 6 logs | 1 quarrier; finite stone |
| Hunting lodge | 6 logs | 1 hunter; mature woodland habitat into game |
| Carpenter workshop | 6 logs | 1 carpenter; optional occupied-home improvements |

There is already useful variety. Hall comparisons show fewer repeated journeys, not higher food output; comfort comparisons show fewer rest trips, not a demonstrated economic return. Their campaign purpose must be honest. A civic project can be a chosen goal without claiming it is always the optimal producer investment.

## The next five chunks

### 1. F23b4 — the square as a gathering place

**Player outcome:** identify the village's shared center and understand where residents actually gather.

Replace the generic platform/table treatment with restrained ground, substantial shared furniture and a clear open frontage. First inspect `Leisure.cs`: visitors reserve reachable, unblocked cells within two tiles of the entrance, outside the blocked building footprint. Supper uses its own gathering requirements. Design around those actual positions. A bench inside the footprint must not imply residents can sit there; a new fixed-seat system is outside this slice.

Keep six-log cost, four recurring visitors, supper rules and navigation. Update only misleading placement/inspection wording exposed by the new model. Review empty, occupied and supper scenes at 960/1440 and all four building orientations, plus construction stages. Verify actual participants, pause/reload and approach clearance. If it takes a seating-system rewrite to make the proposed furniture honest, simplify the furniture.

### 2. F11b3 / F18b3 — campaign decision and clarity review

**Player outcome:** understand a failing condition, take a useful action, and see recovery without arbitrary waiting.

Review one river and one lake run through the current interface. Use a small scorecard: first meaningful choice, confusing condition, action taken, visible consequence, idle interval, and recovery. Include a compact or competent route and one imperfect layout/food-service situation using existing fixtures. Separate wall time, simulation time and speed settings. A scripted route is a diagnostic, not a substitute for a first-time human player.

Rank findings by player impact. Implement at most the highest-supported correction in this chunk: e.g. wording/action placement, one confusing transition or evidenced waiting. The exact change stays TBD until evidence exists; do not preemptively change visit windows, costs or quotas. If no supported defect emerges, deliver the review and retain the human questions. If a correction touches goals, verify alternative paths, cumulative versus rolling progress, exact saves and narrow-screen navigation. Larger systemic changes become separately scoped entries.

### 3. F23b5 — compact storage and calmer surroundings

**Player outcome:** read empty/working/stocked storage while the village remains visible at large reserves.

Cap or tier the yard's visible log/plank stacks; exact inventory remains authoritative in the HUD/inspector. Keep a genuinely empty display and avoid implying that a capped display is an exact physical count. Give the local stockpile a coherent bay and obvious approach. Reduce excessive path/yard-ground contrast in the same matched scene; avoid a global lighting rewrite.

Exercise empty, ordinary, full local capacity and unusually high Creative reserves. Confirm quantities/reservations never change, collection remains visible, geometry stays bounded and repeated updates do not leak nodes. Capture village-scale readability and measure the target stock workload with frame-sync settings recorded. Do not cap actual storage, introduce local stone storage or retune hauling in this art/presentation chunk.

### 4. F26b2a — Built to last, concrete scenario brief

**Player outcome:** choose how to fund and supply a shared civic project while keeping a working village served.

Draft an authored map with a small nearby stone source and a larger distant one, a functioning starter village and competing food/housing/civic land. Account for the exact initial logs, planks, food, workers, accessible stone and target construction costs. The hall needs twelve stone: do not accidentally write two supposedly distinct approaches that both exhaust the same compulsory deposit. Decide whether nearby stone can finish one hall or whether expansion is the distinct second decision; document the choice.

Write three or fewer phases: secure production/access, commit to the civic project, demonstrate actual use and continued food service. Describe two viable approaches and a recoverable inefficient one. Carry relevant findings from chunk 2 into explanations and proof length. Population, reserves and thresholds stay provisional until budgets and routes support them. No new resource chain, compulsory comfort or invented minimum duration.

Deliver the brief, map sketch/coordinates, resource budget and acceptance routes. Self-review the design for choice, overlap with river/lake, payoff and scope. If the hall goal merely means waiting for stone deliveries with no meaningful allocation decision, revise the map or defer the scenario.

### 5. F26b2b — one playable quarry settlement

**Player outcome:** finish a distinct settlement that visibly uses its civic project.

Implement the viable brief as one campaign scenario, using existing quarry, sawmill, hall, food and home systems. Author starting state, concise guidance, staged player-triggered actions and objective evidence. Keep all buildings available. Count real attendance and continuing support; building ownership alone is insufficient, and the hall need not be universally superior to cheap squares.

Verify both budgeted routes, recovery after a poor placement or allocation, depleted-deposit feedback, saved phase transitions and ordinary completion/replay. Exercise the goal UI at 960/1440. Report measured simulated routes separately from human pacing; do not call the duration target validated. If this requires new economy rules, rescope instead of burying them in scenario work.

## What moves later, merges, or gets cut

- **Merge duplicate review reminders** into one review register; retain linked measurements without repeating their logs in the queue.
- **Keep F21l complete.** New UI follows observed player problems; it is not another automatic five-dashboard batch.
- **Keep comfort optional.** A required comfort campaign waits for a worthwhile payoff; do not lengthen a level with upgrades alone.
- **Use existing environments first.** Living woods follows a successful quarry scenario; long haul/limited land become scenario ingredients or optional challenges. They are not all committed standalone levels.
- **Defer new needs and chains.** Education, reflection, orchards, pasture, river mill and thirst remain candidates with explicit purpose still to establish.
- **Defer terrain/population expansion.** Authored geography is sufficient for this queue; profile before raising scale.
- **Preserve exclusions:** no seasons, save migration project, compulsory per-level unlocks, combat or multiplayer.

After each chunk, record what changed, what was learned, unresolved player questions and the next recommendation. Reorder the queue if evidence supports it; do not treat this document as five unconditional implementation promises.
