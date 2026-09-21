# Whole-game review75 — September21,2026

Fixed source/build: `87de771`. Outcomes72–75: place watching, direct yard-ground choice, shared-meal world controls, and the matched-inventory grouped farmstead. User requested execution through the next review and confirmed no human playtest yet. No76 implementation.

## Verdict and direction

**Partially convincing as a small village-arrangement game; motivation and New France identity remain unaccepted.** Real crops/meals, shared labor, reversible changes and voluntary finishing support a coherent pleasure: shape a useful place, then watch people inhabit it. Better controls now make that possible. They do not establish a personally wanted intervention or a sustained reason to continue.

The opening already provides12 housed residents, working cultivation, paths and generous initial food. It can ask the player to invent a task after the interesting livelihood problem is solved. Furnishing may become compliance with the offered button. The grouped layout corrects the capacity confound: six homes, two fields, one garden,36 productive tiles/48 crop, identical people, woodland and starting supplies match the bank. But it compares developer-authored arrangements; it does not prove player-authored transformation is satisfying. Later51 versus48 food is a route/work consequence, not starting capacity or preference evidence.

**Decision: stop automatic interaction/catalogue expansion. Next test one visibly awkward but viable home-and-land situation with several recoverable responses.** Keep one matched opening as control. Reuse existing fields, homes, crossings, paths and meal places. Moving production, improving access or redistributing useful ground must lead to different visible scenes and ordinary journeys. No hunger emergency, compulsory bridge, new need, population quota or timer. Reject a universally dominant cheapest solution. Current code and successful tests do not justify retaining the whole hybrid indefinitely.

The substantial competing direction is an explicit domestic composition game: fewer foreground concepts, stronger before/after land composition and daily spectacle, broad economy kept secondary. Choose that if people want expression and find administration incidental. The current uncommitted hybrid risks the burdens of management with too little expressive payoff.

## Independent findings and disagreements

| Role | Verdict / strongest finding |
| --- | --- |
| Game designer · fresh | Partially convincing. Fairer opening comparison still does not test a wanted transformation. Shared meals take precedence over home meals, so adding commons can suppress the advertised domestic payoff. Test a deliberate geographic/place dilemma; don't restore archived service certificates. |
| UX/onboarding · fresh | Partially convincing. Grouped clearings and explicit yard commitment improve clarity. Three layouts × two modes and nine always-visible resource counts make the comparison apparatus feel like the product. Prefer one public opening and contextual information after choosing the direction. |
| Development lead · fresh | Partially convincing. Concrete relocation/commons and Watch/preview edge cases need closeout. Retain the public-place identity boundary and real physical rules; don't rewrite architecture before deciding the game. Hunger results are observations, not an asserted guarantee. |
| Playtest evidence auditor · reused review60 context, independent of other75 findings | Partially convincing. No actual discovery/motivation evidence. Prioritize a counterbalanced human bank/grouped comparison before selecting more features; furnishing language may prime the apparent preference. |
| Visual/audio · dependent UX-context supplement | Partially convincing. Grouping is stronger, but uniform lawn, geometric shore/inlet and repeated roofs still dominate. Redesign ground/material/silhouette hierarchy, not another small prop pass. Audio architecture has rationale; listening was unperformed. |

**Reviewer limitation:** fresh-agent creation repeatedly hit the thread limit. Three fresh independent roles plus an independent reused older context supplied the four core disciplines. The visual/audio pass reused the current UX reviewer and is dependent, **not a fifth independent verdict**. Whole-game coverage is recorded, but full five-role independence was unavailable; do not describe this as five independent reviewers or a passed audio/playtest review.

Designer/UX lean toward a motivated geographic transformation next; the playtest audit prioritizes observing the existing matched pair first. Lead synthesis: the next implementation hypothesis is the bounded dilemma, but human observations take precedence if supplied. No automatic batch of unrelated improvements. Composition versus livelihood preference remains unknown.

## What earns retention; what does not

Keep shared labor, physical goods/meals, ordinary rest/recreation, recoverable arrangement, optional arrivals, voluntary finishing and matching Normal/relaxed daily life. These make choices consequential without forced growth. Keep direct ground selection and unobstructed place watching because they connect intention and consequence; never fabricate a visitor for the camera.

Contain archived campaign, court and legacy Creative behind developer entry. Their later geographic constraints may be reused; required deliveries, recent-service windows, population targets and assessment gates do not become desirable merely because they work. The full catalogue and overlapping yard/commons/seating garden/square/hall functions remain hypotheses. No new needs, venues or production chains as compensation for weak motivation. Current building availability is not proof of distinct player value.

The public menu and inventory strip should eventually reflect the retained activity, not every experiment. That is dependent on a chosen direction, not an excuse for another standalone UI batch. More density is not the visual goal: the fresh32-person archive obscures small daily life under roofs, labels and props.

## Presentation and evidence

Grouped960 and opposite1440 views read as smaller working clearings rather than identical rows. Real harvest maturity/depletion and moved resident positions are visible across sampled1× frames. Warm simplified materials, low broad homes, grounded shadows and readable crop fields deserve provisional retention. Extensive continuous lawn and sharply geometric water still make it an arrangement on a board rather than a distinctive inhabited New France landscape.

