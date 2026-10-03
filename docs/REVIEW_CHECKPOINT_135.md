# Whole-game review135 — October3,2026

Fixed `b7a2753bae9f8de3f7f2fad40cf3395714e288da`, DLL `849AD0F8D3C27C893B33AA854F34D62190071F50A41615C65F786306C1EF88DF`.

## Verdict

Five independent disciplines: **partially convincing**. Recommended entry and visible distinctions between woodland, meadow, cultivated ground and shallows improve recognition. They do not resolve the central ambiguity between landscape arrangement and consequential settlement management. The broad clearing still lets the player apparently do everything, while repeated roofs, small fields and a stepped shore continue to dominate composition. Another five decorative details would evade the finding.

## Independent inputs and disagreement

Reused separate reviewer contexts due the established thread quota:125design,125UX,120play,120design acting as development lead,115/120visual/audio. All read-only at the same fixed boundary; no other135 reports were read before verdicts. No independent live play or listening. The play role explicitly audited source, native scripts/logs and stills.

Design calls for a meaningful spatial livelihood problem in the existing clearing: limited near-home open ground can become cultivation or domestic/shared ground, while the shore offers another livelihood. Keep an unchanged village viable; no forced growth/hunger or new fertility/ownership rules. UX and play prefer one sustained local arrangement interaction, replacing accumulated home/yard/field controls and then watching actual use. Lead supports perceptible edit consequences and identifies a measurable rendering seam. Visual/audio insists that changing the relationship of homes, productive land, woodland and river matters more than another surface treatment.

**Chosen synthesis:** realize the spatial alternative in the existing clearing, then remove interaction friction that obscures that choice. A pure UI-polish batch would leave the design objection intact; a new preset or new needs would merely add breadth. The geography change must remain recoverable and support different viable uses from the same start. Full catalogue stays available. The interface consolidation serves this test, not an expanded management system.

## Authorized remaining five

136. Revise the existing clearing's actual river/usable-ground relationship so near-home cultivated and domestic space compete; retain modest viability and a practical existing dock option. No additional menu start.
137. Replace the expanded home editing stack with a compact arrangement tray; keep household relocation/details secondary and retain position/orientation/appearance actions.
138. Choose and furnish domestic ground in one faithful proposal with actual material consequence; preserve free rearrangement of existing furniture and no-yard validity.
139. Bring existing cultivated-strip manipulation into a compact world proposal, retaining size, crop-loss, material and pause/cancel behavior.
140. Exit arrangement into observation of the selected local home–work ensemble and actual inhabitants, preserving return context; validate contrasting same-start arrangements and review the entire game again, then stop.

Each must deliver a playable interaction/situation; existing functionality and validation alone cannot be counted again. Adapt scope if inspection finds a step already fulfilled. No new needs, catalogue entries, inheritance, seasons or unlock campaign.

## Whole-game assessment

Keep physical work/materials/meals, home use, crop harvesting, reversible placement and optional stopping because they can make spatial decisions perceptible. Founded, finished hamlet and working clearing remain different entry hypotheses, not permanent equally prominent commitments. Normal/relaxed distinguish useful material deliberation from unwanted delay; legacy foodless Creative remains separate. The19-building catalogue is available, but processing, storage variants and three recreation capacities have not earned compulsory roles. Fields, river and woods have the strongest geographic identities.

Keep archive campaign separate. Early named-building recipes and later population/service/meal assessments are not the public destination. Later river/lake/woodland/quarry constraints are useful source material for situated choices. More service obligations would not resolve weak agency.

Retain the warm renderer, readable people, house entrances/chimneys, practical landing and actual crop changes provisionally. Vegetation now differentiates ground, but geometric margins and bank steps remain visible. Generic rural forms are not an accepted historical reconstruction. Audio remains source-only: work coupling, ambience, music rests and voice limits cannot establish enjoyable timbre, balance or repetition. No water/fishing-specific cue was found; audition existing scenes before extending sound.

## Falsifiable comparison

From the revised same start, construct a near-home cultivation arrangement and a domestic/shared-ground plus shore arrangement. Use ordinary controls where feasible, then verify actual meals, work and home/commons use; inspect ordinary-scale scenes. If both uses can occupy the same valuable ground without a choice, or one modest arrangement cannot function, the mechanical bet fails. If the difference can only be found in metrics, presentation remains inadequate. Human preference stays unknown; a short satisfied session and an indifferent abandonment must not be conflated. Do not indefinitely defer a concrete experiment awaiting taste evidence.

## Evidence and limits

Bundles under `artifacts/review/runs/20261003-`:

- `155920-347-river-hamlet-57b182`: same135 DLL, dirty precommit1440 ordinary12s1x, crop/landscape stills and frames.
- `155950-254-plot-revision-77c06a`: sameDLL native960 real field manipulation, preparation/crop evidence.
- `155722-305-working-clearing-008c76`: predecessor134 native960 menu/household/save/move preview and actual home-to-field path.
- `154643-572-working-clearing-920beb`:131 compact menu, both modes visible at960.
- `160212-156-working-clearing-relaxed-b1e586`: clean fixed135 opposite1440 relaxed clearing.
- `160235-452-dense-6e8f7b`: clean fixed135 dense archive1440,12s1x.
- `160359-519-opening-4ff4d2`, `160450-253-river-c31cf2`: clean fixed135 introductory/later archive960.

Positive-WallMs medians/P95/max: ordinary10.465/15.094/486.187ms (1030 rows), dense22.343/31.45/503.835ms (487). These short samples do not establish smoothness or a regression. Lead inspected the486ms event:1.821ms callback, .058ms simulation, .355ms actors,484.375ms whole-frame thread CPU. It lies largely outside the instrumented callback. Separately, recurring34–35ms actor-setup frames plausibly include worked-ground rebuilds. Do not conflate these causes.

Unknown: uncoached choice/enjoyment, continuous motion, listening, full campaign/catalogue traversal, fresh legacy Creative, historical authenticity. Existing current-save checks cannot close the unexplained108 save replacement denial. Rendering/entry changes had zero-warning builds and targeted native verification; broad final simulation regression remains planned at140.

## Tooling and corrective follow-up — no extra outcome

Accept one WorkedLandMs/rebuild counter in the existing trace. Beneficiaries are developers/reviewers diagnosing repeated ground-update cost. Small implementation/maintenance scope; validate attribution before optimization, not a new profiling framework. Initial instrumented ordinary run `160849-145-river-hamlet-ac27cd` measures twelve rebuilds: mostly28–35ms, first59.112ms, closely matching actor setup. This confirms the narrow target; it does not explain the486ms outlier.

Reuse static ground tint samples until buildings/paths/trees/world/terrain change, while recomputing actual-use tint and excluded vegetation. This is a local cache, not a renderer rewrite. Compare the same scene and trace afterward. Preserve real walking wear and dynamic edits. Performance correction and instrumentation do not increase the playable count. Defer engine-side outlier investigation until a controlled reproduction; no speculative infrastructure or audio pipeline.

Corrected ordinary run161105-109-river-hamlet-a0d8cc preserves the scene and measures recurring rebuilds at10.8–13.9ms, versus28–35ms before caching; first rebuild33.3ms. This addresses the measured recurring cost, not the unexplained long wall outlier or dense-scene performance.
