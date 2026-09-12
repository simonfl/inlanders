# Connected fences — F09c

September 12, 2026. Adjacent decorative fence tiles now form continuous straight runs, corners, T-junctions and crossings. The end of a run has a terminal post. Isolated pieces keep their saved orientation and can be turned with R; connected pieces follow their neighbors.

Placement previews show both the new piece and the changed neighboring pieces. Removal previews show the remaining neighbors without the removed connection. Canceling or moving over the HUD restores the ordinary models. Placing, removing and loading a settlement rebuild the derived connections. Upright posts sample the terrain; rail halves meet at the same sampled tile boundary on slopes.

Fences remain free, solid one-tile decorations. They do not create gates or new walking rules. Existing protection of entrances, workers and reachable resources remains authoritative. Connections add no saved state.

## Views and verification

[Cottage garden](images/fences-garden.png) · [960px view](images/fences-garden-960.png) · [Raised terrain](images/fences-slope.png).

`./Play.ps1 -FenceSmokeTest` passes with a clean build. It exercises all sixteen neighbor combinations through actual pointer placement/removal, preview state isolation, hiding/restoring affected models, isolated rotation, exact reloads, protected worker/yard/home access and refusal to close the last garden opening. It captures the garden at 960/1440 and checks upright posts on a sloped map. The initial garden fixture hit a protected berry-access tile; moving the fixture preserved that protection rather than weakening it.

`./Play.ps1 -HudSmokeTest` also passes, including the existing decoration palette, removal, save/load, tool switching and broader 960/1440 HUD checks. Its old fixed preview-key assertion was replaced with a selected-kind/rendered-preview assertion because fence previews now depend on neighbors.

This is presentation work; no simulation placement or route code changed. The full simulation suite was not rerun for this chunk. Automated checks establish behavior, not player enjoyment.

## Roadmap review

F09c is delivered. Promote **F09d — decoration brush strokes**: connected boundaries are now coherent, but building a longer one still requires a click on every tile. Scope continuous one-tile placement/removal with per-cell rejection and clear input cancellation before adding broader brushes or gates.

The larger review still stands: campaign first-play clarity and pacing, architectural appeal, optional comfort, audio/music and menu polish remain open. More production chains or needs should earn their place through distinct decisions; this cosmetic improvement does not settle those design questions.
