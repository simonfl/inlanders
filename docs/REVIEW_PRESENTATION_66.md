# Whole-game presentation review66 — change the place, not every upgrade flag

## Verdict and decision

**Partially convincing.** Direct furnishing is a justified removal of a redundant workplace dependency. Shared labor and real delivered planks now lead directly from selecting a home to an inhabited forecourt. That is a playable improvement, but it does not establish a compelling central activity. The most plausible current endpoint is still furnishing every home in an already orderly village.

The warm miniature is readable and worth retaining provisionally. The framing/roof treatments and benches give homes more physical character. They are **not acceptance of F40a's substantial house family or of the New France landscape**. Repeated narrow gables, smooth roof masses, small crop tokens, uniform lawn and sharp river edges remain dominant at ordinary zoom. More tiny props would not fairly test the desired direction.

**Next: merge F39b/c and F40a/b/c into a coherent working-farmstead comparison.** Keep a compact baseline and replace part of the arrangement with meaningful functional cultivation, a small number of home groups, useful domestic/shared ground, a woodland margin and articulated shore approach. Test broader/lower dwelling massing with dated references. Keep12 residents, matched inventories/rules, and explicitly account for any changed productive extent/yield/labor. This is a structural spatial experiment, not a cosmetic perimeter treatment or catalogue expansion.

The central pleasure to test remains choosing a personally preferred place and watching residents use it. Physical meals/materials, shared workers, useful routes, reversible moves, optional growth and voluntary stopping support that. Their retention has a causal rationale; human preference remains unknown.

## Freeze, cadence and independence

Fixed playable commit **f3932b7730f5ffd3af2e34524170fc0bbcedd7a3**, checkpoint66. Test assembly identity `4a64f43a-6f54-43fb-a712-a0453bbd4c21`. Five fresh independent read-only roles: `review66_design`, `review66_lead`, `review66_ux`, `review66_visual`, `review66_playtest`. A concurrent third-child spawn hit the thread limit; later roles ran in batches. No reused role substituted for an independent review. UX supplied a narrowly requested factual amendment after its initial report.

This early presentation review replaces the initial intention to implement through70 in this user run: the first F40 domestic slice reached an art-direction acceptance gate. **Only one playable outcome,66, was delivered.** The regular periodic and visual/audio review remains70; this early review does not reset it. Stop after closeout; no67 implementation.

Scope is the entire game: public compact/bank Normal/relaxed, archived campaign/Free, full economy/building/needs, controls/onboarding/ending, village presentation/audio, reliability/performance and iteration cost. Source, actual stills, scripted native controls and simulation evidence were assessed. **Hands-on playtest was unperformed**, not passed; that role audited evidence. No listening, continuous-motion acceptance or isolated native-performance claim.

## Independent verdicts and synthesis

| Role | Main conclusion and consequence |
| --- | --- |
| Game design | Direct work materially simplifies the means of change, but the binary home upgrade does not give the player much authorship. Normal starts with0 planks, so the first appealing domestic action still requires sawmill procurement. Compare spatial uses of useful ground rather than adding another service/need. |
| UX/onboarding | Direct home actions, matched modes and recoverable changes are justified. The960 card and persistent resource strip foreground accounting while the improvement is small in the world. Prototype a concrete transformation preview and one actionable blocker; put reservation detail behind Details. |
| Playtest evidence | The script opens the home card programmatically; simulation supplies the sawmill strategy. Neither demonstrates independent discovery, meaningful intention or satisfaction. Keep cancellation and voluntary ending; observe one actual self-chosen intervention before calling the loop enjoyable. |
| Development lead | No demonstrated new hard simulation defect. Global conservation and four-phase cancellation tests are meaningful, but active-phase uncancelled continuation was missing. Keep the implementation small; test consequential interruptions. Avoid a general rules or replay framework. |
| Visual/audio | A warm readable miniature, still insufficiently specific to the requested New France direction. Useful foundations/recesses and forecourt furniture are increments; land use, massing and village composition need replacement-scale work. Audio remains unaccepted. |

Agreement is strong on retaining embodied daily life and removing the workshop gate, and on rejecting a furnish-all checklist. The important tension is materials: Normal procurement may be satisfying planning or simply an obstacle before the appealing act. Do not decide by assuming every cost adds depth. Give both comparison arms enough real material for one modest intervention; leave later work under ordinary costs. Compare Normal and relaxed motivation separately.

The alternative is a **home-and-land-first arrangement game**: the player chooses where domestic work/rest, nearby cultivation and shared meals belong, and residents make the consequence visible. This is more substantial than retaining the binary upgrade and adding a tooltip. Its risks are hidden essential information, overconstrained expression and one dominant layout. Keep the current model as a comparison; do not declare the replacement fun before testing it.

Archived campaign geography may supply useful situations, but its mandated building types and delivery quotas should remain archived. The current two layouts are comparative experiments, not an established campaign or four permanent products. Gardens, grain/bread, shore and woodland have potential spatial differences; available buildings and numerical throughput do not establish that all choices are worthwhile. Add no needs or longer objectives to manufacture motivation.

The dense archived fixture is a stress/context view, not a growth target: roof overlap, small labels and crowded people obscure domestic ground. Retain finite scale while testing the core activity. The actual world ending is appropriate and should stay.

