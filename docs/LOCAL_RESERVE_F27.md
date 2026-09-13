# Local food retention — checkpoint 15

September 13, 2026. Each edible producer in the selected settlement workflow now has Keep locally controls, 0–24 portions, default 4. Haulers reserve only surplus above this unclaimed amount, including welcome-table shipments. Residents can eat retained food. Existing claims finish after policy changes; production pause still serves stored food, and full stores redirect new output. Pantry targets remain a distinct destination control. Current-format saves are version 42; no migration work.

## Matched five-minute comparison

One normally built 12-resident village, all producers set to 24 for 120 simulation seconds before the common fork. Each branch changes retention to 0, 4 or 24, performs an exact four-second save continuation, then samples 300 seconds. Local/other meals count distinct requests observed eating at producer/other sources, not a satisfaction score. The final saved totals include another four-second continuation.

| Keep locally | New producer shipments | Portions claimed | Local / other meals | Hungry person-seconds | Final stored |
| --- | ---: | ---: | ---: | ---: | ---: |
| 0 | 26 | 89 | 12 / 50 | 0 | 108 |
| 4 | 23 | 70 | 19 / 43 | 0 | 104 |
| 24 | 0 | 0 | 14 / 49 | 0 | 105 |

The setting changes physical distribution, but more retention does not imply more local meals: full stores redirect output and people still select accessible sources. All branches stay fed. This is optional agency, not evidence of a difficult second act. Review whether the control deserves its cognitive cost rather than inflating scarcity to justify it.

`--local-reserve` passes bounds, actual claims, unchanged active shipment on raising retention, current saves, conservation and demolition. One demolition exceeded the first 180-second test allowance but completed within 600; no simulation change was used to make it pass. This is a waiting/priority observation for review, not proof of stranded goods. `--workplace-food-all` passes all six producers, paused meals, physical deposits, closures, original-mode pantries and active saves. Builds have zero warnings.

Rendered policy +/- and F5/F9, food view and staffing pass at 960 (`20260913-125504-830-neighborhood-working-village-797fec`) and 1440 (`20260913-125522-798-neighborhood-working-village-7344ce`). The 960 inspector is readable but text-heavy. Keyboard menu smoke passes both sizes; it follows the new prototype submenu and covers Creative/campaign/replay/errors/settings. No native play/listening claim.

Count 15. Freeze this outcome and conduct the whole-project review now, before selecting implementation. Cover the entire campaign, normal/Creative play, building/needs economy, controls, presentation/audio and tooling; the last five outcomes do not define the review boundary.
