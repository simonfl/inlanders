# Completed playtest: challenges and automatic work

October 18, 2026. Immediate strategic update at `c3f6f71c890e5af396509b8741e1408f25116f97`; gameplay remains the checkpoint-210 build. This supersedes the primary-direction decision in [the earlier same-session feedback review](PLAYTEST_REVIEW_210.md). Five independent read-only disciplinary updates completed before selecting this scope. No implementation or playable checkpoint is included.

## Evidence and verdict

The user finished the playtest. Building grouping was “neat.” Jobs versus shared workers were confusing; they expected residents to pick up needed work and saw most shared workers idle. Without a goal the game felt “a bit pointless”; they want structured objectives and actual challenges. Their earlier positive feedback about Harvest & renew grove and negative feedback about individual-tree clicking and identical starts still stand.

They clarified the labor example: nobody was fishing, so they assigned fishers, after which fishing worked. They wondered whether the original inactivity was expected and asked for clarity about shared work and any staffing needed to activate buildings.

**Decision: the current open-ended experience is insufficient as the primary game for this player.** Keep building grouping and renewable woodland as useful means; give those tools a clear purpose through authored settlement challenges. Free play becomes a secondary choice and an option after success. This is a change to the game's main structure, not another optional hint or a claim that adding any checklist will make it fun.

Our earlier reviews repeatedly left motivation unresolved while retaining free tending. This human session supplies contrary evidence. It does not establish approval of the old campaign, the full catalogue, audio, visual quality or future challenge designs.

## Labor findings and chosen contract

Shared residents already switch professions through `ClaimSharedWork`. A completed fishing dock does not require a manually assigned fisher in public shared-work play. However, several rules can stop dispatch:

- In provisioned public play, shared food work normally rests at four stored edible portions per resident when nobody is unfed. New workplaces have establishment exceptions. The working river start has 72 portions for 12 people and already-established producers, so reserve-driven idleness is a plausible initial condition.
- Assigning a profession makes a resident non-shared. They seek work only within that profession and bypass the shared food-reserve gate, while still obeying workplace limits, pauses, routes and output targets. This can make assignment appear necessary to activate fishing.
- Workplace “Automatic” clears a specific workplace assignment but does not restore profession-switching shared work. This is a misleading second meaning of automatic.
- Profession caps, reserved dedicated slots, inputs, route access, growing resources, breaks and existing reservations can also limit work. The shared-worker fallback overwrites individual failed-work explanations with generic waiting text.

These are source findings, not a diagnosis of the user's exact saved state. The fishing report is consistent with the reserve gate; other blockers have not been excluded. Never claim all idle residents prove either a broken scheduler or healthy downtime.

The lead's follow-up found a concrete reporting mismatch: `ReadWorkplace` can return “Provisions ready” based on zero explicit workplace assignments even when a fixed-profession fisher with automatic workplace selection is eligible. Once a worker claims the dock, earlier active-worker branches show the activity. Correct this alongside labor explanations; do not describe manual assignment as dock activation or claim the existing status reliably diagnoses all staffing configurations.

**Public contract: residents automatically take needed, runnable work; the player chooses places, orders and useful priorities.** Remove mandatory-looking profession and dedicated-staff assignment from the ordinary public workflow. Retain old controls in archived experiments if needed; do not preserve them publicly without a demonstrated decision they improve. A worker's current activity is information, not a profession the player must assign to enable a building.

Expose the actual state at the workplace and a concise workforce summary: working, eating/resting, available because reserves are sufficient, or useful work blocked by a named cause. Fishing should explain reserve-driven rest and automatic resumption when applicable. Counts must not imply that everyone in `Work.Waiting` is an unemployed shared worker. Do not invent jobs, eliminate all quiet time, or raise production indefinitely just to keep people moving.

Needed scenario supplies must be recognized by automatic dispatch. A challenge asking for more food cannot silently remain blocked by the ordinary reserve ceiling. Avoid implementing a second eligibility calculation just for UI explanations.

## First playable direction: a place for newcomers

Prototype one complete challenge around accommodating a small arriving group, provisionally four residents. Begin with an inhabited settlement whose available ground and journeys create a real expansion choice. Let the player choose home grouping, where to produce food, how to use woodland, and whether to improve connections or rearrange existing places. Normal supplies meaningful material and labor costs; Relaxed can retain the objective with free construction and gentler consequences. Legacy Creative's foodless rules are not equivalent.

Give a brief visible from the start, a few understandable outcome milestones, a deliberate invitation, visible arrivals, occupied homes and ordinary meals, then an explicit success with Continue building available. Do not require a named bakery, an approved layout, a designated welcome venue, every food type, expiring recreation percentages, or a timer by default. Growth is the premise of this selected challenge, not a universal success requirement in free play.

Housing and continuing food provision must actually serve the added residents; starting food alone must not satisfy the whole challenge. Finalize and show the exact completion rule before implementation, using existing production/use evidence and banked milestones where appropriate. Do not hide a rolling assessment window or make eating undo displayed progress. A shortage remains recoverable.

