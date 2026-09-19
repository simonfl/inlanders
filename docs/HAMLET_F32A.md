# F32a — lakeside hamlet comparison and landscape candidate

September 19, 2026. Presentation review completed against9641145; retain the rendering improvement as checkpoint32, not acceptance of the spacious-hamlet hypothesis.

Four ordinary twenty-minute runs start from the same founded twelve-person village. No resources, food rates, housing capacities or placement permissions change. Legal sites use nearest-all-facings placement; requested/actual cells and displacement are recorded. This is authored simulation, not an unfamiliar player choosing a layout.

| Arrangement | Settled twenty after | Hungry person-seconds | Meal travel | Building ground | Recreation visits |
| --- | ---: | ---: | ---: | ---: | ---: |
| Four more cottages, garden/dock |112s|0|913|73 tiles|0|
| Two lodges, sawmill, seating garden, garden/dock |586s|0|981|68 tiles|144|
| Same lodge arrangement, dock delayed600s |623s|0|1082|68 tiles|145|
| Remain at twelve |—|0|352|42 tiles|0|

Final300-second hunger is zero in every arm. The delayed-food candidate did not produce a shortage, so it does not demonstrate recovery or a necessary dock. The existing shortage experiment remains the actual recovery evidence. The lodge arrangement costs22 logs plus24 planks for its additional buildings, compared with36 logs for the cottage arrangement. Processing costs time, and the new sawmill itself uses space. The total footprint gain is five tiles, not a wholesale release of the center; all arms retain the inherited cottage cluster.

Decision: retain existing housing/food rules. A slower lodge investment can support open space and communal use, but the central cluster still needs actual rearrangement; do not claim that a successful script establishes a compelling village. Staying at twelve remains a valid choice.

The rendering candidate brings founding into continuous surrounding terrain, with worn entrance areas and a buildable boundary shown only while using tools. Distant terrain remains scenery. Lake geometry and building permissions are unchanged. Building labels become contextual to the selected building. No fake connecting paths are drawn in founding; real painted paths remain the movement mechanic.

Tooling: `Review.ps1 Capture -Scenario hamlet-spacious -Snapshot artifacts/hamlet/spacious-late.json` validates and hashes the measured state instead of regenerating a twenty-minute run for every camera. The imported state is explicitly labelled, with its origin path/hash and current-format validation; the report owns original experiment provenance. Existing fixture and bundle workflows remain available. This bounded extension benefits repeated comparison captures; no general cache/framework was added.

Simulation evidence: `artifacts/hamlet/*-report.json`, matching start/middle/late saves; command `dotnet run --project Tests/SimulationTests.csproj -- --hamlet`. World validation and exact current-save roundtrips pass. All runs use pre-change simulation rules. Native frame traces and rendered UI evidence will be recorded after candidate verification. No human preference or listening acceptance.

Rendered evidence: `20260919-130413-875-hamlet-spacious-35976a` (1440, ordinary1x advancement); `20260919-130601-849-hamlet-compact-55de54` (1440 UI probe); `20260919-130735-988-hamlet-spacious-4b7e52` (960 opposite camera UI probe). The first captures precede only probe additions/nullability cleanup; production rendering matches. Tests check contextual labels, boundary appearance/cancellation, scenery remaining unbuildable, and unchanged save state while inspecting.

The1x spacious trace has119 frames: simulation median0.37ms/p95 255.75ms, wall median61.32ms/p95 416.38ms; eight simulation seconds took13.97 wall seconds. This is a short native-process sample, not a hardware-independent performance guarantee. Its allocation spikes and simulation time justify measuring the route/obstacle hot path before adding further gameplay. Keep this issue separate from picture preference.


## Independent presentation review and decision

The fresh reviewer and a stale-context restart hit the thread limit. Existing independent presentation reviewer `commons_presentation_review` returned a whole-game visual/audio pass on fixed9641145: partially convincing; retain continuous terrain, contextual labels and tool boundary, but the inherited roof cluster remains dense from both cameras. Free court remains stronger composition evidence. Retain the warm architecture and real daily life; keep legacy campaigns archival. The stepped lake edge remains unresolved. No fresh native interaction, listening or long-session acceptance.

Lead accepts the limited retention verdict. Count32, next full review35. Performance comes first: the additional6x native sample `20260919-131026-328-hamlet-spacious-7bef8d` advanced30.59 simulation seconds in18.96 wall seconds (intended6x); simulation median244.54ms/p95 510.86ms, per-frame allocation median15.9MB. Profile and remove demonstrated hot-path waste without changing simulation outcomes. Then permit meaningful revision of the inherited center, with visible circulation; do not dress the same packed layout with more props or certify it via another target.
