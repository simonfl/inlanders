# Creative relocation foundation — F16c1

September 12, 2026. The simulation now supports checked movement and rotation of a finished Creative building while retaining its object identity and state. **This is not yet a playable feature.** The Move button, destination preview, cancellation, view refresh and rendered acceptance checks remain F16c2 within the active F16c delivery.

## Implemented behavior

`RelocationProblem` checks mode, completion, demolition, supper and active fishing before destination checks. The query temporarily removes the old footprint, applies existing placement rules, then checks the proposed final connectivity against originally reachable resources, workplaces and residents. A `finally` restores the exact building object, geometry and list order. Failed checks and failed moves preserve the saved world exactly. Repeating the existing location/orientation is a no-op.

`MoveBuilding` rechecks the destination and retains the same building ID/object, material investment, stocks, crop state, mature orchard trees, controls and finishes. Affected jobs, visits, food shipments and home/comfort work use existing interruption behavior. Cargo remains real; current routes are recalculated. An interrupted timber carrier that chooses the moved store gets its new entrance, rather than continuing to the old location.

Moored boats move with their dock and update launch position/heading. An active fisher prevents the move until the dock is paused and its boat returns. A bridge cannot be moved out from under a resident, even if its new location is otherwise a valid crossing. Existing destination placement and active fishing-route restrictions remain authoritative.

Building coordinates and orientation now have internal setters with explicit JSON inclusion. The save shape and version remain unchanged at 34. There is no new normal-play movement or material refund behavior.

## Verification

`./Test.ps1 -Relocation` checks seven ordinary building types through all four orientations. Read-only valid/invalid queries, invalid confirmation and no-op movement require exact saved-state equality. Moves retain identity, cottage finish, targets/priority, ripe fruit, pantry fruit, stored timber and bakery grain. Current saves roundtrip and continue identically for 150 ticks.

Additional fixtures exercise actual fruit carrying, paused mature orchards, actual timber delivery, active civic visitors, a resident on a bridge, and active/moored fishing docks. Crops and goods validate throughout subsequent simulation. The focused check passes; the full simulation regression also checks the changed position-field serialization against existing behavior.

## Remaining F16c work

Add Move to finished Creative building inspectors. Use an explicit valid destination click, existing rotation and visible placement refusals. Escape/right-click cancellation must preserve the world. Show the building's current state in its preview, including orchard maturity and visual identity; do not turn moving into a new construction order. Refresh building, crop, shoreline, path and selection views after success. Recheck the target at confirmation if the simulation changed while previewing.

Verify the complete UI at 960/1440, including blocked moves, tool/Watch/menu/reload handoffs, shore orientation, active work, and exact cancellation state. Exercise home improvements, civic identities and pantry shipments beyond the core fixtures where useful. Assess the cost of destination queries during pointer movement; do not claim performance from the simulation checks. F16c stays **in progress** until this player-facing portion is delivered.
