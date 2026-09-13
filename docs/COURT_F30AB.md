# F30a/F30b — arrange a place and follow its daily life

September 13, 2026. One combined playable outcome, **checkpoint 21**. The independent presentation review is required before selecting subsequent work. [Observation instructions](COURT_OBSERVATION.md).

## Delivered comparison

**Settlements → New Willow court** opens an inhabited court without Goals covering it. Four existing homes face connected useful ground around the meeting place. Trees frame the settlement and no longer obscure the eastern garden plots; surrounding ground replaces the exposed board sides. Joined path patches avoid the former checkerboard gaps. Labels are quiet in ordinary court play. This reuses the landscape context seam, not the entire optional Storybook toggle or a replacement building catalogue.

**New Willow inlet** retains the dispersed comparison. Both starts have eight residents, four cottages, three vegetable gardens, one seating garden, 24 central berries, 24 locally stored vegetables and 12 available logs. Timber quantities are preserved while conflicting/view-obscuring trees are relocated. Home/venue positions, paths, tree positions, resident starting positions and presentation differ deliberately. This is a combined experience comparison, not an isolated balance or art A/B.

In the court, click a resident or home to follow daily life. The small card shows an actual claimed meal, the current activity delaying collection, no available food, or unreachable food. Gold follows the actual remaining route. Between requests, blue shows a currently available reachable source, explicitly **not reserved** and not promised as the next destination. Completed eating and next-request timing are visible. Details and food inspection remain available. The card chooses a screen corner that avoids the selected resident/destination where possible; it cannot guarantee a clear whole village at every density.

Try one finished building in another position using existing relocation controls and four orientations. A resident card offers their home directly; other buildings expose the trial in their inspector. One building can be tried repeatedly until restored; restore it before trying another. The trial's original location/orientation persist in saves. Goods, identity and improvements persist through moves. Restoring returns the building, **not time, consumed food, ongoing jobs or removed paths**. Occupied/disconnected return sites are rejected with a reason; keep the original footprint clear. Trial buildings must be restored before demolition. Normal construction elsewhere remains unrestricted.

Welcoming eight newcomers remains available in Goals as an optional milestone. No new score, resource, need, forced crisis or completion certificate was added. The expanded fixture uses real construction/invitation to reach the supported sixteen-resident court, not a forged 32-resident neighborhood.

## Rule clarity and reliability

Shared-work Economy advice now has its own read-only report derived from food, material orders and existing workplaces. A transient hauler no longer demands a stockpile; a farmer with an orchard does not imply a missing farm. Comfort and construction waiting messages no longer ask automatic workers to adopt legacy roles. Legacy employment rules remain unchanged outside shared work.

The journey reader handles food carried after its former source is removed and identifies central storage when returning an uneaten portion. Current saves are format **45**, including the arrangement trial. No migration or personal-save preservation work.

## Observed outcomes

Each opening arm was observed for 600 simulated seconds, without invitations or a compulsory intervention:

| Arrangement | Hungry resident-seconds | Actual meal-route resident-seconds | Final food |
| --- | ---: | ---: | ---: |
| Dispersed inlet | 0 | 142.5 | 88 |
| Court | 0 | 47.4 | 88 |
| Court, one garden relocated west | 0 | 60.1 | 88 |
| Court, all producers paused | 1230.0 | 174.1 | 0 |

The court has shorter measured meal trips in this opening, but the extra garden move does not improve that measure. This is not proof of a perceptible benefit or a harder level. At eight residents production is generous; central storage contained 77 vegetables at the end of the court arm. Producer overflow can feed the center. An initial UI observation wrongly expected the selected resident to collect from a garden; real claims continued to use central storage. That assertion was rejected, rather than manipulating food to manufacture its expected trip. The final probe records whichever longer collection actually occurs, or explicitly records none.

The preserved weak sixteen-resident layout received a same-age control. Both worlds ran through the repaired arm's construction time and 300-second settling period before observing another 600 seconds:

| Arm | Observation start/end | Hungry resident-seconds | Meal-route resident-seconds | Final food |
| --- | --- | ---: | ---: | ---: |
| Unchanged weak layout | 1472.99–2072.87 | 511.9 | 1173.7 | 1 |
| Added western garden | Same | 0 | 661.0 | 9 |

This strengthens the causal case for that intervention compared with the previous unmatched-age recovery. It does not isolate every scheduling/travel cause or prove lasting equilibrium. The paused case's hungry-state transitions report no free food; the weak case includes shortages and collection/eating transitions. Trace samples identify situations to inspect, not a complete causal proof.

## Tooling and checks

- `--court` checks matched initial capabilities, legal/rejected relocation, one-building limit, restore, saved continuation and role-independent advice, then compares opening arms and same-age recovery. Run `--inherited` first for the weak fixture. `--court-layout` reruns changed opening arrangements without repeating unchanged recovery. `--court-reader` covers removed food sources, returns, read-only reporting and next-meal timing.
- Existing `--inherited` five-arm routes, `--relocation`, and `--welcome-meal` (including all six edible pantry resources) pass. Builds have zero warnings/errors.
- Rendered scripted court journeys pass at 960/1440: actual resident click, trial placement/restore, save/load, claimed food/eating, Continue and reset. Precommit runs: `20260913-170730-721-willow-court-c45cfb` (960, 39.9s), `20260913-170608-504-willow-court-4d5365` (1440, 37.39s). The later removed-source guard is covered by the focused reader check; these earlier images retain their own provenance.
- Per-resident transition traces in `artifacts/court/*-trace.json` sample every 0.5 simulation seconds, recording request/due/source/task/remaining route/carried food/nourishment and diagnosis. This is a bounded extension of the current report, not exact event timing or replay. Metric extracts are committed alongside this report.
- Build preflight in Review/Play checks binary contention before MSBuild copy retries. It detected the live simulation test's locked DLL and later permits rebuilding after completion. It stops no process. This is a useful fail-fast check, **not a race-free scheduler or owner-process inventory**; builds and captures still need sequencing. Expected savings are avoided retries, not a measured long-term ROI.
- Reused launcher scenarios: `willow-court`, `willow-court-expanded`, and `willow-dispersed` (same inlet generator, Goals closed for visual comparison). Existing capture/movie tools remain the observation path.

No uncoached human session, actual listening, current native frame profiling or full-campaign replay was performed. Simulation outcomes and scripted clicks do not establish appeal or comprehension. Presentation review decides whether the candidate should progress, be revised or be discarded; it must not simply endorse a completed implementation.
