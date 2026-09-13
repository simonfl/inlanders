# F30d2 — two ways to make a place

Historical F30d2 delivery: the unconditional finite ending described below is superseded by [F30d3](COURT_PROJECT_F30D3.md), which requires actual use of the new meal place.

September 13, 2026. Checkpoint **24**. Next periodic whole-game review **25**. This is a playable experience comparison, not selection or validation of a campaign direction.

## Play it

Open **Settlements → Court comparison**.

- **A place to gather:** the meeting area is squeezed between homes. Open room to sit or move the gathering place closer to the gardens. Choose **Your place [G] → This place is ready** when satisfied. The ending offers watching at 1×, leaving, or reopening the project.
- **An open court:** the identical village with no assigned project or finish button. Make what interests you or watch the residents.

Each version has New and Resume and its own save. Continue follows the last played version; reset preserves the selected version and restores its sixteen-resident starting court. Neither replaces the existing free-arrangement or normal settlement slot.

Both have free instant construction/movement, real shared work and meals, and no hunger penalties. Four additional homes are placed deterministically, then the ordinary arrival process advances to 92 simulated seconds. Both start from exactly the same physical state, including people, routes, resources and meal schedules. This is a new shared baseline, not the older 374-second expanded review fixture. No new hunger, job, producer, forced journey or altered arrival phase was added.

**Show starting footprints** draws amber outlines of the original buildings. The reference stays at the original cells when buildings move and survives saving/loading. It is a footprint comparison, not a replay, a restore command, a score or a fabricated image of past residents. Hide it for watching; watch mode hides the overlay. Editing never replaces the live world with the reference.

## Why this is deliberately small

The comparison changes the brief and the availability of an ending, while holding the playable rules constant. A concrete spatial intention may give arrangement purpose; an open version may inspire a self-chosen next change. Neither is presumed superior.

The player can declare the finite project finished without passing hidden checks, even without making an edit. This makes its limitation explicit: it tests the value of a project and a stopping point, **not strategic challenge, objective achievement or successful transformation**. A finish click cannot prove the meeting area improved. Finishing does not stop simulation, lock edits or grant resources. Reopening is supported.

The strongest case against this finite arm is that it could amount to a dismissible brief and button. The strongest case against the open arm remains the lack of a reason for a second edit. Building either is not evidence of fun. F30d3 must confront those questions before consolidating the menu or extending campaign content.

## Verification

Focused simulation checks establish identical starting physical worlds and identical subsequent physical state after two minutes with one marked finished. Actual meal consumption continues. They cover idempotent finite completion, no completion in the open arm, independent starting geometry after relocation, editable ending, current save roundtrips and invalid free/finished state rejection.

Rendered checks use real menu/control input for both modes, a home move, starting-footprint visibility, finish/watch/reopen, Continue, separate slots, F5/F9 and reset. First 960 capture exposed an ending scrolled past its title; the final UI resets scroll position and shortens the brief. The original Creative court simulation check also guards existing shared-work/food/relocation behavior.

Final build: zero warnings/errors. Focused `--court-experience` and existing `--creative-court` simulation checks passed. Final 960/1440 rendered journeys passed; `git diff --check` passed. Source fingerprint `90718186E2C7BA4B6319604CBDC6FD7351343231451337EF1514CC18B714EC18`; game assembly `FFF6D7D22ED3B942EE2DF40276075DF70FA52912A9CCBD7BB1EBAE28FB3CC8A2`. Captures precede the delivery commit; fingerprints identify the tested source.

- 960px: `20260913-184447-806-court-experience-f85b7a`; all scripted controls passed.
- 1440px: `20260913-184550-913-court-experience-70abde`; all scripted controls passed.

[Ending at 960](images/court-experience-ending-960.png) · [Open court at 1440](images/court-experience-open-1440.png). Lead inspected the final brief/ending/open stills. The live scene remains partly obscured while the optional drawer is open; close it or choose Watch village life to see the village. Scripted input and still inspection are not uncoached human play, motion/audio judgment or native performance acceptance.

## Decisions and observation

Keep both as a small comparison entry for now. Do not expand campaign content or infer preference from test results. Central meal crowding persists; identical newcomer timing is unchanged so it does not confound this comparison. If it prevents seeing the result of an edit, test that hypothesis in a matched timing experiment, rather than adding more decorative props.

For observation, alternate which version is played first. Before acting, record what the player intends to change. After acting, ask what visibly changed and what they would do next. Record whether the ending feels satisfying, arbitrary or premature; a voluntary second edit matters more than elapsed playtime. Use 1× for watching and accelerated time for waits. No observer has supplied those results in this delivery.

Existing review tooling was sufficient: one focused fixture and scripted journey, with isolated session save paths. No new orchestration framework or telemetry platform. The roadmap advances to an explicit experience choice, with the next whole-game review due at 25. See [active queue](NEXT_CHUNKS.md).
