# Whole-game review — checkpoint 220

October 18, 2026 (captures dated October 19 UTC). Fixed reviewed commit: `632f820c1c1e34ce8354ad39099f31f4aabe0bb5`. Five fresh independent read-only roles: game designer, UX/onboarding, playtest, development lead, visual/audio. Each assessed the whole project through its discipline, not only211–220. The ten-outcome authorization is complete; stop at220.

## Verdict and direction

All five verdicts are **partially convincing**. The correction is justified by the human playtest: automatic work replaces a confusing activation procedure, area renewal addresses tedious tree selection, one free-play setup removes the apparent duplicate levels, and authored goals give building a purpose. The proposed pleasure is making a practical village improvement and seeing people benefit. Actual work, arrivals and meals support that premise; whether it is enjoyable remains untested by a person.

The strongest case against the current game is still front-loaded decisions followed by waiting. Successful comparisons place their additions before the first tick, invite automatically and wait for completion. Passing those scripts establishes viable alternatives, not sustained decisions. The new repair variation makes access consequential, but one crossing or a relocated field resolves its central problem before the same newcomer sequence. **Reject it as proof of a deeper second chapter.** Keep it as a short selectable repair variation; do not build a campaign by repeating the four-arrival contract or inflating delivery quotas.

Refocus future design on a **non-growing restoration commission** in an inhabited place. Compare it with the newcomer introduction, which stays as the current opening. The restoration's purpose is to improve existing residents' actual life, with two linked land/access/material choices and multiple viable livelihoods. It must permit a plausible mistake and visible recovery. This is a structural alternative, not another label on the current cut-off-fields variation.

No221 or new batch is authorized. The conditional queue is in [NEXT_CHUNKS](NEXT_CHUNKS.md).

## Independent perspectives and synthesis

| Role | Strongest finding | Direction contribution |
| --- | --- | --- |
| Game design | One-field and farther-field wins do not establish a geographical tradeoff; repair still funnels into identical arrivals and counters. | Keep a short introduction; compare a restoration chapter without population growth. Judge actual choices and beneficiaries, not duration or quota size. |
| UX/onboarding | Purpose and direct repair are clearer, but collapsed settling status represents only newcomer meals; preparation blockers still depend partly on tooltips. | Make any future goal-recovery action follow the actual blocker. Keep actions compact and visible rather than adding an inspector. |
| Playtest | All winning solver plans are front-loaded;60s untouched repair control is too short to prove sustained necessity. | Observe one ordinary session, comparable controls, a poor decision and delayed recovery. Completion is not enjoyment or sustainable supply. |
| Development lead | No confirmed delivery-blocking crash/save corruption; invitation beds and final reachable assignments have a possible adverse-layout mismatch. | Reproduce that case through ordinary controls before changing assignment rules. Preserve current saves and physical consequences; avoid speculative hardening. |
| Visual/audio | The public low-house river scene is clearer than the dense archive, but the arrival is conveyed more strongly by panels than by the inhabitants. Menu art promises a different diorama. | Compare a coherent inhabited farmstead composition and recognizable arrival-to-home event. Use actual motion/listening before more props or audio work. |

Agreement is strong on retaining the corrected public contract and rejecting a claim of substantial campaign depth. Design/playtest emphasize the next decision structure; UX emphasizes truthful progress and targets; visual/audio emphasizes the place and beneficiary that should make those decisions satisfying. Resolve this with one complete restoration comparison, supported by ordinary observation and a matched whole-scene treatment, rather than five independent polish patches.

The substantial alternatives are: (1) retain the current newcomer introduction and deepen its spatial choices; (2) restore an existing settlement without growing it; (3) concentrate the game around one household/farmstead while keeping the broader catalogue optional. Choose2 as the next design bet, with1 retained as control. Defer3 unless ordinary play shows that the village scale dilutes agency or attachment. Do not remove systems merely to manufacture a radical review.

## Whole-game disposition

