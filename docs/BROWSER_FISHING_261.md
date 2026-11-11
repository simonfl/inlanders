# Outcome261 — a shoreline food choice

Delivered private build `5D3C91582153`. Fresh play: `/?scenario=timber-trade`, Food sources → Fishing dock. Also available in the longer river settlement and trade variants; the short crossing keeps its original three actions. No native catalogue migration.

## Why this experiment

The owner described the timber trade as **“A real choice, but quiet afterward.”** Keep that choice and automatic ordinary labor. Test one further food commitment: frequent small catches from shared river stocks versus establishing another large crop. The payoff should include watching a resident row out, fish, return and deliver food. More clicks and more catalogue entries are not acceptance criteria.

The independent design specialist considered job priority, early harvest and another crossing constraint. Priority would mostly duplicate existing hunger-aware automation; early harvest risks a repetitive chore with no future cost at the ending; another crossing repeats the tutorial. Fishing uses an existing simulated livelihood with a different timing and location constraint. This remains a bounded hypothesis, not a resolved management loop.

## Delivered behavior

- Food sources groups garden (4 logs / 8 per harvest), field (10 logs / 20) and dock (12 logs / up to 4 fish per trip). A dock costs the timber for two cottages. Ordinary workers build, fish and deliver automatically.
- Preview distinguishes dry entrance from blue water launch, rejects unsuitable shore, supports rotation and reports available fish, renewal and water distance. Inspection and map labels explain shared grounds. Availability excludes fish already claimed by another trip; it is not a literal count of fish physically remaining.
- The map's two grounds start with16 fish each, renewing6 and4 per minute. Extra docks share them; they do not add fresh stocks. No fishing assignments, new needs or maintenance menu.
- Planned docks show stakes/outline, partial decking during construction, and a finished deck/net rack/crate when complete. Small boats show oars, an aboard passenger, casting net and catch. Fisher positions follow the boat in observations and rendering.
- Cancelling an unfinished plan closes its inspection and fishing labels. Inspection buttons no longer squeeze the description into a narrow column.
- Dock cost changed from8 to12 in the shared simulation definition, including native play. The existing native fishing/lake checks pass. A fishing regression exposed a worker reaching only the rounded entrance tile after repathing; boarding now waits for the final fraction of the walk, preserving valid saved passenger state.

## Balance evidence, with limits

`Tests/BrowserFishingComparison.cs` compares the same purchased-provisions start, grove and two homes, adding one food source at120s. The depleted-ground runs deliberately set ground stocks to zero at the decision: controlled fixtures, not ordinary player actions. Existing northern field remains in every run. Shared reserves and meal timing affect completion; these are not isolated maximum-throughput estimates.

| Ground condition | Added source | First delivery | Added-source food by240s | Complete |
|---|---|---:|---:|---:|
| Full | Field,10 logs |239.8s|4|347.2s|
| Full | Dock,12 logs |174.3s|16|336.9s|
| Depleted | Field,10 logs |239.8s|4|347.2s|
| Depleted | Dock,12 logs |174.3s|10|352.5s|

Initial8-log probes made the dock cheaper and earlier in both starts; revised to12 before independent play. The revised dock supplies earlier small meals at a higher timber cost, while the depleted field route completes slightly earlier. This narrow result does **not** prove broad balance or rule out a dominant opening dock. In particular, the independent early-dock route below is strong. Do not claim that supply depletion forces a second decision in this short level.

## Independent runtime play

Fresh read-only evaluator on fixed commit `c1b0be7`, build `8B274056F3E3`, session `b69c7cc230bd44688bc7b79c97ef360b`. Source-blind public observation/preview/commands; no state edits or authored solution. Declined trade, built dock and grove, then cottages and a second dock. Completed194.2s with all eight housed/fed,28food,28 local deliveries versus9 meals. Recovered a wrong orientation via preview and cancelled a zero-delivery plan. Both docks displayed the same depleted grounds, reaching2 and5 available, then both rested at the food target while stocks renewed.

Verdict: **keep for human comparison**, with real initial timber/location commitments but mostly waiting after the plan. The second dock had little urgency; fresh meals were the last requirement. No matched one-dock baseline in that review, so do not infer its marginal value. Minor findings: guide's older “river only” wording also applies to trade settlements; objective checks can precede completion by one0.1s tick; available-stock changes include reservations.

Raw responses and verdict: `artifacts/browser/fishing261/api-play.jsonl`, `runtime-evaluation.md`. Later client-only changes widen inspection text, remove doubled punctuation and hide stock labels after cancellation. Simulation rules are unchanged from that reviewed candidate.

## Builder verification and presentation

- Client typecheck/production build and server build pass; server reports zero warnings/errors. Existing Vite large-bundle advisory remains.
- Focused fishing/lake scenarios pass: shared catch claims, actual deliveries, complete boat phases, saves/recall/reassignment, blocked water routes, fish meals and settlement progression. All four trade/crop branches and reservation/conservation/neglect checks pass. Four comparison routes above finish and saved returning boats continue identically.
- Ordinary402×681 browser interaction: food menu, invalid/rotated valid preview, construction, inspection, cancel after partial delivery,1×/3× movement and arrival pause. Final client repeated fresh placement/cancel and verified labels disappear. These are desktop phone-size checks, **not a new real-iPhone playtest**. Root UI candidate session `489be673b5aa4325abaca55b3c3cd6cf`; additional final-build fresh sessions remain in the journal.
- Final-build legal-command session `52cfd9f580f1450dbc898103acd73ebf` saved at73s with a returning fisher and4fish. Service restart retained exact boat state/time. Continued to completion170.8s with eight housed/fed and21food. This is an engineering mixed-food route, not independent human play. Short crossing rejects dock commands.
- Independent visual specialist inspected paused phone screenshots: boat hull/oars and aboard passenger read correctly; food menu and planned-versus-built dock information are usable. Shore overlap and tiny cargo weaken visual identification of a loaded returning trip. Keep for this prototype; no motion/listening/human-enjoyment verdict from that specialist. Root separately observed changing positions at1×/3×; no audio changes or listening claim.

Evidence folder: `artifacts/browser/fishing261/`; screenshots `final-boat-phone.png`, `final-menu-phone.png`, `final-inspection-phone.png` capture build479C6CABB705, with identical final geometry/layout;5D3C91582153 adds cancellation-label cleanup. Exact candidate and delivered builds are archived in `artifacts/browser/build-8B274056F3E3.zip` and `build-5D3C91582153.zip`. Native full campaign/Creative UI, sustained crowded performance, full economy and actual phone comfort were not assessed by this bounded experiment.

## Decision and stop

Deliver this comparison and hold expansion. Ask whether choosing fishing felt worth considering and whether watching it was satisfying, without directing the owner to a preferred solution. Keep the reduced grouped interface. No next dock upgrades, production chain or extra needs selected.

If this is another understood choice followed by an empty stretch, reconsider the **scenario's sequence of decisions** rather than adding a fourth food source. A later situation that changes an existing commitment is a candidate for a replacement experiment, not an approved queue. First collect owner reaction to this slice; scripted success cannot resolve the quiet-afterward concern.
