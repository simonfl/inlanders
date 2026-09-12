# Long-frame investigation — F23c3

September 12, 2026. The investigation is delivered; **the stalls are not fixed**. Normal gameplay reproduces both a cold actor-update stall and a warmed-up pause outside the measured `_Process` callback. A targeted visibility change moved the cold stall rather than removing it and was discarded. No simulation or animation behavior change ships in this chunk.

## Reproduction and measurement

Generate the current-format dense snapshot with `./Test.ps1 -FinaleCampaign` and `./Play.ps1 -LargeVillageSmokeTest` if absent. Run `./Play.ps1 -FrameStallSmokeTest`. It records 30 seconds of cold 1x, 30 seconds of cold 4x, then 30 seconds of 4x after 20 seconds of actual simulation warmup. “Cold” means freshly adopted scene, with five paused process frames before sampling; the later conditions share the same engine process and are not cold OS/driver launches. A diagnostic `--cold-stall-probe` engine argument reduces this to one ten-second cold 1x run.

The fixture is the normal 32-resident decorated finale settlement from F23c2. Hardware/settings: Intel Iris Xe, Godot 4.6 GL Compatibility, Debug build, 1440×900, daylight, hidden world labels, camera size 32, focus (3,3), angle .72, frame sync off, uncapped FPS. Scene adoption is excluded. Each scene runs less than the two-minute autosave interval, so autosave is not tested by this recording.

The harness leaves normal `_Process` enabled. It records raw monotonic frame-start intervals, attributing each interval to the preceding callback's work. Godot's supplied delta is retained separately: it can smooth/cap a long interval and is unsuitable as the sole stall measurement. The final callback has no subsequent interval and is excluded from frame statistics; its simulation ticks still count in replay.

Instrumentation is inactive outside this test. Records are value types in a preallocated list. Phases cover input, simulation, maintenance, actors, atmosphere/following, food views, HUD and audio. Actor subdivisions cover setup, residents, trees, stock and buildings; the slowest resident and their task are retained. Mesh counts cover the game's mesh helper and static batching, not every internal engine allocation. Allocated bytes are current-thread managed allocations; GC counts are process-wide collections. These cannot measure native allocations or establish GC pause duration.

`ProcessMs` is elapsed callback time, **not CPU residency**. Windows `ThreadCpuMs` measures current-thread CPU between frame starts with coarse OS resolution; unavailable platforms report -1. Residual wall time is not GPU time. No native engine/driver call stack was captured.

## Findings and rejected correction

The [initial full trace](FRAME_STALLS_INITIAL_F23C3.json) measured:

| Condition | Samples | Frames >100 ms | Largest raw frame | Callback within that frame |
| --- | ---: | ---: | ---: | ---: |
| Cold 1x | 1553 | 1 | 202.72 ms | 113.10 ms |
| Cold 4x | 1388 | 1 | 172.83 ms | 11.12 ms |
| Warm 4x | 1256 | 1 | 504.92 ms | 2.65 ms |

The cold 1x and warmed 4x outliers had no recorded mesh creation or GC. The cold 4x outlier coincided with Gen0/Gen1 collections, which is correlation rather than a diagnosis. Godot reported only 144.55 ms delta for the 504.92 ms raw interval.

Repeated ten-second cold probes narrowed a roughly 110–140 ms actor section to resident 1 while eating (`Work.EatingMeal`, enum value 44). A [temporary fine-grained probe](FRAME_STALLS_VISIBILITY_F23C3.json) measured 118.54 ms in the stool-hide call, with a 203.20 ms raw interval, no simulation tick, no mesh creation and no collection. Revealing the same stool took .005 ms. Other probes showed cheap movement, cargo refresh and transform updates. Main-thread CPU was substantial, so this was not exclusively descheduling/waiting. Launching a probe with `DOTNET_TieredCompilation=0` did not remove it; that observation alone does not establish which runtime policy was active.

An experiment committed final stool visibility once after pose selection instead of hiding/revealing it each frame. The [next probe](FRAME_STALLS_REJECTED_F23C3.json) still had a 211.49 ms raw frame and 127.57 ms callback. The expensive call moved to the tool-visibility reset for resident 17. **Removing one hide/show cycle did not solve the underlying pause.** The experimental animation changes and temporary per-property timers were removed. Historical probe reports use `CpuMs` for elapsed callback time; the retained harness now calls it `ProcessMs` to avoid confusion.

The evidence is consistent with a lower-level cost encountered during visibility mutation, but does not distinguish engine work, driver work, or another mechanism. The separate warmed pause remains unexplained. Do not blame a particular eating pose, GC, mesh rebuilding, or the GPU from these traces alone.

## Acceptance and roadmap decision

Every recorded condition validates the world, roundtrips the current save exactly, and replays the recorded fixed-tick count into an independent world with byte-identical final simulation state. The final retained-harness run is preserved in [the final report](FRAME_STALLS_FINAL_F23C3.json). Build and rendered trace checks pass.

That final run recorded 1,533/1,447/1,410 frames in cold 1x/cold 4x/warm 4x respectively. Their maxima were 192.6/93.8/62.9 ms. Only cold 1x exceeded 100 ms. The cold actor spike reproduced; the earlier 504.92 ms warmed outlier did not recur in this window. This variability is not evidence that an unmodified behavior was fixed.

Close F23c3 as a bounded investigation, retaining the performance risk. A future optimization needs a native CPU/render capture of a reproduced stall; repeating broad geometry rewrites or removing animations is not supported. Do not raise map/population performance claims based on this work. Continue with F10b2 listening and F17b musical variation; working-neighborhood art remains the next visual priority. Audio preference and campaign enjoyment still require actual listening/play feedback.
