# F26b2 — Built to last

September 12, 2026. **F26b2a: brief and tested layout prototype complete. F26b2b: now implemented as level eight; see the [implementation review](QUARRY_CAMPAIGN_REVIEW.md).** This document preserves the design and early prototype measurements. Full routes and current verification are in that review; neither establishes human pacing.

## The experience

An established eight-person settlement wants a shared gathering hall. The nearby outcrop cannot supply it alone. Choose whether to invest in a second camp to shorten stone transport, or build only at the distant source and retain more timber and builder time for the village. Keep food working while moving labor into stone and plank production. Place the hall where residents will actually use it.

The payoff is a visibly used civic building. Do not claim that the hall increases food production or is universally better than a square. Its existing eight visitor slots, twelve-second visits, two-minute interval and four-minute recreation memory remain unchanged.

Unlike river/lake, no bridge or population growth is mandatory. All buildings remain available. The challenge focuses on investment, transport and civic placement; no new need, material, unlock, timer or compulsory comfort upgrade.

## Authored map

The tested flat prototype has a western settlement and an eastern stone clearing joined by a dry narrow neck. Bounds: x −10…17, z −9…10. Western land is x ≤5; the neck is x 6…9, z −3…2; eastern land is x 10…17, z −7…7. Other cells are excluded. No water crossing is needed. Sculpted elevations can wait; they must not silently invalidate the working plots.

```text
North (negative z)
     timber       timber
  near stone    sawmill                 distant stone
    camp       civic green    neck       camp
  homes        food garden  =========  eastern clearing
         yard                            timber
  homes / forager / homes
South (positive z)
```

Coordinates below are centers, rotation 0. They are suggestions, not prescribed placements or locked building zones.

| Element | Coordinate / amount |
| --- | --- |
| Central yard / pickup | (−3,3) / (−2,3), existing engine convention |
| Starting cottages | (−6,0), (−6,4), (−1,7), (3,7) |
| Starting forager hut / vegetable garden | (−6,7) / (2,0) |
| Nearby outcrop / working access | (−7,−5) / (−6,−5): 8 stone |
| Distant outcrop / working access | (14,−5) / (15,−5): 36 stone |
| Proposed quarry camps | (−4,−4), (11,−4) |
| Proposed sawmill / central hall | (3,−5) / (0,−3) |
| Trees, 8 logs each | (−9,4), (−9,7), (−5,−8), (−1,−7), (4,−7), (11,5), (13,4), (15,0) |
| Berry bushes | (−9,0), (−8,9) |

The camps, sawmill and central hall can all be planned simultaneously using authoritative placement checks. Resource accesses, starter homes and residents are connected. The same central land is also attractive for additional food or housing; those optional placements can lengthen recreation trips. Exhausted outcrops retain their footprint and working access under current rules: **do not promise that mining creates a buildable plot.**

## Exact starting budget and competing routes

Eight residents, eight beds, 16 yard logs, zero planks/stone, 72 stored berries. Two bushes provide ongoing foraging; the ready vegetable garden begins unplanted and needs its farmer. Roles: one logger, two builders, two foragers, one farmer, two unassigned. Suggested first new assignments are quarrier and sawyer. Workplace capacity/production still determines when those jobs can start.

Starting structures contain 32 logs: four cottages ×6, forager 4, garden 4. Trees contain 64 logs. Together with 16 yard logs, initial timber accounting is **112 logs**. Deposits contain **44 stone**, of which the hall requires 12. No starter planks or unaccounted finished hall materials.

| New investment | Nearby then distant | Distant only |
| --- | --- | --- |
| Quarry camps | 12 logs for two | 6 logs for one |
| Sawmill | 6 logs | 6 logs |
| Hall timber | 4 logs processed into 8 planks | Same |
| Total minimum new log consumption | 22 | 16 |
| Minimum additional logging beyond yard reserve | 6 | 0 |
| Minimum stone for hall | 8 nearby +4 distant | 12 distant; nearby untouched |
| Tradeoff | More construction and timber, fewer long stone journeys | Less infrastructure, more long stone journeys |

These are minimum material budgets, not predictions of automatic production. Sawyers and quarriers may produce beyond the hall's immediate demand unless the player changes targets or staffing. Stone travels to the central yard; moving a camp does not establish local stone storage. Quarry eligibility uses a four-tile radius from its center; extraction occurs at the outcrop access. Camps do not select a player-locked deposit.

