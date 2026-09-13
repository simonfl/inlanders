# F27c1 — food and land situation

September 13, 2026, delivery commit `8e2b2c3`. Bounded playable campaign situation, delivered as checkpoint ten. The chosen production/distribution rules remain unchanged. All buildings are available; this does not roll the rules across the old campaign.

## Situation under test

A housed eight-person hamlet has one visible western berry patch, 24 initial berries and eastern building space. The patch uses ordinary regrowth and production. One commitment brings eight people after 90 seconds, so the player must plan for sixteen rather than repeatedly invite pairs. Compact landing space and the larger meadow retain actual construction, wood, food, rest and journey costs. The original four-arrival introduction remains unchanged.

Test wild-food-only, prepared vegetable gardens, prepared local grain/bakeries and an early-arrival garden route. Record actual placements/facings and both completion and subsequent meal service. These are whole-situation plans with intentionally different construction orders, not a controlled attribution of every outcome to one parameter. Native/uncoached play and human preference remain unobserved.

## First falsification

The wild-food-only plan completes the unchanged welcome at 555.0 seconds, then has only 57.9% fed resident-time and two stored portions in the final 300-second observation. Reject it as a successful campaign design. More arrivals and scarce food do not fix an ending that precedes its consequences. The prepared gardens plan completes at 921.0 seconds and sustains everyone, but its feasibility does not rescue the false-positive ending. Prepared bread completes at 999.9 seconds with 10.3 total hunger seconds and sustained final service; early gardens complete at 817.8 seconds without hunger. Early commitment can be a useful labor choice; it must not be described as always causing a shortage.

The full first comparison completed in `artifacts/food-land-comparison` with source hashes, saved states and final service measures. No runtime yields, consumption intervals or hunger penalties changed.

## Next objective design

Test a tangible provisioning outcome alongside settlement: all arrivals housed, the welcome shared, and enough stored edible food for two meals per resident. This uses actual distributed stock, visible in stores and the food totals; no timed stability certification, prescribed producer, dominant-food ratio or hidden recent-history gate. It is an explicit finite preparation task, not a claim the economy remains sustainable forever.

The reason to test this is to distinguish the initial welcome from preparing the new settlement to carry on, while retaining different food/land/labor solutions. A higher number by itself is not depth: reject or redesign if this is only a stockpile wait, if the wild-food shortcut still wins before collapsing, or if every cultivated layout succeeds without a meaningful commitment/recovery choice. Compare timing and hunger for prepared versus early commitments; capture the player's actual menu/placement/goal/recovery route before delivery. The checkpoint-ten whole-project review must challenge whether this is a stronger situation rather than accepting the counter as a sufficient design improvement.

## Provisioning revision under verification

The completion condition now additionally checks 32 actual stored edible portions, after all sixteen residents have shared the welcome and the eight arrivals have eastern homes. The food remains available for normal meals; nothing is silently deducted and no counter is reset if work takes longer. The ordinary four-arrival introduction retains its ending. The new menu entry is separate, with one-patch/eight-arrival/reserve guidance and current physical stock shown explicitly.

The revised wild-only route no longer completes. Prepared gardens and bread retain their original completion times and sustain final service: the new objective does not tack a wait onto those sound plans. Full revised comparison, player-control shortage/recovery and current-format countdown saves now pass. Count this playable situation once as checkpoint ten; the full independent whole-project review is now due.

## Final comparison and delivery evidence

| Plan | Welcome-only ending | Provisioning ending | Final 300s fed resident-time | Final stored food |
| --- | ---: | ---: | ---: | ---: |
| Wild food only | 555.0s, false success | Does not complete | 57.9% | 2 |
| Prepared gardens | 921.0s | 921.0s | 100% | 201 |
| Prepared grain/bakeries | 999.9s | 999.9s | 100% | 294 |
| Early arrivals, then gardens | 817.8s | 817.8s | 100% | 191 |

The revised objective rejects the underprepared route without lengthening the other three. Early arrivals remain a valid labor strategy, not a manufactured mistake. Original and revised outcomes use identical production/consumption rules. The first source files are preserved, hash-checked, under `artifacts/food-land-comparison/sources`; its old completed saves belong to that rejected ending and are not current gameplay fixtures. Revised results/source manifest/saves are in `artifacts/food-land-reserve-comparison`. No save migrations are added.

`--food-land` verifies eight committed shared arrivals, no housing gate, actual crossing and exact countdown/arrival save continuation. The original `--welcome-meal` suite passes four arrivals, welcome lifecycle and all six edible pantry cases. Both projects build without warnings.

`./Review.ps1 Capture neighborhood-food-land -Storybook -Width 960 -ProbeControls` and the 1440 equivalent pass through actual rendered controls. The journey builds four eastern cottages, shares the welcome, checks that this has not completed the situation, reaches natural hunger without disabling the remaining berry source, builds two farms/two bakeries and a pantry, then reaches the reserve and saves/reloads the recovered state. Waiting uses simulation ticks, not a native playtest. Final runs: `20260913-060419-133-neighborhood-food-land-8bc68b` (960, 85.42s) and `20260913-060427-821-neighborhood-food-land-49c29f` (1440, 84.19s). They ran concurrently after one build/fixture preparation, so these totals are not isolated performance measurements. The final log wording was corrected afterward to distinguish scarcity from the introductory journey's producer pause; behavior is unchanged.

The earlier 960px run passed mechanics but exposed the remaining food task below excessive text. Goals now shows arrivals, eastern homes, welcome and actual stored portions. The corrected post-welcome screenshot was inspected. The scripted click helper now waits for deferred menu wrapping/scrollbar layout and clicks within the visible clipped button rectangle; the old offscreen comparison-entry failure is retained in run logs. This is test-input reliability work, not a claim that all UI problems are solved.

## Critical limit and review gate

This is a playable food/land situation, not proof of a deep or enjoyable campaign. The two prepared routes and early route become comfortably productive after construction; the counter does not establish continuing interest. It removes a misleading victory and creates a legible preparation/recovery task. Checkpoint ten's independent reviewers must assess the whole project, challenge whether that task earns its place, and may reject it or the neighborhood direction. Broader campaign rollout and further implementation pause for that synthesis. Native interaction, continuous motion judgment, audio listening and human enjoyment remain unobserved in this chunk.
