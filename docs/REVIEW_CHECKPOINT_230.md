# Whole-game review — checkpoint 230

Completed October 18, 2026 (October 19 UTC), before implementing231. Fixed commit: `153c67f7b61c2388c6b97b8330613ffc5f85c81e`. Five fresh independent read-only reviewers: review230_design, review230_ux, review230_playtest, review230_lead and review230_visual. No reviewer received another role's verdict before submitting their own. This is the scheduled ten-outcome review; documentation does not advance the count. Next periodic review240, if authorized.

## Whole-game verdict and decision

**All five: partially convincing.** Practical changes to an inhabited settlement, automatic labor and visible meals support a plausible pleasure. Larger reserves, finishing committed food, area renewal and grouped buildings address human feedback. Restoration is a justified structural alternative to growth. We have demonstrated short solvable situations, not sustained strategic decisions or enjoyment.

The strongest objection is that the intermediate island gates a preselected plan rather than demonstrably causing reconsideration. All six scripted branches choose their strategy before play. Normal first access occurs around47s; Relaxed access is instant. Completion at121–198 simulated seconds does not establish a substantial campaign. Neither more quotas nor extra compulsory buildings would resolve this.

**Choose231: simplify the primary challenge set to two directly named choices, newcomer introduction and After the flood.** Move the overlapping far-bank newcomer variation into developer comparisons. Retain the map/start-condition preview, Normal/Relaxed and ordinary current-save continuation. Four reviewers favor this bounded cut; UX offers it as the fallback to a larger world-orientation interaction. It directly addresses the user's duplicate-level confusion without pretending to solve campaign depth.

Keep the broader catalogue available, but freeze new compulsory needs, buildings and chapters pending evidence. Consider a smaller game of authored restoration commissions and optional continuation; a household/farmstead scale remains a credible alternative if village systems obscure agency.

## Independent findings and disagreements

| Role | Independent assessment | Proposed next outcome |
| --- | --- | --- |
| Game designer | Keep physical work and non-growing restoration. Free relocation carries crops/goods across inaccessible water, weakening salvage versus new cultivation. Current catalogue breadth is not proof every chain deserves an objective. | Two public challenges; compare land-bound crops later. |
| UX/onboarding | Management contract is clearer, but meaning remains panel-led. Displaced residents and broken routes are numbers/prose rather than readable world problems. Inventory accounting still requires interpretation. | A dismissible world survey framing homes/island/fields, with direct inspection; smaller honest chooser if that scope is too large. |
| Playtest | Scripted viability is not ordinary play. Native flood flow jumps from entry/save to an adopted completed fixture. Relaxed delayed island finishes sooner than immediate island, so delay is not established pressure. | Direct two-choice selection; observe a self-directed recovery before more scenarios. |
| Development lead | Architecture is workable, but scenario booleans should not multiply. Current-experience runner omits recent automatic-work/reserve/flood suites. Slow blocked-world model runs warrant attribution, not speculative optimization. | Reproduce a player-facing blocked-state slowdown, fix if verified; otherwise a genuine recovery interaction. |
| Visual/audio | Low river village is readable and warm, but two ruler-straight channels look like irrigation trenches. Scene composition does not tell a flood story. No listening verdict. | Direct two-choice invitation, then matched whole-scene river/flood composition. |

Disagreement is about priority, not demonstrated depth. Choose the low-risk product cut now because human duplicate-level evidence is direct. Queue blocked-state performance before broad content work; source hypotheses about repeated dispatch/access are not an established cause. A world survey should follow a readable landscape rather than become another explanatory panel.

## Coverage and justified retention

- **Campaign:** retain a short newcomer introduction and experimental restoration. Demote duplicate contract; no substantive campaign claim. Alternatives must change choices and visible consequences rather than quotas.
- **Economy/buildings/needs:** retain automatic work,32-log reserve and24 plank/stone buffers plus orders, four edible portions per resident, bounded grain and completion of committed crops/bread. Grouping and grove renewal have positive human feedback. Quarry, processing, orchard, hunting and optional comfort remain available but unproven as interesting tradeoffs. No mandatory new needs.
- **Normal, Relaxed, Creative:** Normal materials/work matter; Relaxed keeps work/meals with free construction and no hunger penalty, removing the staged bridge gate. Archived Creative is a distinct earlier ruleset, not evidence for Relaxed. No fresh full free-play or Creative session in this review.
- **Controls/onboarding:** direct recovery, follow and supply actions are useful; two visible scenario choices should replace cycling. Large goal panels still compete with the place. Ordinary unaided navigation remains unobserved.
- **Village presentation/audio:** keep low homes, warm wood/earth palette, crops and cargo as the control. Compare a coherent irregular river, lost connection and household-to-land relationship, holding economy/mechanics constant. Do not replace every asset or add props as a substitute. Audio source includes spatial effects, ambience and music controls, but sound quality and repetition were not auditioned.
- **Reliability/performance:** exact-save branches and native flows are bounded positive evidence. No migration needed. No newly established corruption/crash. Blocked-state cost, sustained/dense performance and complete catalogue balance remain unresolved.

