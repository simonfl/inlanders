# Outcome262 — a trade that can use upcoming timber

Delivered private build `1B01F85B1539` at `/?scenario=timber-trade`. Fresh start uses the same small action set. No backward-save migration.

## Decision and evidence

Replace the cash-at-arrival purchase with one optional timber shipment. Preserve the quiet opening and paused trader arrival. This is a revised trade commitment, not another timer adjustment or a larger merchant system.

Owner journal `ed678c3b6ff24dc1879d82d1447d3ad0`, build4CD7923F0CB7: dock ordered18.1s, a40-log grove24.5s, trader45.1s with only4logs available. The owner declined, then ordered a field and three cottages. No sampled later frame had12available logs. Merely waiting for sufficient stock would have hidden the trade or offered it after the useful moment. The owner's actions were reasonable; the setup required foreknowledge.

Independent design consultation recommended a pay-on-completion order over resource-gated arrival, smaller purchases or debt. Hypothesis: committing upcoming timber to food remains a usable choice after normal early spending. Falsifier: provisions consistently arrive after local food has solved the need, or acceptance is predictably inferior. Keep this bounded comparison; do not claim sustained engagement from a functioning order.

## Rules

- Agree to pack12logs for16provisions, even with0logs stored. Available central timber is packed immediately and as deliveries arrive, before new worker claims. Existing reservations and carried loads stay with their workers.
- Food arrives only when all12logs are paid. Packed logs are accounted for separately from village stock and exported timber; current-format saves retain the order exactly.
- Cancel an unfinished order to return its packed logs. No provisions/refund duplication, no re-opening a cancelled or completed deal.
- The60s offer window governs acceptance only. An agreed shipment has no fulfillment deadline. The trader stays while packing; delivery pauses the game. An optional unfinished shipment may continue after the village objective is achieved; it creates no unpaid food and is not an additional victory condition.
- The existing trader card shows progress, the effect on new construction and cancellation. No additional worker control, catalogue entry, loan, interest or recurring trade. The opening still offers a grove suggestion and the trader arrives at45s.

## Controlled comparison

`Tests/TimberShipmentChecks.cs` replays the recorded dock, grove, field and cottage positions/timing, comparing acceptance and decline with slow and nearly-ripe initial crops. All four routes completed, with no hungry residents in these particular runs. These are scripted mechanics comparisons, not enjoyment evidence.

| Start | Decision | Logs at offer | Provisions arrive | Food just before | Eight beds ready | Village complete |
|---|---|---:|---:|---:|---:|---:|
| Slow crop | Decline |4|—|—|174.1s|224.2s|
| Slow crop | Shipment |4|96.9s|4|200.9s|329.4s|
| Nearly ripe | Decline |6|—|—|172.7s|223.1s|
| Nearly ripe | Shipment |6|75.6s|8|202.8s|287.6s|

Food reaches the village before the newcomers, while packing delays needed housing by roughly27–30s. Declining finishes faster in both recorded-layout comparisons. Acceptance creates a larger food buffer, not a demonstrated optimal strategy. The nearly-ripe start makes that insurance less pressing. No broad economy rebalance or new claim that docks and fields are equally strong.

Focused checks also cover zero-log agreement, no unpaid provisions, pending orders surviving the old departure time, partial cancellation/refund, duplicate-action rejection, preserved material reservations, exact packed-order save continuation, both crop/decision routes and timber/food conservation. Native campaign/Creative, audio, real iPhone touch behavior and dense performance are outside this experiment's evidence.

## Independent play and delivery

Runtime evaluator used the ordinary public observation/preview/command API on fixed `b5de300`/`A7DE08841F8A`, without reading source, authored solutions or this comparison. In session `46b781ae5fc944a8bad39c6832b01f5a`, an independent homes-and-grove opening agreed with8available logs at45.1s; provisions arrived50.6s while the crop was84%grown, preceding the first observed local food delivery/fresh meal at93.5s. Completed372s with eight fed/housed/fresh meals,22food and34local deliveries versus33meals. Both cottages were already supplied before that trade, so its housing sacrifice was not established by this independent route; the matched recorded-layout comparison above supplies that evidence.

Recovery session `677f159d04d94883a6c18258af0e45c7` accepted at0logs with no free food. Order survived105s, then four packed logs were refunded exactly at150.1s and used for a completed garden. Duplicate acceptance, repeated cancellation, re-buy after cancellation and completed-order refund were rejected without resource changes. Independent verdict: provisional keep. Full report and raw responses are under `artifacts/browser/shipment262/REPORT.md` and its adjacent files.

Builder ordinary402×681 UI checks spent initial timber, accepted at0logs, cancelled, then verified the final build's6/12packing card and reclaimed exactly6logs with no food. Screenshot: `artifacts/browser/shipment262/packing-phone.png`. Final wording shortens repeated explanations to preserve map/control space; economics remain fixed. These checks are distinct from API play and human preference, with no actual-phone enjoyment claim.

Final-build engineering session `f1c7773897584957b87ef722f076263b` saved at45.1s with6logs packed. Service restart preserved time, packed timber, available logs and food; an ordinary grove order then completed the loaded shipment. Focused trade/shipment checks pass, client production/typecheck and server builds pass, server reports zero warnings/errors. Existing Vite large-bundle advisory remains. Exact reviewed and delivered binaries archived as `artifacts/browser/build-A7DE08841F8A.zip` and `build-1B01F85B1539.zip`.

## Next decision

Keep for owner play, with no further trade or catalogue expansion selected. The lead owns choosing and implementing responses to feedback. If a clear and usable shipment still feels pointless, replace the opportunity rather than adding another scheduling condition. The persistent question is what makes the period after the first plan worth attending to; shipping correctness alone cannot settle it.
