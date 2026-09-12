# F11b3 / F18b3 — campaign decisions and clarity

Reviewed September 12, 2026, starting from `e913ba5`. This is an agent review of source, rendered UI and reproducible simulation routes. It is not a first-time human playtest. No new scenario, producer rebalance, quota or longer proof window was added.

## Review method and current measurements

Ran `SimulationTests --river` and `--lake`, `Play.ps1 -RiverSmokeTest`, `-LakeReviewSmokeTest` and `-GoalsSmokeTest`. Reviewed current narrow Goals captures and traced condition help, phase blockers, service filters and phase actions. River rendering exercises progression/completion and restoration; lake rendering exercises fishing/phase controls and the completed working village. Goals fixtures also use synthetic phase states to test layout; those screenshots are not evidence that those states arise in ordinary progression.

| Route | Current simulation milestones | Interpretation |
| --- | --- | --- |
| River, bridge/cottages/gardens | Bridge 48s; first homes 142s; first proof 300s; final homes/preparation 427s; completion 600s | A viable economical route. The last 173s includes work/visits/service proof, not a measured interval of human inactivity. |
| River, southern crossing/bread/lodges | Bridge 162s; first proof 600s; final preparation 732s; completion 900s | A genuinely different construction/material sequence, still with substantial food reserves at completion. |
| Lake, central square/garden | First catch 76s; homes 381s; community ready 450s; completion 600s | Reserving useful central land supports short service trips. |
| Lake, bread/remote recreation | First catch 99s; homes 589s; community ready 827s; completion after nearby seating recovery 1440s | Remote service trips remain a meaningful problem despite ample food stocks. A seating garden is part of the successful route. |
| Lake, rough garden layout | Assessment 406s; nearby-square plot usable 493s; recovery completion 840s | The test detects failures, changes the layout and recovers through a saved continuation. |

These values are simulation seconds from current logs, not wall-clock player times. The runners advance ticks directly; they do not represent ordinary pause/1x/fast-forward behavior. Wall time was not measured as playtime. Older 13/15-minute river and 10/18/16-minute lake reports describe earlier baselines and must not be presented as current runs.

River tests also cover food-service recovery (1020s in its restarted assessment fixture), 18 residents and rebuilding a neighborhood during assessment. Lake tests cover stopping production and restoring supply. Those are separate fixtures, not time penalties to add to the competent route.

## Decision scorecard

| Review question | River | Lake |
| --- | --- | --- |
| First consequential choice | Bridge position and whether spare labor funds food or east-bank construction first. | Reserve central land for recreation or use it for production/housing; choose a legal dock launch. |
| Supported action and consequence | Build homes/food before invitations; later provide a used east-bank Square. The first earned proof stays latched while preparing expansion. | Add food production beyond limited fish regeneration; shorten actual meal/recreation journeys when a remote arrangement struggles. |
| Cause that can be misunderstood | Extra east-bank beds do not prove actual east-bank recreation. Food in storage alone does not pass continuing-service proof. | A large food reserve does not mean residents receive timely meals; “square visit” wording incorrectly discouraged valid smaller venues. |
| UI path | Goals → Why → residents or relevant places; Plan; tracked condition; meal evidence → People/Economy. | Same paths, plus source survey/dock launch guidance and any valid recreation venue. |
| Waiting / uncertainty | Proof and late service windows create stretches after construction. No evidence here establishes that a first-time player finds that time engaging. | A rough layout can spend a long period missing service despite apparent abundance. The recovery test demonstrates an intervention, not that players discover it unaided. |
| Recovery | Reassign workers, restore supply or rebuild home/Square access; saved first proof is preserved. | Improve nearby recreation or restore producers. Fresh delivery and closed-request evidence eventually recover. |

## Findings and the correction

1. **Confirmed misleading lake service guidance — corrected.** `LakeRecreation` accepts recent recreation visits, but `LakeVillageProblem` and the textual objective said “square visit.” The blocker now explicitly accepts squares, seating gardens and halls. Rest wording now describes the earned four-minute window or five after an improved-home visit. Goal cards already described these rules correctly. The change aligns the phase action/assessment explanation with them; no predicates or timings changed.
2. **Confirmed narrow Goals overflow — new F21m.** The 960px river/lake fixture screenshots show a horizontal scrollbar and clipped content in the Goals drawer. Existing tests verify navigation and controls but do not prove that all content fits without horizontal scrolling. Fix this as a separate UI chunk before the storage art pass; do not mark it solved by the current green suite.
3. **Pacing remains open.** Current alternative routes and recovery work, but there is no human decision/idle-time log. Do not slow production or increase quotas to hit a nominal duration. A follow-up playtest should record whether a player understands the recovery action, not only completion time.
4. **Keep the existing system palette for quarry design.** A resource budget and land/labor choice are more useful next than another mandatory need. The hall's longer/less-frequent visits are a tradeoff, not a proven food-output gain.

## Verification and handoff

River/lake checks pass, including alternative routes, adverse service situations, saved phases and completion. New lake checks isolate expired rest and recreation histories in a saved twelve-person village and verify the phase blocker describes valid windows/venues. River, lake and Goals rendered checks pass; the overflow finding is explicitly outside their previous assertions.

Next is F21m: fit Goals to the drawer at 960/1440, retain readable wrapping and accessible phase/Why/Plan/track controls, and verify no horizontal scrolling in collapsed and expanded states. Then proceed to F23b5 storage presentation and F26b2a quarry design. Human enjoyment and first-play duration remain open review questions.