## Evidence and limits

Fixed game assembly SHA256 `DD705D83879A93EE3A51835813E59A01359272E58A3AC2AE176172BFB6A73FE3`; test assembly `126D6605A29B65553B4F977D1CBB086472FBF6BC52AB0C9C354CF11AAFF22617`; source fingerprint `0A210D35200AFD42EEF4ACF8D79E5895A2BFA82CF7D737C5EA9CA99EE28112CB`.

- `artifacts/restoration/island-comparison.json`: six Normal/Relaxed bridge/island/delayed-island branches,360s horizon,12 housed/fed, exact saved continuation. Normal completions126.6/128.3/198.2s; Relaxed124.3/152.0/120.9s. Decisions scripted up front; no uncoached play.
- Checkpoint228's600s no-intervention comparison used the earlier single-channel geography. It is historical evidence of housing/food failure, not a new two-channel control. Actual quarry output was not freshly exercised by222's queued-demand checks.
- Fixed230 focused regression: timber-reserve, material-reserves, food-finish, grain-reserve, automatic-work, challenge-status, challenge-continuation and newcomer-challenge all pass (`artifacts/230-regression.json`,83.2 summed wall seconds). The status case used an existing220 arrival fixture. This is not a full current-experience-suite run.
- Native230 entry/preview/housing/save/completed continuation passed.227 covers newcomer/crossing/free-play/follow flows;225 covers reserve/manual-cap actions. Native flood adopts completed simulation fixtures; it does not prove ordinary end-to-end recovery.
- Fresh fixed230 capture `artifacts/review/runs/20261019-025611-585-river-livelihood-d2e1a7`: explicit imported Normal island-complete state,128.30→136.30 simulation seconds over8.04 wall seconds at1x, before/after images. This is engine advancement, not viewed motion or human play. Menu stills predate the final opaque-panel correction; source/build contain the correction.
- Reviewers inspected representative restoration/menu/reserve and historical dense stills. No new dense operation, full catalogue session, continuous-motion assessment, listening or human enjoyment evidence. Historical220 dense capture is a stress case, not the target aesthetic.

## Ordered follow-up and tooling investment

1. Deliver231 two direct public choices; verify960/1440, Normal/Relaxed, starts/resumes, archive and Continue. Stop this authorization at231.
2. On further authorization, reproduce blocked-restoration cost at1x/3x with matching no-intervention/restored-access states. Measure before narrowing retries or caching; test invalidation after crossings/moves/clearing/pauses. Instrumentation alone is not a playable outcome.
3. Compare existing free field relocation with crops remaining tied to old land and new cultivation restarting. Preserve easy preview/cancel and housing moves. Reject if it only adds waiting/punishes rearrangement; seek a legible salvage-versus-restart choice.
4. Compare one coherent flood/river-farmstead composition with the current board, same economy and viable interventions. A viewer should identify disruption and improvement without prose. Add a world survey only if it improves agency rather than prescribes bridges.
5. Ordinary player session: choose a livelihood, observe consequences, reconsider a plausible plan, explain who benefited. If play remains two homes plus food then waiting, reconsider the restoration loop or reduce scale before extending the campaign.

Supporting tooling, not checkpoints: update the current test registry to include recent suites and explicit fixture prerequisites (e.g. observation before challenge-status), short/full groups and timings. Beneficiaries are implementers/reviewers; expected saving is fewer omitted checks and manual fixture searches, not an unmeasured speed claim. Bound to existing runner, estimated several hours; validate from missing artifacts and reject unknown selections. Record a compact intervention/shortage/meal/recovery timeline in existing comparison reports; another reviewer must reconstruct the decision from that report. Use existing capture/audio tools for a matched60–90s1x recovery and short dense excerpt; under half-day setup, negligible framework maintenance. No new replay framework, ECS, universal scenario system or save migration.

## Post-review delivery231

Two direct choices delivered; far-bank comparison moved to developer archive. Zero-warning game/test build. Native231 flood probe passed960/1440 preview, mode toggle, compact start visibility, entry, housing, save and completed continuation; newcomer probe passed ordinary entry/Continue, archived repair/crossing delivery, free play and resident follow. Inspected final960 screenshot confirms opaque preview and both choices/start visible. No simulation changes after fixed230 regression. Ten-outcome authorization ends231.
