# Les Habitants — roadmap

**A home for generations.** A settlement game about 17th-century French settlers in New France, in the region now called Quebec: farming, making a living from the land, establishing homes and improving everyday life.

**Descendants are a theme, not a system.** No inheritance, genealogy, aging, succession, generational handoffs or legacy scores. The player builds a better village for the people living there; writing and the enduring place carry the longer-term aspiration.

**Inlanders remains the internal codename:** repository, files, project, namespaces, assemblies, scripts, identifiers and save paths stay unchanged. Les Habitants is now the public menu/HUD/window identity.

## Current direction and next step

[The thematic review](THEME_REVIEW_36.md) redirects the generic shore/woodland comparison toward a small agrarian settlement shaped by river frontage, useful growing ground and retained woodland. The [theme/reference brief](LES_HABITANTS.md) gives the historical frame and deliberately leaves exact location, decade and balance TBD.

**Checkpoints121–130 authorized:** reshape existing home and working ground, review at125/130, then stop. See [active queue](NEXT_CHUNKS.md). Previous120 verdict remains the starting evidence, not a human playtest.

**Checkpoint120 complete:** Ten outcomes111–120 and whole-game reviews [115](REVIEW_CHECKPOINT_115.md)/[120](REVIEW_CHECKPOINT_120.md) delivered the river-frontage comparison and a more coherent look/act/observe interaction. [NEXT_CHUNKS](NEXT_CHUNKS.md) records how to try it and the stopping point; no automatic121–125 queue.

The verdict remains partially convincing. A practical landing, inland fields and woodland provide a stronger place; fewer diet/UI obligations help authorship. Whether players want to reshape an already functioning hamlet remains unproven. Public qualitative mood may still imply a checklist. Keep founded/inhabited, inlet/frontage and Normal/relaxed comparisons; no new needs/resources/catalogue expansion to manufacture purpose.

The inhabited frontage now compares livelihoods, not just composition: two fields plus a dock and shared meal ground,40vegetables per combined crop plus fish,8loose logs instead of12 with equal total timber investment. Keep these differences explicit. Retain existing rendering provisionally; broad lawn, repeated roofs and stepped banks remain aesthetic hypotheses for human reaction. Dense stutter, intermittent save replacement and unauditioned sound remain limitations. Below is a hypothesis inventory, not an execution queue.

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



Checkpoint106: everyday palette now offers home, cultivated ground, river landing and existing timber gathering. Bridge/forager remain in the full catalogue. Shared-worker clearing guidance no longer incorrectly requires manual logger assignment. Native960 all choices fit; mark/cancel/reorder and actual shared timber collection/root clearing pass20261003-065259-799-essentials-4c3590. The fixture preserves all trees; the probe explicitly chooses one for clearing, rather than assuming an unpreserved tree exists. Zero-warning build; simulation unchanged. Next107 removes overlapping food inspection surfaces.


Checkpoint107: public food inspection temporarily replaces place/person cards with store labels and a compact Back/Esc control; menus close the overlay. Returning restores selected-place context. Removes the large stale notice and immediate toggle feedback delay; provision-rest uses a semantic report flag. Native960 food view/open/back and unchanged state passed20261003-065531-251-household-24346b; focused crop/revision checks pass, zero-warning build. Next108 groups occasional home alterations, then109 simpler public navigation.


Checkpoint108: home selection leads with residents/use/watching; Move, furnishing and yard changes join occasional actions under Change this home. Native960 household exchange passes065738-923-household-move-c8b9f6. Broader bank probe updated from obsolete Follow resident to actual roster: full yard previews/apply/cancel/furnish, Normal/relaxed, saves/menu and shared-place flow passes20261003-070014-940-cultivated-bank-fc309b. Prior run065904-231 reproduced known replace-target0x80070005,11 attempts414ms; village correctly stayed open. Successful retry is not a fix; cause remains unknown, diagnostics retained. Zero-warning build, simulation unchanged. Next109 reduces public navigation with all management still reachable.


