# F16f — Creative berry-bush relocation

Build → Landscape → **Move berry bush · Creative** moves an existing berry source. Click a bush, click its destination, review the marked picking spot, then press **Move here**. Choose another bush restarts selection; Escape/right-click cancels. Middle-drag and WASD remain available for the camera.

The selected source has a gold marker; the destination and its east-side picking position use green/red validity markers. A ghost shows the destination without moving the original. Confirmation rechecks the live village. No rotation, cloning, deletion, food replenishment or normal-play move is included.

## Design review and implementation

A bounded game-designer/source review recommended preserving food state, releasing only the active picker, and treating the destination as a new solid obstacle. It also confirmed that wildlife capacity and recovery use mature timber trees, not bushes. This was not a periodic reviewer checkpoint or a human playtest.

- Preserve ID, ripe quantity, regrowth progress, list order and current save data. Same-location confirmation is an exact no-op. No save-format change is needed: the existing serialized cell changes position within format 37.
- Successful movement interrupts only a resident who still claims that bush. Walking/picking progress is released; role and assigned workplace remain. Once berries become cargo, the bush claim is already gone and the delivery continues.
- Reject overlap with buildings, resources, decorations, paths, managed woodland orders, occupied/next-step cells, reserved service positions and protected access. Bush and picking cell require dry land. Existing routes must remain connected; walking trips are rerouted after a successful move. No path or grove order is silently erased.
- Wildlife stock, capacity and recovery remain unchanged. Visual wildlife positions refresh because the set of walkable spots changes.
- The bush's existing rendered body moves when ripe stock is unchanged. Confirmation preserves the world instance, camera, pause and speed. Input modes, world replacement and focus loss discard the draft.

## Verification

`./Test.ps1 -BushRelocation` covers walking and picking interruption, an already-carried berry delivery, retained named workplace, unchanged repeated previews and same-place confirmation, repeated movement without replenishment, actual picking from the relocated bush, delivery afterward, exact save continuation, path/grove/decoration/worker/resource refusals, unchanged habitat, and rejection of a bush that would block a one-lane connecting route. Ordinary mode rejects movement.

`./Play.ps1 -BushMoveSmokeTest` covers the actual entry button and map clicks, source/destination markers, explicit confirmation, a path added after preview, cancellation, same-body movement, current saves, camera/time preservation, drawer/placement shortcuts, right-click, focus loss and world replacement at 960/1440. Captures are under `artifacts/bush-move/`.

Full `./Test.ps1` and `./Play.ps1 -HudSmokeTest` regressions pass. This is a village-arrangement tool; it does not alter normal foraging balance or expand wildlife ecology.

## Roadmap decision

F16f is delivered. Keep normal resource clearing separate. The next existing landscaping follow-up to design is player terrain shaping: it needs explicit occupied-ground, access, preview/cancel and recovery rules before an implementation, rather than piggybacking on bush movement. Audio audition remains open.