- **Campaign and purpose:** challenge-led entry stays. The old prescribed-building/service-percentage campaign stays archived. Current situations are introductions/variations, not a finished campaign. A satisfying finite ending is valid; endless growth is not required to replace it.
- **Economy, buildings and needs:** keep actual timber, construction, routes, stored food and meals, renewable woodland and reversible arrangements. Catalogue categories have positive human evidence. Retain complementary food sources and optional domestic/shared places provisionally; no need to force every building into objectives. The current field-heavy comparisons do not establish the value or balance of every processing chain, quarry, orchard, hunting or comfort building. No additional needs, compulsory venues, seasons or inheritance.
- **Normal, Relaxed and Creative:** Normal has real construction investment; Relaxed retains work, meals and purpose with free building/no hunger penalty. They are not equivalent to archived foodless Creative. Free play is honestly one river settlement with a prepared-production option. Optional post-success growth is functional, not proven renewed motivation.
- **Controls and onboarding:** keep automatic shared labor, explicit reserve rest, area timber and direct cut-off-field repair. Keep full catalogue access. Current tests demonstrate ordinary input for specific actions, not uncoached discovery of the whole catalogue or a complete challenge.
- **Village presentation:** retain the warm miniature renderer and low public buildings as a provisional control. Fields, woodland and shore can organize the scene better than increasing decorative density. Dense archival settlement is a stress/contrast case, not the target public look. The primary menu should eventually depict the chosen public scene. A newcomer should be identifiable in the world, not just named in a panel.
- **Audio, reliability and performance:** existing effects/nature/music controls and contact-oriented cues remain pending audition. No listening or continuous-motion acceptance. Current focused checks and historical broad regression support bounded correctness. Short captures do not establish sustained performance; known intermittent stalls need attribution if reproduced in ordinary play. Saves remain disposable; current-format correctness matters, migrations do not.

## Comparisons and evidence

| Evidence | Observed result | Limit |
| --- | --- | --- |
|216 minimum intervention | Homes-only never invited by1200 simulated seconds;750s included at least one unfed resident. One field finished367s, farther field333s, two fields267s without missed meals. | Source plans additions at t=0. The fourth row in `artifacts/216-comparison.json` retains the old label `repair-inventory`; it is **two added fields**, renamed `two-fields-control` in source, not a repair experiment. |
|217 repair | Bridge/move finish Normal192/263s; Relaxed173/249s. Original inaccessible fields cannot be harvested; unreachable-farmer crash found and fixed. | Untouched control lasts60s only. It does not prove eventual failure. Native218 repairs access immediately; it is not delayed recovery after a player's mistake. |
|219 continuation | Both situations in both modes grow to18; automatic residents, restart identity and exact saved continuation pass. | Four seconds of matched continuation after arrival is not a long-run livelihood assessment. |
|220 milestone fixture | Real invitation, arrival, housing, meals and completion at192s; exact current save passes. | Authored solver, not uncoached play. |
|220 native | Challenge entry/save/completion/continue, repair selection, move cancel, crossing preview/place and actual construction/food delivery; consolidated free-play setup; arrival and resident-follow at960/1440. | UI injections plus adopted simulation fixtures and accelerated model ticks. Not a single ordinary playthrough or human enjoyment evidence. |
|215 full regression | All68 suites passed on the fixed215 assembly in956.18 seconds; report `artifacts/215-current-experience-report.json`. | Historical result, **not** a full220 run. Final automatic-work and challenge-focused checks are separate. |
|220 whole-scene capture | Public12-resident/7-building river settlement and archived32-resident/30-building dense settlement load and advance3 simulated seconds at3x. Capture setup/run10.62/14.59s. | Still images and short frame samples, no movie/listening. Startup/capture-adjacent spikes prevent a steady-performance conclusion. |

Fixed-review game assembly SHA256: `F01608B91FF95A5797ABBE8CEDDBEFC173491B26258AA756544D896E9C4D401E`; test assembly: `735B5A848F2FDEA6BEC7017B0CEB81F6CB1C37542C27FFDF7B07374F5F896C07`.

