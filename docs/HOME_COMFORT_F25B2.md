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

## Comparison follow-up completed

See the [September 11 comparison and visual review](HOME_COMFORT_COMPARISON.md): 24 matched branches, competing investments, full/partial lodge occupancy, all-side shutter improvements and four camera directions at 960/1440. The prototype reduces homeward trips but did not improve twenty-minute food output. Keep it optional; F25b3 player enjoyment and campaign adoption remain open. No migration, new need or productivity bonus was added.
