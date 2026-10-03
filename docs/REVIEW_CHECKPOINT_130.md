# Whole-game review 130 — October 3, 2026

Fixed commit `9cacabfd616c151d549bde2f877d7eebfe1d05e3`, DLL `52F422EC646A79C8C0C402B4EC17CDF4F456C66AA8A00EB2369A78A779C5A347`.

## Verdict and direction

All five disciplines: **partially convincing**. The working clearing is a better experiment than the already composed hamlet: four occupied homes and two distant small strips expose a relationship someone could choose to change. Actual construction, crops, meals and trips can make arrangement feel inhabited. Direct field editing, proposed home forms and contingent route previews reduce friction. Descriptive daily life removes another evaluation ladder.

The strongest objection is unresolved: we may be making an increasingly capable village editor without establishing why anyone wants its edits. A clearing that remains viable unchanged correctly permits modest living, but improvement must earn attention through preferred composition or visible daily consequences. Bringing fields closer could become an implied repair puzzle instead of personal authorship. A short satisfied visit is success; indifference after understanding the controls is not. Green tests and ten further outcomes do not decide between them.

**Stop at130.** Keep the existing beginnings available for a human comparison; add no automatic131 queue. The next direction depends on whether the player values composing a place, solving livelihood problems, both, or neither. Do not answer uncertainty by adding needs, catalogue breadth or more presets.

## Independence and disagreements

Five read-only role assessments were completed before synthesis. Thread quota prevented restoring the former125 lead and fresh contexts, so reviewers reused independent prior contexts:125 design,125 UX,120 play,120 design reassigned to development lead, and115/120 visual/audio. They received the same fixed boundary and did not read one another's130 reports. None operated the UI or listened; playtest is explicitly a source/log/still audit, not independent live play.

- Design: the unfinished home–work relationship is a more useful experiment; retain all three starts for comparison, and observe wanted change and actual use.
- UX: genuine progress in direct manipulation and recovery, but three peer beginnings and a scrolling mode menu expose development comparisons as onboarding. A future product should recommend one beginning while retaining alternatives. Expanded home editing still mixes many actions.
- Play: distinguish personal alteration from following an implied repair. A possible route must not become a guaranteed optimization promise. A twelve-second observation cannot establish sustained watchability.
- Lead: current local simulation/preview engineering is appropriate; cancellation repair covers a meaningful lifecycle. Correct a misleading limited-food preview. Avoid an architecture rewrite without a demonstrated seam or bottleneck.
- Visual/audio: permanent shared stools are more legible than target rings, but broad lawn, repeated roofs and stepped banks remain the composition problem. Individual props do not resolve it. Audio is wholly unauditioned.

The principal disagreement is presentation of alternatives: design retains three experiments; UX wants a clearer recommended first visit. Keep alternatives for this immediate comparison, then choose the default from observed preference. Do not add another start to avoid making that decision. Reviewers also distinguish a bounded landscape toy from a small logistics game; these are competing directions, not a mandate to combine more systems.

## Whole-game assessment

Public founded, inhabited and working-clearing play test creation, alteration and an imperfect relationship respectively. Normal/relaxed preserve comparable daily life while varying material/hunger friction; legacy foodless Creative remains a different historical mode. The smaller clearing has different population and inventory, so it is not a controlled equal-economy variant.

Keep real work, physical materials/food, home visits, recoverable edits, modest scale and optional stopping because they let choices become visible in a place. The full catalogue remains accessible as requested. Its nineteen building types have not earned equal prominence: field/shore/woodland livelihoods offer geographic distinctions, while processing, storage layers, housing variants and overlapping recreation capacities need player-valued differences before expansion. No new needs, resource families or services selected.

Keep the archived campaign separated. Early named-building recipes and later growth/service/meal assessments express a different product promise. Later river, lake, woodland and quarry geography may support future situated problems with multiple solutions; restoring prescribed populations and recent-recreation tests would not answer the current question. Intro/later/dense scenes were sampled, not traversed as a campaign.

Presentation retains readable people, substantial small homes, actual crops and a practical landing. The working clearing makes distance legible but can look unfinished. A potential structural art change is one continuous river landscape composed of related farmstead groups, longer productive ground and irregular woodland, initially retaining comparable simulation. A renderer rewrite or catalogue-wide decoration pass is not justified. Current procedural work cues, ambience and spaced music need a real quiet/busy/accelerated audition before acceptance or replacement.

## Corrective follow-up — zero playable increment

