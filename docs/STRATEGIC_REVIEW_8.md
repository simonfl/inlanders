# Strategic reset — make a neighborhood worth inhabiting

September 12, 2026, at playable checkpoint 8. Reviewed commit `aea4dc40e49d2b88ed25723dce33c1318cb8e605`; existing assembly SHA-256 `525634AE57CD9C88EE49ED1078AF5666488A40A411D2C283720C6002FE619B1F`. This review and plan do not advance the playable count.

## Verdict and decision

The game has a useful physical settlement simulation, but the present campaign is not a convincing realization of a peaceful game that is satisfying to arrange, manage and watch. Too much progression consists of placing specified buildings and satisfying service-accounting conditions. The view often gives more prominence to explanations than to village life. More features, higher quotas and isolated visual details would reinforce that direction.

**Refocus on creating visibly functioning neighborhoods.** Build one substantial playable river alternative before expanding or polishing the existing campaign. Local production, workplace interaction, progression and whole-scene appearance are all part of the experiment. The alternative must let players predict consequences, make competing spatial choices and enjoy an observable payoff. Calling the current assessment a festival would not count as a redesign.

This is an authorized design bet, not a finding that the alternative is already fun. Keep the old river experience available as a baseline until comparison; Git retains the rest. Do not migrate saves or preserve obsolete systems merely to avoid replacement work.

## Evidence and review limits

The [role reports](STRATEGIC_REVIEW_8_ROLES.md) preserve findings and evidence boundaries. Design, UX and development lead were independent read-only passes. The service rejected new agent threads; UX was reused for playtest and design for presentation. Therefore the run covers five disciplines but does **not** fulfill five fresh independent contexts. Future scheduled reviews still require independence. This limitation does not prevent a reversible comparative prototype.

Source establishes the central grain route and campaign predicates. Prior [native opening/river observation and finale experiments](CAMPAIGN_REVIEW_F11D.md) establish possible progress and layout consequences. Reviewers viewed ordinary, dense, lake, river and title stills; most predate the fixed source commit and are labeled historical. The runtime changes since those composition views were not a new whole-scene art pass. No current-build visual equivalence is claimed from that fact. No listening or new performance result establishes sound quality or the cause of reported stalls. The playtest report separately records this run's access outcome. Human enjoyment and comparative preference remain unknown.

## Why the findings require structural work

- **Agency:** the river's east-bank Square qualification excludes other recreation venues; assessment initiation and recent-history rules are visible player work. A functioning neighborhood is more recognizable than two closed meal requests and a dominant-food percentage.
- **Spatial logic:** `Simulation/Food.cs` sends grain pickups through `YardAccess`. The finale comparison found central bakery arrangements successful where tested eastern ovens failed within the same horizon. This is real depth with an unintuitive organizing rule. A tooltip can explain it; only a rule experiment can decide whether it belongs.
- **Campaign:** five opening recipes and repeated later assessment structures are not ten distinct experiences. Longer waits are not a substitute for decisions, recovery and consequences.
- **Buildings and costs:** the full 18-building set includes real geographical choices, but some distinctions rest mainly on capacity or memory intervals. Most sticker prices are small log amounts; staffing, land and trips often dominate. Audit those opportunity costs in the slice before repricing everything.
- **Presentation:** current buildings contain depth, but roofs dominate, inhabitants are small, and rigid land/shore slabs weaken the sense of place. The title's stronger composition shows useful potential; it does not validate another prop pass across the old assets.
- **UX:** the observed compact river view divides the screen between long Goals and inspector explanations. The rules create much of that text. Default interactions should answer what this place needs and what a proposed change will do.

## Keep, remove and replace

| Existing work | Decision and reason |
| --- | --- |
| Godot/C#, independent simulation, current-format snapshots | Keep. They allow inexpensive rule experiments without an engine rewrite. |
| Physical construction, goods, travel, crops and material recovery | Keep in the alternative. They make layout and recovery visible rather than abstract bonuses. |
| Crossings, scarce land, woodland/resource geography | Keep as sources of understandable competing commitments. Do not require every resource in one level. |
| Creative arranging, camera controls, pause and fast play | Keep. They support expression and inspection; no immediate new Creative feature queue. |
| Manual assessment phases and rolling-window victory language | Remove from the alternative. Retain internal counters only where useful for diagnostics and comparison. |
| Arbitrary mandatory venue identities and central bread-only ending | Replace with a communal payoff reflecting the actual neighborhood and food strategy. Exact event rules are defined with the slice, not by reusing old quotas. |
| Central-only grain pickup | Replace experimentally with physically carried local supply. Preserve travel, stock reservation and contention; no teleportation or free output. |
| Profession/People panel as the ordinary staffing entry | Replace in the slice with worker slots at selected buildings and a clear shared-worker default. Advanced named preferences earn retention through use. |
| Eighteen-building teaching obligation | Cut. Keep all available as currently requested, but build objectives around situations rather than coverage. Group recreation variants conceptually; test which deserve separate economic roles. |
| Comfort workshop/timer advantage and additional needs | Hold expansion. Test comfort as visible household improvement only if it serves expression or a consequential choice. No new mandatory needs. |
| Existing ten-level campaign structure | Freeze expansion. After the slice, consolidate introductions and replace repetitive later objectives with distinct situations. No automatic conversion of every existing level. |
| Building-by-building detail passes | Replace as the next art approach with a whole playable scene treatment. Existing models are reusable material, not protected output. |

