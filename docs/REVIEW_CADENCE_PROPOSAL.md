# Periodic review team

Accepted by the user on September 12, 2026. This is the repository's checkpoint-based review policy. Track progress in [the checkpoint ledger](CHECKPOINTS.md); this is not a time-based scheduled task.

Every **five playable checkpoints**, review the **entire project and game** at a fixed commit/build with four roles. Recent changes are context, never the boundary of the scheduled review. Assess where the whole game stands: campaign progression and sustained decisions, the complete building/economy/needs set, normal and Creative play, menu/onboarding/management, village appearance and daily life, sound/music, reliability, saves and performance. Each role covers its discipline across that whole-game scope, then prioritizes the strongest findings rather than manufacturing findings for every subsystem.

Use representative ordinary and dense settlements, introductory and later campaign content, and the title/menu. Evaluate how the game looks and reads as a whole, not only close-ups of new assets. Record areas with insufficient evidence explicitly. Feature-specific reviews can supplement this assessment but cannot replace it.

The four roles are:

| Reviewer | Primary question | Evidence |
| --- | --- | --- |
| Game designer | Do choices, tradeoffs and payoffs justify the features and levels? | Concrete scenarios, alternatives, redundancy, waiting and recovery. |
| UX/onboarding reviewer | Can a player discover actions, interpret progress and recover? | Actual visible controls at ordinary zoom and 960/1440, including failed actions. |
| Playtest agent | What goes wrong when playing without developer shortcuts? | UI actions, confusion, idle stretches, mistakes, recovery and timestamps. Distinguish agent behavior from human enjoyment. |
| Game development lead | Is the implementation reliable, performant enough and maintainable? | Current code, relevant tests/traces, known limitations and next-chunk dependencies. |

Add a visual/audio reviewer every **ten playable checkpoints**, or following a substantial presentation change. Evaluate composition, identity, animation contact, sound repetition and mix from actual images/recordings. Geometry/cue counts alone cannot establish appeal.

Each reviewer returns at most three prioritized findings with reproduction/evidence, player impact and a proposed correction. Consolidate overlaps into fix now, investigate next, defer or cut; amend the roadmap and pick the next five chunks. A finding does not automatically start a rewrite. Run independent read-only reviews first; only the consolidating lead schedules implementation.

Count committed playable outcomes, not individual commits, documentation, test-only work or foundation-only code. A feature split across foundation and UI commits counts once when playable. After every chunk, update the ledger and reevaluate the roadmap. Start counting from the recorded baseline; do not retrospectively count earlier work.

At each multiple of five, run the four independent reviewer subagents against the same recorded commit/build before starting the next implementation chunk. At multiples of ten, add the visual/audio reviewer to that same review. A presentation-triggered review is recorded separately and does not reset the regular cadence. Run reviewers in batches if concurrent agent capacity is limited.

Give each reviewer a bounded scope and request read-only findings. Give the playtest agent the objective and ordinary controls without a developer walkthrough; do not claim it is an uncoached human player. Save the consolidated evidence and decisions in a dated review document, link it from the ledger, record the reviewed commit, and update the next five chunks in the roadmap queue. Record unavailable evidence and follow-up checks explicitly rather than treating an unperformed review as passed.

Occasional uncoached human play sessions remain necessary for pacing and enjoyment. An agent that cannot operate the available UI should report that limitation, not relabel source inspection as a playtest.
