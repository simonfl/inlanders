# F27a — a place across the river

In progress. This is the complete gameplay experiment from the [strategic reset](STRATEGIC_REVIEW_8.md), not a new sequence of small campaign patches. It counts as one playable outcome only after the full start → commitment → shortage/recovery → inhabited payoff works through the UI.

## Design decision

A bounded independent game-designer review recommended making expansion a commitment and the welcome an actual activity. Existing `BeginSupper` is unsuitable unchanged: it deducts central bread immediately and globally redirects the village. Existing Automatic workplace behavior is also insufficient: it chooses a workplace within an assigned profession, while Unassigned residents do no work. Renaming either system would preserve the rejected design.

Start from the eight-person west hamlet and author two credible eastern sites: a compact landing with short access to existing supplies and a larger meadow farther away. All buildings remain available. Do not mark compulsory building plots or require a pantry, bakery or particular recreation identity.

One action welcomes four neighbors after a visible journey, initially 90 simulated seconds. Require a reachable eastern arrival destination, not a food-reserve or service gate. Show the commitment clearly before activation. The residents arrive even if preparation is incomplete; homelessness and food shortage need recovery rather than restarting a certification window. Four arrivals and 90 seconds are experiment values, not established pacing.

The objective is housing the arrivals and sharing a welcome meal at an eastern recreation venue chosen by the player. Use any edible foods. Helpers physically transport portions to a visible display; attendees walk over and eat. Normal routines continue during preparation. Delivered supplies persist until used or physically recovered if the venue is removed. A food shortage delays the welcome rather than resetting progress. Completion must not require every villager to occupy a seat simultaneously; attendance can accumulate through the actual activity. Explain a blocked participant/route locally.

Test two plausible strategies: landing homes with directly edible production, and a meadow grain/bakery chain. Their differences must follow from space, construction, labor and journeys, not regional bonuses. Exercise an early arrival commitment with inadequate food capacity, then recover through direct food/labor changes and separately through improved production location. No injected famine or hidden winning recipe.

Shared residents must actually pick work across professions in this experiment. Building controls offer visible worker slots, a shared default, optional dedicated workers and a concise current obstacle. Releasing a dedicated resident restores shared work. Food/construction starvation and the risk that automation removes every labor decision both need testing. Preserve the current baseline for comparison; no automatic campaign-wide rule rollout.

## First implementation — local grain foundation

The local-grain comparison factory now allows a farm to retain up to twelve physically harvested/delivered grain. Bakers reserve two at a reachable source selected by the combined worker → source → oven journey, with central storage still available. Full farm stores send additional harvested grain centrally. Stock, incoming baskets, reservations and oven input are separate conserved quantities. Grain remains inedible.

This is deliberately enabled only by `World.NewLocalSupplyExperiment()` for development comparisons. The ordinary river campaign retains its central route. It is **not the complete neighborhood scenario**, and no new campaign/menu entry presents it as such yet.

Interrupting a farmer returns carried grain physically; interrupting a baker releases the claim. Farm demolition cancels incoming/pickup claims and builders recover stored grain through real trips. Cancellation and original-world versus one-reload continuation are checked. Existing production diagnostics identify the actual farm source. Current save version is 39; no migration is provided.

Focused command:

```powershell
$env:DOTNET_ROOT = Join-Path $PWD '.tools/dotnet'
$env:DOTNET_CLI_HOME = Join-Path $PWD '.tools/dotnet-home'
& .tools/dotnet/dotnet.exe run --project Tests/SimulationTests.csproj -- --local-grain
```

`Tests/LocalGrainChecks.cs` verifies actual farm delivery and baking while central grain is zero, competing bakers, source feedback, interruptions, capacity overflow, demolition/cancellation, malformed-save rejection and exact continuation from active deliveries/pickups. It also runs the baseline central chain. Generated pickup/baked states are in `artifacts/local-grain`. These checks establish mechanics, not enjoyable layouts or pacing.

Verification: the existing full `Test.ps1` simulation suite passed. After the final local inventory/route-label adjustments, the focused command passed local-grain, existing economy and existing supply-route checks. The final game build passed with zero warnings/errors. No rendered or native playtest is claimed for this foundation; full player-facing integration remains below.

## Shared staffing foundation

`World.NewSharedWorkExperiment()` enables a real shared pool alongside local grain. Residents claim existing profession jobs, retain that profession during work and delivery, then return to the pool. Meals and breaks continue normally. Assigning a profession dedicates the resident; assigning Unassigned releases them back to shared work in this experiment. Named workplace assignments retain their exclusive slots. Ordinary campaign staffing remains unchanged.

The initial policy prioritizes food below three portions per resident, limits concurrent food roles to roughly half the population and each profession to roughly a third, then tries construction and supporting production. Shared logging keeps a modest reserve rather than harvesting indefinitely. These are provisional scheduling choices, not validated balance. The complete scenario must still demonstrate meaningful staffing choices and recoverable shortages; automatic completion of this test does not establish that.

`--shared-work` runs the new staffing checks plus existing local-grain and workplace-assignment checks. Verified actual bridge/workplace construction, logging, local bread production, residents changing professions, dedication/release and exact original-versus-reload continuation. The game builds without warnings, and the full existing `Test.ps1` simulation suite passed. Arrivals and welcome activity are still outstanding; this foundation does not advance the playable checkpoint.

## Remaining work in the same F27a outcome

- Integrate separate neighborhood progress, visible arrival commitment and actual newcomer arrival/home assignment.
- Integrate shared staffing into newcomer creation; test shortage/recovery and labor agency in the complete scenario.
- Implement physical welcome-food delivery, reservations, display, visits/eating and recoverable venue removal, with ordinary needs continuing.
- Expose the alternative beside the baseline and register useful states with T01. Replace assessment-oriented default information with one objective and world/context feedback, including precise placement refusals.
- Demonstrate two real strategies and two recoveries, validate changed saves/claims and use ordinary UI play. Record decision opportunities, panel dependence and time spent waiting, separating tool overhead from pacing.

F27b remains the separate whole-scene art experiment, but F27a still needs enough visible activity and contextual feedback to test its own gameplay honestly. Do not ship a simulation-only comparison as the promised playable alternative. The next periodic whole-project review remains checkpoint ten.

## Building staffing controls

The experiment's workplace inspector now shows dedicated slots, current workers and the village-wide shared pool. **Dedicate worker** takes a shared resident (preferring someone already working here), preserves physical cargo recovery, and reserves a slot at this building. **Release to shared** releases a resident dedicated here. Full slots and an exhausted shared pool disable dedication with a reason; this never takes another building's dedicated resident. Storage buildings retain visiting shared haulers and their supply-target controls.

The developer fixture is available with `./Review.ps1 Inspect shared-work`. It is explicitly the staffing foundation, still using the old river objectives; it is not the completed neighborhood scenario. `./Review.ps1 Capture shared-work -Width 960 -ProbeControls` exercises the normal rendered buttons and captures dedicated/released states. The equivalent 1440-wide command checks the larger window. This reuses T01 with one catalog entry and a small targeted probe; no new testing framework was needed.

Focused staffing/local-grain/assignment tests passed, including capacity refusal without mutation and release through the building API. Rendered scripted checks cover dedication, changing slot/pool counts, release and disabled release at zero assignments. The 960-wide capture was visually inspected: both buttons and staffing explanation fit. These are scripted UI checks, not a native playtest or evidence of fun. The next implementation is committed arrivals and the physical welcome activity; F27a remains one unfinished playable outcome.