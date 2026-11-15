# Outcome265 — A harvest to share

November14,2026. One complete situation following the owner's enjoyment of accumulating resources for others. Final candidate source `21cca36`, build `1FC52A64171C`. This is one playable outcome, including internal balance, clarity and reconnect corrections. It is not a new claim of human enjoyment.

## Playable change

Eight residents already have four cottages and a working fishing dock. Anse aux Saules lost its crop; send48food and16logs so its families can eat and repair garden fences. No newcomer/housing chapter, arrival waves, shipment repetition or deadline. The old bridge/two-bank setup is replaced by an inlet with an open inland path. Start with24food,12logs and two renewing tree spots. Residents begin at their homes.

One request replaces the two manifest buttons. The choice is how to develop food surplus: clear compact gardens near homes, cultivate larger inland fields, or expand fishing toward the outer reach. Starting livelihood alone cannot supply the relief request in the tested baseline. Existing building/resource rules remain shared. This is a concise complete situation, not a promised long management level.

Packing, cancellation/refund, local-meal protection, exact dispatch conditions, a12-second1×departure, prior-speed restoration and continued tending are retained. Packing's priority over new construction is disclosed before commitment. The food floor is a packing threshold: residents still consume the reserve afterward. “Cargo packed” does not mean the village has enough food to dispatch yet; the current missing amount remains explicit. Irrelevant fresh-meal credentials and bridge promises are removed from this situation's UI/metadata.

## Fixed-build review and decision

Game design, UX, independent ordinary-command playtest, development lead and visual/audio reviewers assessed `23620ba`/`A162E34C7644`, with the whole-project264 review explicitly reused for unchanged native campaign, public/Creative and shared systems. They state what they did not observe. Review corrections were checked narrowly on `059af2f`/`12998CC10519`; the final reconnect fix is `21cca36`/`1FC52A64171C`. [Original reports and addenda](HARVEST_RELIEF_265_REVIEWS.md).

Consensus: **partially convincing; keep and deliver this complete situation provisionally**. The established start, fixed recipient and production focus distinguish it from264, but the shipment can still be an initial investment followed by quiet accumulation. No reviewer equates completing it with enjoying it.

| Keep / revise / defer | Decision |
| --- | --- |
| Keep | Helping others, one understandable request, automatic useful work, real food/log accounting, different land/production plans, explicit departure and tending. Keep the browser/C# stack and shared ordinary commands. |
| Revise inside this delivery | Original upper fishing renewal made fishing-only relief unsustainable. Increase only this scenario's outer-reach renewal from5 to10fish/min; prove a pure-fishing route. Clarify packing priority/reserve/cancellation and packed-versus-dispatch readiness. Report cottage overlap before clearable trees. Distribute initial residents at homes. Correct open-browser restart recovery. |
| Defer / drop from this scope | No global fishing nerf, quota inflation, new needs/tiers, compulsory fields, extra population, decoration expansion, native catalogue port, campaign framework or new analytics. Existing native systems remain available but are not migration requirements. |

The alternative remains a tighter restoration anthology if accumulation is understood but mostly endured. Conversely, the owner's positive reaction may mean watching a useful stockpile grow is itself rewarding; do not punish an efficient opening just to force clicks. The next experience question is whether this different starting livelihood preserves that satisfaction. Do not expand a shipment catalogue before the reaction to this complete situation.

## Evidence and limits

### Independent ordinary-command play

Initial candidate: one near-home garden plus one inland field, both chosen from public map/preview. A blocked site was moved; the evaluator later turned the garden entrance to reduce its delivery route9→2steps. Mixed cargo and active construction survived a real server restart at33s; cancellation/refund preserved goods. Premature/duplicate dispatch and post-departure refund were rejected. Cargo filled271.8s, but localfood15/16 delayed dispatch until362.4s; ending374.4s. Prior6× switched to1× at departure and returned to6× at the paused ending; continued tending worked.

Corrected ecology: a fresh fishing-only run chose one additional dock near the outer reach, with no crops or later production investment. Mixed cargo survived restart at60s (16logs/8food packed,18food/0logs available), then refunded/reordered exactly. Cargo filled449.2s; an ordinary local meal briefly blocked sending at15/16 before another catch. Ending497.4s, all8fed/housed; continued tending527.4s. This establishes a viable alternative and also shows a one-building-and-wait solution. It does not establish sustained decisions.

The evaluator inherited parent context, although no source/test/roadmap files were opened before the first run and actions were chosen from public state. Therefore this is independent ordinary-command play, **not rigorously source-blind**. No human preference or rendered-motion claim is attached to those API runs.

