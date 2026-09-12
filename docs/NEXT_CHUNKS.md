# Next chunks — sound, music and Goals navigation

September 12, 2026, after F21q. This is the active queue. Delivered acceptance briefs are preserved in [the previous queue](NEXT_CHUNKS_HISTORY_2026_09_12.md); the [roadmap](ROADMAP.md) remains the feature/status index. Reevaluate after each delivery.

## Comprehensive review

| Area | Current assessment | Decision |
| --- | --- | --- |
| Campaign and enjoyment | Ten levels exist. Five are lessons; later maps exercise access, finite resources, habitat and neighborhood support. Finale routes demonstrate later commitments, but automated timings do not establish human enjoyment. | Preserve introductory levels. Add a distinct functional project prototype before proposing level eleven. Do not inflate waiting, population or attendance quotas. |
| Buildings and costs | Seventeen types cover the core economy. Hall identities add appearance, not new building types. Existing [cost audit](ROADMAP_REVIEW_2026_09_12.md#current-building-palette) and civic budgets show construction, hauling, land and food-labor tradeoffs. | No blanket repricing. A future building must offer a different placement or operating choice; compare alternatives on the same map. |
| Needs and progression | Meals, assigned homes/rest and recreation are real routines. Comfort reduces homeward trips without a demonstrated food-output gain. Chapel/court use recreation. | Keep optional improvements honest. No automatic thirst, education, spirituality or technology meter. Restoration needs a finished function, not an attendance prerequisite. |
| Visuals and village life | Building families, four-way rotation, woodland, cottage/civic identities and quiet visits have shipped. Populated comparisons now support quieter shores and continuous ground color. Outer outlines and subtle poses remain open visual questions. | Keep menu art and player visual feedback visible. Do not automatically follow each art pass with another terrain rewrite or equate more geometry with stronger art direction. |
| Interface and controls | Menu, Build, resident assignment and Economy inspection/return have keyboard focus. Goals and broader inspector actions remain open. | Extend Goals evidence and deliberate campaign action navigation next. No new dashboard until an observed question requires it. |
| Audio and music | Procedural positional work sounds and one original looping musical miniature exist. Real-time sound limits already prevent fast-forward from multiplying every cue. | Compare quiet and busy soundscapes, then improve the highest-impact repetition/mix problem. A bell on every routine chapel arrival would overstate the event; it remains silent. |
| Maps and scale | Irregular ground, crossings, raised terrain, clearing and authored scenarios exist. Normal 20/32-resident profiles and decoration batching are delivered; long frames persist in normal-process traces. | No population or terrain-size performance promise. Further stall work needs native profiling; the failed visibility experiment does not justify removing animations. Terrain sculpting is a separate design project. |
| Architecture and reliability | Current-format roundtrips, simulation routes and rendered interaction checks cover delivered slices. There are many partial Game files and specialized smoke fixtures. | Keep checks specific to behavior; do not turn the feature roadmap into a generic cleanup campaign. No save migration. |

This review uses repository code, existing route reports and the new civic rendered checks. It is not a new human campaign playtest or listening test. The biggest unanswered question remains whether players enjoy maintaining and improving a working village after its opening build.

## F10b1 delivered; F10b2 listening review remains open

[Spatial audio review](SPATIAL_AUDIO_F10B1.md): the map-dependent camera/listener defect is fixed, existing controls pass, and matched recordings support a remaining listening review. This does not yet prove a more pleasant soundscape. F21p is now delivered; retain F10b2 before musical variation.

**Outcome:** a busy village remains pleasant to listen to at normal and accelerated speed.

Capture the current mix in the same quiet, busy construction/production and waterfront scenes at close and village zoom, at 1x and fast-forward. Include music and nature together; document their settings. Listen before selecting the correction. Prioritize repetitive impacts/footsteps, voice competition and distance balance; implement the strongest evidenced correction plus a small bounded set of variations if helpful. Preserve actual work-contact timing and the existing Effects/Nature/Music controls.

Check mute/zero volume, pause/resume, camera distance, saved settings and reload without old one-shot events. Avoid allocating voices or generating samples every frame. Keep comparisons reproducible and do not claim perceptual improvement from counters alone. New music themes and transitions are F17b; chapel bells are optional only if the listening comparison supplies a clear purpose.

F21p resident selection and explicit role assignment are [delivered](PEOPLE_KEYBOARD_F21P.md). The restoration experiment and long-frame investigation are also delivered; their findings remain below.

F25d2 is [delivered](RESTORATION_F25D2.md). The crossing shortens the route, but staged restoration adds no second commitment; separate F25d3 crossing integration is cut. Keep the ordinary bridge option and require a new functional brief before reopening restoration.

F23c2 is [delivered](LARGE_VILLAGE_F23C2.md): real 20/32-resident profiles, an isolated decoration cost and regional ornament batching. Typical dense frames improved; isolated long stalls remain unresolved.

## F23c3 delivered — stalls remain unresolved

[Normal-process traces](FRAME_STALLS_F23C3.md) reproduce cold visibility-call stalls and a separate warmed pause outside the measured callback. Removing stool visibility churn moved the cold stall to another prop; that attempted correction is discarded. No performance fix is claimed. The harness and exact continuation checks are retained. Reopen targeted optimization when native engine/driver profiling identifies the cost; do not keep broad speculative rewrites ahead of player-facing work.

## 1. F10b2 — listen and refine the mix

Use the matched recordings and remaining F10b brief above. Audition rather than infer preference from peaks or cue counts. A correction is conditional on those findings; retain the spatial fix independently. This review remains unfinished.

## 2. F17b — musical variation and transitions

**Outcome:** longer settlements have gentle musical variation without distracting state changes or repetitive restarts.

After the soundscape pass, compare the current single miniature with two related original themes and intentional quiet intervals. Use a small deterministic or bounded selection scheme that avoids immediate repeats, and crossfade or finish phrases cleanly. Keep music independent of simulation speed and preserve its volume/mute settings. Menu/game transitions should not restart the same opening phrase every time a panel or save is opened.

Listen through complete transitions, pause, loading and mute/unmute. Check bounded streams/players, no clipping and existing settings controls. Do not add a large adaptive score, mandatory external assets or victory fanfare for every minor milestone. Leave instrumentation and exact durations TBD until auditioned.

## F23b10 delivered — quieter shorelines

[Matched populated comparisons](COMPOSITION_F23B10.md) retain ordinary/dense finale views and improve the working lake's distracting bright border and repetitive water marks. The quieter treatment preserves launch/access rules and reduces lake landscape triangles without changing decoration batches. Visual preference remains open; this is not a completed art direction.

## F23b11 delivered — continuous ground color

[Five-map comparisons](GROUND_COLOR_F23B11.md) support continuous vertex color across adjoining grass tiles. Heights, access, authored meadow variation, paths and placement feedback are retained. Flat-map hidden faces and original grass mesh counts fall. Shared-color, 960/1440 preview, exact continuation and existing slope checks pass. Outer outlines and planting remain feedback questions rather than another automatic terrain chunk.

## F21q delivered — Economy keyboard inspection

[Economy inspection](ECONOMY_KEYBOARD_F21Q.md) now provides visible focus, long-list scrolling, stable issue/place/worker identities, read-only inspector return, keyboard/mouse handoffs and reset. Focused 960/1440 checks, full HUD and People regressions pass. Full inspector editing and resource-survey keyboard navigation remain separate.

## 3. F21r — Goals keyboard evidence and actions

**Outcome:** read a campaign goal, inspect why it is or is not satisfied, follow a relevant link and return, then deliberately activate a ready campaign action using the keyboard.

Use existing goal/evidence controls and campaign rules. Provide visible focus, Tab/arrows, scroll-to-focus and explicit activation through G. Preserve goal and resident/place identity across changing evidence; returning should restore the relevant goal even if its count changed. Revalidate readiness before an assessment/growth action; never activate a replacement button when a phase completes or a list refreshes. Skip disabled actions and keep explanations readable. Escape backs out then returns keys to the world; mouse and text entry still work. Check 960/1440, a short introductory goal and a multi-phase late campaign, completion/phase transitions, reload, and exact unchanged saves for read-only paths. Keep existing confirmation behavior; do not add campaign waits or alter thresholds for navigation convenience.

This task can proceed independently while F10b2 awaits listening feedback. Musical composition still follows the sound review. Broader inspector editing and source-survey focus remain follow-ups.

## Retained follow-ups

- **Visuals:** further neighborhood treatments depend on the composition comparison. F19d title artwork remains separate from menu behavior.
- **Management:** remaining inspectors, Goals/Economy focus, source-route emphasis, and resource filters follow demonstrated tasks. Avoid catalog search while categories suffice.
- **Campaign:** human first-play clarity/pacing, optional comfort scenario and future productive-restoration design (F25d3 crossing integration is cut). Do not require every institution or diet.
- **Village arrangement:** gates with explicit walking rules, richer planting, relocation and area tools; terrain shaping needs its own cost/access/undo design.
- **Economy:** orchards, pasture, river mills, local stone storage and workplace assignments remain candidates. No new producer without a terrain/labor/service comparison.

Seasons, save migration, combat and multiplayer remain excluded. The broad roadmap is a set of revisable directions, not a promise to implement every parked idea. Record delivered results in the roadmap, keep this active queue short, and retain failed experiments as evidence rather than reopening them under a new ID.
