# F23c — fixed model geometry and village rendering

The river scenario exposed slow rendering on the local Intel Iris Xe machine. This pass combines fixed primitive pieces sharing a material within each building or decoration. It preserves the authored geometry, transforms, palette and shadows. It does not change simulation rules or save format.

## Scope and boundaries

`StaticGeometry.cs` bakes anonymous, visible primitive pieces and fixed anonymous subgroups after model construction. Material color, roughness and culling separate the batches. Named displays/rigs, grouped foliage, hidden geometry, emissive surfaces and other special geometry remain separate. Saw motion, oven glow and bread/plank buffers retain their existing node identities. Construction and demolition rebuild the corresponding stage; previews and catalog models use the same model factory.

Batching runs when a model is created or its stage/buffer changes, not each frame. Decorative pebble transforms retain their terrain alignment. No building detail was removed, no shadow setting was lowered, and no simulation inventory was capped.

## Reproduction

Run `./Play.ps1 -RenderingSmokeTest`. It completes the river route, samples the 16-resident village, adds 36 decorations, then holds a legal cottage preview for 600 paused frames. The latter asserts unchanged simulation state and scene node count. It writes frame distributions and mesh/draw/node counts to stdout, and screenshots to `artifacts/f23c-*.png`.

Baseline logs are locally retained as `artifacts/render-baseline.log`; final logs as `artifacts/render-combined.log`. Measurements use Godot 4.6 .NET Compatibility at 1440×900 on Intel Iris Xe, driver 32.0.101.7084. These are short local samples, not a portable benchmark or an FPS guarantee. The ordinary river smoke takes a live sample before pausing, so a few residents/cargo poses can differ between runs; scene layout, population and decoration count are held constant.

| Paused scene | Before median / p95 | After median / p95 | Draw calls before → after |
| --- | --- | --- | --- |
| River, 16 residents | 46.8 / 56.2 ms | 40.3 / 44.2 ms | 4,350 → 2,444 |
| Same village, 36 decorations | 53.2 / 61.9 ms | 45.3 / 54.8 ms | 6,078 → 2,876 |
| Decorated village, stationary preview (600 frames) | 57.8 / 73.2 ms | 47.3 / 53.9 ms | 6,180 → 2,921 |

Building mesh nodes fell from 657 to 136. Draw-call reduction is much larger than frame-time improvement: submission count is only part of the remaining rendering cost. Prior isolated measurements put HUD, actor updates and simulation well below the total frame interval; do not infer a precise GPU bottleneck from that alone.

## Review and next boundary

The decorated overview and placement preview were visually compared with their locally retained `-before` captures. Workshop and construction art checks passed for the moving saw, paused pose, oven state, real buffers, four camera directions and 960/1440 layouts. HUD checks passed for placement, decorations and demolition; map checks passed for rotated bridges and slope-aligned decorations. These establish behavior and visual preservation; aesthetic acceptance of F23a remains a separate human review.

The village is still below a smooth 60 fps on this machine. Keep F25a focused on existing population and buildings. Before expanding population or decorative density, profile resident/cargo rigs, shadow/rendering costs and model rebuild time in a longer representative run. Batch stable pieces within moving rigs only if measurements justify it, preserving their individual motion. The paused preview has no observed continuous node growth; do not rewrite its caching without new evidence.
