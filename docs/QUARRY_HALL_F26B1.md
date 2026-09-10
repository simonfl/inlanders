# Stone and a place to gather — F26b1 / F25c

Implemented prototype, with balance and aesthetic acceptance still open. This supplies a resource-to-service loop in free play; it does not add the Built to last campaign scenario.

## What to play

Start a fresh **Three clearings** settlement, normal or Creative. Two marked outcrops contain 16 and 48 stone. The small western deposit is relatively convenient; the eastern deposit offers more supply but a longer route. Campaign maps and the original clearing do not gain stone automatically.

- A **quarry camp costs 6 logs** and provides one quarrier slot. Its footprint and entrance need level ground, with a reachable unexhausted outcrop within four tiles of the camp. Outcrops and their working access remain clear of construction and planting. The nearby slopes/bushes constrain useful camp sites.
- A quarrier reserves up to **2 stone**, walks to the outcrop, works for **6 work seconds**, and carries the real load to the central store. Nearby camps share remaining deposits. Extraction shrinks the visible rock; exhausted outcrops remain as low rock, without regrowth or excavation tools. Pause and stock targets use existing workplace controls. Local stockpiles still support logs/planks only.
- A **gathering hall costs 8 planks + 12 stone**. Builders reserve and deliver the materials separately; construction starts only when both requirements are met. Cancellation leaves recoverable material piles. Normal demolition physically returns both materials. Creative buildings remain free and generate no construction materials on removal.
- The hall serves **8 visitors** in a six-tile footprint. A completed **12-work-second visit** supplies the existing recreation benefit for **240 simulation seconds**, with a **120-second interval** before another outing. A square remains 6 logs, 4 visitors, a 6-second visit, 120-second benefit and 60-second interval. Both need nearby reachable visit spots; capacity does not promise access.

People visibly gather around the hall's open entrance. Its masonry piers, arch and blue-green roof distinguish it from the timber square. This first version uses outdoor visit spots and existing social poses; the interior seats are architectural detail, not claimed seating or simulated indoor attendance. Quarry work uses hammer strikes; stock and carried stone are visible. The central stone display caps at twelve pieces, while the HUD/Economy show exact inventory.

The hall does not add a new satisfaction meter or research requirement. Completed visits retain their earned benefit after a building is removed; unfinished visits earn nothing. Save format 29 records deposits, stone, construction claims and the earned recreation window. No old-save migration.

## Why the hall changed during the experiment

Extra capacity alone did not justify its cost: one square already completed the same number of visits as the initial hall over five minutes, with both eight and sixteen residents. The adopted prototype instead offers longer, less frequent visits. This tests a service improvement with an actual travel consequence rather than a universal productivity bonus.

Matched 300-second Creative fixtures compare the same housing/population and venue location. Working fixtures use four remote vegetable gardens on an authored flat, connected version of the large map, with residents assigned farming. Creative removes hunger penalties so the comparison isolates routines and travel; these are not campaign playtests or fully optimized staffing plans.

| Working fixture | Hall | One square | Two squares |
| --- | --- | --- | --- |
| 8 residents: visit travel, tiles | 170.7 | 277.0 | 253.0 |
| 8 residents: recreation coverage, person-time | 87.5% | 87.8% | 89.5% |
| 8 residents: vegetables delivered | 52 | 54 | 54 |
| 16 residents: visit travel, tiles | 354.7 | 435.0 | 477.0 |
| 16 residents: recreation coverage, person-time | 85.2% | 84.0% | 87.9% |
| 16 residents: vegetables delivered | 54 | 54 | 54 |
| Venue footprint, tiles | 6 | 6 | 12 |

The hall reduces repeat journeys in working villages, but longer attendance offsets some of that time and food output does not improve in these fixtures. Idle eight-person villages see no travel benefit. Cheap squares remain a sensible opening; distributed squares can offer higher early coverage. Costs are provisional: the result establishes a distinct service option, not universal superiority or a measured construction payback time. Review human use before writing goals that depend on buying a hall.

For stone, the same workers deliver twelve units including camp construction in **123.4 simulation seconds** from the western camp versus **325.7 seconds** from the eastern camp. The latter travels around the river; bridge/path investment is a later player option. A minimal quarry/mill/hall fixture completes the hall at 142 seconds. These are supply experiments, not desired level durations or a finished-village solution.

## Verification and next decisions

`./Test.ps1` includes quarry placement, shared finite claims and an odd last unit, normal extraction/hauling, stock targets, pause/interruption, exact continuation, corrupt inventory rejection, both construction materials, cancellation salvage, demolition, actual hall attendance and benefit expiry across save/load and removal. Existing campaign, bridge, food, storage, home and production tests still pass. Generic Creative/demolition fixtures now supply environmental prerequisites and verify mixed recovery.

`./Play.ps1 -QuarrySmokeTest` checks outcrop/cargo/depletion rendering, pause/reload, an actually used hall, mixed costs, workplace controls, inventory and rejected placement away from stone. Local 960/1440 captures under `artifacts/f26b1-*` were inspected, including the village camera and narrow inspector. Art acceptance remains a player judgment.

Next, improve service coverage feedback (F21i): the player should see which residents miss rest/recreation and follow their destination/reason, rather than infer usefulness from a higher building capacity. F26b2's authored scenario remains after river/lake human pacing feedback. F26c wildlife is a separate later resource experiment. More stone consumers, local stone stockpiles, indoor seats and stronger visual variation remain candidates; do not add them as prerequisites for trying this loop.
