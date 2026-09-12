# F07d2 — playable local stone storage

Stockpiles now accept stone as a third material. They still cost four logs and hold twelve units of one material. Quarriers deliver directly to the nearest matching store with capacity; builders collect near their project. Haulers remain optional and balance the chosen target. Target zero requests hauling out, not closure to producers.

The material button cycles logs → planks → stone → logs, including on construction plans. Occupied piles and committed incoming/outgoing claims prevent switching. The inspector shows stored/reserved/incoming stone, the Economy directory separates central and local stock, and stacks follow actual local inventory. Central stone art now uses central inventory rather than the village total. Save format 36 requires a fresh settlement; no migration.

## Design review and comparison

An independent game-designer subagent reviewed the route screen before the comparison. Its three recommendations were to finish the complete project even when the near deposit exhausts, count setup and displaced labor, and retain successful no-pile routes. All are included below. This was a source/design review, not a periodic review or observed human playtest.

Run `./Test.ps1 -StoneStagingPlayable` to regenerate `artifacts/stone-staging/playable.md`. Five deterministic normal-play routes use the authored quarry map, its unchanged starting stocks and eight residents, ordinary construction, food, rest and finite deposits. The clock starts before planning. All build the same hall at `(11,2)` and include sawmill/quarry establishment. Piles receive construction priority; the same two initial builders otherwise handle all construction. Each pile costs four additional logs, supplied normally. The near-first routes build a second quarry after the eight-stone outcrop exhausts.

| Source policy | Storage / staffing | First eight stone delivered | Hall complete | Total worker travel |
| --- | --- | ---: | ---: | ---: |
| Distant | Central only | 298.1s | 408.0s | 3,120.4 tiles |
| Distant | Pile `(14,2)`, rotation 3; no hauler | 240.9s | 315.3s | 2,429.7 tiles |
| Distant | Same pile; builder 2 becomes hauler after pile completion, target 12 | 366.3s | 415.9s | 2,897.7 tiles |
| Near, then distant | Central only | 192.3s | 405.5s | 3,085.0 tiles |
| Near, then distant | Pile `(8,1)`, rotation 2; no hauler | 228.7s | 406.5s | 3,106.7 tiles |

The strong pile finishes at 27.0s; the weak one at 22.8s. Each consumes 12.1 worker-seconds of actual building activity in the sampled trace, plus material collection and delivery. The full report splits travel by builder, quarrier and hauler. Total travel stops at project completion, so its reduction partly reflects the shorter observation period; it is not a per-minute efficiency measurement. These scripted layouts establish a useful placement/staffing choice, not universal superiority or human enjoyment.

**Decision:** ship optional stone piles. The distant direct route finishes about 93 seconds sooner despite setup; the weak route gains essentially nothing, and taking a builder away to haul loses the strong route's completion advantage. Keep campaign objectives and costs unchanged. No warehouse requirement, new stone consumer or extra level follows automatically.

## Verification

- `./Test.ps1 -StoneStorage`: ordinary quarry/pile construction and real extraction, direct delivery, capacity fallback, local builder reservations and delivery, Economy totals, exact saves/continuation, cancellation and demolition. Four rotations exercise real hauling, interruption, Creative removal, area removal, relocation, draining and invalid inventory rejection.
- Full `./Test.ps1`: simulation regressions pass, including existing log/plank storage and campaign routes.
- `./Play.ps1 -PlankStorageSmokeTest`: existing plank regression plus stone selection through mouse clicks, hauling, filled models, occupied switching protection, central/local art separation, reload and drain/switch at 960 and 1440 pixels. Screenshots under `artifacts/stone-staging/`.
- `./Play.ps1 -HudSmokeTest`: shared HUD regression passes. Subsequent changes are limited to the stockpile inspector's material-specific wording and central-stone art, covered by the focused rerun.

No remaining F07d implementation requirement was identified. Audio audition, broader performance investigation and uncoached campaign feedback remain separate open roadmap work.
