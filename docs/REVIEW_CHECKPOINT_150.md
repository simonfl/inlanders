# Whole-game review150 — October3,2026

Fixed commit `a354a0a`, game DLL `56D76066BCF617F7D614D0ABC369F14BDAA7D9A4046F81B2A482923A6E22AB2B`.

## Verdict and stopping decision

All five independent roles: **partially convincing, materially more coherent than145**. The recomposed clearing, distinct cottage masses, blended shared ground and ordinary visits support a clear hypothesis: make a modest place, see its inhabitants use it, and revise or finish voluntarily. The visual reviewer prefers the new ensemble to the former four-house square at the same camera scale. This is justified provisional retention, not proof of enjoyment.

**Stop150.** Ten authorized playable outcomes141–150 and full reviews145/150 are complete. Finish the frozen regression, commit/push the review and stop. Do not automatically implement151, new needs or another decoration/control batch.

## Independent findings and synthesis

Five separate read-only contexts were reused:125design,125ux,120play,120design(development lead),115visual. Each read145 synthesis but no other150 verdict. The visual reviewer also supplied the147 presentation supplement. These are independent role judgments with reused perspectives, not five new testers. No independent live play occurred.

| Role | Main conclusion |
| --- | --- |
| Game design | Inclusive bounded visits fix a real hidden rule. The central uncertainty is whether anyone wants a personal alteration once the attractive starter already works. |
| UX | Food advice, clicking shared ground, Watch return and path-origin return now form a coherent interaction. Varied houses must not imply unimplemented capacity tiers. No new blocker found in inspected scope. |
| Playtest audit | Active-work/mixed-yard evidence replaces the artificial all-work-paused demonstration. The improved starting composition may also remove the player's obvious reason to edit. Script success is not discovery. |
| Development lead | Explicit saved quiet intent, bounded duration, work priority and exact continuation are proportionate. Current code does not reveal a new concrete save/path defect, but frame outliers and the historical save denial remain open. |
| Visual/audio | Prefer the recomposed clearing: fields and varied homes read as one ensemble. Blank rear walls, partly hidden benches, angular banks and small productive extent remain limits. Stills cannot accept gestures or sound. |

There is no major disagreement about provisional retention. Visual makes a positive preference judgment from matched images; the other roles explicitly decline to infer a better activity from a better starting object. Accept both: retain the visual change while leaving motivation unresolved. Do not deliberately spoil the village or create arbitrary scarcity to manufacture a required improvement.

## What deserves to stay, and what does not

Keep physical materials, food and journeys; real cultivated ground; shared work; optional growth; reversible placement; current-format saves; descriptive public life and voluntary finishing. These connect authored space to actual residents. Keep multiple small shared places and current seating choices because they support distinct compositions; further variants have not earned a place.

Keep the recommended clearing, founded start and inhabited comparison provisionally. They distinguish making from zero, choosing an improvement and observing an existing village. Normal construction/material friction must earn its place through anticipation and payoff; relaxed retains real daily life while testing lower friction. CreativeCourt also simulates meals without hunger; other legacy Creative contexts may be foodless. Do not conflate these.

The full nineteen-building catalogue remains accessible as requested. Processing/storage alternatives and overlapping yards/shared places/squares/halls/seating gardens have not all earned compulsory prominence. Defer catalogue expansion, additional service grades, needs and population targets. A future smaller primary experience need not remove user access to the wider catalogue.

Archive campaign remains separate. Its introductory recipes and later growth/service assessments are a different contract from free authorship. River, woodland and material geography remain useful design material, but reviving its progression is not the default cure for uncertain motivation. Fresh archive/dense images remain abstract and crowded compared with the public village.

Retain focused world proposals and the select–arrange/connect–observe–return flow. Native tests now exercise cancel/apply and origin camera/selection. This establishes operation; uncoached discoverability is unobserved. No generic UI redesign is justified without specific friction.

Retain the renderer and house family.147 changes massing rather than only colors;148 removes isolated pads;149 connects home and cultivation more closely. Cut the former layout as the default but preserve its development comparator. Defer facade ornament, furniture variants and catalogue-wide remodeling. The blank shore-facing gable and partially hidden bench are composition tradeoffs to observe, not automatic new checkpoints. New France recognition without the title and exact historical authenticity remain unaccepted.

## Structural alternatives and next discriminating evidence

