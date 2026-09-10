# Managed woodland — F02b

The player can retain favorite trees and establish a productive grove without marking every successive planting. This uses existing loggers, physical timber collection and the existing three-day growth period; no new building or worker role.

## Playable behavior

- Build → Landscape has Preserve trees, Allow harvesting, Manage grove and Remove grove spots. All support clicking/dragging. The grove count stays visible in the tool footer; marked ground is visible while using woodland tools.
- Preservation immediately stops an ordinary cut in progress. Protected saplings still grow, and trees carry a green trunk band. Explicit clearing overrides protection.
- Up to 32 managed spots can contain existing trees or safe empty ground. Loggers prioritize clearing, then already-marked planting, then grove replanting, then ordinary timber harvesting. They collect all timber before replanting a stump. Multiple loggers claim separate jobs.
- Removing management leaves the existing tree and current planting work. Clearing removes the designation; construction, paths and decoration take over their plots. Cancelling a later order does not restore an earlier designation.
- A designation is not a tree or an obstacle. Actual planting checks access again; temporarily unsafe spots wait. Preservation is not wildlife habitat yet: F26c will define that separately.
- Save format 27 stores both settings. No old-save migration.

## Evidence and limits

The full simulation suite passes, including two loggers repeatedly harvesting/replanting three spots, protected-tree interruption, exact save continuation and clearing/construction/path/decoration precedence. In a 900-second normal-play fixture, one logger completed ten plantings, grew 64 logs and took five home rests while two foragers kept hunger at zero. Grown logs measure maturation, not pantry delivery; the repeated Creative fixture separately checks timber reaching storage. Rest and travel make normal throughput lower than the two-logger Creative fixture.

`./Play.ps1 -WoodlandSmokeTest` builds and exercises the rendered buttons, a real mouse drag/release, tree marker, spot overlay, tool switching, paused save/reload and growing trees. Captures at 960×640 and 1440×900 are in `artifacts/f02b-woodland-*.png`. The narrow view was inspected and its footer shortened to keep the spot count and key behavior visible. These checks establish usable controls, not aesthetic acceptance of the overall game.

The 32-spot limit and individual-tile brush are first-version bounds. Larger brushes, species, a forester building and dedicated wildlife controls remain later possibilities. F07b remains next: explain and improve neighborhood logistics where measured travel demonstrates a benefit. F23a art acceptance and human river/lake pacing remain open.
