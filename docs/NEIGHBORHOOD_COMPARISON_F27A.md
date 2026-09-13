# Neighborhood comparison: the economy has not earned its complexity

September 12, 2026. Simulation baseline: `0d347a7`; comparison runner added in this chunk. This is a bounded F27a diagnostic, not a periodic whole-project review or playtest.

## Verdict

The scenario works mechanically but does not yet demonstrate the promised spatial/economic choices. Both tested neighborhoods welcome and house everyone using the existing berry supply, with no hunger and a growing surplus. Extra food infrastructure is unnecessary for this objective. Do not accept F27a or roll it into the campaign on this evidence.

This does not establish that shared work should be removed or that producers are useless elsewhere. The present expansion fails to make their tradeoffs necessary. Avoid adding a bread quota or invisible food penalty to force a recipe.

## Method and results

Construct the crossing through ordinary shared work, save that common state, then clone it for six arms. Place two cottages and a seating garden at the nearest legal cells to landing `(8,2)` or meadow `(14,6)`. Add no food buildings, a vegetable garden plus pantry, or a farm/bakery plus pantry. Pantry target is twelve. Commit arrivals immediately after planning; select the venue when built. No manual staffing or stock injection. Observe each arm for 1,200 simulated seconds after the crossing, including operation after completion.

| Location / food plan | Completion after crossing (seconds) | New construction (logs) | Final edible stock | Bread baked / vegetables grown |
| --- | ---: | ---: | ---: | ---: |
| Landing / existing berries | 243.8 | 16 | 140 | 0 / 0 |
| Landing / garden | 292.9 | 26 | 208 | 0 / 72 |
| Landing / grain and bakery | 230.4 | 34 | 325 | 164 / 0 |
| Meadow / existing berries | 243.5 | 16 | 135 | 0 / 0 |
| Meadow / garden | 276.7 | 26 | 262 | 0 / 72 |
| Meadow / grain and bakery | 328.1 | 34 | 183 | 116 / 0 |

All six had zero hunger throughout observation. Costs exclude the common crossing and original hamlet. Production columns are cumulative output, not remaining stock. Completion is an objective, not a measure of enjoyment. Berry-only is cheapest, though landing grain completes thirteen seconds sooner; it does not dominate every metric. Meadow grain is slower and produces less bread than landing grain, so location can affect simulation. The objective gives little reason to care.

These are six fixed plans, not an exhaustive search. Pantry/producer additions are bundled strategies; this does not isolate pantry effects. Both sites use the old river map, not the promised authored compact landing versus spacious meadow. Arrival timing alone was not a harmful mistake here. Do not manufacture a recovery demonstration by deleting food behind the player's back.

## Next design experiment

1. Author an experiment-only map alternative: visibly constrained buildable landing near the crossing, larger meadow with production space farther away. Preserve the current map as the control and keep every building available. Test whether arranging housing, workplaces and gathering space creates understandable decisions before adding systems.
2. Measure labor/travel and source capacity alongside completion. Keep existing berries as a serious control. Separate pantry-only from producer additions if deciding whether local supply earns its cost. If food still makes placement irrelevant, choose explicitly between village arrangement with optional production and logistics requiring a different resource/demand design. Do not silently turn the former into a starvation puzzle.
3. Once a natural consequence exists, compare a plausible early commitment or premature workplace replacement with two actual recoveries. Allow completion without a prescribed building list. Then play through ordinary controls to assess predictability and satisfaction.

F27b remains the next presentation experiment; campaign expansion stays conditional. This diagnostic narrows F27a rather than adding a permanent feature. Count remains eight.

## Reproduction and limits

With the repository's bundled .NET environment, run `dotnet run --project Tests/SimulationTests.csproj -- --neighborhood-comparison`. The opt-in runner writes `artifacts/neighborhood-comparison`: common baseline, completion/final saves, sixty-second samples, results and a manifest with simulation/runner source hashes, step/window settings and wall time. A running manifest is not a completed report. Saves validate and roundtrip; failed strategies remain results rather than being asserted away. Output is regenerated locally, not a fixture consumed by normal tests.

The repeat run reproduced all six results, validated snapshots and passed exact save roundtrips in **48.0 seconds**, excluding compilation. No simulation code changed; broader gameplay regressions were not rerun for this tooling-only chunk.

This is the first use of the small T02 runner. It replaces six separately scripted setup/observation passes; savings on reuse are not yet measured. No framework, new game rule, native UI play, listening, aesthetic assessment or proof of long-term fun is claimed.
