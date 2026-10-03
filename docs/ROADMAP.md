# Les Habitants — roadmap

**A home for generations.** A settlement game about 17th-century French settlers in New France, in the region now called Quebec: farming, making a living from the land, establishing homes and improving everyday life.

**Descendants are a theme, not a system.** No inheritance, genealogy, aging, succession, generational handoffs or legacy scores. The player builds a better village for the people living there; writing and the enduring place carry the longer-term aspiration.

**Inlanders remains the internal codename:** repository, files, project, namespaces, assemblies, scripts, identifiers and save paths stay unchanged. Les Habitants is now the public menu/HUD/window identity.

## Current direction and next step

[The thematic review](THEME_REVIEW_36.md) redirects the generic shore/woodland comparison toward a small agrarian settlement shaped by river frontage, useful growing ground and retained woodland. The [theme/reference brief](LES_HABITANTS.md) gives the historical frame and deliberately leaves exact location, decade and balance TBD.

**Next: validate a smaller experience before adding mechanics.** Count **105**. The user's complexity concern triggers [an immediate strategic correction](STRATEGIC_REVIEW_100_CORE.md), superseding the101–105 batch from [whole-game100](REVIEW_CHECKPOINT_100.md). The farmstead is a credible hypothesis, not an accepted core. We have stronger proof that its systems function than that a player wants to use them.

Use the frozen build for one bounded uncoached session, then compare a reduced first-use surface if observed complexity warrants it. Keep homes, cultivation, physical shared work/meals/rest, useful visual quality and forgiving arrangement. Preserve existing code and full catalogue access; lower unearned prominence rather than starting an architectural purge. [NEXT_CHUNKS](NEXT_CHUNKS.md) records the test and decision gates. No new scenario/UI/art/scheduler bundle is scheduled ahead of evidence.

Retain Normal/relaxed as comparators, optional growth/finishing and the archived campaign boundary. A satisfying short visit is a valid outcome; compulsory expansion and busywork are not cures for uncertainty. Below is a parked hypothesis inventory, not an execution queue. Save reliability still matters, but more tooling and source reviews cannot substitute for observing someone play.

## What stays, what changes

| Area | Direction |
| --- | --- |
| Core loop | Read the land, establish homes and a visibly working livelihood, connect work and daily life, recover from mistakes, improve or finish. Test the first productive payoff separately from mature reserve control. Keep shared work, physical materials/meals, optional arrivals and forgiving housing/public-space moves. |
| Land and maps — F01/F02/F12 | River frontage, long cultivated areas, kitchen gardens, woodlots and useful paths give the settlement its structure. Reuse current clearing, preservation, water and route rules. Exact plots remain freely arranged; no land-ownership bureaucracy. |
| Food — F05/F07/F24/F26 | Put cultivation and food processing in the foreground; use fish, gathered foods and woodland as complementary choices. Compare adaptive food plans at equal investment; current batch bread is retained provisionally. Storage should solve actual access/batch problems, not serve a required building checklist. |
| Homes and community — F04/F25 | Comfortable occupied homes and modest shared places express improvement. Keep food, home rest and recreation; no automatic education, religion, warmth or additional satisfaction meters. |
| Buildings — F08/F23/F26 | Reassess names, scale, construction and environment as one small period-informed palette. Keep public buildings available while comparing their usefulness; archived Carpenter dependency remains excluded. A historical setting does not require every historical institution. |
| Campaign/onboarding — F11/F18/F19/F21 | Give each situation a practical land/labor problem, two plausible approaches and recoverable mistakes. Teach ordinary rules in the place being built. Remove population expansion and first-visitor recipes as the default definition of achievement. |
| Visual life — F03/F09/F20/F22/F23 | Make fields, homes, work yards and water approaches readable at ordinary zoom. Retain warm style; reconsider massing and terrain together. Clothes/tools/materials need dated references, not generic period decoration. |
| Sound/music — F10/F17 | Audition work, construction, footsteps, water and quiet intervals first. Extend the existing system toward the selected setting after listening; no automatic new soundtrack framework. |
| Free arrangement — F16 | Keep a relaxed option with the same recognizable place and daily life. Be clear about free construction and relaxed hunger; do not confuse it with older foodless Creative rules. |
| Reliability/tooling | Godot/C# and local Windows remain. Current saves must be correct; migrations are unnecessary. Reuse snapshots, held-input checks and frame summaries; investigate performance when reproduced. |

