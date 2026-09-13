# F31d — A lakeside hall

September 13, 2026. **Optional public-works continuation delivered.** Found the village, declare it ready, then choose **Village → Next project · A lakeside hall**. This stays in the same village/save. Checkpoint28; next whole-project review30.

## Playable behavior

The founding map now has a reachable36-stone outcrop at(15,-1) on the far shore. Existing quarry, sawmill and gathering-hall rules apply:6 logs for either workshop,8 planks plus12 stone for the hall. No new producer, resource type, assessment, population target or time gate. Materials are not injected on beginning the project. The catalogue and outcrop are available before the project too, so players may plan ahead.

The Village panel offers a stone-location link and normal placement shortcuts for quarry, sawmill and hall. The hall may be ordered before its supplies exist; progress shows incorporated planks/stone. A completed ordinary break at a finished hall records its actual resident, then **This hall is ready** offers a personal ending. No attendance streak or claim that one visit proves a sustainable economy. Finishing preserves normal play; Keep building or return to the menu.

Starting, actual hall visits and project completion persist in current format48. Repeated start/finish commands are rejected. Cancelled/demolished halls cannot finish an active project; a replacement may be built normally. A declared ending remains historical if the player later rebuilds. No save migration or preservation work.

The compact brief fits the planning actions at960px: [project](images/lakeside-hall-brief.png). [Completed project](images/lakeside-hall-finished.png) uses the existing hall model, construction and recreation presentation. No new meshes/audio or independent presentation review; these are inspected static views, not motion/listening acceptance.

## What the alternatives actually do

Start every arm from the same ordinarily founded twelve-resident village at127.4 simulated seconds. Begin the project, order a quarry and sawmill at the same requested sites, and vary the hall site. All three orders may be queued while paused. Continue every arm900 simulated seconds, validating conservation/state every10 seconds; measure actual movement and completed breaks.

| Site | Built after project start (seconds) | First completed break (seconds) | Loaded planks/stone travel | Hall-bound walking | Completed hall visits | Unfed resident-seconds |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| home-side | 409.1 | 421.3 | 867.7 | 705.8 | 40 | 0.0 |
| stone-side | 515.7 | 557.0 | 1252.0 | 1460.3 | 23 | 83.3 |
| awkward | 398.3 | 410.5 | 1102.0 | 1173.4 | 35 | 0.0 |

Actual hall sites/facings: home-side(-1,3)/3, stone-side(13,7)/1, northern(8,-11)/1. The test's `awkward` name is a hypothesis, not a verdict: the northern site builds slightly sooner than the home-side site, while requiring more visitor walking. Travel is simulated distance, not wall time, not weighted by cargo amount, and includes activity over the entire equal900-second observation window. It is not an isolated construction-cost metric. Different build times leave different amounts of time for visits within that window.

**Reject the initial assumption that quarry proximity alone improves hall supply.** Extracted stone travels through storage. Without a local stockpile, the far-shore hall increases both loaded-material travel and resident walking, and the run records missed meals. The home-side site is better on those measures in this comparison; do not claim equally attractive strategies. The brief's tooltip explains storage rather than promising a false tradeoff.

Recovery test: take the northern endpoint, order ordinary demolition, build a home-side replacement, wait for dismantling/construction and an actual break at the new hall. It passes conservation/state and preserves declared completion. This proves reconstruction works, not a measured improvement against a same-age unrepaired control.

The project gives existing production chains a concrete purpose and lasts about7–9 simulated minutes before first use in these routes. That is not human play duration or proof of difficulty/enjoyment. Good planning up front remains legitimate. No need to add disruption merely to force more clicks.

## Verification

Build passed. Three common-age construction/use routes, early-finish guards, repeated-action guards, exact current saves, ordinary dismantle/rebuild and replacement use passed (`--founding-hall`). Existing founding routes were rerun against the outcrop/current format. UI journeys exercise the actual continuation button, stone focus, quarry placement mode, active-project F9, ending and Continue. Construction orders and intervening ticks are test-driven, not an uncoached playthrough. Final full probes passed at960 and1440, including the existing food-policy/save and worker-dedication controls.

An earlier full probe completed the hall journey but failed the subsequent food-policy restore check. Its on-disk save still had the previous retention value. The check now verifies persistence immediately after F5 and includes the notice in its failure, separating write failure from restore failure. Two final full runs passed. This does not prove the previously observed intermittent save-write problem fixed; no speculative persistence rewrite was added.

Final source fingerprint `C79898B779ECA29B1591EFD0056D07A05CBC1E6FC06C049A759B8C8BCAF2B7BD`; game assembly `4711C79D8595659001B0C2A09982E5A54EC4C4DD3AE2E7A985EA776ECD44A2F4`; test assembly `BBF0637612558C479E322186D5DF409A8B95EE9D7D9E1570A298CDB0B9B40005`. Base `717f8f1` plus this chunk. Final UI bundles: `20260913-223341-062-founding-hall-68b7a3` and `20260913-223422-265-founding-hall-7d1035` under ignored `artifacts/review/runs`. Simulated alternatives, starts, early/late saves and recovered world live in `artifacts/founding-hall`. Reproduce with `./Review.ps1 Build`, test DLL `--founding-hall`, or `./Review.ps1 Capture -Scenario founding-hall -Width 960 -ProbeControls`.

No native/uncoached play, continuous visual observation, listening, frame-performance measurement or human preference acceptance. No periodic review claimed before checkpoint30.

## Next decision

F31d2: test supporting a remote worksite with the existing local stockpile and food storage before changing its rules. The far-shore hall currently loses on both supply and resident travel. Compare a deliberately supplied remote site with the unsupported control and the home-side hall; make the actual supply route readable in the existing construction/inspection flow. A local depot is an investment to evaluate, not a compulsory building or new logistics framework. Retain or cut the remote-site premise based on that evidence. Do not expand the campaign or assert challenge from elapsed time. Review30 still evaluates the whole project and this direction.
