# F12h3 — playable Creative terraces

September 12, 2026. Completes the first Creative terrain-shaping operation from [F12h1](TERRAIN_SHAPING_F12H1.md) using [F12h2 transactions](TERRAIN_SHAPING_F12H2.md).

Open **Build → Landscape → Level terrace · Creative**. Drag a rectangle, choose elevation in 0.4 steps, compare Before/After, then Apply. Gold outlines identify the level plot; blue identifies its full changed sloped border. Rejected edits show red outlines and an explanation. Include the building entrance when selecting a plot. Close, Escape and right-click discard the preview; Undo last terrain change remains available when reopening the tool in the same village.

The preview changes rendering only. Target heights never replace live simulation data until Apply succeeds. Occupancy is checked again on confirmation; undo can be blocked by later buildings, paths or worker trips. Preview picking follows the displayed surface. Apply/Undo preserve camera, speed, pause and simulation state. Loading another village clears preview and session undo.

Original-board terrain now follows height data while keeping its perimeter, scenery and board footprint. Expanded maps retain their existing terrain renderer. Preview/commit/undo rebuild landscape geometry and wildlife ground details; protected buildings, resources and paths retain their heights. No whole-world replacement or audio reset is needed.

## Verification

- `./Play.ps1 -TerrainSmokeTest`: actual drag events and UI clicks at 960×640 and 1440×900; nonmutating Before/After; stale Apply refusal after adding a border path; Apply and actual cottage placement; elevated picking; lowering preview; Undo; focus/shortcut/placement/world-switch cancellation; camera, speed and pause preservation.
- `./Play.ps1 -HudSmokeTest`: shared HUD/input and existing management checks passed.
- `./Play.ps1 -MapSmokeTest`: large-map switching and rendering, elevated picking from four angles, hilltop placement, rotated crops, slope paths/planting/decorations, working carriers and terrain saves passed. Existing terrain smoke coverage is retained separately from the new shaping fixture.
- Build succeeds with zero warnings/errors. The full simulation suite passed in F12h2; this chunk changes rendering/UI, not the terrain transaction.

Screenshots: `artifacts/terrain-shaping/{before,after,blocked,applied,lowering,undone}-{960,1440}.png`. These are local generated artifacts. This is automated rendered interaction evidence, not human enjoyment or motion assessment.

## Presentation review and corrections

Independent read-only reviewer `terrace_presentation` inspected Before/After/blocked/applied screenshots and source. The reviewed working changes were based on `427610a`; final verified assembly SHA-256: `E6FC402E60744D17EE2AF80E8CCE6D483E46D5D3CD240352B8CB2E45E6254DD8`.

1. **Fix now:** using the expanded renderer on the original board removed unrelated perimeter scenery and changed the board edge. Kept the original board renderer and taught its grass surface to follow heights. Matched screenshots now preserve scenery and footprint.
2. **Fix now:** thin lit gold/blue marks appeared nearly white. Changed to stronger colors, wider rims and an unshaded material. Both regions are now distinguishable at narrow and wide sizes.

The reviewer inspected refreshed screenshots and confirmed both corrections, with no remaining consequential issue in that evidence. A downward-facing outline defect was also corrected during local screenshot inspection before that review. No listening, human interaction or motion review is claimed. This is a presentation-triggered review, not the periodic four-role review.

## Roadmap decision

Count the combined F12h1/2/3 delivery once as playable checkpoint 4. The next periodic review remains checkpoint 5. Normal-play landscaping costs, water editing, freehand sculpting, ramps, retaining walls and map expansion are separate candidates; this delivery does not claim them.

Promote the existing F09 gate candidate for the next bounded design chunk: assess readable fence openings and explicit walking rules before implementing. Audio auditions and campaign human feedback remain open. Do not add another automatic art pass solely because this tool is complete.
