# Next chunks — sound, management and a useful late-game project

September 12, 2026, after F25e3. This is the active queue. Delivered acceptance briefs are preserved in [the previous queue](NEXT_CHUNKS_HISTORY_2026_09_12.md); the [roadmap](ROADMAP.md) remains the feature/status index. Reevaluate after each delivery.

## Comprehensive review

| Area | Current assessment | Decision |
| --- | --- | --- |
| Campaign and enjoyment | Ten levels exist. Five are lessons; later maps exercise access, finite resources, habitat and neighborhood support. Finale routes demonstrate later commitments, but automated timings do not establish human enjoyment. | Preserve introductory levels. Add a distinct functional project prototype before proposing level eleven. Do not inflate waiting, population or attendance quotas. |
| Buildings and costs | Seventeen types cover the core economy. Hall identities add appearance, not new building types. Existing [cost audit](ROADMAP_REVIEW_2026_09_12.md#current-building-palette) and civic budgets show construction, hauling, land and food-labor tradeoffs. | No blanket repricing. A future building must offer a different placement or operating choice; compare alternatives on the same map. |
| Needs and progression | Meals, assigned homes/rest and recreation are real routines. Comfort reduces homeward trips without a demonstrated food-output gain. Chapel/court use recreation. | Keep optional improvements honest. No automatic thirst, education, spirituality or technology meter. Restoration needs a finished function, not an attendance prerequisite. |
| Visuals and village life | Building families, four-way rotation, shoreline structures, woodland, cottage finishes and civic identities have shipped. Quiet civic visits now follow real attendance. Sparse Creative fixtures still show regular ground/edges; individual poses are subtle at village zoom. | Judge composed working villages as well as isolated models. Keep terrain/shore and menu art visible in the backlog; do not equate more geometry with stronger art direction. |
| Interface and controls | Service and goal evidence are substantial. Main menu and Build catalog have keyboard focus. Other management pages still lack a complete focus path. | Extend one useful task end to end: inspect a resident, understand their activity, change a role and return. No new dashboard until an observed question requires it. |
| Audio and music | Procedural positional work sounds and one original looping musical miniature exist. Real-time sound limits already prevent fast-forward from multiplying every cue. | Compare quiet and busy soundscapes, then improve the highest-impact repetition/mix problem. A bell on every routine chapel arrival would overstate the event; it remains silent. |
| Maps and scale | Irregular ground, crossings, raised terrain, clearing and authored scenarios already exist. Broad decorated-population performance is not established. | Measure a representative settlement before expanding population or terrain size. Terrain sculpting is a separate design project. |
| Architecture and reliability | Current-format roundtrips, simulation routes and rendered interaction checks cover delivered slices. There are many partial Game files and specialized smoke fixtures. | Keep checks specific to behavior; do not turn the feature roadmap into a generic cleanup campaign. No save migration. |

This review uses repository code, existing route reports and the new civic rendered checks. It is not a new human campaign playtest or listening test. The biggest unanswered question remains whether players enjoy maintaining and improving a working village after its opening build.

## F10b1 delivered; F10b2 listening review remains open

[Spatial audio review](SPATIAL_AUDIO_F10B1.md): the map-dependent camera/listener defect is fixed, existing controls pass, and matched recordings support a remaining listening review. This does not yet prove a more pleasant soundscape. F21p is now delivered; retain F10b2 before musical variation.

**Outcome:** a busy village remains pleasant to listen to at normal and accelerated speed.

Capture the current mix in the same quiet, busy construction/production and waterfront scenes at close and village zoom, at 1x and fast-forward. Include music and nature together; document their settings. Listen before selecting the correction. Prioritize repetitive impacts/footsteps, voice competition and distance balance; implement the strongest evidenced correction plus a small bounded set of variations if helpful. Preserve actual work-contact timing and the existing Effects/Nature/Music controls.

Check mute/zero volume, pause/resume, camera distance, saved settings and reload without old one-shot events. Avoid allocating voices or generating samples every frame. Keep comparisons reproducible and do not claim perceptual improvement from counters alone. New music themes and transitions are F17b; chapel bells are optional only if the listening comparison supplies a clear purpose.

F21p resident selection and explicit role assignment are [delivered](PEOPLE_KEYBOARD_F21P.md). The restoration experiment is also delivered; continue with the remaining long-frame investigation below.

F25d2 is [delivered](RESTORATION_F25D2.md). The crossing shortens the route, but staged restoration adds no second commitment; separate F25d3 crossing integration is cut. Keep the ordinary bridge option and require a new functional brief before reopening restoration.

F23c2 is [delivered](LARGE_VILLAGE_F23C2.md): real 20/32-resident profiles, an isolated decoration cost and regional ornament batching. Typical dense frames improved; isolated long stalls remain unresolved.

## 1. F23c3 — diagnose intermittent long frames

**Outcome:** determine whether the observed stalls are reproducible game work and remove one supported cause.

Use the preserved larger-village snapshots. Compare longer cold/warm runs, including normal `_Process` rather than only the manual timing harness. Warm actual simulation/render state, not just frozen frames. Record frame outliers alongside fixed ticks, actor and food-view rebuilds, geometry creation, allocations/GC and engine/render waits. Keep renderer, window, camera, speed and sync settings explicit. Separate scene adoption and menu opening from sustained play; do not attribute a long frame to the GPU from residual wall time alone.

If a particular rebuild or allocation reliably coincides with stalls, make one bounded correction and replay the same events to check it. Preserve actual crop/cargo/building changes and current saves. If the cause is only host/driver noise or cannot be reproduced, record that result without an unsupported architecture rewrite. Do not use a lower median to claim the tail is fixed. This follows measured outliers; it is not a blanket performance rewrite.

## 2. F10b2 — listen and refine the mix

Use the matched recordings and remaining F10b brief above. Audition rather than infer preference from peaks or cue counts. A correction is conditional on those findings; retain the spatial fix independently. This review remains unfinished.

## 3. F17b — musical variation and transitions

**Outcome:** longer settlements have gentle musical variation without distracting state changes or repetitive restarts.

After the soundscape pass, compare the current single miniature with two related original themes and intentional quiet intervals. Use a small deterministic or bounded selection scheme that avoids immediate repeats, and crossfade or finish phrases cleanly. Keep music independent of simulation speed and preserve its volume/mute settings. Menu/game transitions should not restart the same opening phrase every time a panel or save is opened.

Listen through complete transitions, pause, loading and mute/unmute. Check bounded streams/players, no clipping and existing settings controls. Do not add a large adaptive score, mandatory external assets or victory fanfare for every minor milestone. Leave instrumentation and exact durations TBD until auditioned.

## Retained follow-ups

- **Visuals:** F23b10 working-neighborhood composition; softer shore rims, better ground variation and clear paths. First compare a populated scene at matched normal zoom. F19d title artwork remains separate from menu behavior.
- **Management:** remaining inspectors, Goals/Economy focus, source-route emphasis, and resource filters follow demonstrated tasks. Avoid catalog search while categories suffice.
- **Campaign:** human first-play clarity/pacing, optional comfort scenario and future productive-restoration design (F25d3 crossing integration is cut). Do not require every institution or diet.
- **Village arrangement:** gates with explicit walking rules, richer planting, relocation and area tools; terrain shaping needs its own cost/access/undo design.
- **Economy:** orchards, pasture, river mills, local stone storage and workplace assignments remain candidates. No new producer without a terrain/labor/service comparison.

Seasons, save migration, combat and multiplayer remain excluded. The broad roadmap is a set of revisable directions, not a promise to implement every parked idea. Record delivered results in the roadmap, keep this active queue short, and retain failed experiments as evidence rather than reopening them under a new ID.
