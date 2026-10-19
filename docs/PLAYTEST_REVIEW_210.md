# Playtest feedback after checkpoint 210

**Later same-session update:** the completed playtest found free tending pointless without objectives and staffing confusing. [The challenge review](CHALLENGE_REVIEW_210.md) supersedes this document's primary direction and ordering; the woodland and same-map findings remain valid.

October 18, 2026. Immediate strategic review of fixed `b873c5e8ff52cdfdc77505e41f46ae363f0b5a5a`, prompted by the first human feedback after the batch. This is a design/documentation outcome, not checkpoint 211. No gameplay changes or new batch are included.

## Human evidence

The user reported:

1. Choosing individual trees to harvest is tedious; harvesting any tree or selecting a rectangle would be preferable.
2. “Harvest and renew grove was great!” but hidden; it appeared to address the first problem.
3. Both levels seemed identical, followed by: “am I crazy? I tried start both levels and they look the same?”

The user is correctly observing the same landscape. `NewRiverLivelihood` creates the inhabited river landscape, removes its two fields and fishing dock, and refunds their timber. Homes, people, trees, river and approaches are shared. These are starting conditions for one place, not two distinct levels. Our previous comparison overstated their experiential distinction.

The positive grove feedback is evidence for one enjoyed action, not proof of the entire game's appeal. It does not establish whether the user used the separate rectangle tool: the exact named action is the brush hidden under “Individual tree tools” in `WoodlandUi.cs`.

## Verdict and direction

**Partially convincing.** Choosing useful ground, delegating recurring work and seeing the land support village life has a specific positive human signal. The current interface undermines it: everyday timber, first arrival and shortage recovery teach individual tree permission while renewable woodland is secondary. The public menu similarly exposes a development comparison as two peer beginnings.

The strongest objection remains structural: the game offers many operations and systems, but their implementation differences can outweigh their perceived purpose. Fix the hierarchy and remove redundant public choices before adding content. Do not turn this finding into another tooltip or new need.

Choose **one river settlement, with an optional prepared food supply**, and **area-based renewable woodland as the ordinary timber action**. Retain Normal and Relaxed as separate rule choices. A future second place must create visibly and mechanically different spatial decisions, not merely remove a few starting buildings.

## Selected next scope

These are proposed playable outcomes, not an automatically authorized batch:

1. **Make woodland management the normal timber action.** From first arrival, building and material shortage, open one clearly named Harvest & renew area interaction. Select existing trees for harvesting and subsequent renewal; make planting additional bare ground deliberate. Keep standing and clearing remain explicit alternatives, with individual exceptions secondary. Reuse existing simulation behavior rather than add another forestry system. Avoid a rectangle silently adding so many new planting spots that an ordinary selection hits the 32-spot limit. Preserve the originating project/view through cancel and completion; a direct call to `BeginWoodlandCare` currently clears supply-return context through `CloseManagementUi`.
2. **Present one settlement honestly.** Replace the two peer river entries with one place and an optional “Food production already established” starting condition. Explain that this adds the fields and landing on the same map. Keep mode choice separate. Internal save identifiers may remain; no migration or preservation project is needed.
3. **Then decide the next distinct place.** Reuse an archived geographic situation if helpful, such as useful ground across water, without restoring its service-percentage assessments. Require different initial siting and later tradeoffs that survive dependable food. Do not create another map just to retain the appearance of two levels. Exact scenario and objectives remain TBD.

For the first slice, validate actual timber delivery and renewal, pure cancel, preserved trees outside the area, meaningful selection limits and return to the wanted addition. The next human check can be one short continuation: get timber for a change the player already wants. Do not ask them to replay both nearly identical starts to prove what source inspection already establishes.

## Alternatives and falsifiers

Automatic harvesting with preservation exceptions would remove designation entirely. Prefer area intention first because the user enjoyed grove renewal and chosen woods can matter to landscape and hunting. Reconsider this if area selection remains ceremony and the player consistently wants all woodland managed.

A broader direct-ground interaction for woodland, cultivation and clearing is a substantial alternative to the current collection of panels and tools. Test woodland first; generalize only if it improves intention, feedback and fine control. Reject area-first if it creates accidental planting/clearing, repeated limit failures or continued cell-by-cell correction.

