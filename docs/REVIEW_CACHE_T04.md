# T04 — reuse simulation fixtures across UI edits

September 13, 2026. Tooling only; playable count remains 18, regular whole-game review at 20.

`Review.ps1` now distinguishes the current build from the prepared world's inputs. UI and launcher edits still invalidate the build, but do not rerun an unchanged simulation fixture. The selected generator is part of fixture identity; camera/description changes in the scenario catalog are not. Changed snapshot bytes always invalidate the cache.

The explicit boundary lives in `Development/ReviewIdentity.ps1`: all Simulation/Tests/Development C# and relevant project/config/data inputs, root project/NuGet/MSBuild/SDK configuration, selected generator and actual .NET SDK version. Tests are deliberately included broadly; this is two conservative input sets, not a dependency graph. Generated bin/obj files are excluded. Changes to the boundary helper itself invalidate both identities. When introducing new external fixture inputs, extend this boundary and its regression checks.

Fixture metadata preserves its original full-source fingerprint, preparation time and test-assembly hash. A new capture separately records current source/build/test hashes, SDK and fixture fingerprint. Reuse does not relabel an old world's provenance as newly generated. Captured bundles still require the full current source identity; sharing simulation inputs does not make old presentation evidence current. Old cache manifests regenerate rather than migrate.

## Validation and measured saving

`Development/TestReviewIdentity.ps1` verifies the boundary in an isolated scratch tree: UI/launcher/catalog edits, simulation/generator/helper/config edits, SDK/generator identity, newly added source, and ignored build output.

`Development/TestReviewCache.ps1` exercises the actual launcher/compiler/renderer. It creates temporary comment-only UI, simulation and generator source probes; removes them afterwards; restores the deliberately altered fixture bytes; and retains review artifacts. Run it without another review/build process editing the same cache. It checks reuse after a UI build, unchanged preparation provenance, correct dual identities in a rendered capture, stale-bundle rejection, simulation/generator invalidation and snapshot-tampering rejection. It finishes by validating the restored snapshot.

The prepared recurring-commons world took **15.88 seconds** to generate. After an actual UI-only rebuild, the cached Prepare step took **0.26 seconds**, with no fixture/metadata rewrite. The build itself still took **6.09 seconds**, and the rendered capture **6.56 seconds**. This saves the preparation stage; it is not a claim about gameplay frame rate or human iteration speed. Capture evidence: `artifacts/review/runs/20260913-150309-450-commons-recurring-34d43e`.

## Queue decision

This bounded investment is sufficient for the next presentation comparison. Do not expand it into per-test dependency tracking, another runner or a general editor. Next **F29a** compares readable shared-place composition at village scale; then challenge the meadow's distinct decision. Native preference, motion/audio acceptance and the underlying product questions remain open.
