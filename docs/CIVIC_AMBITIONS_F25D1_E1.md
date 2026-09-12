# Optional civic ambitions — F25d1 / F25e1

September 12, 2026. **Go: prototype chapel and planted-court identities for the gathering hall. Revise: a separate learning prerequisite for restoration.** Neither becomes a mandatory need or a new campaign level. This is a design decision supported by existing-system budget probes, not a shipped education/reflection system.

## The two concepts

**Learning/restoration: restore an abandoned riverside workshop.** The initial idea was for adults to learn joinery and wheel repair at a workbench, then contribute to a visible repair: cleared foundations → repaired timber frame → an open reading/workshop room. The proposed commitment is 16 planks and 24 stone, plus a six-log teaching workshop. The finished room would serve existing recreation rather than multiply production.

That proposal currently fails its distinct-purpose test. If the finished room only supplies recreation, the separate instruction step adds a prerequisite and time without changing the decision made by ordinary builders. The larger invoice is affordable, but affordability does not justify an education mechanic. **Merge practical learning into possible restoration work presentation; do not implement an attendance gate, skill token or teaching building yet.** Keep a functional restoration candidate open until its finished use offers something more specific than another hall. A working river mill would need a separate production/environment design; it is not implied by this proposal.

**Reflection/identity: choose the character of a community place.** Let an existing or planned gathering hall use its ordinary hall architecture, a small chapel, or a planted court. Chapel and court use identical costs and recreation rules. The choice is how the village looks and feels, with convenient versus scenic placement retaining the existing access tradeoff. It does not measure belief, penalize residents for lacking religion, or require both religious and secular buildings.

The chapel uses a narrow pitched nave, an arched entrance and a modest bell gable. The court uses low masonry, an open timber shade and a clearly ornamental tree with planted edges. Both keep the actual entrance and visitor space readable. The existing hall remains available. This is a **civic identity prototype**, not a claim that changing architecture introduces strategic depth.

## Budget and land

The probe starts on fresh Built to last terrain with eight housed residents, an operating forager hut and vegetable garden, 16 loose logs, 64 harvestable logs, 72 berries and 44 finite stone. The baseline roles are one logger, two builders, two foragers, one farmer and two unassigned residents. No materials are injected into the probe.

| Choice | New construction invoice | Minimum raw logs including sawing | Land and staffing |
| --- | --- | --- | --- |
| Central hall/identity | Hall: 8 planks + 12 stone; new sawmill and remote quarry: 12 logs | 16 | Hall at (0,−3), six footprint tiles plus open frontage. Fill the two spare roles with sawyer/quarrier; retain all three food workers. |
| Scenic hall + local square | Same, plus 6-log square | 22 | Hall at (12,1), local square at (−3,0). Spend additional land/materials to keep convenient recreation while choosing a distant landmark. |
| Restoration envelope | Proposed project: 16 planks + 24 stone; workshop 6 logs; sawmill/quarry 12 logs | 26 | Tested with two real halls and a carpenter workshop: 30 footprint tiles including infrastructure. This is an invoice/hauling proxy, not a restoration footprint specification. |

The sawmill converts two logs into four planks; the table excludes unconsumed production buffers. Reusing infrastructure reduces the new invoice. Every ordinary building has twelve work-seconds of construction, but hauling, interruptions and services dominate elapsed time. The feasible routes exploit two spare residents: they do **not** prove that the proposal forces a hard labor choice.

## Measured existing-system probes

Run `./Test.ps1 -CivicBudget`. The checked-in [results](CIVIC_BUDGET_RESULTS.json) are generated from the current simulation. Times are simulated seconds from map creation, not human completion targets. A supported checkpoint requires all ordered buildings complete, four residents with a recent recreation visit, and reliable recent meals with fresh supply covering demand. It does not require visits to the scenic project specifically.

