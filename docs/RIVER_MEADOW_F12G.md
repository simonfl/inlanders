# River meadow composition — F12g / F23b9

September 12, 2026. Replay level ten to start with the revised authored landscape.

## What changed

The initial layout had a regular timber perimeter, a straight bright water strip and an obvious tile checkerboard. The new layout groups the same timber into groves, trims unused corners and turns the western cutouts into water inlets. The channel extends into small northern and southern pockets. The main crossing and tested neighborhood sites remain usable.

The saved river-meadow presentation blends grass color between vertices, uses short earth shelves instead of a continuous bright bank rim, and adds restrained reeds, pebbles and occasional ripples. These shore details stay in water cells; the narrow crossing channel has no reeds. They supply no resources and introduce no navigation rules. Water geometry is combined into seven material batches.

The mesh terrain's side triangles were facing inward, making newly surfaced ground look paper-thin. Their winding now faces outward; the existing raised-map rendering uses this correction too. Terrain heights, picking and movement rules are unchanged.

[Before opening](images/river-meadow-before.png) · [After opening](images/river-meadow-after.png) · [Working village](images/river-meadow-settled.png).

Views use the same camera, soft daylight and hidden world labels. The opening remains a broad empty building meadow; the grid-based island silhouette remains visible. This is a first composition pass, not a claim of final visual polish or human acceptance.

## Budget and playability

Dry land changes from 485 to 465 tiles; water from 11 to 31. Total timber remains **168 logs**: 32 incorporated in existing buildings, 16 at the yard, 24 in western trees and 96 east. Fifteen trees and the original food source remain. Tests require at least three legal crossing choices in addition to checking the reference crossing.

Early iterations obstructed a northern replacement-home approach and the remote bakery's southern exit. Those were map failures; tree positions and the southern passage were corrected instead of discarding the routes. All four integrated campaign routes now pass:

| Route | First assessment | Expanded assessment | Supper finished |
| --- | ---: | ---: | ---: |
| Central food, 20 residents | 247s | 675s | 1547s |
| Eastern pantry/gardens, 20 | 247s | 664s | 1793s |
| Poor bread placement then recovery, 20 | 247s | 664s | 2373s |
| Arrivals during assessment, 22 | 247s | 933s | 2364s |

These are simulated timings. The map still supports the tested development choices, while different tree travel changes construction timing. They do not measure human difficulty or enjoyment.

## Verification

- `./Test.ps1 -FinaleCampaign`: all four real campaign routes, unchanged timber budget, alternative crossings, physical recovery and phase saves pass.
- `./Test.ps1 -FinaleDecision`: all five earlier staged/prebuilt experiments pass, including early baker staffing and poor-placement recovery. Prebuilt late/early staffing suppers finish at 1690s/2000s. One script now waits for a villager to clear a bakery footprint; no placement rule was relaxed.
- `./Play.ps1 -RiverMeadowSmokeTest`: regenerates campaign fixtures, then checks opening/settled views at 960/1440, central views, seven shore batches, actual bridge preview/picking and exact paused/theme reloads. Before-state comparisons run when baseline fixtures are available; the retained comparison images above were captured in this change.
- `./Play.ps1 -MapSmokeTest`: expanded-map switching, water/bridge placement and construction, plus raised-terrain picking, paths, planting, decorations, working carriers and saved terrain pass after the side-winding correction.

No save migration was added. The map's presentation flag is part of fresh world saves; replay creates the revised map.

## Roadmap review

The depth/service/landscape sequence is delivered. Next promote **F09b — choose cottage finishes**, already part of the village-character backlog. The completed neighborhoods reveal repeated automatic roof colors; let players arrange a coherent set themselves through the existing inspector. Keep the palette bounded, preserve material/entrance contrast, and verify all orientations and improved homes. This adds expression without another mandatory need or production chain.

Connected fence runs and decoration brushes remain subsequent possibilities in F09. Broader visual acceptance, human pacing, sound/music refinement, and the rest of the optional backlog remain open.
