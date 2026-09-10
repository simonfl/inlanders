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

## F03b2: remaining work presentation, next

Review delivery handoff, hammer contact, field tending and harvest motions at the ordinary camera. Choose one coherent slice rather than applying the logger's swing to every profession. Any brief pickup/drop pose must reflect actual cargo and remain correct when paused, reassigned, demolished or loaded; it must not leave phantom goods. Use work progress where available and retain recognizable loads. Avoid adding constant particles or prolonging work merely to show an animation.

## F04b: inhabited homes and gathering places, after work contact

Present actual home rests and square visits with a small set of varied, recognizable social moments. Residents should face the place or one another plausibly. Keep the existing need/service rules: this pass does not add fatigue, friendship meters or compulsory recreation buildings. Observe an inhabited settlement before multiplying gestures.

## Roadmap review

F03b1 is the first slice, not completion of F03b/F04b. Work handoffs/contact remain next, then home/social presentation. F23a still needs aesthetic feedback before the full building-family pass; river/lake human pacing is also open. F07c food services and F26b quarry/hall remain later decisions. This work uses existing residents and models, so no population or map expansion is needed.