## Concrete findings and closeout

- `HomeComfortUi` taught an open-workshop prerequisite after it was removed from public hamlets. The Carpenter catalogue entry is already hidden publicly; the risk is searching for an unavailable solution, not constructing a visible redundant public workshop. Correct public Details to shared work/planks and explain the existing sawmill supply; keep archived rules accurate.
- `HomeComfort.ComfortSummary` returned general RiverFarmstead text before the cancelled-plank recovery branch. A disabled action could therefore appear unexplained except on hover. Show recovery in the primary summary until delivered materials/claims clear.
- Relaxed forecourt Details overwrote the free label with a plank price. Preserve the free label.
- The new test reloaded each active phase then cancelled, and compared deterministic continuation only after completion. Add uncancelled continuation from all four captured phases to the exact same completed world, plus relocation during installation and demolition during an in-transit delivery. These are meaningful interruption/conservation checks, not another gameplay credit.

UX initially criticized the primary ending based on the later Village management capture and inferred an exposed Carpenter from generic catalogue source. Its explicit amendment withdraws those two conclusions after checking `BuildingGroups` and capture0005. The optional reopened Village page still carries older objectives/project wording; this is a lower-priority consistency item, not a broken primary ending.

## Evidence

All paths below are local generated artifacts under `artifacts/review/runs/`; their manifests carry source/build/fixture fingerprints. Ignored artifacts are reproducible evidence, not committed assets.

| Run | Evidence and provenance |
| --- | --- |
| `20260920-190351-519-transformation-74c5c9` | Compact960 scripted native order/cancel, hidden workshop, public entry/ending/save/restart. Dirty precursor at16393f1; assembly/source fingerprints match the clean66 build.0004 direct order,0005 actual world ending,0006 subsequent Village management. Programmatic card selection is not picking/discovery evidence. |
| `20260920-190416-898-transformation-0e5eef` | Furnished compact1440; same dirty precursor source. Real simulation-produced domestic state imported for viewing. |
| `20260920-190703-514-cultivated-bank-67862f` | Clean fixed66 bank960 public scripted controls, current saves, actual relocation/construction/comparison. |
| `20260920-190747-518-cultivated-bank-289322` | Clean fixed66 furnished bank1440, imported domestic snapshot,1x20-second engine recording.492 extracted frames/20.5s audio container, indexed stills/WAV. Existence of recording does not establish listening or motion/performance acceptance. |
| `20260920-190830-938-court-experience-b07aaf` | Clean fixed66 archived Free/court1440 context. |
| `20260920-190927-228-dense-af9df9` | Clean fixed66 generated archived32-person/30-building scene. Not natural public growth. |

Before closeout: zero-warning build; all18 current-experience suites passed (`artifacts/current-experience/report.json`), broad default regressions exited0 (`artifacts/review66-regression.log`), focused direct-domestic and legacy carpenter tests passed. Compact/bank960 native checks passed. No observed save-target denial in these runs; the previous intermittent issue is **not resolved**.

No fresh actual campaign traversal, prolonged uncoached Normal/relaxed play, natural public growth, audio audition or continuous-motion judgment occurred. Previous broader context is in [review65](REVIEW_CHECKPOINT_65.md). Observation-tool hangs in60/65 were tool failures, not demonstrated game hangs. No new attempt was made using the same ineffective in-kernel timeout approach.

## Next experiment and tooling

Use only: “Make one change you want here, or keep it.” Record intention before action, alternatives, predicted result, actual resident behavior, a mistake/recovery, and why to continue/watch/finish. A satisfied choice to keep the village is valid. Reject the alternative if people merely comply with an expected arrangement, need accounting to identify the payoff, perceive no difference beyond longer walks, or converge mechanically on furnishing everything.

Accept a half-day extension of the existing evidence index with intention/action/consequence references **alongside an actual session**. Beneficiaries are reviewers and the developer; low maintenance, target reconstruction of one choice and its clip within two minutes. Do not add a new capture format, replay framework, general editor or architecture migration. Existing index/extraction tools suffice for static review.

Accept the small direct-job continuation/interruption matrix as closeout. Defer a named domestic capability seam until another treatment genuinely needs it. On save-denial recurrence, obtain a bounded OS-operation trace before changing retry policy. Actual play needs a human session or genuine external supervisory cancellation before another native observation attempt; unavailable evidence must stay explicit.

The [replacement queue](NEXT_CHUNKS.md) combines working land, domestic/shared uses, broader/lower house massing and place-first controls. Catalogue-wide asset polish, new campaign/maps, additional needs and isolated prop passes are deferred. User exclusions remain: no inheritance/succession, seasons/winter deadline, ownership bureaucracy or save migration.


## Closeout validation

The four concrete copy/recovery/coverage findings above are corrected. Zero-warning build; expanded `--direct-domestic` passes both layouts, including uncancelled exact continuation from all four phases, installation relocation, delivery demolition, conservation and recovery feedback. Final native960 compact run `20260920-191616-741-transformation-388ec4` passes public Details without the workshop prerequisite, free relaxed forecourt label, order/cancel and the existing menu/finish/save/restart/control contract. `git diff --check` passes. These corrections and this review add zero playable outcomes. No new save denial observed; no claim of root-cause resolution.
