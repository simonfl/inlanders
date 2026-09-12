# Orchard timing comparison — F05b

September 12, 2026. **Go to a small playable orchard slice, with existing farmers.** The optimistic comparison finds a modest perennial-food tradeoff: later first delivery, fewer repeated sowing trips and slightly better mature output. It does not justify a dedicated orchard keeper, a mandatory fruit need or an orchard campaign yet. Visual payoff and human enjoyment remain untested.

## Design review

Gardens already grow unattended while farmers do other work. An orchard must therefore offer more than another idle growth timer. The candidate retains productive trees between harvests, exchanging an initial commitment for less repeat sowing. Quick gardens remain valuable during establishment and after clearing land. Keeping a grove, mixing it with quick food, or freeing its occupied plot are the intended choices.

Mechanical differentiation is modest. The useful next question is whether visible permanent trees, readable establishment and less repetitive farmer work make this worthwhile in play. Do not promise a substantial new management system from these results. A slower repeat cycle without another benefit mostly penalizes the player.

## What was actually tested

Run `./Test.ps1 -Orchard`. [Raw results](orchard-comparison-results.json) contain 48 twenty-minute comparisons: four routes, establishment at 180/300 seconds, repeat growth at 60/75/90 seconds, and with/without one background forager. All use eight housed residents, 24 initial berries, two existing six-tile garden footprints on the quarry map, two farmers and the same builders/logger. One plot begins complete; the second is built normally. The experiment restricts its crop orders to these two plots; the map itself is not newly restricted.

The **proxy** replaces only the growth/replant timing of selected vegetable gardens. Initial planting uses the actual farmer action. Mature plots regrow after the last harvest without resowing. Eight units per cycle, two units per collection, garden construction cost, footprint, routes, storage delivery, meals, rest, food conservation and demolition remain real existing behavior. Candidate fruit is accounted as vegetables inside this test; the game has no playable orchard or fruit resource yet. Grove age/maturity are separate experiment state, not a production save feature.

This is deliberately favorable to orchards: four logs per plot, no extra tending labor, no spoilage or per-tree walking. It does not establish final orchard costs, access rules or fruit-variety effects. It avoids inventing fertility, seasons and additional needs.

Routes are gardens only, two early groves, one garden plus one grove, and two groves followed by clearing/rebuilding one as a garden at ten minutes. Every route reassigns one farmer to building work from minute six to eight. Replacement uses real interruption, crop recovery, dismantling and construction; it does not exchange models or erase stored food.

## Results

Without background foraging, at 180-second establishment and a 60-second repeat cycle:

| Route | First delivery | Delivered by 20 min | Farmer seconds with a workplace claim | Sowing seconds | Any missed/skipped meals |
| --- | ---: | ---: | ---: | ---: | --- |
| Gardens | 77.0s | 142 | 1068.6 | 77.9 | No |
| Early groves | 197.3s | 146 | 955.3 | 8.2 | No |
| Mixed | 77.0s | 144 | 1016.6 | 49.2 | No |
| Replace half at 10 min | 197.3s | 128 | 873.6 | 24.6 | Yes |

The grove delivers four more units with about 11% less claimed farmer time. That is useful but small. Claimed time is not all useful work, and reported farmer travel also includes personal journeys; neither is a promise of construction output gained. Lower labor in a shortage route can simply mean less food production. All three unmodified routes retain their two original crop footprints.

At a 75-second repeat cycle, early groves deliver 128 and miss meals, while mixed planting delivers 140 without a miss. At 90 seconds, both miss meals. At five-minute establishment with 60-second repeats, early groves miss meals before recovering; mixed planting avoids misses but delivers only 136. This supports interim food rather than longer mandatory waits.

The background forager keeps all tested routes free of missed meals and masks most establishment risk. That is a valid complementary plan, not evidence that establishment never matters. Garden-only results are identical across the candidate timing parameters.

Clearing/rebuilding the second plot issues the new garden order about 42–84 seconds after demolition starts (42–51 without background foraging). Rebuilding completes in all cases, but physical recovery does not guarantee sufficient continuing food. Without background food, all replacement routes experience a miss; some recover recent meal reliability by twenty minutes while still failing fresh-supply coverage. The fixture's two-plot restriction deliberately leaves little headroom. A playable orchard must explain this disruption and preserve alternative food plans.

## Verification and limits

All 48 routes validate simulation invariants every tick. At ten minutes, current-world and proxy state roundtrip separately and replay 100 identical ticks, then restore the checkpoint for the measured route. Demolition/rebuilding must actually complete. The checks assert conservation, continuation and recovery mechanics; they do not assert that all candidate balances are good or that every meal is served. Game code, resource definitions, save version and campaign requirements are unchanged.

## F05c integration brief

Add a visible orchard plot using the existing Farmer role, with distinct fruit tracked through harvest, carrying, storage and actual meals. Start the playable experiment near the tested 180/60 timing, eight-unit yield and six-tile footprint; validate costs and handling again rather than treating this proxy as final balance. Keep gardens available everywhere and avoid mandatory fruit/diet quotas.

Show initial planting, immature trees, mature growth, ripe fruit and progressive picking. Explain time to first harvest, repeated harvest behavior, shared farmer availability and clearing consequences in the catalog/inspector. Food counts and reservations must remain honest; do not relabel vegetables as fruit in production code. Existing priorities, targets, pause, demolition and recovery should work. Save/load immature, ripe and carried-fruit states; test all four orientations, long-run food service and 960/1440 UI. No extra need, separate tending job, seasons or campaign level in this slice. Reevaluate after seeing and playing the orchard.
