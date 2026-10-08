# Whole-game review170 — October8,2026

Fixed commit `6525a95`, DLL `C00D2BBE51576EA19AEFB48FFC9967D433B833E1DF67D58D11DE58F696056010`. Five independently reused role contexts: game design, UX/onboarding, playtest audit, development lead and visual/audio. Read-only reports preceded synthesis; no reviewer consulted another170 report. Design additionally inspected the final whole-game capture inventory. This is not five fresh contexts or an uncoached human playtest.

## Verdict and stop decision

All five: **partially convincing, more coherent than165**. Retain the river direction and stop at170. The central pleasure is choosing a relationship between homes, useful work and land, then recognizing people living in it. Establishing a livelihood and tending a working place now describe understandable intentions. The public menu and connected woodland earn retention through concrete improvements; no automatic171 feature queue follows.

The strongest countercase remains substantial: Establish may be one obvious food purchase followed by waiting, after which the player inherits the already-solved Tend experience. Six ready homes focus the food decision but remove part of founding authorship. Tend may be an attractive supplied illustration with no wanted change. No human feedback or continuous viewing settles either hypothesis. A brief satisfied visit, including no edits, remains success; duration and expansion are not acceptance criteria.

## Independent judgments

**Design:** retain the coherent pair of intentions, actual livelihoods and connected landscape. Row collection increases effective vegetable throughput; unchanged crop yield is not balance neutrality. Both tested approaches contain vegetables, so they do not establish the relevance of every other food producer. Fresh dense/archive/Creative images reinforce keeping those contracts separate, without claiming population32 is intrinsically undesirable.

**UX:** primary menus fit960 and describe experiences instead of equal-capacity experiments. One mode/New/Resume flow is clearer. Woodland gathering is mentioned in the arrival invitation but requires the full catalogue; the Everyday palette instead prominently offers field, dock and timber. Possible discovery mismatch, not an observed blocker. The usable/scenic forest boundary is likewise untested with a new player. Freeze further controls rather than treating these hypotheses as demands for new panels.

**Playtest audit:** the native arrival probe covers entry, household inspection, save/load and Watch; it does not establish food through ordinary controls. The new simulation checks immediately place known viable fields or field+dock at former authored locations. They establish pristine viability, not forgiving discovery, delayed action, poor siting, overspending or first-session recovery. The older20-person shortage-recovery scenario cannot stand in for this arrival. No preview-deadlock claim is retained from the corrected165 inference.

**Development lead:** small pose/HUD visibility interventions are justified by repeated measured costs and fresh evidence. No new concrete correctness blocker found in inspected code. Keep current save/restart identity and bounded recovery; reject a new undo or comparison framework. Outside-callback stalls remain a different unresolved problem.

**Visual/audio:**170's woodland and entrance-facing camera are materially stronger; the river–home–cultivation–forest relationship is readable, including partial harvest and the different mixed-food composition. Broad lawn, stepped banks and rectangular worked surfaces remain limits, but another prop batch is not the obvious response. Title aside, identity is still broadly pastoral; dated references should guide any later consequential historical-form changes. Audio and continuous motion remain unaccepted, not judged poor.

## Whole-project decision

Retain physical food/materials, real journeys, visible production, optional finishing, Normal/Relaxed and full catalogue access. Relaxed removes building friction and hunger penalties while preserving meals; CreativeCourt also has meals, while other legacy Creative modes differ. Do not conflate them.

Keep archived campaign separate. Early building recipes and later population/service assessments are a different contract from the current public game. Their river, woods and crossing situations may be useful, but prescribed buildings and compulsory density do not automatically deserve revival. Dense scenes remain crowded with labels and repeated roofs; archived opening and CreativeCourt retain an abstract board-like presentation. If they return to public prominence, reconsider their spatial organization rather than applying catalogue-wide decorative parity.

No new need, institutional ladder, resource, season, inheritance or required growth follows. Existing overlapping storage, processing and recreation options remain available without being made obligatory to justify prior work. Freeze public-start proliferation and arbitrary inspector/arrangement expansion. Retain group recovery as a limited session action that preserves elapsed life; it is not persistent unrestricted Undo.

Twenty four-log trees preserve80 initial logs while changing clearance, obstacles, collection locations and potentially habitat. Four-portion field loads with four seconds of contact reduce return trips and change throughput. Accept both as explicit design experiments, not purely cosmetic or economically identical comparisons. No disagreement requires immediate reversal: reviewers converge on retention with these qualifications, while the desirability of management versus composition remains unresolved.