Prefer a coherent illustrated farmstead treatment: larger worked-soil/worn-approach/rough-margin shapes, meaningful bank transitions, differentiated silhouettes and usable domestic ground readable from both sides. Reject improvements visible only close up or only with title/labels. Clothing/hat identity requires dated reference, not a generic period assertion. Existing spatial sound limits, mix controls and quiet music intervals have implementation justification only.

| Evidence | Result / limitation |
| --- | --- |
| `artifacts/review75-current.log` | All21 current-experience suites passed on fixed75 build. |
| `artifacts/review75-regression.log` | Broad simulation regressions passed on fixed75 build. |
| `20260921-115942-723-grouped-farmsteads-8cab01` | Same-code precommit960 actual scripted entry, furnishing, field move, exact save/restart and relaxed identity; opening/menu inspected. |
| `20260921-120238-976-grouped-farmsteads-4b6e65` | Clean fixed75 grouped1440 opposite orientation. |
| `20260921-120512-451-grouped-farmsteads-298803` | Clean fixed75 lived grouped snapshot,1×20-second recording;492 frames/20.5-second WAV extracted. Sampled stills inspected, not continuous motion/listening acceptance. |
| `20260921-120552-511-court-experience-88b435` | Fresh fixed75 archived Free court still. |
| `20260921-120649-041-dense-7aef71` | Fresh fixed75 dense32-person legacy village still. |
| `20260921-115217-910-cultivated-bank-8b0f1a`, `20260921-115459-680-cultivated-bank-624173` | Earlier-in-batch direct yard/watch/shared-card regression evidence; not fresh independent play. |
| `20260921-120812-160-cultivated-bank-638b33` | Fresh fixed75 bank control run reproduced atomic save replacement denial:11 attempts,425ms,0x80070005. Run failed truthfully at return-to-menu. |

No independent hands-on discovery, human preference, listening, continuous-motion/contact judgment, natural-growth session or isolated native performance measurement. Campaign source and regression tests were reviewed, but no new introductory/later campaign UI traversal. Movie encoding and scripted setup are not performance or enjoyment evidence.

## Corrections and rejected findings

Reproduce and close the furnished-yard relocation into existing commons edge case, and cancel a yard proposal when entering Watch so a single Escape exits consistently. Corrections count zero. Existing building obstruction may intentionally reduce usable yard ground; the summary already explains clearing it. Do not convert this into a land-ownership system or silently outlaw all recoverable obstruction.

The designer's carpenter-purpose concern is **not an exposed public catalogue defect**: `UpdateBuildingGroups` hides the entire carpenter card/button in public hamlets; `MakeBuildingCard` parents its title/purpose under that button. Legacy purpose text remains appropriate for archived workshop rules. No speculative copy change.

Intermittent save replacement denial remains unresolved and has recurred in this review. Unique temporary writes succeed; target attributes were Archive; no actor identified. Prior OS FileIO trace start was denied even via elevated tool execution. Do not blame Dropbox/antivirus, increase retries blindly, or count a later pass as a fix. Current-format reliability matters despite disposable saves.

## Next falsifiable comparison and tooling

Human invitation: “Make one change you want here, or keep it.” Counterbalance bank/grouped order if both are tried. Record attention and intention before opening a panel, alternatives, expected/actual consequence, recovery and reason to continue/stop. Use3×/6× for waits,1× for judging life. Keeping a satisfying place is valid. Arbitrary edits, furnish-every-home compliance, or a payoff detectable only in accounting reject the current arrangement loop. The geographic alternative fails if it is merely an obvious repair instruction, one dominant solution, or a path-statistics exercise.

Accept a small intention → action → consequence annotation in the existing capture index alongside the next actual session/comparison. Beneficiaries developer/reviewers; estimated half-day, low upkeep; validate reconstructing one decision and its relevant before/after evidence within two minutes. No replay framework. A concise current-public-flow source map is optional if it removes repeated discovery work; no architecture migration.

Targeted save-operation evidence remains the reliability priority, not speculative infrastructure. Defer ECS, generalized editor, broad asset migration and another unsupported native automation attempt. Next regular full review80; visual/audio earlier after substantial presentation. Stop this requested run after review closeout, at75.


## Closeout result

Confirmed the commons conflict with a failing regression before the fix: furnish the bank's first home's right side, place commons at(2,-9), then relocation to(-2,-9), rotation0 was wrongly accepted. The same side/claim validation now rejects shared-ground intersection at furnishing, rearrangement and relocation; a current-state invariant detects it. Normal/relaxed counterexample searches and rejection purity pass. Recoverable obstruction by buildings still intentionally reduces usable places and is explained in the home summary.

Entering Watch now cancels a yard proposal before hiding management controls. Native960 verifies no simulation mutation and a single Escape return. Final closeout run `20260921-121502-080-cultivated-bank-98e263` passed all bank controls, including saving; it does not resolve the earlier save denial. Zero-warning build; focused yard-arrangement, normal-commons, home-yards and direct-domestic suites pass after corrections. Current21/broad results above remain the fixed75-build results. No playable increment; stop at75.
