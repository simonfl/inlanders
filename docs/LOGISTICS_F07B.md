# Neighborhood logistics — F07b

## F07b1: see the trips, implemented

Economy → Show supply routes displays actual remaining worker and boat routes. Gold means carrying goods; blue means traveling empty to work or collect. Arrowheads indicate the destination. Each entry shows a resident, destination, load and remaining tile distance; selecting it opens that resident's inspector. Stationary work and rest/recreation trips are excluded.

These are assigned trips, not proposed links between every building. Distances are geometric remaining route lengths, not delivery-time or full-cycle estimates. Terrain, paths and work time still affect throughput. The view refreshes four times per real second, reuses controls and one mesh, and retains unchanged geometry while paused. It hides in Watch mode and resets off when changing worlds. No simulation behavior or save format changed.

The accompanying explanation makes current rules explicit:

- Loggers deposit at nearby log stores without a hauler. Builders and sawyers collect locally. Haulers move existing logs toward targets and return surplus.
- Food and planks return to central storage. Crops, workshop buffers, boats and carried goods are not extra local stores.
- The forager hut provides worker capacity. Berries travel from bushes to the central pantry, not through the hut. Placing a hut beside a bush currently does not shorten that delivery.

Focused simulation checks exercise logger, sawyer, forager and hauler trips, detached read-only snapshots, and outbound/loaded-return boat routes. `./Play.ps1 -RoutesSmokeTest` checks rendered paths and links at 960/1440, pause, Watch and settlement switches. Captures are `artifacts/f07b1-routes-*.png`. The narrow screenshot was inspected; the route toggle scrolls its legend and trip list into view. This is a management aid, not a performance or aesthetic acceptance claim.

## F07b2: useful local storage, next

F07b is still open. Route visibility explains the current economy; it does not yet make neighborhood food or plank storage work.

Compare two candidate changes before choosing the implementation:

1. **Local planks:** let a stockpile accept a bounded plank inventory so sawyers can deposit near building projects and builders can collect there. Compare a mill/construction cluster near timber but far from the yard against the same arrangement with central delivery. Include depot cost, space, filling/draining, reserved capacity and all extra hauling labor. Test an awkwardly placed depot too; storage should not be an automatic upgrade.
2. **Food collection:** compare a neighborhood pantry or a real hut collection point against today's direct central deliveries. Decide who collects and who eats where before adding storage. A nearer drop followed by an equally long mandatory haul can simply move work onto another resident. Count that labor and ensure capacity does not strand food while the village goes hungry. Do not silently make remote stores available to central meals without deciding how that abstraction affects the promised logistics choice.

Use equal worker budgets, geography, final useful output and simulation duration. Count setup and compare both startup and established operation. Record loaded/empty travel, output delivered, waiting, extra staffing and land used. Keep current home/rest routines active. Test a compact settlement and a split settlement; don't enlarge population just to manufacture a payoff.

The earlier [F24b experiments](BALANCE_REVIEW_F24B.md) established that local log storage could repay setup near sustained timber use, while a different depot location did not; extra hauler labor sometimes bought speed rather than efficiency. Those historical numbers predate later home routines and are motivation, not current food/plank measurements.

Choose and implement the option with an understandable benefit, with real inventory/reservations, live inspector controls, removal/recovery, current saves and visible deliveries. If neither candidate earns its cost, record the evidence and revise the design rather than adding an inert market. The remaining candidate stays explicit in the roadmap; do not close F07b merely because route checks pass.

## Roadmap review

F07b2 remains the next chunk. Keep F03b/F04b activity and F23a visual acceptance prominent; routes hide during watching to avoid turning the village into a permanent diagram. F26b stone/hall and later hunting remain campaign additions after this logistics work and river/lake feedback. Do not add carts, broad warehouse filters, permanent workplace assignments or a new transport profession in the route slice. Resource filtering/selected-route emphasis can follow if the view becomes crowded in larger villages.
