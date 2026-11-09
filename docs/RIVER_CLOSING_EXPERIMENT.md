# Before the river closes — a longer preparation experiment

November 9, 2026. The owner found recovery meaningfully different but wanted more scope and difficulty than a short tutorial. This bounded scenario tests whether helping a growing village sustain itself creates worthwhile preparation over a longer session.

## Playable situation

Start with the broken-crossing village: four housed residents, eight food portions, four logs, and a ripe field across the damaged crossing. Recover that supply or grow nearby. Two announced boats bring four residents each: minute 8 includes 24 food portions; minute 16 brings no food. The river closes at minute 24. All twelve must have reachable homes and eat fresh food after closure. Sustain fed residents and at least twelve stored portions for four continuous minutes, with production covering meals eaten across that interval. Shortages reset the stability timer and remain recoverable.

Crops are delivered in batches. The proof compares cumulative deliveries and meals across the trial, not a rolling rate at every instant. Reaching four minutes with a production deficit waits for production to catch up while living conditions stay healthy. Imported boat provisions never count as production.

Prepared players can welcome the next boat early, then end the supply season early. The contextual objective action becomes available with enough beds for the next population, two meals per person stored, everyone fed, and three minutes of production at the future demand rate. Automatic arrivals still happen on schedule when unprepared. Pause, 3x and 6x remain available.

Each scenario retains its own save. No seasons system, new needs or decoration objectives were introduced.

## Mechanical evidence

The focused `--river-closing` comparison uses actual construction, forestry, crop work, arrivals and meal consumption. The no-expansion control repairs the crossing and orders renewable forestry but adds no homes or fields. Expanded branches prepare four additional cottages and extra crops in two stages or all at once. These are authored strategies, not independently discovered solutions.

Initial measured outcomes before the final save/UX checks: opening-only control remains incomplete at 26 minutes with twelve residents and four beds; staged expansion completes at 1,727 simulated seconds (28.8 minutes); early full preparation with readiness advancement completes at 889 seconds (14.8 minutes). Final rerun results follow below. Duration varies with placement and orders, and does not measure interesting decisions.

The tests check arrival count, imported-food conservation, failed opening-only shortcut, default closure floor, and current-format saved continuation. The final test additionally resumes the middle of the self-sufficiency trial, including the early schedule. Existing household/recovery regressions are run separately.

## Evaluation and limits

Independent read-only evaluator: river_closing_review. Source review identified forced-wait risk, misleading food-building guidance during a healthy trial, and missing early-trial save coverage. Addressed with readiness advancement, watch/resume guidance, and trial continuation testing. Compact native captures are inspected on the final build separately from the model checks.

The real pressure is population growth and food/housing capacity. Closure starts the assessment; it does not change weather or production, and there are no later food shipments to remove. This is a longer growth-and-supply experiment, not evidence of a developed late-game survival system. Reusing the recovery map keeps the opening familiar; the test is what happens after recovery.

Ordinary independent mouse play, motion inspection and listening remain unverified because desktop input tooling was previously blocked. Native stills and simulation cannot establish enjoyment, difficulty or sustained agency.

## Provisional decision

Deliver for owner play and stop. Keep clear objectives, recoverable food pressure and the option to advance when prepared. Do not expand the campaign until the owner can judge whether preparing for arrivals produces decisions worth making or mostly waiting. Ask where the level required a change of plan, where it became idle, and whether the final village felt earned. If the early-control experience still reduces to ordering everything immediately, revise the sequence or economy rather than lengthen the clock.
## Final verification

Zero-warning game and test build, DLL SHA256 `7D6EF124334969DEA486B083B7CF2AB2219FCF576A82EA1C47B3B5601D7C6143`. Final rerun retains 1,727s staged completion and 889s early completion; opening-only remains incomplete at 1,565s. Arrival and mid-trial exact continuation pass, including early closure. First-home, lean-landing and crossing-recovery regressions pass, including delayed hunger recovery.

Native 960px captures under artifacts/review/runs (each capture-0001/view.png):

- Opening: 20261109-165829-897-crossing-recovery-e07f77.
- Completion: 20261109-165848-172-crossing-recovery-ab0c00.
- Ready early / end supply season action: 20261109-165912-441-crossing-recovery-3bc47a.
- Closed and underprepared: 20261109-165931-967-crossing-recovery-f71998.

The recovery scenario identifier supplies camera framing; these are explicitly imported river-closing snapshots, not recovery completion evidence. All four objective/action panels fit the compact viewport. Captures do not exercise ordinary mouse interaction or demonstrate the early-advance button was clicked.
Independent final verdict: ready for owner trial, not a proven 25–40-minute challenge. All four 960x640 cards and primary actions are legible, with no visual blocker. The evaluator notes that housing guidance still takes priority when both homes and food are missing; check whether that feels sensible during recovery. This remains an explicit observation for owner play, not a reason to add another speculative UI system before feedback.