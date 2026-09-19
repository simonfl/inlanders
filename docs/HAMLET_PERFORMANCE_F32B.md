# F32b — reduce measured simulation stalls

September 19, 2026. Checkpoint33; next full review35.

The founding hamlet generated measurable simulation spikes, not merely slow screenshot setup. Every A* obstacle query enumerated each building footprint and allocated LINQ closures/enumerators. Replace that hot predicate with live collection loops and an allocation-free inverse-rotation footprint test. No occupancy cache or invalidation layer, path ordering, movement rates, needs or save changes.

The same twenty-person snapshot, advanced300 ticks, took11,738ms before and3,955ms after. Allocations dropped883,333,592 to61,959,984 bytes. Tick p95 fell226.71 to71.22ms. The complete resulting JSON is byte-identical (SHA256 D9A32BE655AB34FBF894200D48E311EA0E0408CAE0E80B1CE8F09543248E6BCB). Predicate tests compare the original rule across the map, plus every building kind/facing against the existing footprint iterator. Existing rotation tests pass construction, homes, demolition, bridges and actual fishing. Both game/test builds pass.

| Native sample, same1440 view/input | Before | After |
| --- | ---: | ---: |
|1x, eight simulation seconds: wall time |13.97s|9.75s|
|1x simulation p95 |255.75ms|89.64ms|
|1x wall-frame p95 |416.38ms|210.28ms|
|6x, roughly thirty simulation seconds: wall time |18.96s|8.27s|
|6x simulation p95 |510.86ms|172.59ms|
|6x wall-frame p95 |614.79ms|290.79ms|

This improves real play but does not establish smooth performance: ordinary median wall frames remain around60ms, and tails are still high. Samples are short on this Intel Iris Xe machine; native capture is not an uncoached player session. Native ending times differ by less than one accumulator batch; exact continuation equivalence is established separately with the fixed300-tick test.

Evidence: artifacts/route-perf/{before,after}/{report.json,after.json}; native runs20260919-130413-875-hamlet-spacious-35976a and20260919-131026-328-hamlet-spacious-7bef8d before,20260919-131709-755-hamlet-spacious-30c462 and20260919-131825-669-hamlet-spacious-79593a after. `--route-perf <snapshot> <output-directory>` makes this bounded comparison repeatable. No larger performance framework or engine rewrite.

Queue decision: retain the improvement, carry unresolved frame tails to review35, then test actual inherited-center rearrangement. Do not turn this into indefinite optimization before testing the spatial direction.
