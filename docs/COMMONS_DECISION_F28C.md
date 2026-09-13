# F28c — finite settlement first; recurring life remains optional

September 13, 2026. This is a product decision and comparison package, not a new playable checkpoint. Count remains 17; regular whole-project review at 20.

## Decision

Choose a finite welcoming-settlement arc as the primary game: prepare a place, commit to arrivals, house and welcome people, then finish. The player may stay and reshape the village. Do not author a mandatory persistent commons project, attendance quota, longer reserve treadmill or new need to manufacture a reason to continue. The recurring commons remains an optional comparative epilogue, not the premise of the campaign.

This is a provisional design decision supported by visible weakness of the continuing-play payoff; it is not a human preference result. Actual motion, ordinary-input discovery and listening remain unknown. The substantial alternative—a village-transformation game beginning in an established settlement—is still testable using the branches below, but has not earned primary status.

## Independent presentation review

Reviewer `commons_presentation_review`, fresh independent read-only context, assessed source at `add3facdc7edd1a38eb0672b14bf8653b4827fa1`, the checkpoint-15 whole-game images/reports, contextual inspector, and commons captures at 960/1440. Supplied images were captured before commit with dirty provenance; no independently rerun fixed-commit gameplay is claimed. 960 capture build hash: `D72337A6777D403745CD410E7301BEE4B0F169B5C2BBEF1AEA0720A7009A818D`; fingerprint `D01C71D9AF1376F086FF7FCCCCADFC2286164D5E0F61E2C18C89CAF034874F1E`.

Whole-game verdict: partially convincing village diorama. Warm roofs, distinct ovens/crops, bridges and physical carrying deserve retention. Dense decoration/labels compete with people, while empty meadow foreground can make the village incidental. More individual model detail will not settle composition. Current inspector is materially clearer, but dedication remains prominent relative to automatic shared work. Old campaign certificates should stay secondary; Creative needs explicit different-rule labeling. No fresh late-campaign play, native onboarding, normal/dense performance or listening occurred.

The strongest objection: the recurring place reads as scattered rings beneath foliage, and diners resemble ordinary workplace meals. Lower administration does not itself supply creative ambition. The reviewer recommends finite completion plus optional stay/rearrange. The lead agrees. Important qualification: one shaded site is not a fair rejection of every commons composition; compare an open alternative before discarding it. No disagreement is hidden by declaring the prototype accepted.

## Three launchable branches

All use the same deterministic viable village, simulation time, population, buildings, food and camera. Only the chosen shared activity differs. They start paused and use normal game controls.

```powershell
.\Review.ps1 Inspect commons-untouched -Storybook -Speed 1
.\Review.ps1 Inspect commons-event -Storybook -Speed 1
.\Review.ps1 Inspect commons-recurring -Storybook -Speed 1
```

Close each review window before opening the next. `Check` in place of `Inspect` validates the prepared save. `Capture` exports a state/image bundle. F8 captures a moment while playing. Review files are isolated under artifacts/review; personal saves are disposable but unrelated files are not touched.

For an audition, start each branch fresh. Watch one ordinary meal cycle at 1x with sound, then freely change something you care about. In the recurring branch try a visible open site as well as the original shaded spot through Goals → Rearrange shared place. Include a farther food journey and removal, not only the most attractive arrangement. Use 3x/6x for production waits, returning to 1x to assess approach/eating/departure. Keep camera, window and audio settings matched when comparing. Record screen/audio using the existing movie option or a normal screen recorder; a file existing is not a listening result.

Record: what changed visibly, which action you wanted next, whether you voluntarily stayed, whether you understood a failed placement/meal, and which branch you would choose. A second arrangement or long watch is evidence of preference only when freely chosen. More visits or a faster test completion is not acceptance.

## Tooling and next work

Three catalog rows and one fixture helper reuse T01; no new runner. Beneficiaries are future reviewers and the player, avoiding source navigation to recreate comparisons. Cost is a small helper/catalog surface; validate each launch/save and matching non-activity state. Existing source hashing repeatedly rebuilds ~20-second gathering fixtures after UI edits; track this cost before a bounded fingerprint split, not a dependency framework.

F28d now implements a guided opening, explicit finite ending with optional stay, a separately described meadow situation, and clear Creative/legacy entry contracts. No additional campaign chain or generalized scenario factory is needed for these two existing identities. The next queue should test a truly distinct spatial problem and whole-scene readability, not expand numerical assessments.

Validation: all three prepared snapshots pass current-format validation/roundtrip. The recurring branch renders through the launcher. State comparison matches economy, time, people and buildings; event activation additionally appends history and resets the existing job-retry timer from 0.1 seconds to zero. This tiny expected activation difference is recorded rather than claiming byte-identical worlds.
