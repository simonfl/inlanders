# F07c — meals and neighborhood pantries

Design decision for a prototype, not shipped behavior. The aim is an inhabited food destination and a useful choice between a compact village and supplied outlying neighborhoods. Do not add a pantry that is merely another global food counter.

## Current behavior and implications

`Simulation/Food.cs` sends production cargo to `YardAccess`. Every 60 simulation seconds it instantly consumes up to one edible portion per resident from central stock. The round-robin food selection balances the types actually eaten; it does not allocate particular portions to particular residents. Hunger is the unserved fraction and lowers work efficiency by up to half. Residents make no everyday meal journey. Supper is a separate physical gathering with a bread payment.

`Simulation/FoodFlow.cs` records pantry arrivals as production supply and meals as consumption. `Happiness.cs`, newcomer readiness and river/lake campaign assessments rely on those totals. Timber stockpiles have local stock, reservations and shared haulers, but do not implement food service. All these consumers must be revisited together when meals change. Old-save migration is unnecessary; fresh saves must preserve reservations, cargo, service progress and assessment history exactly.

## Selected player experience

Residents obtain one edible portion at a reachable stocked pantry and take a short visible meal break nearby. A central food point is available from the start. A player can build a neighborhood pantry near remote homes/workplaces and supply it through direct producer deliveries or existing haulers. Food in a basket or an unfinished pantry cannot feed anyone yet.

Food, home rest and recreation remain separate activities, but each occurs between jobs. Finish a committed work unit and deliver its cargo before starting a meal; eating must not strand boat crews or construction shipments. An overdue meal takes priority over another optional rest/recreation visit or new production job. Do not interrupt every resident simultaneously at a day boundary.

Choose a **pantry**, with an open counter, actual food baskets and visible visitors. A staffed market, stallholder, prices and trade are unnecessary for the first version. Serving food needs no permanent worker; the cost is construction, land, transport and residents' time. Grain remains an ingredient, not edible pantry stock. Support all five current edible types rather than forcing a new diet.

### Meal accounting to prototype first

- Retain the nominal demand of one portion per resident per 60 seconds. Introduce a meal request at each resident's staggered due time, with a 60-second service window. Initial requests are spread across the first minute. This explicitly changes startup grace and campaign timing; measure it rather than silently preserving old completion targets.
- At most one outstanding request per resident. A failed window records one missed meal and opens the next request; there is no accumulating debt or catch-up binge. Keep a stable request ID so reload and retries cannot count a meal twice.
- Claim one actually stored portion at the chosen reachable pantry, accounting for other claims. Prefer the nearest route with stock and a usable visit spot; break ties deterministically. Do not promise portions still in transit. Rotate request ordering so low resident IDs do not always win shortages. Food-type balancing uses actual recent consumption among eligible local foods; distant variety is not compulsory.
- Walk to collect, then eat at a reserved nearby spot. Move stock into carried meal cargo at collection and count consumption only on completing the meal. Trial four seconds of eating; duration and stagger are tuning values, not extra food requirements.
- A request already collected may finish after its service deadline; it remains the resident's only request until completion. Mark it late, and never consume or count it twice. A request with no collected portion expires at its deadline and releases its claim. Decide the next due time from the prior cadence, skipping elapsed slots rather than accumulating meal debt. This bounded late-meal exception needs explicit tests and UI wording.
- Count eaten portions when eaten and record missed/late service separately. Hunger and campaign checks use closed request outcomes, not goods reserved or collected. Keep the existing maximum hunger slowdown initially, with no deaths or additional fatigue penalty. The exact rolling evaluation window and late-meal treatment are an implementation decision to settle in F07c1 before gameplay integration; do not wire campaign completion to provisional totals.

The prototype must resolve timing/accounting before exposing this as ordinary gameplay. A late job should cause an understandable delay, not an unexplained permanently hungry worker. If one-minute demand overwhelms normal work/home routines, revise the routine and remeasure all producers instead of adding pantries to compensate for an arbitrary travel tax.

### Pantry supply and ownership

Trial a six-log pantry on the standard six-tile footprint, with 24 edible portions shared across types and a default target of 12. These values are provisional. Keep capacity, stored, meal-reserved and incoming counts visible. The central pantry remains unlimited in this first pass and cannot be demolished.

Producers may deliver edible cargo directly to a nearer completed pantry with available capacity. They reserve destination space when starting the delivery; otherwise they return to central storage. Existing haulers optionally transfer food from central storage to local targets, in batches up to four of one food type. Targets count stored plus committed incoming stock; meal claims remain part of stored stock until collection. Direct deliveries do not require a hauler. Do not add local-to-local rebalancing or a new delivery worker role initially.

