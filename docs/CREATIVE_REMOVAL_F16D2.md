# F16d2 — Creative area selection and confirmation

Open **Build → Landscape → Remove an area · Creative**, drag on the map, review the counts/highlights, then press **Remove selected**. Reversed drags work. Intersecting any part of a building selects its whole footprint. Decorations and paths are included; trees, bushes, resources, grove orders and terrain stay. The panel states the scope and warns that unripe crops/partial processing are lost.

Dragging only selects and draws markers. A whole-selection transaction runs on release to explain restrictions, and confirmation runs it again against the live world. No preview copy is retained for later publication. A refused selection changes nothing and can be retried after resolving the issue or replaced by another drag. There is no partial-success behavior.

Escape, right-click, another drawer/tool, Watch, world replacement and loss of window focus cancel. Middle drag and WASD remain camera controls. On success the validated world replaces the previous world once, views are rebuilt, and pause, speed and camera are retained. Existing inspectors are cleared so old object references are not reused. Normal-play demolition is unchanged.

## Verification

`Test.ps1 -CreativeRemoval` covers the atomic simulation behavior, stocks/cargo, stale selection, active/moored docks, joint bridge safety and exact continuation. `Play.ps1 -AreaRemovalSmokeTest` uses actual pointer drags and buttons at 960/1440 for mixed selections, reversed corners, visible markers, cancellation, stale confirmation, full success, saved state and control/view preservation. It also covers keyboard/mouse tool changes, focus loss and normal-world replacement. Screenshots are in `artifacts/area-removal`.

Shared HUD regression passes, covering the existing input routing and panel hit testing. Bridge/dock batch safety is tested at simulation level; this chunk does not claim a separate rendered boat-removal walkthrough. Broad map-size/performance claims remain unchanged; world copying happens only on explicit review and confirmation.

## Roadmap decision

F16d is delivered for buildings, decorations and paths. Natural-source editing, undo and terrain shaping remain distinct backlog items. Audio audition is still open. Next independent candidate: Creative resource setup (F16e), already present in the Creative backlog. Scope it around central inventory and honest accounting; protected/reserved/carried goods must not disappear to satisfy a requested stock number.
