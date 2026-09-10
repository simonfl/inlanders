# Neighborhood logistics — F07b

## F07b1: see the trips, implemented

Economy → Show supply routes displays actual remaining worker and boat routes. Gold means carrying goods; blue means traveling empty to work or collect. Arrowheads indicate the destination. Each entry shows a resident, destination, load and remaining tile distance; selecting it opens that resident's inspector. Stationary work and rest/recreation trips are excluded.

These are assigned trips, not proposed links between every building. Distances are geometric remaining route lengths, not delivery-time or full-cycle estimates. Terrain, paths and work time still affect throughput. The view refreshes four times per real second, reuses controls and one mesh, and retains unchanged geometry while paused. It hides in Watch mode and resets off when changing worlds. No simulation behavior or save format changed.

The accompanying explanation makes current rules explicit:

- Loggers deposit at nearby log stores without a hauler. Builders and sawyers collect locally. Haulers move existing logs toward targets and return surplus.
- Food returns to central storage. F07b2 adds local plank stores below; crops, workshop buffers, boats and carried goods are not extra stores.
- The forager hut provides worker capacity. Berries travel from bushes to the central pantry, not through the hut. Placing a hut beside a bush currently does not shorten that delivery.

Focused simulation checks exercise logger, sawyer, forager and hauler trips, detached read-only snapshots, and outbound/loaded-return boat routes. `./Play.ps1 -RoutesSmokeTest` checks rendered paths and links at 960/1440, pause, Watch and settlement switches. Captures are `artifacts/f07b1-routes-*.png`. The narrow screenshot was inspected; the route toggle scrolls its legend and trip list into view. This is a management aid, not a performance or aesthetic acceptance claim.

## F07b2: local plank storage, implemented

Each stockpile now holds either 12 logs or 12 planks. Select the material on its construction plan or while it is empty with no committed trips. Existing piles default to logs. Producers choose nearby matching storage with room; builders choose nearby matching supplies. Optional haulers refill/drain targets with two-unit loads. Different materials never share a capacity or reservation. Normal demolition evacuates goods; Creative removal returns them to central storage. Local plank meshes, counts, supply routes and format-28 saves follow real inventory. No migration.

### Candidate comparison and decision

**Food relay:** the experiment observes a current two-berry pantry delivery. Its best-case round trip is 10 worker-tiles; splitting the same path at an ideal midpoint into two returning two-unit carriers still costs 10 worker-tiles, before handling or construction. This is a geometric lower-bound comparison, not a simulated food-depot economy or scheduling proof. A larger haul load or local meal destination could change the result. Keeping current load sizes and central meals does not justify adding a compulsory transfer merely to make a hut collect food. Neighborhood food service remains F07c below.

**Planks:** run `--local-storage` in the simulation test project. All six variants use campaign 3's eight residents and starting homes/hut, two loggers, two builders, one sawyer, two foragers and one spare. Food starts at 1,000 berries to isolate construction; meals and home routines stay active. This is not a food-sustainability test. Build three lodges in successive waves. Include the mill, optional four-log depot, all clearing and trips from time zero. Stop harvesting after construction needs plus twelve spare logs, and stop new milling after eighteen logs become exactly thirty-six planks. Every run ends with three finished lodges, twelve stored logs and no spare planks. No hauler or new resident is added.

| Layout | Storage | Finish (simulation seconds) | Travel (person-seconds) | All active work/routines (person-seconds) |
| --- | --- | ---: | ---: | ---: |
| Compact | Central only | 257.2 | 512.6 | 757.7 |
| Compact | Nearby candidate pile | 258.4 | 577.1 | 840.0 |
| Compact | Alternate pile | 257.0 | 555.1 | 816.6 |
| Remote mill/lodges | Central only | 494.3 | 957.0 | 1,253.3 |
| Remote mill/lodges | Nearby plank pile | 460.9 | 758.6 | 1,065.4 |
| Remote mill/lodges | Awkward plank pile | 520.0 | 1,013.4 | 1,326.3 |

Travel includes empty and loaded walking by the five construction/production residents, including their return trips after reassignment. Active time includes their non-waiting work/rest routines. Each fixture runs until equal useful output, not equal elapsed time; finish time is the result. Residents take 9–16 total rest visits depending on duration and layout. The remote pile saves 33.4 elapsed seconds and 198.4 travel person-seconds including setup. The compact piles fail to repay their extra labor; the awkward remote pile makes things worse. This is a useful location choice, not a universal upgrade.

The exact coordinates and checks live in `Tests/LocalStorageExperiments.cs`. Compact mill is at (-4,2); candidate/alternate piles at (3,-3)/(0,6), lodges at (3,6), (6,6), (-5,6). Remote mill is at (9,0), candidate/awkward piles at (9,3)/(0,6), lodges at (12,0), (12,3), (12,6). Pile plots are unused in their central-only counterparts. These are first-version balance fixtures, not validated human pacing targets or a broad performance benchmark.

The earlier [F24b experiments](BALANCE_REVIEW_F24B.md) established that local log storage could repay setup near sustained timber use, while a different depot location did not; extra hauler labor sometimes bought speed rather than efficiency. Those historical numbers predate later home routines and are motivation, not current food/plank measurements.

Full simulation checks pass, including local plank deposits/capacity, builder claims/cancellation, competing haulers, exact saved delivery/hauling continuations, drain/material switching, and normal/Creative removal. Demolition during a committed plank delivery releases its destination safely. `./Play.ps1 -PlankStorageSmokeTest` exercises planned material selection, visible plank stacks and inventory, occupied switching guards, saves and drain/switch at 960/1440. The narrow rendered view was inspected. Historical log-storage checks still pass after sharing the same storage rules with planks.

## F07c: neighborhood food service, later

Food collection remains unimplemented. Start from a useful resident destination or a clearly justified bulk-transfer design, then compare total labor and service coverage against direct pantry trips with equal workers. Decide who eats where, preserve actual food access and explain full storage before authoring a level around it. The forager hut remains a capacity building for now. Do not treat village-wide inventory as proof that food physically reached a local meal. A market should earn its footprint through a new choice; this candidate is deferred rather than silently included in F07b's completed scope.

## Roadmap review

F07b's useful first version is complete: routes explain trips, and local planks have a measured purpose. F03b/F04b activity is next; F23a visual acceptance remains open. F26b stone/hall and later hunting remain campaign additions after river/lake feedback. F07c is a later service design, not a prerequisite for these visual/activity improvements. Carts, broad warehouse filters and permanent workplace assignments remain separate possibilities. Resource filtering/selected-route emphasis can follow if the view becomes crowded in larger villages.
