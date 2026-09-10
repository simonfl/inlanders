# F24b: food transport and the value of local timber storage

Baseline: `286fd17`, after the investment pass. This completes the first F24 building-role/payoff review with measured tradeoffs, not a claim that the economy is finally balanced or human playtested.

## Adopted behavior

- Bakers carry the complete four-loaf batch to the pantry in one trip.
- Farmers harvest up to four grain per load. A six-grain crop takes two collection trips; vegetable harvest loads remain two.
- Cargo models show the actual contents: two layers of bread and two rows of grain. Reassignment still returns physical cargo; save/load preserves it.
- Per-load harvest work remains two seconds, so a six-grain crop also needs one fewer harvesting action. No price, recipe, crop-growth, movement-speed, staffing or stockpile-rule changes in this chunk. Bakers still fetch two grain per batch.
- Building guidance explains the pantry trip and that stockpiles need no hauler for local logger deposits. Haulers are optional redistribution labor.

## Food evidence

Run `./Test.ps1 -Balance`. All runs use two food workers, the same campaign-2 homes/hut, 96 starting food, two loggers and two builders. No paths or interventions. 9,000 fixed 0.1-second ticks give nominally fifteen minutes; float meal boundaries produce fourteen meals. Delivered totals include food subsequently eaten. No hunger occurred because starting reserves covered opening deficits; this alone is not evidence of sustainability.

The first field is at `(3,-3)`. The second building is either distant `(6,-3)` or near the pantry `(-4,2)`. Three-building cases add a field/garden at `(0,6)`. Each building occupies six tiles, excluding entrances and walking space. Garden comparisons use exactly the same sites as their bread counterparts.

| Setup, two food workers | Baseline delivered | Four-loaf basket only | Adopted basket + grain loads | Adopted deliveries in minutes 5–15 |
| --- | ---: | ---: | ---: | ---: |
| Two foragers, existing reachable bushes | 338 | 338 | 338 | 224 |
| Two gardens, distant second site | 90 | 90 | 90 | 68 |
| One farm + distant bakery | 66 | 96 | 108 | 84 |
| Two gardens, near second site | 112 | 112 | 112 | 82 |
| One farm + near bakery | 96 | 96 | 120 | 92 |
| Three gardens, distant second site | 120 | 120 | 120 | 96 |
| Two farms + distant bakery | 64 | 108 | 108 | 84 |
| Three gardens, near second site | — | 168 | 168 | 124 |
| Two farms + near bakery | — | 140 | 168 | 128 |

The baseline distant baker spends 353.2 of the established 600 seconds collecting/delivering bread, with no grain shortage. A whole batch removes the second collection trip. With that change alone, the near baker instead waits 323.4 seconds for grain. Four-grain harvest loads reduce that wait to 260.7 seconds with one field and 118.1 seconds with two. A second field does not help a distant bakery once transport limits the baker; placement matters.

**The resulting choice:** two gardens cost eight logs; one farm plus bakery costs twelve, with the same twelve building tiles and two workers. Gardens deliver sooner and are simpler to staff. Bread now offers higher continuing output in both tested layouts, and the near chain covers eight-person demand after startup. Three near gardens and two near farms plus bakery both deliver 168 in the full run; bread's slight steady-output advantage costs four extra logs. This is a modest option, not an automatic upgrade. Nearby foraging remains exceptionally strong but depends on existing bushes; do not weaken it just to force bread construction.

The garden/field results depend on geography, growth cycles, stocking and staff. Later campaign maps should test constrained berry access and competing land, with live production feedback so the player can discover these choices. These results are not universal building ratings.

## Storage evidence

The previous three-home startup test remains in the harness. Added: twelve cottages in three four-home waves on the dry, flat Three clearings map. One layout expands east; another grows across the northern woodland. Exact legal cells are printed by the harness. The stockpile is at `(9,2)` or `(-9,-3)` respectively. All variants use identical cottage sites; the depot footprint is reserved when choosing the layout even for the yard-only case.

Two loggers and two builders work throughout the expansion; the hauler variant uses an additional worker. Starting food isolates timber work, and no newcomers are invited: this measures building supply, not whether a 24-person village can feed itself. Loggers stop after collecting the construction requirement plus twelve spare logs. Their return trips still count as labor. This normalizes the final useful timber output instead of rewarding a variant for producing fewer spare logs. Stockpile construction, its four logs and every hauling trip are included. Target remains six. Placement retries footprints temporarily occupied by workers.

| Twelve-home layout | Storage | Finished at | Logger + builder + hauler active time | Final stored / carried logs |
| --- | --- | ---: | ---: | ---: |
| East | Yard only | 468.8s | 1,488.2 person-seconds | 12 / 0 |
| East | Stockpile, no hauler | 495.7s | 1,500.6 | 12 / 0 |
| East | Stockpile + hauler | 482.0s | 1,693.6 | 12 / 0 |
| Northern woodland | Yard only | 530.9s | 1,605.4 | 12 / 0 |
| Northern woodland | Stockpile, no hauler | 509.3s | 1,574.2 | 12 / 0 |
| Northern woodland | Stockpile + hauler | 480.5s | 1,846.8 | 12 / 0 |

An unstaffed woodland pile repays setup by the twelfth home, saving 21.6 elapsed seconds and 31.2 person-seconds including its extra construction. At eight homes it has not yet paid back. The hauler finishes that neighborhood 50.4 seconds sooner than yard-only but spends 241.4 additional person-seconds. That is a speed-versus-labor decision, not an efficiency improvement. The eastern pile fails to pay back in this run; placing a depot simply because a construction site is distant is insufficient.

**Keep the existing storage rules.** Local harvest and local use give the pile a purpose without another worker. Redistribution can be worthwhile with spare labor and a timing need; blanket recommendations to staff every pile were misleading. Do not add larger log loads or carts just to make the hauler win every comparison. Routes, workplace links and production diagnostics are the next way to make this understandable.

## Verification and roadmap consequences

- All five campaign lessons still complete with current-format save roundtrips. Scripted seconds: 122.6, 222.5, 221.7, 234.8, 302.7. Bread lessons become shorter; campaign depth belongs to authored decisions, not inefficient deliveries.
- Simulation checks cover four-grain/four-loaf loads, exact save continuation, reassignment during transport and resource conservation, along with existing production, storage and campaign coverage.
- Rendered food smoke covers the visible loads, active-batch save/load, supper, woodland and sawmill. Basket captures are written under `artifacts/f24b-*-basket.png`.
- The HUD suite passes, including the six-to-two-grain partial harvest, rotated crop rows, paused save/load, stockpile controls and wide/narrow layouts. The food smoke checks that meals occurred without requiring four meals before supper: the faster chain now qualifies after three in that fixture.
- Local experiment logs: `artifacts/f24b-before.log` (two-unit transport), `artifacts/f24b-final.log` (bread basket and expanded layouts), `artifacts/f24b-grain.log` (adopted behavior). These are ignored local outputs; the tables above preserve the evidence in the repo.

F24b closes this measured first pass. F21h is next: distinguish input shortage from collection/travel, show recent food production against consumption, and let the player direct workplaces. Keep logistics route visibility with F07b. F11b/F18b still needs substantial human-playtested scenarios, and F23b still awaits acceptance of the art slice. No further blanket price or production adjustment is justified by these experiments alone.
