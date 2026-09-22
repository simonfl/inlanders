# Across the inlet — decision evidence, checkpoint76

Fixed experiment: `718865e`. These are **developer-authored intentions and scripted branches**, not a player's account. No human playtest happened. Current building inventory matches the cultivated bank: six homes, two fields, one garden; twelve residents,72 berries,12 logs,4 planks,36 productive tiles/48 vegetables per full crop. The compact comparison has lower productive capacity and is not an equal-capacity control.

## Intention → action → consequence

| Branch | Authored intention | Actual actions | Observed consequence / tradeoff |
| --- | --- | --- | --- |
| Keep | Keep the existing home bank and avoid spending or disrupting crops. | Let ordinary life run. | Retains clustered homes and the western detour. Viable in the sampled ten minutes; no change is required to finish. |
| Cross | Keep the cluster but connect it more directly to the northern crops. | Place a bridge at(5,0), let workers deliver six logs/build, connect an actual path from the yard to the eastern field after completion. | The bridge and path remain visible; Normal retains6 starting logs. This does not grow more food. |
| Reshape | Put part of the cultivation beside the homes while moving a household beside the remaining northern field. | Move the home at(5,7) to(1,-2), pause the eastern field, wait for residents to clear its new ground, move it to(5,9), resume and connect approaches. | Five homes and a field share the southern bank; one home joins the north. No timber spent; growing crops need fresh sowing. A field initially proposed at(5,8) correctly failed because it covered the kitchen garden's entrance. |

## Simulation observations

Each branch runs600 simulated seconds after its setup, followed by100 matching continuation ticks on both the original and loaded world. Reshape setup includes a short ordinary wait for occupied ground to clear; these are not equal-cost or perfectly time-matched causal trials. The crossing's construction occurs during the observed interval. Figures are rounded movement totals and final snapshot inventory, not production efficiency.

| Mode / branch | Hungry sample ticks | Final edible food | Stored logs | Total resident movement, tiles |
| --- | ---: | ---: | ---: | ---: |
| Normal keep | 0 | 48 | 12 | 4582 |
| Normal cross | 0 | 48 | 6 | 4145 |
| Normal reshape | 0 | 46 | 12 | 3680 |
| Relaxed keep | 0 | 48 | 12 | 4582 |
| Relaxed cross | 0 | 49 | 12 | 4154 |
| Relaxed reshape | 0 | 46 | 12 | 3680 |

`walked` adds every person's positional movement, not travel for matched completed work. Less movement can mean different work or waiting. The totals do not prove efficiency, relieved inconvenience, branch dominance, pacing or enjoyment. Exact save continuation, profile identity, actual harvest and finished crossing are checked; leave-it viability is asserted for this sample. No universal balance guarantee.

## Matched scene access

Paths below are local ignored artifacts, generated through the existing review launcher; committed code can recreate them.1440 branch captures use the same camera framing. Crop phases differ and must not be treated as a controlled visual variable.

- [Keep](../artifacts/review/runs/20260922-001421-223-across-inlet-55a42b/capture-0001/view.png)
- [Cross](../artifacts/review/runs/20260922-001356-174-across-inlet-3594bb/capture-0001/view.png)
- [Reshape](../artifacts/review/runs/20260922-001538-892-across-inlet-c26599/capture-0001/view.png)
- [Reshape recording / extracted frames and audio](../artifacts/review/runs/20260922-001538-892-across-inlet-c26599/movie-index.md):1×,20.5s extracted audio/492 frames. Availability is not listening or motion acceptance; encoding time is not native performance.
- [960 scripted controls](../artifacts/review/runs/20260922-001118-288-across-inlet-06e9f8/index.md): real UI entry, furnishing commitment, saves and modes. Field pause/move use simulation APIs; this does not test discovering/executing the strategic response through ordinary controls.
- [Opposite1440 opening](../artifacts/review/runs/20260922-001040-924-across-inlet-a0901e/capture-0001/view.png)

Recreate with `./Review.ps1 Build`, then the test executable's `--inlet-choice` switch. Capture `across-inlet` with `-Snapshot artifacts/inlet-choice/Normal-keep.json` (or `Normal-cross.json` / `Normal-reshape.json`). The test writes starting and lived snapshots for both modes.

This small annotation uses existing captures and snapshots; it is not a new replay/report framework. It should let a reviewer reconstruct one proposed decision within two minutes; that timing has not been independently measured. The next missing evidence is a matched actual collection journey and an uncoached player's intention/action/consequence, not more aggregate movement statistics.