Retaining separate establishment/tending entries would require evidence of distinguishable wanted experiences before and after the opening. Existing source differences and two menu labels do not meet that standard. Distinct scenery alone is also insufficient for a future level if the same strategy and later adjustments solve both.

## Whole-game disposition and limits

Keep shared labor, physical materials and meals, route-sensitive work, recoverable shortages, reversible arrangements and optional stopping. Geography gives cultivation, fishing and gathering different possibilities. Broader buildings, processing chains, furnishing, growth and communal venues remain provisional optional choices; this feedback does not establish their desirability. Do not add needs, mandatory diets or population goals to manufacture purpose.

Keep the assessment-driven campaign archived. Its prescribed chains and recent-service proofs are not a solution to sameness. Reuse its geography and recovery situations selectively. Normal retains material investment and hunger consequences; Relaxed retains work/meals with free building and no hunger penalties. Legacy Creative's foodless contract is different; CreativeCourt retains meals. Do not transfer evidence between them without qualification.

The same-looking starts are principally a content/setup problem, not new evidence that the entire art direction fails. Retain the public river ensemble provisionally while keeping review210's deeper working-frontage alternative available. No new human judgment about audio, motion or watching pleasure was supplied. Existing source, stills and scripted checks cannot certify those qualities.

Technical evidence remains as bounded in [review210](REVIEW_CHECKPOINT_210.md): full 67-suite regression at 205, targeted later simulation/native checks, and a local rendering improvement with mixed whole-scene timing. No new build, performance run or gameplay validation was performed for this documentation review. Save compatibility remains out of scope.

## Independent review synthesis

Five fresh read-only roles reviewed the same fixed commit: `feedback210_design`, `feedback210_ux`, `feedback210_playtest`, `feedback210_lead` and `feedback210_visual`. All returned partially convincing verdicts. No gameplay implementation proceeded during review.

- **Game design:** promote area stewardship and collapse the duplicated public choice. Keep a prepared-food starting option; do not manufacture another map to justify the old menu.
- **UX/onboarding:** every ordinary timber entry teaches the tedious action. Consolidate the overlapping brush/rectangle hierarchy rather than make both more prominent. A consistent direct-ground interaction is the larger alternative.
- **Playtest:** the named grove brush and rectangle are distinct; do not attribute rectangle acceptance to the user. No second test is required to establish that the maps are shared. Observe one wanted addition after the interaction change instead of assigning a mode/start matrix.
- **Development lead:** explicitly reset Renew, separate bare-ground planting, disclose capacity, and preserve supply context and pause/return behavior. Preferred establishment as the sole public default with provisioned tending kept as a development fixture.
- **Visual/audio:** inspected public `capture-0004` in the current livelihood bundle, dense archived `capture-0002`, and `artifacts/210-bakery-everyday.png`. Retain the public ensemble provisionally; the duplicate starts are not evidence against its art. Make area extents and continuing work readable without another large panel. No motion viewing or listening.

The substantive disagreement is whether prepared tending remains a public option. Select an optional setup within one place: it preserves access to an existing lower-pressure beginning without implying another level. This does not establish a preference for establishment or tending. All roles reject new obligations as a response to the reported friction. The larger shared land-area interaction and genuinely different authored geography remain alternatives to test, not automatic expansions.

Reviewers used source and prior review evidence; only the visual role inspected fresh selections of existing stills. None performed new UI play, build/test runs or listening. The user's reports are the new human evidence. Prior scripted checks remain reported evidence, not reviewer play observations.

## Tooling decision

Use the existing launcher and evidence bundles. Alongside woodland implementation, add or adapt one reproducible timber-shortage setup and a short session note recording intention, entry used, assistance and perceived result. Beneficiaries are the developer and subsequent reviewers; estimated effort is hours, maintenance low. Measure whether another reviewer can reopen the same condition and distinguish discovery from scripted success without reconstructing the session. This is not a prerequisite for acting on the current feedback.

Defer new frameworks, telemetry systems, broad scenario registries and architecture migrations. Motion/audio capture remains a separate conditional investment when a presentation decision needs listening; more screenshots will not answer that question.
