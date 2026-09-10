# F25c — neighborhood seating garden

The garden gives a small established neighborhood a place to meet without reserving a square-sized building plot. It uses the same recreation need; it does not add another satisfaction meter or worker role.

| Venue | Construction | Reserved footprint | Concurrent visitors | Visit / return interval / benefit |
| --- | --- | --- | --- | --- |
| Seating garden | 4 logs | 1 tile | 2 | 6s / 60s / 120s |
| Square | 6 logs | 6 tiles | 4 | 6s / 60s / 120s |
| Hall | 8 planks + 12 stone | 6 tiles | 8 | 12s / 120s / 240s |

All three also need open reachable ground near their entrance for actual visitors. The planted garden tile is blocked; it is not a walk-through decoration. Visitors reserve distinct destinations using existing recreation rules. Seated residents bring the existing temporary stools to their reserved spots; there are no unused permanent chairs outside the footprint. The planter, trellis and flowers fit the reserved tile and appear in construction stages. Rotation changes its entrance and model orientation.

## Design review and measured tradeoff

Keep the square's better capacity per log. A garden should win on fitting spare land or reducing travel in a scattered village, not stronger satisfaction or universal production bonuses. Free decorative flowers remain visual only. Four logs makes two gardens more expensive than one square while offering the same total visitor capacity.

The focused five-minute comparison uses eight unassigned residents beginning in the same two clusters on Three clearings. One central square costs 6 logs and occupies 6 tiles; two local gardens cost 8 logs and occupy 2 tiles. Both also use open visit space. Results:

- Central square: 36 completed visits, all 8 residents served, 76.9 resident-seconds travelling to recreation.
- Two local gardens: 38 completed visits, all 8 residents served, 37.4 resident-seconds travelling to recreation.

This isolates service travel and placement. It does not measure construction payback, food output, a working household commute, or human enjoyment. Both arrangements work. Placement checks also demonstrate a planted neighboring tile blocking a square while allowing the garden, in both rotations. Do not claim that a garden needs only one tile of total public space.

## Verification and remaining review

`Tests/SimulationTests.csproj -- --seating` covers small/rotated footprints, capacity, actual visits, exact active-visit continuation, four delivered construction logs, demolition and removal. The full simulation suite includes these checks. `./Play.ps1 -HudSmokeTest` includes garden catalog/preview checks and square/garden inspector, seated pose, paused save and reload checks at 960/1440. Screenshots use `artifacts/recreation-SeatingGarden-*`.

The focused checks, full `./Test.ps1` suite and full HUD regression passed. The build had zero warnings/errors. Wide and narrow captures were visually reviewed. The first HUD run exposed a test assumption that all ordinary previews used a six-tile model center; the updated check covers the garden's one-tile preview and verifies the placed model center as well.

Review the inhabited result in ordinary play before adding more recreation venues. Costs remain provisional. New campaign objectives do not require the garden; it is available anywhere its normal land/access requirements hold.

## Roadmap review

Small seating completes the first F25c venue palette alongside squares and halls. The next useful design experiment is F07c neighborhood food service: first establish actual meal journeys and a clear benefit for a nearby pantry, because current meals consume central food without a resident collection trip. Specify delivery, ownership/reservations and shortage behavior together; local storage by itself would not create that benefit.

F23a art acceptance and human river/lake pacing remain open before expanding the building family or authoring new mastery scenarios. Comfort remains behind the visual direction. Additional food chains, learning/reflection, larger populations and terrain tools stay optional; no new priority for another recreation venue, seasons or save migration.
