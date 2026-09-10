# Inlanders: from a working simulation to an enjoyable village

Review date: 9 September 2026. Baseline: `32fb060`.

## Assessment

**There is a substantial game foundation here, but the experience has not yet earned the visual appeal and satisfying decisions its feature list suggests.** The strongest ingredients are physical work, visible growth, a peaceful mood, and the ability to arrange a settlement. The weakest parts are architectural character, readable cause and effect, and reasons to keep improving a village after its initial setup.

The user's reaction that the buildings look flat is the most important evidence for the next phase. Completing more systems will not resolve that reaction. We should make a small village feel worth looking at, then make managing and improving it equally rewarding.

This review combines source inspection, existing rendered captures from the current implementation, a fresh passing simulation suite, and specialist design, visual/UX, and persistence reviews. It is **not a new independent human playtest**. Statements about actual mechanics are confirmed below; proposed prices and judgments about pacing are hypotheses to test. No gameplay or asset changes were made for this review.

## What already works

- **Work is physical.** Villagers harvest, carry, deliver, construct, plant, bake, saw, and gather. Reservations and inventories provide a strong basis for believable activity.
- **The tone is coherent.** No combat, deadlines, deaths, or forced failure. Hunger slows work; players can recover. That should remain the direction.
- **The modes have distinct purposes.** Campaign currently teaches and should grow into authored challenges that test mastery; normal free play supports an economy; Creative supports arranging and watching.
- **The five lessons reduce repeated setup.** Later settlements start with useful buildings. Progress survives consumption, and completion allows continued play.
- **There is already enough material for better scenarios.** Hills, crossings, storage, planting, housing, gardens, and social spaces can create interesting combinations without another resource chain.
- **The implementation is unusually well checked for a prototype.** This reduces the cost of changing design. Passing tests establishes rules and reliability; it does not establish enjoyment.

Keep Godot/C# and the existing simulation. This review does not justify an engine change, an ECS rewrite, save migration work, or a large technology tree.

## Why the appearance feels flat

“Flat” describes the composition and architectural treatment, not the absence of 3D geometry. Cottages already have pitched roofs; the bakery already has an oven and chimney. Nevertheless:

1. **Most buildings are similarly sized objects on rectangular trays.** Their mass, base, and footprint communicate little difference in importance or purpose.
2. **The lodge literally starts with `MakeCottage`.** Siding and a second roof do not communicate twice the housing capacity.
3. **The sawmill and forager hut read as thin shelters.** Posts and shallow roof slabs lack the substantial shapes that remain readable from the town camera.
4. **Detail is concentrated at the wrong scale.** Bread scores, roof seams, and small props disappear before they improve the overall composition. Large shapes and dark openings need to work first.
5. **Ground contact is weak.** Slab edges isolate each object from paths and landscape. Fields resemble small display trays. The square reads more like a picnic table than a place a village gathers around.
6. **The surroundings compete with the buildings.** Strong green tile variation, conspicuous labels, bright paths, and sometimes enormous log stacks draw attention away from homes and people.
7. **Activity and decoration sometimes disagree.** Static bread, logs, planks, or oven glow can suggest work or stock regardless of actual buffers. A village becomes more rewarding to watch when appearance responds to what it is doing.

Evidence: [cottage construction](../Visuals.cs), [lodge and sawmill](../SawmillVisuals.cs), [bakery](../BakeryVisuals.cs), [fields and forager hut](../FieldVisuals.cs), [yard stock rendering](../Game.cs), and current `f12d-working-hill`, `f12d-raised-crops`, `f21a-build-960`, `f08-sawmill`, and campaign captures under ignored `artifacts/`. These captures are implementation fixtures, not carefully composed player villages.

### Proposed art direction

**A handmade woodland village: substantial buildings, simple expressive forms, and visible working spaces.** Thick timber, warm plaster, chunky stone supports, deep shaded entrances, and a restrained clay/sage/slate palette. Keep the low-poly simplicity. Give buildings different proportions rather than making everything taller or more ornate.

