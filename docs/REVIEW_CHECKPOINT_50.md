# Whole-game review 50 — September 20, 2026

Fixed pushed source/build: `b2309fd1526a028d43daaa7a18422d9afc1ac0af`. Third requested periodic review, following40 and45. Playable count50. Four independent disciplinary verdicts and a dependent playtest-evidence pass are complete. This finishes the requested three-review run; do not start the next feature.

## Verdict and direction

**Partially convincing.** F36 changes the payoff of a working village: shared labor can stop generating surplus, people use household ground and ordinary meal places, and world cards connect buildings to real activity. Keep those changes. The evidence does not demonstrate longer, skillful play. Sixty simulated minutes of stability is not an hour of meaningful decisions; more roofs and bread are not a satisfying arc.

The next product experiment must fairly test a sustained transformation situation against a relaxed version of the same place. Do not default to more domestic props, interface polish or new food chains. The user previously asked for deeper, longer levels; quiet composition has not won that comparison and must not silently replace that ambition.

## Independent verdicts and disagreement

- **Design (`review45_design`, reused):** partially convincing. Keep physical activity and provisioning, challenge setup-heavy decisions. Prototype an established hamlet where convenient ground is scarce, living space competes with production, retained woodland competes with expansion and remote ground needs access. Each intervention must change what is sensible next. Grain's fixed-producer capacity result needs adaptive alternatives.
- **UX (`bush_move_review`, reused):** better local causality, unresolved overall purpose. Prefer a substantial normal-rule transformation episode over another cycle of cards/quiet activity. Public normal/Free differences undermine learning. Retracts review45's implication that deliberately expanded policy screenshots were the default inspector; folding already existed.
- **Development lead (`review15_lead`, reused):** more coherent, still partially convincing. Favors a persistent village with voluntary transformations and an optional farewell; first compare normal/relaxed versions of the same rules and equal-investment food choices. Confirms farmer anchor, explicit ten-suite coverage and recipe accounting. Identifies domestic-reader mismatch, evidence overwrite and moving card-side issues.
- **Visual/audio (`cultivated_visual`, reused):** peaceful toy is more convincing than a longer skillful game. Keep homes/commons use and warm materials; redesign terrain, working ground and outdoor relationships as a whole. Free remains the strongest composed place. No motion or listening acceptance.

The disagreement is productive: design/UX/visual want a constrained later situation now; development lead favors testing the persistent village and aligning public modes before another scenario branch. **Lead synthesis: one bounded transformation prototype, with normal and relaxed variants of identical geometry and rules.** Reuse scenario infrastructure; keep it a development comparison until it earns promotion. This tests constraint-driven decisions and mode consistency together without promising a campaign series. A finite expressive toy remains a valid alternative, but is not selected as the default merely because its tests pass.

The dependent playtest-evidence pass traces an attractive but distant shared meal place: placement allows reachable ground, while real use requires nearby food. Preview warnings, rearrangement and removal exist and are scripted-tested; unaided discovery is not observed. Pause/recovery similarly works under scripted input. Opening construction is supplied by simulation commands, so native finish/menu checks do not establish player learning. The next experiment must include an imperfect player-created plan and recovery that changes subsequent decisions.

## Retain, challenge, cut

Retain shared labor, actual goods/meals, meaningful occupied fields, forgiving movement, optional arrivals, current domestic use and direct resident/workplace reading. Their contribution is understandable, reversible experimentation with a lived place. Keep food/rest/leisure; no additional need meters.

Retain the eight-portion normal bread batch provisionally. Its21-cell/12-log setup sustains the documented16 residents while unchanged two-garden/one-dock configurations require expansion. This is capacity evidence. Compare against adaptive three/four-garden and mixed shore/garden plans at comparable investment; do not claim optimality. Orchard/hunting, quarry/hall and overlapping communal systems still need reasons to matter beyond catalogue variety.

Keep the easy opening and old campaign accessible. The opening is an introduction; archived service certificates are not the progression template. Extract useful geography/woodland/finite-material situations from that archive without reviving building recipes or attendance quotas. Freeze new catalogue entries, needs, milling and another series of tutorial-like scenarios.

Normal/Free should ultimately share current building and domestic rules, differing only in deliberate construction/hunger constraints. Their current recipe/scheduler differences are temporary experimental history, not desirable public complexity. Test consolidation before expanding mode dispatch.

