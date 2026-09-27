# Whole-game review 90 — September 27, 2026

Fixed playable commit **d390bf15f50a306249e78ad34b6956f297aa9e66**. Five fresh, independent, read-only contexts completed: `review90_design`, `review90_ux`, `review90_play`, `review90_lead`, `review90_visual`. Dispatch was staggered within the three-reviewer concurrency limit. None read another90 verdict before responding. The playtest role audited source, stills and recorded input evidence; it did **not** perform independent or uncoached play. No human enjoyment, listening, continuous-animation or native performance acceptance is claimed.

## Verdict and decision

**Partially convincing.** Founding restores authorship: the player decides where homes belong and whether gardens, bread or fishing shape the place. Physical construction, work, meals and home visits could make that arrangement rewarding to observe and revise. The game has not established a satisfying first session or a reason to keep improving after initial necessities are placed.

The strongest shared objection is a causal gap. Eight settlers begin with 120 food; `Simulation/SharedWork.cs` suppresses automatic food work until edible reserves fall below four portions per resident or someone is unfed. At eight meals/minute, the reserve difference nominally represents about eleven simulated minutes. This is a rule-based estimate, **not measured first-work or first-meal latency**. The earlier twelve-minute founding check had no new harvest; current twenty-minute branch checks establish eventual production, not when a player understands or enjoys it. A new workplace can appear inactive after construction, encouraging staffing-panel investigation or fast-forwarding past the life that should reward choosing it.

Choose **working-place establishment** as the next design experiment: keep generous provisions, but allow a bounded real first productive cycle before mature reserve maintenance takes over. Test gardens, the complete grain/oven chain and fishing, including actual output reaching a meal. Do not simulate a fake animation, force consumption of one source, remove the safety reserve, or prescribe a building sequence. Reject the change if it merely relocates waiting, wastes labor or produces no intelligible consequence.

A second, linked question concerns the unit of progress. The day-20 founded scenes still read as four repeated homes plus a detached livelihood. Compare that with a connected, freely editable home/work-yard ensemble using actual access, domestic ground and productive land. **Do not turn the visual finding into a list of decorative props.** Do not turn the UX suggestion into a compulsory two-house template. Landscape and activity should make a personally chosen arrangement legible.

Keep founding primary only provisionally. The inhabited comparison remains valuable because immediate daily life may motivate better self-chosen changes. Its lower placement on the960px menu biases discovery; equalize access before drawing conclusions from entry choice. No selection counts in this review establish preference.

## Independent findings and disagreements

| Role | Whole-game finding | Proposed structural response |
| --- | --- | --- |
| Game design | Founding adds authorship but may become four houses plus one food recipe. Optional goals alone do not supply intention. | Real first livelihood payoff; compare resource-constrained founding with composition-first working village-making. |
| UX/onboarding | The opening teaches buying an object, while relationships among home, meals and land remain inside inspectors. Chooser retirement after one building creates an unverified transition. | Compare an editable spatial sketch of a place against individual placement; preserve all catalogue access and cancellation. |
| Playtest evidence | Successful predetermined branches show feasibility, not a player noticing a reason to revise the village. | Record entry through first food and an uninstructed second choice, including idle stretches and panel visits. |
| Development lead | Shared physical simulation and recovery are useful; the reserve gate undermines establishment. Save retry stalls are plausible, not attributed observations. | First-cycle comparison plus narrow timestamp evidence; no engine rewrite or more blind save retries. |
| Visual/audio | Low houses, river and fields are coherent; blank ground, repeated spacing and detached objects do not yet express habitation. Dense archive adds clutter rather than a suitable target aesthetic. | Compare a connected home/work-yard ensemble at ordinary zoom, then assess actual motion and sound. |

Four roles explicitly prioritize the first productive payoff; visual emphasizes a more legible inhabited ensemble. UX emphasizes planning before construction; visual emphasizes the spaces between buildings; design also proposes removing resource management as the primary layer. These are distinct bets. The lead selects first-cycle establishment first because it tests the causal gap without requiring a new editor or choosing an art interaction prematurely. Connected composition follows as a fair comparison. A multi-object sketch is conditional on a desired arrangement that individual placement makes difficult; it is not automatically the next feature.