Haulers may claim only unreserved central food, leaving enough for current central meal requests. Limit incoming reservations by capacity. Record a producer's first pantry deposit as delivered supply; a later transfer is not new production. Food coverage includes all stored edible food but excludes carried meals and shipments; show unreserved versus promised stock separately. Invitation readiness must explain the distinction between village reserves and local service access.

| Change during a trip | Required behavior |
| --- | --- |
| Destination fills | A valid incoming reservation still fits; another transfer cannot steal its space. |
| Pantry closes for demolition | Stop new claims. Release uncollected meal claims and choose another food point. Redirect incoming deliveries to central storage; evacuate stored food physically before dismantling. |
| Resident reassigned or supper starts | Release uncollected claims. Return collected but uneaten food physically before another incompatible job; do not count it as eaten. Supper waits for safe cargo handling. |
| Route or visit spot unavailable | Explain access/space separately from food shortage; release unusable claims and retry a reachable alternative without endless rerouting. |
| Save during collection, eating or transfer | Restore the same request, source, reservations and cargo; consumption/delivery events occur exactly once. |

Supper remains a distinct celebration, not a way to reset daily hunger. Its bread payment must use unreserved food and cannot take an ordinary resident's claimed portion. Initially require central unreserved bread for the celebration and explain this in its button; broader collection can follow if needed. Trade likewise cannot spend meal-reserved berries.

## Does local service earn its place?

Compare the new central-meal routine against the same routine with local pantries. Also report the change from today's instantaneous meals: adding journeys is a real labor cost, not an optimization of existing behavior.

An illustrative upper-level calculation, **not a simulation result**: eight residents making round trips of 20 tiles each way use 320 tiles of travel per meal round. A local food point four tiles away uses 64. Supplying eight portions in two four-portion loads over a 16-tile supply route adds 64, for 128 total. This assumes the stated round trips, full loads and no extra detours; actual workers continue from their destinations and supplies may originate elsewhere. Measure actual paths and work time before adopting the building's cost or claiming payback.

Use three matched fixtures: compact village, two separated neighborhoods, and an awkward pantry placement. Keep population, starting stocks, producers and measurement duration identical. Include construction and hauling labor; measure timely/missed meals, resident and supplier travel, production, rest/recreation participation and stock left in every location. A garden-only food economy must remain viable. Add recovery from no hauler, an empty remote pantry, blocked access and demolition during deliveries. The poor layout should be recoverable through staffing, supply or relocation by demolition/rebuild.

## Implementation chunks

1. **F07c1 — request/accounting experiment.** Build a focused simulation fixture for staggered demand, claims, collection, eating, missed/late windows and interruptions. Resolve the rolling hunger/campaign accounting decision above. Establish a central-only baseline including actual travel. Keep ordinary gameplay unchanged until the complete routine passes; this is explicitly a development experiment, not a shipped pantry. Produce measurements and adjust this design before integration.
2. **F07c2 — playable central and neighborhood service.** Add the pantry, direct deposits, optional hauler supply, physical meal visits and visible eating. Integrate food conservation, trade/supper, resident reasons, Economy, invitations, saves and existing campaign assessments together. Cover every edible resource. Render actual stock and trips at narrow/wide UI sizes. Compare central/local/poor layouts; remeasure all current campaign routes without padding goals to disguise reduced productivity.
3. **F07c3 — campaign review, conditional.** Review human understanding and working-village pacing. Only then use food access in a new scenario or revise an existing challenge. No mandatory pantry count or new food type; a compact central solution remains valid where geography allows it.

If actual meal travel mostly produces congestion and waiting, revise or cut this feature. An alternative worth reconsidering is local producer deposits with distributed consumption, but that would improve producer logistics rather than visibly serve residents; label that change honestly. Avoid building both systems merely to keep the feature ID alive.

## Whole-roadmap review

The next implementation is F07c1, not another food producer or an authored mastery level. Existing resource prototypes and recreation alternatives provide enough choices to test. F23a aesthetic acceptance still precedes a broad art pass; river/lake human pacing still precedes more mastery scenarios. Home comfort remains a candidate after the art direction. Education/reflection, orchards, livestock, a river mill, terrain shaping and larger populations remain tentative. Keep UI feedback within this feature and profile actual added routes before increasing village size. No seasons, migration framework or compulsory new needs.