### Scripted mechanics and comparison

Corrected simulation, same starting state:

| Plan | Completion, simulated seconds | Observation |
| --- | --- | --- |
| Existing dock only | Incomplete at1801 | Only12food packed; localfood0; a real supply problem, not an automatic win. |
| Three nearby gardens | 345.1 | Complete, no hungry time during measured route. |
| Two inland fields | 334.1 | Complete, no hungry time during measured route. |
| Two added docks, no crops | 479.1 | Complete, no hungry time during measured route. |
| Delay480seconds, refund packing, then add fields | 918.0 | Recovery and completion; the request remains achievable after a poor start. |

Checks cover current-format exact save continuation at stages/partial cargo/departure, refunds, claims/conservation, rejected wrong manifest/arrival/bridge actions, no double send and continuing after success. Two early scripted garden/field coordinates were invalid (cottage overlap and tree access); those failures were corrected as test plans, not hidden as game defects. The overlapping-cottage error was made clearer because it previously reported the clearable tree first.

All four264 village-story routes pass unchanged after shared-message/validation edits. Independent lead checks cover fresh/load validation of all ten native campaign starts, Creative, public Normal/Relaxed and prior river-home, plus exact relief continuation/refund. Those are in-memory checks, not fresh native whole-campaign completion.

### Browser and restart observation

Builder used an actual402×681 browser viewport for briefing, food grouping, field preview/confirmation, request commitment, cargo/ending and continued play. Middle progression was prepared through ordinary public commands in a disposable session; this is mixed UI/engineering verification, not human play. Final visual evidence and reconnection results are recorded in the release addendum below.

The first restart test exposed an open-client bug: a displayed11-second state rejected the server's restored10.1-second paused checkpoint because its step counter decreased. The final fix attaches a process identity, accepts a restored state from a new process, and rejects late responses from retired processes. Same-process out-of-order guards remain, and replay may still scrub backward. Independent lead found no blocker in this bounded fix; multiple previously unseen rapid restarts and crash-proof every-frame persistence are not claimed.

No new physical-iPhone test, audio audition, dense performance result or full native-mode playthrough is claimed. Owner feedback supports producing for others in264;265 remains awaiting actual preference evidence. The final handoff is one complete playable situation, with no internal approval queue for the owner.


## Final browser release checks

On `1FC52A64171C`, an open running browser reconnected after a real server stop/start and displayed the restored paused village with an operable Resume control, without a page reload. The earlier observed stale-running state no longer persisted. The root placed a field through actual touch-style preview/confirm, used ordinary public commands to prepare a garden and advance work, then ordered and sent relief through the UI. Captures show48food/16logs and16localfood at readiness, actual loaded-boat movement upriver at1×, recipient-specific ending, and continued-tending controls. At completion all eight were housed/fed,18localfood/22logs remained. No console errors were observed. This mixed engineering/UI run does not replace the independent evaluator or human play.

Final capture set is local under `artifacts/browser/relief265/phone-*-final.png`; exact server/client build archive is `artifacts/browser/build-1FC52A64171C.zip`. The ordinary recording remains available to session-summary; reports are preserved in the repository. No owner saves or journals were altered or deleted by these checks.

Final visual addendum on the five corrected-build stills found no release blocker: cargo retains map scale, cultivated land makes the investment visible, and the departure/ending communicate the contribution. This is independent still inspection; root motion observation remains separately attributed, and audio is unverified.


## Owner correction — shorter shipment actions

Owner clarified that the excessive text was inside the collection/dispatch panels, not assistant updates. Reduce the default request to one log-priority consequence and a Start packing button with quantities. Reduce progress to packed counts, local food requirement, Send/Cancel and the current blocking reason. Remove repeated recipient story, option descriptions and ready-state filler. Move mechanics and recent supply behind collapsed How packing works; reopening the panel returns to the compact view. Apply to both village stories. No economy or progression change and no new outcome count. Targeted phone runtime verification is recorded below; do not repeat the full strategic review for this correction.

Build `6639F0101660` verification: client/server build passed. Independent actual402×681 browser check found about22 visible request words and33 partial-cargo words, versus the prior approximately90/150. Optional detail opens and resets on reopening; current blocker remains visible until ready, then disappears. UI cancellation returned exactly3food/16logs, reordering worked, and Send started the visible1×departure. No blocker found. [Narrow runtime report](CARGO_TEXT_265_REVIEW.md). No new strategic review, economy test battery or playable outcome was added.
