# Les Habitants — roadmap

**A home for generations.** A settlement game about 17th-century French settlers in New France, in the region now called Quebec: farming, making a living from the land, establishing homes and improving everyday life.

**Descendants are a theme, not a system.** No inheritance, genealogy, aging, succession, generational handoffs or legacy scores. The player builds a better village for the people living there; writing and the enduring place carry the longer-term aspiration.

**Inlanders remains the internal codename:** repository, files, project, namespaces, assemblies, scripts, identifiers and save paths stay unchanged. Les Habitants is now the public menu/HUD/window identity.

## Current direction and next step

**Completed161–170:** Matched founding, bounded arrangement recovery, the river-farmstead landscape, row-sized field collection and measured actor/HUD fixes are committed. Public Play now offers **Establish life by the river** or **Tend a river settlement**, with Normal/Relaxed and earlier beginnings retained. Whole-game reviews [165](REVIEW_CHECKPOINT_165.md) and [170](REVIEW_CHECKPOINT_170.md) choose provisional retention and **stop170**. Final52-suite regression passed on the reviewed build; delivery closes the batch. No171 authorized. [Batch outcomes and future hypotheses](NEXT_CHUNKS.md).

[The thematic review](THEME_REVIEW_36.md) redirects the generic shore/woodland comparison toward a small agrarian settlement shaped by river frontage, useful growing ground and retained woodland. The [theme/reference brief](LES_HABITANTS.md) gives the historical frame and deliberately leaves exact location, decade and balance TBD.

**Completed151–160:** user authorized another ten on October7. 151 delivered batched path strokes with measured active-route savings. 152 adds actual-trip path proposals and a focused journey card. 153 exposes actual household journeys and return context. 154 adds actual shared-place visitors with follow/return. 155 adds chosen path approaches. 156 consolidates world path proposals and removes transient scraps/duplicate controls. 157 delivers atomic translated/turned home groups with actual model preview. 158 includes productive ground with growing/ripe crop consequences and continued real meals. 159 makes carried approaches explicit and preserves external junctions. 160 delivers matched arrangement comparison and direct observation of actual use. Ten outcomes complete; [full review160](REVIEW_CHECKPOINT_160.md) retains the group experiment provisionally, freezes further control expansion and stops160. Final47-suite regression passed on corrected02f1193; delivery closes the batch. First five complete; [review155](REVIEW_CHECKPOINT_155.md) cuts further trip-panel expansion and chooses a bounded farmstead-group composition experiment156–160; full reviews155/160. No new human play evidence. [Active queue](NEXT_CHUNKS.md).

**Completed141–150:** full reviews [145](REVIEW_CHECKPOINT_145.md) and [150](REVIEW_CHECKPOINT_150.md). The combined field/shared-ground test rejected the forced binary dilemma. Shared places now support small independent arrangements and ordinary bounded visits; varied homes, blended ground, nearer cultivation and contextual paths form the retained ensemble. **Stop150** after final regression/push. [Handoff and future hypotheses](NEXT_CHUNKS.md); no automatically scheduled151 or new needs/resources/growth.

**Earlier checkpoint120:** Ten outcomes111–120 and whole-game reviews [115](REVIEW_CHECKPOINT_115.md)/[120](REVIEW_CHECKPOINT_120.md) delivered the river-frontage comparison and a more coherent look/act/observe interaction. The subsequently authorized121–130 batch is now complete.

The verdict remains partially convincing. A practical landing, inland fields and woodland provide a stronger place; fewer diet/UI obligations help authorship. Whether players want to reshape an already functioning hamlet remains unproven. Public qualitative mood has been replaced by descriptive daily life. Keep founded/inhabited, inlet/frontage and Normal/relaxed comparisons; no new needs/resources/catalogue expansion to manufacture purpose.

The inhabited frontage now compares livelihoods, not just composition: two fields plus a dock and shared meal ground,40vegetables per combined crop plus fish,8loose logs instead of12 with equal total timber investment. Keep these differences explicit. Retain existing rendering provisionally; broad lawn, small productive extent and stepped banks remain aesthetic hypotheses for human reaction. Dense stutter, intermittent save replacement and unauditioned sound remain limitations. Below is a hypothesis inventory, not an execution queue.

## Review170 direction

