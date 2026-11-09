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
