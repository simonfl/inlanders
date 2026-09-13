# F29c — Willow inlet

September 13, 2026. One playable outcome, checkpoint **20**. Regular whole-game review is due before the next implementation chunk.

## What is playable

Settlements → **New Willow inlet** opens an inherited eight-person village: four occupied cottages west of an inlet, three working vegetable gardens east, a western seating garden and an existing southern footway. Eight newcomers can be invited immediately, arriving after 90 simulation seconds. A bridge is an optional shortcut. Homes and the welcome venue can be on either shore. Keep the footway, bring food closer, settle near the gardens, or make another arrangement with the unrestricted catalogue.

The situation changes what the player starts with, not crop yields or construction prices. It reuses river/lake geography, a small fish habitat and current shared-work/local-food rules. The original meadow remains available as a comparison control. Its reserve gate is unchanged.

Goals links directly to the existing food/world view. Automatic labor no longer generates legacy role-allocation recommendations from instantaneous jobs. **Keep building** leaves controls active; **Watch village life** is a separate choice. New scenario identity survives menu/Continue, restart and manual save/load. Current saves use version **44**; no migrations.

## Ending and its limits

Complete the welcome with eight housed newcomers and sixteen participants who have eaten at the chosen place. For this situation, residents must currently be fed and able to reach an open producer with an available edible portion. There is no 32-portion stockpile target, delivery percentage or rolling assessment window. Pausing all food production cannot win on inherited welcome stock alone.

This recognizes a completed welcome with current food access, **not permanently sustainable supply**. The east-home comparison below subsequently misses meals. That is material counterevidence against treating the ending as an economy certificate. Continued play remains available; review must challenge whether the finite endpoint is satisfying and sufficiently honest about future village life.

## Ordinary-construction comparison

`--inherited` in the simulation test executable runs five branches from the same fresh serialized inlet. `--inherited-recovery` then repairs the saved weak east-home layout. Actual legal cells/facings and current saves are in `artifacts/inherited`. Each arm constructs four newcomer cottages, selects the inherited western venue and invites people; bridge/local-food arms add their intervention first. The no-change control means **no food/access intervention**, not no new housing.

| Arrangement | Welcome complete, sim s | Food-related walking in following 600s, person-s | Hungry resident-time in that 600s | Final food |
| --- | ---: | ---: | ---: | ---: |
| Keep footway, western homes | 422.6 | 1973.9 | 0 | 13 |
| Bridge, western homes | 422.1 | 1801.3 | 0 | 26 |
| Add western garden, western homes | 422.3 | 1836.1 | 0 | 50 |
| Eastern homes, western meeting place | 516.2 | 1087.1 | 612.4 | 3 |
| Food shutdown, then resume | 1042.5 | 1142.7 | 82.3 | See metric extract |

The bridge and local-food alternatives reduce observed food walking by about 9% and 7% respectively; completion times are effectively equal for the first three plans. They have not established greater challenge or enjoyment. Eastern housing is not automatically better: its lower food walking accompanies missed meals, so it cannot be called an efficiency win. Adding a western garden to that saved layout, allowing 300s for its first harvest/routines, then observing another 600s produces **zero hungry resident-time**, with nine portions left. That recovery uses real construction and output, not edited inventories.

These are informed scripted plans with fixed construction order and nearest-valid placement. Different completion times mean the equal 600s post-completion windows occur at different village ages. Food walking is a selected task measure, not total travel or productivity. We have not isolated the precise cause of the east-home shortfall; the western venue and work/rest trips are plausible contributors, not proven attribution.

An earlier local probe retained the opening's western forager and weakened the intended food geography. The delivered start removes that inherited producer; players may still build one. No branch is made illegal to protect a prescribed intervention. The single western berry source remains a freely usable alternative.

Compared with the old meadow, this is not a controlled difficulty A/B: both begin with eight residents, invite eight and have 24 central berries/one patch; the inlet additionally inherits three gardens with 24 local vegetables, a venue, 12 loose logs and an existing footway. Construction investment and completion criteria differ deliberately. The [observer sheet](INLET_OBSERVATION.md) compares experiences rather than asserting matched completion performance.

## Validation and evidence

- Five complete construction/arrival routes, world validation, exact saves and deterministic saved continuation.
- Deliberately paused producers prevent completion; resuming recovers. Weak-layout recovery measured separately.
- Existing `--welcome-meal` suite passes, including original four arrivals and all six edible pantry cases.
- Rendered 960 inlet journey: `20260913-161612-912-neighborhood-inherited-b1dce5` (79.86s wall). Full entry/reset/resume, food view, shortcut/west homes/west venue, production shutdown/recovery, completion, separate build/watch and reload pass. This precedes a text-shortening edit.
- Rendered 1440 inlet journey: `20260913-162237-372-neighborhood-inherited-97fc18` (90.75s wall); final gameplay/UI source, same checks pass.
- Original neighborhood 960: `20260913-162237-519-neighborhood-journey-21b154` (117.43s wall); original scenario and changed build/watch controls pass.

Runs are under `artifacts/review/runs`, with semantic indexes, snapshots and provenance. They were collected before the feature commit and retain dirty/precommit metadata. Production waits use direct simulation ticks; inputs are developer-selected, not uncoached native play. Both projects build without warnings. Some earlier build attempts failed because a running test held its assembly; retry after completion passed. This is a launcher/process-ordering issue, not a simulation regression or a measured game stall.

No human preference, continuous animation judgment or audio listening is established. Default presentation still uses the board/labels; the optional Storybook study is not promoted by this chunk. The checkpoint-20 reviewers must examine the whole project, including whether this is a more meaningful second situation or merely another preparation recipe.
