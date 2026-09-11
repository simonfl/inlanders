# F21l — five playable campaign UI chunks

Implemented September 11, 2026. River and lake rules are unchanged; this pass makes them inspectable and actionable.

1. **F21l1: compact goals.** The current phase and its action appear before compact count rows. Each condition has an expandable explanation; the full narrative no longer pushes progress far down the panel.
2. **F21l2: resident evidence.** Expand housing, rest or recreation and choose **Inspect residents**. People can show residents who count or do not count, with a reason and existing home/venue links. East-bank Square filtering uses the same predicate as the campaign counter. Lake recreation accepts any recreation venue within two minutes; improved-home rest uses its earned five-minute window.
3. **F21l3: places and plans.** Expanded conditions list the relevant homes, Squares, venues or docks, including unfinished/removing status. **Show** opens the inspector and moves the camera. **Plan** starts the appropriate build preview without placing anything; east-bank goals focus the eastern land.
4. **F21l4: food evidence.** Assessments show residents with two closed requests, missed/skipped counts, actual variety and fresh deliveries. An explanation distinguishes the moving three-minute window from synchronized meals or stockpiles. Meal-trip and Economy links support recovery. The first river proof remains visibly earned while preparing to expand.
5. **F21l5: optional tracking.** Choose **Track while playing** in a condition's explanation. A compact count appears while Goals is closed and can reopen that goal. It updates as visits expire, hides in Watch/placement, and clears when the settlement changes or the condition disappears. This is temporary UI state, not part of the save.

## Verification

`./Play.ps1 -GoalsSmokeTest` covers both campaign maps at 960/1440: explanations, counted/not-counted lists, safe planning, meal/Economy links, tracked-goal placement/input isolation, Watch behavior and exact saves during navigation. It ran after each chunk, expanding alongside the feature.

The existing river and lake simulation checks passed after integration, including alternative approaches, recovery and saved assessment continuation. `./Play.ps1 -RiverSmokeTest` completed a rendered level-6 playthrough and persisted completion. The final 960-pixel Goals screenshot was visually inspected. No gameplay costs, visit windows, win conditions or save format changed.

## Review and next recommendation

Play level 6/7 with these controls and assess whether it is now clear why a count drops and which action can help. This is the next review, not an automated claim of usability or enjoyment. The food-trip shortcut uses People's full recent three-minute history; a just-started assessment can cover a shorter interval, as its tooltip explains.

Do not expand this into more metrics before that feedback. Keep art acceptance, food-service enjoyment and campaign pacing reviews open. The existing fishery/quarry/wildlife systems provide enough building variety for the next campaign design decision; mandatory comfort, new needs and further mastery scenarios still need their distinct payoff established.
