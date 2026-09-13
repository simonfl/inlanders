# Checkpoint 15 — disciplinary findings

September 13, 2026. Frozen source `2f2ebd792ae271e5fc0f7f2e4daac2a1bba4cfd5`. Read-only reviews, no implementation by reviewers. [Synthesis and decisions](REVIEW_CHECKPOINT_15.md).

Three fresh independent contexts: `review15_design`, `review15_ux`, `review15_lead`. Creating `review15_playtest` failed with agent thread limit; attempting the historical playtest context also failed. The designer supplied a separately labeled visual/audio pass; UX supplied a separately labeled playtest-evidence audit. Thus **three independent contexts, five disciplinary passes**, not four independent core agents or five fresh reviewers. Native computer APIs were disabled in the available CUA tool and no deferred native tool was found. No native interaction or listening was performed. This review records the cadence limitation rather than claiming that missing observation occurred.

## Game design

Verdict: partially convincing as an expressive local village; unconvincing as sustained campaign. Physical local food, crossings, shared work and recoverable construction make arrangement causally meaningful. The meadow gives a comprehensible preparation problem. After provisioning, however, route inspection, retention and an optional gathering are more things to operate without an established reason to keep operating them.

The commons comparison correctly rejected artificial land sacrifice. Keep its free-square counterexample. SharedGathering requires everybody, synchronizes eating, then returns to ordinary life: a moment, not a lasting place. The 180-second assembly timeout may turn one delayed person into everyone's wait. No current motion/play observation establishes pleasurable anticipation. Retention changes shipments but all compared branches stay fed; local meals are non-monotonic. Neither feature earns second-act status from these results.

Keep food chains as geographic/expressive alternatives, not a required collection. Consolidate the first five tutorial levels into one optional guided settlement. Retain later river/lake/quarry/woods premises as material, replace rolling service certificates. Challenge carpenter/comfort and three recreation venues: visible improvement or distinct spatial use must justify them; interval/slot differences alone do not. All buildings remain accessible. Creative remains legitimate arrangement play, with its current difference from normal food life explicit.

Alternative: one persistent village with a recurring commons used by small groups during ordinary meals. Compare one-shot gathering, recurring place and no added activity in the same viable village. No new need, attendance gate or mandatory venue. Falsify if players do not notice or voluntarily watch/rearrange it, or mainly administer participation/food. If it fails, a finite satisfying settlement is preferable to indefinite maintenance without purpose.

Evidence: current source and comparative reports; initial working-neighborhood still, then fresh opening/river/dense/gathering stills in the visual addendum. No direct play, motion or listening.

## UX/onboarding

Verdict: partially convincing promise, increasingly manual-like controls. Main menu now exposes the selected settlements, invitations explain their commitment, completed Goals is quieter, and actual carried goods/source links connect outcomes to places. These changes earn retention.

The inspector hierarchy does not: removal and explanatory prose precede production/staffing in ordinary views. Three thresholds have overlapping terminology but different semantics: global output target, local retention, pantry destination target. A novice cannot safely infer what raising a target does. Routine operation should show current work, food here, shared/dedicated workers and one relevant action first; exact policies and removal belong in secondary sections. This is an interaction redesign, not a shorter paragraph.

Legacy campaign assessments certify rolling visits, fresh supply and closed meal requests rather than a visible village ambition. Keep them secondary. Creative appears alongside the selected workflow but uses different food rules; either align its rules with relaxed constraints or explicitly label that divergence. README still describes the old entry flow.

Alternative: a newcomer project played primarily through direct world selection, with one understandable bottleneck and visible resident consequence. Compare current and contextual controls in matched villages, including poor placement. Falsify if diagnosis still requires developer prose, automation erases meaningful choices, or different placements feel equivalent. Do not gate the catalog.

Tooling: semantic UI-state bundles and a contact sheet/index would reduce repeated capture discovery. Existing semantic tags help but are incomplete. Ask a future reviewer to locate opening Goals, production actions, failure and Creative without source spelunking. Small extension, not a new automation framework.

Evidence: frozen source, fresh broad stills and same-build precommit neighborhood UI at both sizes. No native onboarding, continuous motion or listening.

## Playtest evidence audit — reused UX context