Use three scales of detail:

- At overview distance: roof shape, enclosure versus open frame, crop mass, and a recognizable civic center.
- At normal play distance: entrance, work frontage, chimney, material groups, and incoming/outgoing goods.
- Close up: joinery, utensils, trim, small plant variation. Add these after the first two scales succeed.

First establish geometry and material values in normal daylight. Then tune ground contrast, shadow harshness, and light. Golden-hour lighting should complement the buildings, not conceal their weaknesses.

### First visual slice — F23a

Build one small, connected scene with **cottage, bakery, sawmill, path, trees, garden, and villagers**. Preserve gameplay footprints and costs for this slice so we can judge the visual change independently.

- **Cottage:** a compact solid home with a substantial roof, a recessed doorway/porch corner, dark window openings, and stone feet meeting a worn doorstep. It should look inviting before decorative props are added.
- **Bakery:** keep the successful oven/shop idea. Merge the oven and chimney into a dominant masonry mass, lower the shop beside it, deepen the serving opening, and simplify the canopy. Show baking and real input/output stock through a few large cues.
- **Sawmill:** heavy exposed frames and braces, a stronger broken roof profile, and a visible cutting line from timber cradle to saw to plank rack. Let the sawyer and moving saw remain visible from the ordinary camera.

Judge all three together at camera size 23, at 960×640 and 1440×900, from four orientations, with labels hidden. Compare grayscale captures for depth. Check empty, working, and stocked states. Most importantly, ask whether this scene makes the user want to linger. Do not expand the style across all ten buildings until this slice is convincing.

An authored mesh workflow is an option if procedural geometry becomes awkward, but the first milestone is the visual result, not a new asset pipeline. Do not buy an asset pack or introduce Blender as a required dependency without a concrete reason.

## Complete building and cost review

Normal-play values below are from [Settlement](../Simulation/Settlement.cs), [Food](../Simulation/Food.cs), [Sawmill](../Simulation/Sawmill.cs), [Storage](../Simulation/Storage.cs), and [Leisure](../Simulation/Leisure.cs). Creative construction is free and instant. Every normal building currently needs **12 builder-work seconds after delivery**, excluding travel and hunger slowdown.

| Building | Current cost / footprint | Purpose and payoff | Main design question / visual direction |
| --- | --- | --- | --- |
| Cottage | 6 logs / 3×2 | Two beds; basic housing | A welcoming small home. Keep it the economic baseline. Housing is global capacity, not an individual household simulation. |
| Lodge | 8 planks / 3×2 | Four beds; material-chain upgrade | Give it a visibly larger communal mass and porch/side volume. Decide how strong the housing-efficiency reward should be. |
| Forager hut | 6 logs / 3×2 | Allows two simultaneous foragers | Currently a worker permit: foragers travel bushes → yard, never through the hut. Its location does not create a gathering radius or depot. Either explain that honestly or later give its placement a real function. Broad woodland roof and a visible basket alcove. |
| Farm | 6 logs / 3×2 | Six grain after 45s growth; one concurrent farmer | A broad productive field, not a house-sized planter. Grain needs a bakery; land and tending should matter more than an arbitrary building fee. |
| Vegetable garden | 6 logs / 3×2 | Eight directly edible vegetables after 60s growth | Lush kitchen-garden identity with mixed plant shapes. Its simpler chain should be a legitimate choice, not a deliberately weak precursor to bread. |
| Bakery | 6 logs / 3×2 | Two grain → four bread in 10 work seconds; one baker | A substantial processing workshop and a visible reward for a longer chain. Current oven/shop identity is worth retaining. |
| Sawmill | 6 logs / 3×2 | Two logs → four planks in 10 work seconds; one sawyer | Exposed machinery and timber movement. Its fixed global target of eight planks limits later management choices. |
| Village square | 6 logs / 3×2, plus nearby gathering space | Four concurrent recurring visitors; hosts supper | An open communal place framed by seating, shade, and ground treatment. People should be the focal point. Choose whether it is a modest meeting place or a larger civic structure before pricing it. |
| Stockpile | 6 logs / 3×2 | Holds 12 logs, target 0–12; local collection and hauling | Its value is shorter trips: the main yard already has unlimited storage. A compact, inexpensive storage bay may invite more useful experimentation. |
| Bridge | 6 logs / one water tile | Creates a crossing and can shorten routes | Geography determines its value. Strengthen timber structure and bank approaches; do not change cost just because its footprint is smaller. |

