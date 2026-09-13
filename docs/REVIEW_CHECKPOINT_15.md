# Checkpoint 15 — choose a village worth inhabiting

September 13, 2026. **Verdict: partially convincing as a village-making game, unproven as a sustained management campaign.** The last five outcomes make local life and controls more concrete, but do not establish the missing reason to keep reshaping a successful village. Do not convert this review into another sequence of thresholds and explanatory text.

## Frozen evidence and review integrity

Source: `2f2ebd792ae271e5fc0f7f2e4daac2a1bba4cfd5`.

- Game DLL SHA256: `DCC96B8068CF52D1855FFA0649CDA1899256E10ACEEE24A615DE64CB739586B2`.
- Test DLL: `09B1A26AC618D8F49A19FF702DF0CA3740C0885B62749812D91D7BE5F9A38D06`.
- Source fingerprint: `58114D392285F4C2487326D581E9595B23BAA5E89106D624D44DD8BABB54DA2E`.

[Disciplinary reports](REVIEW_CHECKPOINT_15_ROLES.md) contain three fresh independent reviewers (design, UX, lead), plus reused-context playtest-evidence and visual/audio passes. Fresh fourth-agent creation and historical playtest reuse hit the thread limit. **This does not satisfy the intended four independent core contexts.** Record the shortfall explicitly. Available native APIs were disabled; no native play or listening occurred. Review synthesis is complete with those limitations; do not backdate missing play or reviewer independence. Next periodic checkpoint is 20; arrange the fresh playtest role when available, and retain presentation-triggered reviews.

Representative fresh fixed-build runs under `artifacts/review/runs/`:

| Area | Run / result |
| --- | --- |
| Opening campaign, 960 | `20260913-125820-562-opening-f7a28a`; paused scene/basic controls pass |
| River campaign, 1440 | `20260913-125843-695-river-ef807d`; paused scene/basic controls pass |
| Dense 32-resident village, 1440 | `20260913-125834-963-dense-7f6d36`; rendered, 14.68s fixture preparation |
| Ordinary 20-resident finale, 1440 | `20260913-130145-637-ordinary-d36021`; 6.10 simulated seconds at 3x in 2.12 wall seconds; partial concurrent capture, not an isolated benchmark or motion recording |
| Gathering, 960 | `20260913-125901-152-gathering-5d888f`; seating/completion captured, later leave/resume transition **failed** |
| Current start/menu/resume, 960 | `20260913-130140-194-neighborhood-workplace-food-17c1e5`; complete scripted flow passes separately |
| Menu/Creative/recovery | `artifacts/checkpoint15-menu-full.stdout.log` and `checkpoint15-menu.stdout.log`; full mouse smoke and keyboard 960/1440 pass, including Creative arrangement, separate saves and failures |

Current food/policy 960/1440 runs `20260913-125504-830-neighborhood-working-village-797fec` and `20260913-125522-798-neighborhood-working-village-7344ce` precede the commit but have the same code/build fingerprint. Their manifests retain dirty/precommit provenance. Tests `--local-reserve` and `--workplace-food-all` passed before freezing. No new gameplay code changed during review.

Selected stills are committed for portable reference: [dense village](review15-dense.png), [gathering](review15-gathering.png), [food view](review15-food-view.png), [Creative](review15-creative.png). Still images do not establish routine appeal, audio quality, pacing or human satisfaction. Late campaign, economy/needs breadth and Creative were covered through source, representative stills and reported tests, not a fresh uncoached playthrough of every level.

## What the game offers, and where it loses purpose

Its strongest present pleasure is choosing where a village works and watching real people carry goods, cross water, eat and occupy what was built. Shared work and recoverable shortages support peaceful experimentation. Those are positive reasons to keep the physical simulation.

The original campaign often substitutes prescribed chains and recent-service certificates for a visible ambition. The selected meadow improves preparation and causality but becomes stable after a competent food investment. A one-shot meal produces a clear ring of diners, then leaves no continuing place. Retention changes shipments without demonstrating a valued dilemma in its matched fixture. More numbers, events or food requirements will not automatically join these into a satisfying game.

The village has appealing individual buildings and a coherent warm palette. Its life is harder to read than its roofs; dense props and tiny labels compete with routines. The inspector increasingly asks the player to understand implementation policies before acting. This is a product/interaction problem, not a missing-prop list.

## Chosen direction

