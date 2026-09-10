# F24a: building investment, first measured pass

Baseline: `e53735c`. The first pass changes construction investment and shares building definitions between the simulation and UI. **F24 is not finished:** food throughput and stockpile payoff remain open in F24b. These are deterministic layout experiments, not human playtests or proof of enjoyment.

## Adopted values

| Building | Before | Now | Reason |
| --- | --- | --- | --- |
| Cottage | 6 logs | 6 logs | Keep basic two-bed housing stable. |
| Forager hut | 6 logs | 4 logs | Low investment while its main role is enabling two foragers. |
| Farm | 6 logs | 4 logs | Put more investment in processing than in preparing a field. |
| Vegetable garden | 6 logs | 4 logs | Make direct-food cultivation affordable to try. |
| Bakery | 6 logs | 8 logs | Farm + bakery still costs 12 logs altogether. |
| Sawmill | 6 logs | 6 logs | Keep the processing entry cost stable. |
| Lodge | 8 planks | 12 planks | Three whole batches; later housing saves timber and land but requires processing. |
| Village square | 6 logs | 6 logs | Keep social space accessible; no new civic tax. |
| Stockpile | 6 logs | 4 logs | Lower the cost of trying local storage; this alone does not establish its payoff. |
| Bridge | 6 logs | 6 logs | Its value depends on geography, not its one-tile footprint. |

Sawmills now target **12 planks**, enough to prepare a lodge before placing its plan. The target derives from the lodge cost, rounded to whole four-plank batches. Building footprints, housing capacities, staffing slots, recipes, growing times and 12-second construction work are unchanged. No save migration is provided.

`Simulation/Buildings.cs` owns names, costs, construction materials, beds, worker roles/slots and construction duration. Construction reservations, completion, housing, catalog prices and placement affordability use these definitions. This is a small built-in catalog, not a modding system.

## What was measured

Run `Test.ps1 -Balance` to reproduce the current experiments in `Tests/BalanceExperiments.cs`. Baseline comparisons used the same code/layouts with the original six-log prices, eight-plank lodge and eight-plank stock target, then restored the adopted definitions. Local raw logs are `artifacts/f24-baseline.log` and `artifacts/f24-candidate.log`.

**Campaign:** identical scripted layouts and staffing, all five lessons complete and survive save/load. Completion times (simulation seconds):

| Lesson | Before | Now |
| --- | ---: | ---: |
| 1: food and homes | 124.7 | 122.6 |
| 2: bread | 306.2 | 295.7 |
| 3: lodge/woodland | 182.0 | 221.7 |
| 4: supper | 317.1 | 317.1 |
| 5: vegetables | 314.6 | 302.7 |

These remain introductory lessons. The new campaign chapter must earn its length through decisions; this price pass is not an attempt to stretch the tutorial.

**Housing eight:** four cottages finish in 67.7s in both versions, using 24 logs and 24 building tiles. A mill and two lodges finish in 170.4s before and 225.8s now, using 18 tiles. At completion, raw logs already spent on construction/processing rise from 16 to 20; four spare planks remain, including carried planks. Required building investment alone is 14 → 18 raw logs. The experiment lets sawyers keep preparing stock, so extra inventory must not be mistaken for a building cost. Current builder-active time is 201.0 person-seconds plus 189.4 sawyer-active seconds, versus 167.6 builder seconds for cottages. Active time includes travel; populations have ample food to isolate housing work.

For the **first** lodge with a new mill, required raw timber is now 12 logs, matching two cottages; both occupy 12 building tiles. Later lodges cost 1.5 raw logs per bed against cottages' 3, with processing labor. This is a clearer speed-versus-expansion tradeoff, not an across-the-board housing upgrade.

**Food:** each run starts with campaign-2 homes/hut and 96 food, two loggers, two builders, two food workers and two unassigned residents. Gardens/bread require new construction; berries have their hut ready. 9,000 fixed 0.1-second ticks (nominally fifteen minutes), fixed placement, no intervention or paths. Floating-point meal boundaries result in fourteen meals during this run:

| Setup | First delivery before → now | Delivered food before / now | Current reserve at 5 / 15 min |
| --- | --- | --- | --- |
| Two foragers | 9.5s → 9.5s | 338 / 338 | 178 / 322 |
| Two gardens, two farmers | 131.8s → 121.0s | 90 / 90 | 86 / 74 |
| Three gardens, two farmers | 140.3s → 121.0s | 120 / 120 | 88 / 104 |
| One farm, one bakery, farmer + baker | 145.1s → 134.3s | 66 / 66 | 80 / 50 |
| Two farms, one bakery, farmer + baker | 168.3s → 155.4s | 64 / 64 | 78 / 48 |

No run experienced hunger because the initial reserve covered deficits. That does **not** make the declining-reserve setups sustainable. Cheaper fields improve startup, not throughput. In this layout three gardens roughly replace the eight-person food demand; bread does not. The extra field is distant and does not improve this chain, which warrants inspecting grain waits and delivery travel rather than simply speeding crops. Foraging is strong with these ready, reachable bushes; different geography remains untested.

**Storage:** three-house construction, two loggers/two builders, plentiful starting food, with stockpile construction included. The dedicated-hauler case uses one additional worker. Stockpiles retain their default six-log target.

| Layout / storage | Completion before → now | Current builder travel (tiles) | Current hauler-active seconds |
| --- | --- | ---: | ---: |
| Near, yard only | 86.3s → 86.3s | 145 | 0 |
| Near, pile without hauler | 101.9s → 99.8s | 161 | 0 |
| Near, pile + hauler | 94.7s → 95.3s | 133 | 38.8 |
| Far, yard only | 129.8s → 129.8s | 289 | 0 |
| Far, pile without hauler | 157.4s → 149.6s | 333 | 0 |
| Far, pile + hauler | 143.0s → 132.8s | 279 | 95.9 |

The cheaper pile is easier to try, but it does not beat yard-only startup here. Lower builder travel is not a net labor saving when a hauler supplies it. The near-hauler run also shows that an earlier building can alter job timing without reducing final completion time. Longer-lived remote work, local timber harvesting and amortized supply need separate measurement.

## F24b: still required

1. Measure baker grain-wait time, baking time, output-delivery travel and deliveries during minutes 5–15, at current and nearer bakery locations. Test carrying the complete four-loaf batch in one trip before changing recipes or growth. Keep physical cargo, interruption and saves correct.
2. Establish whether bread has a defensible labor/land payoff beside gardens and foraging. Do not infer that from a larger number of loaves per recipe or the supper requirement alone.
3. Test whether a well-placed stockpile repays **total** setup and hauling investment over a realistic remote neighborhood build. Improve or simplify its behavior if it still does not; do not manufacture value by worsening the yard.

The original F24 acceptance remains open until these questions have evidence. Production controls and local food/plank storage remain F21h/F07b unless these experiments justify a smaller, specific intervention first.
