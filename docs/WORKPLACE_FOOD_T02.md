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
