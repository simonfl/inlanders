# F25b — improve the homes people already use

F25b1 implementation brief. **The prototype and working comparisons are complete; player acceptance remains open.** See the [F25b2 comparison](HOME_COMFORT_COMPARISON.md) for measured tradeoffs and the decision to keep comfort optional. Final building art and campaign use still follow their open reviews.

## Player experience

Select an occupied cottage or lodge and order a home improvement. A carpenter collects real planks, carries them to the home and installs the improvement. The home stays occupied. Residents visibly use an improved seat during their normal rest visits; shutters and a modest furnishing detail distinguish the finished home at the normal camera distance.

The decision is to spend planks and worker time on existing residents or on additional housing and other construction. A cramped settlement can improve life without growing. It must remain reasonable to keep ordinary homes, especially near work and food.

## First rules to test

| Item | Prototype starting point |
| --- | --- |
| Carpenter workshop | 6 logs, usual 12 work-second construction, one carpenter slot, existing six-tile workshop footprint. No intermediate furniture resource. |
| Cottage improvement | 4 planks and 12 installation work-seconds; one permanent level. |
| Lodge improvement | 8 planks and 24 installation work-seconds; same investment per bed as a cottage. No extra beds. |
| Ordinary rest | Preserve existing six-second visit, 180-second interval after completion, 240-second recent-rest memory. |
| Improved rest | Same six-second visit; trial 240-second interval and 300-second recent-rest memory. Actual completed use is required. |
| Satisfaction | Keep the existing 10-point recent-rest contribution and 100-point maximum. No new comfort bar, productivity multiplier, compulsory upkeep or penalty for ordinary homes. |

The longer interval is the proposed practical benefit: fewer repeated trips can leave more time for work, meals or recreation. The longer memory gives the same 60-second margin between the next due visit and expiration. It does not prove the resident has enough time to obtain other services. These are starting values, not a promised production increase.

A simple scheduling estimate motivates the experiment. Ignoring all job/meal/recreation delays, the rest share is `(round-trip travel + 6) / (interval + round-trip travel + 6)`:

| Round-trip travel | Ordinary share | Improved share | Theoretical time released per resident over 20 minutes |
| --- | ---: | ---: | ---: |
| 10 seconds | 8.2% | 6.3% | 23 seconds |
| 40 seconds | 20.4% | 16.1% | 51 seconds |
| 80 seconds | 32.3% | 26.4% | 71 seconds |

These are arithmetic estimates, not simulation results or construction payback. Other routines can consume the released time. A near ordinary home remains much cheaper to visit than a far improved home. If the working comparisons show no noticeable benefit, revise or cut this rule rather than adding a second bonus to justify the workshop.

## Orders, work and recovery

- Allow orders only on completed, occupied cottages/lodges not marked for demolition. Explain a missing workshop, carpenter or available planks before ordering; a queued order may wait for supply. One order per home, no repeat tiers.
- Keep existing beds and home visits available during installation. Use the existing entrance for delivery and a reachable work spot that respects meal/rest/recreation reservations. If all spots are busy, wait and explain why; never evict an active resident visit.
- One carpenter owns an active order. Use existing job/meal scheduling and physical material reservations; collect from reachable central/local plank stores. Start with two-plank loads. Show planned, reserved, carried, delivered and installed amounts distinctly.
- Use oldest-order-first for the first prototype. Per-order priority and an elaborate workshop queue UI can wait unless the comparison exposes a need. A paused workshop finishes the current committed delivery/work step before claiming another order, following existing workplace conventions.
- An order follows its building, not its original residents. If the home becomes empty, stop claiming new deliveries/work until somebody lives there again; a carried load may finish its delivery. Household changes do not duplicate an order or its investment.
- Cancel an unfinished order: stop future claims, release uncollected reservations, physically return carried portions, and make delivered planks available for physical recovery. No instant refund of goods already moved. Cancellation does not erase beds or existing rest history.
- Demolition cancels unfinished improvements and recovers delivered/installed improvement planks alongside the building's original materials. Never recover the same material through both order cancellation and demolition. Removing/pausing the workshop leaves the home's remaining order inspectable and resumable from another workshop.
- Capture the rest quality when a resident starts resting. Finish that visit with its captured interval/memory; an improvement completed during the visit takes effect next time. Store the earned recent-rest window on the resident, as recreation already stores its actual visit benefit. Moving or demolishing the home does not rewrite a completed visit's history.
- Creative builds/installs instantly and freely, using its existing material-accounting convention. Normal play retains physical work. Save the new state explicitly; bump the format and reject old saves if needed, with no migration work.

