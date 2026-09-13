# F31c1 — A home by the water

September 13, 2026. **Playable founding candidate delivered; deeper challenge not accepted.** Checkpoint27, next whole-project review30. Play now offers founding and the existing short introduction. Free arrangement remains available separately.

## What the player gets

Play → Found a village · A home by the water → New. Eight residents arrive with one finished cottage and a forager hut. Eighteen logs replace the investment in three removed inherited cottages; no extra timber is minted. Forty starting berries and the ordinary lake resources remain. Workers share jobs. Build homes, establish food where desired, and invite two households through Village or People. Each invitation brings two residents immediately; no automatic growth timer or stock quota. Finished spare beds, clear arrival space and current meal service are required.

The optional ending requires at least twelve residents, current housing, an actual consumed meal for each newcomer and current fed status. It acknowledges settlement, not sustainable throughput or transformation quality. Keep building or return to the menu afterward. New arrivals join shared work; founding meals use producer-local supply. All buildings remain available.

The source audit retained the lake's two fishing habitats and woodland trees, adding a reachable hunting habitat among existing mature trees. The living-woods campaign's larger habitat/assessment sequence was not imported. Normal logging can reduce woodland capacity. No new producer, renderer, audio system, balancing override or rolling assessment was added.

A separate `saves/founding.json` participates in New/Resume, Continue, F5/F9 and reset. Current format47 persists founding identity, actual newcomer consumption and the declared ending; no migrations. The short introduction and free-court saves remain separate. The opening and ending fit at960px after shortening the first verbose draft: [brief](images/founding-brief.png), [ending](images/founding-finished.png).

## What the evidence says

`--founding` constructs homes and a fishing dock or hunting lodge through ordinary commands, then adds homes/garden and invites two households. It validates state/resource conservation, real newcomer meal records and exact save round trips. The bulk control orders its homes and dock immediately and only sends invitations when the normal action becomes available. No food is injected during testing.

| Route | Ending available, simulated seconds | Unfed resident-seconds through ending +180 seconds | Final stored food |
| --- | ---: | ---: | ---: |
| Fishing plus later homes/garden | 186.1 | 0 | 138 |
| Hunting plus later homes/garden | 184.0 | 0 | 69 |
| Bulk homes/dock | 180.1 | 0 | 86 |

The fishing route recorded22 fish eaten; the woodland route recorded3 game eaten, with berries still dominant there. These are not two equally strong economies. The staged scripts wait until120 seconds to add work, so their duration is not measured player deliberation. The test observes only another180 seconds after eligibility; it does not certify long-term balance.

**Bulk ordering succeeds too comfortably.** Retain this as an accessible gentle candidate, reject it as the requested longer skillful level. Do not expand it into five levels, raise the population target to stretch time, or call the scripted ending enjoyment. Its useful improvement is a playable founding activity and normal growth controls; its difficulty remains unresolved.

## Checks and limitations

Build passed. Three simulation routes and current saves passed. The preexisting finite/free court simulation check passed. Founding UI probes at960/1440 use actual menu/New/invite/finish/Continue/F9/cancel/Resume buttons. Home/garden orders and intervening ticks in that probe are driven by the test, not a human playing. Both final UI runs passed; invitation and ending guards, separate slot and legacy-goal hiding are checked. The renderer also exercised ordinary pause/speed, selection, F8, food view and shared-worker dedication controls.

Final founding UI bundles: `artifacts/review/runs/20260913-215428-028-founding-16a81c` (960), `20260913-215522-465-founding-ced370` (1440). Source fingerprint `A25E7038B2F1F00496B0FAE3479FA79C36673C9988951ACF3E8CC6B1F062472E`; game assembly `4461BC3906D9FE2F41268ABF526DB6593C722B57A5046E55269A7CB2C0845BBC`, test assembly `66F7390888990F9911F80ACCB7646661AFC7ED5B2BD5DCAA4B040F0EB9F74FA5`, base `39ddae0` plus this chunk. Simulation route artifacts are in ignored `artifacts/founding`; route tests preceded the final brief shortening and strengthened finished-save validation. Final UI routes exercised both changes.

The first legacy court UI regression run failed when replacing Continue returned an access-denied file error; menu error handling kept the page open. This was not a founding-rule failure. The unchanged-source retry `20260913-215640-927-court-experience-f38df1` passed the complete introduction/free/save/reset/archive UI journey. This intermittent file replacement failure remains a reliability issue to investigate if repeated; no save-preservation or compatibility work was added. No native/uncoached play, listening, motion-quality or human pacing/preference acceptance. This chunk does not claim an independent or periodic whole-project review.

## Queue decision

Next F31c2 must address the bulk-order bypass through a consequential expansion decision, not another ending gate. Audit actual source output, useful land and the timber/habitat conflict in this playable candidate. Choose one concrete conflict where growth changes a previous siting or production choice; implement it in the candidate and keep a bulk control. If it still plays like an easy introduction, say so and cut the extension rather than inflate goals or wait time. Keep the court and free arrangement; freeze new needs, catalogue expansion and multi-level rollout. Whole-game review30 must reassess the direction.