Use the existing inlet/crossing geography as a candidate, not a requirement to reuse the old neighborhood's east-bank rules. The map must make at least two arrangements meaningfully different. If placing obvious cottages beside the initial pantry solves everything and the geography is irrelevant, redesign the situation rather than adding arbitrary placement bans.

The goal is a substantive play session with several choices and a noticed result, not another two-minute tutorial. Duration and numerical balance remain unvalidated; do not stretch the experience with a waiting timer. First build and compare two strategies and one recoverable poor arrangement before asking the user to test again.

## Scope and alternatives

Replace the previous polish-first queue with one challenge slice, supported by a coherent automatic-labor contract and the already-selected area timber interaction. Consolidating the two identical river starts now belongs to the secondary free-play entry, rather than preceding the question of purpose.

The strongest alternative is a self-supporting arrival: house an existing small group and establish food that residents actually obtain and eat. It is cheaper and naturally teaches the economy, but may reduce to one field and waiting. Use it as the opening portion or a bounded comparison if the newcomer challenge obscures the core loop; do not build two full campaigns in parallel.

A spatial repair challenge—reconnect homes, useful ground and a domestic yard—tests arrangement pleasure without population growth. A selectable project board is a later alternative if ordered challenges feel restrictive. Neither should become a way of asking the player to invent their own purpose again.

Reject the chosen slice if the goal is understood but play consists mostly of prescribed placements and waiting, if labor remains opaque, if only one layout works, or if completion is just a counter with no visible change. Positive evidence is purposeful choice, understandable consequences, successful recovery, and interest in another challenge. Completion alone is insufficient.

## Whole-game review and independent perspectives

All five roles changed their recommendation from provisional public free tending to challenge-led play. Existing simulation and visual components remain promising; that does not justify preserving the former primary contract.

- **Game designer (`feedback210_design`):** preferred a self-supporting arrival with incomplete housing and free choice of food. Keep actual work and grouping; remove ordinary profession assignment, reserve dedication only if it earns its complexity.
- **UX (`feedback210_ux`):** preferred a food challenge followed by a small household arrival. Remove ordinary public profession/dedication controls; explain blocked work at its source. Keep building groups.
- **Playtest (`feedback210_playtest`):** preferred a spatial newcomer challenge using inlet geography, explicitly testing whether grouping and connections matter. Do not require another human session before making a concrete candidate.
- **Development lead (`feedback210_lead`):** preferred a small newcomer group, reusing arrival/housing primitives without importing the old mandatory crossing, welcome venue or whole-population meal tally. Prioritize a bounded scheduler trace.
- **Visual/audio (`feedback210_visual`):** preferred a visible home-area repair and actual meal/yard use. Make progress legible through the inhabited scene, not decorative busyness or a large new panel.

The disagreement is about the first challenge, not whether purpose is missing. Choose the newcomer scenario because it gives building grouping, added production and woodland a shared practical purpose and a visible endpoint. Keep self-supporting arrival as the cheaper comparison and spatial repair as an alternative if growth is not motivating. Dedication remains an archived capability; it will not be required in the new public slice.

Retain grouped placement/relocation, renewable woodland, reversible plans, shared labor, physical materials and meals, meaningful routes and recoverable shortages. Do not add new needs or production chains until this slice reveals a real gap. Existing optional buildings remain hypotheses, not a list every scenario must teach. Reuse archived campaign geography and infrastructure selectively; do not restore its assessment contract wholesale. No inheritance, seasons or save migrations.

Retain the public low-poly river ensemble provisionally. Challenge progress should be visible in construction, arrivals, food journeys and use of homes. Art replacement, extra props and audio additions do not resolve the present motivation finding. Whole-scene performance and listening remain unresolved as in review210; no new reliability claim is made.

Reviewers independently reassessed the entire project using source, this human feedback and prior evidence. They are reused disciplinary contexts from the immediately preceding review, not five fresh agents. The visual role reused its previously inspected public/bakery/dense stills; nobody performed new UI play, motion viewing, listening, build or performance tests. The five initial updates preceded the fishing clarification; the lead subsequently checked that specific distinction. The root inspected fishing dispatch and UI conditions directly.

## Tooling and next evidence

Extend existing diagnostics with a bounded shared-work decision trace: actual shared/fixed status, work considered, reserve suppression, occupied/reserved capacity, input/access blockage and selected work. Capture a short interval around a known food or construction order. Reuse actual dispatch checks; inspection must not mutate state or diverge into a second scheduler.

Beneficiaries are implementation, playtest diagnosis and subsequent reviews. Estimate half to one focused day with modest maintenance; validate against sufficient-reserve idleness, a fixed-profession resident, occupied slots and a deliberately blocked input, with unchanged simulation outcomes. Measure time to explain the first actual obstruction. This supports the challenge; it is not a playable checkpoint itself.

Use existing scenario/provenance tooling for alternative arrangements and before/after evidence. Defer generalized goal engines, new telemetry frameworks, scheduler rewrites and architecture migrations. Motion/audio capture remains conditional on a presentation decision requiring it.

Checkpoint remains **210**. This review selects the next implementation scope; it does not claim implementation, authorize another ten-outcome batch, or establish challenge enjoyment.