## Concrete closeout corrections

These do not advance the playable count:

1. Replace resident reader status-string branching with semantic available-at-home state; furnished household work must expose its actual home and activity, with real meal requests retaining precedence.
2. Make daily-food interpretation acknowledge purposeful provisioning. Low recent deliveries alone must not imply shortage when residents are fed and reserves are adequate.
3. Preserve seven-arm comparison reports when regenerating one arm; include run identity and selected-arm scope. Label F36a's earlier-recipe measurements historical, and distinguish current results.
4. Choose the workplace card's screen side when it opens, then preserve it during camera movement and constrain it on resize.

All four corrections implemented after synthesis, with no playable increment. Available-at-home reading uses actual shared-worker/task/route/home proximity, including furnished yards; claimed meals retain precedence. Food guidance distinguishes replenishment, ready reserves, low stores and missed meals. Workplace cards retain their opening side during panning and clamp to the viewport. Full and focused comparison reports have separate outputs and assembly/scope/timestamp manifests. Historical provisioning measurements are labeled with their earlier recipe.

Closeout validation: clean game/test builds, zero warnings/errors; all ten current-experience suites pass on assembly `e99ba199-8761-4c63-af48-7eae09d67cc9`, including new domestic-reader purity/meal-precedence checks and existing unfurnished-home/work-interruption checks. The broad default regression passed the fixed review build before these bounded corrections. Native integrated probes pass at960 (`20260920-051431-195-working-village-83613e`) and1440 (`20260920-051431-358-working-village-4c6888`), including card stability across camera pans, held pause/resume, actual worker/collector following, furnishing and current-save recovery. The960 workplace still was inspected. These are post-review dirty-candidate validation, not a replacement fixed-build independent review. Full seven-arm livelihood rerun passes and reproduces the documented current recipe results.

## Tooling decision

Accept isolated comparison outputs and the semantic-state regression: bounded changes, repeated benefit to every comparison/review, low maintenance. Retain the current ten-suite profile (about78s in this run), broad default suite and focused land shortcut. No new runner/framework. Reuse snapshots and recording tools for paired resident days and decision traces; record actual interventions, reasons and consequences, not only throughput. Scenario descriptors wait until the shared normal/relaxed prototype reveals the required scope. No speculative rendering/architecture rewrite.

## Evidence, staffing and limits

Agent limits rejected a fresh playtest agent and follow-ups to three older independent contexts. Four distinct reused contexts supplied independent design, UX, lead and visual/audio verdicts, without reading one another's review50 conclusions. The UX context performed the additional playtest-evidence discipline separately; it is **not an independent fifth reviewer**. This staffing limitation must not be presented as five independent reviewers or a human playtest.

Clean fixed-build captures:

- `20260920-050158-992-farmstead-6072e9`: actual normal opening and full scripted controls at1440.
- `20260920-050158-383-working-village-792d69`: imported sixty-minute16-person **RiverFarmstead-derived** grain village; launcher label says working-village, but the state has no inlet. Ordinary construction/invitations, not uncoached play.
- `20260920-050200-731-court-experience-ef6a80`:960 menu, Free/shared life and controls;0014 is an explicitly expanded inspector.
- `20260920-050305-084-dense-fd9657`:32-person archived finale-derived stress fixture, not naturally grown current play.

Candidate integrated960/1440 probes `20260920-045811-151-working-village-a25dbd` and `20260920-045848-282-working-village-594bdc` show commons, workplace card, ordinary furnishing, current saves and actual worker/collector following. These are labeled candidate evidence, not clean-commit captures. Full default and separate ten-suite current-experience checks passed the final assembly. Source/reports cover all buildings and needs, normal/Free, archived campaign, controls, presentation and reliability.

No uncoached human session, full archived native traversal, continuous-motion viewing, listening or new long-session render-performance assessment occurred. Audio's possible field-position mismatch remains an audition target, not a heard defect. Roof dominance, tiny domestic gestures and dense label congestion remain visible limitations. Human preference and sustained enjoyment are unaccepted.

Focused growth rerun also passes; the full seven-arm report hash remains unchanged and the focused manifest identifies its own arm and assembly. All requested review syntheses are recorded; stop at playable50.
