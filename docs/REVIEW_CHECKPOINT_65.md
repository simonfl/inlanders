# Whole-game review65 — usable changes still need a worthwhile payoff

## Verdict and direction

**Partially convincing.** Les Habitants now has one recognizable public experience: a small inhabited place, the same daily life in Normal and relaxed, recoverable changes and a genuinely voluntary stopping point. These are justified improvements. They establish usable means of changing a village, not that players independently want the changes on offer.

The core pleasure remains shaping a place and watching residents inhabit the consequences. The strongest case against the current implementation is that both openings already work and look orderly. Moving a useful kitchen garden away from a home may make the place worse in a player's eyes. The brief supplies a garden-versus-commons intention; scripts then execute supplied intentions. Neither supports a claim of self-directed motivation. A comparison overlay shows relocation, not an improvement to life.

**Next, prototype working outdoor places as the unit of improvement.** Bring the existing domestic payoff forward: direct shared-labor/material work on home forecourts, then a coherent outdoor shared-place concept rather than several overlapping service buildings. Test substantial functional cultivated land and visible journeys as part of the place, not additional tiny production tokens. Replace the compulsory workshop step if it merely gates domestic life; do not add a new comfort meter or household economy. Keep the low-poly language provisionally and preserve the user's freedom to keep gardens, remain small, or finish.

This is a shift in interaction and economy, not a new checklist. Compare it with the current catalogue-based experience. Do not add another map, resource tier, need, quota or timer to compensate for weak motivation. The existing compact/bank alternatives remain comparisons until evidence selects or rejects them; four save slots do not imply four enduring products.

## Freeze, independence and scope

Reviewed fixed commit `05bff294534dfea131867cc3fe83807806c80557`, test assembly identity `e9b20bbc-adea-4853-b377-4da5d8eb2953`. Five fresh independent roles were assigned in batches: game design, UX/onboarding, playtest, development lead and visual/audio. Agent-capacity failures delayed spawning the later roles; no dependent role substitution was used. Presentation review is required here because the composition changed, even though the next regular visual/audio checkpoint is70.

The review covers public compact/cultivated-bank Normal/relaxed, finishing and reopening, the full catalogue/economy/needs, archived campaign and Free context, controls, presentation/audio and reliability. Recent changes are context, not the review boundary. Post-freeze corrections are separately recorded and add no playable outcome.

## Independent findings and synthesis

| Role | Central finding | Lead disposition |
| --- | --- | --- |
| Game design | Partially convincing. Already working places need a wanted change; making a commons is not inherently superior to nearby gardens. Forecourt life is a strong thematic payoff buried behind a workshop/industrial chain. Social-place rules overlap. | Retain physical goods/life and voluntary finishing. Test direct domestic work and unified outdoor-place interaction, replacing gates instead of adding services. |
| UX/onboarding | Partially convincing. World cards/public consolidation help, but inventory dominates the invitation. Finishing still opens a management menu; comparison emphasizes unchanged homes as much as changed ground. | Correct ending hierarchy and comparison wording in closeout. Put lived consequences before accounting in the next experiment; test an optional resident-led opening, not forced requests. |
| Playtest | Partially convincing analytically; zero screenshots/inputs due to another blocked window-state request. An intact hamlet may invite arbitrary edits; preserving it is valid. Proposes an authored, recoverable spatial inconvenience as an alternative. | Record the failed attempt honestly. Compare a neutral intact opening with a visible repair opportunity only if it preserves multiple personally preferred answers; reject compliance/checklists. |
| Visual/audio | Partially convincing miniature. The three-plot strip does not fairly test substantial worked land; dense legacy labels/roofs are not a growth goal. No listening or continuous viewing. | Retain warm style provisionally, but require real functional land extent and readable people at ordinary zoom. Do not substitute more props, roofs or WAV files for acceptance. |
| Development lead | Partially convincing. Small public profile improves reset/save identity, but UI still derives policy from legacy flags. Options escapes into legacy maps; relaxed tooltip incorrectly disables food; bank title is wrong. | Correct demonstrated public-boundary defects with focused rendered checks. Keep a narrow retained-place policy boundary; no universal scenario framework. |

Presentation supports keeping optional comparison; UX warns that unchanged outlines undermine its meaning. Closeout keeps the comparison while showing only changed footprints.

