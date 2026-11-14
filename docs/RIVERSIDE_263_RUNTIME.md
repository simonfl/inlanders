# Independent runtime evaluation — Room by the river

Fixed candidate: commit `7ba6c16`, reported build `35DEB9CFCD29`. Fresh session `4d9d81c4e83b4eeb9089ab0f99e313e8`, `room-by-river`. Evaluation performed through public guide, preview, command and observation API. No source, roadmap, solution or owner journal read before play. NEXT_CHUNKS read only after completion. No state edits, CUA, server restart or save changes.

## Verdict

**Revise the map once; do not promote this candidate as evidence for the experience hypothesis.** The visible map admits two open near-bank homes and another open near-bank garden, preserving all four managed woodland spots and avoiding both river crossings. The stated near-woodland versus far-clearing choice was unnecessary. One extra food order followed by waiting solved the objective. My late attempt to improve supply contributed no output before success. This run demonstrates a bypass, not universal dominance across every arrangement.

The bounded automatic-work premise is mechanically coherent. Preview explained tree loss and cancellation restored the uncut managed spot. The optional bridge preview clearly disclosed six logs and a 23-to-11-step path. I had no practical reason to buy that path after using open near-bank ground. I did not build it, so actual new-bridge routing is unobserved here.

## Decisions and sequence

All times are simulation seconds, not human play duration. API calls made on demand; time advanced only through ordinary paused commands, at most 30 seconds per call.

| Time | Observation or decision |
| --- | --- |
| 0 | Four residents, four beds, 12 food, 10 logs. Existing near garden at (-6,6), old bridge (-8,-1). Previewed home (-2,6) and (5,2), both valid with zero clearing. Previewed wooded home (2,6): one tree spot, eight logs recovered, permanent grove loss disclosed. Previewed shortcut: six logs, 23 to 11 steps. |
| 0 | Ordered wooded home (2,6) as recoverable mistake, then canceled site -1000 before advancing. Order changed tree0 from managed true to false. Cancel restored managed true, removed plan, preserved 10 logs/12 food. This tests cancellation before felling only. |
| 0 | Ordered homes (-2,6) and (5,2), both rotation0. No clearing. |
| 28.5 | First home complete; six beds. Second home 4/6 timber. Automatic woodcutting and hauling observed in public worker states. |
| 54.1 | Both homes complete; eight beds, food10, logs12. Welcome permitted; welcomed four neighbors early. New goal: four newcomer fresh meals, 16 local portions delivered, reserve8, everyone housed/fed. |
| 54.1–57.1 | Wanted another garden to feed doubled population. Garden (5,7) valid with no clearing. Tried nearer (2,7): first blocked by Jeanne; advanced3s, then rejected for cutting routes. Chose previously valid (5,7), ordered57.1s. This is the only spatial reconsideration after housing. It did not make the far bank useful. |
| 87.1 | New garden building37%; food11, local4/16, newcomer meals2/4. |
| 94.8 | Garden complete. Food12, local6/16, fresh3/4. |
| 124.8 | Food8; both gardens growing. Most workers home or resting, some renewing/hauling timber. |
| 154.8 | Food2; original/new crop growth .963/.788. Chose to wait for imminent harvest rather than reorder. |
| 184.8 | Food4, delivered10; both gardens harvesting/delivering. All eight still fed. |
| 214.8 | Fresh4/4, delivered18/16, food6/8. Waited for remaining harvest. |
| 244.8 | Delivered20/16, food still6/8. Concern about ongoing supply prompted a third garden at (-2,2), again open near-bank ground, valid with no clearing. |
| 255.7 | Success. Eight housed/fed; newcomer meals4/4; delivered22/16; food8; logs26; woodland4; no shortcut. Third garden unfinished at28%, therefore no food contribution. |

There was no construction order between57.1 and244.8, a187.7s stretch of automatic growth/harvest observation. The extra garden arranged at57.1 produced useful food, but there was no later demonstrated route improvement arising from that arrangement. The reserve requirement triggered my late capacity order; success arrived before it mattered.