## New France art direction — F40

**Make the setting unmistakable in the village itself.** The user explicitly prioritizes a stronger New France aesthetic. [F40 — New France art direction](NEW_FRANCE_ART_DIRECTION.md) turns the provisional F34 palette into concrete work:

| Priority | Item |
| --- | --- |
| Partial66–71; whole-scene identity unaccepted | **F40a — substantial habitant houses**, with researched forms, deep openings, foundations, chimneys and material contrast. |
| Fields/ground advanced through 81; connected ensemble next | **F40b — cultivated landscape** and **F40c — lived-in domestic yards**: real fields, useful forecourts, woodlots, earth paths and river frontage. |
| Landing 82 and oven 83 delivered; ensemble still provisional | **F40d — distinctive oven/work/storage structures** and **F40e — working river landing and boats**. |
| Extend the chosen scene | **F40f — clothing, tools and everyday poses**; **F40g — light, atmosphere, sound and music**. |
| Across relevant UI work | **F40h — French names, local writing, menu art and restrained typography/material styling**. |

Review100 advances the connected farmstead into an asymmetric river-facing landscape comparison using the existing low houses, fields, landing and oven. Domestic/work ground, bank transitions and woodland margins must respond to real placement and use; do not substitute preset decorative templates. The older colorful menu diorama should give way to the selected public village language. Judge the whole view at ordinary zoom before spreading the style across the catalogue. Specific historical forms need date/place references; the working anchor remains rural St. Lawrence,1670–1680. Details and later ordering stay TBD. This art track does not add seasons, inheritance, needs or compulsory buildings.

## Feature priorities after the first situation

These are candidates, not a shopping list. Reevaluate after seeing the themed slice. All existing feature IDs retain their history; F34 retains the initial theme-integration history; F40 is the active art-direction track. Review 90 prioritizes connected farmstead composition over additional standalone assets; the following expansion candidates stay behind that comparison.

| Priority | Candidate | Decision it should add / smallest scope |
| --- | --- | --- |
| Alongside next comparison | **F34b — worked land and farmstead presentation** | Fields/gardens, homes, stored produce and shore access form a coherent visible whole. Start with representative assets and actual activity, not 18 simultaneous building replacements. |
| Delivered, evaluate | **F34c — cultivated ground** | Real 3 × 5 grain fields with walking/work, compact gardens retained. Whether grain deserves its larger footprint remains an open design question. |
| Conditional | **F34d — milling and bread** | A mill or combined milling/baking treatment may give grain a recognizable, geographically meaningful chain. Test siting/labor payoff; do not add flour plus another mandatory wait solely for historical completeness. The farm/bakery chain already exists; compare its value under current rules. |
| Later | **F26/F34e — pasture or river exchange** | Livestock could compete for cleared land/feed; a landing could carry actual goods. Prototype one only when it differs from gardens or land hauling. Animal breeding, full trade markets and fleets are not implied. |
| First step delivered | **F25/F34f — improve daily life** | Quiet residents now return home and yield to new work.  Home repairs/comfort, a common oven, work gathering or contextual chapel identity only where there is a useful activity and supported reference. No compulsory civic ladder or faith/education meter. |
| Across relevant chunks | **F21/F34 — understandable choices** | Preserve compact category browsing, direct inspection and visible phase actions. Show the need, terrain/input and practical result. Avoid another permanent dashboard. |

