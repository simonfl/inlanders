# F26c2 — The living woods

September 12, 2026. **Original F26c2a design and simulation prototype. F26c2b is now implemented as level nine; see the [campaign review](WOODS_CAMPAIGN_REVIEW.md).** The design and measurements below describe the earlier standalone prototype. All measurements below are scripted simulation seconds, not player wall time or evidence of enjoyment.

## Player decision and payoff

An eight-person village has two established woods and wants to support twelve residents without exhausting its wildlife. Habitat trees are convenient timber. Preserve both woods and harvest farther away, or selectively cut nearby trees and invest in cultivation while retaining useful habitat. Support the enlarged village and leave both woods capable of recovering.

The consequence is visible in existing systems: mature trees determine wildlife capacity and regrowth, hunters share animal stock, and actual game travels to food storage and meals. Cutting too much removes habitat support; hunting too quickly drains stock without removing the trees. Those must have different explanations and recovery actions.

The payoff is a working village amid living woods, not a prescribed number of hunting lodges. No leather, butcher, weapons, extra need, climate, research tree or new material chain. All buildings and tools remain available.

## Authored layout and exact budget

Bounds x −12…17, z −11…12, with tapered corners: exclude cells when x+z>23, x−z>23, −x+z>20 or −x−z>19. Dry connected ground; no compulsory bridge. The ready village uses the same proven starter-house layout as Built to last, but the northern land is two wooded habitats rather than a remote stone clearing.

```text
North
  WEST WOOD                 EAST WOOD
  six trees                six trees
       hunting/cultivation choices
       homes — central yard — homes
           existing food
  outside timber           outside timber
South
```

| Element | Current prototype |
| --- | --- |
| Western / eastern tracking clearing | (−6,−5) / (9,−5), radius five |
| West trees | (−9,−5), (−8,−8), (−5,−8), (−3,−6), (−7,−3), (−4,−3) |
| East trees | (6,−6), (7,−8), (10,−8), (12,−6), (11,−3), (8,−3) |
| Timber outside both habitats | (−10,6), (12,6), (7,8) |
| Ready cottages | (−6,0), (−6,4), (−1,7), (3,7): eight beds |
| Ready forager / vegetable garden | (−6,7) / (2,0) |
| Berry bushes | (−9,0), (−8,9) |
| Suggested western / eastern lodge plots | (0,−3) / (5,−3), rotation zero |
| Suggested added cottage plots | (2,−6), (5,0), rotation zero |
| Optional second garden | (−3,0), rotation zero |

Fifteen trees ×8 logs =120 standing timber. Twelve habitat trees contain 96 logs; the three outside trees contain 24. The yard starts with 16 logs. Existing buildings contain 32 logs, so initial accounting is **168 logs**. No starting planks, stone or game. Pantry starts with **32 berries**; both habitats start with **12 animal stock**. Eight residents have one logger, two builders, two foragers, one farmer and two unassigned roles. Each habitat initially recovers **3 game/minute**, before travel and hunting labor.

| Route | New investment | Immediate implication |
| --- | --- | --- |
| Preserve / hunt | Two lodges 12 logs + two cottages 12 logs =24 logs | Protect all twelve habitat trees. At least eight logs beyond yard reserves come from outside timber. Assign both spare residents to hunting. |
| Mixed cultivation | One lodge 6 logs + one garden 4 logs + two cottages 12 logs =22 logs | Clear the two southern west-wood trees for 16 convenient logs, preserve the other ten. Assign one hunter and one additional farmer. West capacity becomes eight, recovery two/minute; east remains twelve and three/minute. |

These are minimum investments; the logger may collect more than immediately required. Both routes keep interim food and can invite two newcomer pairs through ordinary food/bed rules. New arrivals remain available for later staffing. No scenario-specific free delivery, replacement trees or instant growth.

## Proposed campaign stages

1. **Understand the woods.** Deliver four game to food storage. Explain shared stock, mature-tree support and the difference between preservation and managed timber groves. Source links inspect both woods; clearing previews show the capacity/regrowth consequence before committing. Existing agriculture remains usable from the start.
2. **Choose the enlarged village.** Prepare homes and food for twelve; all residents must have homes. Proposed ecological floor: both habitats retain at least four mature trees (capacity eight, regrowth two/minute). This allows selective harvesting and cultivation; it does not demand twelve untouched trees. Protect restoration plantings if those trees are meant to remain habitat. Begin assessment explicitly when ready.
3. **Leave a supported woodland village.** Apply actual closed-request/fresh-delivery food evidence together with the ecological floor and recoverable animal stock. Provisional stock floor: two unclaimed game per habitat. No required final lodge count or compulsory diet share; a cultivation route may rest its hunting grounds. Keep habitat and service conditions current, not permanently credited after one earlier snapshot.

The stock floor and full phase design still require integrated route evidence. Do not add a long timer merely because the prototype is quick. Construction/catch progress stays earned; a temporary shortage or tree loss delays the ecological/service assessment without resetting the village. Optional extra residents increase housing/food demand normally.

## Prototype evidence

`Test.ps1 -WoodsBrief` validates map occupancy, budget, legal construction, actual game eaten, expansion, food service, exact saves and both recovery mechanisms.

| Diagnostic | Result |
| --- | --- |
| Broad preservation with two hunters | Twelve housed, at least four game eaten, reliable closed meals and fresh supply at 286s. Mature trees 6/6; stock 7.3/9.3. |
| Mixed cultivation / one hunter | Same village/meal checks at 253s. Mature trees 4/6; stock 3.4/12.0. |
| Hunting pressure, trees intact | West stock falls to 1.7 with all six trees retained. Pause hunting; after saved continuation, stock recovers to eight with reliable fresh food by 620s. No planting required. |
| Excessive west clearing | West capacity and regrowth both reach zero at 473s. Collect timber, remove roots, plant and preserve four replacement trees. A saved continuation restores mature habitat plus reliable fresh meals at 884s. |

An initial restoration attempt replanted but did not preserve the replacements; the active logger harvested them after maturity, so habitat recovery did not hold. The successful route protects the planting orders using the existing tool. This is a player-facing lesson to explain, not a new growth rule.

The competent setups take only four to five simulated minutes. These are **not completed campaign runs** and do not settle the request for longer, more demanding play. They establish that the material choice and distinct recovery mechanisms function. The prototype checks food history from startup; integrated assessment must instead use its actual start time and pass separate fresh evidence after expansion.

## F26c2b acceptance and design review

- Integrate picker, concise arrival guidance, staged actions, condition/source/clearing links, replay and exact phase saves. Preserve availability of every building/tool.
- Run both full routes with the actual assessment start, plus optional growth and a poor clearing choice. Verify food remains workable while replacements grow. Compare a pause-for-stock recovery with a replant-for-capacity recovery; never advise only waiting when no mature trees remain.
- Test absent stock, existing hunter reservations, partial harvest and tree growth around ecological thresholds. Count available stock after claims, not the same animals twice.
- Render the two woods, clearing preview, wildlife and recovery at normal village zoom and 960/1440. Feedback must distinguish animal stock, habitat capacity and regeneration.
- Reassess decisions after the opening: if all ecological preparation is still a one-time checkbox before waiting, revise the authored commitments rather than declare duration solved. Do not inflate quotas or add mandatory buildings to meet a nominal number of minutes.

The current design has a supported geographical resource choice and genuinely different failure causes. Its challenge across a whole level remains unproven. The later chapter finale, dock/bridge presentation and other roadmap work remain separate; this brief does not shrink the overall roadmap to this scenario.