## Hypothesis comparison after play

The active card asks for “I made this arrangement; now I want to make it work better.” This run supports legible purpose, automatic execution, reversible uncut clearing, early welcome and spatial placement constraints. It does not support a consequential woodland-versus-far-bank tradeoff or later route ownership. The map bypass makes preserving the woodlot compatible with all relevant nearby construction. Keep the automatic work and transparent previews; revise land composition; hold expansion and experience acceptance.

## Evidence limits

This is actual agent interaction with public gameplay commands and observations, not a hidden-state simulation or source-only review. It is not human enjoyment/preference evidence. No audiovisual observation, normal-speed UI, phone placement, camera, controls, music/sound, save continuation, cancellation after felling, actual shortcut use, far-garden comparison, campaign, public/Creative breadth or long-run food sustainability were evaluated. A brief goal completion does not establish an enduring food balance. Server was released back to lead immediately after this run completed.

# Revised-map evaluation

Fixed candidate `cd230fd`, reported build `BCCF107BEC4E`. Fresh session `3c34070266734b259d9a18584de1c971`. Same public API method. I retained first-run learning but read no revised source or scripted routes. Initial API state showed eight trees/four managed spots and the same resources. Lead requested an adversarial preservation/no-needless-shortcut repeat, not an authored far-bank solution.

## Revised verdict

**Do not accept the claimed richer arc from this run. Keep the prototype/evidence; the one map revision did not establish the advertised woodland-versus-far-homes commitment.** Two near homes again fit without clearing. However, this was not a second effortless identical success: my dense arrangement made food trips long, and I responded with another garden on the far bank. That is evidence of spatial consequences and a reason to improve supply. It is not evidence that extra capacity is itself an enjoyable or richer arc, nor proof that the optional bridge provides the wanted improvement.

The useful unresolved issue is narrower than “add more mechanics”: **I wanted to revise an existing arrangement, while the public actions allowed canceling unfinished plans only.** At248.6s I inspected a vegetable carrier taking a long west/north loop and would have preferred turning or relocating my completed garden over simply adding capacity. My garden at(-1,5), rotation0 occupies x=-2..0/z=4..5 and has south entrance(-1,6). The home I put at(2,7) occupies x=1..3/z=6..7, obstructing its east exit; the garden footprint obstructs the short north approach. A north-facing entrance on the same garden footprint appears to be the concrete improvement I would seek, but it was not tested or offered as an ordinary completed-building action. The central bridge cannot resolve this local obstruction. Do not equate an imagined fix with verified design value.

The later far garden at(-6,-3) was deliberately beside the existing crossing, so I did not buy the central shortcut. Its preview still advertised23→11 steps to the authored far clearing around(1,-5), rather than the work I had actually placed. The statistic was legible but did not answer the practical question about my garden. Actual new-bridge behavior remains unobserved.

## Revised sequence

| Time | Observation or decision |
| --- | --- |
| 0 | Former home(-2,6) now previews one cleared tree. Home(5,3) and home(2,7), both rotation0, preview valid with clearing0. Near candidates(-2,5),(-2,7) require clearing;(-2,3) conflicts with yard;(-2,8)rot2 exceeds map. Far home(-6,-3) also valid, but no need to use it. Ordered the two open near homes. |
| 28.5 | First home complete. Previewed rotated garden(-2,4), rotations1/3; yard or collection-point conflict. Far garden(-6,-3) valid. Garden(-1,5)rotation0 valid, clearing0. |
| 58.5 | Ordered near garden(-1,5), clearing0. Second home still building. |
| 69.3 | Eight beds complete, food12. Welcomed neighbors early. |
| 98.6 | New near garden complete. Food11, local2/16, fresh2/4. |
| 128.6 | Fresh4/4, food8, local4/16; gardens growing/sowing. |
| 158.6 | Food4; existing/new crop growth .487/.660. |
| 188.6 | Food0; still8/8 fed. New crop delivering; existing nearly ripe. |
| 218.6 | Food2, local8/16, both harvesting/delivering. |
| 248.6 | Food1, local12/16, still8/8 fed. Inspected carrier route with18 remaining tiles, west and north around original cottage before reaching pantry. Added far garden(-6,-3), clearing0, beside the old bridge. This was a response to underperforming food flow, not a forced goal instruction. |
| 278.6 | Far garden timber4/4, building0%. Food2, local16/16; reserve remains short. |
| 293.8 | Far garden complete. Food1, local20/16. |
| 323.8 | Food0; all three gardens growing. Shortcut preview still generic23→11. Did not buy: central crossing was not an evident improvement for my west-side far garden. |
| 353.8 | Food0, all8 still fed; crops nearly ripe. |
| 383.8 | Food3, local24; three gardens harvesting/delivering. |
| 413.8 | Food2, local30; fed5/8. Recovery still possible; no new orders. |
| 439.1 | Complete: food9, logs34, local38/16, fresh4/4, all8 housed/fed, woodland4. Far garden finished its first crop; both near gardens still had harvest remaining. No clearing orders or shortcut. |