Checkpoint109: public persistent navigation is Build/Village; Village has People, Supplies and Options links, while existing V/I/O shortcuts remain. Archived worlds restore the five domains. Shared native menu helper now follows visible public controls. Native960 all three secondary pages/Escape preserve simulation20261003-070234-747-household-7eb426; founding menu/placement/cancel/save-load/relaxed entry passed070246-356-first-place-6b6450. Zero-warning build; no simulation change.110 completes place watching with return context and compact controls, validates the batch and stops after full review.


Checkpoint110: watching a selected place retains it for return without changing pause/speed; public watch mode starts with Return, clock controls and expandable view options. Expanded controls fit compact view. Native960 navigation/household/watch/return/supply/optional finish/watch/reopen/exact save-load passed20261003-070608-239-quiet-visit-993d74; image inspected. Zero-warning build. Final broad regression and refreshed full20-minute founding session underway; freeze for whole-game110 plus visual/audio, then stop. No claim of human discovery, enjoyment or listening.


Checkpoint111: public founding menu now compares inlet and river frontage, with distinct current save/restart identity in Normal/relaxed. Same eight people,48logs/4planks/120food and existing rules. Native960 actual menu/save/load plus construction, meals and home rest passed20261003-130234-242-river-frontage-f7aa6d. Focused both-mode garden/fishing routes and exact continuation pass; zero-warning build. Landing tests choose legal bend geometry rather than assume the inlet coordinate. Next112 supplies an inhabited comparison; preference unobserved.


Checkpoint112: inhabited frontage is selectable beside the inlet, with six homes, two fields and a kitchen garden at equal population/reserves/crop capacity. Actual960 menu, household/watch/food controls pass130826-298-river-hamlet-a95b57; overview inspected. Both-mode actual meals/restart/exact continuation plus open garden/fishing routes pass, zero-warning build. Authored fields run inland; no objective recipe added. Next113 reuses existing resource survey with land-orientation access instead of adding another management overlay.


Checkpoint113: Village now opens the existing resource survey with optional shore/open-land/woodlot views, computed from actual ground and standing trees. No soil bonus or prescribed site implied. Finish/Esc restores the entry camera; clock and simulation stay unchanged. Native960 all three views and exact return pass131111-488-river-hamlet-36bb44, zero-warning build. Reuses one inspector rather than adding an overlay. Next114 addresses broad water depth/shore readability, with115 presentation review.


Checkpoint114: public river water now grades from shallow banks to deeper channel with fewer calmer ripple marks; distant water follows actual map-edge geometry rather than the old straight shore. Land/boat/placement rules unchanged. Zero-warning build, frontage1440 overview inspected131352-803-river-hamlet-cce182 and opposite inlet960 capture131425-838-across-inlet-bbbfe4. No motion/audio acceptance claimed. Next115 gives existing workable woodland distinct silhouettes and a coherent ground edge, then full presentation-inclusive review.


Checkpoint115: existing workable trees now have mixed tall conifer and branched/light-bark silhouettes; overlapping canopy ground joins into a forest floor and clears with the real trees. No new resources/obstacles. Frontage1440 before/after8s1x captures131558-762-river-hamlet-1ade4e inspected; this is snapshots, not motion acceptance or isolated performance (another capture overlapped). Actual timber mark/cancel/collection/root clearing passed131613-218-essentials-f2337f, zero-warning build.37 current suites running; freeze for whole-game115 with visual/audio before116. Remaining outcomes are contingent on synthesis, not automatically more decoration.


Checkpoint116: resource survey can hand its current camera directly to everyday building/clearing choices. Back/Esc still returns to the original view; Build in this view deliberately keeps the chosen ground. Native960 survey/action/field-preview/cancel preserves simulation and camera132408-091-river-hamlet-af946d; zero-warning build. Next117 aligns the first-place vocabulary with that same small palette, retaining full catalogue access.


