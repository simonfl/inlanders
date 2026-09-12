# Cottage finishes — F09b

September 12, 2026. Select a cottage, expand **Finish**, and choose a named swatch. The actual house updates immediately. **Automatic** restores the original stable color variation.

Clay, Sage and Ochre retain the original roof family. Slate pairs a blue-gray roof with pale plaster; Rose pairs a muted rose roof with warmer plaster. Stone, structural timber, entrances and comfort shutters keep their existing treatment so the house remains readable. The controls live in the existing inspector and are collapsed initially or when selecting another cottage.

The choice is free and applies to that cottage only. It can be set while construction is planned; plaster and roofing use it when those stages appear. It persists through saves and home improvements. Demolition retains the appearance as the house is dismantled and disables further finish changes. Other building kinds have no cottage-finish controls. There is no new resource cost, task, comfort benefit or production effect.

## Visual comparison

[Automatic colors](images/cottage-finishes-automatic.png) · [Chosen finishes](images/cottage-finishes-chosen.png) · [Improved homes](images/cottage-finishes-improved.png) · [960px inspector](images/cottage-finish-inspector.png).

The gallery uses the same four cottage orientations, camera and soft daylight, with labels hidden. Choosing finishes offers player expression; it does not resolve every remaining architectural or village-composition question.

## Verification

`./Play.ps1 -CottageFinishSmokeTest` builds and checks:

- All five finishes and Automatic on all four orientations, with actual rendered roof colors and exact save roundtrips.
- Real normal-play construction and physical demolition, including chosen plaster during intermediate stages and a saved dismantling state.
- Inspector clicks for Rose and Automatic at 960×640 and 1440×900, with compact named swatches and live model updates.
- Home improvements retaining colors, subsequent repainting retaining improved shutters, and exact improved-home reloads.
- Finish commands rejecting missing, removed, non-cottage and invalid choices; demolition disables changes.
- Changing and restoring a finish returns the exact prior world snapshot, proving the cosmetic command changes no other simulation state.

These focused checks pass with a clean build. The broader campaign suite was not rerun for this cosmetic change. Palette selection is serialized per cottage; no migration layer or general material editor was added.

## Roadmap review

F09b is delivered. Next promote **F09c — connected fence runs** from the village-character backlog. Finishes let players coordinate homes; connected fences should make the spaces around those homes easier to compose. Keep the existing decorative footprint and access rules, show the same joins in preview and placement, and update neighbors after removal or reload. Drag painting remains a separate follow-up.

Further facade details, richer planting, audio/music, menu artwork and human campaign/visual review remain open elsewhere in the roadmap.