## UI and visible use

The home inspector shows **Improve home · 4 planks** or **8 planks**, the benefit in ordinary language, current occupants, and an order/cancel control. During work, show the carpenter link, delivery/install progress, and the current reason for waiting. Keep this beside household information rather than buried after generic demolition controls.

The carpenter inspector shows its active home and a link to it, pending order count, staff, pause and an explanation when no eligible orders remain. It makes home improvements, not saleable furniture; do not show an irrelevant global output-stock target.

The resident inspector and People services use the same rest explanation: ordinary/improved last visit, earned benefit remaining and next visit due. Remove hard-coded claims that all rest lasts four minutes. Home assignment previews should identify improved homes but still show spare beds and location.

Start with a small visible addition to current home models, preserving roof variants, entrances and footprints. Show a work board/materials during installation, finished shutters/furnishing afterward, and a cushioned version of the actual temporary rest seat while residents use the improved home. Do not make a decorative bench imply service when residents are resting elsewhere. Final art treatment remains open; no full building-family expansion is required to test the rules.

## Comparisons required before adoption

Use normal construction and material production, starting each approach from the same snapshot. Keep populations and available workers fixed. Record workshop setup and installation costs separately from subsequent operation.

1. Compact ordinary village versus the same village with improved homes. Include a no-upgrade route that spends the resources on a real competing project, rather than leaving its workers artificially idle.
2. Dispersed ordinary versus improved homes, then compare both with a better-located ordinary neighborhood. Improvements must not make placement irrelevant or become the universal answer to missed meals.
3. Two cottages versus one lodge, accounting for bed occupancy, route length, one shared workshop, and investment per resident. Do not force all beds to be occupied just to make an upgrade look useful.
4. Interrupt installation, reassign the carpenter, close/remove its workshop, empty/reoccupy the home, cancel, and demolish. Check material ownership through every transition, including saves during collection, carrying, installation and recovery.
5. Keep meals, home visits and recreation running during the work. Report actual service outcomes, travel, useful production, and missed/skipped meals. A larger happiness number alone is not acceptance.
6. Render normal construction, installation and actual improved rest at 960/1440, with pause/reload and rotation. Confirm orders remain understandable and the improvement is recognizable without enlarging footprints.

Keep the feature if it offers a visible, useful investment in an established village while ordinary homes remain viable. Tune cost/interval from those comparisons. If it simply makes residents disappear from home life for longer without an enjoyable payoff, revisit the design instead of adopting it because the tests pass.

## Campaign and chunk boundary

**F25b1 — this brief:** concrete rules, material ownership, UI and comparison requirements. No gameplay change.

**F25b2 — prototype:** workshop, physical orders, improved actual rest, visible home/seat changes, UI and current-format saves as one coherent playable slice. Run the comparisons above and record outcomes; do not ship only a satisfaction flag or a decorative upgrade.

**F25b3 — player review and possible scenario:** try improving an inconvenient working village. Decide whether this belongs in *A place to call home* or enriches another scenario. Require actual residents using the improvement, not a quota of furniture or every home upgraded. Campaign sequence, exact objectives and final art remain TBD; existing buildings stay available.
