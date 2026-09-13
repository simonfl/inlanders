# Whole-game review — checkpoint 25

September 13, 2026. Fixed source **`bb71c561adb3afc39f4868294abb92a7777b3928`**. Three fresh independent read-only reviewers (`review25_design`, `review25_ux`, `review25_lead`) and a reused independent playtest context (`full19_playtest`) after fresh fourth-agent creation hit the thread limit. Four disciplinary contexts, **not four fresh contexts**. No uncoached human/native play, continuous-motion judgment or actual listening. Periodic count **25**; next full review **30**, including the regular visual/audio role.

## Whole-game verdict and lead decision

**Partially convincing as an inhabited village-arrangement prototype; not yet a convincing sustained campaign or self-sustaining village toy.** The strongest current pleasure is forming a useful place and seeing residents use it. Real production, homes, meals and reversible edits make that more than decorative placement. The new court can acknowledge a real meal, but that does not establish an interesting problem, a visibly better village or voluntary continuation.

Choose **small authored spatial transformations supported by daily life** as the provisional main experience. Offer free editing/watching afterward and a clearly labeled unrestricted start with the same daily-life rules. Do not restore the ten-level certificate campaign or replace the original ambition for skillful, longer scenarios with a tiny finish-button task. This is a reasoned design bet, not observed preference.

Keep the court as an introductory interaction. Stop adding completion conditions to it. The next substantive situation must put two plausible arrangements in tension through existing geography/buildings and show their different lived consequences. Consolidate ordinary entry points around that experience rather than accumulating another showroom of prototypes.

## Independent findings and disagreements

| Role | Verdict and distinctive evidence | Recommendation |
| --- | --- | --- |
| Game design, fresh | Finite arm now establishes inauguration, not transformation quality. Eighteen building types and multiple ordinary experiments are ahead of evidence for depth. Current test can finish without a home move. | One finite spatial-restoration mainline plus free continuation; preserve geography and real investment alternatives, cut mandatory recipes/service certificates. |
| UX/onboarding, fresh | At 960, finite New is visible while open New falls below the choice-page fold. Free-mode prose directs the same meal/home action as the finite brief. Successful eastern commons is partly hidden by a cottage/garden crowd. Dense labels overlap. | Equal immediate choice visibility, neutral free guidance, fewer primary entries and a visible world consequence. More success prose is insufficient. |
| Development lead, fresh | Current saves, separate slots and continuing simulation are soundly scoped. Commons clipping cache omits building positions/rotation even though clipping depends on footprints; same-count relocation can retain old geometry. | Targeted rendering regression/correction, no cache framework. Finite transformation remains provisional; test spatial alternatives rather than stricter gates. |
| Playtest evidence audit, reused | The UI script moves a western home near (-7,2), then places commons at (10,4). Those operations succeed independently; it does not prove the move enabled use. First diner at about110.1s, only18s after the start. | Reject treating this as the requested longer challenge. Observe an intended change, its understood consequence and voluntary next action. No scripted route can establish preference. |

All four favor finite authored transformation as the next bet; none claims acceptance. UX treats the 960 drawer as a major obstruction; playtest finds it readable and improved over older management prose. These are compatible observations: action text can be readable while its subject is obscured. Retain closable local guidance, avoid expanding it, and judge the village with panels closed.

## Whole-project decisions

- **Campaign:** preserve river/lake/quarry/woodland geography, retire repeated delivery recipes and rolling service assessments from the product spine. The court is introductory. Do not add five new projects before one deeper spatial situation works.
- **Economy/buildings:** retain physical collection, construction and shared work. Garden versus bread processing, cottage versus lodge, crossing versus local food, and woodland versus habitat offer potential investment/space tradeoffs. Catalogue prices are not balance evidence. Keep all buildings available; stop making objectives to justify every item.
- **Needs:** keep eating, home visits and recreation as understandable daily life. De-emphasize comfort, orchard and three civic capacities until they supply distinct decisions. More need meters would broaden the system before the experience is established.
- **Normal/Creative:** one supported daily-life model, with explicit construction/hunger relaxations. Current court supports that. Historical foodless Creative and alternative worker rules remain development controls, not equal primary games.
- **Controls/onboarding:** retain direct home moves, actual seat previews and current save/undo recovery. Correct unfair choice presentation and free-mode priming. Consolidate experiment taxonomy out of the ordinary route after this review; do not ask players to choose our architecture or historical rules.
- **Presentation/audio:** retain warm materials, timber architecture, recognizable bakery/civic forms and landscape detail for their visible strengths. Court and dense evidence still show convergence, small residents and overlapping labels. Redesign public-space hierarchy before indiscriminate mesh detail. Audio remains unassessed, neither accepted nor rejected.
- **Reliability/performance:** retain Godot/C#, current-format saves and provenance checks. Investigate the concrete clipping invalidation finding. No engine/ECS rewrite, migrations or broad mode framework. Short normal-process observations do not certify frame performance or long-session reliability.

## Next direction and falsification

