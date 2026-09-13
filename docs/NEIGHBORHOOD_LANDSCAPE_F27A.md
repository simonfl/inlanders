# F27a: spatial alternative and failed recoveries

September 12, 2026. Follow-up to the [six-arm control](NEIGHBORHOOD_COMPARISON_F27A.md). F27a remains under evaluation; this is not a whole-project cadence review.

## What changed

The Neighborhood experiment menu now offers **Try landing and meadow** alongside the original map. The west hamlet, initial population, edible stock, timber quantities and bush count remain unchanged. The east bank has a compact landing, a three-cell-wide passage and a larger meadow. Resources are relocated at generation, not deleted during play. Starting either experiment replaces the experiment save; Resume and restart retain the chosen map. No campaign rule or save migration changed.

The first meadow was too small to fit a plausible garden/storage recovery. That failed layout prompted a larger clearing before accepting any comparison. This is still a geometric blockout: the rendered scene is legible as two sites but is not an inviting landscape. F27b must address the whole scene, not decorate this outline and declare the visual problem solved.

On the enlarged map, all six plans completed. Landing berries/garden/grain took 235/200/204 seconds after the crossing; meadow equivalents took 364/381/376 seconds. Meadow hunger lasted 0.8/20.5/63.6 seconds respectively; landing plans had none. Berry-only walking increased from 7,633 to 10,935 cell units (about 43%). These results supersede the first smaller meadow. They show a real journey cost, but do not yet demonstrate a compensating benefit that makes meadow production worth choosing. The final six-arm run passed validation/roundtrips in 34.3 seconds, excluding compilation.

## What the recovery experiment found

Through ordinary commands, build a crossing, plan two landing cottages and a seating garden, start a meadow grain farm, dismantle the old forager hut and commit arrivals. Select the venue when built. Wait for the first hunger, then clone this same state for alternative recoveries. No food, time, population or worker state is injected. This is a deliberately mistaken plan, not a claim that an uncoached player made it.

The welcome is already complete before hunger begins. That matters: the event proves an arrival was celebrated, not that the expansion has a viable food supply. Treat it as a welcome milestone, not a successful logistics challenge.

| Recovery from the same shortage | Observation | Finding |
| --- | --- | --- |
| Restore one forager hut | 1,200 seconds | Everyone fed after about 182 seconds; no hunger in the final 300 seconds; 81 portions remain. |
| One garden or one bakery | 1,200 seconds | Neither restores fully fed status during observation. |
| Two gardens, with/without pantry and paused grain | 1,200 seconds | Insufficient; even the best of these variants still has hunger throughout the final 300 seconds. |
| Two bakeries plus pantry | 1,200 seconds | Some fully fed intervals, but hunger during 264 seconds of the final 300. Not stable recovery. |
| Three gardens, pantry, unused grain paused | 2,400 seconds | Hunger during 229 seconds of the final 300. |
| Four gardens, pantry, unused grain paused | 2,400 seconds | Improved but still hunger during 108 seconds of the final 300. |
| Three bakeries plus pantry | 2,400 seconds | Hunger persists; extra buildings alone are not an adequate answer. |

The capacity branches intentionally use a longer horizon to distinguish recovery from a brief fully-fed reading. These are fixed plans, not optimal staffing or placement. New fields and ovens are placed near the meadow, with the nearest legal zero-facing position preferred and other facings available if needed. Both material investment and subsequent labor matter; do not infer a universal producer ranking from these runs.

## Direction and next work

Do not scale the current economic design into the campaign. Retain the physically delivered welcome as an introductory village event, with honest limits. Keep the original and spatial maps available for comparison. Production choice and recoverability still need redesign evidence.

Code inspection found a candidate structural contributor: hunger globally slows walking, working **and eating**, even for fed residents, while meal deadlines continue at full speed. Shared workers also retain fixed profession/concurrent-food caps during shortages. These rules may create an opaque recovery spiral. Next compare removing the global slowdown in the neighborhood experiment against the exact same shortage snapshot; change no production yields, stock or deadlines in that arm. Inspect the resulting food and worker activity before deciding whether to keep the change. If insufficient, test the labor restriction independently instead of bundling fixes.

This is a falsifiable mechanics comparison, not a commitment to make every bad layout sustainable. Keep failures, assess whether the player can understand the consequence, and then choose a direction. Native play and the whole-scene visual comparison remain necessary. Count stays eight; no accepted playable checkpoint is claimed here.

## Evidence and tooling

`--neighborhood-landscape` reuses the six-arm runner, adding distance traveled, moving person-seconds and idle person-seconds over the observation. Spawn positions are excluded from travel distance. These are aggregate costs, not subjective pacing or productive labor. Control/alternative keep separate outputs in `artifacts/neighborhood-comparison` and `artifacts/neighborhood-landscape`.

`--neighborhood-recovery` and `--neighborhood-recovery-capacity` record the natural mistake, saved branches, first fully-fed time, hunger duration and final-300-second hunger. Saves validate and roundtrip. The runner now records source hashes and completion status for future recovery runs too. The longer capacity run was performed before that metadata-only addition; its observations and snapshots remain the evidence above.

T02 reuse required one map factory and measurements in the existing runner, not a framework. It exposed an undersized clearing and prevented treating intermittent recovery as stable. Setup-time savings have not been measured. Engine capture reached the map and scripted controls; native discovery returned no targetable Inlanders window on repeated enumeration although the review-owned process existed. That process was closed. No native gameplay or listening is claimed.

Final UI verification: game/test builds passed without warnings. The 960-pixel rendered probe passed alternative-menu entry, objective isolation, Continue, restart preserving the landscape, manual load, original-river switch and Resume, dedication/release, committed arrivals and actor/roster feedback. The process exited successfully after 124.9 seconds including build/preparation. The enlarged-map capture was visually inspected. This is scripted UI evidence, not native play.
