# Work and village life — F03b / F04b

## F03b1: logging, implemented

Logging now has a slow wind-up, faster strike and short recovery. Four strokes follow the worker's actual chopping timer, including slower work when hungry. The logger braces closer to the trunk, and the longer axe reaches it at the strike. Chopping sounds follow the impact phase, subject to the existing voice limits and a short real-time cooldown at fast-forward speeds.

A mature tree observed standing tilts away from the logger over 0.9 simulation seconds after the cut, then its canopy collapses into the ordinary timber-pile presentation. It retains its full size through the first part of the fall. There is no new physics simulation, blocked ground, extra yield or delay to resource delivery. Creative immediate clearing still removes trees immediately. Sapling clearing does not invent a mature-tree fall.

Pause freezes both the chopping pose and fall. The work timer already lives in saves; the fall is a transient presentation event. Loading a felled tree displays its saved stump/logs without replaying the fall. Preserving a tree mid-cut releases the work pose and keeps it standing. Regrowth clears the old fall presentation.

### Visual and functional checks

Run `./Play.ps1 -LoggingSmokeTest`. The Godot build and focused rendered check pass:

- Axe blade reference is 0.22 tiles from the trunk center at height 0.55, within reach of the blade's width. Wind-up and strike captures were inspected at 960/1440.
- No chop cue during the wind-up; one cue on crossing the impact phase. Pause emits no extra strike.
- The falling tree tilts at full size before resolving into the log pile. Paused transforms and simulation state remain identical.
- Loading during the visual fall does not replay it. Preserving an active cut leaves the tree upright and removes the axe pose.

Local captures: `artifacts/f03b1-before-*.png` record the preceding implementation; `f03b1-after-*.png`, `f03b1-windup-960.png`, `f03b1-falling-960.png` and `f03b1-falling-village-960.png` show the new version. The smoke regenerates the new captures. The normal-distance view uses camera size 20; close comparisons use 10. These establish a readable action and safe presentation transitions, not player acceptance of the village's overall visual style. No simulation rules or save format changed.

## F03b2: handoffs and hammer work, implemented

Workers reach on the final 0.85 tiles of a collection trip, lift newly acquired cargo over 0.4 simulation seconds, and lower actual loads on the final delivery step. The simulation still controls the transfer: an empty worker never displays reserved goods, and delivered cargo disappears immediately when ownership changes. This covers construction, timber stores, mill/bakery inputs, food collection and pantry delivery, including landed fish. No travel or handling delay was added.

The brief pickup lift is transient presentation. It only starts when a recent rendered observation saw empty hands become loaded, so loading a save or skipping a long period does not replay an old pickup. Delivery lowering is derived from the current route and position, so it restores directly. Cancellation/reassignment follows the actual returned cargo. Pausing now places the rendered body at its simulated position instead of continuing to ease toward it while time is stopped.

Builders and active dismantlers use a small temporary work board with a slower wind-up and quick hammer strike. The board is a tool prop, not extra inventory or a path obstacle. It gives early construction a contact surface before walls exist. Hammer sounds follow the work timer's strike phase. Demolition evacuation and final material recovery use a handling pose; they do not keep hammering while collecting goods. Completion or reassignment hides the board and tool.

`./Play.ps1 -HandoffSmokeTest` passes with actual loads for logs, planks, berries, grain, bread, vegetables and fish. It checks delivery lowering, empty pickup reach, newly acquired lift, frozen poses, exact delivery-pose reload, cancellation with returned goods, disappearance after delivery, hammer/board contact, strike-timed sound, completion and demolition/reassignment. Local captures include `artifacts/f03b2-delivery-960.png`, `f03b2-delivery-1440.png`, `f03b2-delivery-village-960.png` and `f03b2-hammer-960.png`; close views use camera size 10 and the village view 20. The unobstructed close views were inspected. No simulation rules or save format changed. Field-specific work is covered below.

## F03b3: field sowing and harvest, implemented

Farmers now sow with a seed pouch and soil-reaching gesture, cut grain with a curved sickle, and bend to pick vegetables by hand. The pose follows the existing four-second sowing or two-second harvest work timer. A brief visual stance brings the resident into reach of the bed, then returns them to the simulated entrance before collection. Partially harvested plots target a remaining plant, including rotated beds. There is no new tending job during growth.

The seed pouch is a tool prop, not a new seed inventory. Grain and vegetable cargo still appear only on the real simulation transfer; F22's standing/cut crop masks remain unchanged. Growth, yields, routes and save format are unchanged. Pause and reload derive the same pose from saved work progress; reassignment releases the stance and tools immediately.

Run `./Play.ps1 -FieldWorkSmokeTest`. The rendered check covers both crop types and orientations, tool/hand contact against actual remaining crop nodes, partial harvest masks, advancing gestures, actual pickup amounts, pause, exact pose reload and interruption. `-HandoffSmokeTest` also passes as a regression check. Close captures at 960/1440 and village-distance captures are saved under `artifacts/f03b3-*`. Inspected images show a distinct low picking pose; the sickle is subtle at the wider camera. This is a focused presentation improvement, not acceptance of the overall art direction.

## F04b: inhabited homes and gathering places, after work contact

Present actual home rests and square visits with a small set of varied, recognizable social moments. Residents should face the place or one another plausibly. Keep the existing need/service rules: this pass does not add fatigue, friendship meters or compulsory recreation buildings. Observe an inhabited settlement before multiplying gestures.

## Roadmap review

F03b1–3 logging, handoffs/hammer work and field work are complete; F04b home/social presentation is next. Start with believable facing and quiet actual visits, not more constant gestures. F23a still needs aesthetic feedback before the full building-family pass; river/lake human pacing is also open. F07c food services and F26b quarry/hall remain later decisions. This work uses existing residents, so no population or map expansion is needed.
