# Independent shortage-status review

Candidate **0FEF517B76A7**, reviewed November 24, 2026. **Keep the bounded status correction.** Ordinary API interaction reproduces the baseline shortage, now explains its cause, and verifies cancellation recovery without stale cargo text.

Baseline **88E2F151D169** (commit `97e3d91`) left the second field at `Logs 2/10 · building 0%` at 90 and 120 simulation seconds with zero available logs, 16 packed logs and available workers. Cargo cancellation returned resources and construction recovered. The broader cargo hint already explained cancellation; the defect was local building inspection. See local `artifacts/browser/shortage-check/independent-baseline.txt`.

Candidate session `130be716a9cf4839b9dd3350088a0694` used the same ordinary sequence: fresh harvest-relief, shipment-food, preview/place field `(4,-4,0)`, advance twice by 30 seconds, preview/place field `(-4,-7,0)`, advance twice by 30 seconds, cancel cargo, advance twice by 30 seconds.

| Observation | Candidate result |
| --- | --- |
| Initial field, zero available logs | `Logs 0/10 · Waiting for reachable logs (12 aboard the boat)` |
| First field at 30 seconds | `Logs 4/10 · 4 incoming · Waiting for reachable logs (16 aboard the boat)`; Louis was collecting 2 reserved logs and Jacques was delivering 2 logs, matching the incoming count. |
| First field at 60 seconds | `Building 0% · Builder assigned`; no obsolete materials shortage. |
| Second field at 90 and 120 seconds | `Logs 2/10 · Waiting for reachable logs (16 aboard the boat)`; zero available logs. |
| Cargo cancelled at 120 seconds | Accepted; explicitly returned 16 logs and 12 food. |
| Second field at 150 seconds | `Building 0% · Builder assigned`; Marie was walking to build this field, matching the status. |
| Second field at 180 seconds | Complete, `Sowing`; first completed field `Walking to work`. Neither retained shortage/cargo text. |

Snapshots: local `artifacts/browser/shortage-check/candidate-shortage.json` and `candidate-recovered.json`. Candidate session remains paused at 180 seconds.

Evidence limits: this is independent ordinary public API runtime interaction, not direct UI play or human preference. CUA initialization failed on the baseline with Windows cursor-access denial; no phone-size layout, screenshots, audiovisual observation or touch controls were evaluated, and no launch retry was made for the candidate. No source inspection, state injection, service restart or source edits were used. The distinct no-builder-assigned/materials-ready branch was not reached; the materials-complete/assigned-builder branch was observed and matched the worker's activity.

Previous baseline full-cargo verification remains separate: sending at 15/16 village food was rejected with the correct explanation; after deliveries restored 16 food, sending succeeded. That unchanged cargo-mechanics check was not rerun as a whole-level candidate playthrough. Neither API success nor this status correction establishes enjoyment or full-level experience acceptance.

Builder regression: ordinary API session `f639d18e66e743028c9795dc51f7f67b` on this candidate ordered the existing crossing repair, stalled at4/6logs, then recovered after a visible near-bank grove order. Inspection changed to2incoming at139s, builder assigned at144s, construction progress, then Crossing open at157s. Session left paused. Client/server build passed with no server warnings/errors; private endpoint returned this build. No simulation or client markup changed.

