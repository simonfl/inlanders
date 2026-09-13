# F30c — free arrangement and daily life

September 13, 2026. One playable outcome, checkpoint **22**. The [independent presentation review](PRESENTATION_REVIEW_F30C.md) assessed fixed commit `83f99c4` and prioritizes recognizable daily life before campaign selection. Periodic whole-project review remains **25**.

## What changed

Creative now enters Willow court with shared work, actual food collection, home rest and recreation. Construction is instant and free; every completed building can move independently, without the normal court's one-building trial lock. Missing meals still register as missed food but impose no work-speed or food-mood penalty. Welcoming eight newcomers remains optional. Existing placement, rotation, safe removal, landscaping and stock tools are reused. The old foodless two-map rules are under Earlier prototypes → Legacy Creative, preserving useful historical capabilities without presenting them as the ordinary Creative experience.

Both court variants now let shared workers with no available job wait near home instead of staying at their last storage stop. This uses existing Waiting/routes, creates no service credit, and lets meal requests replace that journey. Workers check for real work again on arrival; home moves/removal interrupt the affected walk. Construction, solid decorations, bridges and terrain shaping preserve its destination access. It is not a new job or an artificially extended food trip.

The resident card starts with the current activity and a short meal heading. Journey expands food details; Details opens the existing inspector. Escape and × dismiss it. No always-on activity overlay. Free mode labels Move home/Move building and omits trial restoration. Its dedicated `creative-court.json` slot, Continue, F5/F9, Resume, Reset and Goals agree about the selected rules. Format **46**; no migrations. Instant removal now closes all food-store claims and clears a removed welcome-table selection, which the historical Creative rules never needed.

## Comparison and verification

`Tests/CreativeCourtChecks.cs --creative-court` starts from the same genuinely expanded sixteen-resident snapshot. The free arm moves two homes and the seating garden toward a western lane; it adds no residents, producers or food. Normal source is unchanged; neither edit advances time. Both arms then run 180 seconds, save/reload exactly and continue another 30 seconds identically from the saved copy.

| At the common final age | Constrained court | Free lane |
| --- | ---: | ---: |
| Stored food | 26 | 25 |
| Hunger | 0 | 0 |
| Cumulative home rests | 40 | 40 |
| Cumulative recreation visits | 76 | 79 |

These are deterministic behavior observations, not preference scores or an isolated causal experiment. Layout and forgiving rules both differ. A separate quiet-village arm observes home waiting, removes a home, runs a genuine shortage, resumes production, removes a selected welcome table, then removes a producer with a live local meal claim. Resource validation and exact saves pass. Free construction/removal also preserves accounting.

The normal court layout/paused-shortage comparison, relocation, batch removal and daily journey reader checks pass. Both projects build without warnings. The full rendered main-menu/recovery suite passes at 960/1440 for historical campaign/sandbox paths. New scripted Creative court controls pass at 1440 from the opposite camera: menu, two paused moves, actual placement/removal, real collection/eating, card dismissal, save/load, Continue/Resume, normal-slot isolation and Reset. Clean fixed-source 960/1440 presentation captures and the independent verdict are recorded in the presentation report; early dirty-source captures are exploratory only.

## Product limits and next decision

The opening can now be rearranged freely while daily life continues. Early expanded views show people at more home/river/garden locations, but roof occlusion and small activity silhouettes remain. Shorter cards cover less ground; they cannot establish that the closed-card village is self-explanatory. No native uncoached play, human preference, continuous-motion judgment, listening or native frame-performance acceptance is claimed.

Keep the content freeze. The next decision is whether constrained settlement transformation or free place-making gives a reason to observe and continue. Do not manufacture another level, resource or quota to avoid that question. Use the [observation sheet](COURT_OBSERVATION.md); the independent verdict now splits F30d into recognizable daily life (F30d1), then the supported-experience decision (F30d2).

Tooling used existing named fixtures, capture/provenance and build preflight. No new framework. The same-age expanded/arranged fixtures eliminate replaying an invitation and construction sequence manually for every camera; maintenance is two small fixture variants. More capture machinery cannot replace the missing human observation.