Verdict: promising direction, incomplete evidence of a reliable whole journey. Current gathering run `20260913-125901-152-gathering-5d888f` reaches seated/completed captures, then the next probe cannot click hidden Economy after leaving/resuming. GatheringReview checks only unchanged world JSON and prints PASS. The old in-memory world can satisfy that assertion even if entry failed: **confirmed false-positive oracle**. Remaining in the menu is a strong inference; menu state/error was not captured at failure.

Root cause is unresolved. MenuAttempt may catch a load/save error only displayed in the menu, or focus-induced scrolling on mouse-down may move the button before release. UiClick already waits for layout and clips its target to scroll ancestors. More arbitrary waiting is not a demonstrated fix. Capture menu/HUD visibility, menu error, focus and button/pointer geometry around press/release; assert active HUD, paused state, expected scenario and exact save after entry. Preserve the initial failure.

Direct-tick gathering waits establish states, not normal pacing. Helpers scrolling to known policy controls do not establish discoverability. Invalid-water refusal, cancellation and returned food are useful scripted recovery evidence. Root's full menu smoke and keyboard checks pass, but do not resolve this exact mouse-driven transition. Fresh chosen opening/menu/Creative-resume probe also passes on a separate run; that makes the failure intermittent, not disproved.

Prioritize a complete credible journey: enter, choose a place, encounter a shortage, recover, share, leave and resume. Genuine uncoached play, listening and later-campaign experience remain unavailable.

## Development lead

Verdict: partially convincing foundation; product divergence is a greater risk than basic simulation correctness. Physical ownership, current saves and peaceful recovery are useful. The selected food rules are gated on Neighborhood while legacy campaign and Creative use other behavior. Freeze expansion; choose a primary experience before extending scenario branches.

Reset dispatch uses nested flags/map-name strings; experiment behavior depends on origin. After choosing the product, introduce a small explicit scenario identity/factory if it removes real branches. Reject an ECS, generic rules engine or wholesale architecture rewrite.

Retention has not earned primary UI cost. Its comparison is one warmed stocked village, so it does not prove that retention never matters. A fixed automatic default and an advanced comparison control are fair alternatives. One demolition took more than 180 but less than 600 simulation seconds: conservation and eventual success do not establish understandable recovery. Inspect the waiting jobs and visible blocker before changing priorities.

FoodMap observations cache at 0.5s but each enabled frame still hides/re-shows labels, builds sets/lists, projects stores, resolves overlaps and allocates line arrays, including work while management hides the overlay. This is avoidable work, **not a measured stall attribution**. Profile off/on/hidden/camera movement in ordinary/dense scenes before optimizing.

Tooling priorities: obtain a launchable paired experience and actual human screen/audio evidence using existing facilities (estimated half day, low upkeep); bounded overlay timing probe (half day, low upkeep); defer fixture dependency splitting until repeated cost dominates. Current dense preparation is 14.68s and any root UI source change invalidates fixtures. A simple simulation/generator fingerprint split might cost half to one day but carries stale-evidence risk; no dependency graph yet.

Evidence: source/reports, opening/river stills and root-reported tests. No independent test execution, native timing acceptance or listening.

## Visual/audio — reused design context

Verdict: partially convincing; improve playable composition rather than adding asset details. Warm roofs, recognizable work buildings and small people support a quiet miniature village. The neighborhood's continuing landscape is a stronger inhabited-place candidate than the legacy floating boards, while boards make build boundaries very clear. Resolve that tradeoff through actual placement comprehension before universal rollout.

The dense scene has decorative variety and charm, but repeated roofs, tiny labels and bright resident clusters compete with individual routines. Adding more ornament is unlikely to solve it. The shared-meal circle reads clearly as a coordinated event, but its empty center and disappearance do not establish a lasting communal place. Rebuild the relationship between places and ordinary routines first. Keep the palette, visible carrying and contextual landscape provisionally; reduce label prominence, retain detailed diagnostics on demand.

Audio source has spatial cues, voice caps, cooldowns and variation; three related 96-second themes with quiet intervals and separate volume controls. Sensible ingredients do not prove a pleasant mix. No audible defect or audio acceptance is asserted. Compare ordinary/dense work at normal/accelerated speed with music on/off before adding cues or themes.

Evidence: fresh opening, river, dense, seated/completed gathering and current neighborhood stills. Root separately inspected fresh Creative and title evidence; visual reviewer did not directly inspect those. No continuous motion, native play or listening.