Optional garden 4 logs, pantry 6 logs, stockpile 4 logs, cottage 6 logs, and dirt paths offer existing responses. None is mandatory. Paths improve long travel without material cost, so acceptance runs must compare paths on both routes before claiming a durable advantage for either. Replanting and material recovery prevent ordinary overbuilding from becoming a hard resource lock.

## Two campaign stages

**1. Build a shared place.** Show the hall's delivered planks/stone and construction status, plus both outcrop balances and source links. Opening copy: “The nearby outcrop holds 8 stone. A gathering hall needs 12. Use both outcrops for shorter hauling, or start at the distant one to save a camp.” Offer normal planning links, not mandatory construction order. Count materials already incorporated in the hall; do not require the player to stock them again. A completed available hall enables **Assess the gathering place**.

**2. Keep it working.** Start an assessment on that explicit action. Proposed conditions: every resident housed; at least half have their latest completed recreation visit at an available gathering hall within four in-game minutes; reliable closed meal requests and fresh deliveries covering eating/demand using existing `ReadMealAssessment` evidence. Show numerator, denominator, window and actionable blocker. Use `Reliable` and `FreshSupply`; mixed diet is not another compulsory lesson here. Apply these together, rather than latching visits once and ignoring a failing village later. Initial population is eight; optional newcomers increase the denominators.

No fixed minimum duration and no reset of earned construction. Service evidence starts at assessment and recovers through the existing rolling window. A missed meal delays completion but does not erase the hall or force a level restart. Explain that residents visit recreation automatically between work, and that a closer square/garden may receive their latest visit instead. Hall-specific visits must be named as such; generic “recent recreation” is insufficient.

Thresholds remain provisional until both full routes and recovery pass. Do not copy lake's two-minute venue wording: halls have four-minute memory. Completion must demonstrate current hall use and continuing food service, not building ownership alone.

## Current evidence and implementation gates

`dotnet run --project Tests/SimulationTests.csproj -- --quarry-brief` verifies starting accounting, legal simultaneous plots, map occupancy, exact saves and two real construction/visit routes. Both leave the initial production staff working, assign the two spare residents and advance 0.1s simulation ticks.

| Diagnostic route | Hall complete + four recent visitors | Remaining near / far stone | Yard logs |
| --- | --- | --- | --- |
| Two camps | 307 simulation seconds | 0 / 30 | 18 |
| Distant only | 383 simulation seconds | 8 / 22 | 32 |

Actual extraction exceeds the minimum because production continues. These are early route measurements, not completed campaign runs, player wall times, proof of enjoyment or a target duration. They currently take only a few simulated minutes; do not sell this as the requested longer skill level on that evidence alone.

F26b2b must deliver:

1. Campaign/menu/replay integration, staged actions, save validation and condition/help/source navigation at 960/1440. Correct existing quarry messages that only mention Three clearings.
2. Full two-camp and distant-only completion runs, with real food service and actual hall visits. Include paths in the comparison; record decisions, deficits, deliveries and simulated milestones.
3. A deliberately poor distant hall or diverted food-labor route. First prove the actual service failure, then recover through staffing, paths, food placement or rebuilding; save/reload while recovering. Materials recovered by demolition remain accounted.
4. Clear exhausted-deposit feedback and a check that a completed hall never asks for its already-used construction materials again. All building types remain available.
5. Rendered village/visitor review and a pacing reassessment. If the route is just queueing four buildings and waiting, improve the existing map/labor choice or rescope the scenario before calling it finished. Do not extend it with higher quotas or arbitrary waiting.

## Design review

The material/transport tradeoff is real and the nearby source is optional; two routes have now built and used the hall. The dry neck distinguishes the map from another bridge tutorial. Recovery can reuse existing demolition, staffing and service tools. Scope is one authored scenario using current systems.

Unproven: whether paths make one route dominant, whether the generous working starter village makes labor allocation trivial, whether players discover a poor hall placement's cause, and whether the civic payoff feels worthwhile. Full assessment and adverse-route evidence come next. Human first-play timing remains an explicit open question, not an implementation blocker or an automated acceptance claim.