The public candidate is clearer; its central pleasure is still unproven. Test a wanted livelihood or alteration through real use, including a plausible mistake and recovery. The two tested food approaches both contain vegetables; catalogue-wide food balance and woodland-food discovery are not established. Short satisfied observation is valid. Freeze automatic start/tool/prop expansion. Do not add needs or mandatory growth to manufacture purpose.

Retain the connected woodland and entrance-facing view;20 smaller trees preserve total timber but change clearance/habitat/access. Row collection changes throughput, not yield. Remaining stepped banks and broad lawn are secondary to actual motion/audio and intention evidence. Specific pose/HUD costs improved, but outside-callback stalls and historical save denial remain unresolved. See [review170](REVIEW_CHECKPOINT_170.md) for alternatives, falsifiers and conditional profiling investment.

## Review160 priorities (historical)

Compare wanted change in the clearing with founding a useful farmstead before another capability batch. Post-application recovery is an open design issue: restoring an arrangement must not rewind elapsed food/work. A river-farmstead landscape with substantial productive strips and revised framing is the structural visual alternative to the current lawn/stepped shore; it is not an approved prop list. Preserve full catalogue access, but stop automatic generalization of group tools, templates and inspection panels.

Measured ordinary resident-animation and dense outside-callback stalls remain unresolved. A bounded attribution/intervention and actual synchronized motion/audio audition are higher-value evidence investments than new infrastructure. See [review160](REVIEW_CHECKPOINT_160.md) for experiment falsifiers, costs and evidence limits. No161 is authorized by the completed batch.

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

Current public game: recommended working clearing plus player-founded and inhabited comparisons, with Normal/relaxed constraints and voluntary finishing. Earlier farmstead, court, lake/gathering and campaign experiments remain behind developer access. Nineteen buildings, meals/material routes, homes/rest/recreation, optional comfort, fishing/stone/wildlife, woodland/landscaping, four-way buildings, paths, save/resume, audio and menus provide the working base.

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


Checkpoint124: selected homes/workplaces can start a path at their actual entrance, preview the existing route algorithm to another place/ground, and return to the origin after apply or cancel. No new path rules or resources. Native960 real click route/cancel/origin return and exact save/load passed20261003-135931-864-river-hamlet-5af888 (34.6s), zero-warning build. Next125 improves the legibility of worked ground and freezes for the full direction review; additional home controls remain a review concern.


Checkpoint125: cultivated strips now have raised earth shoulders, dark furrows and a low working edge instead of permanent floating field placards. Ground tint invalidates on changed field depth, so released/restored ground updates with editing. Same actual footprints/yields.1440 ordinary1x10-second before/after captures20261003-140134-628-river-hamlet-902d35 inspected;960 relaxed overview140207-122-cultivated-bank-relaxed-5f8d69. Both-mode extension checks pass; zero-warning build. Five playable outcomes committed; freeze for independent whole-game125 plus visual/audio before126. Furrows improve local legibility; broad scene appeal and motivation remain unproven.


Checkpoint126: Play now offers A working clearing: eight housed residents, two small strips apart from four homes,16 spare logs/4planks/80food, Normal/relaxed and distinct current save/restart identity. It is a different starting situation, not equal-population comparison with the finished hamlet. One four-row strip failed a60-minute no-change test despite passing20minutes; replaced with two two-row strips at identical area/investment to give sufficient concurrent farm work. Keep, move-one-field and add-landing alternatives each pass60simminutes with zero hungry ticks in both modes and exact continuation. Native960 actual menu/household/save and ordinary life passed142319-713-working-clearing-7dabc9 (24.9s), zero-warning build. Next127 tests direct manipulation of an existing field edge; no new needs or prescribed completion.


Checkpoint127: existing strips expose a gold far-edge handle that can be dragged directly on the land. Dragging changes only the proposal; explicit apply/cancel, timber/preparation and crop-loss rules remain shared with button controls. Native960 actual mouse press/move/release, unchanged-world proposal, apply and preparation passed142636-635-plot-revision-2f484c (55.47s), zero-warning build. Next128 reuses route previews while moving existing places to connect arrangement with possible daily journeys.