The design alternative focuses on usable outdoor places, UX on following real resident routines, and development lead on choosing a visibly distinct livelihood landscape. Combine their causal strengths in one bounded prototype: a resident uses a place, the player changes its real land/work arrangement, shared workers realize that change, and the same daily life reveals the consequence. Do not combine them into three independent feature programmes.

## Whole-game decisions

- **Campaign:** one public inhabited place with two composition alternatives is not an established campaign. Keep archived geographic dilemmas for possible later places; do not revive quotas or the building-introduction ladder as a cure for uncertain purpose.
- **Economy and needs:** physical meals, transport, rest, ordinary recreation and shared workers deserve retention because they give land arrangements consequences. Recoverable shortages and optional growth remain. Audit square, seating garden, hall and commons as overlapping player concepts; different capacities alone do not establish distinct experiences. Test removing the compulsory carpenter workplace from domestic furnishing rather than adding another improvement tier.
- **Normal/relaxed:** matched daily life is a valid comparison; differing constraints should not teach different food rules. Mode text and navigation must follow the retained place, not historical Creative assumptions.
- **Controls:** cards preserve the scene and make ordinary actions accessible. Keep detailed stock/assignment policies available, but avoid more permanent planning panels. Starting footprints are an optional reference, not a score or benefit forecast.
- **Presentation:** warm timber, tools/cargo and crops support an intimate place. The bank comparison joins the same18 productive tiles into a strip; it does not test a genuinely larger cultivated-land footprint. Housing/bare ground still dominate. Do not call F38b a completed answer to the working-landscape concern.
- **Reliability/performance:** green tests establish behavior within coverage. They missed public navigation/copy defects. Intermittent target replacement denial remains unresolved; no isolated current performance verdict or audio acceptance is claimed.

## Delivered outcomes

| Checkpoint | Commit | Playable behavior |
| --- | --- | --- |
| 61 | `3c52558` | Public Play opens one inhabited hamlet in Normal/relaxed; archived experiments behind developer access; retained profile supplies creation/restart/save identity. |
| 62 | `3e57ef5` | Finish for now without production/building/population gates; watch, save and reopen the living village. |
| 63 | `00e2b74` | Optional cultivated-bank composition, same population/building inventory/resources/rules, separate matched-mode saves and restart. |
| 64 | `e3fc801` | Compact home/social/construction cards; resident observation, staging/cancel, and workplace relocation returning to the card. |
| 65 | `05bff29` | Persistent opening footprints compared against moved/added places over continuing daily life. |

## Evidence

All17 current-experience suites pass (`artifacts/review65-current.log`, `artifacts/current-experience/report.json`), as do broad default regressions (`artifacts/review65-full.log`). New ending checks cover no prescribed goal, continuing life, exact reload and reopening in both modes. Bank checks cover equal initial inventory/population, actual cultivation, real paths, current saves and retained restart identity. Both ten-minute bank runs recorded zero hungry ticks and47 final food; this is feasibility, not preference or a ranking against compact play.

Native scripted controls exercise public entry, both mode slots, voluntary finish/watch/reopen, world home/construction actions, real garden movement/replanting, opening comparison purity, and current saves. These use actual rendered controls but supplied plans. They are not uncoached play.

Runs under `artifacts/review/runs/`:

| Run | Evidence |
| --- | --- |
| `20260920-180355-490-cultivated-bank-ace82a` | Candidate960 with same playable code as freeze; dirty precursor metadata. Opening, mode choice, home/construction cards, actual move, comparison, saves. |
| `20260920-175604-415-cultivated-bank-30e672` | Earlier candidate1440 opening composition. |
| `20260920-180642-929-transformation-a88e57` | Clean65 compact960 full controls/ending/current saves pass. |
| `20260920-180729-876-cultivated-bank-d589d8` | Clean65 evolved bank1440,1×20-second movie,492 frames and20.5seconds PCM audio. |
| `20260920-180900-696-court-experience-4fa48f` | Clean65 archived Free/court960 controls, ending and current saves pass. |
| `20260920-181103-376-dense-720bd3` | Clean65 renderer with archived32-resident/30-building snapshot. Not natural public-hamlet growth or a performance run. |

Some roles also inspected explicitly historical review60 images/source context. The new movie has a colocated WAV, four extracted JPEG frames, metadata and a small movie index. Extracted frames are not continuous motion, audio samples are not listening, and fixed-frame movie encoding time is not native frame performance.