## Alternatives considered

1. **Neighborhood settlement — chosen for the next experiment.** Local work, limited land, growth commitments and visible communal life preserve management while aligning consequences with the world.
2. **Explicit logistics puzzle.** Retain strong hubs and demanding constraints, expose flows, and author tight optimization puzzles. Coherent, but less aligned with the user's wish for an appealing village to arrange and watch. Revisit if locality removes the only interesting decisions and the user prefers harder logistics.
3. **Peaceful village-making.** Greatly reduce survival/labor administration and focus on expression, routines and optional ambitions. A serious fallback if spatial management still adds chores rather than satisfaction; it needs stronger presentation before a fair trial.
4. **Continue the present design with clearer panels and more detail.** Rejected as the main path because it leaves the strongest objections intact.

Reviewers broadly agree on the first direction. Open tensions remain: local supply can trivialize logistics; automatic staffing can remove deliberate labor tradeoffs; a more authored landscape can reduce freedom or become costly to produce. The experiment must expose these risks rather than assume they are solved.

## One fair playable slice

Use a west-bank hamlet and two credible eastern development locations. Ask the player to establish a welcoming neighborhood, with one meaningful commitment to arrivals instead of repeated pair invitations to hit thresholds. Allow at least two economic/layout approaches, including a local grain/bakery chain and a contrasting directly edible food route. Keep all buildings available; do not require the full catalog.

Residents really arrive, occupy homes, work, carry food, eat and gather. The shared place is an observable destination with a concluding communal event. A concrete recoverable shortage must arise from an arrangement or commitment, and at least two sensible responses must work. Avoid a hidden compulsory pantry/service-radius stamp. Detailed stock and service diagnostics remain optional inspection.

The visual treatment covers the whole neighborhood: land/shore shapes, vegetation grouping, building masses, work yards, communal space, lighting and camera framing. Prefer an authored storybook landscape for the first alternative; a deliberately crafted tabletop toy is a credible art counterproposal if cheap to compare. Neither procedural geometry nor imported art is mandatory. Choose authoring methods by iteration cost and result.

Compare current and alternative rules with the same starting resources, people and geography where meaningful; record intentional differences. Compare current/alternative presentation on the same running state where possible. This avoids attributing every improvement to the single local-supply rule when several parts changed together.

Observe placement predictions, meaningful decisions after initial construction, ability to explain shortages, successful recovery, panel dependence and voluntary watching/improvement after success. Separate wall time, simulation time and tool overhead. Use 3×/6× for waits and 1× for movement/sound judgment. Include a less favorable layout, not only a scripted winning route.

Reject or revise the bet if the player still follows an obvious recipe and waits; all adjacent layouts succeed automatically; only one secret arrangement is viable; the new event conceals the same assessment; or the view is attractive only in a staged screenshot. Completion speed and test passes cannot select the more enjoyable version. Offer the user concrete A/B saves or launch commands for preference feedback; silence is not acceptance.

## Tooling decision and delivery order

Adopt **T01: named scenarios plus a minimal capture/provenance bundle first**, then **T02: a narrow shared comparison runner alongside the experiment**. Existing engine PNG capture, mixer WAV export, fixtures and saved-baseline comparisons are reused. Separate build/preparation/inspection/checks, refuse stale fixture reuse, and expose ordinary controls at a paused review state. Record actual savings before broadening the tooling. See the [investment assessment](ITERATION_TOOLING_REVIEW.md) for effort estimates, beneficiaries and measurements.

The [next chunks](NEXT_CHUNKS.md) are T01, the complete neighborhood gameplay alternative F27a, the whole-scene presentation alternative F27b, comparison/direction decision T02, and evidence-led campaign restructuring F27c. T02 preparation can accompany F27a; it is not an excuse to build a generic framework first. Regular checkpoint-ten review still occurs before further delivery, even if the comparison remains unfinished.

The prior F21v placement feedback can be incorporated where the new placement interaction needs it. Audio audition accompanies presentation; native stall attribution becomes a prerequisite only if stalls compromise the experiment. Neither is considered solved or removed. Do not execute the old patch queue by inertia.
