# F12h1 — Creative terrace design and geometry evidence

September 12, 2026. Foundation only: there is no terrain-edit command or player control yet. Source baseline: `e4eba58`. Reproduce with `./Test.ps1 -TerrainShapingReview`; the report is written to `artifacts/terrain-shaping/geometry.md`.

## Player purpose and chosen operation

Let a player make a usable building terrace and compose a village across gentle hills. Decoration tools cannot change whether a building and its entrance fit level ground. This does not introduce an economic producer or a travel-speed advantage: current routing charges for paths versus ordinary ground, not elevation.

The first tool is **Level terrace**, Creative only. Select a rectangle, choose a target elevation from 0 to 4 in 0.4 increments, inspect the result, then explicitly Apply or Cancel. Raise/lower controls change this target. Do not implement independent stamps that leave the intended building site sloped. Include room for the entrance when choosing a terrace.

Heights live at shared grid corners. Flatten the selected rectangle's corners and blend into existing ground with at most 0.4 rise per cardinal edge. The prototype clamps each old corner into the allowable interval at its Manhattan distance from the terrace. Preview both the selected plateau and the entire changed border; the border can be much larger than the selection.

Protect the entire changed area, including neighboring cells changed through shared corners. Reject the whole edit when it touches water, excluded ground, buildings and entrances, bridge banks, dock launches, resources and access cells, paths, managed woodland, decorations, meeting/meal seats, people, active routes or work destinations. Explain the obstruction. Never silently trim the terrace, move an occupant, or delete an object. Protect current trips locally; do not require the whole village to become idle.

Provide **Undo last terrain change** using the previous affected heights, with fresh validation against intervening construction, people and routes. Undo must not load an old world snapshot or rewind goods/time. Undo is session-local and cleared on world replacement; persisted heights still use current saves. A rejected undo retains the pending inverse so the player can remove its obstruction and retry. A successful new terrain edit replaces the previous inverse. No-op and rejected edits must preserve both world and undo state.

## Independent design review

The bounded read-only game-design reviewer `terrain_shaping_review` recommended a usable rectangular terrace, protection of the complete changed border, and a terrain-only inverse. Those recommendations informed this policy. This was a source/design review, not a UI playtest, human enjoyment result, or the periodic four-role review.

## Verified geometry

| Scenario | Cottage cell | Target | Selected cells | Changed cells | Border cells |
| --- | --- | --- | --- | --- | --- |
| Flat clearing | (4, 0) | 0.4 | 9 | 25 | 16 |
| Raise a flat site on the large map | (-10, -1) | 0.8 | 9 | 45 | 36 |
| Level an existing slope | (0, 8) | 0.8 | 9 | 34 | 25 |

Each scenario places an actual completed Creative cottage with a level entrance, preserves the preconstruction reachable-cell set, validates the world and roundtrips its current save. The source world remains unchanged during the prototype preview. A path outside the selected rectangle rejects an edit because its shared corners change. Intervening construction overlaps the protection set; this proves detection, not a working undo command. Restoring only terrain after simulation ticks preserves time and stock. A separate contour experiment proves that raising and then lowering cannot reconstruct the original hills.

The three scenarios are distinct: the first two explicitly require initially level sites and the third requires an initially sloped site. The prototype searches for legal examples; it does not establish acceptance of arbitrary rectangles or exhaustive world safety.

## Remaining implementation chunks

**F12h2 — authoritative transaction and undo.** Implement input bounds, dry-land selection, full changed-cell protection, nonmutating previews, fresh Apply checks and a bounded terrain-only inverse. Protect against stale terrain/world changes, preserve normal games and failed/no-op transactions, and test changes in occupancy between preview/apply/undo. Cover map boundaries, water, routes, building/access geometry and consecutive edits. Validate current saves with edited terrain. This foundation alone does not count as a playable checkpoint.

**F12h3 — playable controls and presentation.** Add rectangle selection, target controls, a before/after terrain preview, distinct full-border marking, obstruction feedback, Apply/Cancel and Undo. Preserve camera controls and cancellation conventions. Rebuild all terrain-dependent visuals and invalidate height caches after apply/undo; verify ray picking follows the surface. Check narrow/wide UI, placement on the result, live workers, repeated edits and world changes. Run a presentation review on actual rendered evidence. Count F12h once when these controls deliver a verified playable outcome.

Normal-play costs, water editing, larger-map generation, freehand sculpting, ramps and retaining walls remain separate roadmap decisions. No save migration is needed. Audio listening and campaign human feedback remain open; the periodic review is still due at playable checkpoint 5.
