# The price of provisions — outcome260

Delivered November10, private build `4490B590F21D`. This is a bounded comparison of consequential intervention with ordinary work still automated, not a longer campaign release. The [research and direction review](AUTOMATION_AND_AGENCY_259.md) selected the question.

Play `/?scenario=timber-trade`. Village also links to `timber-trade-ripe`, with an almost-ripe opening crop. `timber-control` is the same slow-crop setup without a visiting trader, for controlled comparison. The prior crossing and longer river scenarios remain available.

## Experience and rules

Four residents have four portions, twelve logs, a working crossing and a growing field. Four more people arrive after two simulated minutes. A trader at the landing offers sixteen provisions for twelve logs and leaves after one minute. Those twelve logs also build the two cottages needed by the arrivals. Choose once, defer while the offer remains, or let it pass. Pausing stops the deadline. Routine construction, logging, harvesting and replanting remain automatic; no worker-assignment or priority panel was added.

The exchange consumes only uncommitted central timber. Carried/reserved construction loads cannot be sold twice; exported logs are explicitly conserved and saved. Imported provisions remain distinct from local harvests. The small boat visibly departs, but the cargo exchange itself is immediate at storage; there is no new simulated trader-hauling profession.

Completion requires all eight housed and fed, each having eaten fresh local food since arrival, eight portions in reserve, and local deliveries since arrival covering meals eaten with at least sixteen deliveries. Completion pauses immediately. There is no additional stability timer. The local-food row names deliveries and actual meals eaten, including the minimum while relevant. This is evidence of settling this group, not a guarantee of perpetual sustainability.

## Evidence

Controlled simulation routes use identical construction/grove orders apart from crop readiness and trading. These test consequences, not enjoyment or optimal play:

| Crop | Decision | Eight beds ready | Completion | Lowest recorded food |
| --- | --- | ---: | ---: | ---: |
| Newly planted | Keep timber |64s|243s|0|
| Newly planted | Trade |108s|230s|13|
| Nearly ripe | Keep timber |51s|251s|0|
| Nearly ripe | Trade |126s|330s|16|

No hungry resident occurred in those four scripted routes. Trading buys a buffer and delays housing; its completion benefit is not universal. Zero stored food and actual hunger are different observations. These routes do not prove neither choice dominates every possible strategy.

Independent source-blind evaluator played fixed commit `6867d7d`, build `64BD230309FC`, session `74175bd50ad14b57a4c1e880044335b0`, through public legal commands only. Trade stalled a planned cottage; a delayed grove and second cottage recovered temporary housing shortage. Duplicate trade and an irrelevant event-advance shortcut were rejected. Falling food prompted gardens at270.1s and390.1s. Completion at566s had eight housed/fed/fresh,20 food and60 local deliveries versus57 meals. Final construction was175.9 simulated seconds before completion.

Evaluator verdict: **revise, hold expansion**. The choice has consequences, but waiting still dominates stretches. The moving food denominator needed earlier explanation. One focused revision names delivered/eaten directly, corrects the remaining-homes hint and irrelevant advance message, checks a fulfilled opening reserve correctly, and documents API request fields. Rules and balance are unchanged from the independent run. No second independent play of the final wording is claimed. Replaying the evaluator’s accepted commands against the final build reproduced completion at566s with all three objective rows checked; this is a regression check, not another independent decision play.

Builder actual browser interaction at402×681 exercised buying, cottage placement, two-corner grove preview/confirmation,3× automatic work, arrival pause and inspection of remaining goals. It also exposed excessive opening zoom-out; the experiment now uses tighter initial framing. A deliberately underexpanded builder village exhausted food, consistent with the need to expand local production. This was not a completed second human-style playthrough. Final private HTTPS, opening card and post-arrival wording were checked separately. No real-iPhone test, new listening evaluation or human-enjoyment acceptance is claimed.

Engineering checks cover both crop starts/decisions, one-shot/expired offers, reservation safety, imported/local-food distinction, exact simulation save continuation, neglect not winning, arrival auto-pause, background interruption and exact current-build service restart. Existing crossing recovery and longer river routes still pass, including delayed recovery and finish-deficit checks. Client compilation and service build pass. The existing client bundle-size warning remains; no performance improvement is claimed.

## Decision and tooling

Deliver this comparison for owner play; **retain provisionally, hold expansion**. One opening trade has real economic consequences but does not establish a sustained loop of decisions. The cumulative production catch-up may still feel like waiting, especially after late expansion. Do not add more traders, currencies or needs until this is judged. If a clearly understood trade still feels empty, shorten or replace this approach rather than filling it with recurring purchase chores.

A small `Browser/session-summary.mjs` replaces repeated manual journal extraction. It was used on the owner's prior longer session and the independent trade session and checked against their known completion/resources. It summarizes accepted/rejected orders, phase changes, food lows and time after the last order, explicitly refusing to equate these with enjoyment. No new recording framework.

Local evidence: `artifacts/browser/trade260/` contains route results, API/restart fixture, owner/evaluator summaries, independent report and phone captures. Exact reviewed and delivered builds are archived alongside other browser builds. These artifacts remain local. Source and documentation are versioned. Count260 advances once for this playable outcome.
