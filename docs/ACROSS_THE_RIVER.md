# Across the river — first substantial campaign settlement

F11b / F18b working design. The existing five settlements remain the introductory chapter. This sixth settlement tests planning and recovery with the buildings already available. Human first-play target: roughly 20–30 minutes; this is not a minimum duration or a claim established by a scripted solution.

## Decisions to test

Begin with eight housed residents, a forager hut, modest food reserves, and limited timber on a compact west bank. The east bank offers more woodland and building space. A continuous river prevents walking around a crossing. Several bridge locations are valid; proximity to the yard, fields, and future houses should matter.

- Build an early crossing to reach timber, or first improve food supply on the starting bank.
- Use cottages and directly edible vegetables, or spend workers and timber on milling, lodges, grain and bread.
- Invite newcomers when housing and food can support them. New arrivals need jobs; population alone supplies no production bonus.
- Recover an awkward layout through demolition or a second crossing. Recovery takes real labor and retains materials.

No building unlocks, deadlines, deaths, daily taxes, surprise disasters, fishing/quarry prerequisites, or compulsory perfect build order. A competent player may prepare ahead and finish faster.

## Settlement progression

1. **First neighborhood:** expand to at least twelve residents, house everyone, and provide four beds across the river. The player explicitly starts a supply assessment when ready.
2. **Prove the first expansion:** serve two consecutive full, mixed meals with at least a quarter of portions outside the dominant food. Fresh pantry deliveries during the assessment must cover the portions consumed. Missing a meal condition resets that short meal streak; no construction progress is erased. Explain unmet conditions in Goals. The player chooses when to begin the final expansion after this proof.
3. **Village on both banks:** expand to at least sixteen residents, house everyone, provide eight east-bank beds and a completed east-bank square. At least half the current population must have completed a visit to an east-bank square within the last 120 simulation seconds. Begin the final assessment explicitly; housing and recreation must remain valid at each assessed meal.
4. **Prove the final village:** repeat the supply proof over three meals. The outcome is a working settlement, not a stored-food quota. Extra population increases actual meal and housing requirements. Food may be produced on either bank and travel through the existing shared pantry.

Assessment starts measure deliveries from that point forward, exclude food still carried or growing, and include the actual population at each meal. Supper and visitor trades must not count as ordinary meals or fresh production. Phase changes, delivery baselines, meal streaks, and completion save exactly. Recovery remains possible after a shortage. A completed phase stays completed; final operational conditions are checked again before victory.

Homes currently provide village-wide capacity: describe east-bank beds honestly, without claiming that specific residents have assigned east-bank homes. F25a will add home destinations later.

## Playable checks before calling this slice complete

- Run at least two viable approaches, recording construction, invitations, bottlenecks, food deliveries, and elapsed simulation time.
- Try a plausible inefficient layout and recover it through changed staffing, demolition, or a better crossing.
- Confirm that starting food, a single vegetable, undelivered crops, and an unused east-bank square cannot satisfy the operational proof by themselves. East-bank production is not mandatory; real square participation is.
- Check growing beyond the nominal targets, shortages during assessment, pause/reload, pending demolition, and all transitions through the campaign menu.
- View the entire map and the Goals panel at 960/1440. Show the current phase, preparation requirements, assessment result, and the next action without a long simultaneous checklist.

If most elapsed time is passive waiting, revise geography and competing work rather than lengthening meal windows or inflating quotas. Human playtest feedback is still needed to judge engagement and the first-play duration; scripted completion proves feasibility and exposes pacing problems, not fun.

## Implemented proof and remaining review

The sixth scenario is playable with saved phase transitions, fresh-delivery baselines and actual square participation. A completed first proof remains latched while the player prepares to advance. Current saves use format 23; old development saves can be discarded.

Two scripted approaches succeed: early bridging with cottages/gardens finishes at 660 simulation seconds (11 minutes); food-first bread production with a southern crossing and lodges finishes at 780 seconds (13 minutes). The cottage route stops logging once final construction is funded, retaining eastern woodland. These optimized runs are feasibility checks, not measured human playtimes. Both build food surpluses, so sustained challenge and the 20–30-minute first-play target still need human review; do not lengthen counters to meet that target.

Simulation checks cover extra growth to 18 residents, stored-food and token-variety exclusions, shortage recovery, exact saved continuation, and demolishing/rebuilding a house and square during an assessment. Rendered checks cover Goals at 960/1440, phase buttons, manual checkpoint restore and persisted completion. Run `./Test.ps1` and `./Play.ps1 -RiverSmokeTest`; the full `-CampaignSmokeTest` also traverses the introductory sequence and this scenario.

**Performance follow-up:** the completed 16-resident scene on the local Intel Iris Xe measured 48.9 ms median / 55.4 ms p95 over 120 rendered frames, with 4,352 draw calls and 4,542 rendered objects. Isolated C# measurements were approximately 0.42 ms per HUD update, 0.09 ms per actor/food-view update and 0.06 ms per simulation tick. This points toward investigating rendering/submission costs; it does not prove one specific bottleneck. Profile a prolonged paused placement preview and representative decorated scenes next, then reduce measured rendering costs without flattening the architecture. Do not expand population or decorative density first.