**Test a persistent inhabited village that the player reshapes, with ordinary use of player-created places as the payoff.** Compare that experience fairly with the current provision/arrival/one-shot event and an unchanged viable village. The next major slice is a recurring small-group commons, not a second attendance certificate. It must create a place recognizable when empty and visibly used during normal life, without holding everyone for one late resident.

The interaction changes with the game: primary selection shows current work, local food, workers and the relevant action. Output targets, numeric retention and removal move behind deliberate secondary controls. This removes administrative emphasis while preserving the all-buildings-available constraint. The test is whether players choose, understand and revise a place, not whether a report records more visits.

Choose a finite settlement ending if recurring life fails to earn voluntary interest. A deeper logistics campaign remains an alternative, but would need a new demonstrated geographic/production dilemma; do not pretend the reserve or forced commons experiments already supplied it. The near/far compulsory land-sacrifice counterexample remains binding evidence against that particular scenario, not against all future geography.

## Keep, cut, replace

- **Keep** local food/grain, actual construction/transport, shared workers, direct selection, crossings, homes and peaceful recovery: they tie arrangement to visible consequences.
- **Replace** primary inspector hierarchy. Stop presenting global output, local retention and pantry targets as equal beginner decisions. Keep exact retention as advanced comparison access for now; no tuning requirement.
- **Replace** the five prescribed tutorial levels as the future primary journey with one guided opening. Preserve old campaigns as benchmarks and reuse their river/lake/stone/woodland material. Do not expand the existing rolling assessment structure.
- **Challenge** venue and comfort layering. Retain buildings as expressive spatial alternatives; no requirement to collect them, no new need meter to justify them. Stop new producers/resources/needs during this experiment.
- **Keep Creative**, but resolve its rule divergence before presenting it as the relaxed version of the chosen game. Broader Creative feature expansion remains parked.
- **Prefer the neighborhood landscape as the next comparison candidate**, retaining the board as a boundary-clarity control. No universal art rollout or new ornament/music pass before ordinary motion, placement comprehension and listening evidence.

Disagreements matter: the lead favors removing manual retention from primary play; design/UX favor advanced retention while testing its value. Choose advanced access for the comparison, not deletion based on one abundant fixture. All agree it has not earned primary prominence. The designer favors recurring social place; the lead favors broader village transformation. Combine them narrowly: the recurring place is the first falsifiable transformation, not a commitment to build every proposed project.

## Reliability finding that comes first

The frozen gathering run reached completion, then a hidden Economy control exposed an invalid post-resume UI state. GatheringReview had checked only unchanged JSON and printed PASS. The old world survives in memory when entry fails, so that check is insufficient. A menu error or a focus/scroll click issue could explain it; the exact cause is unrecorded. Separate menu flows pass, which does not invalidate the failure.

First capture transition state/error/focus/geometry and repair the demonstrated cause. Require active HUD, paused state, expected scenario and exact saved state after resume. Keep the failed artifact. Do not paper over it with a delay or broaden a PASS message. This is a bounded reliability/evidence chunk, not a reason to abandon the product redesign or inflate the playable count.

## Tooling decisions

**Accept now:** extend the existing transition diagnostic and semantic evidence index, rather than create another runner. Every reviewer benefits; estimated hours to half a day, low upkeep. Validate the exact intermittent transition and whether an unfamiliar reviewer can find first-entry Goals, production controls, failure, Creative and comparison states without source search.

**Accept alongside the prototype:** two one-command playable comparison states and a short screen/audio audition procedure using existing launch/record facilities. Estimated half day, low upkeep. Success requires usable ordinary interaction/listening evidence; generating a recording or invoking a script is not acceptance. Human preference remains unknown until actually observed.

**Bounded measurement only:** food-view off/on/hidden/camera-moving timings in ordinary/dense worlds; suppress unchanged work only if measured. Existing per-frame allocations/visibility churn are reasons to investigate, not proof of a stall.

**Defer:** fixture dependency graph, generic rules engine, ECS, asset migration and broad optimization. Track fixture preparation over two slices; consider a simple simulation/generator versus presentation fingerprint only if its repeated savings outweigh stale-evidence risk. After choosing the primary scenario, a small explicit scenario identity/factory can replace actual fragile reset branches.

[The next chunks](NEXT_CHUNKS.md) implement this decision, with falsification and count rules. This review adds no playable checkpoint. Stop at this review as requested.
