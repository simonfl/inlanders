# A lasting village — decision prototype

September 12, 2026. F11b5 / F18b5. **Go with conditions:** the existing systems support a later commitment worth integrating. This is a developer prototype, not selectable level ten. Nine campaign levels remain playable. Automated routes establish feasibility, not human enjoyment or first-play duration.

## The settlement and its decisions

Eight residents occupy four cottages in a cramped western village with a forager and vegetable garden. A one-tile channel separates a larger eastern neighborhood. The opening investment is a bridge and two homes, followed by growth to twelve. Growth to twenty makes those convenient central home sites compete with production and services.

Two tested food arrangements work: relocate the opening homes east and reuse their central plots for gardens; or retain them initially and establish eastern gardens with a neighborhood pantry. Neither route requires a particular food mix. All buildings remain available.

The later commitment is a shared supper: twenty people require forty loaves available centrally, in addition to keeping everyday meals running. One remote farm/bakery produced no reserve after another 600 seconds. Moving grain production centrally with only one bakery also failed to build surplus in an exploratory run: bread was being eaten. The successful investment frees central space for a farm, two bakeries and a small recreation garden, with two bakers. These are example solutions, not mandatory building objectives. Both tested food routes converge on this bread arrangement; broader bread alternatives remain untested.

The recovery route physically dismantles its remote farm/bakery, recovers materials, reloads, and builds the central arrangement. This changes both placement and capacity; the result cannot be attributed to either alone.

## Budget and layout

The authored footprint contains 485 land tiles in a 36×22 bounding area. The channel lies at x=6; the working village is west and the expansion land east. Exact cells and geometry are in `Simulation/LastingVillageMap.cs`; placements and role changes are reproducible in `Tests/FinaleDecisionChecks.cs`.

Initial timber totals 168 logs: 32 already invested in buildings, 16 at the yard, 24 standing west and 96 standing east. Initial food is 32 berries, with one replenishing berry source. There is no starting bread, grain, plank or stone stock. Existing roles are one logger, two builders, two foragers, one farmer and two unassigned residents.

The opening bridge and two cottages cost 18 logs. The central food expansion costs 54 gross and recovers 12 through home demolition; the local expansion costs 48. The later central route costs another 30 gross and recovers 14; the local route costs 42 and recovers 18. Thus the final building investment is 108 logs centrally or 122 locally. The poor remote chain temporarily ties up another 12 logs, recovered through actual work. The material budget leaves room for alternatives; labor, location and disruption are the intended constraints.

## Measured routes

Seconds from settlement creation; tests advance the actual simulation at 0.1-second steps.

| Route | Twelve fed | Twenty supported | Supper finished | Later support diagnostic |
| --- | ---: | ---: | ---: | ---: |
| Central gardens, staged | 247 | 680 | 1584 | 1699 |
| Eastern pantry/gardens, staged | 247 | 669 | 1789 | 1906 |
| Eastern layout, prebuilt, late baker staffing | 1156 | 1298 | 1719 | 1836 |
| Eastern layout, prebuilt, early baker staffing | 1166 | 1309 | 1849 | 1967 |
| Eastern layout, remote bakery then recovery | 247 | 669 | 2337 | 2457 |

The twelve-person probe checks reliable meals and fresh supply. Twenty-person support additionally checks recent home rest for at least three quarters of residents and recreation for at least half. The post-supper support check is diagnostic, **not another proposed completion gate**. Supper consumes forty bread and genuinely gathers the residents; save/reload during gathering continues to completion.

An earlier population-only experiment reached supported twenty in 458 seconds by prebuilding versus 669–680 seconds staged. That experiment did not include the later civic investment and is not directly comparable to the prebuild rows above. It demonstrates why population alone is insufficient as the finale. Planning ahead should still be rewarded; these routes do not prove an optimal strategy or a minimum duration.

## Presentation and acceptance conditions

[Initial overview](images/finale-initial.png) and [recovered working center](images/finale-recovered.png) show recognizable architecture and neighborhood growth. The initial east bank is visually sparse, with a regular tree perimeter and strongly geometric ground. Before presenting this as a polished finale, author a more convincing landscape composition while preserving the tested budget and approach space. No terrain sculpting system is needed for that pass.

Next address a concrete explanation problem: production deliveries are not stored reserves, bread may be eaten, and local pantry contents are not necessarily available for the central supper. Trace this in existing Economy, workplace, resident and supper controls before adding UI.

Then integrate earned twelve-person progress, supported growth to twenty and a player-triggered supper. Prevent an early eight-person supper from satisfying the finale. Do not lock building availability or require a prescribed farm/bakery count. Define additional-resident behavior, preserve earned milestones and test replay, phase saves and 960/1440 guidance. Human pacing and whether the later rebuilding feels rewarding remain open questions.

## Reproduction

- `./Test.ps1 -FinaleDecision`: five complete routes, physical recovery, validation and exact save roundtrips; fixtures under `artifacts`.
- `./Play.ps1 -FinalePrototypeSmokeTest`: builds, repeats routes, renders five states at 960/1440 plus central views and checks paused-state isolation. Passed; this does not test campaign integration.

No production rules, costs, save format or selectable campaigns change in this prototype.
