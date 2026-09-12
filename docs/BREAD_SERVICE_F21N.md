# Bread reserve investigation — F21n / F07c3

September 12, 2026. The lasting-village fixture supplied a concrete service explanation problem: the bakery was delivering bread, the village was fed, but the forty-loaf supper reserve stayed empty.

## Finding and change

The existing Economy page reports recent food deliveries and total eating, but did not pair bread deliveries with bread actually eaten. The supper button already explained central storage in its tooltip. Local pantry controls already explained targets, direct deposits and surplus returns. The missing connection was visible evidence and navigation from the blocked supper.

Goals now has **Inspect bread supply**, which opens an expanded section in Economy. It shows recent bread deliveries and actual bread meals over the same window, central stored/reserved/available bread, local stock, bakery buffers and cargo. In the stalled fixture the last 180 seconds show **12 bread delivered and 12 eaten**, with **0 centrally available out of 40** required. The report explicitly distinguishes deliveries from reserves and suggests checking staffing, grain, targets and travel before increasing capacity.

Buttons link each bakery and pantry to its existing inspector. If local bread exists, guidance explains lowering pantry targets and assigning a hauler. This returns surplus food generally, not bread specifically; normal meals still claim food. No automatic bread reservation or forced pantry use was added. The level-four objective now uses unreserved central bread, matching the actual supper rule.

The section is collapsed by default. Opening it from Goals scrolls to its start; narrow windows scroll through the detail and workplace links. Completed supper hides its investigation prompt. The summary remains available in Economy for production inspection.

## Scope and evidence

The existing People meal/rest/recreation investigation and destination links remain the route for those needs. The finale's two food layouts already exercise actual meals, home rest and recreation; this pass fixes the demonstrated bread explanation rather than introducing additional service controls without evidence.

`./Play.ps1 -BreadSupplySmokeTest` builds, regenerates all five finale routes, and verifies rendered navigation at 960×640 and 1440×900. The tests inspect the real stalled fixture, click from Goals to Economy to bakery/pantry controls, and compare exact paused saves. They also run production until bread actually reaches a local pantry, lower its target, pause new deposits, assign a hauler, observe a real surplus return shipment, and roundtrip that active save. Completed-supper guidance is checked after adopting the recovered route. All pass. The original five supper timings remain unchanged.

[Narrow investigation](images/bread-supply-960.png) shows the equal delivery/eating counts and missing central reserve. This is a rendered/read-only interaction check, not a human comprehension study. The return probe observes a shipment in transit; full finale recovery to a completed supper is established separately by the route tests.

No production, meal, hauling, construction, save-format or campaign-completion rules changed. No new persistent history was needed: the report reads existing food deliveries, actual consumption and stock reservations.

## Roadmap review

F21n/F07c3 is delivered for the supported bread investigation. Proceed with **F11b6/F18b6**, integrating the tested finale with earned phases, explicit supper gating and real campaign/replay/save checks. Keep **F12g/F23b9** next for authored landscape composition. Broader logistics controls, new needs and balance changes remain separate possibilities; these findings do not establish a need for them.
