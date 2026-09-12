# F16d1 — atomic Creative removal transaction

Foundation only: no new player control is delivered yet. F16d2 must add area selection, whole-object highlights, count/rejection feedback, explicit confirmation, cancellation and one presentation refresh.

## Selection and scope

`SelectCreativeRemoval` snapshots buildings whose footprint intersects the inclusive rectangle, decorations and paths. Reversed corners work. A partial footprint selects the whole building; the UI must highlight that entire object before confirmation. Trees, bushes, resource sources, residents, managed-grove orders and terrain are not selected. Construction plans are refused with a direction to finish/cancel them individually.

Building targets retain ID, position, kind and orientation; changed or missing targets reject the selection. Decoration values and path locations are checked. Current stock and worker state are deliberately read again when preparing removal.

## Transaction contract

`PrepareCreativeRemoval` never edits its source. It returns either a complete validated replacement world or a reason with no replacement. It runs existing goods recovery, worker interruption and access rules on a current-format save copy. Decorations go first because removing a fence can open access; buildings are removed in an eligible order, then paths. If the remaining group is unsafe, the entire temporary result is discarded.

The UI must call preparation again against the **current world** on confirmation. Do not retain a preview world and publish it after simulation advances. On success, replace the active world once and rebuild affected presentation without resetting pause/speed/camera. Existing object references belong to the old world and must be cleared/refreshed. Save migration and undo history remain out of scope.

Copying/validating a world is appropriate for explicit review/confirmation, not pointer motion or every frame. Preview can display selection bounds/counts immediately; run the transaction check at a bounded explicit boundary. Final confirmation always rechecks.

## Verification

`Test.ps1 -CreativeRemoval` covers mixed object types, reversed corners, partial footprints, normal-mode refusal, stale selection, retained source saves, ripe orchard/local fruit recovery, actual carrying workers, active/moored docks, exact replacement saves and continued simulation. A river fixture verifies that two individually removable bridges cannot both be removed when their combined loss breaks access. Failed preparation leaves the original settlement exact.

The new checks also run in the default simulation suite. Focused checks and application build pass; UI interactions remain unimplemented and unverified.

## Design review / next chunk

This is an arrangement tool, not a normal-economy shortcut. The useful choice is which existing objects to erase together while protecting transport and physical goods. The smallest useful UI includes buildings, decorations and paths together; a building-only button would not fulfill the selected scope. Use visible selection and explicit confirmation instead of deleting during pointer drag. F16d2 must exercise camera controls, mode handoffs, live stale selection, cancellation and actual pointer interaction at 960/1440. Keep natural resource editing separate and label the exclusion clearly.