The only post-58.5s investment was the far garden at248.6s. The entire game was not an unchanged initial queue, but most elapsed simulation time was waiting for automatic work/crops. Completion took439.1s versus255.7s in the first candidate; these are different placements/arrival times and not a controlled performance comparison. No claim that the revised map alone caused the difference.

## Revised evidence limits

Actual public-command runtime interaction establishes placement, visible route, progression and hunger recovery facts. My desire to turn/reposition the garden is an agent evaluator judgment, not owner preference or human enjoyment. No exhaustive packing search or universal best-strategy claim. No completed-building relocation, alternate orientation on this occupied footprint, actual shortcut construction, far-start variant, current-save continuation, phone/UI/audio or broad-game review. Cancellation was not repeated; the first candidate's pre-felling cancellation remains the only cancellation evidence here. API play released to the lead immediately on completion, before any further inspection.

# Narrow garden-entrance correction verification

Fixed `5cd0fd1`, build `2CD7EDA0B457`, fresh session `9ccae696094a45e18a6ddeb3f6694398`. **Learned correction verification, not blind play or human experience acceptance.** Lead disclosed the new action and expected22→1 result; I independently recreated the cramped arrangement using public previews/commands and observed the result without source/state edits.

At0s ordered homes(5,3),(2,7) and garden(-1,5), all rotation0 and zero clearing. This differs from prior run's later garden order. Homes complete95.1s; garden complete128.3s; welcomed at128.3s.

At158.3s the garden had growth0.42833298. `garden-entrance` preview for site7 was valid, zero timber, actual food-store walk22→1, opposite entrance(-1,3). The old22-cell route matched the previously observed west/north detour; proposed route contained only pantry(-2,3).

Command accepted at the same simulation time. Independently compared before/after: same six footprint cells, exact growth0.42833298 retained, harvest0 retained, logs16 retained; entrance moved(-1,6)→(-1,3), anchor became(-1,4), rotation2. The garden continued growing, reaching0.9283325 at188.3s.

Actual short journey observed: Marie approached(-1,3) at203.3–204.3, harvested there at205.3 and206.3, then was at pantry(-2,3) carrying two vegetables at207.3. Jeanne had also been observed carrying two vegetables at this pantry at196.3. Public delivered count rose4 at197.3,6 at201.3, and10 by237.3. At237.3 the corrected garden had emptied its crop and was growing the next at0.16166656; all eight residents remained housed/fed. No obvious work regression in this observed cycle.

**Keep this narrow correction.** It supplies the concrete same-ground route repair wanted in the revised-map run and makes its benefit visible in both preview and actual work. It does not resolve the original near-bank bypass or establish enjoyment, a richer full-game arc, or long-run balance. No full completion attempted in this correction check; no save/restart, loaded-crop/active-carrier entrance switch, UI/audio or broad regression checks. API released to lead at237.3s immediately after verification so persistence testing could proceed.
