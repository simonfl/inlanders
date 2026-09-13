# Tooling investment for faster design iteration

Requested by the user during the checkpoint-eight strategic reset. Assess this alongside the game direction, before choosing the next implementation sequence. These are candidate investments, not an approved infrastructure backlog.

## Observed friction

- Ordinary campaign observation required many separate native control/capture calls. Some snapshots took tens of seconds, and activating the actual game window was necessary to obtain the right surface. The earlier review produced no play evidence after a capture failure.
- Scenario setup and fixture generation are scattered across individual simulation checks and rendered smoke tests. Some focused visual runs also regenerate substantial simulation scenarios before the UI starts. Existing setup is valuable but expensive to discover and repeat.
- The test and play entry points have many individual switches. Experiment code duplicates campaign preparation, while artifacts and their prerequisites are not consistently discoverable from one place.
- Comparing eight finale arrangements required a purpose-built runner, saved results and manual extraction of comparable fields. This produced useful evidence, but future design bets should not need the same plumbing rewritten.
- Sound acceptance and native frame attribution are still open because the necessary evidence has been harder to collect than numeric checks or screenshots. Better capture should address actual listening/profiling needs, not generate more indirect metrics.

These observations identify costs; they do not establish the best architecture for solving them. Inspect current tools before building replacements.

## Candidate investments to evaluate

| Candidate | What it should make easier | Bounded first version | How to judge the payoff |
| --- | --- | --- | --- |
| Review/scenario launcher | Reach a representative game state and inspect it without repeated manual setup | A discoverable scenario list with fresh/reused fixture options, window size, speed, paused start, and a reproducible run command. Record build/seed/settings. Reuse current scripts and fixtures where practical. | Time and commands from checkout to a chosen inspectable scenario; stale/missing fixture failures. |
| Development capture and inspection tools | Obtain reliable screenshots, short motion/audio recordings and relevant game state together | An explicit development-mode capture workflow with matching timestamps, objective transitions, key resource flows and selected-object state. Assess engine-side capture and available native tools before choosing a mechanism. | Time to produce usable review evidence; capture failures; ability to diagnose a reported problem from the bundle. |
| Reusable experiment runner | Compare alternate rules, layouts or objectives without rewriting setup and result extraction | Shared scenario preparation, named variants, identical starting conditions, configurable observation windows, samples and a compact comparison report. Support failures and counterexamples rather than only pass/fail completion. | Time to add a new meaningful comparison; repeated setup code removed; confidence that comparisons hold intended variables constant. |
| Faster focused validation | Run the right checks and avoid rebuilding/regenerating unrelated evidence | Clear separation of build, fixture generation, simulation checks and rendered checks; dependency-aware reuse tied to relevant inputs. Expose commands through one discoverable entry point. | Wall time for a typical edit → check → inspect cycle, plus evidence that reuse does not conceal stale behavior. |
| Easier content/rule experimentation | Change costs, pacing, scenario constraints and visual parameters without invasive edits | Identify the frequently adjusted values and evaluate a small data/configuration or development override layer. Prototype one alternate design with it before generalizing. | Time to produce and compare variants; clarity of the source of truth; reproducibility without accidental permanent overrides. |
| Repeatable performance/audio captures | Resolve stalls and assess the actual soundscape | A matched scene run with frame timing, autosave markers, settings, recordings and engine/native traces where available. Include quiet/busy scenes and accelerated play. | Can a reviewer attribute a stall or actually listen to a repeatable mix, and compare a before/after change? |

## Investment discipline

For each serious proposal, name the repeated task it eliminates, who benefits, its expected frequency, setup effort, maintenance burden, and how savings or evidence quality will be measured. Use actual timing where available; label estimates. Compare extending an existing script, using an existing tool, and building new infrastructure. Prefer a small tool that removes a demonstrated bottleneck over a generalized framework.

Select only the tools that support the chosen game-design experiments. It can be right to invest in tooling before another feature, and wrong to spend weeks on infrastructure for mechanics we may remove. Do not require a universal editor, plugin architecture, ECS migration, CI platform or deterministic replay system without a concrete repeated need.

Automation should preserve evidence boundaries. Scripted scenarios are useful for regression and comparison; they are not uncoached playtests. Debug access must be explicit and separate from the normal player flow. Faster capture does not itself prove appeal, good pacing or enjoyable audio. Keep fresh fixtures disposable; no save migration or preservation project.

## Required strategic-review output

Rank the strongest tooling investments alongside design work, recommend a concrete first increment and identify when it pays back. Specify which design experiment it enables, and whether it is a prerequisite or can proceed alongside that experiment. Record what is deliberately deferred. Reassess actual benefit after use; simplify or remove tools whose upkeep exceeds their value.
