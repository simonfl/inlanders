# F29b — the meadow has not earned its second-level premise

September 13, 2026. Comparison source `1a47c8cb286b6dce4d2ccf489be66b9ec822419e`. This chunk completes the decision experiment, not a new playable scenario. Count stays **19**. The user-requested [whole-game review](WHOLE_GAME_REVIEW_19.md) chooses the replacement experiment; do not inflate the reserve and count that as difficulty.

## Matched experiment

`Tests/MeadowDecisionComparison.cs`, invoked with the test executable's `--meadow-decision` flag, clones one serialized fresh meadow into eight arms: gardens/bread × split-bank/meadow homes × early/late invitation. All use ordinary construction, freely legal sites, existing yields, the same pantry target and current completion rules. Early invites immediately after crossing construction; late invites after supply, homes and venue. Nearest-legal placement and actual facings are recorded. Each completed world continues for 600 simulation seconds, validates and roundtrips its current save.

This deliberately separates home layout from supply and timing; earlier comparisons changed those together. It still tests eight informed recipes, not all player strategies. Builds are sequential, food sites and pantry policy fixed, and the placement helper can find alternatives without a human noticing them. Construction time differs, so cumulative travel/hunger before completion is not a matched-duration causal estimate. No uncoached play or enjoyment observation occurred.

## Results

| Supply / homes / invitation | Completion, sim seconds | Food at completion | Fed resident-time in following 600s |
| --- | ---: | ---: | ---: |
| Gardens / split / late | 931.4 | 67 | 100% |
| Gardens / split / early | 776.8 | 52 | 100% |
| Gardens / meadow / late | 847.6 | 57 | 99.93% |
| Gardens / meadow / early | 778.3 | 53 | 100% |
| Bread / split / late | 1041.5 | 86 | 100% |
| Bread / split / early | 724.6 | 70 | 100% |
| Bread / meadow / late | 990.7 | 84 | 100% |
| Bread / meadow / early | 836.8 | 75 | 100% |

All eight have zero hungry resident-time before completion. All finish one 0.1-second tick after homes and welcome are ready; all already exceed the 32-portion reserve. Early invitation is faster in all four matched pairs. The garden construction package costs 52 logs including bridge, pantry, homes and venue; bread costs 64. Bread is not universally slower: shared early labor completes the split-home bread route fastest. Layout and construction choices change time and travel, but this comparison does not demonstrate a reason to revise them during play. At 3x these prepared plans take roughly four to six minutes of simulation advancement, excluding player decision time.

Full source hashes, initial state, placements, metrics and final saves are local in `artifacts/meadow-decision`; the committed [metric extract](meadow-decision-f29b.json) preserves the results without duplicating eight large saves.

## Counterevidence matters

The fresh rendered meadow walkthrough deliberately builds homes before production. At **571.51s** all sixteen people have shared the welcome and eight newcomers have homes, but only **2/32** food remains and hunger is present. The reserve correctly prevents premature success. Real construction of farms, bakeries and a pantry then recovers and completes at **1110.58s**. See [the shortage](review19-meadow-shortage.png) and the fixed-build run recorded in the review.

Therefore the reserve is **not universally redundant**. It filters underprepared settlements, as the original F27c1 wild-only counterexample already showed. It has not demonstrated the richer spatial/commitment decision claimed for the second primary level. Removing it alone would restore that false success. Keep the old scenario as a comparison control until a replacement is playable; cut reserve escalation and its status as the future scenario's defining ambition.

## Decision

Do not ship another threshold adjustment. Replace the next design bet with one inherited, functioning shoreline village whose food journeys and usable shared space can be changed. Keep a finite welcome as the clear endpoint in that comparison. Doing nothing, adding a compact alternative, or building elsewhere must remain legitimate controls; do not recreate the rejected compulsory near/far commons sacrifice. The full review defines the scope and what would falsify it. A changed spatial scenario is **next work**, not claimed as implemented here.

Validation: eight completed arms and save roundtrips; `--food-land` countdown/arrival checks; fresh scripted normal opening at 960 and meadow at 1440, including shortage/recovery and save/load. These are mechanics and rendered-control evidence. They do not establish human skill, discovery, preference or native frame performance.
