# F09e2 — playable fence gateways

September 12, 2026. Delivers the [reviewed design](FENCE_GATEWAY_F09E1.md).

Choose **Build → Landscape → Fence gateway** in the decoration selector. Gateways are free, instant and always open in normal and Creative play. R/Shift+R selects four saved facings. The active placement hint explains that villagers and paths pass through. A gateway marks a garden/courtyard entrance without changing routing speed or introducing locks.

Gateways are walkable decorations. Paths coexist in either placement order and remain after gateway removal, including Creative area removal of the gateway alone. Ordinary decoration/building/resource overlap rules remain; placement protects entrances, work spots and reserved seats while allowing people to walk through an ordinary path tile. The existing current save format stores a gateway's full facing separately from the two-axis appearance of other decorations. No migration was added.

Fence connections are still derived visual state. A gateway offers only its side-post edges; a pair of adjacent gateways connects only where both sides agree. Placement/removal previews update neighboring fence segments. Posts and open leaves follow local ground heights, keep cardinal walking centerlines clear, and remain visibly open. There is no directional collision, gate animation, speed bonus, campaign objective or enclosure simulation.

## Verification

- `./Test.ps1 -Gateways`: all four saved facings, normal/Creative free placement/removal, path-first/gateway-first coexistence, invalid facing rejection, side connections, Creative area removal, service/water rejection, exact saves, and a loaded worker actually crossing a gateway and completing construction. Existing decoration checks also pass.
- `./Test.ps1`: full simulation suite passed, including gateway and existing decoration checks.
- `./Play.ps1 -GatewaySmokeTest`: actual placement clicks and R-key rotation, 960/1440 controls, full saved/rendered facing, neighboring previews, compatible/incompatible gate pairs, courtyard comparison, four camera sides and four slope facings. Paths remain after removal.
- `./Play.ps1 -FenceSmokeTest`: all 16 ordinary fence connection masks, placement/removal previews, cancellation, reload and raised terrain passed.
- Game build has zero warnings/errors. `./Play.ps1 -HudSmokeTest` passed after updating the palette fixture to count the expanded enum instead of assuming five decorations.

Generated images live in `artifacts/gateways/`: gap/placed/preview comparisons at both sizes, `view-0..3.png`, and `slope-0..3.png`. The verified assembly SHA-256 is `37B9160D864611488B88E6184B5BCC741E9ACE172F0FB4780038515C2CF59E4B`.

## Independent presentation review

Reviewer `gateway_presentation` recommends retaining the model: its capped posts/open leaves make an intentional entrance clearer than the empty gap, while leaving the path visible. Three initial findings were addressed:

1. Four model facings did not prove four camera-side views. Captured actual four-side comparisons; the reviewer confirmed the passage and side connections remain readable.
2. The green translucent preview blended with its footprint. A stronger warm preview tint separates gateway/rail geometry from the green ground marker; the reviewer accepted the revised contrast.
3. Passability appeared only in the detailed description. The active hint now says “always open · paths pass through.”

The reviewer confirmed all three corrections and visually inspected one slope facing without obvious floating or burial. This was read-only still-image/source review, not human play, motion, traversal verification or listening. Actual loaded traversal is established separately by the simulation test. The review is presentation-triggered and does not replace checkpoint five's periodic team.

## Checkpoint and next work

F09e1 was documentation only; F09e2 is one playable outcome, checkpoint **5**. Before any further implementation, run independent game-design, UX/onboarding, playtest and game-development-lead reviews against this committed build. Consolidate their evidence/limitations, update the roadmap and select the next five chunks. Do not claim the periodic review has already happened.

Audio auditions, human campaign feedback and long-frame uncertainty remain open. Gateway presentation does not resolve those questions.
