# Periodic review team

Accepted by the user on September 12, 2026. This is the repository's checkpoint-based review policy. Track progress in [the checkpoint ledger](CHECKPOINTS.md); this is not a time-based scheduled task.

**Mandate corrected after checkpoint 8:** use the [critical design review mandate](DESIGN_REVIEW_MANDATE.md). The purpose is to decide whether this is the right game and what needs to change, including fundamental redesign. Whole-project coverage alone is insufficient if the outcome merely protects the current design and schedules small repairs. The user has explicitly empowered reconsideration of everything already built. The [strategic synthesis](STRATEGIC_REVIEW_8.md) now chooses a comparative neighborhood redesign and records its reviewer-independence/playtest limitations. This does not waive the next periodic review or establish that the alternative is enjoyable.

Every **five playable checkpoints**, review the **entire project and game** at a fixed commit/build with four roles. Recent changes are context, never the boundary of the scheduled review. Assess where the whole game stands: campaign progression and sustained decisions, the complete building/economy/needs set, normal and Creative play, menu/onboarding/management, village appearance and daily life, sound/music, reliability, saves and performance. Each role covers its discipline across that whole-game scope, then prioritizes the strongest findings rather than manufacturing findings for every subsystem.

Use representative ordinary and dense settlements, introductory and later campaign content, and the title/menu. Evaluate how the game looks and reads as a whole, not only close-ups of new assets. Record areas with insufficient evidence explicitly. Feature-specific reviews can supplement this assessment but cannot replace it.

The four roles are:

| Reviewer | Primary question | Evidence |
| --- | --- | --- |
| Game designer | Do choices, tradeoffs and payoffs justify the features and levels? | Concrete scenarios, alternatives, redundancy, waiting and recovery. |
| UX/onboarding reviewer | Can a player discover actions, interpret progress and recover? | Actual visible controls at ordinary zoom and 960/1440, including failed actions. |
| Playtest agent | What goes wrong when playing without developer shortcuts? | UI actions, confusion, idle stretches, mistakes, recovery and timestamps. Distinguish agent behavior from human enjoyment. |
| Game development lead | Is the implementation reliable, performant enough and maintainable? | Current code, relevant tests/traces, known limitations and next-chunk dependencies. |

The development lead also owns the [tooling/infrastructure investment assessment](ITERATION_TOOLING_REVIEW.md). Review the cost of the development and review process itself: startup, scenario setup, build/test selection, debugging, automation, capture and comparisons. Recommend investments by expected repeated benefit and maintenance cost, not by generic engineering best practice. Every strategic synthesis must explicitly accept, defer or reject the strongest tooling proposals.

Add a visual/audio reviewer every **ten playable checkpoints**, or following a substantial presentation change. Evaluate composition, identity, animation contact, sound repetition and mix from actual images/recordings. Geometry/cue counts alone cannot establish appeal.

Each reviewer first returns a whole-game verdict, the strongest case against the present direction, what is worth retaining and why, and the structural changes their discipline recommends. Include concrete evidence and distinguish observation, inference and untested hypotheses. Prioritize findings, but do not cap the assessment at three small defects. Evaluate alternatives, including a substantially redesigned game; use the mandate's required output. Run independent read-only reviews before synthesis so reviewers do not merely endorse the existing queue.

The lead synthesizes agreements and disagreements into a direction decision: retain with a reasoned case, refocus, or redesign. The user has authorized cutting, merging and replacing existing work and substantial redesign within the personal Windows game project. The lead may schedule comparative prototypes and implementation without seeking routine permission again. Evidence does not have to prove a replacement is fun before it may be prototyped; uncertainty should produce a design experiment, not automatic retention. Do not rewrite for spectacle or substitute an architecture cleanup for a player-experience decision.

Only after choosing a direction should the lead rebuild the roadmap. The next work may be a coherent vertical slice or a new core loop rather than five independent patches. State which previous priorities and acceptance criteria are invalidated. Unknown human enjoyment remains unknown; green tests and simulated completion are never substitutes for that judgment.

Count committed playable outcomes, not individual commits, documentation, test-only work or foundation-only code. A feature split across foundation and UI commits counts once when playable. After every chunk, update the ledger and reevaluate the roadmap. Start counting from the recorded baseline; do not retrospectively count earlier work.

At each multiple of five, run the four independent reviewer subagents against the same recorded commit/build before starting the next implementation chunk. At multiples of ten, add the visual/audio reviewer to that same review. A presentation-triggered review is recorded separately and does not reset the regular cadence. Run reviewers in batches if concurrent agent capacity is limited.

Give each reviewer a bounded scope and request read-only findings. Give the playtest agent the objective and ordinary controls without a developer walkthrough; do not claim it is an uncoached human player. Save the consolidated evidence and decisions in a dated review document, link it from the ledger, record the reviewed commit, and update the next five chunks in the roadmap queue. Record unavailable evidence and follow-up checks explicitly rather than treating an unperformed review as passed.

“Bounded scope” means a discipline across the whole game, not a small feature or a restriction to low-risk proposals. Include visual/art direction in every strategic reset, rather than waiting for checkpoint ten. A user concern about direction triggers a review immediately; it does not reset the regular five-checkpoint cadence or retroactively count as a completed review. Use accelerated play for production waits and pause for inspection; use normal speed when motion or timing itself is being evaluated.

Occasional uncoached human play sessions remain necessary for pacing and enjoyment. An agent that cannot operate the available UI should report that limitation, not relabel source inspection as a playtest.