Representative images/logs: `artifacts/220-native.log`, `220-arrival-960.png`, `220-newcomer-960.png`, `220-newcomer-1440.png`, `218-cut-off-field.png`, `218-restored-field.png`, `219-free-play-menu.png`. Fresh fixed220 whole-game runs: `artifacts/review/runs/20261019-011546-366-dense-8af100` and `20261019-011625-913-river-livelihood-d40293`. Reviewers performed no builds, mutations, UI operation, listening or motion viewing. Source/stills/test results are distinguished from direct play. Human pacing, full catalogue discovery, later campaign enjoyment, sustained final-build performance and audiovisual appeal remain unobserved.

## Concrete follow-ups and review closure

Two directly observed presentation errors were selected for a narrow final220 correction: a newcomer walking home was headed “On the way to work,” and the Normal challenge briefing implied a deadline after invitation. Correct the heading/inspection target for the home-arrival journey and state that there is no deadline. These are completion fixes to the existing outcome, not checkpoint221. Final build/native verification is recorded below; the independent whole-game verdict belongs to the fixed commit above, not retroactively to the correction.

Remaining bounded findings belong to the next authorized work: collapsed `Settling ·4/4` can conceal a delivery/housing/current-meal blocker; the person link does not prioritize a housing-access blocker; the compact introduction's duplicate “Choose buildings” action can fall below the fold, although persistent Build remains available. Invitation's total spare-bed condition versus reachable assigned homes is a **source-inferred risk**, not a reproduced ordinary-layout failure. Do not automatically reassign homes or change placement protections without a counterexample.

## Tooling decision and future acceptance

Accept a bounded extension of the existing comparison/Review.ps1 workflow alongside the restoration experiment: explicit fixture dependencies instead of historical numbered filenames, equal observation horizons, first-obstruction snapshots and a compact intervention/resource-use timeline. Beneficiaries are implementation, design, playtest and debugging on every scenario comparison. Estimated cost: several hours to half a day; low/modest maintenance. Expected benefit is fewer manual generation commands and less reconstruction of incomparable solver branches; no measured savings claimed yet. Validate from an empty artifact directory, reuse on an unchanged build, stale-fixture rejection, and a second reviewer explaining the first obstruction without reading solver code.

Use existing movie/audio capture for one normal-speed session before building recording infrastructure. Capture preparation is already practical; absent listening/motion evidence is the main gap. Defer generic telemetry/replay/goal frameworks, architecture migration, new save compatibility and broad catalogue infrastructure.

For the next restoration prototype, require two materially different viable interventions, one meaningful decision after work begins, and ordinary recoverable consequences. Compare actual land/material use, journeys, recognizable beneficiaries and desire to continue. Reject or fold the experiment back into the introduction if one obvious placement resolves it, alternatives produce essentially the same village, or recovery is mainly correcting an arbitrary rule. Do not enforce a minimum click count/session duration or replace that test with larger quotas.

## Final delivery verification

The narrow presentation correction built with zero warnings/errors and the existing complete native probe passed again: `artifacts/220-final-native.log`, with empty error log. Root inspected the regenerated960 resident view: “Arriving at a new home,” “Walking to a new home,” and “Inspect home” now agree. The briefing explicitly states there is no deadline. No new checkpoint was counted.

Final game assembly SHA256: `80CC21230A5E2633CF8621B866777C3728DA4B13D3AE575FD20E1234603E2016`; test assembly: `5D008AE67CCAF70872D2EF8ED3E78FAE7EE6393968B0DBDB7C5EB901C80C1EF9`. Numbered native PNGs were regenerated by this final probe; the original `220-native.log` and immutable whole-scene run bundles above retain the fixed-review evidence. This targeted closure does not turn the independent source/still reviews into playthroughs or a full final-build regression. All ten outcomes211–220 and both scheduled reviews are complete; stop220.
