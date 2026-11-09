# Villager presentation pass — outcome 256

November 9, 2026. The owner requested a focused appearance/animation pass while the longer scenario awaits play. The hypothesis is that distinct clothing/headwear silhouettes and more grounded movement make the residents easier to recognize and more enjoyable to watch. This does not change the campaign, economy, jobs, travel or saves.

## Changes

Muted coats/vests, linen sleeves, aprons, collars, belts, stockings and boots replace the identical bare-arm figures. Three headwear silhouettes—linen covering, soft wool cap and broad felt brim—break the repeating straw-hat crowd. Deterministic hair, complexion and garment palettes add variety, with small eyes/noses visible close up. These are New France-inspired stylized clothes, not a claim of historical reconstruction. Clothing does not encode jobs or change stats.

The existing hip, shoulder, hand and tool locations remain unchanged. Lower legs now have knee joints, counter-rotating beneath seated hips so feet fall below knees rather than jutting straight out. Named joints/headwear survive static geometry batching. Faces, clothes and sleeves are batched within their rigid pieces, not across moving joints.

Walking phase advances from actual displayed horizontal travel; pausing, standing still, boat motion and large teleports do not spin the gait. Stride and arm motion ease into movement, knees lift the trailing foot, the torso has slight counter-motion, and travel headings turn smoothly. Existing work/social facing stays exact. Stopping still returns promptly to the next work/idle pose; this is not a full blended animation state machine. Walking phase is transient presentation, while existing work/rest poses remain derived from saved state.

## Evidence

Pre-change settled-village native capture: artifacts/review/runs/20261109-213757-891-crossing-recovery-db144b/capture-0001/view.png (744c3f1).

The focused --villager-presentation-smoke-test exports a staged eight-person lineup and walking/carrying/seated pose sequence, then sampled actual-world movement at 1x and 6x under artifacts/villagers-256. The staged figures are model previews, not eight new residents or a gameplay state. The native test checks intact knee geometry, seated bending, distance-driven gait, paused transforms, and identical simulated continuation with/without rendering. Sample capture waits stretch wall time; this is not ordinary continuous motion observation. RenderActors timing excludes engine rendering and other frame work.

Existing native logging, handoff/hammer/cargo and rotated field-contact checks pass. Axe contact remains 0.220 tiles from the trunk; field hand/tool contacts remain within 0.009 tiles. Pause/reload/interruption, physical cargo and strike-timed sounds are covered by those checks. A social regression initially exposed the new walking head offset on a fresh paused arrival; scaling it by the movement blend restores the neutral arrival and avoids a false quiet-visit pose. Final rerun results follow below.

## Review and decision

One independent read-only visual reviewer inspected the baseline, lineup, seated poses, normal twelve-person village and sampled movement. It found the clothed arms and headwear families a clear readability improvement, with feet beneath seated knees and no obvious detached joints/cargo clipping in the samples. It requested darker linen because bright sunlight flattened the pale caps/sleeves; the final pass lowers the flax palette and varies apron tones. More detail was not needed.

Deliver this focused pass, then hold further character systems for owner feedback. Normal zoom supports recognizable clothing/headwear groups; it does not make eight tiny faces uniquely identifiable. Motion samples and contact checks do not establish full-session smoothness, GPU performance or human aesthetic preference. The longer-level gameplay question remains pending.
## Final verification

Zero-warning final DLL `BF61363E6DEE537431D04ED01CAB09EF8752CB90022608B005C397EECF1F3ADC`. Focused native presentation checks pass at 1x/6x, with identical simulation continuation and frozen paused poses. Measured RenderActors means were 0.34/0.39ms over the small scripted fixtures; these exclude engine rendering and are not overall frame-rate claims. Social, quiet-civic, home-rest, interruption and pose-reload checks pass on the final build. Work-contact regressions passed before the final palette and neutral walking-offset revision; their contact geometry was not changed by that revision.

Final normal-view captures (each capture-0001/view.png):

- Four-person opening: artifacts/review/runs/20261109-214816-212-crossing-recovery-71cea8.
- Twelve-person village: artifacts/review/runs/20261109-214830-985-crossing-recovery-ef4b66.
- Close lineup and sampled poses: artifacts/villagers-256/lineup.png, poses-00 through poses-07.png, walk-1x/6x-00 through19.png.

The independent reviewer inspected these final views and accepts delivery with no visual blocker. It found headwear/clothing groups readable at village zoom and coherent close walking/carrying/seated samples. The fresh opening bunches residents together, so it is weak evidence for individual recognition. Continuous-motion smoothness, historical authenticity and owner preference remain unverified. No further character detail is selected before ordinary play.