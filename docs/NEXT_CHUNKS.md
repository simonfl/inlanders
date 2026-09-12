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

## 1. F10b — a calmer working soundscape

**Outcome:** a busy village remains pleasant to listen to at normal and accelerated speed.

Capture the current mix in the same quiet, busy construction/production and waterfront scenes at close and village zoom, at 1x and fast-forward. Include music and nature together; document their settings. Listen before selecting the correction. Prioritize repetitive impacts/footsteps, voice competition and distance balance; implement the strongest evidenced correction plus a small bounded set of variations if helpful. Preserve actual work-contact timing and the existing Effects/Nature/Music controls.

Check mute/zero volume, pause/resume, camera distance, saved settings and reload without old one-shot events. Avoid allocating voices or generating samples every frame. Keep comparisons reproducible and do not claim perceptual improvement from counters alone. New music themes and transitions are F17b; chapel bells are optional only if the listening comparison supplies a clear purpose.

## 2. F21p — keyboard management of residents

**Outcome:** open People, inspect a resident, understand their current job and change their role through a visible keyboard path.

Audit the existing People list and selected-person controls before changing focus. Support predictable Tab/arrows, explicit activation, scroll-into-view and Escape back through the current layer. Remember a useful selection. Handle a resident becoming unavailable or a row rebuilding while focused without silently assigning someone else. Preserve mouse takeover, typing, camera shortcuts outside management and existing assignment rules.

Verify real role changes and their visible outcome at 960/1440, a longer population list, disabled actions, opening/closing, world changes and keyboard-to-mouse transition. Do not extend every inspector and management tab at once. Permanent workplace assignments and household swaps remain separate features.

## 3. F25d2 — restore something with a useful finished function

**Outcome:** decide whether an optional restoration gives a working settlement a worthwhile second commitment.

The prior learning-gate proposal duplicated builder labor and ended in recreation; that version was rejected. Prototype one functional restoration, with two candidate payoffs considered briefly before choosing: a reopened crossing that changes access, or a recovered productive site that changes food/material routes. Prefer existing systems for the first executable probe. A reading workshop or education attendance requirement is not part of this experiment.

Start from an imperfect working village. Budget actual materials, hauling and labor diverted from food, and compare restoration with improving existing routes/production. The first stage must change the value of the next choice. Record what remains to do after funding the project, its visible final use and a recoverable pause/cancel path. Test two credible routes and one poor allocation with current saves. Reject a project whose only distinction is a larger invoice or elapsed time.

Deliver an executable map/route prototype, a small progress UI sketch and a go/revise/cut decision. F25d3 campaign or feature integration is conditional on this result, not automatically promised. No new level, building-menu entry or need meter in this chunk.

## 4. F23c2 — representative larger-village performance

**Outcome:** know how much village we can comfortably support on this Windows machine and remove a demonstrated bottleneck if present.

Use an ordinary established settlement and a denser decorated version with actual residents, workplaces, cargo and paths. State population, objects, resolution, camera, speed, renderer and frame-sync settings. Measure steady state, camera motion and management opening; separate simulation work, rendering cost and one-off scene construction. Preserve the original fixture and comparable settings.

Implement at most the most significant measured bottleneck, if one threatens the intended experience. Validate identical simulation continuation and visible quantities before/after; record frame-time distributions rather than only average FPS. If the representative village already runs comfortably, deliver the measurements and stop. No renderer rewrite or larger map/population claim from a tiny test scene.

## 5. F17b — musical variation and transitions

**Outcome:** longer settlements have gentle musical variation without distracting state changes or repetitive restarts.

After the soundscape pass, compare the current single miniature with two related original themes and intentional quiet intervals. Use a small deterministic or bounded selection scheme that avoids immediate repeats, and crossfade or finish phrases cleanly. Keep music independent of simulation speed and preserve its volume/mute settings. Menu/game transitions should not restart the same opening phrase every time a panel or save is opened.

Listen through complete transitions, pause, loading and mute/unmute. Check bounded streams/players, no clipping and existing settings controls. Do not add a large adaptive score, mandatory external assets or victory fanfare for every minor milestone. Leave instrumentation and exact durations TBD until auditioned.

## Retained follow-ups

- **Visuals:** F23b10 working-neighborhood composition; softer shore rims, better ground variation and clear paths. First compare a populated scene at matched normal zoom. F19d title artwork remains separate from menu behavior.
- **Management:** remaining inspectors, Goals/Economy focus, source-route emphasis, and resource filters follow demonstrated tasks. Avoid catalog search while categories suffice.
- **Campaign:** human first-play clarity/pacing, optional comfort scenario and conditional F25d3 restoration integration. Do not require every institution or diet.
- **Village arrangement:** gates with explicit walking rules, richer planting, relocation and area tools; terrain shaping needs its own cost/access/undo design.
- **Economy:** orchards, pasture, river mills, local stone storage and workplace assignments remain candidates. No new producer without a terrain/labor/service comparison.

Seasons, save migration, combat and multiplayer remain excluded. The broad roadmap is a set of revisable directions, not a promise to implement every parked idea. Record delivered results in the roadmap, keep this active queue short, and retain failed experiments as evidence rather than reopening them under a new ID.
