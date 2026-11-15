# Shipment worksite runtime evaluation

2026-11-14. Independent evaluator; no source inspection, code edits, service restart, or commits. Fixed candidate browser build **88E2F151D169**, localhost:5180, IAB at **402 × 681**. This is agent UI interaction and ordinary public API preparation, not human acceptance.

Disposable sessions:
- Harvest relief: `e5495947e1704d5fb2bb3dfd1f6916e4`.
- River home opening: `8b91f71c10854dc7b23089cc6a960cd6`.
- Preliminary request-only observation on superseded build C3C6784996D2 used `89f69f40…`; excluded from fixed-build conclusions.

## Observed results

- Fixed-build caption-origin drag visibly panned the map without opening cargo. Keyboard Enter on the focused caption opened the compact request dialog.
- UI Start packing produced partial cargo (12/16 logs, 0/48 food), `Packing supplies` caption, matching strip, disabled Send, and `Pack 4 more logs` in the dialog.
- UI Cancel returned all 12 packed logs (home logs 0 → 12), reset cargo, and restored the request caption. UI reordering succeeded.
- Cottage placement remained placement when tapping the boat caption: the dry-land validation appeared rather than the cargo dialog. Cancel dismissed the unconfirmed placement.
- Direct hull tap opened the compact cargo request dialog.
- Ready cargo reached 48 food/16 logs with 16 food at home. Map caption and strip both said `Ready to send`; the strip button said `Send help upriver`. Ready dialog showed enabled Send.
- Explicit UI Send started departure at 1×. Native screenshots at game times 7:57 and 8:04 showed the boat at different positions farther upriver, with `Help on its way` caption. The ending appeared at normal speed and reported 48 food and 16 repair logs sent. This is sequential still/motion observation, not video or detailed animation-quality review.
- Fresh river-home opening showed disabled `Cargo boat` caption, no cargo strip, and hull tap left the opening UI unchanged. No premature cargo dialog was exposed.

## Preparation and limits

Economic preparation used only the session's public command/preview API: field at (-3,-7), rotation 0 selected from public map/valid preview; field at (4,-4), rotation 0 supplied as a legal placement hint by the lead. Ordinary bounded advance commands brought the same UI session to readiness at simulation time 437.4 seconds. UI performed order, cancel/refund, reorder, hull/caption access and send. No hidden state editing or direct save modification.

Full cargo blocked by local needs was **not observed**. Eight ordinary five-second advances after first readiness retained readiness while home food rose from 16 to 31; the evaluator stopped the hunt. Thus the local-needs captions and blocker strip remain unverified by this evaluator. One accessibility-index click immediately after API advances did not open the ready dialog; a fresh screenshot and native coordinate caption tap did. Subsequent send worked. This isolated observation does not establish a reproducible application defect.

No blocking regression found in observed scope. No human play/preference evidence, audio assessment, full campaign/Creative/public-mode review, or production promotion recommendation.

## Native screenshot evidence

- `ready-phone.jpg`: full ready map, caption and strip, dialog closed.
- `departing-phone.jpg`: boat visibly farther upriver at 1×.
- `completed-phone.jpg`: relief ending and delivered cargo.
- `river-opening-gate-phone.jpg`: opening chapter after hull tap, cargo still gated.

Screenshots are stored locally under `artifacts/browser/shipment-worksite/`. Builder inspected the ready phone still. A separate ordinary-command two-field probe (`9aad312fb52d4535a04b78d39415fced`) also reached readiness, but did not supply the missing blocked-state observation. No artificial state was injected.

