# F30d3 — a place residents actually use

September 13, 2026. Playable checkpoint **25**, feature commit `bb71c56`.

## Outcome

The court's finite project now creates a persistent place for ordinary meals. **Your place → Choose a meal place** previews six actual reachable seats; residents who collect nearby food can bring meals there. The tool works immediately in both court versions, without completing the older welcome objective. The open court has no assigned ending.

The finite ending becomes available after a neighbor actually consumes a meal at that place. It names the first diner; this acknowledgment persists with the place and resets if the place is replaced. It is not a food reserve, attendance streak, ongoing-service guarantee or reward. The player still chooses when to finish, watch, leave or reopen. Finished status does not alter physical simulation.

The planner reports whether currently available food lies within the existing eight-tile straight-line supply radius and is reachable. This is a current availability hint, not a reservation, path-length promise or prediction that every resident will visit. A place may be built before food is available; recover by moving it or providing nearby food. Existing one-shot gatherings retain their original welcome gate.

## Design choice and counterevidence

Choose **small authored spatial transformations supported by daily life** as the provisional main experience, with free editing/watching afterward and a clearly available free start. This preserves the original ambition for meaningful settlement decisions. It does not declare the current court a sufficiently challenging campaign level or claim player preference.

The previous finite comparison supplied a brief and an unconditional finish. That was too weak to establish a functional consequence. The new slice connects a real place to an actual meal, but remains introductory: the simulation test completes without relocating a building. The UI test moves a western home and places a commons near eastern food; those are separate actions, not evidence that the first caused the second to succeed. At 1440 the first diner appears around 110.1 simulated seconds, roughly eighteen seconds after the start. This is not the requested longer, skillful level.

Do not respond by adding move counts, more diners, stock targets or waiting windows. The next substantial test needs a geographical decision with at least two visibly different viable arrangements. If placing on the first legal patch is equivalent to redesigning the village, that test has failed.

## Verification and provenance

Fixed review source `bb71c561adb3afc39f4868294abb92a7777b3928`, source fingerprint `156C1CB4642A04D955C0DCB59082F9E1C49FF22CADF943EB7B35F265DBC8A7DA`, game assembly `0D0CD09B05F5547B014114885053A603846B6F6E3DA4A62BF24B96A1DC3E8094`.

- Court960: `artifacts/review/runs/20260913-190012-137-court-experience-c2737a`; precommit request metadata, identical source/build to fixed review.
- Court1440: `artifacts/review/runs/20260913-190104-386-court-experience-56ab54`; clean fixed commit. Actual planner/placement, first meal, finish/watch/reopen, separate saves, Continue, F5/F9 and reset passed at both widths.
- `--court-experience`: identical starting/continuing physical worlds, empty/unused project rejection, actual first meal, editable ending, current saves and invalid state rejection passed.
- `--commons`: eighteen actual meal trips, peak three simultaneous claims, protected places, saved continuation, movement/removal and food recovery passed in the existing non-court situation.

Builds passed with zero warnings/errors after nullability cleanup. These are scripted checks, not human enjoyment, uncoached play, continuous-motion or listening evidence. The [checkpoint-25 whole-game review](REVIEW_CHECKPOINT_25.md) chooses the next direction and records presentation/reliability corrections separately.

Post-review corrections: both mode starts visible immediately at960; neutral open-mode guidance; reproduced and fixed stale commons clipping after same-count home relocation. See the [review synthesis](REVIEW_CHECKPOINT_25.md) for exact failed/passing runs and limits. No extra playable checkpoint counted.
