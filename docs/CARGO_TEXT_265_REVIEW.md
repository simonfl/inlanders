# Compact cargo UI verification

Independent targeted engineering/runtime evaluation, November 14, 2026. Live build `6639F0101660`, harvest-relief scenario, disposable session `b2d4fa96d82f4fa2a9bb189808531150`, CUA in-app browser at 402 × 681.

**Result: pass; no blocking finding.** Default request and progress dialogs fit the phone viewport comfortably, with readable counts and reachable primary/cancel controls.

- Request: about 22 visible words including heading and controls, versus the supplied prior estimate of about 90. The log priority consequence remains immediately visible. `Start packing` includes 48 food and 16 logs.
- Partial cargo: about 33 words versus the supplied prior estimate of about 150. Shows food 3/48, logs 16/16, home food 23, keep 16, disabled Send, and the single current blocker: pack 45 more portions.
- Optional `How packing works` opened successfully in request and progress states. Progress details include mechanics and supply history. Closing/reopening either dialog resets the disclosure to collapsed.
- Actual UI cancellation returned the exact packed amounts: food 23 → 26 and logs 2 → 18, matching 3 food and 16 logs. Actual UI reordering restored the same packed counts.
- Ready: food 48/48, logs 16/16, home food 17/16. Send enabled; blocker text absent. Actual UI Send closed the dialog, changed the objective to “Relief is on its way,” showed “Relief boat departing,” and resumed at 1×.

Evidence boundaries: request, disclosure/reset, packing, cancellation/reorder and dispatch were actual CUA interactions. Two legal field placements with valid public previews and 13 ordinary public API advances of at most 30 seconds prepared readiness; this was not a full UI playthrough or human enjoyment evidence. No source edits, service restarts, hidden state edits or owner-save access. Audio, full departure completion, other scenarios, desktop layout and whole-game design remain unobserved by this narrow check. The session was paused, temporary viewport reset and temporary tab closed afterward.

Screenshots: [request](../artifacts/browser/compact-cargo/request.jpg), [packing](../artifacts/browser/compact-cargo/packing.jpg), [ready](../artifacts/browser/compact-cargo/ready.jpg), [dispatch](../artifacts/browser/compact-cargo/dispatched.jpg). JPEG is the browser's native screenshot format.
