# Automation, agency and the longer village — November 10

Research and strategic assessment after owner play. Fixed game: commit `22c7dca`, private build `E023B6D3E615`. No gameplay change or new playable outcome.

## Verdict

Retain automatic ordinary labor and the reduced phone interface. Revise the decision structure before adding content. The longer scenario produces useful concern for residents, but its economy mostly rewards adding more homes and fields. Once capacity exists, the ending often certifies a settled plan. Automation exposes that limitation; removing it has not been justified.

The owner reports nervousness when food was low, insufficient choices, and a desire for some active intervention despite liking automation. This is subjective evidence, unlike successful simulations. It does not establish that per-person jobs, frequent priority switching, or harsher shortages would help.

## Play evidence

Owner session `73c89881a437444ea9563543c201372b` completed at 1,303.2 simulated seconds with twelve residents, 73 food, 36 logs, twenty beds and seven fields. The owner issued fifteen building/grove orders, continued expanding after the first boat, and placed the last field at797.1s. The recording's minimum food was four; no recorded hungry resident does not invalidate experienced anxiety. We cannot infer whether extra construction reflected enjoyment, caution or unclear sufficiency.

The independent evaluator's prior API-only run (`f02769c` / `25F3938F2878`, session `0e7d5119eae74e0fb883b9a1371e06fa`) finished at771.2s after its final construction order at227.5s. At the stability timer's expiry,44 delivered portions versus48 meals correctly blocked completion. Eliminating empty waiting must not falsely declare an unsustainable village successful.

Both routes show why action count, surplus and elapsed time are insufficient measures of agency or enjoyment. Owner actions were much more numerous than the lean evaluator's, yet the owner still reports few choices.

## Research: useful developer accounts

These are design accounts and examples, not controlled evidence of a universal rule.

- [Soren Johnson, Old World: Orders](https://www.designer-notes.com/old-world-designer-notes-1-orders/): actions become rote when there is no reason to forgo them. A shared limit makes pursuing one activity displace another. Application here: identify a worthwhile competing use of timber or labor before exposing another control. Do not copy an energy currency.
- [Wube, Factorio: generic interrupts and train priority](https://www.factorio.com/blog/post/fff-395): replace repeated item-specific train instructions with reusable rules, while retaining ways to express priorities and diagnose exceptions. Application: let villagers repeat understood work; intervene when intentions conflict, rather than re-entering the same instructions.
- [Michał Ogłoziński, Against the Storm developer interview](https://www.gamedeveloper.com/business/how-against-the-storm-managed-to-mix-city-building-and-roguelite-play): stability creates a pacing problem; their run structure ends successful settlements before dominance becomes boring. Variable resources and several routes to success support adaptation. Application: a longer timer is not a longer game, and a solved village can deserve an ending. This does not select seasons, roguelite resets or the entire Against the Storm structure for our game.

Our inference: automate routine execution; reserve consequential commitments for the player. Make costs and delayed work visible, allow anticipation and relief, and avoid an always-correct click that merely repairs deliberately weak automation. A compact set of actions can support depth if the desirable choice changes with circumstances.

## Independent review and disagreements

Five independent roles assessed the direction read-only, reusing existing play evidence where appropriate:

| Role | Main finding | Preferred test or caution |
| --- | --- | --- |
| Game designer | Additive economy offers few competing intentions. | Optional timber-for-food boat opportunity competing with homes and local harvest; reject an always-profitable trade. |
| UX/onboarding | Clear goals and automatic labor deserve retention. | One bounded worksite commitment; reject recurring priority maintenance and per-person assignment. |
| Playtest | Prior actual route needed no revision for much of its ending. | Shorten solved sustain first; timber trade only matters if timber has another real commitment. |
| Development lead | Current stack supports the needed comparison. | Risk-bearing arrival timing is a smaller alternative; extra workers might make immediate acceptance dominant. |
| Visual/audio | Legible consequences can reward watching, but cannot create missing alternatives. | Show the chosen work and what waits in one contextual card, with an actual visible outcome. |

Agreement is stronger on diagnosis than on the next mechanic. A finite-land spatial-planning direction is another coherent alternative: depth could live in settlement layout rather than frequent interventions. It is not selected now because the owner's new feedback most directly calls for a consequential response to changing food pressure.

Whole-project scope: campaign/economy need competing plans rather than a catalogue migration; the small browser action set and automatic ordinary work have positive owner evidence; post-success tending may support expression but cannot substitute for purposeful challenges. Existing native needs, buildings, Creative mode and presentation investments remain candidates, not requirements. The browser/shared-command/recording architecture has demonstrated utility and warrants retention. There was no new native campaign/Creative walkthrough, rendered play or listening in this assessment. The visual reviewer reused earlier stills and current documentation; the runtime reviewer reused the older fixed-build API session. No fresh audiovisual acceptance or agent claim of human enjoyment is made.

## Recommended bounded comparison

Hypothesis: one optional opportunity that competes with a valuable village commitment creates satisfying intervention while routine work remains automatic.

Use one boat offering food for timber before departure. Keeping timber supports needed homes; spending it buys food security while local crops are still growing. Declining must be viable. Reuse current resources, crops, housing and boat presentation; exact quantities and timing remain TBD until legal-action probes demonstrate the conflict. Test a near-ready crop and a slower crop so the preferred route can change with the situation. If spare timber/labor makes every plan equivalent, reject or revise the setup before presenting it.

First establish that this decision exists with current controls. Only add a contextual, task-bounded priority if the player cannot express the chosen plan. It should finish one named job and release the crew automatically. No per-person assignments, permanent scheduler panel, recurring collect button, cooldown reward or second resource system. More buttons are not the hypothesis.

Separately replace unnecessary sustain duration with honest evidence of the settlement outcome. Use the same ending in both comparison variants so the benefit of shorter waiting is not mistaken for the benefit of the boat. Preserve evidence that local production can support residents, rather than accepting a purchased stockpile as self-sufficiency. Exact replacement criterion is a design task, not settled here.

Success: the owner understands two plausible plans, deliberately gives something up, notices the consequence, and has a reason to reconsider when circumstances change. Falsification: the same answer dominates both starts; repeated toggling is required; normal workers become incompetent without intervention; or the player makes extra taps but still experiences waiting. Keep recoverability and pauses. This is a recommendation, not an implemented feature.

## Tooling and adjacent issues

No new framework or rewrite. A small recording-summary command is justified only alongside the comparison: reuse phase, order, resource-low and idle-gap extraction now repeated by hand. Beneficiaries are the lead and evaluator; budget approximately one focused hour, low maintenance. Validate two sessions against their underlying recordings. Never score enjoyment through actions per minute.

Track the observed placement message referring to unavailable browser action `Clear trees & stumps [C]` as a separate clarity correction. It must not replace the structural gameplay experiment.

Playable count remains259. Research request fulfilled without changing the live game.
