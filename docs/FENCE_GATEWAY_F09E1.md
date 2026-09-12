# F09e1 — fence gateway design

September 12, 2026. Source baseline `f551d78`. This is a reviewed design, not an implemented gateway or a playable checkpoint.

## Decision

Add a **Fence gateway** to the existing free decoration palette. It is visibly open and always walkable. Its purpose is to make a garden or courtyard entrance recognizable; it has no routing advantage over an empty gap. Success therefore requires a rendered comparison against a plain gap at normal play zoom.

Keep the center visibly unobstructed, using side posts and open leaves to identify the opening. Do not draw a closed barrier that villagers walk through. The cell remains traversable from every cardinal direction; model placement must respect those walking centerlines even when the gateway stands alone. No locks, permissions, schedules, opening animation, movement modifier or animal-enclosure behavior in this version.

## Rules

| Concern | Chosen behavior |
| --- | --- |
| Cost | Free and instant in normal and Creative, matching all current decorations. Removal spends/returns nothing. |
| Paths | Coexist in either placement order. Removing either leaves the other. Paths retain their existing speed effect; the gateway adds none. |
| Occupancy | Clear dry land, no overlapping decoration, resource or building footprint. Preserve the existing requirement to remove decorations before building. |
| Passing workers | Walkability does not change, so a route through the cell is not itself grounds to reject placement. Keep claims, cargo and destinations intact. |
| Service locations | Keep yard collection, resource work spots, bridge banks, building entrances and reserved meal/comfort spots visually clear. These are active working places, not ordinary path tiles. Explain a rejection rather than repurposing the solid-obstacle reachability test. |
| Terrain | Use ground-following posts/leaves and verify gentle slopes. Do not promise directional collision or new terrain restrictions to hide a visual mismatch. |
| Orientation | R cycles four saved visual facings. The current decoration boolean expresses only two axes; represent the gateway's full facing without a save migration. Existing symmetric decoration appearance must remain intact. |
| Connections | Gateway offers connections only on its two side-post edges. Ordinary fences retain their automatic connections. Gate-to-gate links require compatible side edges on both objects. Never extend rails across the walking opening. |
| Preview/removal | Show the gateway and any changed neighboring fence segments. Removing it restores the neighboring terminal appearance without removing paths or adjacent decorations. |

Placement text should say **Always open; villagers and paths pass through**. An isolated gateway remains useful as a recognizable threshold; it does not require a closed enclosure or introduce a campaign goal.

## Source evidence and independent review

`Simulation/Decorations.cs` makes all existing decorations except pebbles solid, removes paths under solid placements, and spends no resources. `Simulation/Paths.cs` permits paths on unblocked land. `Simulation/Placement.cs` separately protects footprints, service access and reserved work/meal positions. These distinctions matter: a gateway needs deliberate walkable placement rules, rather than treating it as a fence and secretly reopening its cell later.

`FenceVisuals.cs` derives connections from all cardinal adjacent fences, with no saved topology. `DecorationBrush.cs` currently reduces visual rotation to a boolean. Both the connection contract and rotation state need updating; adding only a new model would leave misleading previews or lose facing after load.

Independent read-only reviewer `gate_design` recommended the always-passable threshold, explicit side-only connections/four facings, and the existing free decoration economy. These recommendations are adopted. The service-location protection above is the lead's refinement: preserve visual working space while allowing ordinary walking routes. This was source/design analysis, not a rendered readability check or human playtest.

## F09e2 implementation and acceptance

Implement the walkable decoration, full facing, path coexistence, models and neighbor-aware previews as one coherent playable delivery. Verify:

- Normal and Creative placement/removal leave resources unchanged.
- A worker actually traverses the gateway and finishes a physical delivery; placing/removing it during travel preserves cargo and claims.
- Both path/gateway placement orders, individual removal and Creative area removal preserve the surviving object.
- Four facings survive save/load and render correctly; ordinary fences still form lines, corners and junctions.
- Fences meet side posts, never span the opening; compatible/incompatible neighboring gateways and preview/removal updates agree with the final result.
- Occupied/wet/service spots reject clearly; slope rendering follows the ground; toolbar rotation and narrow/wide controls work.
- Screenshots from four sides compare gateway versus empty gap in an actual courtyard. Use a bounded presentation review to decide whether the entrance is clearer, and revise or cut the model if it is not.

Keep current-format saves correct; no compatibility migration. Do not expand this into a universal decoration-state rewrite unless the actual implementation requires it.

The ledger remains at checkpoint 4. Once the gateway is committed as a verified playable outcome, checkpoint 5 triggers the independent game-design, UX/onboarding, playtest and development-lead review before further implementation. Their findings select the next five chunks. Audio auditions and human campaign feedback remain open.