Checkpoint117: first-place and everyday Build now share home/cultivated-strip/landing choices plus timber gathering. Opening cultivation uses the same directly drawn ground as later play; kitchen gardens and grain remain in the full catalogue. Native960 all choices/cancel/timber/catalogue/Normal-relaxed/save/firstplacement pass132514-141-first-place-3c3bc5; zero-warning build. No new building/rule. Next118 makes the river a practical livelihood and explicitly accounts for the changed investment mix.


Checkpoint118: inhabited frontage now uses two inland fields, an existing fishing landing and shared outdoor ground. Landing replaces kitchen garden and costs4additional invested logs, leaving8rather than12 in yard; total initial timber, people and72food stay matched. Crop capacity now40vegetables plus actual catches, not the old48vegetable claim. Both-mode fish/crop meals/exact continuation pass;960 native menu/household/survey/actions pass132702-230-river-hamlet-3866ea.1440 actual60simseconds at6x inspected132623-160-river-hamlet-98439a; no preference claim. Next119 removes public diet-score pressure rather than adding another purpose through needs.


Checkpoint119: public resident mood no longer rewards a three-food diet or displays a completion score. It reflects actual meals/home/rest/breaks; relaxed missing meals remain penalty-free but are reported honestly. Public People and meal summaries omit variety grading; archive happiness/variety remains unchanged. Both-mode one-food versus varied-food independence, actual continuation and archive happiness checks pass.960 native mood inspection (person API-selected) plus menu/survey paths passed132905-348-river-hamlet-e2e0da, zero-warning build. Next120 completes observation camera return, full review and stop.


Checkpoint120: following a resident now remembers the original place and camera (position/zoom/orientation); Back restores that view without rewinding time or changing the clock. Cycling residents preserves the observation origin. Native960 actual household follow/return now asserts exact camera and simulation, plus survey/action/mood paths passed133032-719-river-hamlet-16bdc6; zero-warning build. Ten authorized outcomes complete. Freeze for whole-game120, isolated ordinary/dense1x frame samples and final regression; no121.


120 review follow-up (zero new playable count): independent design/UX reviewers found mandatory-sounding recreation wording beside shared meal ground. Public RecreationSummary now neutrally distinguishes commons meals from a separate recreation venue; no leisure simulation or grade added. PublicMood and archived happiness checks pass. Base120review/performance fixed2c240c3 remains recorded; narrow text correction is separately committed and presented to reviewers.


Checkpoint121: existing cultivated strips can extend to eight rows with two logs per extra row and ordinary shared preparation in Normal; relaxed remains free. Stored food, identity and prior work setting stay; growing crops restart. Native960 existing plot shrink/cancel/extension and actual preparation passed 20261003-134951-417-plot-revision-da5692 (43.74s); proposal image inspected. Both-mode real delivery/crop/meals and exact continuation checks plus previous plot revisions pass. Zero-warning builds. Next122 tests direct home orientation; no new economy system.


Checkpoint122: homes can turn around their existing anchor from Change this home, with pure footprint/entrance preview, authoritative access/yard checks and explicit apply/cancel. Identity, household, goods and clock stay. Native960 turn cancellation and legal application passed20261003-135444-003-river-hamlet-4d7d66 (29.03s), including existing observation/survey paths; zero-warning build. Initial residents standing at doors correctly prevented turns until actual life moved them; scripted probe now observes that wait. Next123 brings appearance choices into the same home interaction.


Checkpoint123: home appearance is directly selectable in Change this home, with free reversible existing roof/plaster finishes and immediate world feedback. Editing hides observation controls, keeping the compact960 card above navigation (also fixes122 expanded-card overlap). Native dropdown selection, turn/cancel and exact save/load passed20261003-135653-456-river-hamlet-a1995f (31.92s); actual image inspected. Zero-warning build. Next124 connects selected places directly rather than expanding the general toolbar.
