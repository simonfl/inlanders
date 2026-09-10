# Rest and recreation coverage — F21i

Open **People → Rest / recreation**. The collapsed button shows residents with recent completed visits. Expand it to find residents missing either visit, rest, recreation, or to inspect everyone. Reasons reuse the resident inspector: no home, first visit scheduled, current job, travel/attendance, recent completion, venue capacity or a reachable free spot. A missing first visit is not automatically a crisis.

Each result links to the resident and their assigned home. Recreation links identify the current outing or the last completed visit while its benefit lasts. Removed venues do not leave dead links; a completed visit can retain its earned benefit. The resident inspector also has this venue link. Navigation never moves homes, changes work or orders construction.

## Design review

The decision this supports is where to put homes and recreation, and whether a resident is actually waiting for service or simply finishing work. It complements Economy's food/supply guidance. It adds no need penalties, scheduling changes, service-radius claims or save fields. Existing inspectors remain the place for home reassignment and building details.

The initial explanatory paragraph crowded the 960px drawer. The adopted version uses a short benefit comparison with detailed visit/return timings in a tooltip, collapsible above work assignments. Result rows show reasons directly instead of requiring a trip through every resident inspector. Larger neighborhoods may eventually benefit from grouping by home or venue, but add that only after using these filters in play.

This is current coverage, not a history of missed opportunities or a prediction of future service. A missing accessible spot is reported as such; the UI does not invent an exact capacity-versus-route diagnosis that the simulation has not established. Food remains explained in Economy and the resident happiness inspector.

## Playable checks

`./Play.ps1 -HomeSmokeTest` exercises both square and hall fixtures, first versus completed visits, each filter, active/last destinations, resident/home/venue links, removed venues, missing housing/recreation, reload reset and unchanged saves during navigation. It captures 960/1440 layouts under `artifacts/f21i-*`. The check is also included in the broad HUD suite.

No simulation rules changed. Current save format remains 29. Review the coverage view in an ordinary dispersed settlement to judge whether it makes the next improvement obvious; scripted navigation establishes behavior, not player understanding.

## Roadmap review

F26c wildlife is the next independent prototype: retained mature woodland should have a food value that competes with expansion and logging. Keep the resource experiment separate from The living woods scenario. F23a aesthetic acceptance and river/lake human pacing remain open; neither is inferred from this UI work. Keep comfort, learning and reflection tentative, seasons removed, and broader neighborhood food services in F07c. No new need meter or additional management dashboard is warranted by this chunk.