Costs, outputs, field dimensions, chapter length, specific crops, new resource types and precise architectural variants remain TBD. The theme changes priorities, not every balance constant at once.

## Campaign concepts

[Five situation sketches](CAMPAIGN_SYSTEMS.md) explore getting established, cultivating food, woodland/shore choices, useful connections and improving an existing village. They are a source of distinct scenarios, not five promised levels or a family chronology. Introduce one unfamiliar building group at a time, keep the full catalogue available, and let the player solve the situation in more than one way.

## Explicit exclusions and deferred work

- No inheritance, generational handoff, family simulation or legacy scoring; “for generations” is narrative tone only.
- **F13 seasons stays removed.** Do not reintroduce an annual cycle, winter deadline or cold-survival system through the theme.
- No combat, conquest, multiplayer, large technology tree, compulsory religious/education systems, detailed seigneurial taxation or full historical economy.
- No internal rename or save migration. F34a ships public branding only; further historical art remains provisional.
- Avoid expanding the catalogue, polishing every model, or adding needs before the livelihood-and-improvement experience proves useful.

## Delivered foundation and history

Current public game: provisioned player-founded farmstead and inhabited inlet comparison, each with Normal/relaxed constraints and voluntary finishing. Earlier farmstead, court, lake/gathering and campaign experiments remain behind developer access. Nineteen buildings, meals/material routes, homes/rest/recreation, optional comfort, fishing/stone/wildlife, woodland/landscaping, four-way buildings, paths, save/resume, audio and menus provide the working base.

Recent outcomes: [F33a action/food-choice clarity](CHOICES_F33A.md), [T07 interaction sampling](INTERACTION_T07.md), [F32 landscape comparison](HAMLET_F32A.md), [simulation performance](HAMLET_PERFORMANCE_F32B.md), [rearrangement](HAMLET_REARRANGEMENT_F32C.md), [connected paths](HAMLET_PATHS_F32D.md). Their tests do not establish enjoyment or historical authenticity.

The [pre-theme roadmap](ROADMAP_PRE_HABITANTS.md) preserves delivered feature tables, IDs and older concepts; [earlier F31/F32 queue](DELIVERED_F31_F32_QUEUE.md) and [pre-theme F33 queue](F33_QUEUE_BEFORE_THEME.md) preserve prior decisions. Historical proposals do not override the active scope or explicit exclusions above.

After every chunk, reevaluate this roadmap and the active queue; record outcomes in [CHECKPOINTS.md](CHECKPOINTS.md). Follow the [critical review mandate](DESIGN_REVIEW_MANDATE.md) and [whole-game reviewer cadence](REVIEW_CADENCE_PROPOSAL.md). Count playable outcomes once, and keep unobserved play/listening/preferences explicit.

Checkpoints41–45 delivered real cultivated ground, home waiting, resident journeys, workplace relocation and an inhabited inlet. Checkpoints46–50 delivered [provisioning](PROVISIONED_LIFE_F36A.md), [normal shared meals](NORMAL_COMMONS_F36B.md), [furnished forecourts](HOME_YARDS_F36C.md), [grain capacity](FOOD_LIVELIHOOD_F36D.md) and [world workplace cards](WORKPLACE_READING_F36E.md). Reviews [40](REVIEW_CHECKPOINT_40.md), [45](REVIEW_CHECKPOINT_45.md) and [50](REVIEW_CHECKPOINT_50.md) record retention, rejected assumptions and evidence limits. Corrections/tooling do not advance the count.

Checkpoints56–60 delivered contested cultivation/shared ground, paused garden relocation with replanting, actual approach paths and fuller crops, concise entry/action hierarchy, and paired current food-access inspection. [Review60](REVIEW_CHECKPOINT_60.md) records the five-role critique and final evidence. The hands-on reviewer was blocked before any input by a window-state tool hang; scripted controls, simulation and stills remain distinct from play or enjoyment evidence. Save replacement denial remains open. F38 was the following implementation queue; F39/F40 now define the active priorities.