The lead found that home connection previews only inspect vegetable plots, ovens and landings but said homes needed those livelihoods. Foraging, hunting, orchards and stored food can support a village too. Correct the empty-result wording to explicitly identify the preview's limited subset and say other sources are not shown. No simulation or route algorithm change, no extra checkpoint. The independent lead verified this exact delta and considers the wording finding resolved; broader route coverage remains limited. Fixed130 images precede this narrow wording correction; final regression and subsequent build are recorded separately below.

## Evidence

Local bundles under `artifacts/review/runs/20261003-`:

- `144230-957-river-hamlet-66d421`: clean fixed130,1440 ordinary1x12-second before/after and frame samples.
- `144302-196-dense-fdd7b0`: clean fixed130,1440 dense archive1x12-second sample, run sequentially after ordinary and before regression.
- `144420-232-working-clearing-relaxed-8ce832`: clean fixed130 relaxed1440.
- `144442-133-opening-23d3ba`, `144511-521-river-60e4a4`: clean fixed130 archived intro/later960.
- `144016-477-working-clearing-89962c`: same130 DLL, **dirty precommit**960 actual menu, household follow/return, save/load, ordinary life and move/cancel preview;24.55s.
- `143934-282-commons-recurring-a90a8c`: same130 DLL, **dirty precommit**1440 shared-ground scene and15-second1x observation;30.17s. Not public-opening evidence.
- `143711-232-river-hamlet-d23d32`: predecessor129 actual960 turn/appearance/path/survey/Daily life;23.78s.
- Earlier121 field delivery/extension and127 edge-drag native evidence is in the ledger;126 keep/near-field/landing branches each ran60simminutes in both modes with no hungry ticks and exact continuation. These are viability tests, not enjoyment tests.

Play reviewer independently summarized positive-WallMs rows:

| Scene | Frames | Median | P95 | Maximum |
| --- | ---: | ---: | ---: | ---: |
| Ordinary130 |902|11.923ms|17.935ms|75.599ms|
| Dense130 |512|21.800ms|29.546ms|198.768ms|

Short isolated samples show a dense-scene concern, not a proved regression, identified bottleneck or acceptance of motion. Known108 save replacement denial remains unexplained; later successful saves do not close it.

Unobserved: uncoached human choice/enjoyment, independent live UI play, continuous-motion acceptance, actual listening, full campaign traversal, exhaustive building use and fresh legacy Creative operation. Some native selections use fixture APIs; injected ordinary controls are not discovery evidence. Historical authenticity remains provisional.

## Tooling decision

Accept reuse of existing select→preview→apply→actual-use captures, plus the four new meaningful lifecycle/daily-life/clearing suites in the current-experience runner (41 total). Developers/reviewers benefit from catching extension cancellation and insufficient observation horizons in normal validation; setup is small and maintenance limited to those behaviors. Measure successful inclusion and useful failures, not test count as product quality.

Defer the lead's bounded performance investigation until a reproduced stall or next authorized reliability work: estimate half a day to inspect existing ordinary/dense worst-frame markers, low ongoing cost, benefit is avoiding speculative optimizations. Validate by correlating a repeatable spike with a named operation; do not optimize an unattributed maximum. No generalized telemetry, ECS or new audio pipeline. Existing tools already provide the comparison; human attention and listening are the missing evidence.

## Proposed next decision, not an implementation queue

Try Play → Tend a working clearing → New clearing. Choose anything worth changing, or leave it alone. Afterwards ask what was worth preserving, why an alteration was wanted, what changed visibly in ordinary life, and whether stopping felt satisfying, confusing or indifferent. Briefly compare founded/completed-hamlet and Normal/relaxed only if useful.

If composition is valued and management interrupts it, deliberately narrow toward an inhabited landscape toy. If logistics are valued, test one geographically constrained livelihood problem with multiple solutions, reusing stronger archive geography. If neither is valued after controls are understood, reconsider the premise rather than add explanation. If only discovery fails, choose one recommended start and simplify mode comparison. No131 implemented.


Final130 validation: all41 current-experience suites passed exit0, run bef67629662c4de8beefe27a569bfb1a, assembly 558ab58f-6ece-4486-b425-0ec5473e255a, log artifacts/checkpoint130-current.log. This includes extension cancellation, relocation connections, descriptive life and all six60-minute working-clearing branches. Corrected DLL FF13CE9E2CB5FA7F0B18C0C87150B66F791A02AB31AC213C91DEB124B4BA78E1 built with zero warnings. The only postreview game delta is the independently verified limited-preview wording; fixed130 native controls/scene evidence remains explicitly before that text correction. Ten outcomes and both whole-game reviews complete. Stop130, no131.
