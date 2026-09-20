# Whole-game review 55 — a worthwhile opening, an unproven second act

## Verdict and decision

**Partially convincing.** All five fresh independent reviewers reach this verdict. Les Habitants can support making an imperfect settlement work, arranging it and watching everyday life. We have not established the longer, skillful campaign the user asked for. F37 remains a development comparison, not the accepted public game.

The strongest case against the direction is structural: one adequate food investment may remove every meaningful pressure, leaving optional furnishing unrelated to the opening problem. Shared workers resting when provisions are sufficient is desirable peaceful behavior. Raising production quotas, stretching timers or requiring more arrivals would disguise the missing purpose.

**Choose one inhabited-place experiment with a consequential second transformation.** After initial food recovery, offer contested useful ground where a personally chosen common space or home frontage requires revising a working livelihood arrangement. Support at least two approaches and remaining small. Make cultivation, storage, home approaches and actual resident journeys form a readable working landscape. This is not ownership, a household economy or inheritance. Reuse existing systems; do not implement another catalogue tier.

Compare that experience with a deliberately finite arrangement of the same place. A finite village the player chooses to leave and reopen is a serious product alternative. If Normal adds waiting without different decisions from relaxed play, prefer the finite direction rather than manufacture management pressure.

## Freeze, independence and scope

Reviewed source: `53a8e72b372c2e661777a03bd79f615309696b7e`. Five **fresh independent read-only** subagents: game designer, UX/onboarding, playtest, development lead, visual/audio. Each inspected the whole game and representative source/stills; none read another review55 verdict before returning. The lead then separately checked the suspected quarry geometry. Presentation changes triggered the fifth role at55.

The reviews covered primary farmstead, inhabited alternatives, development hamlet, public Free/court, archived campaign and dense settlement, economy/buildings/needs, interaction, presentation and reliability. Coverage differs by discipline; no reviewer personally played the UI or listened. The playtest role audited scripted evidence rather than conducting an uncoached playtest. No enjoyment, continuous-motion quality, audio acceptance or representative native performance claim is supported.

## Independent findings and synthesis

| Role | Independent verdict and central finding | Lead decision |
| --- | --- | --- |
| Game designer | Partially convincing. Provisioning alternatives are viable openings, not a sustained sequence; the current fresh-food finish cannot certify transformation. Prefer two or three inhabited situations over ten prescribed lessons. | Retain physical livelihoods and optional growth; replace the assumption of campaign depth with a second-transformation gate. |
| UX/onboarding | Partially convincing. Public Normal and Free teach different games. Finish/Invite outrank useful planning, opening reserves imply viability before a livelihood exists, and the hamlet brief explains decisions before play. | Keep staging and direct inspection. Next experiment puts intention and world exploration first; shorten entry, move optional ending/growth later, make current reserves distinct from ongoing supply. |
| Playtest | Partially convincing. Six arms establish viable investment and one recovery, but never require woodland release, home rearrangement or a second revision. | Require a decision trace with a stated intention, consequence and voluntary next change; no inference of enjoyment from simulation duration. |
| Development lead | Partially convincing. Current architecture is suitable at this scale, but overlapping scenario flags risk rule/save divergence. Save failures are unresolved and staging coverage is log-only. | Correct concrete regressions and diagnose actual save operations now. Before another public mode, resolve a small explicit place/rule profile; no generic rules framework. |
| Visual/audio | Partially convincing. Sparse building stamps and crowded legacy roofs both fail to make a coherent working settlement; the scene explains less than its panels. | Compare a functional working-homestead composition at ordinary zoom. Prioritize one home, cultivated workplace, shore workplace and shared place, not all eighteen assets. Listen before changing audio. |

There is no material verdict disagreement. There are different emphases: UX proposes persistent alternative plans; design proposes a contested second use of land; presentation proposes a composed working homestead. **Do not implement three new subsystems.** Use existing staging and movement in one authored comparison, then add an interface only when a demonstrated decision needs it. The conservative peaceful sandbox and finite transformation remain alternatives; neither wins merely because evidence for sustained management is missing.

## Whole-game retain, challenge, replace

- **Retain with reasons:** shared labor, actual cargo/meals/stores, optional arrivals, safe movement and pausable construction connect plans to visible consequences and support recovery. Food, home rest and ordinary social activity suffice for now.
- **Economy:** gardens, grain/bakery, shoreline and woodland offer different footprints and dependencies. Equal food budgets show viability, not equally satisfying play. Orchard, lodge and multiple recreation venues need stronger distinct spatial/expressive reasons; keep available provisionally, but grant no automatic progression stage.
- **Campaign:** primary farmstead can be an introduction. Archive prescribed building/delivery/population certificates; reuse their river, lake, quarry and woodland geography. Do not promise a longer campaign through more of the old assessments.
- **Normal/relaxed:** eventually offer the same chosen place and recognizable daily simulation. Current public Free is still the older court; the matched hamlet is comparative evidence, not completed consolidation.
- **Controls:** retain contextual cards and food routes as inspection of current state. They do not forecast production or promise an eventual resident meal source. Prioritize useful world action over Finish/Invite and administrative reading.
- **Presentation:** retain modest timber silhouettes, warm light, water separation and miniature residents provisionally. Replace isolated stamps on a uniform plane as the default composition; adding decorative density is not the remedy. Reduce irrelevant world labels. Domestic improvement must reward looking at the place, not only reading a benefit number.
- **Audio/performance:** mechanisms exist; sound pleasantness, repetition, motion readability and smoothness remain unobserved. No rewrite justified from stills or source.

## Evidence

Final fixed-code six-arm comparison: `artifacts/transformation/report.json`, corresponding manifest and `artifacts/review55-transformation.log`. Twelve residents, 20 simulated minutes; hunger and meal walking measured over the final ten minutes. Gardens/mixed/grain each add12 logs; other arms have different budgets.

