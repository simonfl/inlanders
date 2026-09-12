# Decoration brush strokes — F09d

September 12, 2026. Hold the left mouse button in the existing Place/Remove decorations tools to draw a one-tile strip. Clicking still edits one tile. The selected decoration and rotation apply throughout the stroke; fences join their neighbors as they are placed.

Skipped pointer cells are interpolated. Diagonal movements form a cardinal staircase, evaluated from the lower-X endpoint (then lower Z), inserting X before Z where both coordinates change. Reversing the same segment follows the same cells, so erasing can retrace a boundary. A multi-turn gesture follows its sampled segments.

Each tile uses the existing decoration command and access check. A refused tile does not replace another object or stop later valid tiles; the first refusal shows a reason and sounds once per stroke. Revisiting a tile within the same stroke does nothing. Accepted edits remain when a stroke stops. No resource costs, gates, broad fill or undo were added. Sunflowers remain locked in ordinary play until earned, and available immediately in Creative.

Release, a keyboard action, focus loss, another mouse button, tool/world changes, or entering the HUD ends the stroke. Returning to the map requires a fresh click. Right/middle camera dragging remains available with the tool ready. Preview-only motion does not edit the village.

## Verification and views

`./Play.ps1 -DecorationBrushSmokeTest` passes at 1440×900 and 960×640 with a clean build. It exercises actual pointer events for fast straight strokes, known diagonal cells and reverse erasing, repeated traversal, partial refusals, isolated rotation, exact saves, Creative/ordinary sunflower availability, and HUD/Escape/focus/tool/world/button-mask/rotation cancellation. It also checks right-button camera isolation and refusal of the final fence that would disconnect a cottage.

[Drawn garden](images/decoration-brush-garden.png) · [960px garden](images/decoration-brush-garden-960.png).

`./Play.ps1 -HudSmokeTest` also passes, including path drag interpolation, decoration palette/removal/tool switching, preview isolation and the broader responsive HUD checks.

The garden test initially sent several turns in one frame; Godot combined the motion into a straight segment. Separate frames model the intended gesture, and the fixture now verifies both side boundaries before checking the final access refusal. No access rule was weakened. The full simulation suite was not rerun; simulation commands and navigation are unchanged.

## Roadmap review

F09d is delivered. The village-character sequence now covers house palettes, coherent fences and arranging longer strips. Keep richer planting, facade details, gates and terrain sculpting available as separate follow-ups rather than automatically growing the brush.

Promote **F19c — main-menu keyboard navigation**. Inspection shows MenuButton delegates to the shared helper with FocusMode.None. Address this concrete UI gap with initial focus, traversal, activation, Back and scrolling on the title screen and its pages. Keep gameplay focus changes, controller support, title artwork and save-slot browsing separate.

Campaign enjoyment, visual appeal, optional civic ambitions, audio/music and the rest of the roadmap remain open. Completing this interaction does not establish their completion.
