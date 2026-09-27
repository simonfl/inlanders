# Whole-game review 100 — September 27, 2026

## Freeze, independence and evidence

Reviewed game commit `97f9b7fe887c4208b885f37b49c4f5e4677de570`; Godot assembly SHA256 `623BEB9788824A13BC132CB4F4A9A771208A3876D9F2B9A48B8DD445BF01BA86`. No gameplay edits during review. Checkpoints91–100 deliver ten playable outcomes; documentation/tooling and reviews do not add to the count. User requested stopping at100.

Four fresh independent read-only roles: `review100_design`, `review100_ux`, `review100_play`, `review100_lead`. A fifth fresh visual role and attempted reuse of visual96 hit the agent thread limit; existing `review90_visual` was reactivated with the new freeze and evidence. It retains its old90 context but receives none of the new roles' verdicts before responding. This is five disciplinary passes, not five fresh contexts. Source/evidence review is not independent UI play. Role conclusions below are root summaries of their independent reports.

The fixed mature view imports the validated world from an earlier working100 ordinary-control session; it does not retroactively make that session a fixed-commit playtest. Final changes after that session concern UI and separate comparison tooling, not simulation.

| Evidence | Local artifact under `artifacts/review/runs/` | What it establishes |
| --- | --- | --- |
| Public entry at960, fixed |20260927-233241-092-first-place-f8401f| Scripted actual menu/choice/cancel/save-load controls |
| Mature founded farmstead, fixed1440 |20260927-233232-627-land-first-55636f| Whole scene from validated1200s state,8 residents/6 buildings |
| Same farmstead, opposite960 snapshots,1x |20260927-233428-157-land-first-de02b2|8.5s engine recording,204 frames at1440×900 despite960×640 requested window/snapshots; extracted samples/audio, not listening or performance acceptance |
| Inhabited Normal, fixed1440 |20260927-233418-896-across-inlet-97b19f| Public whole-scene comparison |
| Inhabited relaxed, fixed960 opposite |20260927-233342-575-cultivated-bank-relaxed-5b3a95| Same daily-life direction with free construction |
| Archived opening, fixed960 |20260927-233411-666-opening-159f0e| Introductory campaign presentation; not traversal |
| Archived dense later village, fixed1440 |20260927-233355-314-dense-3ed267|32 people/30 buildings; archive presentation and clutter |
| Free court, fixed960 |20260927-233459-426-creative-court-9805e0| Earlier arrangement reference, NOT legacy foodless Creative play |
| Four-direction drawing, final same-source precommit960 |20260927-233041-227-land-drawing-2fd436| Escape/UI-release purity and actual anchored placement |
| Household exchange, final same-source precommit960 |20260927-233109-608-household-move-e519d9| Resident selection/return, world target, cancel/confirm |
| Full ordinary-control predecessor100 session |20260927-231943-605-land-first-f2f8f6| Public entry,4 homes/2 drawn strips,20 simulated minutes, actual meals/home visits, observation and land revision; scripted choices/camera/6x waits, no world injection after entry |

Zero-warning final build. All37 current-experience suites completed exit0 (`cb92501286d14ee1802d11cd49201df8`, assembly MVID `44814283-21ca-48bf-8e6e-45bb424bb695`), broad regression exit0; final UI changes do not alter that tested simulation. Final six mature comparisons also completed exit0. Native probes use engine input, not an independent uncoached player. No complete campaign traversal, fresh legacy Creative UI session, human preference, continuous-motion judgment, listening or native performance acceptance.

## Material findings

### Mature cultivation has consequences, but not yet proven competing pleasures

Six scripted1800-second comparisons start with8 residents,4 homes and the same6 cultivated rows/18 tiles/12 logs. One nearby six-row strip, one remote strip, or two nearby three-row strips; one row is released after900s when ripe harvest allows it. Normal and relaxed use the same daily-life rules.

| Arrangement | Normal grown / ending food / hungry person-ticks | Relaxed grown / ending food / unfed person-ticks |
| --- | --- | --- |
| One nearby strip |144 /12 /0|144 /10 /0|
| One across the inlet |104 /2 /8442|104 /0 /2123|
| Two nearby strips |156 /32 /0|156 /31 /0|

Source/results: `Tests/MatureCultivationChecks.cs`, `artifacts/mature-cultivation/report.json`, `checkpoint100-mature-final.log`. Person-ticks are accumulated unmet feeding state, not a count of people or measured player distress; relaxed has no hunger penalty. The initial remote-versus-split comparison confounded location and subdivision; the final nearby-single control corrects that omission. Worker slots, crop timing, routes and actual revision timing still differ, so this is a comparison of configurations, not a size-only causal claim.

