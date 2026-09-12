# F16c2 — Creative relocation controls

Finished Creative buildings now offer **Move building** in their inspector. The existing placement markers show destination validity and entrance, with R / Shift+R rotation. Escape or right-click cancels without moving the source. Successful confirmation retains the building object and selects it at its new location.

The preview uses the existing building state, including finishes and orchard growth. Destination checks are cached for up to a quarter second while hovering; confirmation always checks the live world again. Moving rebuilds presentation once without reloading the world or resetting pause/speed. Paths and grove orders beneath the destination are cleared only on success, matching construction.

Active fishing boats and disconnected access remain guarded by [the simulation foundation](RELOCATION_CORE_F16C1.md). This is Creative only; normal demolition and construction retain their costs and rules.

Verification: `Test.ps1 -Relocation` covers state/goods, actual worker interruption, active/moored docks, bridge connectivity, exact saves and continuation, read-only entrance queries and destination path/grove cleanup. `Play.ps1 -RelocationSmokeTest` exercises actual inspector and confirmation clicks, Escape cancellation, invalid and stale destinations, cottage/orchard/pantry previews and all four rotations at 960/1440. Captures are under `artifacts/relocation`. Full simulation and shared HUD regressions pass. Rendered shore-specific relocation and human interaction feedback remain follow-up coverage; core shore checks are not visual evidence.

Next review: prioritize sound listening and campaign first-play evidence before additional producer systems. Relocation removes a concrete Creative arrangement frustration; it does not establish campaign pacing or visual appeal.
