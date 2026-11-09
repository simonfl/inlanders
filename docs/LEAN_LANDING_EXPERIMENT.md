# A lean landing — return play with food pressure

November 9, 2026. The owner reported that the clearer objectives made progression understandable and moving forward felt good. They also liked dwindling provisions and asked for some danger beneath the cozy presentation. This is positive human evidence for clear progress and anticipatory pressure, not proof of campaign depth.

## Bounded direction

Keep the three-part objective structure. Clean its visual hierarchy and try one different opening before adding systems. A first home remains available; A lean landing has its own save under Household stories.

The new opening has four people, twelve food portions instead of twenty, and ten stored logs. The host cottage sits below a new river inlet. Berry bushes lie inland; fishing water is near the landing. Guidance prioritizes fresh food, then housing and household meals. The player can use the broader Build menu and choose another order. A cottage costs six logs, gathering/gardening four, and a landing eight, so initial timber can fund a cottage and a cheap food source or most of the fishing route before further forestry.

Pressure uses existing meal consumption and hunger slowdown. There is no new death rule, arbitrary countdown, winter, compulsory need, or inherited-property system. Shortages are visible and recoverable. Fresh food clears hunger through actual meals.

## Presentation

Shared objective cards separate title, completion count, three objectives, food reserve and next action. Completed goals are green. Low reserves and hunger use distinct warm colors with quantities and consequences. The lean opening lists food before housing. Secondary tools remain collapsed. Food choices show costs up front with purpose tooltips; expanded choices suppress duplicate helper text and tools so the three options fit the compact drawer.

## Evidence

Scripted simulation, not player discovery:

| Route | First food | Recovered completion | Peak hungry |
| --- | ---: | ---: | ---: |
| Prompt fishing | 52s | 177s | 0/4 |
| Prompt gathering | 42s | 234s | 0/4 |
| Prompt gardening | 89s | 227s | 0/4 |
| Wait 300s, then fishing | 400s | 480s | 4/4 |

Times are simulated seconds; completion output includes five seconds of exact save-continuation checking. Timely routes deliberately build food before the cottage. The delayed branch verifies empty reachable stores, hunger slowdown, actual food recovery and completion. Both original first-home routes still pass. Distinct save/profile creation and exact continuation pass for all new routes.

Independent evaluator inspected native 960px opening, shortage, completion and original first-home captures, then matched source behavior. It accepted the clearer hierarchy and identified two corrected details: empty stores require recovery wording, and timber-choice costs need to be visible. Prompt routes prove feasibility, not a meaningful strategic tradeoff: they finish in roughly three to four simulated minutes without hunger. Owner play must tell us whether dwindling food changes priorities or becomes background once a food building exists.

Desktop input tools previously hung retrieving Godot's window state. They were not retried. Stills/source and scripted button activation must not be described as uncoached play, motion inspection or listening.

## Small tooling investment

The optional `Review.ps1 Capture -HouseholdFoodChoices` path opens the food chooser through the ordinary action button's signal, asserts that three choices appear, and captures it using the existing review harness. Beneficiary: UI layout review when desktop input is unavailable. Expected saving: avoid repeated multi-minute window-state hangs and manually prepared captures. Cost/maintenance: one narrow request flag and scoped household assertion; no replay framework. Validation is the compact native food-choice capture. It does not replace human input testing.

## Stop gate

Ask the owner to try A lean landing without a placement recipe. Does the reserve create useful urgency? Does choosing and completing a plan still feel good? Is the pressure fair and recoverable, or merely waiting/an annoyance? Hold further expansion until that reaction. A second situation is a test of the retained structure, not an automatic campaign roadmap.

Final handoff: independent reviewer accepts for owner return play, with no remaining concrete visual blocker in inspected states. Final build SHA256 D2585192B05A58014DD1F0A21F9C96E500138832CB3A24EB5606958F70993F37. Zero-warning build. Final native chooser capture through the action signal: artifacts/review/runs/20261109-141700-942-river-b9c59c; corrected shortage capture: artifacts/review/runs/20261109-141714-205-river-ef294b. All three costed options fit with objectives still visible. Earlier opening/completion/baseline captures: 20261109-141112-376-river-20358f, 20261109-141159-573-river-9582c1, 20261109-141213-084-river-a1d373. These are controlled captures, not independent ordinary play.