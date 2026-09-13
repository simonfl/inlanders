# F27b valley study: retain direction, withhold acceptance

September 13, 2026. Independent read-only follow-up by `/root/f27b_visual_direction`, at playable count eight. Fixed source `c38b40a` plus the valley study, fingerprint beginning `20B33A0E`; full provenance is in the run manifests. This does not replace checkpoint ten.

## Whole-game verdict and decision

Retain this as an experimental direction, not completed F27b. Continuing the river and surrounding land removes the decorated-board emphasis. Restoring the original camera and shortening completed Goals improves the gathering's visibility. Contextual labels and clustered tree crowns are useful. These changes support village arrangement as a possible central pleasure; they do not validate the production economy or administrative campaign objectives.

The reviewer challenges two design failures. Background grass and trees look usable and harvestable until placement reveals the boundary. Entrance tint remains a set of soft halos, rather than a courtyard linking bridge, homes and welcome activity. The primary agrees. Next test a rougher edge-of-clearing transition and quieter distant woodland, then a connected shared outdoor shape on the existing cluster. Keep cosmetic ground distinct from constructed paths. Do not add ornament density or start a universal building-art pass.

The surrounding landscape intentionally applies only to the neighborhood river experiment. Its authored river coordinates are not a generic solution for lake, raised terrain or Creative maps. Keep the original rendering selectable on the same world. A later rollout requires representative checks; no default style change is made now.

## Evidence and limits

Current candidate initial view: `artifacts/review/runs/20260913-042811-923-neighborhood-complete-5e75da/capture-0001/view.png`. Opposite camera view: `20260913-042941-753-neighborhood-complete-a1deb3/capture-0001/view.png`; primary inspected it, reviewer did not inspect the additional angle before final verdict. Same source and initial world; the camera turn differs deliberately. Capture 2 in the first run is the unobstructed later state; capture 3 shows the usable-land outline and concrete refusal outside the map.

The normal-process interval advanced 30 simulation seconds at 3x in 127.42 wall seconds. It validates continued simulation and before/after state, not smooth motion or native human play. A second capture process overlapped part of this run, so this is not a clean isolated benchmark. Investigate frame timing against the original before accepting performance. Native window discovery did not expose the game this turn. No actual listening was possible and no audio verdict is claimed.

An attempted 30-second engine movie was stopped after roughly 500 CPU seconds without a completed recording. Godot used its 1440x900 project viewport despite the 960 window request. The unvalidated Movie option was cut; bounded normal-process snapshots remain. This failure establishes tooling cost, not the cause of gameplay stalls.

The reviewer used current source/images and earlier dense/lake/welcome, campaign, Creative and audio evidence. The older dense scene warns about roof repetition and crowding; the lake's actual dock/boat activity offers stronger environmental identity. First-play onboarding, long-session engagement, awkward player layouts, all four angles, 1x motion and audio appeal remain unobserved in this review. The existing economy findings still prohibit automatic campaign rollout.

## Next acceptance work

Compare original and study frame timings without concurrent game processes. Resolve usable-land/resource distinction and connected outdoor space. Exercise placement, cancellation, selected labels, details expansion and same-world style switching; verify no simulation mutation. Then inspect awkward legal arrangements and representative dense/lake views, and obtain actual motion/listening evidence where available. Do not infer preference from passing controls or screenshots.

Count remains eight. No playable outcome or periodic review completion is recorded.

## Follow-up: HUD stall attribution

The isolated six-second interval in `20260913-043418-437-neighborhood-complete-0d499b` took 29.96 wall seconds at 3x. Existing frame instrumentation captured 20 frames; excluding the final incomplete wall sample, mean callback time was 1494.90 ms, HUD 1489.34 ms, simulation 3.84 ms, actor rendering 0.77 ms and atmosphere 0.35 ms. Mean thread CPU was 1497.53 ms and allocation about 554 MB/frame. This implicates managed HUD work, not a GPU/foliage inference.

Source inspection found repeated full eastern-land path searches in invitation status, including after the one-time commitment. The fix skips the search after commitment and uses one reachability flood fill for landing selection instead of pathfinding separately to every tile. Fresh arrival and shared-work checks pass, including unchanged nearest landing, duplicate-commit rejection, saved continuation and a bounded-allocation committed-status regression. Rendered verification passed.

Final fixed run: `20260913-043827-811-neighborhood-complete-232267`. The same six simulation seconds take **1.93 wall seconds**, versus 29.96 before. Mean frame interval is 14.63 ms, HUD 1.41 ms, callback 3.71 ms, allocation about 116 KB/frame (final incomplete sample excluded). The post-interval simulation state is exactly equal to the pre-fix run. The full scripted control suite passes in 20.71 seconds including the build, versus 238.84 seconds before: style/camera/save invariance, placement outline/cancellation, completed-details expansion/collapse, speed/pause, F8 and dedication/release. This is a measured local stall fix, not proof that all earlier native stalls are resolved.

A sequential original-rendering control completed at `20260913-043856-492-neighborhood-complete-925886`; no concurrent game process was used for these final timing comparisons. Builds have zero warnings/errors. `--neighborhood` tests pass; no save format change. Original art remains default, and F27b remains under design validation.
