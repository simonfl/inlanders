# F31b2 — staged redevelopment

September 13, 2026. **Reject staged redevelopment on Willow inlet as the next challenge.** Bulk ordering all construction and demolition at the start beats both careful approaches without missed meals. Demolishing first can cause trouble, but avoiding that mistake does not require sustained decisions. This is evidence and a direction decision, not a playable outcome: count26, next periodic review30.

## Matched comparison

Start with `World.NewInheritedShoreline()`. Build four homes through ordinary construction, invite eight residents through the existing arrival system, and allow another180 simulated seconds. Clone that sixteen-resident world at379.4 seconds for every arm. No Creative edits, supply injection, cost overrides, new terrain or new hunger rules. Free relocation is correctly unavailable under these ordinary rules.

Replace three eastern gardens with three legally placed western gardens. Compare no action, building all replacements before demolition, one-at-a-time replacement, demolition before any replacement order, and all six orders at the beginning. Continue every arm900 simulated seconds at0.1-second steps, validating conservation/state every10 seconds and exact current-save round trips at the end.

| Arm | Rebuilding finished (simulated seconds) | Unfed resident-seconds | Unfed in final300 seconds | Total meals | Final stored food |
| --- | ---: | ---: | ---: | ---: | ---: |
| unchanged | — | 0.0 | 0.0 | 238 | 0 |
| retain-until-built | 148.1 | 0.0 | 0.0 | 240 | 9 |
| replace-one-at-a-time | 323.3 | 0.0 | 0.0 | 240 | 6 |
| demolish-first | 154.4 | 500.9 | 178.6 | 210 | 7 |
| bulk-orders | 129.7 | 0.0 | 0.0 | 239 | 15 |

The one-at-a-time route lasts5.4 simulated minutes because the script waits between orders, not because the evidence establishes5.4 minutes of interesting decisions. The bulk route finishes in2.2 simulated minutes with no follow-up action. Doing nothing also feeds everybody. No in-game completion gate or score was added; “finished” in this table means the three old gardens are gone and three replacements are built.

Placement uses the existing nearest-legal search, and legal sites depend on live activity. The third demolition-first garden lands at(-2,7), while the bulk and all-built-first routes use(2,3). Therefore the demolition-first hunger cannot be attributed solely to order; timing and geometry are confounded. The matched bulk/all-built-first comparison is sufficient to reject the need for staging on this site. This is one initial state, not a universal verdict on construction gameplay.

## Recovery and presentation

From the demolition-first world at the end of the900-second test, clone a same-age control. In the other clone, construct an additional eastern garden at(10,2) through ordinary labor/materials. Continue both600 seconds. In the final300 seconds the control has162.2 unfed resident-seconds; the repaired village has0. Recovery is possible, but “add another garden” remains a weak basis for the deeper game. It does not diagnose all reasons for the original shortage.

Inspected four1440px stills from the exact measured worlds: opening, all-built-first at120 seconds, demolition-first at120 seconds, and bulk at900 seconds. [Opening](images/redevelopment-initial.png) and [bulk result](images/redevelopment-bulk.png) show gardens moved among crowded homes and an emptied eastern shore. This is legible spatial change, but the empty land has no demonstrated purpose or rewarding destination. Small legacy world labels remain visually noisy. No renderer, audio, ordinary UI or game-entry changes were made.

## Direction after two failed challenge candidates

Stop trying to turn the self-sufficient court/inlet into difficulty through rearrangement or longer construction. Retain the court as an introduction/free arrangement. Suspend the old F31c redevelopment delivery.

**Next: F31c1, one playable founding-settlement candidate.** Shift the activity to creating a functioning village where it does not yet exist. Use normal construction and existing shoreline/woodland food systems, one authored site and player-triggered growth. Choose where the next homes and food will go, then respond to the resulting journeys and supply. The opening must give the player a concrete reason to build; doing nothing should leave the intended settlement unbuilt, without requiring an artificial disaster. Keep the catalogue available, free arrangement separate, and an optional ending after actual use.

This is a falsifiable implementation bet, not evidence that founding will be fun. Audit the existing lake/woodland openings before choosing the site; avoid merely renaming a legacy assessment. Deliver one accessible candidate for actual play instead of another standalone courtyard metrics harness. Check a bulk-order strategy during development: if it resolves the whole scenario without another meaningful choice, report that failure and revise the activity rather than stretching time. No attendance certificates, required move counts, new needs or five-level rollout. Human observation and whole-game review remain necessary; these experiments do not replace them.

## Reproduction and limits

`./Review.ps1 Build`, then `.tools/dotnet/dotnet.exe Tests/bin/Debug/net8.0/SimulationTests.dll --redevelopment`. The command writes ignored `artifacts/redevelopment` starting/early/late worlds, event log/table and the same-age recovery pair. `--redevelopment-recovery` reruns only the recovery from the stored demolition-first endpoint. Both checks passed; no simulation production code changed. The full comparison ran first, then the additive recovery method was built and run separately; its final integrated invocation was not repeated.

Fixed base `fc7dd98`. Comparison source fingerprint `31EF53FCF5A0B4921D97F52523C4775F7CF97ACF3D453B945D8501B953A1438F`, test assembly `40F6A45D74EC6A77A9C814158CF96AEA86AFD586E97B2BDBE609D359EAD6D4D5`. Recovery addition fingerprint `E87FA62756A9942D0BBEB7DB7E1719069EE03B316F5F03F3AF778E0C893C07F7`, test assembly `5025C44C310C5AB589CDD7EF591772C83596F0F09B4BBC775E0C2240C748FCDA`. Unchanged game assembly throughout: `51D6F5BD5EC701D61B031732313B9BE15D9A913343FD7CF41FD5A7FB6FFD9A02`.

Local still bundles: `artifacts/redevelopment/views/{initial,retain-until-built-early,demolish-first-early,bulk-orders-late}/capture-0001`. Explicit review requests load hashed measured artifacts through the existing normal Godot review runner; they are not new fixture generators. First two views precede the additive recovery build, last two follow it; request files retain the corresponding build hashes. No native/uncoached play, continuous observation, listening, human pacing/preference or frame-performance acceptance. No independent or periodic whole-project review claimed.