The substantial alternative remains **composition-first village-making**: free construction and safe meals, with physical work, paths and daily use giving arrangements character. Current relaxed rules provide a base, but simply renaming relaxed mode is not the experiment. The scene and interactions must support a coherent arrangement and revision. Prefer this alternative if resource constraints mainly add delay without interesting choices. Reject it if locations feel interchangeable and players miss the meaning of material/labor consequences. Unavailable human preference is an uncertainty to expose, not evidence that either option won.

## Entire project: keep, cut and redesign

- **Core loop:** retain shared workers, transported material/food, chosen livelihoods and recoverable moves because they can show consequences of siting. Redesign establishment scheduling; do not preserve it merely because tests pass. Staying small and finishing voluntarily remain valid.
- **Buildings/economy/needs:** homes, cultivation, shore work, woodland, access and useful storage have distinct spatial roles. Grain/oven, plank/stone work and larger homes must justify their investment in the chosen village. Nineteen definitions do not prove nineteen meaningful decisions. Keep the full catalogue accessible, with overlapping scales/services secondary. No new chains or needs to manufacture activity. Meals, rest and recreation should read as lived activity; composite happiness must not become an implicit service checklist.
- **Campaign:** keep archived geography as material for future situations, not an obligation to restore ten chapters. Early hut/home/farm/bakery recipes and later population/service certificates do not solve current motivation. Introductory and dense later states were freshly viewed; a full campaign traversal was not performed.
- **Normal/relaxed/Creative:** retain the same public place and daily life with different resource/hunger consequences. Legacy Creative has different foodless rules and remains archived. The fresh free-court image is not evidence of legacy Creative play; legacy rules received source review.
- **Controls/onboarding:** direct world selection, actual yard previews, contextual watch/journeys and Pause & move enable experimentation. Keep diagnostic Economy and policy depth secondary. Do not answer the lack of meaningful second decisions by adding more tutorial prose or a permanent dashboard. Equal entry visibility at 960 is required for a fair comparison.
- **Visual direction:** retain low domestic silhouettes, restrained colors, cultivated rows and river negative space. Retire the dense archive's tall colorful board-island aesthetic as the future baseline. Redesign ensemble relationships: entrances, usable yards, worked ground and retained vegetation. Historical authenticity remains provisional; no fresh historical research was performed here.
- **Audio:** spatial cues, buses, existing themes and quiet intervals merit audition before replacement. Source inspection cannot accept the mix. A new1x recording is available but was not listened to or assessed continuously.
- **Reliability/performance:** current simulation and scripted controls pass. Atomic replacement denial `0x80070005` remains unexplained; later successful saves do not resolve it. Existing retries can synchronously wait up to 400ms, a plausible stall mechanism rather than attribution of a measured stall. No migration, additional blind retry or repeated unavailable WPR request. Native frame performance remains unmeasured.

## Next experiments and evidence that could reject them

1. **First working cycle:** same inlet, population,120 provisions and ordinary costs; compare reserve-only control and bounded establishment. Measure completion→work→output→locally produced meal, construction contention and surplus. Capture ordinary controls as well as scripted timing. Reject if comprehension, voluntary observation or a wanted next decision does not improve, even if latency falls.
2. **Connected farmstead composition:** same livelihood/capacity, compare current independent buildings with freely editable relationships among home, yard, work and access. Include one awkward placement and recovery. At960/1440 ask a viewer to identify homes, productive activity and one wanted improvement without overlays. Reject if ground detail is noise, templates constrain expression or relationships remain unreadable.
3. **Management versus expression:** compare Normal and a complete composition-first slice beyond the first meal. Observe voluntary revision, furnishing, growth, watching or satisfied finishing. Reject Normal's management premise if people consistently skip it without losing a valued tradeoff; reject the expressive alternative if every arrangement feels equivalent. A human session would settle preferences better than another scripted success route.

[The active queue](NEXT_CHUNKS.md) is conditional. No next-five convenience backlog is protected. Bring a direction review forward if the first comparison fails; the next regular review remains 95 after five further committed playable outcomes. Documentation, timing tools and comparisons without a new playable experience receive no count.

