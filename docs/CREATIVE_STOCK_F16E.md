# F16e — Creative central resource setup

Options now offers a resource picker, current central quantity, reserved minimum, desired quantity and explicit **Apply central stock**. All ten resources are supported; values range from 0 to 9,999. Typing does not apply. **Use current quantity** discards the draft, and loading/replacing a settlement resets it. Ordinary play does not expose or permit these edits.

Edits affect central storage only. Reserved material inputs (including comfort and hauling), bakery grain and central food pickups set the minimum. Apply checks current reservations again. Carried goods, local inventories, raw deposits, crop growth and completed construction do not change. Workers keep their jobs and can consume protected reservations normally.

## Accounting and saves

Separate added/removed ledgers record Creative edits; production, initial resources, recipes and finite deposits remain truthful. Conservation includes the net adjustment for each resource. Cumulative food delivery excludes these adjustments, so setup cannot fabricate a worker delivery. The UI displays ledger totals; injected conditional resources also become visible in the top bar.

Save format is **35**. Earlier versions are rejected; no migration. Ledger entries must use valid resources and positive totals, with a one-billion-unit lifetime limit per direction/resource. That bookkeeping limit protects arithmetic; it is not a capacity increase. Normal saves require empty ledgers.

## Verification

`Test.ps1 -CreativeStock` covers every resource, no-op/invalid edits, separate production and delivery history, active bakery/sawmill/pantry reservations, protected minima, real continued work, unchanged carried/local stock and exact saves. These checks are also in the full simulation suite, which passes with the new conservation equations.

`Play.ps1 -CreativeStockSmokeTest` covers unapplied drafts, actual Apply clicks, additions/removals, resource-bar visibility, save/reload draft reset and normal-mode exclusion at 960/1440. Captures are under `artifacts/creative-stock`. Shared HUD regression checks the surrounding interface. Fresh snapshots are required when replaying historical render fixtures.

## Roadmap review

F16e is delivered. Audio/music audition and human campaign feedback remain open. The next independent candidate is local stone storage (F07d): compare actual quarry-to-yard-to-project travel against local staging before extending stockpiles. Use existing quarry/civic scenes and preserve all reservations, finite-source accounting and physical cargo; do not add a new material consumer solely to justify storage.
