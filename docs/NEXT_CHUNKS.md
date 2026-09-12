# Next chunks — settlement depth and village character

September 12, 2026. This supersedes the ordering in the earlier [roadmap review](ROADMAP_REVIEW_2026_09_12.md). The roadmap is the priority index; this document defines the next deliverables. Scripted runs establish behavior, not enjoyment.

## Review conclusions

- **Campaign depth is the largest design gap.** Ten levels exist, but the first five are lessons. Quarry routes finish in six to eight simulated minutes; fourteen reserve/food experiments mostly delayed the same opening plan. Keep those lessons and reject timer inflation. Later settlements need a second meaningful commitment after the village starts working.
- **Visual progress is substantial but uneven.** Homes, workshops, fields, forager shelter, square, storage and hall have received distinct treatments. Dock/bridge, woodland and finale landscape passes have now shipped. Cottage finishes, connected fences and decoration brush strokes have shipped. Main-menu keyboard focus is the next concrete UI gap. Matched screenshots support comparisons; player appeal remains an open question.
- **UI needs explanations at the point of decision.** Goals already expose service evidence and relevant places. The recent width audit found no Goals layout defect. Habitat recovery and bread-reserve investigation now connect evidence to existing controls. Keep future UI work tied to demonstrated problems.
- **The building set is sufficient for the next scenarios.** Seventeen types already cover food, homes, work, storage, access and recreation. The earlier review's cost table remains the building reference. Costs alone omit labor, travel and land: cheap gardens, grain/bread, fishing and hunting should be compared in their intended landscapes before repricing them. Halls and comfort reduce repeated journeys; neither has demonstrated a general food-output benefit.
- **Map expansion should first mean better authored space.** Irregular ground, water, crossings, raised terrain and clearing exist. Use competing sites, distant resources and evolving neighborhoods before increasing raw map size or adding terrain sculpting.
- **Audio and menu have first versions.** Retain their polish backlog. New needs, production chains and population growth are candidates, not prerequisites for making the current game enjoyable.

## 1. F26c2b — a playable Living woods settlement (delivered)

Implemented as level nine; [route and UI evidence](WOODS_CAMPAIGN_REVIEW.md). F23b7 and F23b8 are also delivered; F11b5/F18b5 is also delivered; F21n/F07c3 is delivered too; F11b6/F18b6 is delivered; F12g/F23b9 is delivered; F09b/c/d are delivered; continue with F19c. The remaining scope below records the acceptance brief.

Use the [budgeted prototype](LIVING_WOODS_F26C2.md). Integrate campaign selection, arrival guidance, earned milestones, an explicit assessment start, replay and saves. Support both preservation/hunting and selective clearing/cultivation. Count real meals, mature habitat and available stock after hunter claims. Explain the difference between pausing hunting and restoring protected trees.

Check both complete routes, additional residents, a saved over-clearing recovery and an intact-but-depleted habitat. Inspect goals and source links at 960/1440. Record when decisions occur and how much of the final stage is idle. If the full route is only an opening checklist, leave challenge unresolved and revise its commitments; do not call it a longer mastery level.

## 2. F23b7 — woodland character and visible consequences (delivered)

[Matched comparison and verification](WOODLAND_ART_F23B7.md). The acceptance brief is retained below.

Give woodland edges, hunting lodges and tracking clearings a coherent normal-zoom presentation. Start by capturing intact, actively hunted, depleted and recovering states. Strengthen the lodge silhouette and habitat context where those views show a weak distinction. Keep actual tree growth and wildlife stock authoritative: no permanent deer or decorative mature trees implying a healthy source.

Show four orientations, construction stages, active work and protected saplings in the same camera/light conditions. Confirm selection, clearing previews, approach cells and pause/reload still agree with simulation. Keep geometry bounded. This is a targeted family pass, not a new ecosystem or renderer rewrite; retain successful existing models where appropriate.

## 3. F23b8 — docks, bridges and inhabited shores (delivered)

[Structure comparison and verification](SHORE_ART_F23B8.md). The original scope below is retained for reference.

