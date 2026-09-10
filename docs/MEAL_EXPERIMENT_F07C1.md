# F07c1 — meal request experiment

Implemented as an explicitly test-only experiment under `Tests/Experiments`, invoked with `dotnet run --project Tests/SimulationTests.csproj -- --meals` using the repository's bundled .NET environment. Ordinary gameplay, production food rules, saves and campaigns are unchanged. This establishes accounting behavior; it does not ship a pantry.

## Decisions from the experiment

- One outstanding request per resident, initially staggered through 60 seconds; each request has a stable ID and a 60-second deadline. Stored claims remain in stock until collection. Carried portions count as eaten only after four seconds of eating.
- At the deadline, append one immutable service outcome: timely or missed. Uncollected requests release their stock claim. A collected portion can finish up to 30 seconds late; afterward it enters physical return, retaining cargo until the return completes. No forced disappearance or duplicate consumption.
- Late consumption immediately restores that resident's nourishment but does not rewrite the missed deadline. **Nourishment and reliable service are distinct.** Hunger is the fraction of residents currently marked unfed: missed deadlines clear their fed state, eating restores it. Initially everyone is fed, giving an explicit staggered startup grace. Retain the existing maximum half-speed hunger penalty when integrating; this experiment does not simulate it.
- Advance to the next future slot in the original cadence after a late meal or return. Record intervening slots as skipped demand, rather than queueing catch-up meals or hiding reduced demand. Skipped slots have their own due time and event time; they are not timely meals or closed request outcomes. Expose them in diagnostics and include them in reliability assessments.
- Rotate simultaneous request ordering by demand round. Balance selection among locally available food types by recent actual consumption. Every current edible type works on its own; no rule requires consuming every type. Release uncollected claims on interruption; collected portions return before retrying.

For F07c2 campaign integration, evaluate service over the trailing 180 seconds using closed deadlines and skipped demand whose due/deadline overlaps that window. Require at least two closed requests per current resident and no missed/skipped service before declaring stable service; pending requests do not count as success. Keep actual consumption/variety and fresh producer deliveries as separate proof conditions. New arrivals restart the population assessment. This replaces instantaneous-meal assumptions and must be checked on both river/lake routes; current campaign thresholds are not silently reusable. Nourishment can recover immediately while a stability window rebuilds.

## Measurements and scope

Ten-minute deterministic fixtures, eight residents, 200 initial berries, no producers or home/recreation routines:

| Fixture | Consumed | Timely / closed requests | Skipped | Outbound travel | Eating |
| --- | --- | --- | --- | --- | --- |
| All at the central pantry | 80 | 72 / 72 | 0 | 0.0 resident-seconds | 320 resident-seconds |
| Half at a distant work point | 78 | 72 / 72 | 0 | 545.5 resident-seconds | 312 resident-seconds |

The experiment obtains outbound paths from the existing World's private pathfinder through a test-only partial-class probe. The distant route takes 138 fixed 0.1-second steps at the game's normal walking/path rates. Each request assumes the resident starts at the fixture's work point again. It does not simulate return-to-work trips, contention for visit spots, changing jobs, hunger slowdown, supplier travel, pantry capacity or local routing decisions. Central zero-distance visits still have a state-transition tick. The source World remains byte-for-byte unchanged.

More portions can be consumed than requests closed at the measurement boundary because their deadlines are still pending. Report both; do not mistake this for duplicate meals or full service coverage. Compared with today's instantaneous meals, even the central fixture adds 320 resident-seconds of eating. The dispersed fixture adds about 858 seconds of measured travel/eating, before return trips or competing routines. These are a baseline and a warning about added labor, not proof that local pantries improve a working settlement.

## Verification

Focused checks passed for all five edible types, exclusive stock claims, collection versus consumption, interruption/physical return, deadline expiry while walking, late nourishment without rewriting history, bounded late return, skipped demand, shortage recovery and rotating winners under scarcity. Waiting, walking, eating, returning and satisfied snapshots roundtrip exactly and continue deterministically. Conservation is checked every step. The game project builds with zero warnings/errors and excludes the experiment files.

One initial check incorrectly expected all five foods to appear within a short recent-history balancing window. That would imply a diet requirement the design rejects. It was replaced with an independent single-food run for each type; availability and conservation, not mandatory variety, are the acceptance criteria.

## Roadmap review and next chunk

F07c1 is complete as a development experiment. **F07c2 remains the playable integration:** reuse the proven accounting decisions in World's actual task lifecycle, rather than shipping this independent scheduler. Add central/local service, producer deposits, optional hauling, capacity/claims, visit positions, models and UI together with food conservation, supper/trade, invitations, fresh saves and campaign assessments. Compare central/local/poor layouts including construction, supplier labor and actual work/home routines. Revisit timing or cut the feature if service travel mostly adds frustrating waiting.

Keep the art and human pacing reviews open. More food producers, civic needs and mastery scenarios do not gain priority from this experiment. Existing neighborhood seating and resource feedback should help assess the integrated routine; no additional mandatory needs, seasons, save migrations or population expansion are introduced.