The current choice is an inhabited-landscape game with a valid short satisfied session. If starting again, its core would be a few farmstead ensembles and visible ordinary life, with management retained only where it gives a chosen change meaning.

A logistics-led alternative would need genuinely different land/work situations and competing investments, not another need ladder or an artificially exclusive commons/field site. The141 combined arm already falsified that binary dilemma. If neither composition nor provision attracts an informed player, reconsider the premise rather than adding content to it.

For the next human session, invite a place worth keeping without a sequence or required duration. Observe the wanted change or preserved feature, whether the player can realize it, what actual use they notice, and whether they finish satisfied, confused or indifferent. Compare Normal and relaxed only if the material waiting itself matters. A second edit is evidence of interest, not a required success condition.

These are future hypotheses, not an authorized151 queue.

## Evidence and reliability

Fresh fixed150 captures under `artifacts/review/runs/20261003-`:

- `225645-649-ordinary-outdoor-life-acf953`: ordinary1440 twelve-second1x process advancement from ten minutes of active production with furnished/shared ground.
- `225705-206-working-clearing-relaxed-33106b`: opposite1440 public view.
- `225712-113-opening-9e8036`, `225717-607-river-ab6e4c`, `225727-576-dense-d3e0b3`, `225748-327-creative-court-6e2913`: representative archive/Creative views; dense includes12s1x trace.
- Native960 `225453-056-shared-places-d6fed0` verifies actual injected path preview/cancel/apply, selection/camera return, independent-place edits and save/load. Same-DLL precommit evidence.

Predecessor149 comparison: `225218-443-working-clearing-3909d2` and `225237-545-clearing-original-609afd`, same1440 camera after ten actual simulated minutes. Starting population, housing, supplies, crop capacity/growth and building investment match. Subsequent work/crop/food states differ naturally;13 versus15 vegetable meals is not a preference or throughput-equivalence result. Four comparison arms and six60-minute branches passed, with no hunger and exact continuation. Public960 working-clearing225130-924 also passed ordinary menu/household/move/path/Watch/save controls.

Fresh frame traces do not establish smoothness:

| Scene | Median/p95 wall | Worst wall / callback | Observed attribution |
| --- | --- | --- | --- |
| Ordinary |8.128 /11.621ms|95.397 /85.648ms|84.073ms people region, one resident83.8ms; no recorded GC. |
| Dense |21.408 /28.907ms|560.123 /17.407ms|10.778ms simulation,3.786ms actors; most wall delay outside instrumented callback, no recorded GC. |

The dense event occurs early after resuming, but cause is unproven. The prior ground-cache correction does not explain it. Neither engine/hardware blame nor renderer replacement follows from this trace. The108 atomic-save replacement denial remains unexplained; later successful saves do not close it. Current save format54 is deliberate, with no migrations.

Frozen150 regression passed all45 suites: run `30d4f38e1a7146d0addfd4c33f9b2925`, assembly `769565d2-de49-40b0-967c-c0cbfab79b0b`,452.36 seconds summed; log `artifacts/checkpoint150-current.log`. A subsequent narrow correctness check reproduced stale seated quiet intent when removing a home. Home loss/reassignment now interrupts that intent; Normal demolition and Relaxed removal, exact continuation, quiet-life, home-waiting and home-assignment/rest suites pass on the corrected zero-warning build. This correction adds no outcome and does not replace the fixed review boundary; the full45-suite result belongs to the frozen build. No complete campaign/catalogue traversal, independent live interaction, human preference, continuous-motion approval or listening occurred. Audio source still has procedural work/nature cues and synthesized themes with rests; no timbre/mix/repetition acceptance.

## Tooling decision

Retain the bounded active-work/mixed-yard fixture: it exposed145's misleading idle-only setup and verifies real home/work/meal coexistence at low upkeep. Retain same-camera comparison and native consequence checks. These concrete investments earned their place; no generalized behavior editor, replay framework or dashboard.

Defer new implementation after150. Future bounded diagnosis: roughly half a day to attribute the slow resident region using existing state and one finer marker; separately investigate the dense outside-callback event. Beneficiaries are developers/reviewers; success is reproducible attribution and a comparable before/after result, not speculative optimization. For product evidence, one ordinary-speed screen/audio session plus an uncoached edit is likely more valuable (estimated one setup/session hour, low maintenance). A usable recording and understood player intention are the measures; visitor counters are not enjoyment metrics.
