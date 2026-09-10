# F26a — Life by the lake

Implemented: one complete fishery loop and the seventh campaign settlement, at the existing 8–12-resident scale. Prices, yields and objectives remain tunable. Fish are immediately edible; no preservation chain or additional needs are prerequisites. Human pacing and enjoyment acceptance remain open.

Fish participate in meals, delivery history and current saves; the authored lake has two shared replenishing habitats. Docks support normal construction, cancellation with accessible salvage, Creative placement/removal and demolition. A fisher reserves up to four fish, boards, rows to a reachable ground, spends eight work seconds fishing, returns, unloads and carries the catch to the pantry. Reassignment, pause and supper recall the boat before releasing its passenger; occupied docks cannot disappear and bridges cannot strand active trips. Boats, passengers, oars, nets and actual catch are rendered together. Phase saves, competing reservations, target stops, recall/supper and actual delivery pass focused checks. The broad simulation suite and lake render smoke also pass. Placement shows launch access and reachable grounds; the inspector explains available shared stock. Overview habitat labels remain small at 960, with readable information in the drawer and at closer zoom.

## Design review

**Decision:** invest in water-based food versus cultivation, and choose a landing with useful water and pantry access. Grounds farther along the shore may have more fish; that does not imply building a dock there is better. Weigh timber, worker time, water travel and pantry travel against gardens and bread. Additional docks share habitat stocks; they can improve access or staffing but cannot multiply replenishment.

**Existing logistics constraint:** all food still reaches the central pantry. A remote dock may lose more time on land deliveries than it saves rowing. Compare complete trip cycles before claiming that a farther landing is better; a near dock making longer boat trips can be a valid alternative. Do not quietly assume local food storage exists. Revise the shore/ground arrangement if one landing dominates every sensible plan.

**Visible payoff:** a recognizable landing and small boat; a fisher walks to the dock, rows out, catches fish, returns and unloads, then carries a basket to the pantry. Residents eat fish through the ordinary meal system. Habitat feedback explains both available catch and replenishment before construction.

**Smallest complete scope:** a compact shore building, one boat and one fisher slot per dock, authored fish-ground markers on connected water, bounded replenishing stocks, ordinary pause/target controls, delivered fish, current saves and a lake campaign. All buildings remain available where their placement prerequisites can be met. No fleets, oceans, weather, spoilage, predators or fishing technology tree.

**Provisional tuning:** a dock costs eight logs, including its boat. Boats bring up to four fish per trip. Fishing takes work time in addition to rowing and land delivery. The first authored grounds contrast a small nearby stock with a larger, faster-replenishing distant stock. Test sustained output and depletion before adopting final numbers; do not balance only against an opening stock burst.

## Integration contracts

- Fish count only after being caught; boat cargo, land cargo and pantry stock conserve the same goods. Meals, variety, invitations, Economy, recent food flow, carried baskets and saves include fish. Three balanced foods can still earn full variety credit; adding fish does not require every village to produce four foods.
- Ground stock belongs to the habitat, never to an individual dock. Competing boats cannot claim the same available catch. Cancelling a trip releases an uncollected claim. Show stock, regeneration and unavailable/claimed supply in ordinary language.
- Boat travel has its own water route. A passenger remains attached to the returning boat after a job change, pause or supper call. Land work and home/rest routines resume only after landing. The selected fisher's visual position and camera follow must match the boat.
- Boat and shore access must survive placement/removal. A dock with a boat at sea cannot disappear; explain how to recall it. A bridge cannot strand an active boat or invalidate its landing. Navigation and dock orientation must work on both sides of the lake, not only one favorable shoreline.
- Rest, recreation and meals continue to matter. Current workplace targets and pause controls apply without introducing a second management system. Boat updates stop while paused, and exact saves preserve outbound, fishing, returning and unloading phases.
- Preserve rendering limits: bounded boat count, batched fixed dock geometry and restrained water/habitat effects. Measure a working lake at the existing population before increasing density.

