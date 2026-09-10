# Neighborhood food service — F07c2 review

The simulation suite and rendered UI groups pass. Human appeal and campaign pacing have not been established by these checks.

## What the player gains

Residents collect, carry and eat real portions. A six-log neighborhood pantry holds 24 edible portions and accepts direct producer deliveries. Existing haulers can optionally replenish it to a target or return surplus to central storage. Target zero drains through haulers while direct deposits and resident meals continue. Grain stays central; there is no local-to-local redistribution.

Food already reserved for meals or hauling is unavailable to another claimant. Transfers and returned meals do not count as fresh production. Supper uses unreserved central bread; reducing a pantry target can bring local bread home. Food reserves count stored portions, while service assessments count actual eating and closed deadlines. New arrivals restart an active population assessment.

## Matched working villages

Run `--pantry-layouts` in the simulation test project. The fixture creates its world from source, without a saved-game dependency. Both layouts use the flat, dry Three clearings outline, eight residents, 96 starting berries, four vegetable gardens, four cottages, two seating gardens, two loggers/builders and four farmers. All buildings require normal construction. Compact neighborhoods cluster around the center; dispersed neighborhoods occupy opposite clearings. Each run lasts 1,200 simulation seconds, including startup.

Local service adds two pantries near the neighborhoods. Awkward service places them away from daily destinations. Both use target zero and no hauler, testing direct deposits. The hauler variant uses local pantries at target 12 and transfers one builder to hauling after all construction completes. Population, food producers, initial stocks and duration remain matched. This is a garden-only production economy after starting berries run out.

Times below are simulation seconds; travel and hauling are summed resident/worker seconds. The underlying result also records all timber/construction work (including continued logging after construction), eating time, rest/recreation, inventory by location and closed service outcomes. It is not a construction-only labor estimate. First-run JSON called that aggregate `SetupWorkerSeconds`; the source now names it `TimberAndConstructionWorkerSeconds` to avoid that ambiguity.

| Layout / service | All buildings ready | Construction logs | Vegetables delivered | Meal travel | Producer delivery travel | Hauler time | Rest / recreation visits |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Compact / central | 317 | 48 | 288 | 334 | 626 | 0 | 43 / 112 |
| Compact / local | 423 | 60 | 268 | 343 | 575 | 0 | 41 / 104 |
| Compact / awkward | 450 | 60 | 288 | 386 | 626 | 0 | 43 / 111 |
| Compact / hauler | 423 | 60 | 278 | 324 | 551 | 395 | 39 / 106 |
| Dispersed / central | 536 | 48 | 162 | 771 | 770 | 0 | 34 / 88 |
| Dispersed / local | 638 | 60 | 190 | 639 | 610 | 0 | 36 / 90 |
| Dispersed / awkward | 676 | 60 | 164 | 789 | 738 | 0 | 35 / 91 |
| Dispersed / hauler | 638 | 60 | 196 | 640 | 599 | 367 | 36 / 89 |

All eight runs had zero integrated hunger and 152 timely closed requests, with no missed or skipped requests. Pending requests at the end are not counted as completed service. They sustain residents, but do not establish a benefit from every pantry.

**Keep pantries optional.** Compact central service is a good choice. In the dispersed fixture, local direct deposits add 28 delivered vegetables and save about 292 combined meal/delivery seconds despite extra construction. A dedicated hauler adds only six deliveries over that direct-deposit version while consuming 367 worker-seconds. Awkward empty pantries are recoverable wasted investment, not a radius bonus. These results support a situational six-log building, not a universal recommendation or a precise payback period.

## Cost of making meals physical

Run `./Tests/CompareInstantMealBaseline.ps1` to extract the pre-integration simulation from commit `634095b` into an isolated artifact project and run the same central-only placement, roles, stocks, tick size and duration. It uses instantaneous meals. This comparison includes the routine's downstream effects; it is not a frame-rate benchmark.

| Central-only layout | Earlier buildings ready | Physical buildings ready | Earlier / physical deliveries | Earlier / physical rest visits | Earlier / physical recreation visits |
| --- | ---: | ---: | ---: | ---: | ---: |
| Compact | 282 | 317 | 280 / 288 | 44 / 43 | 113 / 112 |
| Dispersed | 436 | 536 | 168 / 162 | 39 / 34 | 93 / 88 |

Physical eating adds roughly 650 resident-seconds in each current run, plus 334/771 seconds travelling for meals. Together that is approximately 10%/15% of the eight residents' available time. Crop-cycle boundaries and time previously spent waiting mean output does not decline proportionally. Construction and home life still incur a real cost, especially in the dispersed village. Local food service can reduce that cost; it does not make the new routine free. The visual benefit of meal journeys remains a player judgment.

## Campaign and recovery findings

Both river routes pass actual service, variety and fresh-supply assessments: cottage/garden at 600 seconds and lodge/bread at 900. Shortages, extra residents and rebuilding a neighborhood remain recoverable. Current lake checks pass the garden route at 600, the bread route with added nearby seating at 1,440, and the rough-layout nearby-square recovery at 840 seconds. These are scripted timings, not human play durations.

Tracing the lake's distant square showed residents spending 20–31 seconds reaching recreation, six seconds attending, then 23–32 seconds returning for meals. A nearby seating garden addresses that conflict more effectively than the tested remote pantry. The earlier saved-snapshot experiment found some pantry benefit but still intermittent missed service; its exact timings predate the final claim ordering. Use the self-contained layout comparison above for current quantitative pantry claims.

Campaign guidance should explain missed trips, supply and alternative service locations. Keep the one-minute request cadence and recoverable hunger for this prototype; do not silently extend deadlines to make a poor layout pass. Do not require pantry ownership. F07c3 remains human review before more food-access scenarios.

## Verification and remaining acceptance

The full simulation suite passes, including all five foods through meal phases, reservations, capacity, exact saves, interruptions, pantry draining/demolition, actual producer deposits without haulers and closing a pantry during an incoming delivery. Read-only resident feedback and actual-source links have focused rendered coverage at 960/1440. The new tests distinguish consumption, pending requests, late nourishment, missed service and skipped demand; simultaneous scarce claims rotate, and food choice accounts for already-promised portions.

The broader HUD groups pass across the main run through directory navigation and the focused `./Play.ps1 -HudServicesSmokeTest` group for food, leisure, decoration, happiness, homes, coverage, demolition, camera views and visitors. `./Play.ps1 -PantrySmokeTest` covers actual pantry controls, resident/Economy location links, paused inspection and reload at 960/1440, then profiles a normally built 16-resident village with two pantries and 36 decorations.

That profile measured 49.1 ms median / 51.3 ms p95 paused, 39.4 / 49.8 ms during actual meals/hauling at 1×, and 46.2 / 51.8 ms over a 600-frame paused pantry preview. The preview preserved the exact save and node count. These are different sampled scene states, not proof that simulation improves frame rate. Rendering remains constrained; this fixture is not directly comparable to earlier differently populated art/wildlife scenes. Do not expand population on the strength of this result.

Next is F07c3 human review: can the player recognize a missed meal's cause and improve the village without feeling forced to build pantries? Keep art acceptance and human river/lake pacing open. Do not add another food chain to compensate for an unreviewed routine.
