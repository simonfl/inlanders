# A first home — bounded experience comparison

November 8, 2026. Provisional challenger to the After the flood baseline at 249. This is an experiment, not the next campaign chapter or an accepted replacement direction.

## Question

Does ordering useful work for a small, named household make the player care about its result? The owner enjoyed grove renewal. Four residents, an explicit grove order, a cottage and a livelihood give that action a modest human purpose. The competing explanation is that the game still consists of placing buildings and waiting, with names layered over the same weak loop.

Marie and Louis host Jeanne and Pierre. The player chooses timber, a home site and food work. Existing carriers, construction, production and meals perform the work. The story ends after Jeanne and Pierre have a reachable new home and have each eaten fresh food. Starting provisions alone cannot finish it. Other features remain available; this slice adds no new needs, inherited property or decorative controls.

## Mechanical evidence

The focused `--first-home` check covers four-person identity, blocked population expansion, no unordered timber, cancelled/replaced cottage planning, actual construction and production, household meals and exact save continuation. The gathering route completed at about 346 simulated seconds; gardening at about 234 seconds (printed test times include five seconds of save-continuation checking). These are scripted feasibility results, not player discovery or enjoyment.

The checks exposed two integration issues before handoff: the save loader assumed at least eight residents, and the first meal condition incorrectly excluded produce hauled to central storage. Both were corrected. The opening contains only berries; other edible kinds are fresh production regardless of storage destination. Berries count when collected at a producing hut, or after consumption has exceeded the opening stock.

Existing producer-attribution checks in Normal/Relaxed and crossing benefit/cancellation checks pass. The build has zero warnings. Full legacy regression was not repeated for this provisional scene.

## Runtime evidence

Independent evaluator launched the ordinary game on DLL SHA256 `BE1EC9DD711E09D795226213CC37784028A9D21320CBDAAF1624B99D2D2FD9D1`. Window inventory succeeded, but both desktop-control routes hung while retrieving the window/state (about six minutes and two minutes). Both were interrupted. No screenshot, menu action, gameplay, motion or audio observation was obtained. The launched session was closed.

Ordinary-control evaluation is **blocked, not passed**. No new human play has occurred. The builder subsequently fixed a source-inspection finding: the household panel must hide when switching into a non-household mode. This small change does not resolve the external desktop-control failure. The handoff remains provisional and asks the owner to try entry/controls as well as the experience.

Builder native capture on final DLL `41045507A7F46A851ACA47389682E1A9E1B4BE83D13DD42A0D41219E0BC13607` loaded the completed four-person garden save, advanced two seconds at 1x and exported two snapshots without engine errors. The inspected 960px still shows the four residents, two cottages, garden and settled story button. Local evidence: `artifacts/review/runs/20261109-041831-538-river-7f4241`. This is controlled rendering/advancement evidence, not ordinary UI interaction, a motion review or listening.

## Decision gate

Offer the owner roughly five minutes in A first home and five in After the flood, without a placement recipe. Ask what felt worth doing and what they wanted to try next. Keep, revise or drop based on that reaction. A clear yet empty experience is evidence against this hypothesis; a specific clarity obstacle earns one focused revision. Do not build more chapters or systems while the answer is pending.

## November 9 — owner feedback and focused clarity revision

The owner found the many equally prominent actions overwhelming, then said the objective and progress remained unclear. This is direct owner feedback about usability, not acceptance or rejection of the underlying care hypothesis.

Outcome 251 applies the single allowed clarity revision. A persistent compact checklist shows cottage planning/material delivery/construction, food production, and fresh meals for 0/2 through 2/2 residents. One contextual action handles the next step; when paused during automatic work it offers Resume village life. Food choices open on request and secondary People/Supplies/Options/survey/arrangement tools are collapsed behind Village tools. Cancelling a cottage restores the build prompt. Completed production does not expire with the rolling food-history window. Other scenes hide the household card.

Validation: both livelihood routes and exact saves pass, with added checks for initial guidance, cancellation recovery, timber blocker, completed 3/3 objectives and query purity. Native 960px captures show opening, Timber 0/6 and completion states. Independent read-only evaluator accepted their readability and reduced action overload for owner retry. Capture build SHA256: 93303A232D856346E44B9C4742623EE2B1904B697456B5D112B81978FDE76869. Evidence directories under artifacts/review/runs: 20261109-135225-811-river-243fe9, 20261109-135252-320-river-fd6a94, 20261109-135300-602-river-290bae. Reviewer caught stale milestone notices; final copy now points to Objectives and actual meal progress.

Limitations: independent review inspected scripted native stills and matching source, not ordinary clicks, motion, audio, food-choice drawer or narrower layouts. Known hanging desktop tools were not retried. Final notice-only copy was rebuilt rather than recaptured. This supports clarity, not enjoyment. Return to the owner to try the revised opening; do not add systems or another unobserved polish batch.

## November 9 — positive owner response

The owner reported clear progression, knowing what to do, and satisfaction in moving forward. They also valued dwindling provisions. They authorized UI cleanup and one different situation with the same objective structure. Keep the structure provisionally and test A lean landing; see [the return-play experiment](LEAN_LANDING_EXPERIMENT.md). This provides human evidence the earlier reviews lacked, while deeper challenge and repeatability remain open.