## Tooling decisions

| Decision | Beneficiary / repeated task | Scope, cost and validation |
| --- | --- | --- |
| Accept alongside 91 | Design, UX and play reviewers repeatedly reconstruct production delays from source and end-state images. | Extend existing experiment/capture events with completion, first work/output/meal, move/resume and input timestamps; paired snapshots and provenance. Estimated half to one day, low upkeep, not a new runner. Measure setup/review time for both variants and require the bundle to explain the idle interval. |
| Accept before preference comparison | Players/reviewers need fair access and continuous evidence. | Equal visibility of founding/inhabited choices at 960; one ordinary-control session from entry through an uninstructed revision. Reuse existing capture; no replay framework. Label prompted/scripted actions. Selection rate alone is not preference. |
| Accept when investigating recurrence | Developer/user need an attributable save failure. | Correlate existing slot/Continue diagnostics, operation duration and frame markers. Estimated half-day correlation work; investigation unknown. Success criterion is exact failing operation and surrounding timing, not another successful save. |
| Reuse before art/audio acceptance | Visual/audio reviewers need matched scenes and audible evidence. | Existing paired capture and soundscape tools; quiet/busy1x scenes through a full theme and rest. Small setup cost. Verify a reviewer can actually observe/listen before calling it reviewed. |
| Defer | No demonstrated beneficiary for a general platform rewrite. | ECS, replay infrastructure, generalized editor, public/archive engine split, catalogue-wide asset rewrite and new needs. Variable plots and multi-place planning require a concrete blocked composition first. |

The 31-suite report now has explicit run identity and completion counts; do not reopen that completed work as a fresh infrastructure project.

## Validation and provenance

Zero-warning final build. All **31 current-experience suites** completed successfully (`artifacts/review90-current.log`, run `7cd29345a9ff42be8c830ffcad19795e`, report 31/31). They ran before the final UI-only overlap/copy fix; simulation and test assembly were unchanged. **Broad regression passed, exit0**, on the final build (`artifacts/review90-regression.log`).

Fresh fixed90 captures under `artifacts/review/runs/`:

| Run | Evidence |
| --- | --- |
| `20260927-171247-662-first-place-fb39a7` | 960 actual primary entry, four cancelable choices, full catalogue/dismissal, F5/F9, relaxed first home; overlap fix inspected. |
| `20260927-171354-384-first-place-5a101b` | 1440 same first-place input contract. |
| `20260927-171515-661-across-inlet-543505` | 960 inhabited Normal/relaxed controls, furnishing, relocation, save/restart, food view and staffing. Continue-failure warning is an intentional injected fault, not a new denial. |
| `20260927-171427-566-across-inlet-2bbd5d` | 1440 inhabited opening. |
| `20260927-171337-194-player-founded-f3294f` | 1440 established bread branch, imported actual 20-minute state. |
| `20260927-171450-738-player-founded-2a45a4` | 1440 opposite-view fishing branch, imported actual 20-minute state. |
| `20260927-171401-986-dense-6ee951` | 1440 dense archived later-campaign village. |
| `20260927-171428-109-opening-e35c88` | 960 archived campaign opening. |
| `20260927-171450-596-creative-court-2511ee` | 1440 earlier free arrangement. |
| `20260927-171646-811-player-founded-b69832` | 1x bread observation,300 extracted frames/12.5seconds audio. Available evidence only; no listening/continuous-motion/performance acceptance. |

Scripted probes use explicit API scene/camera setup and actual specified input afterward. Endpoints do not establish spontaneous intentions. Reviewer coverage varies: design and lead inspected a narrower image subset; UX, play and visual covered the broad representative set. All five assessed whole-project source/evidence through their discipline, with unobserved areas explicit above.

Checkpoint 89's native960 invitation/meal/home-rest probe also passed on its working build (`20260927-170229-962-home-invitation-2e0d3d`); it is not mislabeled as a frozen90 run.90's initial captures caught a duplicate introductory hint overlapping the chooser; the final fixed build removed it. Review documentation adds no playable count. **Stop at 90;91 has not started.**
