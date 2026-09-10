# F25b2 — home-comfort playable prototype

Implemented September 10, 2026. Balance, aesthetic acceptance and campaign adoption remain open.

Build a carpenter workshop for 6 logs and assign a carpenter. An occupied cottage can order an improvement for 4 planks; a lodge costs 8. The carpenter collects two-plank loads and installs furnishings for 12/24 work-seconds. Residents keep using their homes. Improvements add shutters and a cushion to the actual rest pose, with no extra beds or needs meter.

Rest still takes six seconds. A visit begun in an improved home earns five minutes of recent-rest benefit and schedules the next visit four minutes after completion, versus four/three minutes normally. The happiness contribution stays 10 points. Quality is captured at visit start.

Orders survive workshop removal. Empty homes stop new installation until reoccupied. Cancellation releases reservations and returns carried/delivered materials physically; demolition also recovers installed planks. Creative improvements are free and immediate. Save format 32 rejects older formats without migration.

## Verification

- Full simulation suite passed, including cottage improvement phases, cancellation/recovery/reordering, empty/reoccupied homes, pause/resume, workshop demolition/replacement, home demolition, actual earned rest, and exact save roundtrips.
- A subsequent focused run also passed normal lodge construction and its eight-plank improvement with four residents and a save roundtrip.
- `./Play.ps1 -ComfortSmokeTest` passed order/cancel buttons, home-to-worker and workshop-to-home links, absence of an irrelevant output target, installation hammer/work-board pose, actual cushioned rest and pause stability at 960/1440. Run `./Test.ps1` first to regenerate its normal-play snapshots.
- Screenshots are in ignored `artifacts/f25b2-*.png`. The 960-pixel improved-home inspector was visually inspected; the panel is readable and scrollable. The model addition is modest, not accepted final art.

## Required follow-up before adoption

The original brief bundled implementation and broad comparative balancing. Keep the working prototype independently reviewable; retain the uncompleted comparisons explicitly as F25b2 follow-up rather than claiming them done. Run matched compact/dispersed villages, better-located ordinary housing, and another use of the same timber/labor. Record setup costs, actual production, travel, meals, rest and recreation; compare two cottages with a lodge, including partially occupied homes. Current tests establish costs and correctness, not payback or fun.

Also review rotated cottage/lodge details at the normal camera and improve their visual distinction if needed. F25b3 remains the human enjoyment/campaign decision after those comparisons. Do not expand into furniture inventory, compulsory comfort or additional upgrade tiers to justify a weak result.

Work pauses after this commit at the user's request; no next chunk starts automatically.
