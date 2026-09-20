# T07 — bounded interaction attribution

Count stays36: diagnostics are not a playable outcome. Reproduce with `Review.ps1 Capture -Scenario interaction -Snapshot artifacts/hamlet-paths/connected-start.json -Width 1440 -ProbeControls`. The input is the retained twenty-resident hamlet; reuse any validated comparable snapshot explicitly. Each of move/path preview, actual brush strokes, camera motion and save is exercised paused and running at1x. Direct-call timings and native phases are separate; scripted input is not human play.

Final run `20260920-024231-946-interaction-567e67` has242 completed frame intervals per segment,120 input steps. Wall p95 ranges17.81–23.04ms; move input p953.00ms paused/2.53ms running. Path input p951.39/1.59ms. Maximum wall intervals include132.59ms during paused moves and179.75ms during paused camera motion. The earlier short sample also had low ordinary intervals. These samples do not reproduce the sustained heavier hamlet costs reported in F32b, and do not explain all sporadic stalls. Machine load/phase differences are possible, not established causes.

**Decision: no speculative performance patch.** Keep the residual tail risk; gather native attribution if it recurs during the chosen design slice. No occupancy cache or engine rewrite. Paused previews preserve exact world state; validation and builds pass. Camera/saves/brushes are scripted actions, not a normal-play responsiveness acceptance.

`Development/SummarizeFrames.py <frame-json>...` reports completed interval count, median/nearest-rank p95/max, elapsed sample and source filename. It excludes the final unfinished zero-wall interval. No dependencies beyond Python. Preparation/build+full run took51.99s; this is diagnostic setup cost, not frame time. The summary removes repeated ad-hoc percentile calculations; evaluate actual savings over the next two comparisons before expanding it.

Next: the whole-place shore/woodland comparison. Do not turn an unreproduced concern into another optimization cycle.
