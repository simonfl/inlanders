# Founding finish click regression

September 13, 2026. User report: “This village is ready” would not click in A home by the water. The inspected autosave had twelve residents, everyone housed and fed, and all four newcomers settled. The save was read only; no player progress was changed.

The HUD refresh hid the founding panels every frame before showing the active panel again. Hiding a button's ancestor between mouse-down and mouse-up cancels the press. Keep the active founding or hall panel visible throughout its phase; switch visibility only when the phase changes. Completion requirements and save format are unchanged.

Earlier scripted clicks pressed and released synchronously in one frame, missing this failure. The founding and hall journeys now hold each project-panel click across six process frames with HUD refreshes before release.

Evidence in local `artifacts/review/runs/`:

- Before fix: `20260913-235726-489-founding-346fe3` failed “Ending action failed” after verifying the village qualified, using a held finish click.
- After fix: `20260913-235828-772-founding-db0f10`, founding at960, passed invitations, finish, continued building and save/resume.
- After fix: `20260914-000158-037-founding-hall-d14ae8`, hall at1440, passed its project journey and current-save checks with held project clicks; capture process exited successfully.

These are scripted Godot input journeys, not native human play. Reopen the game to use the rebuilt UI, then load the existing save and finish normally. This corrects existing behavior rather than adding a playable outcome: count remains30, next whole-game review35. The next planned work remains F32a, the spacious working lakeside hamlet comparison.
