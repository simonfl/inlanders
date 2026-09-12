# F07d1 — local stone route screen

Status: design evidence, not playable local storage. Current stockpiles still accept logs/planks only. Run `Test.ps1 -StoneStaging` for the twelve-row report at `artifacts/stone-staging/routes.md`.

The screen uses the real authored quarry map, legal hall/stockpile footprints, orientations and path-cost routine. It considers central and distant projects, both deposits, and twelve nearest legal candidate placements/orientations around each of the source, project and yard. These are representative candidates, not an exhaustive optimum.

The baseline has no extra stockpile; local routes include its blocked footprint. Both use two-unit transport round trips. The optional pile adds four logs and two corresponding yard-to-pile hauling round trips. Quantities respect the finite deposit: eight stone from the near outcrop, twelve from the far outcrop. The near outcrop cannot complete a twelve-stone hall by itself.

## Findings

All values below are weighted path units, **not seconds or measured worker productivity**.

| Route | Central transport | Local transport + setup travel | Implication |
| --- | ---: | ---: | --- |
| Distant outcrop → distant hall, pile near project | 2,880 | 720 + 400 = 1,120 | Strong reason to prototype avoiding a central-yard detour. |
| Near outcrop → distant hall, pile near project | 1,400 | 1,160 + 260 = 1,420 | Staging is not automatically useful; setup can outweigh route savings. |
| Near outcrop → central hall, pile near project | 760 | 360 + 160 = 520 | Smaller potential saving that may disappear once construction/work allocation is included. |
| Distant outcrop → central hall, pile near source | 1,920 | 1,080 + 480 = 1,560 | Quarrier convenience alone does not determine the best system-wide placement. |

The screen omits construction time, logging opportunity cost, worker starting positions, meal/rest trips, capacity contention and hauling policy. It assumes workers can use the proposed pile; that behavior is not yet implemented. Baseline snapshots remain exact after all comparisons.

## Decision: prototype F07d2

Extend the existing material-store abstraction to stone, with a stored-stone field, finite capacity, actual quarry deposits, builder pickups and optional haulers. Reuse existing stock targets; explicitly test direct deposits without a hauler and a target/hauler case that may add unnecessary transport. Do not force a warehouse into any campaign goal.

Compare actual normal-play runs for the strong distant/distant case and weak near/distant case. Include the pile's four-log cost and construction, initial resources, workforce, completion time and travel by role. Keep matched layouts otherwise unchanged. A prototype passes the design gate only if it preserves a useful optional choice; geometric savings alone cannot justify a performance claim.

Integration must cover local/global/central stock distinctions, capacity reservations, material switching, interruption, demolition/Creative removal, relocation, area removal, Creative stock setup, readable stone stacks and saved continuation. Existing log/plank and quarry conservation checks remain required. No new stone consumer is needed.
