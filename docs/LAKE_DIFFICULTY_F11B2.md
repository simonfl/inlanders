# F11b2 — campaign choices on the lake

Implemented in the seventh campaign. The home shore now has less spare land, two nearby timber trees, one berry patch and modest shared fishing stocks. Central production competes with recreation, while the far shore offers room and timber. The campaign brief explains those constraints before investment. Human enjoyment and first-play pacing remain open; longer completion time alone is not success.

## What the original scenario allowed

Repeat with the simulation test argument `--lake-pressure`. Each comparison uses normal construction, staffing, home visits and meals, with no modified work speed or assessment duration.

The original near-dock route needed no cultivation at all: fish plus berries completed in 420 seconds, then the food reserve grew from 126 to 286 over fifteen minutes. Adding a garden still completed in 420 seconds and grew reserves from 142 to 344. Bread completed in 540 seconds and grew reserves from 188 to 436. Production was too generous to make the extra buildings a consequential choice.

An isolated supply experiment retains just one berry patch and gives each fishing ground eight starting fish and replenishment of two fish per minute. This makes the garden useful, but fish/garden and fish/bread still complete in 480/540 seconds on the broad map. Resource scarcity alone does not solve the opening-and-wait problem. The fish-only route can still eat from reserves for a long time, and eventually pass a short proof during a favorable delivery burst; a three-meal check is not a guarantee of indefinite sustainability.

## Adopted narrow shore

Keep the same eight housed residents, forty starting berries, twelve-resident objective, food rules, rest/recreation rules and three-meal assessment. Restrict spare land on the inhabited shore; retain the larger far-side land around the lake. Two timber trees remain near the village, with five elsewhere. One berry patch and the limited fishing stocks from the isolated experiment make cultivation useful. All buildings remain available.

At the untouched opening there are only four overlapping unrotated near-shore building anchors: effectively one convenient plot. More sites become usable as people move, woodland is cleared and the far shore is developed. This is an authoring observation, not a hard slot limit. Compare both orientations and actual routes when designing a village.

The first test policies select legal plots near a preferred location, without privileged materials or instantaneous clearing. They intentionally expose consequences of rough placement; they are not optimized human playthroughs:

| Approach | First catch | First expansion ready | Assessment starts | Complete |
| --- | --- | --- | --- | --- |
| Garden takes the central plot; square ends up far away | 71s | 197s | 425s | 1500s |
| Bread chain takes convenient plots; square ends up far away | 111s | 542s | 808s | 1080s |
| Reserve the central plot for a square; grow vegetables elsewhere | 71s | 363s | 426s | 600s |

Reserving the center changes the outcome substantially without adding resources or changing counters. The rough garden route spends most of its extra time failing the operational proof after construction. That is a warning to investigate, not a desirable twenty-five-minute level. A player should recognize the travel/supply problem and improve the village rather than wait for a lucky delivery streak.

## Deliberate routes, diagnosis and recovery

The rough garden route fails nine meals on fresh supply. It spends 2,650 person-seconds walking to recreation over 1,500 simulation seconds, versus 365 over 600 for the central-square route. Those are cumulative totals over different runtimes; average residents walking to recreation are approximately 1.77 and 0.61 respectively. The difference is not simply missing a service building: long service trips take workers away from production and delivery.

`LakeChecks` now uses explicit, reproducible placements for two successful routes: central recreation with a garden (600 seconds) and the bread chain (1,080 seconds). A separate test waits for two actual supply failures in the rough garden layout, then builds a nearby square when the plot is clear. It completes in **960 seconds**, versus 1,500 without intervention. A saved snapshot during recovery resumes correctly. This is the useful decision: reserve central land early, or respond to travel costs by adding a convenient venue later.

Failure feedback now shows actual fresh deliveries versus required portions and suggests checking food capacity and trips to homes/squares. The opening explains that the two grounds renew four fish per minute in total while twelve residents consume twelve portions per minute. It does not claim regeneration equals delivered output. All food mixes remain legal, and a short assessment remains an operational sample rather than a guarantee of indefinite sustainability.

The lake scenario is revised in place instead of adding another similar introduction. Save format 26 rejects earlier prototype snapshots rather than showing the new brief on an old broad lake. No save migration work was added.

Verification: `./Test.ps1`; `./Play.ps1 -LakeReviewSmokeTest` creates the completed twelve-person fixture through normal play, then checks dock/boat/UI behavior and captures the completed village at 960/1440. Review images live under `artifacts/f11b2-village-*.png`. The opening, full village, shoreline access and resource feedback have been visually inspected. These checks establish coherent behavior and readable geography, not player enjoyment.

No new needs, technologies, unlocks, quotas, longer proof windows or global producer nerfs were added. The twelve-person scale remains sufficient. Next, managed woodland can help players retain and replenish the trees: unrestricted logging currently leaves the completed lake visually bare. Keep the human pacing review open rather than multiplying unreviewed campaign levels.