| Scripted arm | Added logs | Hungry resident-seconds | Meal-walking seconds | Final food |
| --- | ---: | ---: | ---: | ---: |
| Unchanged | 0 | 270.8 | 1215.1 | 2 |
| Crossing | 6 | 0 | 760.6 | 1 |
| Gardens | 12 | 0 | 389.9 | 48 |
| Mixed shore/garden | 12 | 0 | 374.7 | 51 |
| Grain/bakery | 12 | 0 | 418.3 | 52 |
| Crossing then garden recovery | 10 | 0 | 456.6 | 41 |

The recovery script sees25 food at600s and adds a nearby garden at601s because reserves are below three portions/person. It establishes a conditional recovery, not a human-created plan or sustained mastery. The crossing eliminates measured hunger in this final orientation but ends with almost no reserve; earlier flat/all-same-facing numbers in F37 history must not be substituted. Design/lead initial reports preceded final six-arm completion; the final playtest verdict and this synthesis use these numbers.

Native capture bundles under `artifacts/review/runs/`:

- `20260920-121603-995-transformation-a47076`960 and `20260920-121647-916-transformation-9e1f1c`1440: final candidate code, dirty prior-commit provenance, not clean frozen-commit captures. Actual menu/mode/save, routes, staging and held controls pass.
- `20260920-121934-546-farmstead-54cfe9`1440 and `20260920-121937-712-court-experience-b1a6b8`960: clean frozen source, current public experiences and scripted controls.
- `20260920-121935-380-dense-a04aa7`1440: clean frozen source rendering an imported archived32-resident state. Not natural growth from current play.

Two earlier1440 F5 probes failed with access denied (`121010-255-transformation-9a6c1f`, `121432-947-transformation-a150dd`, same date prefix). Later passes do not resolve them. No evidence identifies synchronization, permissions, collision or a held handle as cause.

## Review corrections and validation boundary

Corrections after the freeze count **zero** playable outcomes:

- Move the stone outcrop one tile east. The new ridge otherwise leaves no level quarry within its four-tile reach. Add actual normal construction, extraction/delivery and exact continuation coverage; occupancy validation alone was insufficient.
- Replace the food-access test coordinate now occupied by a rotated home with reachable clear ground and an explicit fixture precondition. This was a test error, not route failure.
- Diagnose atomic save operation, actual target/temporary path, HResult and replacement attempts without increasing retry duration. Keep the original exception if cleanup also fails. Distinguish successful F9 slot save from failed Continue update in UI reporting. The intermittent failure remains open.

Final game/test build passes with zero warnings/errors. Atomic lock checks pass, including exact failure context, prior-save retention and40 successful replacements (`artifacts/review55-atomic-closeout.log`). The corrected native960 probe passes menu/brief/modes, save/Continue/restart, food routes, staging and controls (`20260920-123124-054-transformation-8a0aa0`); its food-route still was inspected. This is post-freeze dirty candidate evidence, not an independent rerun by reviewers. All **fourteen current-experience suites pass** on corrected source, including staging, legal quarry production, food routes and active-save continuation (`artifacts/review55-current-closeout.log`; report assembly `76e3104d-c7a6-4130-90ce-c004131bde32`). The initially failed thirteen-suite run is retained as historical evidence rather than described as a pass. Existing broad regression pass predates final home orientations; independent review captures predate these bounded corrections. Mixed plank/stone staging and pause→cancel accounting remain a targeted coverage task, not a known defect.

## Next experiment and rejection criteria

Use identical geography, recipes, resident count and daily behavior in Normal/relaxed. Observe a twenty-to-thirty-minute uncoached session when available; accelerated simulation is useful for supply/recovery, normal speed for readability and sound.

Record: what the player wants to improve; alternatives considered; a consequential first change; an imperfect intervention and recovery; a second intended change; why they continue or stop. Do not script the desired answer in the brief.

Reject sustained transformation if one food investment ends meaningful choice, the second phase is one obvious purchase, staging becomes bureaucracy, or continuation comes only from expected objectives. Reject the finite alternative if players want consequential continuation and feel abandoned. Reject the presentation change if livelihoods/consequences still require panels or are merely preferred decoration. With no uncoached evidence, mark the gate **unproven**, never accepted by passing tests.

## Tooling investment

| Investment | Beneficiary / expected savings | Cost and validation |
| --- | --- | --- |
| Atomic failure context | Developer on each recurrence; avoids guessing which of slot/Continue/write/replace failed | Bounded correction now; low maintenance. Permanent-lock test verifies actual target, operation and attempts while preserving prior save. |
| Fixture preconditions and resource usability | Developer after authored terrain/layout edits; catches hidden loss of a production chain | Small tests now. Verify real quarry production and clear query ground, not duplicated geometry formulas. |
| Extend existing intervention bundle | All reviewers; less manual reconstruction of intended changes and consequences | Target under one day, low maintenance. Timestamp actions plus before/after state/stills and separately stated player intention. Success: another reviewer reconstructs a revision unaided. No telemetry framework. |
| Matched short motion/audio capture | Presentation reviewers/developer; repeatable ordinary-speed comparisons | Target half–one day using existing capture path. One invocation produces two attributable audible clips; measure setup time. Do not claim listening until performed. |
| Small explicit place/rule profile | Developer before public-mode consolidation; reduces repeated scenario/save exceptions | Conditional narrow refactor only for retained modes, no migrations or generic rules engine. Validate equal rules and distinct save routing. |

Playable count55. Review55 complete; next periodic and regular visual/audio review60, earlier if direction is challenged or presentation substantially changes. Stop this requested run after closeout; no checkpoint56 implementation.