## Campaign direction

Life by the lake begins with eight housed residents, a modest food buffer, timber and shore access. The narrow settled shore competes with farming and workshops; a walk around the lake reaches alternative land and a better fishing ground. Introduce the fishery through an actual delivered catch, then let the player prepare an expansion to twelve residents. A short operational proof measures full mixed meals, fresh food supply and actual resident participation. Food production may mix fish, gardens, gathering and bread; final success should not mandate a fixed dock count or ownership of every producer.

Author two credible approaches before finalizing quotas: an early nearby dock backed by cultivation, and an investment in the farther fishery supported by a stronger starting food economy. Test overbuilding docks on the same habitat and recovering by reallocating workers or diversifying food. Depletion should explain a decision, not force a long wait for an arbitrary stock threshold. Duration and enjoyment remain human playtest questions.

## Completion checks

Shared-stock depletion/regrowth over several cycles; blocked water and shore placement; competing docks; interruption at sea; return-before-removal; exact saves in every travel/work phase; delivered fish and mixed meals; pause/targets; home visits after landing; two viable campaign approaches and a recoverable inefficient one. Render the dock, rowing, catch basket, habitat preview and management feedback at 960/1440. Review the entire roadmap and record measured tradeoffs before committing the complete slice.

## Implemented objectives and measurements

Deliver four fish to the pantry, then explicitly begin preparing the village. Grow to at least twelve, house everyone, and provide recent home rest for three quarters of residents and square visits for half. Start assessment when ready: three consecutive full mixed meals, fresh deliveries covering consumption, and the same living standards. Extra residents scale requirements. A missed condition resets the short proof and explains why; the player can recover without restarting. Final meals can use any food mix, with no required dock count.

Automatic trips prefer the largest available catch (up to four), then the shortest water route. The initial nearest-ground-only rule repeatedly collected single fish from depleted shallows and ignored fuller grounds. Changing that rule increased near-landing deliveries from 58 to 112 over twelve minutes; no habitat yields or work times were inflated.

Same authored opening, one additional food worker, twelve simulation minutes after construction, with actual home routines:

| Producer | Logs | Built by | Delivered food | Water travel | Land travel | Completed rests |
| --- | --- | --- | --- | --- | --- | --- |
| Near dock | 8 | 45s | 112 fish | 241s | 188s | 4 |
| Far dock | 8 | 134s | 52 fish | 57s | 515s | 3 |
| Nearby garden | 4 | 36s | 40 vegetables | 0s | 319s | 4 |

These are layout-specific comparisons, not universal rates. Land travel includes work and home visits. The near fishery is a strong investment on this map; distant docks currently lose to longer rowing from near the pantry. Do not require a remote landing or pretend neighborhood food storage already exists. Garden output adds a distinct food and does not consume the shared water supply. Broader fishery balance remains a playtest question.

Scripted campaign routes complete in **420 seconds with near fishing/gardens** and **540 seconds with far fishing/bread**. A paused-production test fails the fresh-supply proof despite stored food, then completes after staffing resumes. Owning homes/squares without visits cannot start assessment. These seven-/nine-minute routes do **not** establish the desired longer mastery campaign: the lake is currently another resource introduction. F11b2 should revise scenario pressure and decisions before adding more similar levels; increasing quotas or waiting windows is not the remedy.

Verification: `./Test.ps1`, `./Play.ps1 -FishingSmokeTest`, and focused test arguments `--fish`, `--lake`, `--fish-balance`. The lake smoke covers the actual saved campaign phase button, fish inventory, passenger/boat agreement, paused oars and deliveries at 960/1440. A close 960×640 view of eight residents and one paused boat measured **35.0ms median / 37.5ms p95, 597 draw calls** across 120 frames on the development machine. This is not a populated twelve-person performance claim or a 60fps result. Continue profiling before expanding population or fleets.