Checkpoints61–65 delivered coherent public entry, voluntary finish/watch/reopen, the cultivated-bank comparison, compact home/construction actions and saved opening-footprint comparison. [Review65](REVIEW_CHECKPOINT_65.md) records independent findings, concrete public-flow corrections and replacement priorities. Checkpoint66 subsequently delivered direct furnishing and the first limited house/yard treatment; see [review66](REVIEW_PRESENTATION_66.md).

Checkpoints67–71 deliver larger working fields, the broader bank composition, four real yard locations, candidate previews and an explicit furnishing transaction. [Review70](REVIEW_CHECKPOINT_70.md) challenges motivation and scene structure; [the active queue](NEXT_CHUNKS.md) pauses for human play and makes the next five scopes conditional. Ground overlap fixes count zero. Current saves remain required; intermittent replacement denial remains unresolved. See the ledger for validation and chronology.


Checkpoints72–75 add place watching, direct ground-side choice, shared-meal world controls and the matched-inventory grouped farmstead. [Review75](REVIEW_CHECKPOINT_75.md) records the whole-game critique, reviewer-independence limits, evidence and reliability closeout. The next queue prioritizes a wanted transformation rather than another convenience batch.


Checkpoint101 — October3: home cards lead with actual residents and domestic use. Optional invitations, household exchange and full details sit behind More home actions; selection resets the expansion. Capabilities retained, no simulation changes. Zero-warning build and native960 household roster/follow/return/expanded exchange/cancel/confirm pass20261003-063041-296-household-move-8b5108. Reassess: retain this reduced surface provisionally; next102 offers a small reusable build palette with full catalogue access. User resumed through110 with105/110 reviews; prior wait-for-playtest execution stop is superseded, not a claim of enjoyment.


Checkpoint102: public Build opens a four-choice everyday palette (home, drawn cultivation, foraging, bridge). Full catalogue remains one click away and B retains keyboard catalogue navigation. No unlocks or simulation changes. Zero-warning build; native960 palette/field preview/cancel/full catalogue and household flow passed20261003-063324-291-essentials-b34aa6. Next103 unifies returning from a followed resident to the actual place of inspection, instead of opening detailed inspectors unexpectedly.


Checkpoint103: following a resident from a place retains an explicit return to that place; direct resident inspection offers their household, and food-source inspection opens the same public place card. Removes a surprising jump to full management. Zero-warning build; native960 household follow/return plus explicit workplace-origin return passed20261003-063651-145-household-381ff3, query state unchanged. Workplace origin setup in the extra assertion is API-prepared, return is actual input; not an uncoached play claim. Next104 removes manual pause/resume choreography from cultivation revision while preserving work state and cancel purity.


Checkpoint104: cultivated-ground revision previews without stopping work; applying atomically pauses/revises/restores prior work state. No-op/cancel/rejection remain pure, explicit pauses survive. Existing ripe-crop and prepared-capacity rules retained. Both-mode simulation checks and native960 actual crop/meal/preview/Escape/apply/automatic working restoration passed20261003-064020-302-plot-revision-36dd70. Initial probe read the throttled card before refresh; forcing its refresh before assertion corrected the probe, not game timing. Zero-warning build. Next105 ties selected food-place status to actual supplies, then freeze for whole review.


Checkpoint105: selected food places link directly to actual world food stores and back; reserve-resting state gives current supply/population without implying a guaranteed future. Crop growth now precedes provision-rest status so living crops are not hidden behind an idle explanation. No scheduling changes. Zero-warning build, both-mode crop-state/revision checks and native960 selected-place food view open/close/pure-state passed20261003-064251-393-household-3fc3a0. Freeze for independent whole-game105 before choosing106–110.
