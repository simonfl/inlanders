# Proposed periodic review team

Proposal from the conversation; not yet an adopted execution policy. No recurring automation or automatic agent spawning is configured.

Every **five playable checkpoints**, review a fixed commit/build with four roles:

| Reviewer | Primary question | Evidence |
| --- | --- | --- |
| Game designer | Do choices, tradeoffs and payoffs justify the features and levels? | Concrete scenarios, alternatives, redundancy, waiting and recovery. |
| UX/onboarding reviewer | Can a player discover actions, interpret progress and recover? | Actual visible controls at ordinary zoom and 960/1440, including failed actions. |
| Playtest agent | What goes wrong when playing without developer shortcuts? | UI actions, confusion, idle stretches, mistakes, recovery and timestamps. Distinguish agent behavior from human enjoyment. |
| Game development lead | Is the implementation reliable, performant enough and maintainable? | Current code, relevant tests/traces, known limitations and next-chunk dependencies. |

Add a visual/audio reviewer every **ten playable checkpoints**, or following a substantial presentation change. Evaluate composition, identity, animation contact, sound repetition and mix from actual images/recordings. Geometry/cue counts alone cannot establish appeal.

Each reviewer returns at most three prioritized findings with reproduction/evidence, player impact and a proposed correction. Consolidate overlaps into fix now, investigate next, defer or cut; amend the roadmap and pick the next five chunks. A finding does not automatically start a rewrite. Run independent read-only reviews first; only the consolidating lead schedules implementation.

Count committed playable outcomes, not individual commits, test-only work or foundation-only code. If adopted, start an explicit checkpoint ledger at the agreed baseline, record the last reviewed commit and next due checkpoint, and keep the cadence in repository instructions. A proposed cadence must not silently become a mandatory gate.

Occasional uncoached human play sessions remain necessary for pacing and enjoyment. An agent that cannot operate the available UI should report that limitation, not relabel source inspection as a playtest.
