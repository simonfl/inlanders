# Development process reset

Adopted November 8, 2026 under the user's instruction to reconsider the whole process. This supersedes checkpoint batch planning and the every-ten review cadence. The existing game at 249 is the baseline for bounded experiments.

## Diagnosis

We rewarded delivered increments, generated long lists of plausible improvements, and repeatedly reviewed similar evidence. Reviews correctly identified a weak loop yet permitted another batch of local repairs. Source inspection, scripted success and screenshots accumulated faster than knowledge of what the owner enjoyed. Five perspectives on one evidence set cannot supply five independent observations of player experience.

The latest bridge proposal also reflects implementation convenience. It may create a satisfying spatial puzzle or an irritating artificial penalty. The user's positive response to grove renewal suggests a separate source of enjoyment in expressing an intention and watching useful work happen. Both deserve consideration. Difficulty, reconsideration and efficiency are optional design goals. Calm mastery, anticipation, expression and attachment can carry the game too.

## One active question

Before implementing, write a brief experiment card in NEXT_CHUNKS.md with:

- The moment we want the player to enjoy and why it might matter to this owner.
- The strongest competing explanation for the current problem.
- The smallest playable comparison and the existing baseline.
- What observation would support the idea, what would disprove it, and its time budget.
- The minimum presentation needed for that experience to be judged.

Keep one challenger active. Budget one focused development session, at most half a day, before reassessing scope. A prototype can reuse existing code or replace it entirely. Prefer whichever gets the experience into playable form sooner. Every change must serve the active question or remove a blocker to trying it.

## Build, observe, decide

The builder makes the slice usable and verifies relevant mechanics, failure recovery and current-format saves. A fresh evaluator then uses ordinary controls, attempts an unintended shortcut and tries a recoverable mistake. Report scripted simulation, direct UI interaction, motion inspection and human play separately. Source review alone cannot satisfy the interaction step.

Prepare a fresh start that launches in one action. Ask the owner for roughly 10–15 minutes per design decision, at a convenient time. Provide a neutral premise and functioning controls. Observe intention, confusion, anticipation, moments worth watching, voluntary continuation and what they want to try next. Two useful questions are “What felt worth doing?” and “What would you try next?” The owner owes no metrics spreadsheet or formal bug report.

Record keep, revise or drop before expanding the idea. Allow one focused revision when observation identifies a specific obstacle. If a clearly understood experience still feels empty, replace the idea. If evidence is unavailable, leave it provisional and hold expansion. A repeated unsupported hypothesis cannot earn another cycle through new wording.

A promising first run earns a second play or a fresh layout. Expand the game only when the pleasure survives that return. Then harden the chosen slice, add one experience that builds on it, and evaluate again. Avoid endless prototypes by resolving one question before opening another.

## Presentation and tools

For a game about watching a settlement, timing, motion, sound and visible human activity belong in the smallest credible prototype. Use existing assets or cheap authored staging. A blank prototype can test spatial constraints but cannot settle whether the village feels alive. Capture a short normal-speed example of the intended payoff and check accelerated play separately.

Keep Godot/C# and existing fixtures provisionally because they already produce playable scenes. A stack replacement needs a concrete experiment bottleneck. Budget at most two hours for launch/capture improvements around the first comparison. Target a warm launch under one minute. Additional tooling must remove a repeated cost and demonstrate benefit by the second use. Reuse focused checks during experiments and broader checks when integrating a chosen design.

## Reviews and records

One builder and one independent runtime evaluator are sufficient for a normal experiment. Request another specialist only for a named question. Full game designer, UX, playtest, development lead and visual/audio review occurs when choosing a substantially new direction, promoting a prototype into production, or assessing a substantial integrated release. These reviewers remain independent and read-only on a fixed build. They assess the whole game, name unobserved areas and may recommend cuts or replacement. A design review must conclude with a decision and the evidence that could overturn it.

Use fresh observation to resolve uncertainty. Repeated discussion of an unchanged build reuses its existing review and states the remaining uncertainty.

ROADMAP.md records the chosen experience and deferred bets. NEXT_CHUNKS.md holds one active experiment and its decision. CHECKPOINTS.md retains historical delivery facts. Update these when evidence changes the decision or a slice is delivered. No numeric batch target determines what to build. Documentation and tooling do not advance playable outcome counts.

## Current decision

The owner found The broken crossing distinct but too short, and authorized Before the river closes. Outcome 254 tests preparation for a village growing from four to twelve, with visible deadlines, recoverable shortages and a self-sufficiency trial. Ready players can advance events to expose rather than hide idle time. Deliver this one longer scenario and hold for owner play. Scripted completion and compact captures establish functionality, not sustained enjoyment or difficulty. See NEXT_CHUNKS.md and RIVER_CLOSING_EXPERIMENT.md.