# F25a — homes, rest and recreation

Implemented first slice. Residents have stable homes and actual rest visits; the player can improve access and service participation using existing cottages, lodges and squares. No additional buildings or population growth in this slice.

## Design review before implementation

- **Choice:** keep stable households near useful routes, or move a resident into a vacant home closer to their work. A compact neighborhood should leave more time for production and public life than dispersed housing.
- **Scope:** a short visible rest at home between jobs, with staggered starts. This is an occasional home routine, not synchronized nightly sleep. A day/night schedule remains TBD.
- **Initial tuning:** six seconds resting, about three minutes between completed visits, recent-rest credit for four minutes. Travel is part of the cost; existing hunger speed rules still apply. No fatigue/productivity multiplier.
- **Overlap:** rest serves the home; recreation remains actual square visits. Split the existing twenty housing satisfaction points between an assigned home and recent rest, keeping the total score at 100. Avoid a new decaying gauge.
- **Interruptions:** role changes, home reassignment, demolition and supper release pending rest without granting a completed visit. Existing cargo returns finish before personal routines. Stable home IDs and exact rest state persist in current-format saves; no migration.
- **Clarity:** show home, recent rest and recreation reasons in the resident inspector, with a home camera link and explicit assignment. A house lists its residents. Report village participation without implying unused beds or an empty square satisfy individual needs.

## Playable checks

Compare compact and dispersed layouts at the same population and staffing: meals remain reliable, useful work continues, residents actually rest and socialize, and moving homes/adding paths can improve the arrangement. Record activity/travel time and food deliveries rather than tune solely for a passing scenario. Re-run the river's two approaches because routines reduce available labor.

Verify homes never overfill, assignments survive saves, newcomers obtain spare beds, and invalid homes release their residents. Exercise interrupted travel/rest, demolition cancellation, Creative removal, supper and a blocked/busy rest destination. Inspect visible home visits, paused poses, resident/home links, and feedback at 960/1440. Keep final scheduling and satisfaction tuning open to playtesting.

## Implemented behavior

Residents automatically fill completed cottages/lodges, choosing nearby available beds when they have no home. Existing assignments stay stable. New assignments have a staggered first visit after 45–87 seconds. Subsequent visits become due 180 seconds after completing rest and wait for the current job or cargo return. Residents sit beside home for six work seconds. Rest completion postpones square eligibility by at least 15 seconds, leaving an opportunity to work between personal routines.

Homes never exceed their bed counts. The resident inspector shows recent rest and recreation reasons, links to the current home, and allows previewing a spare home before assigning it. Preview moves the camera while retaining the selected resident. A house lists its residents. Moving home interrupts only an active home visit; other work continues. Full homes are unavailable as destinations—household swaps remain a possible later convenience, not automatic reassignment.

Demolition releases households immediately, and unhomed residents fill available beds. Cancelling demolition makes its beds available again; it does not evict residents from another home they have already obtained. Creative removal and supper release unfinished rest without credit. Placement preserves access to active rest spots. Rest and square visits reserve separate free positions.

Satisfaction remains out of 100: optimism 10, meals 30, actual food variety 20, assigned home 10, recent home rest 10 and recent square break 20. Rest credit lasts 240 seconds, square credit 120. Creative disables food needs but retains homes and social routines. Save format is now 24; older development saves are rejected without migration.

## Measured layout comparison

Eight residents, identical buildings/resources/staffing, ten simulation minutes. Four residents produce food near the west bank; remaining residents are unassigned. Only household assignment differs. This deliberately isolates travel, not a maximum-throughput economy.

| Outcome | Nearby west-bank homes | Distant east-bank homes |
| --- | --- | --- |
| Home travel, total person-seconds | 105 | 270 |
| Full village meals | 10 | 10 |
| Completed rest visits | 24 | 24 |
| Completed square visits | 65 | 58 |
| Food delivered | 226 | 219 |

Both layouts work. Nearby homes leave more time for public life and a little more food production; distant homes remain recoverable choices. The test proves a useful travel cost, not that the tuning is universally balanced or that households are already an enjoyable long-term management system.

The river's two scripted approaches still pass, now at 780/900 simulation seconds (13/15 minutes) for cottages/gardens and bread/lodges respectively. Preserve these as feasibility results, not human playtime claims. Human pacing and aesthetic acceptance remain open.

Validation: full simulation suite; focused home checks for capacity, exact travel/rest continuation, interruption, rest expiry, home choice/removal/cancellation and supper; rendered home controls and paused pose at 960/1440; existing HUD/placement/decoration/demolition checks. Run `./Test.ps1`, `./Play.ps1 -HomeSmokeTest`, and the broader `-HudSmokeTest`. Home screenshots are under `artifacts/f25a-*.png`.

## Follow-ups to earn through play

Observe whether six seconds reads clearly as rest at the normal camera and whether players can explain missed visits. Consider household swaps or a small resident-summary disclosure if assignment becomes repetitive or the inspector becomes crowded. Keep nights, fatigue penalties, comfort upgrades and additional service buildings deferred. The next new system is the fishery and its lake scenario, at the existing population scale.
