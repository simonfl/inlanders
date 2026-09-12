# Inlanders working instructions

- After each chunk, reevaluate the roadmap and active queue in `docs/ROADMAP.md` and `docs/NEXT_CHUNKS.md`. Update scope, priorities and status based on evidence.
- Follow the user-accepted reviewer cadence in [docs/REVIEW_CADENCE_PROPOSAL.md](docs/REVIEW_CADENCE_PROPOSAL.md) and update [docs/CHECKPOINTS.md](docs/CHECKPOINTS.md) after each chunk.
- Every scheduled review assesses the entire project/game, not just the latest changes. Cover overall campaign, economy/buildings/needs, normal/Creative play, controls/onboarding, village presentation/audio, and technical reliability/performance; use recent changes as context. Select representative whole-game evidence and record unobserved areas explicitly.
- Every five committed playable outcomes, run independent read-only game designer, UX/onboarding, playtest and game development lead subagents on a fixed commit/build before implementing the next chunk. Add a visual/audio reviewer every ten outcomes and after substantial presentation changes. This cadence explicitly authorizes these reviewer subagents.
- Consolidate findings, save evidence and decisions, amend the roadmap, and select the next five chunks. Review findings do not automatically authorize broad rewrites. Report limitations honestly, especially when actual UI play or listening could not be performed.
- Count playable outcomes once, not commits. Documentation, tests and foundations alone do not advance the count. Do not backdate review completion.
- Save backward compatibility is not required for this prototype. Keep current-format saves correct; do not add migrations unless requested.

- The user treats personal game saves as disposable. Do not add compatibility or save-preservation work for review/testing; current-format save correctness still matters. Do not delete unrelated data.
