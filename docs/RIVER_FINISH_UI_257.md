# River-closing finish clarity — outcome 257

The owner reached a displayed Self-sufficient 4/4 without the checkmark. The old decimal timer both rounded up before four minutes and presented only one of two final conditions. This fix changes presentation, not the completion rules.

The final goal now shows the stability timer as floored minutes/seconds and a separate food-delivered / meals-eaten comparison for the current trial. Completing the timer checks only its own line; a production deficit remains visibly unchecked with an exact remaining portion count in the guidance. Once the river closes, fed residents, minimum reserve and residents who have eaten fresh food are visible together, alongside the existing housing count. The unrelated rolling180s food total is removed from that stage so it cannot be mistaken for the winning requirement.

A reset hides stale trial totals until the new timer starts. Completed objectives show the passed trial rather than continuing to change historical completion counters as the village eats. Existing completion, save and food rules are unchanged.

## Verification

The focused longer-level comparison adds states for239.99s (must show3:59),240s with10delivered/16eaten (timer checked, food unchecked,6missing), reset/fresh-meal blocker, and completed trial. Reading the objective must not mutate the world. Native compact captures and independent UX review follow below.

These checks passed, along with the unchanged longer-scenario outcomes: staged success at1,727s, early success at889s, and the opening-only control still incomplete at1,565s. Native compact captures show the timer checked independently from the six-portion deficit, reset fresh meals0/12, and latched success. Controls fit without overlap. The UX and visual/audio stack reviewers also inspected the finish panel; this is still/source evidence, not a new ordinary playthrough.

Final native build DLL SHA256: `D7A41E3975D8AEB248C0CD9FB973469DACE30834495D54CBB7D13997F4E1F29C`. Captures under `artifacts/review/runs/`, each `capture-0001/view.png`:

- Deficit: `20261110-013308-710-crossing-recovery-ae7042`.
- Reset: `20261110-013327-293-crossing-recovery-ac5b3b`.
- Success: `20261110-013355-002-crossing-recovery-c11456`.

This addresses observed owner confusion. No campaign extension or new gameplay system is selected; return to the active playtest after delivery.
