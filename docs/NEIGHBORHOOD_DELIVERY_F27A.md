# F27a playable neighborhood delivery

September 13, 2026, delivery commit `d27335c`. The chosen neighborhood gameplay is delivered as one combined outcome. This accepts an introductory settlement slice with real spatial food choices and recovery; it does not accept the welcome as a challenging campaign or establish human enjoyment. The gameplay foundations were not counted separately.

## What the player gets

Main menu > Neighborhood experiment > New neighborhood now starts the landing/meadow layout with shared workers, local grain, producer food stores and physical distribution. The original neighborhood, old food workflow on the same landscape, and original river campaign remain explicit comparisons. Continue, restart and save/load preserve the chosen rules.

Players choose where to cross and build eastern homes, commit to four arrivals, and prepare a physical welcome at a venue they select. The same village continues operating afterward. Workplace placement affects collection and delivery; optional pantry targets redistribute food. The [matched food comparisons](WORKPLACE_FOOD_T02.md) establish viable western foraging without a pantry, meadow foraging with a pantry, and a balanced local farm/bakery chain. Poor placements and premature replacement can cause recoverable shortages. No yield changes or mandatory pantry stamp were used to make the alternatives work.

## Evidence

`./Review.ps1 Capture neighborhood-journey -Storybook -Width 960 -ProbeControls` starts from the chosen menu entry. It uses rendered pointer/keyboard controls to place and rotate buildings, choose a venue, invite newcomers, pause the only food producer, build the replacement farm/bakery/pantry chain, set pantry targets, and save/load the recovered village. Waiting uses accelerated simulation ticks with validation and rendered updates; this is scripted UI coverage, not native or uncoached play. At 960px the complete run passed in 73.9 seconds including build/setup, in `artifacts/review/runs/20260913-053717-838-neighborhood-journey-7437b7`. Welcome, shortage and recovery snapshots were preserved; the welcome and recovered views were inspected. The 1440px journey also passed in 64.66 seconds in `20260913-053937-724-neighborhood-journey-d9b108`, including the single-write launch request.

That path found a real defect: paused workplace stores continued serving meals, but the validator rejected those reservations. Validation now permits paused producer stores while retaining ordinary pantry closure rules. A focused regression pauses a forager with an active meal reservation, continues the exact saved state and observes collection from the paused store. `--workplace-food` and the original `--welcome-meal` suite, including all six edible pantry cases, pass. Both projects build without warnings.

`./Review.ps1 Inspect neighborhood-working-village -Storybook` opens a normally constructed 12-resident, 14-building local farm/bakery district after the welcome, with original foraging paused. Its source generator builds the village from a fresh chosen workflow; it does not import an undocumented old save. A 30-second 1x movie finalized in `artifacts/review/runs/20260913-053233-171-neighborhood-working-village-0de8cb`, 56.57 total seconds including preparation/build. Media inspection finds 732 frames, 725 distinct frames and stereo 48kHz audio. Stills were inspected; these measurements are not listening or continuous motion judgment.

A transient request-file mapping failure also exposed redundant immediate writes in Review.ps1. The complete launch request is now written once. Resume failures preserve expected state and a screenshot for diagnosis; one earlier resume failure did not recur on the subsequent full journey. Do not claim all sync/OS file contention resolved.

## Product decision and remaining work

Count the combined F27a delivery once, as checkpoint nine. Keep the neighborhood's production/distribution direction because physical journeys now support multiple credible layouts and recoveries. Keep the welcome as the introduction, not a disguised long-term audit. Stop adding small comparison variants to avoid making a direction decision.

The unchanged four-patch opening can still support the village without new food infrastructure. A player can finish the welcome before running into a consequential economic choice. That is acceptable for this introduction, insufficient for the campaign the user wants. Next author one stronger food/land situation using the chosen rules and scarce visible sources; test directly edible and grain/bakery responses, premature commitment and recovery. Do not simply extend a quota, add another mandatory building, or port all ten old levels.

F27b remains a retained comparison candidate with presentation/audio acceptance outstanding. Native interaction, listening and human preference remain unknown. The next committed playable outcome reaches checkpoint ten and requires the independent whole-project review, including visual/audio, before subsequent implementation. It must challenge the chosen logistics direction as critically as the original game.