Make shoreline structures feel anchored to the terrain, with recognizable supports, deck thickness and clear approach/launch space. Preserve existing footprints, crossing rules and boat routes. Inspect the current scene before choosing details; water effects are optional only if they improve readability within this slice.

Compare ordinary village views before/after. Exercise all legal orientations, construction, boarding/return, a busy crossing and demolition protection. No wider-bridge simulation, freight boats or new shoreline placement rules in this chunk. If a geometry change needs different navigation, split that design out.

## 4. F11b5 / F18b5 — a lasting village, decision prototype (delivered)

[Prototype decision and five measured routes](LASTING_VILLAGE_F11B5.md): conditional go. Successful supper routes finish in 26–31 simulated minutes, with a recoverable poor allocation at 39. These are automated timings, not human pacing. The original brief follows.

Design one later settlement around an imperfect working village, competing prime land and a second player-triggered commitment. Draw from the long-haul and limited-land concepts instead of adding both as overlapping levels. The first improvement should change where the next investment makes sense: for example, a new neighborhood changes which production and services are worth moving or duplicating.

Account for exact materials, labor and accessible plots. Compare two credible strategies and one recoverable poor allocation. Record the decisions before and after expansion, including whether simply prebuilding everything removes the intended choice. Use existing systems; all buildings remain available. Do not enforce one food mix, require every civic building or manufacture difficulty with surprise deadlines.

Deliver a map/route prototype and a go/revise/defer decision. Only promote full campaign integration after the prototype demonstrates a meaningful later choice. Human first-play duration remains TBD.

## 5. F21n / F07c3 — understand and repair a neighborhood (delivered)

[Bread investigation and verification](BREAD_SERVICE_F21N.md). The delivered fix connects supper to actual bread eating and storage evidence, then to existing bakery/pantry controls. No service rules changed. The brief follows.

Start with the finale fixture: distinguish bread baked/delivered, bread eaten, locally stored bread and central bread available for supper. One bakery can be busy while reserves stay empty. Use existing explanations first, and verify any correction against actual counters and destinations.

Follow one actual meal, home-rest and recreation problem in the later scenario or an existing dispersed-village fixture. Trace what the player sees from the falling condition to the resident, destination and available remedy. Compare a central-service solution with a local pantry/layout solution, including setup labor.

Implement the best-supported navigation, wording or placement-feedback correction. Keep the scope to the observed problem; no automatic assignment system, new needs meter or productivity dashboard. Verify the repair changes the actual service outcome and remains readable at 960/1440. If the current interface already explains it adequately, record that result and skip speculative UI work.

## 6. F11b6 / F18b6 — integrate the lasting village (delivered)

[Campaign integration and route evidence](FINAL_CAMPAIGN_F11B6.md): level ten is playable with earned assessments, later supper, extra residents, recovery and replay. The original brief follows.

After the service review, add level ten selection, arrival guidance, earned twelve-person progress, supported twenty-person growth and a player-triggered shared supper. Prevent early supper from banking completion. Support extra residents with explicit current-population requirements, phase saves, replay and completion navigation. Keep all buildings available and avoid prescribed production layouts. Repeat both food routes and saved poor-allocation recovery through real campaign states; inspect Goals at 960/1440. No post-supper waiting gate. Human pacing remains TBD.

## 7. F12g / F23b9 — authored landscape composition (delivered)

[Landscape comparison and route verification](RIVER_MEADOW_F12G.md). Grouped timber, water inlets and blended grass ship with the same timber budget; all four campaign routes pass. The original brief follows.

The prototype overview exposes a sparse eastern bank and regular tree perimeter. Improve ground/shore silhouettes and natural grouping, with clear neighborhood sites and views at ordinary zoom. Preserve the tested resource budget, access and enough credible alternative plots; rerun routes if gameplay geometry changes. Compare matched initial and settled views. This is authored map/art work, not a terrain editor or a larger map requirement.

## 8. F09b — choose cottage finishes (delivered)

