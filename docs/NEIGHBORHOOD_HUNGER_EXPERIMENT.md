# F27a: removing the hunger slowdown is not the recovery fix

September 12, 2026. Control: `4e9edc4`. The opt-in `--neighborhood-no-slowdown` comparison generates the same natural shortage as the [landscape recovery experiment](NEIGHBORHOOD_LANDSCAPE_F27A.md), then changes only the saved neighborhood rule `HungerSlowsActivity` to false. Population, stock, placement, yields, meals and deadlines are unchanged. Normal neighborhood/campaign behavior retains the existing rule; this candidate is not adopted as the default.

| Recovery plan | First fully fed, control / candidate (seconds) | Hunger during final 300 seconds, control / candidate |
| --- | ---: | ---: |
| Three gardens + pantry, grain paused | 617.7 / 233.2 | 229.1 / 205.5 |
| Four gardens + pantry, grain paused | 418.3 / 274.7 | 108.4 / 74.7 |
| Three bakeries + pantry | 424.2 / 255.0 | 266.3 / 279.2 |

Each branch observes 2,400 simulated seconds after the mistake. Earlier fully-fed readings are real but do not establish sustained recovery. Candidate results and validated/roundtripped saves are under `artifacts/neighborhood-recovery-capacity-no-slowdown`, with a source-hash manifest. No native gameplay, listening or enjoyment claim.

Verification: the game builds without warnings; existing welcome-meal, committed-arrival and all six edible pantry checks pass, including physical transfers, closure/recovery and exact saved continuation. The new rule is serialized inside neighborhood progress; baseline modes keep their existing activity multiplier.

The global slowdown contributes to delayed recovery, but removing it does not solve the food/logistics design. Do not declare success based on improved first-fed time or combine several unmeasured rule changes. The shared caps are not the obvious next suspect for these particular plans: paused-grain garden branches have at most four food workplaces, matching the four-worker profession cap and below the six-worker total cap. Bakery branches have three ovens plus a farm, also below those limits. Larger expansions may differ.

## Direction decision

Stop expanding this sequence of small recovery adjustments. Retain the welcome as a physical introductory event and the two maps as concrete comparison scenes. Do not market it as the campaign's substantial logistics challenge. Current producer roles, transport costs and demands remain unresolved and must be redesigned before extending this loop across campaign levels.

**Next is F27b's whole-scene visual/interaction alternative**, working with the running neighborhood. This tests the village-arrangement direction selected in the strategic review. It is independent of making every economic branch successful. Compare the full scene and ordinary management view; do not substitute a nicer single building or more props. The existing blockout and menus remain the control.

After that comparison, decide between an appealing village-making core with introductory food pressure and a stronger logistics game requiring redesigned production/service chains and environment-specific constraints. A later logistics experiment should test a consequential change, such as producing/serving food at workplaces with separate distribution labor, rather than another yield or deadline adjustment. Keep that conditional until the experience comparison chooses it.

This is a change in work order, not retroactive acceptance of F27a. Ordinary play, recoverability and product appeal remain unverified; count stays eight. F27b must trigger the required presentation review, and the next periodic whole-project review remains checkpoint ten. Keep the failed alternatives available and do not count source/provenance tooling as playable outcomes.
