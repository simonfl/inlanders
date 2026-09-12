# F11b4 / F18b4 — quarry challenge experiment

September 12, 2026. **Experiment complete; proposed reserve-only difficulty changes rejected. The shipped campaign is unchanged.** This is simulation evidence about fixed scripted strategies, not a human playtest or proof of enjoyment.

## Question and method

Can tighter initial supplies make Built to last demand more decisions after its opening plans, without adding proof time or stone quotas?

`Test.ps1 -QuarryChallenge` runs fourteen accounted variants: seven setups × two quarry routes. Each plans the same central hall, sawmill, and either both camps or the distant camp. The near camp is planned first on the two-camp route. Spare residents become quarrier/sawyer. Assessment starts when the hall is complete. Existing simulation rules, yields, costs and assessment windows remain identical. These comparisons do not add paths; earlier path comparisons remain in the campaign implementation review.

Candidates are made by editing current-format snapshots in the experiment, adjusting material/food conservation before loading. No game factory or save file used by players changes. Each tick validates the world. Metrics include actual hall completion, campaign completion, minimum stored edible food, distinct missed/skipped requests, and a timestamped command log. Commands are a rough action proxy: the two initial role assignments are logged together; these are not mouse-click counts or measures of thinking/idle time.

## Results

Times are simulation seconds. Each cell shows **two camps / distant only**. All fourteen runs completed; all had **zero missed or skipped requests**.

| Setup | Hall finished | Campaign finished | Minimum stored food |
| --- | --- | --- | --- |
| Shipped baseline: 16 logs, 72 berries | 251 / 367 | 420 / 480 | 71 / 71 |
| 4 logs, 16 berries | 265 / 395 | 420 / 540 | 14 / 15 |
| 4 logs, 8 berries | 265 / 395 | 420 / 540 | 6 / 7 |
| Food frontier, untouched | 251 / 367 | 480 / 1620 | 22 / 13 |
| Food frontier, opening garden | 255 / 384 | 420 / 540 | 23 / 23 |
| Food frontier, opening bread chain | 320 / 397 | 480 / 540 | 22 / 22 |
| Food frontier, garden intervention at 600s if still running | 251 / 367 | 480 / 720 | 22 / 20 |

“Food frontier” retains 16 logs but starts with 24 berries, one berry bush, and no completed vegetable garden. Removed building timber is deducted from initial accounting. Its alternatives are a four-log garden at (2,0), or a four-log farm there plus an eight-log bakery at (−3,−1), with one forager reassigned to baking. The existing farmer remains available. Neither food chain is a new system.

The late-garden two-camp run finishes at 480s, **before any intervention**. It must not be described as successful recovery. The distant-only run is still blocked at 600s by fresh deliveries failing to cover eating/demand. It saves/reloads exactly, adds a garden, and completes at 720s. Its untouched counterpart eventually completes at 1620s. The delayed strategy demonstrates an acceleration after a visible service blocker, not an otherwise impossible village being rescued. There is no evidence of starvation in these comparisons.

## Decision

Do not lower shipped reserves or remove starter food buildings on this evidence. Low reserves add up to a minute to an unchanged plan. Removing food capacity can add nineteen minutes of mostly unattended assessment waiting to the distant route. Adding a garden or bakery mostly adds opening construction commands. None establishes a sustained sequence of new decisions.

The active production capacity matters more here than the initial stock buffer. That is an inference from these controlled runs, not a universal balance rule. A ready food economy with spare workers can fund the project without further adjustment. A long completion time by itself is a poor acceptance criterion.

Keep the existing two-source choice, clear recovery tools and six-to-eight-minute baseline. Keep the broader request for longer, more demanding campaign play **open**. The next campaign design should include changing settlement commitments or environmental consequences that make the player reassess production and land use. Existing wildlife/woodland interactions are a candidate: harvesting useful timber also removes habitat, and preservation/replanting/alternative food can provide recovery. Do not declare that design successful until actual routes demonstrate the consequences.

## Handoff

The next visual chunk is F23b6: give the gathering hall a distinctive civic silhouette and inviting, accessible frontage, using existing attendance behavior. This addresses the flat hall model observed in the completed quarry scene. Retain its current cost, capacity, visit timing and working access; check all four orientations, construction stages and actual visitors.

After that, the existing Living woods scenario can be developed around a real habitat/timber/food choice rather than repeating this rejected reserve experiment. Human challenge, clarity and enjoyment remain separate review questions. The experiment is repeatable through `Test.ps1 -QuarryChallenge`; it is intentionally separate from the normal regression suite because these are exploratory comparisons, not fixed balancing requirements.
