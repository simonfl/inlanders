# Critical design review mandate

September 12, 2026, following the user's concern at checkpoint 8. This supersedes the previous bias toward narrowly evidenced corrections and preserving shipped systems. The existing reviews covered the whole project but largely converted their conclusions into incremental fixes. That is not enough.

## Purpose

Decide whether the game is moving toward a peaceful settlement experience that is satisfying to arrange, manage and watch. Ask whether someone would want to play this game, what they would enjoy doing, and whether the current design produces that experience. Technical correctness is necessary; it is not the product verdict.

Nothing earns protection because it is implemented, tested, documented, numbered in the roadmap or expensive to replace. Review the core loop, resources, buildings, labor, needs, campaign, objectives, controls, map, art and architecture as revisable choices. Cutting half the buildings, replacing assessments, changing how labor works, rebuilding the interface or adopting a different art direction are legitimate proposals. Earlier roadmap cuts and exclusions can also be reconsidered as design hypotheses. The actual project constraints—personal, local Windows game—still apply.

Be critical without manufacturing objections. Make a positive case for what deserves to stay, supported by its contribution to the intended experience. A lack of proof that a replacement is better is not proof that the existing design is good.

## Questions every synthesis must answer

- What is the central pleasure of this game today? What decisions or moments actually produce it, and how often?
- Is the player making interesting choices, expressing a personal village, solving a logistical problem, or mostly placing prescribed buildings and waiting for counters? Where does the game sustain or lose interest?
- Do campaign levels offer distinct situations and meaningful recovery, or repackage a tutorial and service checklist? Would different objectives, freer play, different constraints or fewer deeper levels serve the game better?
- Which buildings, resources and needs create distinct tradeoffs? Which duplicate other systems, demand routine upkeep, or exist mainly because settlement games usually include them?
- Does the whole village look appealing and alive at normal play scale? Is the chosen visual language worth developing, or does it need replacement? Do not reduce this to adding details to existing models.
- Does the interface let players act on the world and understand consequences, or require navigating panels to manage the simulation? Can a structural interaction change remove the need for more explanatory text?
- If we started today knowing what we know, what would we build differently? Which current assumptions would we deliberately discard?

## Independent reviewer output

Use game design, UX, playtest, development lead and visual/audio roles for the immediate strategic review. Each works across the full project through its discipline, read-only, at a recorded commit/build. Give all reviewers this mandate and the original player-experience intent. Treat the current queue as historical context, not their assignment to validate.

Each report contains:

1. **Verdict:** is the present direction convincing, partially convincing or fundamentally off course? Explain the player experience behind the judgment.
2. **Strongest case against it:** the most consequential weaknesses and why local polish would or would not solve them. Include concrete play situations, views, source evidence or missing evidence.
3. **Keep / cut / redesign:** assess existing systems, not just proposed additions. Explicitly distinguish a justified decision to retain something from an untested assumption.
4. **Alternatives:** propose a coherent alternative direction where warranted, and compare it with keeping the current design. Consider at least one substantial redesign in the synthesis; do not prescribe radical change merely to satisfy the format.
5. **Design experiments:** identify the smallest playable slice that can fairly test each consequential uncertain bet, the experience it should produce, and observations that would falsify it. A prototype must preserve the essence of the proposed change, not reduce it to an easy cosmetic test.
6. **Evidence and limits:** separate actual UI play, recordings/stills, simulation, code inspection and inference. Record disagreements and unavailable evidence. Do not infer enjoyment from completion, reliable meals, feature count, geometry count or test passes.

## Lead decision and implementation authority

Synthesize before scheduling more features. State the chosen direction, rejected alternatives, what existing work will be removed or replaced, what remains uncertain, and how the next playable iteration will test the direction. Major findings must remain major in the plan: do not translate “the campaign is bureaucratic” into another tooltip, or “the village lacks appeal” into a few props without explaining why that addresses the actual problem.

The user has empowered substantial redesign. The lead may replace the roadmap, prototype alternative mechanics and rebuild systems within the project's intent without another generic approval round. Retain Git history for reference; saves are disposable. Escalate only a real unresolved user preference or action requiring authorization, not the mere size of a redesign.

Prototypes should compare experiences, not only throughput or completion time. Seek human feedback on competing playable slices when it would settle enjoyment or taste; keep independent work moving meanwhile. Unknown taste is a reason to make alternatives concrete, not to claim acceptance or indefinitely avoid design decisions.

## Immediate action

Routine feature execution is paused at checkpoint 8. Run the strategic review against the current full game, including recent ordinary opening/river observation and finale experiments as evidence rather than proof that the game is good. Include the user's continuing dissatisfaction with flat/uncompelling visuals and tutorial-like campaign pacing. Produce the synthesis and replace the delivery queue before continuing implementation. This mandate update is not the review itself and does not advance the playable checkpoint count.