## Actual playtest boundary

The independent playtest reviewer launched frozen `Review.ps1 Inspect transformation -Width1440 -ReuseOnly`, run `20260920-180834-184-transformation-872110`. Window discovery returned one matching game. Its next window-state request remained pending despite a15-second Promise.race and20-second node tool timeout. Parent interruption ended the attempt after166.1seconds. **Zero returned screenshots and zero gameplay inputs**; no actual plan/recovery or listening. The reviewer then independently inspected source and identified candidate stills. It verified ownership and closed parent1488 and children5964/24396; a final lookup found none remaining.

The failure belongs to observation tooling, not the game. In-kernel timeouts have now failed twice; do not repeat that strategy or promise they bound the next attempt. Use an external supervisor capable of interrupting the blocked tool session, or seek a human session with the existing engine capture. The present environment did not deliver that supervisor; parent interruption is a bounded recovery, not a completed infrastructure fix. Agent/still analysis does not establish human enjoyment.

## Save failure and tooling

Checkpoint64 run `20260920-180025-260-cultivated-bank-9236e6` reproduced target replacement denial after temporary creation:11 attempts,`0x80070005`. Afterward the target had Archive attributes; no evidence identifies the denying actor. The native probe then incorrectly indexed Play after save failure prevented the menu transition. Its guard now reports the save notice directly. The subsequent run `20260920-180134-699-cultivated-bank-4c794a` passed.

AtomicSave now records elapsed time and target/temporary attributes, lengths and timestamps at failure, without changing retries or replacement policy. Focused tests pass for temporary lock recovery, bounded permanent lock failure, intact previous save and40 replacements. The intentional lock test also validates readable failure metadata. This does not resolve the intermittent native denial. A bounded OS file-operation trace is the next diagnostic step; no supported tracing tool was found on PATH during this chunk. Do not blame synchronization or antivirus without evidence, or lengthen blocking retries speculatively.

Accepted tooling work: reuse the one-off movie extraction as `Development/ExtractReviewMovie.py`, standard-library only, with indexed stills/WAV/provenance links. It extracted the current clip successfully and removes repeated manual RIFF parsing from reviews. This is evidence access, not an audio-review substitute. Extend the existing run index with intention/action/result references when a real session exists; half–one day budget, reconstruct one choice in under two minutes. Defer replay frameworks and generic scenario rewrites.

## Post-freeze closeout (zero playable outcomes)

Correct the public Options escape at both visibility and command boundaries, and prevent ordinary Continue from opening an archived ruleset (developer entry remains available), and describe relaxed meals truthfully. Use the retained profile for bank/compact naming. Shorten the entry so mode selection is prominent. Finishing now closes management and presents the village with Watch / Keep shaping / Save and leave; these are corrections to checkpoint62's intended flow. In the comparison, unchanged footprints no longer compete with changed/vacated ground, and the exit says Hide comparison. A final check found that newly placed buildings needed the outline cache refreshed without a full actor rebuild; comparison now refreshes when live layout geometry changes, including additions/removals. Focused native checks exercise both modes of both layouts; final run provenance:

- `20260920-181558-643-transformation-fca7c9`: compact960 entry/ending/current saves/public contract pass; root inspected both visible mode choices and the world ending panel.
- `20260920-181755-424-cultivated-bank-c5905c`: bank960 public contract and ordinary controls pass.
- `20260920-181915-380-transformation-05f16a`: compact960 pass after Continue boundary correction.
- `20260920-182043-616-cultivated-bank-3b2b73`: final bank960 pass, including actual moved plot/newly placed home, changed-only outline refresh, current saves and both-mode public contract. Root inspected comparison0007.

Final build has zero errors/warnings. Native stderr retains the environment's root-certificate-store warning, with no save denial or probe failure in these closeout runs. Broad17-suite/current and default regressions passed on the frozen build; closeout changes UI/profile descriptions and inspection caching, not daily-life simulation rules. Full suites were not needlessly repeated.

## Next queue and stop

[The replacement queue](NEXT_CHUNKS.md) should realize and test working outdoor places, not award another five patches automatically. Whole-game review70 and regular visual/audio70 remain next; substantial presentation changes or renewed direction concerns trigger earlier review. Count stays65. Stop this requested run after closeout, commit and push.