Checkpoint128: relocation previews now show possible food/work connections using the same authoritative route query as new placement. They describe opportunities, not promised meals. Queries restore the original building/order without changing saved state. Four Normal/relaxed home/field move cases pass actual food production and exact continuation after15 simulated minutes; the initial five-minute test ended before starting food reserves triggered field work. Native960 actual move/cancel preview passed142928-586-working-clearing-cb312b, zero-warning build. Next129 removes qualitative public resident grades in favor of observed daily life.


Checkpoint129: public resident panels now say Daily life and show actual activity, eating history and home/recreation visits. Public rest feedback no longer expires into a deficit, the staffing panel drops its recent-rest quota, and public idle poses no longer use hidden mood grades. Archived happiness rules remain available with archived play. Both-mode descriptive/pure-history and exact-save checks plus archived happiness pass; native960 actual panel and village controls passed143711-232-river-hamlet-d23d32 (23.78s), zero-warning build. Next130 gives shared meal ground a quieter furnished presence, then freezes for the full review.


Checkpoint130: shared meal ground now has six permanent low stools at the actual eating positions, replacing empty target rings; the existing eating pose uses these seats without duplicate furniture. Carried food remains tied to real meals. The development mats comparison remains available. Ordinary1440 shared-place before/after15seconds1x capture143934-282-commons-recurring-a90a8c inspected; native960 working-clearing menu, household/save and relocation preview passed144016-477-working-clearing-89962c. Zero-warning build. All ten authorized playable outcomes are complete; freeze for whole-game130, broadened41-suite current regression and review synthesis. No131.


Final130 validation: all41 current-experience suites passed exit0, run bef67629662c4de8beefe27a569bfb1a, assembly 558ab58f-6ece-4486-b425-0ec5473e255a, log artifacts/checkpoint130-current.log. This includes extension cancellation, relocation connections, descriptive life and all six60-minute working-clearing branches. Corrected DLL FF13CE9E2CB5FA7F0B18C0C87150B66F791A02AB31AC213C91DEB124B4BA78E1 built with zero warnings. The only postreview game delta is the independently verified limited-preview wording; fixed130 native controls/scene evidence remains explicitly before that text correction. Ten outcomes and both whole-game reviews complete. Stop130, no131.


Checkpoint131: Play recommends the existing working clearing while retaining the founded/inhabited alternatives. The clearing mode page shows Normal and relaxed descriptions with compact New/Resume action rows, avoiding inventory paragraphs and hidden mode choices. Native960 actual menu rectangle assertions, entry, save and household controls passed154643-572-working-clearing-920beb (36.71s including build), zero-warning build. Next132 begins the continuous riverbank treatment; no new starting variant or rule system.


Checkpoint132: public riverbanks now slope into a continuous silt/shallow-water band with joined corner normals and interpolated colors. Authoritative land/water, terrain height, fishing and placement remain unchanged; the build boundary stays explicit in tools. Rejected an initial segmented border after inspecting it; revised whole-scene1440 capture155042-668-working-clearing-56664a removes the ladder-like seams (16.46s including build). Zero warnings. The large grid bends remain visible and are not claimed solved. Next133 makes existing woodland read as a connected margin; assess the entire landscape at135.


Checkpoint133: low irregular woodland undergrowth now joins the actual standing mature trees into a wooded margin. It clears from building footprints, paths, home yards, shared places and actual well-used ground; felled/removed trees leave the margin and regrown trees restore it through the existing visual refresh. Single batched mesh, no resource or collision introduced. Opposite1440 view155253-009 inspected and density softened; final960155403-183-working-clearing-3d3c12 inspected, zero-warning build. Next134 treats the remaining open lawn and actual walking wear together; whole-scene acceptance remains for135.


Checkpoint134: open meadow now has broad restrained color variation and low grass distinct from woodland cover. Buildings, domestic space, paths and repeated real footfall suppress it, making occupied and traveled ground stand out without changing movement or resources. Ordinary1440 before/after12seconds1x155625-560-working-clearing-31e345 inspected; native960 actual home-to-field path through the meadow and updated ground passed155722-305-working-clearing-008c76 (35.58s including build). Zero warnings. Next135 completes readable crop cover, then the review must judge whether this is enough of a scene-level change rather than merely more detail.


