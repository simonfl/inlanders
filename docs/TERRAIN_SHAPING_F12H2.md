# F12h2 — terrain transaction and session undo

September 12, 2026. Implements the simulation portion of the [terrace design](TERRAIN_SHAPING_F12H1.md). This remains foundation work: no player control or rendered preview is delivered here.

`PreviewTerrain(first, last, target)` returns read-only target heights, the full changed-cell area and an obstruction explanation. It checks rectangular dry land, finite elevations from 0 to 4 in 0.4 increments, and occupied/access ground throughout the sloped border. Preview does not edit the world. Map dimensions bound iteration before any rectangle loop, including extreme coordinate input.

`ApplyTerrain(preview)` checks the originating world, map identity/bounds and original terrain, then rechecks current occupancy and trips. It validates the proposed heights on an independent current-world copy before publishing heights alone. Rejected edits leave the world and previous undo untouched. A no-op also keeps the previous undo.

`UndoTerrain()` restores the terrain before the latest successful edit only. It rechecks the map, expected current terrain and current obstructions, then validates the candidate. Buildings or paths added since the edit block undo; clearing them permits a retry. It never restores simulation time, stocks, jobs or an old world snapshot. Storage is bounded to one before/after corner grid, not a growing history. New world instances and loaded saves have no undo record; saved terrain itself remains supported by the existing format.

Protected ground includes resources and their access cells, buildings and entrances, bridge banks, dock launches, the central yard, paths, decorations, woodland orders, people, walking routes, active destinations, meeting spots and reserved/carried meal seats. Unrelated workers can continue their trips while a terrace is edited. Changes to heights do not alter simulation routing costs.

## Verification

`./Test.ps1 -TerrainShaping` covers three distinct buildable terrace geometries; nonmutating and foreign-world previews; live obstruction revalidation; border paths blocking Apply and Undo; construction blocking Undo followed by successful retry; no-op/rejected edits preserving undo; lowering and undoing a terrace; consecutive edits; extreme coordinates and invalid target heights; water/excluded cells; changed map bounds and externally modified terrain; and current saves with deterministic live-worker continuation. The checks are also part of the default simulation suite.

Validation results: the full default simulation suite passed. After the final lowering/undo assertion and validation-exception handling, the focused terrain suite passed again and the game built with zero warnings/errors. No rendered test is claimed for this foundation.

The F12h1 search prototype remains separate evidence for the original design; runtime checks exercise the production commands. Automated tests establish geometry and state safety, not visual quality or enjoyment.

## Next: F12h3 controls

Add rectangle selection, target elevation controls, before/after surface preview, full-border marking, useful obstruction feedback, explicit Apply/Cancel and Undo. Recompute previews as occupancy changes; show a fresh preview after a terrain/world change. After successful Apply/Undo, rebuild height-dependent rendering and invalidate caches while preserving camera and ongoing simulation. Verify ground picking, narrow/wide layouts, live workers and actual building placement; obtain a presentation review using rendered evidence.

F12h stays in progress. This chunk does not advance the playable count of 3 or satisfy the periodic review due at 5. Audio listening and campaign human feedback remain open.
