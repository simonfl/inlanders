# F30d1 — daily life, partial presentation gains

September 13, 2026. One playable outcome, checkpoint **23**. Periodic whole-game review remains **25**; this substantial presentation change received an independent whole-game visual/audio review.

## Result and decision

Court play now starts closer, with a higher viewing angle. Actual eaters remove their hats, sit at a small board holding their real carried portion and use a distinct eating pose. Resting residents have a reclined, hatless chair silhouette. Planting includes a ground-contact digging phase. These are rendering changes: food, routes, jobs, task durations and save format are unchanged. Ordinary non-court presentation retains its existing behavior.

Retain these modest gains provisionally. They make more ground and occupied places visible, especially at 1440 from the opposite side. They do **not** establish that players recognize all activities or want to keep watching. Central meals still merge with storage and other residents at 960. Direct inspection remains useful.

The comparison also tested lower cottage roofs and conditional house translucency. The reviewer and lead rejected both: no isolated lower-roof benefit, and no demonstrated useful reveal despite positive internal veil counts. Restore roof massing and remove the ray/material/cache machinery rather than maintain an unproven effect. Candidate commit `18dfe94`; review-directed final implementation `fa2dcb3`.

[Final court view](images/court-daily-life-1440.png) · [matched control](images/court-daily-life-control-1440.png). Same actual meal moment, opposite camera at 1440; the larger image shows the gain more clearly than the crowded 960 view.

## Independent whole-game review

Reviewer: fresh independent read-only `court_daily_life_review`, fixed source `18dfe94`. The reviewer independently recomputed source fingerprint `8BB46386E110CD772052FC7DF72858D34796B4E13CCF47DDE81F33DE6B5E19C8`; assembly `019E7A73EC83006283E51F5EB17093F02EBCE0C83A35C46177F5CC74E04F3D98`. Initial request commit/dirty fields reflect precommit capture timing; the exact source matches the fixed review. No claim of a separate five-role periodic review.

Whole-game verdict: increasingly coherent arrangement prototype, without demonstrated sustained play or reliably readable daily life. Advance the experience comparison; do not begin another indefinite visual-polish sequence. Lead agrees; no material disagreement remains. Camera, hatless breaks, boards and poses are retained as a coherent provisional language, not separately proven micro-features. The same reviewer inspected the final 1440 paired stills after the removals: roof character and the modest gain remain, with no visible regression in that comparison; central crowding and enjoyment remain unvalidated.

- Campaign: one opening and useful geography do not yet constitute a compelling deeper campaign. Repeated welcome/reserve/service-window recipes remain a risk. Preserve river/lake/quarry/woodland material; freeze new levels.
- Economy, buildings and needs: actual meals, shared work, construction and production routes are useful foundations. Do not preserve every building/need merely because it exists or add civic needs to simulate depth.
- Normal and Creative: free editing with real residents and forgiving hunger is a useful comparison vehicle. An edit must visibly change life. Open-ended continuation remains an unproven design bet.
- Controls/onboarding: closed cards recover world space. The 960 HUD lacks the persistent camera hints present at 1440; record this limitation, but do not add panels as a substitute for visible activity. No fresh uncoached discovery evidence.
- Visual direction: retain warm materials, timber buildings, distinct bakery, crops and surrounding landscape. Historical dense imagery remains crowded/label-heavy. Central convergence is also a layout/rhythm issue, not simply poor meshes.
- Audio: unassessed; no listening. No justified sound-quality, repetition or new-content conclusion.
- Reliability/performance: scripted checks and matched state support correctness, not native frame performance, animation quality or enjoyment. No fresh full campaign replay or ordinary/dense performance acceptance.

Fresh image inspection covered 960 turn0 meal, 960 turn2 rest, 1440 turn0 work and 1440 turn2 meal. Historical title, commons, dense and river images and [checkpoint-20](REVIEW_CHECKPOINT_20.md)/[checkpoint-19](WHOLE_GAME_REVIEW_19.md) reports supplied the broader context; those are not new playthroughs.

## Evidence and tooling

`./ReviewCourt.ps1` runs real sixteen-resident arranged-court activities and emits `artifacts/court-life-comparison.md`. Every work/meal/rest pair holds authoritative state and resident heading constant across candidate/control rendering. It checks common source/build identity, identical world hashes and passing scripted controls. Boards/chair backs/tools follow actual tasks, and rendering changes preserve current saves. No forced fake activities are injected.

Initial four-view comparison: 127.53 seconds total capture/index workflow time, **not game frame performance**. Runs under `artifacts/review/runs/`:

| View | Initial reviewed candidate run |
| --- | --- |
| 960 / turn 0 | `20260913-182711-802-court-life-2870a1` |
| 960 / turn 2 | `20260913-182742-250-court-life-b61311` |
| 1440 / turn 0 | `20260913-182811-236-court-life-cfde47` |
| 1440 / turn 2 | `20260913-182842-176-court-life-8ea6eb` |

Each run's captures 3/4 show meal candidate/control, 5/6 work, 7/8 rest. Original control and candidate comparison artifacts remain local. The index is regenerated for final retained presentation verification; original run identities above remain the review provenance.

Final retained-presentation validation on `fa2dcb3`: game/test builds passed with zero warnings/errors; all four activity/control runs passed, twelve identical-world pairs in total. Source `D627BC5C719285F280D31979AE008157A18C1B302824D3EAA1A943FEC354F231`, assembly `B540016461CA3DAB20E62B243F6256843655E41BD3593C973DDD04AF69E31A6D`. Runs: `20260913-183148-875-court-life-607b3f`, `20260913-183222-209-court-life-0f1a92`, `20260913-183254-990-court-life-6e34ca`, `20260913-183327-084-court-life-f5826e`. Restoring roof pitch/removing translucency is included in these final captures. `git diff --check` passed.

Creative controls passed at 960 on the reviewed source: `20260913-182937-301-creative-court-b58cf7`. Unchanged sixteen-resident layout observation: `20260913-183034-311-creative-court-expanded-2d53d1`, simulation time 374.11362 to 380.11398 at 1× (5.9816 wall seconds). This process interval is not watched motion, listening or performance acceptance, and overlapped part of the other capture. Full matched activity pairs cover the arranged layout; complete unchanged-layout paired coverage and uncoached self-chosen arrangement remain missing.

The helper benefits recurring presentation reviews by replacing manual path hunting and ensuring paired-state provenance. It is a small sequential wrapper over existing tooling, with low expected maintenance. The observed 127.53-second cost establishes repeatability; no speculative net time-saving figure is claimed. Do not expand it into an evidence platform.

## Next experiment

F30d2 compares finite transformation and free continuation using the same court, population and rules. The finite arm needs a visible spatial intention and satisfying stopping point; the free arm permits the same changes without prescribing an ending. Observe predictions, edits, noticed consequences and a voluntary second change. Missing human evidence stays missing; script completion cannot choose preference.

Newcomers currently share `NextMealTime = Food.Time + 15`; recurring due times support a synchronized-meal hypothesis for crowding. This chunk deliberately leaves the simulation unchanged. If crowding prevents observing the transformation, compare staggered arrivals against a same-age control before selecting a rule. Do not create artificial trips or needs to generate motion.

F30d3 then consolidates the supported experience and campaign, cutting redundant ordinary entries and recipes while preserving useful geography/fixtures. This replaces an immediate campaign rollout. See [active queue](NEXT_CHUNKS.md).
