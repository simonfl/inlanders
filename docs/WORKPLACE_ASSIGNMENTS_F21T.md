# F21t — optional assigned workplaces

Residents can keep a named workplace while other sites with the same role remain active. Automatic remains the default; no campaign objective or ordinary staffing setup requires named assignments.

## Reviewed policy

Independent game-design and UX subagents reviewed dispatch and inspector code before implementation. Both recommended strict assignment rather than a preference that silently sends workers elsewhere, distinct persistent/current-work labels, reserved slots, and changes at job boundaries. These were bounded source reviews, not periodic checkpoint reviews or human playtests.

- **Automatic:** existing priority/ID dispatch, crop collection priority and dock-distance selection remain, using slots not reserved by named assignments.
- **Assigned workplace:** new jobs come only from that site. Paused sites, growing crops, targets and missing supply can leave a worker waiting. Choose Automatic explicitly to let them work elsewhere. Meals, rest, recreation and supper retain their normal behavior.
- **Changing a workplace:** current work, input reservations, physical deliveries and boat trips finish first. Persistent assignment is separate from the transient workplace attached to the current job.
- **Capacity:** named assignments cannot exceed the existing worker slots. Those slots stay reserved during breaks. An automatic worker already using a slot finishes normally; the assignee can wait for that worker.
- **Lifecycle:** changing role, removing the site or ordering demolition clears the assignment to Automatic. Cancelling demolition does not silently restore it. Pausing or exhausting a site retains it. Creative relocation preserves the site's identity and assignment.
- **Scope:** foragers, farmers (grain fields, vegetable gardens and orchards), bakers, sawyers, fishers, quarriers, hunters and carpenters. Loggers, builders, haulers and unassigned residents remain village-wide. A chosen hut does not define a berry territory; carpenter home orders and material-store selection remain village-wide.

## Player controls

In a resident inspector, choose **Workplace: Automatic** or a site identified by type, ID and coordinates, then **Apply workplace**. Drafts do not change the simulation. The visible explanation includes full assignment capacity or waiting state. The assigned-site link is separate from the current job-site link. Changing roles clears assignment; the role button explains that consequence.

Workplace inspectors distinguish named assignments, current workers and village-wide role counts. Assigned residents stay listed while on a break or finishing another job. The existing +/− role controls still change village-wide staffing; they do not implicitly bind a worker to this building.

People keyboard navigation includes the workplace picker, Apply and assigned-site link. Left/right chooses, Tab moves, Enter applies; Escape returns to residents and discards the unconfirmed workplace draft. Mouse Apply works too. World/role changes reset the picker to the real assignment, and removed site IDs cannot remain as actionable stale choices.

## Verification

- `./Test.ps1 -WorkplaceAssignment`: real dispatch for all ten supported building types; strict paused/growing waits; choosing Automatic; full-slot rejection without mutation; an already busy worker completes its job before moving; automatic workers cannot retake named slots; actual at-sea dock changes; normal carpenter input/delivery/installation; role changes, demolition/cancellation, removal and relocation; exact current-save continuation and over-capacity save rejection.
- Full `./Test.ps1`: simulation and campaign regressions pass with Automatic as the unchanged default.
- `./Play.ps1 -PeopleKeyboardSmokeTest`: existing resident keyboard regression plus workplace drafts/Apply, duplicate-site identity, live capacity conflict, paused explanation, assigned resident links, Escape, mouse takeover, removal, role changes and reload at 960/1440. Captures are under `artifacts/workplace-assignment/`.
- `./Play.ps1 -HudSmokeTest`: shared HUD, staffing, navigation, production and recovery controls pass. The final picker sizing adjustment is covered by the focused keyboard rerun.

Current saves use format 37. No save migration. This delivers control and clear waiting behavior; it does not establish a productivity gain or a human preference for micromanagement. Observe actual player usage before adding assignments to campaign teaching or introducing a softer fallback mode.