[Palette comparison and verification](COTTAGE_FINISHES_F09B.md): five named choices plus Automatic, saved per cottage, with all four orientations and improved homes verified. The original brief follows.

Let the player select a cottage finish from a small, coherent set in its existing inspector and return to the automatic colors. Use named palettes, preview the actual selected house, and retain roof/wall/timber contrast and clear entrances. This is a free cosmetic choice, with no new production chain, work task or satisfaction benefit.

Keep the controls compact at 960/1440. Verify choice persistence, automatic reset, four orientations, construction/demolition appearance and improved-home shutters. Compare a settled neighborhood using both automatic and chosen finishes under matched light. Carry further facade details, connected fences and brush painting forward separately; do not turn this into a general material editor.

## 9. F09c — connected fence runs (delivered)

[Implementation and verification](CONNECTED_FENCES_F09C.md). The acceptance brief below is retained for reference.

Make adjacent decorative fence tiles form coherent straight runs and corners. Preserve the existing solid footprint and protected-access rules; this changes presentation, not where villagers can walk. Isolated pieces retain understandable orientation controls. Show the resulting joins during placement and update neighbors after placement, removal and reload.

Inspect a cottage garden and a longer boundary at ordinary zoom. Verify all connection directions, corners, interruption/removal, adjacent buildings and refused placements that would cut off access. Keep preview and placed models consistent. Gates, new navigation rules and drag painting are separate follow-ups rather than prerequisites.

## 10. F09d — decoration brush strokes (delivered)

[Interaction and verification](DECORATION_BRUSH_F09D.md). The original acceptance brief follows.

Let the existing Place/Remove decorations tools support a held left-button stroke, with one-cell width and the selected decoration/orientation. A click still edits one tile. Interpolate skipped cells deterministically so fast movement leaves a continuous run; choose a consistent cardinal route for diagonal fence strokes and document it. Preview remains read-only; edits happen only while the stroke is active.

Use the existing decoration command and access check at every cell. Do not replace another decoration automatically or spend resources. Preserve accepted edits when a later tile is refused; show a concise reason without repeating error sounds every frame. Locked sunflowers stay locked. Fence neighbors update as the stroke progresses.

Stop or break strokes on release, Escape, focus loss, tool/world changes and entering the HUD; never bridge across the HUD when returning to the map. Preserve camera dragging and the existing path/woodland tools. Verify long/fast strokes, diagonal corners, repeat visits, removal, blocked entrances, crossed UI, cancellation and current saves at 960/1440. Compare the interaction on the cottage garden and a longer planting strip. Broad brushes, fill, gates, terrain sculpting and undo are separate decisions.

## 11. F19c — main-menu keyboard navigation

The title menu uses the shared button helper, which sets FocusMode.None. Add keyboard navigation specifically to the menu and its subpages: a visible initial focus, Tab/Shift-Tab and directional movement, activation, and Escape/Back. Skip disabled actions. Scroll focused controls into view at 960/1440, including later campaign entries. Returning from a subpage should restore a useful parent focus; entering a settlement should release menu focus and preserve existing game shortcuts.

Cover Continue, Campaign, Free play, Creative, Settings and confirmation/error pages. Settings must allow keyboard changes to their actual controls without invoking village shortcuts underneath. A navigation key must not accept replay, overwrite, start a village or quit; those require explicit activation of the relevant control. Keep existing mouse behavior and save/error handling. Do not globally change every gameplay button's focus policy in this slice.

Exercise an entire keyboard-only start/back/settings/replay-confirmation path with isolated test saves, disabled Continue, long campaign lists and narrow layout. Verify visible focus, actual setting changes, canceled confirmations and keyboard-to-mouse transitions. Controller support, title artwork and save-slot browsing remain separate. This addresses an observed entry-point gap; further in-game focus work remains F21.

## Reevaluate after each delivery

Record what shipped, evidence, unresolved player questions and the next recommendation. Keep completed work out of the active queue. Finale integration is delivered; optional comfort/civic ambitions, audio/music polish and menu artwork follow evidence rather than a promise to implement every idea. Seasons and save migration remain excluded.