Checkpoint135: public vegetable fields develop spreading faceted leaf cover from seedlings to mature rows; each visible plant still represents one actual remaining portion, and harvesting exposes stubble/soil. Same footprint, yields and timing.1440 ordinary12-second1x scene155920-347-river-hamlet-57b182 inspected; native960 existing field drag/extend and real preparation passed155950-254-plot-revision-77c06a (43.4s), zero-warning build. First five committed outcomes complete; freeze for full135 plus independent visual/audio review. Presentation is not accepted merely because more detail exists. Ordinary trace includes a486ms wall outlier and67ms process maximum; investigate provenance before attributing it or claiming smoothness.

Checkpoint145: quiet neighbors can visit actual nearby shared seats between jobs, with seated poses and truthful resident/place readings. Furnished home use remains; new work and ordinary meals take priority. Selected removal/moving interrupts only that place. Both-mode checks passed actual visits, food, single-worker construction preemption and exact active save. Native960 multiple-place controls222510-359 passed; ordinary1440 twelve-second1x quiet scene222438-628 inspected. Zero-warning build. Freeze for full145 with five independent roles before choosing146–150; quiet activity is not evidence of enjoyment.

Checkpoint146: all shared workers may make bounded nearby visits, including furnished-home residents. Visits alternate with home time, use actual paths, yield to work/meals and clear on edits; actual arrived companions mutually face/gesture, without a relationship need. Save54 records visit intent/timing. Four ten-minute Normal/relaxed single/split-place comparisons kept ordinary work active and demonstrated domestic use, vegetables/meals, both previously eligible/excluded residents, companion activity and exact continuation. Native960223856-175 passed visible-ground selection and Watch camera/selection return alongside prior multi-place controls. Ordinary1440 twelve-second1x active-work snapshot223908-638 inspected. Corrected meal-only wording and moved-building surface invalidation, no extra count. Next147 addresses house massing rather than more routine substeps.

Checkpoint147: public cottages now form a stable three-part massing family: low horizontal timber dwelling, compact steeper-roofed framed home, and expanded dwelling with a lower adjoining room. Foundations, wall courses, openings, roof proportions and chimney positions differ; same beds/cost/footprints and saved building-ID appearance. Museum reference supports modest timber construction and expanded one-room dwellings; this is stylized, not a reconstruction.1440 overview224207-230 inspected; native960 opposite-camera working-clearing224215-567 passed menu/household/save, movement, paths and Watch return. Zero-warning build. Next148 integrates shared ground; presentation supplement due now without changing regular150 cadence.

Checkpoint148: public shared places now blend into the existing domestic/path/worked ground instead of sitting on a hard polygon pad. Linear seats form simple joined benches with a clear outward orientation, used by actual diners/quiet visitors and marked in the proposal; gathered seats remain separate stools. Layout/direction now participate in geometry invalidation. No occupancy/cost/collision change. Native960224625-114 passed line turn, gap selection, Watch return, independent editing/removal/save; ordinary active mixed-place1440 twelve-second1x224525-693 inspected. Zero warnings. Next149 composes houses, growing strips and approaches together, not additional furniture variants.

Checkpoint149: recommended clearing now staggers homes, turns two toward village approaches and brings both real growing strips closer to the domestic group. Initial eight people/eight beds, two fields/four rows, growth, building investment,16logs/4planks/80food remain matched with the former layout. Default public framing is closer. The old composition remains development-only. Matched ten-minute Normal/relaxed comparisons passed actual vegetable meals (former13/recomposed15),32grown each, no hunger and exact continuation; this is not a throughput-equivalent or preference claim. Six60-minute branches passed. Native960225130-924 passed menu/household/save/move/path/Watch. Same-camera1440 ordinary12s1x225218-443(recomposed) and225237-545(former) inspected: retain recomposed provisionally for its closer home/field relationship. Next150 completes selected shared-place path approaches, then full review and stop.

Checkpoint150: selected shared places now start an actual path proposal to homes, workplaces, other shared ground or open land. Existing route computation/commit remains authoritative; visible shared ground snaps to its center. Apply and cancel return to the selected origin and previous camera, also consistently for building-origin paths. Native960225453-056 passed preview, no-mutation cancellation, actual connection, selection/camera return, multi-place edits and current save/load. Zero-warning build. Ten authorized outcomes141–150 are committed next; freeze for full150 review, final regression and push. No151.