Nine buildings share a footprint; nine share a price. Eight share both. Uniformity simplifies the current implementation, but these values should no longer be treated as settled design.

### What the current economics imply

**Housing:** a cottage costs three logs per bed. Eight planks for a lodge require four raw logs, so its marginal material cost is one raw log per bed. Including a new six-log sawmill, the first lodge costs ten raw logs versus twelve for two cottages. Two lodges plus the mill cost fourteen raw logs versus twenty-four for four cottages. Processing takes labor and the mill occupies land: the first lodge/mill pair uses the same twelve tiles as two cottages. The advantage grows with expansion. This can be an intentional reward; the art and UI should communicate it.

**Food:** a garden needs at least 4s sowing + 60s growth + four 2s harvest actions: 72s for eight vegetables, before travel, transitions, and breaks. Its no-travel ceiling is about 6.7 food/minute, below the eight food/minute needed by the original population. A farm needs at least 4 + 45 + three × 2 = 55s for six grain, convertible to twelve bread: about 13.1 bread/minute before baking/transport constraints. These are per-field analytical ceilings, not measured village throughput; a farmer can tend multiple fields during growth. The bakery adds another building, specialist, and transport stage.

Bushes regenerate one berry per eight seconds, or 7.5/minute per bush before harvesting and travel limits. Foraging is cheap but geographically constrained; cultivation lets the player choose where production happens. That is a better distinction than assuming later food must always be superior.

**Investment is more than logs.** Compare material cost, land including access, worker time, travel, startup delay, upkeep, and visible payoff. Increasing construction waits to make buildings feel expensive could make the game worse.

### Candidate tuning experiments — F24

These are alternatives to test, **not approved new prices**:

| Building | Candidate range or experiment |
| --- | --- |
| Cottage | Keep 6 logs / two beds as the baseline. |
| Lodge | Compare current 8 against 10–12 planks / four beds. Retaining 8 is valid if the upgrade is deliberately generous. |
| Forager hut | 4–6 logs while it remains a permit; reassess if its spatial function changes. |
| Farm | 2–4 logs, emphasizing cultivated land and farmer time. |
| Garden | 4–6 logs; preserve its approachable direct-food role. |
| Bakery | 8–10 logs only if the production reward and level-2 pacing support it. No new plank prerequisite for this earlier lesson. |
| Sawmill | 6–8 logs, evaluated with lodge economics rather than independently. |
| Square | 4–8 logs after settling its civic identity. Avoid making social play feel like an expensive tax. |
| Stockpile | 2–4 logs and/or a smaller footprint, conditional on actual travel savings. |
| Bridge | Keep 6 initially; test usefulness with alternative crossings. |

Run a few comparable settlements with different food mixes, cottage/lodge choices, and near/far storage. Record time until useful output, sustained surplus, worker allocation, travel, and how often the player makes a worthwhile decision. Centralize building definitions when doing this so costs, descriptions, previews, and tests cannot drift. Do not introduce a general modding framework.

## Is it fun, and where does satisfaction come from?

The intended loop should be: **choose something the village needs → shape a plan → see people do understandable work → enjoy a visible improvement → notice a worthwhile next possibility.**

