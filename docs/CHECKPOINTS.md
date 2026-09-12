# Playable checkpoint ledger

Policy: [periodic review team](REVIEW_CADENCE_PROPOSAL.md), accepted September 12, 2026.

- Baseline: `b3a232344ad6cb0a16bc567b5afb515a38dbfb1e` — F07d1 route screen and reviewer proposal.
- Playable checkpoints since adoption: **2**.
- Last periodic review: **none yet**.
- Next four-role review: **checkpoint 5**.
- Next regular visual/audio review: **checkpoint 10**, alongside the four-role review. Substantial presentation changes also trigger a separate presentation review.

## Chunk ledger

| Checkpoint | Commit | Outcome | Review status |
| --- | --- | --- | --- |
| Baseline / 0 | `b3a2323` | Start counting after this commit. Earlier features and the stone route analysis are not counted retrospectively. | No review claimed. |
| — | `b23c35c` | Repository instructions and ledger; documentation only. | Does not advance the count. |
| 1 | `74c4f75` | F07d: playable local stone stockpiles, complete-project comparisons, UI and physical recovery. [Evidence](STONE_STORAGE_F07D2.md). | Bounded game-designer review informed comparisons; periodic four-role review remains due at 5. |
| 2 | `3b85cf4` | F21t: optional named workplace assignments, reserved slots, explicit controls and safe job transitions. [Evidence](WORKPLACE_ASSIGNMENTS_F21T.md). | Bounded game-design/UX reviews informed policy; periodic four-role review remains due at 5. |

For each subsequent chunk, append its commit and outcome. Assign the next checkpoint number only when a committed playable outcome is delivered; use a dash for non-playable work. Update the totals and due checkpoints above. Group commits that deliver one playable outcome in one numbered entry.

## Review records

None yet. Each review record must link its consolidated report, identify the fixed commit/build, list participating roles and evidence limitations, and point to roadmap decisions and the next five chunks. Record presentation-triggered reviews separately without resetting the regular count.
