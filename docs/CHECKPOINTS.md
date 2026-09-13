# Playable checkpoint ledger

Policy: [periodic review team](REVIEW_CADENCE_PROPOSAL.md), accepted September 12, 2026.

- Baseline: `b3a232344ad6cb0a16bc567b5afb515a38dbfb1e` — F07d1 route screen and reviewer proposal.
- Playable checkpoints since adoption: **8**.
- Last periodic review: **checkpoint 5**, [whole-project assessment](REVIEW_CHECKPOINT_5.md). Four roles reported; interactive playtest not performed (native capture hung).
- Immediate strategic review: **synthesis recorded at checkpoint 8**, [decision and limits](STRATEGIC_REVIEW_8.md). Three independent agents; two further disciplinary passes reused contexts after thread-limit failures. Fresh native observation reached only the menu; no new gameplay or listening. This is not five fresh independent reviews or a successful playtest. The new queue tests a neighborhood redesign; documentation does not advance the count.
- Next four-role review: **checkpoint 10**, with whole-project scope and the visual/audio role. Interactive campaign observation remains an earlier follow-up.
- Next regular visual/audio review: **checkpoint 10**, alongside the four-role review. Substantial presentation changes also trigger a separate presentation review.

## Chunk ledger

| Checkpoint | Commit | Outcome | Review status |
| --- | --- | --- | --- |
| Baseline / 0 | `b3a2323` | Start counting after this commit. Earlier features and the stone route analysis are not counted retrospectively. | No review claimed. |
| — | `b23c35c` | Repository instructions and ledger; documentation only. | Does not advance the count. |
| 1 | `74c4f75` | F07d: playable local stone stockpiles, complete-project comparisons, UI and physical recovery. [Evidence](STONE_STORAGE_F07D2.md). | Bounded game-designer review informed comparisons; periodic four-role review remains due at 5. |
| 2 | `3b85cf4` | F21t: optional named workplace assignments, reserved slots, explicit controls and safe job transitions. [Evidence](WORKPLACE_ASSIGNMENTS_F21T.md). | Bounded game-design/UX reviews informed policy; periodic four-role review remains due at 5. |
| 3 | `a41c734` | F16f: Creative bush relocation with preserved food state, picker release, protected routes and explicit confirmation. [Evidence](BUSH_RELOCATION_F16F.md). | Bounded source/game-design review informed scope; periodic four-role review remains due at 5. |
| 4 | `9c46539` | F12h: Creative terrace selection, Before/After, full-border preview, Apply and Undo. [Evidence](TERRAIN_SHAPING_F12H3.md). | Presentation review corrected board/scenery preservation and border legibility; four-role review remains due at 5. |
| 5 | `1c89149` | F09e: always-open fence gateways, path coexistence, four saved facings and side-only connections. [Evidence](FENCE_GATEWAY_F09E2.md). | Presentation review accepted the model and verified clarity fixes; whole-project review recorded below; interactive play remains unverified. |
| 6 | `5e5fdb5` | F12h4: distinct terrain blocker markers, actionable refusal and inline blocked Undo with retry. [Evidence](TERRAIN_FEEDBACK_F12H4.md). | Focused simulation and rendered UI checks at 960/1440; whole-project review remains due at 10. |
| 7 | `4307a8f` | F21u: accurate orchard assignment guidance, UI recovery and stronger original-versus-reload evidence. [Evidence](ASSIGNMENT_GUIDANCE_F21U.md). | Focused simulation and rendered People/inspector checks at 960/1440; whole-project review remains due at 10. |
| 8 | `3d857aa` | F11d/F18d: ordinary opening/river observation, matched finale alternatives and recovery, clearer central-grain guidance. [Evidence](CAMPAIGN_REVIEW_F11D.md). | Real native play and separate simulation/rendered checks; does not backdate checkpoint-five playtest. Whole-project review remains due at 10. |

F12h1 terrain design and test-only geometry evidence: [report](TERRAIN_SHAPING_F12H1.md). This foundation does not advance checkpoint 3; runtime commands are delivered in [F12h2](TERRAIN_SHAPING_F12H2.md). F12h3 now delivers their player controls; the combined feature counts once as checkpoint 4. Its bounded game-design review does not replace the periodic review.

F09e1 [gateway design](FENCE_GATEWAY_F09E1.md) adopts a bounded independent source/design review. Documentation only; no playable checkpoint increment and no periodic review claimed.

For each subsequent chunk, append its commit and outcome. Assign the next checkpoint number only when a committed playable outcome is delivered; use a dash for non-playable work. Update the totals and due checkpoints above. Group commits that deliver one playable outcome in one numbered entry.

## Review records

**Strategic checkpoint 8 — September 12, 2026:** [synthesis](STRATEGIC_REVIEW_8.md) and [disciplinary reports](STRATEGIC_REVIEW_8_ROLES.md), fixed source `aea4dc40e49d2b88ed25723dce33c1318cb8e605`, existing assembly hash in report. Game design, UX and development lead independently challenged the whole game. Fresh additional agent creation failed; the designer also covered visual/audio and UX also attempted native play with inherited context. New native observation reached the menu only; owned process was closed. No listening/performance result. Chosen direction: comparative neighborhood redesign plus bounded scenario/evidence tooling. [New queue](NEXT_CHUNKS.md) replaces the incremental patch sequence. No playable increment; periodic checkpoint-ten obligations remain.

**Checkpoint 5 — September 12, 2026:** [whole-project review](REVIEW_CHECKPOINT_5.md), fixed commit `7e271fe7f22771d579096f3e3ff2f39ace4b0ade`, assembly hash recorded in that report. Independent game-design, UX/onboarding and development-lead reviews assessed the full game; the playtest role reported a native capture failure and zero gameplay findings. The review is recorded with that limitation, not as a successful playtest. Findings were consolidated and the next five chunks selected. This documentation work does not advance checkpoint 5.

Presentation-triggered F12h3 review at `9c46539`: [evidence and corrections](TERRAIN_SHAPING_F12H3.md). One independent visual reviewer verified corrected scenery preservation and plot/border legibility; no human play, motion or listening claimed. The periodic four-role review remains due at 5.

Presentation-triggered F09e2 review at `1c89149`: [evidence](FENCE_GATEWAY_F09E2.md). Independent reviewer accepted the courtyard entrance after four camera-side views, stronger preview contrast and active passability guidance. Still-image review only; this presentation review did not replace the separate whole-project review.

Each review record must link its consolidated report, identify the fixed commit/build, list participating roles and evidence limitations, and point to roadmap decisions and the next five chunks. Record presentation-triggered reviews separately without resetting the regular count.