**Campaign direction clarified after this review:** the user finds the present campaign closer to a short tutorial and wants levels that require skill and take longer than a few minutes. Treat the five existing lessons as onboarding. The roadmap now proposes a 20–30-minute first substantial river settlement, followed by 30–60-minute scenarios involving distance, constrained land and staged growth. These are unvalidated first-play targets, not enforced timers. Challenge should come from construction sequencing, labor allocation, supply planning and adapting a layout; simply increasing output quotas would preserve the current weakness. Prototype one such settlement before authoring the rest. See [campaign expansion](ROADMAP.md#campaign-expansion--f11b--f18b) for scenarios and acceptance checks.

We support the middle of that loop better than its beginning and end. Current goals often become a short burst of placement followed by cumulative production. Level 4 especially introduces a square after most of the economy is already prepared. That may be peaceful observation or uninteresting waiting; only playtesting can establish the balance. A two-to-five-minute scripted solution does not answer that question.

The most promising sources of satisfaction are:

| Source | What currently weakens it | Direction |
| --- | --- | --- |
| Pride in a place | Generic proportions, isolated bases, sparse composition | F23: a village worth arranging and watching. |
| Understanding and mastery | Shared workers, indirect causes, weak diagnostics | F21g/h: show what a decision changes and why work waits. |
| Experimenting with layout | Completed normal buildings cannot be removed | F16b: safely rebuild with an explicit recovery policy. |
| Caring for residents | Houses are capacity; most happiness inputs are shared | Show credible social/home activity before adding individual need meters. |
| Seeing progress | More output often changes only counters | Visible stock/activity and meaningful neighborhood/campaign outcomes. |
| Long-term tending | Individual tree orders and mostly automatic production | Managed woodland, production controls, useful local logistics. |

**Food variety needs a deliberate redesign.** Current variety is pantry availability before eating, and meals consume berries first. A single vegetable can remain untouched while repeatedly qualifying for variety. This matches current goal wording, but makes variety a weak decision. F14b should connect the score and level-5 lesson to actual consumption, with a clear last-meal breakdown. Decide whether this means village-level participation or a fraction of residents served; do not imply every resident ate a varied meal from one vegetable.

Do not manufacture excitement through deaths, punitive upkeep, seasons, or deadlines. Stronger choices, reversible placement, more appealing outcomes, and believable activity fit this game better.

## Player clarity

**Construction catalog:** text-only cards repeat “6 logs,” while descriptions and the existing-building directory sit below many controls. Use recognizable building thumbnails, one-line purpose, cost, required staff, and next dependency. Separate Place, Landscape, and Existing buildings. Keep placement explanations near the action.

**Staffing:** “+ Worker” in an inspector changes the village-wide role pool and can move a worker from another job. Explain which role/person will change before the action; display local occupancy separately. Consider preferred workplaces after testing clearer controls. Do not promise local assignment while continuing to manipulate a global pool silently.

**Operational feedback:** distinguish no worker, missing input, input travelling, output awaiting collection, player pause, and target met. Show a relevant source/worker link and a short recent food production/consumption summary. An idle worker is not automatically a problem; taking a break should not be labelled simply “Idle.”

**Watching:** allow labels on selection/hover and a truly clean view. Activity should be recognizable at normal zoom without reading floating signs. Start with convincing tool contact, tree felling, construction deliveries, and crop handling rather than a large particle system.

## Confirmed fixes and technical limits

These support the art/gameplay work; they should not take over the next roadmap.

| Priority | Finding | Evidence and proposed check |
| --- | --- | --- |
| Fix before feature work | Post-supper editing can produce an unloadable save | `Food.AdvanceFoodTime` retains `MeetingSpots`; `Maps.ValidateMapOccupancy` requires them reachable forever. Reproduced: finish level 4 with square (7,3), let guests leave, place Flowers at (6,4), save, reload → “Map cuts off village access.” Retire old gathering constraints and cover post-supper building/decorating save roundtrips. |
| High | Session recovery differs by entry point | Main-menu New preserves `.before-new`; in-game `Game.Reset` replaces the current world immediately. Window close has no save handler or timed autosave. Unify recoverable restart and add mode-aware rolling autosave without making F9's meaning ambiguous. This is current-progress protection, not old-save compatibility. |
| Small fix | Camera movement can occur during text entry | `Game._Process` polls WASD regardless of focus; camera views use a `LineEdit`. Verify typing a name containing WASD keeps focus/zoom unchanged. |
| Small fix | Activity/guidance copy is stale | Garden travel says “grain”; leisure falls through to “Idle”; logger tutorial assumes all construction supply comes through the yard. Align copy with actual state. |
| Visible defect / scaling concern | Yard grows into an enormous timber tower | `Game.RenderActors` draws every stored log/plank and rebuilds all stock meshes on inventory changes. Cap visual density while retaining exact numerical inventory. |
| Measure before optimizing | Hidden HUD work, placement previews, and decoration rebuilds | HUD/economy scans run every frame; terrain preview patches allocate meshes; decoration edits rebuild the full view. Profile a decorated 16–24-person village and prolonged paused placement. No measured FPS claim is made here. Cache/update/instance the expensive parts found; no renderer rewrite by default. |

A fresh `Test.ps1` run passed. An isolated deterministic audit also passed 1,800 mixed command attempts and about 30 simulated days each in normal and Creative large maps, with periodic save/reload. These small eight-person runs are not a population/performance benchmark. This supports reservation robustness and also illustrates a coverage gap: broad tests missed the specific post-supper edit sequence. Add targeted regressions for discovered failures, rather than treating a larger test count as proof of fun.

## New buildings worth considering later

These are candidates after the current catalog succeeds, not additions needed to rescue it:

| Candidate | Distinct decision it would need to create |
| --- | --- |
| Orchard | Long establishment and persistent planting versus quickly established crop beds; use ordinary growth/harvest cycles, not seasons. Only worthwhile if it changes land planning rather than adding another food colour. |
| Neighborhood pantry / market | A real local food destination that shortens trips and makes a neighborhood work differently. Start with F07b logistics before adding trade, currency, or a new shopper simulation. |
| Carpenter / building improvements | A continuing use for planks that produces visible improvements to homes or communal spaces. Prefer a small upgrade choice before another abstract resource ladder. |

A forester building is not needed for the first managed-woodland feature; existing loggers can follow zones. Wells and new needs should not be added merely to create more upkeep. Keep candidate names, costs and recipes TBD until their decisions are useful.

## What to validate with the player

Use short sessions, and observe before explaining:

1. **Visual slice:** compare old/new villages at the real camera with labels hidden. Can the player identify buildings and entrances, and do they prefer looking at the new village?
2. **First ten minutes:** can the player explain what to build, who will work there, and what success will look like? Record first confusion and first regretted placement.
3. **Waiting:** when the player accelerates time, are they enjoying activity or skipping an empty interval? Record where the distinction changes.
4. **Economic alternatives:** can they explain why they chose gardens versus bread, cottages versus lodges, or a stockpile location? Multiple defensible answers are preferable to a hidden optimal recipe.
5. **After a goal:** do they have something they want to improve? If not, identify the missing payoff before adding another counter objective.

**F23a now has an implemented visual proposal**, documented in the [matched art review](ART_REVIEW_F23A.md). Its aesthetic acceptance remains pending. F21g has now shipped the visual catalog, separate Build sections, pinned guidance, text-focus fix, corrected activity copy and persistent world-label controls. F24 building-role and cost work is next independently of the F23b art decision. The bounded save fix identified above shipped as F19a after this baseline review: supper now clears its gathering destinations on completion, with post-supper decoration/construction and campaign-book roundtrip coverage. No old-save migration or repair was added. Prices and further systems remain planned experiments. Keep all buildings available during campaign revision for now; progressive unlocking remains a later option. Seasons stay removed.
