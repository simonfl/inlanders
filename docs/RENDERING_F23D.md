# F23d — presentation cost and frame sync

The decorated village was substantially limited by frame presentation on this Windows/Intel Iris Xe setup. The adopted change is a persisted **Options → Frame sync** setting, off by default. It applies immediately and can be enabled to avoid tearing. It does not alter simulation time, camera behavior, meshes, lighting, shadows, population or animations. Frame sync can increase frame time on this setup; results on other displays/drivers may differ. Rendering without sync can use more GPU power and show tearing.

## Evidence

Run `./Play.ps1 -RenderIsolationSmokeTest`. It builds the normal 16-resident, 36-decoration pantry fixture, tests the setting at 960/1440 using an isolated configuration file, compares frame-sync modes, isolates rendering categories, then runs live meals/hauling and a 600-frame stationary legal preview. Logs and captures are under `artifacts/`; the final recorded run is `f23d-final-profile.log`.

At 1440×900, 120 sampled frames per isolation case after 15 warm-up frames:

| Same-process comparison | Median | p95 | Draw calls at end |
| --- | ---: | ---: | ---: |
| Paused, frame sync on | 38.37 ms | 40.97 ms | 1419 |
| Identical paused scene, frame sync off | 9.03 ms | 13.81 ms | 1419 |
| Live, frame sync on | 39.10 ms | 44.78 ms | 1398 |
| Live, frame sync off | 9.06 ms | 12.50 ms | 1411 |

The paused captures `f23d-sync-on.png` and `f23d-baseline.png` have identical SHA-256 hashes. The paused comparison preserves the exact village JSON. Live cases reload the same starting snapshot, but equal rendered-frame counts cover different elapsed simulation times; ongoing food trips explain differing final draw counts. This establishes presentation responsiveness, not faster simulation or an identical live end-state comparison.

With sync off, disabling game processing gives 7.80 ms median, hiding residents 6.36 ms, hiding buildings 7.21 ms, and disabling sun shadows 6.86 ms. Those are diagnostic omissions only; **all remain enabled in the shipped game**. The nearly empty diagnostic scene still includes 244 draws, so it is not a pure driver-cost measurement. Restoring the complete scene gives 9.36 ms. At 960×640 the full scene gives 9.03 ms.

The older 39–49 ms measurements should not be treated as pure rendering or simulation cost. F23c's geometry batching still reduced draw calls, but comparisons of future optimizations must record frame-sync mode, display size and scene state. Do not infer a need for an engine rewrite or more aggressive mesh simplification from the old frame times.

## Scope and next review

On/off clicks, persisted on and off preferences, applied window mode and unchanged settlement JSON pass at both window sizes. The setting is stored with existing visual preferences, outside village saves; no migration is needed.

The subsequent full-scene samples with sync off measure 11.1/15.7 ms median/p95 paused, 10.1/14.9 ms during actual meals/hauling, and 16.8/19.7 ms during the 600-frame paused pantry preview. The preview keeps an identical save and node count throughout. These are different scene/UI states; do not interpret the faster live sample as a benefit from simulation work. The build passes with no warnings or errors.

Review smoothness, tearing and resource usage in normal play. Larger populations remain unmeasured. Further optimization should start with a new uncapped profile of a representative heavier scene, rather than removing visible detail in advance. Human art and campaign enjoyment reviews remain separate.