The nearby single strip finishes its first24 vegetables by roughly180s, still has24 grown through840s, and resumes production as provisions reach the reserve threshold. Quiet may be the reward of a secure home, or it may feel as though the farm has stopped. Higher final production does not settle that question. Do not automatically reduce safety provisions or invent work to keep people busy.

The native full session reached8/8 housed,72 grown and45 eaten at1200s, then revised the land. It closes the earlier evidence gap of stopping after one home and one meal. It does not show a voluntarily wanted revision.

### Independent role findings

**Game design — partially convincing.** Shaping useful ground and seeing identifiable residents use it is a credible central pleasure. The strongest objection is that the practical answer could always be more small workplaces close to homes. Household exchange among interchangeable residents is expressive, not demonstrated strategic depth. Keep physical life and the relaxed comparator; keep campaign geography while cutting recipe/certificate progression from the public direction. Proposes an inhabited river settlement with two viable places to invest, comparing consolidation against supporting separate clusters. Production rhythm needs a separate fair comparison if quiet feels inert; neither constant activity nor added scarcity is an assumed fix.

**UX/onboarding — partially convincing.** The controls increasingly explain how to change the village, but the world does not equally explain why. Household exchange identifies people and deferred timing, yet gives weak spatial before/after meaning. Land proposals foreground logs and crop size while fixed one-farmer throughput and revision restrictions remain harder to infer. Retain direct drawing, resident follow/return and calm public HUD. Proposes replacing redundant navigation with one place → actual people/use → change → observed consequence interaction. Test whether a player can predict and recognize a useful change without an accounting panel; do not add another permanent dashboard.

**Development lead — partially convincing.** Dynamic geometry, crop/material conservation, current saves and household exchange have meaningful invariant coverage. Test passes establish operational confidence, not the mature game's appeal. Revision tests are narrower across orientations than new-field tests; native input precedence remains outside pure simulation checks. Retain Godot/C#, shared life and reversible operations; avoid catalogue or architecture expansion. Proposes an authored reorganization situation with constrained cultivation, domestic ground and crossing choices. Native save replacement `0x80070005` remains unexplained; eleven attempts with ten40ms sleeps can plausibly stall a frame, but no attribution is established. Neither Dropbox nor the separate transient git-index write failure is proven causal. No more blind retries or save migration work.

**Playtest — partially convincing; evidence audit only.** The small mature place is worth retaining, but scripted coordinates, known buttons and accelerated waits cannot validate discovery. The quiet reserve interval and delayed remote shortage may leave a player without an intelligible next action. Retain safe small settlements and different livelihoods, reject campaign compliance as the remedy. Proposes a shorter settle–observe–revise loop and unfamiliar ordinary-control sessions. The first bounded productive cycle already exists since91; synthesis rejects implementing it again under another name. The unresolved comparison concerns mature rhythms and visible consequences.

**Visual/audio — partially convincing; reused90 reviewer.** The shared worked ground and nearby cultivation create a stronger domestic center than90. Lower houses, restrained materials and readable people earn retention. The broad smooth terrain, tiled inlet, repeated roofs/tree groups and old colorful menu diorama still undermine a distinctive whole-place identity. Reject a props-only response. Proposes an asymmetric river-facing settlement with meaningful yards, bank transitions, retained woodland margins and long cultivated ground, keeping the same editable productive inventory. F40's HUD-hidden New France identity criterion is not accepted. Fourteen images/frame samples inspected; no listening or continuous-motion judgment.


## Synthesis and direction decision

**Partially convincing, with a structural next experiment.** The strongest present pleasure is making a modest working home and observing physical life. Ground drawing and household observation now support that pleasure coherently. None of the roles accepts sustained motivation, independent discoverability or whole-scene identity as settled. One useful improvement may be enough for a satisfying short visit; extending duration through chores is not the goal.

Agreement: keep small optional-growth villages, real shared work and meals, generous recovery, useful yards, founded/inhabited starts and Normal/relaxed. Do not expand needs/catalogue or restore campaign certificates to manufacture purpose. The disagreement is emphasis: design/lead ask for competing spatial investments, UX asks for perceivable consequences, visual asks for landscape composition, play asks for the lived timing of decisions. These are related but cannot all be resolved by one panel or one successful automated run.

Choose **one inhabited river settlement with two credible improvement directions** as the next substantial alternative to open founding. It must be viable before the player acts. One group has compact domestic life and constrained productive ground; another has useful food/shore/woodland access with awkward daily journeys. Consolidating or supporting both clusters must incur different visible land, work and domestic-use consequences. A bridge cannot be the prescribed single solution. Keep existing rules/buildings available, eight residents as the initial comparison, and optional growth/ending. Exact layout TBD after legal-ground and equal-investment checks.

The scenario is coupled to a replacement interaction: select a place, see its actual people/use, propose a change, observe the next affected visit. Replace redundant navigation; do not add a monitoring obligation. Household exchange stays secondary until it creates an intelligible result. Revise crop ground through a meaningful competing use, not an instructed one-row reduction.

