# The broken crossing — recover an existing village

November 9, 2026. The owner liked clear progression and helping villagers obtain food, but judged A lean landing too similar to A first home. Changing reserves and layout did not provide enough variation. This experiment changes the action required and the existing village state, not merely the order of the same housing objectives.

## Situation and difference

Four named residents already occupy two completed cottages. A broad channel separates them from their existing field with twenty ripe portions. They have four logs and eight food portions. No arrival or new-home objective exists.

- Repair the six-log crossing: mark a little south-bank timber, reopen the fixed crossing site and carry the saved harvest back.
- Plant a four-log garden nearby: start with the existing logs, wait for new crops and feed the village without repairing the crossing.

An independent design check selected this minimum contrast over multiple artificial crossings or special bridge costs. The village uses its surviving central store in this scenario, so production must travel back before delivery counts. This reuses existing central-delivery behavior; other household stories retain local workplace stores. Starting berries cannot satisfy fresh-meal progress.

The objectives are food access, actual fresh delivery, and 0/4 through 4/4 neighbors eating fresh food. The crossing and stranded crop are labeled in the world. Two costed recovery actions explain the material/growth tradeoff. Repair directly orders the known crossing; it does not ask the player to hunt for a valid water tile. Inspection and cancellation remain available by selecting the bridge. Hunger slows work and remains recoverable.

## Mechanical evidence

The `--crossing-recovery` suite checks the housed opening, inaccessible ripe crop, no-action shortage, pure crossing consequences, cancelled-plan recovery, two distinct successful routes, physical vegetable cargo crossing the bridge, no new cottages, and exact save continuation. Existing first-home and lean-landing checks also pass.

| Route | Crossing accessible | First food at store | Completion | Peak hungry |
| --- | ---: | ---: | ---: | ---: |
| Prompt repair | 37s | 65s | 134s | 0/4 |
| Prompt garden | Remains cut off | 92s | 163s | 0/4 |
| Wait 240s, then repair | 314s | 363s | 385s | 4/4 |

These are simulated seconds with authored placements/orders. Printed completion includes five seconds of exact continuation testing. Repair uses no new food workplace; the garden route leaves all twenty old-field portions untouched and builds no bridge. Cargo was observed on the actual crossing in simulation, not inferred from completion text.

## Native/UI evidence and limits

An independent evaluator inspected 960px opening, costed choices, repair order, repair completion and garden completion captures on DLL `5288A933BA1E4B986D1FF0849E9A2EF5AC2FB53D9C49038DD271684E79CDF6A3`, then matched source behavior. It found the changed opening and outcomes legible, and supported removing the automatic bridge inspector and specifying the two-log shortage. These focused revisions are included in the final handoff.

Native scripted button signals open recovery choices and order the fixed crossing; assertions check the resulting bridge and timber guidance. The existing capture harness gained a narrow RecoveryPlans option and a recovery framing/fixture entry. This supplies repeatable control-state evidence despite the known hanging desktop window tools. It is not independent ordinary mouse play, motion inspection, listening, or enjoyment evidence.

Initial evidence directories in artifacts/review/runs: 20261109-151643-821-crossing-recovery-bd97f7 (opening/choices/order), 20261109-151703-978-crossing-recovery-0e951d (repair finish), 20261109-151712-232-crossing-recovery-6b6659 (garden finish).

## Decision

Deliver one bounded recovery comparison and stop for owner play. The new activity is diagnosing lost access, repairing it and bringing back existing crops. The alternative garden route necessarily retains building/growing; do not claim both branches are wholly new gameplay loops. This remains short, and the successful scripts do not establish that the repair tradeoff is difficult or that the player notices its payoff. Ask whether recovering something their villagers already depended on feels meaningfully different and satisfying before adding further scenarios.

Final handoff: zero-warning DLL 8A2EAF9B8AE66A485B3DDEF32577C76E84E03857B01AD89D2D74514362288374. Final native opening/plan/order capture: artifacts/review/runs/20261109-152113-926-crossing-recovery-b191a1. The evaluator inspected the revised repair view and accepts for owner play: the exact two-log shortage and single action remain prominent, with the bridge inspector available only on manual selection. All three recovery routes were rerun with unchanged reported timings, physical cargo crossing and exact saves. This is not a claim of independently observed mouse play or enjoyment.