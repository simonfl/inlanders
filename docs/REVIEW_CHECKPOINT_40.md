# Whole-game review 40

Fixed source/build: **772a653**. Five fresh independent read-only reviewers: `review40_design`, `review40_ux`, `review40_playtest`, `review40_lead`, `review40_visual`. All returned before synthesis and further implementation. This is the **first of the three requested periodic reviews**, followed by 45 and 50. Playable count stays 40.

## Whole-game verdict

**Partially convincing.** Keep the peaceful village-making foundation, but test working land and visible daily improvement as the core experience before adding production chains. The new opening requires a real food choice and permits staying at eight people. Its competent setup still lasts roughly two simulated minutes. More surplus, another building or a longer certificate does not establish sustained play.

The strongest objection is structural: four roofs and a timber hub dominate while tiny crops support abundant food on a broad empty bank. The player can complete setup before geography matters. Free arrangement's inhabited court currently communicates shared space more clearly. The farmstead has not earned protection simply because it is the selected opening.

## Independent findings and coverage

| Role | Principal findings |
| --- | --- |
| Design | Retain shared labor, physical meals, forgiving arrangement and optional growth/endings. Cultivated land must reserve meaningful space and compete with other uses. `WaitNearHome` excludes the primary farmstead, weakening domestic life. Older campaign producer/service certificates should stay archival. |
| UX/onboarding | Primary menu and compact Food comparisons improve clarity; the start menu does explain shared work and six-minute provisions. The completed village still asks players to invent their own next purpose. Current Free vegetable inspector incorrectly says food needs are disabled despite real meals. Test world-first relationships instead of more goal prose. |
| Playtest | Matched routes and delayed recovery are valid bounded evidence, not a human playtest. Normal producer/storage mistakes require rebuilding while home moves are forgiving; test that asymmetry. First food plus housing/fed status is an optional ending, not sustainability proof. Extra square journeys may be desirable life, not automatically inefficiency. |
| Development lead | Confirmed restart dispatches river founding to the old lake and then uses the wrong backup slot. Bread guidance contradicts local-grain rules and exposes irrelevant supper requirements. Prefer one shared immutable footprint definition and one larger Farm experiment across scenarios; update old authored placements rather than add geometry flags or migrations. |
| Visual/audio | Lower homes and river context deserve retention, full composition does not. Fields are accessories; broad lawn/straight river and distant framing undermine the setting. The old menu diorama and dense roof/label clutter are secondary. Audio, continuous motion and historical authenticity remain unaccepted. |

All roles challenge campaign depth and optional improvement, rather than merely requesting more assets. They differ over emphasis: managing a small livelihood versus composing an inhabited place; short episodes versus a persistent settlement; shorter journeys versus visible social bustle. Neither output rates nor visit counts settle those preferences.

## Chosen direction and alternatives

Retain the normal opening as a gentle baseline and Free as a legitimate alternative. Keep the full existing catalogue available, current food/rest/recreation, real supply, paths and recoverability. Freeze new needs, mills, livestock and extra resource chains. The next playable bet is **functional cultivated land plus domestic life**, followed by a contrasting already-working village whose arrangement needs improvement.

Compare that transformation situation with starting from scratch. It should produce a player-chosen improvement and recognizable resident use without mandatory growth, named building recipes or assigned coordinates. If larger fields merely inconvenience placement or compact food always bypasses them, revise or reject the field change. If players only improve when given a checklist, reconsider improvement-led play itself. A finite satisfying settlement-making game is a valid outcome; extended population growth is not a requirement.

The existing farm footprint and crop-contact offsets are a real design/implementation constraint. Test a fixed longer grain plot, retaining compact kitchen gardens. Reserve its actual cells, render inside them, preserve all rotations and entrances, and make work travel readable. Reauthor incompatible prototype layouts; personal saves are disposable. Do not paint fake acreage over buildable ground.

A new land-constrained continuation should use the same rules, not become another runtime mode. Keep its selection/integration together as one playable outcome. Test misplaced production recovery before retaining or relaxing the current move restriction. Show current work, home and meals through the existing resident tools rather than introduce another dashboard.

## Immediate corrections and tooling

Before the next feature, correct the source-confirmed restart/restore and mode-specific food guidance. These are corrections, **not checkpoint 41**. Extend the existing farmstead probe through restart → restore → F5/F9 → Continue, including both widths. This benefits every scenario change and avoids repeated manual setup; a few hours of work, low maintenance, validated by preserving scenario and slot across the complete journey.

Accept a shared immutable footprint/bounds definition as part of the field experiment. It benefits placement, path obstruction, previews, validation and art by eliminating duplicate geometry assumptions; bounded implementation cost, no new framework/cache. Validate enumerated and point-tested occupancy for all kinds/rotations plus active field saves and crop contact.

Reuse the launcher, paired snapshots, movie inspector and frame summaries. The five-second movie has 132 valid MJPEG frames and matching PCM; integrity is not listening or motion acceptance. Do not build another review platform. Human play/listening remains the important missing evidence; keep it explicit while continuing independent implementation. No speculative performance rewrite: T07 did not reproduce sustained old stalls, and fresh dense stills do not prove smoothness.

## Evidence and limits

Fresh clean-772a653 native court/Free workflow: `20260920-033120-307-court-experience-74144f` (actual scripted menus, placement/relocation, meal use, finish, separate saves, Continue, F5/F9, archive navigation and reset). Fresh old river: `20260920-033204-490-river-75fea1`. Fresh dense: `20260920-033332-897-dense-df77f6`, 32 residents/30 buildings generated from a finale route and density fixture, not a normally grown farmstead.

Primary farmstead entry/finish at 960/1440 immediately precedes the commit: `032751-717-farmstead-7e1eb6` and `032810-660-farmstead-3dc3da` (prefix `20260920-`). Improvement stills: `032516-570-farmstead-1af776` and `032621-015-farmstead-cb7808`. Opposite bread: `032007-256-farmstead-ac1cf3`. These are pending-candidate captures, not clean reviewed-commit runs. [F33b](RIVER_FARMSTEAD_F33B.md) and [F33c](FARMSTEAD_COMPARISON_F33C.md) document simulation route scope. Review-35 evidence supplies historical context only.

Reviewers inspected source, reports and actual broad stills. None performed uncoached native play, listened, viewed continuous motion, traversed the full campaign or established long-session responsiveness. Agent playtest review is not human enjoyment acceptance. Confirmed restart and copy findings are source-based, with a fresh Free inspector image supporting the latter. Historical details remain provisional. Review complete with these limitations; next periodic review 45, then 50.