| Route | All construction finished | Supported checkpoint | Recent project visitors at checkpoint | Food result |
| --- | ---: | ---: | ---: | --- |
| Central | 367s | 383s | 4 | No observed misses |
| Scenic + local square | 425s | 425s | 0 | No observed misses; local recreation covers the village |
| Food diversion, then repair | 347s | 1,185s | 8 | 24 missed/skipped outcomes in the window before repair; clean recent window after repair |
| Restoration invoice proxy | 676s | 676s | 8 at the central proxy hall | No observed misses |

The scenic result is important: a finished landmark is not automatically an inhabited destination. Zero visitors at this checkpoint does not prove it will never be used, but it prevents claiming a participation benefit. Start the identity prototype with a central occupied venue; preserve inconvenient placement as a player choice, not a mandatory scenic-site condition.

The poor route deliberately takes the three food workers for quarrying, sawing and construction while leaving the spare residents unused. At 900s it saves/reloads, restores the food roles and staffs materials work with the spare residents. Recovery is demonstrated, but this is a preventable allocation error, not evidence for a new mastery campaign.

Every run validates conservation and current saves, including matching continuation after reload. A separate partial-build check diverts builders for sixty seconds, confirms progress stops, cancels delivered planks/stone into physical salvage, reloads, collects it and rebuilds on the same plot. It waits for the salvage collector to leave the footprint before placing again. These are existing cancellation/recovery rules. No learning, new identity or future ceremony is simulated by these checks.

## Player-facing sketches — proposed, not implemented

The existing hall inspector gains one compact identity choice, not a second objectives panel:

```text
GATHERING HALL · Built
Identity  [ Hall | Chapel | Planted court ]
8 recreation places · same service in every identity
Recent visitors: actual existing visitor list
```

During ordinary construction, show the real delivered materials and progress already tracked by the building. An identity can be chosen on its plan; changing it is cosmetic and free. No additional attendance bar appears. Existing demolition/cancellation controls remain authoritative.

The unapproved restoration UI would have shown “prepare workshop / repair frame / open reading room,” delivered materials, assigned workers and a pause/reassign option. Since that currently duplicates builder progress, **do not implement this extra panel or its workshop requirement**. A future restoration proposal must first identify its finished function and a consequential decision between stages.

Concept silhouettes (planar sketches, not in-game assets):

```text
CHAPEL                        PLANTED COURT
       bell gable                  ornamental canopy
          /\                           (   )
     ____/  \____                    ___| |___
    / pitched roof\                 | shade   |
   | masonry / timber |           low planted masonry
   |  arched entrance |          open entrance / visitor space
   +------------------+          +-----------------------+
```

## F25e2 implementation brief

Add saved `Hall / Chapel / Planted court` identity to gathering halls, retaining one building kind, 8-plank/12-stone cost, six-tile footprint, entrance and eight recreation slots. All identities keep twelve-second visits, a two-minute interval and four-minute recent-service memory. Plans and built halls accept free changes; dismantling retains its appearance and disallows edits. Default remains Hall; no save migration.

Build distinct code-native models through all construction stages and four orientations, retaining material batching and actual visitor space. Label the courtyard tree ornamental; it must not add timber or habitat. First scope the inspector, architecture and exact save/render behavior. Quiet poses or bell cues should be separate additions driven by actual visits, not permanent fictitious crowds or repeated ceremonies.

Verify identity changes cannot change costs, assignments, meal/recreation accounting, construction/demolition progress or routes. Use normal construction, occupied visits, cancel/demolish, exact reloads and 960/1440 inspector checks. Compare all three identities at the same camera/light in the central occupied fixture and one mixed neighborhood. Retain the ordinary hall model. Do not add level eleven, a spirituality meter, a research queue or a new food chain in this prototype.

## Roadmap review

F25d1/F25e1 design is delivered; F25e2 is next. F25d remains a revised functional-restoration concept, not a completed learning system. Player enjoyment, quiet presentation/audio and a worthwhile final restoration use remain open. The short budget routes do not justify another long campaign claim. Other UI, landscape, music and remaining roadmap work retain their scope.