Consolidate to one obvious introductory project and an unrestricted start, with earlier experiments under Earlier prototypes. Then reuse an inherited site for a **two-arrangement spatial test**: reclaim a crowded food-side court versus establish a different riverside neighborhood, or crossing-dependent homes versus allocating land to local food. This is a design brief, not a commitment to a particular recipe. Choose the geography only after demonstrating both layouts are viable and visibly distinct.

Use the existing buildings and terrain to create the compromise; no mandatory move counts, attendance streaks, timers, arbitrary reserve quotas or catalogue locks. Preserve a recoverable weak arrangement. Record a player's intended change before intervention and whether the next ordinary journey makes the consequence understandable without Economy exposition.

Reject the finite direction if a first-legal-patch click remains equivalent to redesign, the answer is always another garden, players follow prescribed coordinates, or consequences only appear in metrics. If logistics repeatedly fails to produce a perceptible reward, explicitly choose a forgiving composition game rather than preserving it through more indicators. Reject the open-toy bet if players require an assignment to start or continue. Missing human observation remains a gap, not an automatic veto or permission to claim enjoyment.

## Tooling choices

**Accept:** a bounded move-after-commons rendering regression if it reproduces the lead's finding. Beneficiary: presentation developers during free edits. Estimate hours, low upkeep; validate current footprint clipping and unchanged simulation. No screenshot-golden platform.

**Use, do not expand:** existing before/action/after capture, isolated save sessions and observation sheet. Beneficiaries: all reviewers and the user on direction decisions. Estimate under half a day to prepare a short consumed session, minimal maintenance. Success means somebody actually recognizes a consequence and supplies a preference, not another counter. Measure time to reviewable evidence.

**Defer:** finer fixture caching, deterministic replay, universal editors, telemetry framework and ECS. Dense regeneration was expensive after source invalidation, but one such run does not establish repeated savings. Measure repeated cost before complicating cache boundaries. Capture cost is not game performance.

## Fixed evidence and limits

Source fingerprint `156C1CB4642A04D955C0DCB59082F9E1C49FF22CADF943EB7B35F265DBC8A7DA`; game assembly `0D0CD09B05F5547B014114885053A603846B6F6E3DA4A62BF24B96A1DC3E8094`.

- Court960: `20260913-190012-137-court-experience-c2737a`, precommit dirty `d60cf6d` metadata but matching exact source/assembly.
- Court1440: `20260913-190104-386-court-experience-56ab54`, clean fixed commit. Both scripted journeys pass actual placement, eaten meal, ending/watch/reopen, Continue/separate saves/F5/F9/reset.
- Dense1440: `20260913-190303-691-dense-66cadb`, 32 residents/30 buildings;2623.5085→2626.5115 simulated seconds at1× in3.4589 wall seconds.
- River960: `20260913-190328-662-river-fd63a9`,8 residents;0→3.10 simulated seconds at3× in1.2622 wall seconds. Root and playtest role inspected fresh river; other roles' river coverage was historical.
- Focused court-experience and existing non-court commons checks pass; details in [F30d3 delivery](COURT_PROJECT_F30D3.md).

Fresh stills/logs and source support the observations above. Normal-process intervals are before/after snapshots, not motion recordings or performance certification; some preparation/testing overlapped. Older campaign/menu/audio/performance reports are context. No new full campaign run, ordinary player mistake/recovery session, uncoached choice, listening or long-duration stability result. Review completion means four disciplinary assessments were synthesized, not that unavailable playtesting occurred.

## Review-directed corrections

The lead's clipping finding was reproduced in `20260913-190809-349-court-experience-2920d1`: moving the nearest home after placing the commons, preserving building count, left `_commonsGroundExcluded` based on the previous footprint. The targeted rendered check failed. Landscape rebuilds now invalidate the separate commons visual cache; this covers the relocation rebuild and terrain-refresh path without a new caching framework. The same check then passed in final960 `20260913-191008-722-court-experience-a9c99a`. This is a current-footprint rendering regression, not a claim of broad terrain visual acceptance.

The choice page now puts both starts before the longer descriptions; [final960 view](images/court-project-choice-960.png) shows both without scrolling. Open-mode text is neutral, while its meal-place tool remains available. The finite arm retains its explicit project. These corrections improve comparison fairness; they do not validate preference. The [fixed-review meal view](images/court-project-used-1440.png) preserves the criticized partly-obscured payoff as evidence, not a visual acceptance image.

These fixes belong to checkpoint25 and do not advance its count. Entry-point consolidation and the deeper spatial situation are queued in [NEXT_CHUNKS](NEXT_CHUNKS.md); no further feature was implemented ahead of this review synthesis.

Final correction validation: both960 and1440 full court journeys passed, including the reproduced move-after-commons regression and open-choice/open-guidance checks. Final1440 run `20260913-191135-110-court-experience-98491a`; source `E1BE66151C53A290046F361057A237FAAEB1F8AB63DDD80C4586A5190340E8ED`, game assembly `6BB2ED67707B0EF5875A6A1C681CB8BE3F7D703E0A0828AC0DEEB1F16A355A61`. This post-review build differs from the fixed reviewed assembly above. Build reports zero warnings/errors; `git diff --check` passes. No simulation rule changed in these final presentation corrections.
