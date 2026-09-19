# F32d — connect paths through the village

September 19, 2026. Checkpoint35; full review due before further implementation.

Build → Landscape → **Connect paths · two clicks** lays a free route between clear ground tiles or completed building entrances. The first click anchors a preview; the second confirms. Esc cancels without editing the village. The route follows actual walkable ground around obstacles and across completed bridges; it cannot build crossings or clear obstructions. Paint/remove brushes remain available, including P / Shift+P.

Routes use the existing path cost and25% movement benefit. A connection applies atomically and reroutes current walkers once. Confirmation rechecks current geometry; previews are read-only and cached for at most250ms. No new save fields, resources, needs or balance changes.

## Comparison and verification

Starting from F32c's revised twenty-person center, connect two homes to the seating garden and the garden to the dock:22 unique path tiles. Both this arm and the unchanged control remain20/20 housed with zero hungry person-seconds over five simulated minutes. Exact save continuation passes. This demonstrates a usable connection, not meaningful difficulty or human enjoyment.

`--path-connection` checks unbridged-water rejection without mutation, completed bridge use without water paving, connected route steps, read-only queries, repeated-route idempotence and exact current-save continuation. The original path suite also passes, including actual faster travel, weighted route preference, active claims and replacement by buildings/trees. Both projects build without warnings/errors.

Scripted native960/1440 probes exercise the real button, building entrance snap, pointer preview, held-click confirmation, cancellation, F9 and the old brush. Captures:960 `20260919-133845-172-hamlet-spacious-8ea38a`;1440 opposite `20260919-133931-869-hamlet-spacious-95e740`. A final color-only revision makes the preview cyan to distinguish it from existing beige paths; its recapture is recorded below. Simulation evidence is in `artifacts/hamlet-paths`.

Stills show a more legible approach but persistent central crowding and an angular lake edge. Retain the tool provisionally and stop for whole-game review35. No uncoached play or listening acceptance.
`20260919-134018-122-hamlet-spacious-8b1a7c`: final1440 opposite-view probe passed; cyan preview inspected. No further production changes before review35.