The art comparison must change **large-scale bank/clearing/woodland/domestic composition**, not only roofs or props. Compare current compact spacing with an editable asymmetric river-facing place. The main menu should ultimately show the selected public village language; archived tall colorful buildings stop defining the product's promise. Archive content can remain for reference without a catalogue-wide art rewrite.

Do not first retune reserves. Quiet is potentially a legitimate payoff. If the coherent inhabited situation still loses the player to an opaque reserve countdown, compare bounded smaller work episodes with current scheduling under equal total food obligations, preserving home rest. Reject busywork. The current scheduler remains the control, not an accepted final pacing design.

### Whole-game disposition

| Area | Retain, change or leave unaccepted |
| --- | --- |
| Public founded Normal | Retain authorship, physical cost and generous supplies; mature interest unaccepted |
| Inhabited Normal/relaxed | Main next comparison; same lived rules, no free-mode semantic mismatch |
| Archived intro/later campaign | Retain geography/recovery fixtures; cut recipes, growth thresholds and service certificates from active progression |
| Legacy Creative / earlier court | Reference only; foodless Creative not equivalent to public relaxed, fresh actual UI unobserved |
| Homes, garden/strip/orchard, grain/oven, fishing, woodland | Distinct potential roles; no proof all earn equal catalogue emphasis. Work slots, maturation, shore/woodland and processing must yield competing choices |
| Sawmill/quarry, pantry/material stores | Retain only through worthwhile construction/access consequences; no automatic resource tier progression |
| Shared places and furnishing | Actual use justifies retention; different scale/siting must justify parallel types, no new satisfaction checklist or Carpenter requirement |
| UX/controls | Direct world action/recovery strong; reason for changing and spatial before/after still weak |
| Village presentation/audio | Domestic cluster improved; landscape/menu direction needs structural comparison. Audio unheard; no replacement approved on source alone |
| Reliability/performance | Tested geometry/material/save invariants credible. Save replacement denial still open, native timing/long sessions not accepted |

### Experiments and failure gates

1. Compare open founding with the viable inhabited two-cluster situation, Normal and relaxed. Without a building sequence, can someone identify a wanted change, explain the alternative forgone and recognize the consequence? Reject the new situation if everyone simply packs food beside homes, follows a prescribed bridge, or cannot find a reason to act. Contented finishing is valid; distinguish it from leaving for lack of an intelligible activity.
2. On identical states compare existing cards with place→use→change→consequence. Require pure cancellation, clear actual-versus-predicted trips, and one visible ordinary-speed aftermath. Reject if it adds obligatory following or changes no one's understanding. No human acceptance is claimed until observed.
3. Compare asymmetric landscape/domestic composition against the current cluster at960/1440 and both views, no HUD labels. A viewer should identify home, cultivation, useful outdoor ground and access while retaining freedom to revise. Reject a beautiful fixed diorama or less readable work. Observe a complete work/meal/rest sequence and actually listen through a music theme and quiet interval before accepting audiovisual life.
4. Only if the above leaves a pacing problem, compare reserve rhythms with unchanged obligations. Measure both work and home/quiet intervals, not output alone. More motion is not a success criterion.

## Tooling decisions

**Accept alongside the next comparison:** a compact causal timeline extending existing mature reports—harvest/reserve transitions, unmet-feeding episodes, home/work/travel intervals and player changes. Estimated half to one day, low maintenance. All reviewers benefit each experiment. Validate that a second reviewer can locate and explain the quiet interval/shortage without reconstructing JSON and source; measure analysis time saved. Do not build a universal simulator.

**Accept use of existing recordings before more capture infrastructure:** paired before/action/ordinary-speed-after references for one field/home and failed-revision recovery. Reuse current bundles, timestamps and audio. Estimated hours to one day for narrow indexing, low maintenance. Validate that another reviewer can explain the actual consequence from the evidence; an uncoached session would add more than another scripted known-success route. The current movie renderer writes1440×900 while requested snapshots are960×640; report dimensions separately, do not call recording throughput native performance.

**Conditional reliability work:** correlate existing native frame/save markers on a reproduced replacement denial or stall. Estimated half to one day, low maintenance; success means attributable frame outliers/failure operation, not more retries. Current-format correctness remains mandatory; saves disposable, no migrations.

**Defer:** ECS, generalized editor/replay/analytics, archive-wide rewrite, new soundtrack framework and catalogue expansion. Tooling-only work does not advance checkpoints.

## Closeout

The next conditional101–105 sequence lives in [NEXT_CHUNKS](NEXT_CHUNKS.md). It is a plan, not work executed under this request. Review100 completes with four fresh roles plus one independent reused visual context. Stop at100 after documentation commit/push. No101 implementation, human enjoyment, listening or native performance acceptance is implied.
