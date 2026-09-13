# F31d2 — remote support does not rescue this hall site

September 13, 2026. Base `74df46f`, plus this chunk. The home-side hall remains the useful default. A nearby stone depot slightly accelerates remote construction, but neither tested support investment beats the home-side control. Cut the premise that a hall by this quarry is a useful alternative; retain unrestricted placement and existing storage tools. Do not tune hunger, yields or objectives to force support buildings.

## Comparison

`--remote-support` reuses the founding/hall driver and identical ready settlement. All four arms run 900 simulated seconds from the same baseline, using ordinary construction, shared labor and real goods. Quarry, sawmill and hall are placed before extra support so their positions stay fixed. Remote hall remains (13,7), rotation1; home hall (-1,3), rotation3.

Stone-depot adds a 4-log stockpile near the quarry, configured for stone while planned and targeted to12 when complete. Depots-pantry additionally orders a 4-log plank pile (target8) and a 6-log pantry (target12). Targets use normal setters; no goods are injected. Exact initial/early/final worlds and machine-readable reports are in `artifacts/remote-support`. The comparison is rerunnable with the test flag; artifacts are local.

| Arm | Built, seconds | First use, seconds | Loaded material distance | Hall walking distance | Completed visits | Unfed person-seconds |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| home-side | 409.1 | 421.3 | 867.7 | 705.8 | 40 | 0.0 |
| stone-side | 515.7 | 557.0 | 1252.0 | 1460.3 | 23 | 83.3 |
| stone-depot | 489.5 | 531.4 | 1329.4 | 1583.9 | 26 | 84.7 |
| depots-pantry | 568.4 | 589.6 | 1372.0 | 1081.5 | 20 | 528.8 |

The stone depot saves about26 seconds against unsupported remote construction, costs4 logs and increases total loaded material travel. The fuller support setup costs14 extra logs, finishes later and records more hunger. This does not prove every pantry/depot arrangement is harmful: it rejects these investments as sufficient justification for this site. We have not isolated the cause of the extra missed meals.

Travel covers all loaded stone/plank movement during the fixed window, including production and restocking; it is not quantity-weighted or isolated hall delivery cost. Visit totals have unequal post-completion availability. No novice difficulty, enjoyment or universal optimum is established.

## Player-facing clarification

Construction inspection now shows live pickups grouped by material and actual source (central storage or numbered stockpile), separately from loads already being carried. It does not invent a source after pickup: the simulation releases that source reference. Hover text explains storage routing and Keep targets. No new routing state, pathfinding, persistence format or compulsory building.

## Validation and limits

Both builds pass. Four comparison arms pass actual-use completion, periodic world validation and exact current-save roundtrips. The ordinary Godot founding/hall UI journey checks the live central-storage pickup text, active save/reload, real use, finish and Continue. Captured at960 and1440; representative inspector stills reviewed. This is scripted UI/simulation evidence, not uncoached play or listening. No performance claim. The earlier intermittent workspace save failure remains unproven as fixed.

This is a decision/evidence chunk with a small inspection clarification, not another playable scenario: count remains28; whole-game review30 remains due.

## Next direction

F31e should reconsider progression at village scale before another single-building project: compare the existing twelve-resident village with optional growth to a larger mixed neighborhood, using existing homes, food and jobs. Define the intended spatial/labor tradeoff first; check whether ignoring it still works. Good up-front planning remains valid. Do not add a new mode, catalogue, arbitrary timers or needs to guarantee difficulty. Keep scope TBD until that design comparison identifies a worthwhile transformation; abandon it if it only adds waiting. Watching an uncoached player remains more valuable evidence than more elapsed-time metrics.

Evidence identity:960 run `20260913-225411-293-founding-hall-e602ac`;1440 run `20260913-225454-526-founding-hall-5d623e`, each `capture-0004/view.png` contains the inspected pickup display. Final source fingerprint `416CD7AF1D9F4B12D4DD68D605AB87B57EB0BAA5D9267F4C25F945BAC5DE79BA`; game assembly `7628B69120C2F7A175DB1D38A43CA0F55748B2C485580CACDE9A6A349EF9AB9F`; test assembly `795537CB11CB28D0579827047C678C924AEE934CB807345E9F8D55995B52471A`. Comparison ran before the UI-only edits; test/simulation source was unchanged between comparison and final build.