## Substantial alternatives and falsifiers

A short livelihood-making game could center on different land/supply situations with multiple ordinary solutions and voluntary finishing. The existing prepared settlement would offer immediate composition. If this direction wins, spend effort on meaningful spatial investments and recoverable mistakes, not compulsory building lists.

An inhabited landscape game could instead make free composition and quiet observation primary, with believable production supporting life. If resources merely delay a wanted composition, reduce that friction rather than attach more management to it.

The present establish/tend and Normal/Relaxed entries can distinguish these alternatives without implementing another premise. Establish weakens if understood choices are interchangeable or dominated by an obvious producer, work feels like opaque waiting, or the resulting activity does not feel connected to the chosen arrangement. Tend weakens if understood tools inspire neither wanted changes nor contented watching. Do not demand a second edit or a long session as proof.

Future authorized work should follow an observed failed intention. The most discriminating next evidence is ordinary placement through first useful food, a plausible mistake and recovery, and a quiet household interval at1× with sound. No automatic171–175 queue is selected. Keeping the current candidate long enough to examine it is justified retention, not proof that it is finished.

## Evidence and technical limits

Fresh fixed170 runs under artifacts/review/runs/20261008:

- 191855-648-river-landscape-fa6a6c: supplied settlement, current warmed snapshot,12s1×.
- 191915-831-river-livelihood-025127: mixed food established by simulation, warmed snapshot,12s1×.
- 191935-922-river-livelihood-569841:960 native public entries/modes, household, save/load and Watch.
- 191955-113-ordinary-outdoor-life-2e72d6: ordinary compact clearing,12s1×.
- 192019-998-dense-9aa531:32-person dense archive,12s1×.
- 192042-304-opening-f53367 and192050-549-creative-court-fd3641: archive/Creative stills.

All roles inspected source and representative stills within their scope. Earlier unchanged campaign/catalogue evidence supplies context where fresh traversal was absent. No independent live/uncoached input, complete later campaign traversal, every building silhouette, long-session assessment, continuous-motion viewing or listening. Generated observation frames/traces are not a claim that someone auditioned a recording.

Focused validation: both public modes and two new livelihood approaches sustained20-minute meals without hunger and exact continuation. Eight-row first-crop collection in the controlled Normal case fell160.9→99.8s with unchanged32 vegetables; smaller sizes and relaxed mode also tested. Native field contact, four orientations, remaining crops, real cargo, pause/reload/interruption passed. These establish operation, not equal balance or enjoyment.

Fixed170 wall median/p95/max, then callback on the maximum-wall row:

| Scene | Median | p95 | Max wall | Callback at max |
| --- | ---: | ---: | ---: | ---: |
| Supplied river |12.292ms|18.020ms|82.248ms|2.024ms|
| Mixed livelihood |15.506ms|24.984ms|306.710ms|2.599ms|
| Ordinary clearing |10.054ms|14.850ms|142.201ms|4.323ms|
| Dense |24.920ms|32.820ms|122.815ms|2.992ms|

None of these maximum-wall rows recordsGC. Resource-region maxima1.417/1.640ms and pose-reset maxima0.288/0.357ms in ordinary/dense retain the specific visibility fixes. Most maximum-frame delay is outside the instrumented callback; do not guess the subsystem.169 separately reproduced522ms dense. General smoothness and historical save-replacement denial remain open. Native current saves passed their exercised cases.

Final current-experience regression: **all52 suites passed** on fixed170, exit0, including construction interruption/retained goods and exact saves. Evidence: artifacts/checkpoint170-current.log and artifacts/current-experience/report.json (completed52/52). Build had zero warnings/errors; no gameplay changes after the reviewed commit.

## Tooling decision

Retain bounded pose/HUD timings: each enabled a concrete intervention. Reject a generalized evaluation or balance dashboard; current fixtures/probes suffice for a focused mistaken-opening branch or food comparison. Motion/audio capture and an ordinary session with a stated intention are higher-value product evidence than more activity counters.

If later performance work is authorized, accept an existing engine/native profiler or narrow boundary trace for outside-callback delay. Beneficiaries: developers/reviewers and players experiencing stalls. Estimate half to one focused day including setup, with modest retained capture instructions; success requires reproducing a costly subsystem and comparing an intervention. More timers solely inside the existing callback may miss the present problem. This is a conditional investment, not an authorized171 outcome.
