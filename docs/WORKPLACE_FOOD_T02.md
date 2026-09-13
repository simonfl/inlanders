# Workplace food comparison — T02

September 13, 2026. Retain this as a bounded logistics candidate, not the default campaign or an accepted fun verdict.

## Playable change

Main menu > Neighborhood experiment > Try workplace food starts the landing/meadow map with producer storage. Foragers, vegetable gardens and bakeries carry output to their own 24-portion store. Residents physically collect meals there. Haulers retain four unreserved portions locally and carry surplus to requested neighborhood pantries, then central storage. Full stores use the existing overflow delivery. Pausing production keeps food available; demolition recovers it through physical jobs. Farms already store grain locally. Other producers and the original modes retain their existing behavior. Continue, restart and current-format saves preserve the workflow.

This separates production from distribution without changing yields, hunger, demand or deadlines. The small stock tray and selected workplace inspector show food ownership. The four-portion rule is an experimental policy, not an established balance constant.

## Matched recovery evidence

`--workplace-food-comparison` builds one natural shortage by replacing the original forager before the new food chain is ready. It clones that exact state for six plans and observes 2,400 seconds. The workflow flag is the only difference within each pair; new buildings are normally constructed. The two-farm/two-bakery plan corrects the earlier single-farm/three-bakery comparison's ratio limitation. Three gardens here are all near the meadow; earlier runs placed the first near the landing, so cross-run figures are not identical-plan controls.

| Recovery plan | Workflow | Final 300s resident-time fed | Seconds with anyone hungry | Recent meals on time | Stored food |
| --- | --- | ---: | ---: | ---: | ---: |
| Restore foraging | Original | 100.0% | 0.0 | 36/36 | 153 |
| Restore foraging | Workplace | 92.4% | 227.4 | 30/35 | 0 |
| Three gardens + pantry, unused grain paused | Original | 53.0% | 300.0 | 9/37 | 2 |
| Three gardens + pantry, unused grain paused | Workplace | 99.8% | 6.7 | 34/36 | 14 |
| Two farms + two bakeries + pantry | Original | 91.0% | 234.8 | 27/34 | 2 |
| Two farms + two bakeries + pantry | Workplace | 100.0% | 0.0 | 36/36 | 33 |

The bakery plan sustains service; gardens almost do. Foraging regresses. No food-pickup hauling occurs in its final observation window, so do not blame excessive surplus hauling without further evidence. A workplace deposit changes the producer's route and meal access; evaluate a better-sited hut and an optional pantry before choosing whether that tradeoff is understandable. Do not tune yields to make every fixed placement win.

Artifacts: `artifacts/workplace-food-comparison`, six start/end saves, 60-second stock samples, last-window person-task seconds, source hashes and a shared mistake save. All states validate and roundtrip. Runner wall time: 200.44 seconds. The runner reused baseline construction, placement and save checks rather than duplicating the scenario. These scripted plans establish feasibility and failure, not optimal strategies, human pacing or player preference.

## Verification and next decision

`--workplace-food` passes physical workplace deposits, meals, surplus collection, pantry delivery, active-save continuation and demolition recovery. `--welcome-meal` passes the original welcome/neighborhood and all six edible pantry cases. Both projects build without warnings. Rendered menu/Continue/restart/load, original-mode isolation, staffing and arrival probes pass. The selected workplace stock explanation was explicitly captured and inspected at 960px in `artifacts/review/runs/20260913-052350-064-neighborhood-workplace-food-27e8b5/capture-0006/view.png`; full capture took 19.6 seconds including builds. This is scripted rendered control evidence, not native play or listening.

Next: settle the foraging location/distribution tradeoff, then choose the production/distribution direction and a concrete post-welcome decision. Stop indefinite balance or static-art iteration. The welcome remains introductory; a fed settlement by itself does not supply the skill and continuing agency the user requested. Campaign rollout waits for this direction decision. F27a/F27b remain under experience validation; playable count stays eight.

## Siting follow-up and direction decision

`--workplace-food-siting` reuses the same shortage and runs six new 2,400-second branches. It considers all four legal facings before moving away from a requested center, and records actual building cells/facings. The previous helper preferred rotation zero anywhere on the east bank: its nominal landing hut was actually at (15, -3). Earlier results remain valid for that actual arrangement, but do not describe a close landing hut.

| Hut arrangement | Original workflow: fed resident-time / food | Workplace workflow: fed resident-time / food |
| --- | ---: | ---: |
| West patches, (-7, -2), facing 0 | 100% / 169 | 100% / 123 |
| Meadow patches, (17, 8), facing 2 | 100% / 164 | 99.89% / 20 |
| Same meadow hut + pantry at (19, 5) | 100% / 156 | 100% / 37 |

The meadow hut alone has four seconds with someone hungry; the other workplace plans have none in the final 300 seconds. This establishes a western recovery without a compulsory pantry and an eastern recovery with distribution. No runtime rules changed in this follow-up. The source manifest includes the baseline digest; all buildings completed, saves validate/roundtrip. Six arms took 78.77 seconds in `artifacts/workplace-food-comparison-siting`.

**Choose workplace production and physical distribution for the neighborhood direction.** Hut location now affects actual return trips; local bakeries and directly edible food both support recovery. Keep the original workflow as an explicit comparison, not the preferred design by inertia. Do not require every poor placement to recover automatically. Stop this balance-comparison sequence. The supply-route help now describes the actual workflow instead of incorrectly claiming huts are bypassed; rendered controls at 960px pass and the corrected help is captured in `20260913-052902-991-neighborhood-workplace-food-e312e5`, total 20 seconds including builds.

Next deliver the chosen complete neighborhood entry with clear comparison access and a realistic scene of its working economy, then assess the combined gameplay/presentation outcome. Campaign progression should create a consequential food/land commitment after the short introduction, rather than treating a completed welcome as sustained challenge. Keep human preference, native play and listening limits explicit; this direction choice is not proof of enjoyment.
