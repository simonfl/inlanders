# F31b — arrangement comparison

September 13, 2026. **Reject this court as the deeper challenge.** Keep it as an introduction and free arrangement. Deliberate rearrangement changes where people gather, but the easy placement works too well to require it. This is an experiment and direction decision, not a playable checkpoint: count stays **26**, next periodic review **30**.

## What was compared

All five arms clone the same sixteen-resident world at 92 simulated seconds. Actual relocation and commons APIs preserve the starting building count, population and stored food. They continue for six simulated minutes at 0.1-second steps; no extra buildings, food or residents are injected. The automatic nearest-legal search authors fixtures, not recommended player coordinates.

| Arrangement | Action | Shared meals / distinct diners | Peak eating together | Total meals | Meal travel |
| --- | --- | ---: | ---: | ---: | ---: |
| Unchanged | No intervention | 0 / 0 | 0 | 97 | 482.8 |
| Easy placement | Nearest legal commons to original court, at (-1,2); no building moves | 55 / 15 | 4 | 97 | 600.8 |
| Home court | Move two homes west and one existing garden into the home district; place commons | 66 / 16 | 6 | 96 | 536.8 |
| Garden side | Move eastern home out; place commons beside working gardens | 25 / 13 | 5 | 97 | 637.3 |
| Remote | Commons at (-7,-6), away from available food | 0 / 0 | 0 | 97 | 482.8 |

First shared consumption occurred after 24.9 seconds for easy placement, 24.8 for home court and 18.7 for garden side. All five had zero unfed resident-seconds. Food at the end varied (25,18,35,23,25 respectively); this is a snapshot including timing of production and claims, not a sustainable-throughput ranking. Creative rules do not apply a hunger work penalty.

Meal travel sums simulated movement during food collection, walking to eat and meal returns; it excludes other work and hauling. Total meals includes shared meals (the raw report's field name is `ordinaryMeals`). Shared consumption is counted by actual ledger events associated with commons request IDs, not arrival or a pose. This is one deterministic starting state, not exhaustive balance testing.

After six minutes, clone the remote world and move only its commons near food. Continue both for three more minutes: the control still has no first diner; the recovered place records resident14. This demonstrates recoverable siting, not that every poor layout recovers automatically.

## Visible result

Consumed 1440px stills at the same world age and camera:

- [Easy placement](images/spatial-first-legal.png): central ground is already a usable gathering place; the surrounding cottages still crowd it.
- [Home court](images/spatial-home-court.png): a visible working garden joins the homes, but the court remains enclosed and partly occluded. It is not an unmistakable dramatic improvement.
- [Garden side](images/spatial-garden-side.png): clearly exposed seats and working plots form a second place beside the crossing. It has fewer shared meals despite the clearer space.

Also inspected opposite-camera 960px views of easy placement and home court. The central space remains small and crowded in both. No camera trick, mesh change or new normal-game UI was introduced. Still images cannot establish motion quality, listening, player preference or enjoyment.

## Decision and next bet

The home court has a measurable gathering benefit; it is not equivalent on every metric. But the easy alternative already serves fifteen residents, completes the introduction quickly and keeps the village functioning. The garden-side alternative offers visual character without a demonstrated strategic advantage. We should not turn these differences into a required attendance score or longer timer.

**Next: F31b2, staged redevelopment.** Test whether keeping a working settlement supplied while replacing its layout creates decisions that free instantaneous relocation does not. Reuse existing construction, demolition, material recovery, workers and physical food. Compare retaining production during rebuilding with building a replacement food district first, plus a recoverable premature-demolition control. Include a do-nothing control. Prefer an existing constrained site before authoring more terrain.

The hypothesis must earn a playable scenario: two viable strategies, a reason to respond after the first action, and a legible everyday consequence. Existing map/resource rules must provide the conflict; do not manufacture mandatory demolition, move counts, attendance windows, building locks or punitive food timers. If uninterrupted free relocation bypasses the conflict, keep it in Free arrangement and explicitly test ordinary construction rules for finite play. Do not silently change the current introduction or revive an entire legacy campaign. If staged rebuilding is mostly waiting or routine bulk placement, reject that bet too and reconsider the core activity before F31c.

F31c scenario delivery is blocked on this evidence. No additional needs, catalogue expansion or five-level rollout is justified by this result.

## Reproduction and provenance

Run `./Review.ps1 Build`, then `.tools/dotnet/dotnet.exe Tests/bin/Debug/net8.0/SimulationTests.dll --spatial-choice`. Outputs under ignored `artifacts/spatial-choice` include starting, two-minute and six-minute worlds, the same-age recovery pair and `report.json`. Checks cover legal moves, equal starting resources/capabilities, periodic world validation and exact current-save round trips. Build and the full five-arm comparison passed.

Review fixtures: `spatial-unchanged`, `spatial-first-legal`, `spatial-home-court`, `spatial-garden-side`, `spatial-remote`; e.g. `./Review.ps1 Capture -Scenario spatial-home-court -Width 1440`. These are developer fixtures, not more menu choices. The three rendered fixture hashes exactly matched the corresponding measured six-minute worlds.

Fixed tested source: base `bb71fdfd47d954fa196ba975a3803cd0e0bba406` plus this chunk's test/catalogue changes. Source fingerprint `98483A8EB5F18951728C1FCF29454C39391BAAC2D95C2B1B2328E4B2852C4D91`; game assembly `4A5CF46C0F0EF82E773373D7B6F59D8F1DCF00067F33AADB2C7EBA1ACD80F6E6`; test assembly `945C39DD3FDFFF2A5DDFE09B1B785600377448B7DEBFA72CB01D67BF719676D0`.

Local capture runs under `artifacts/review/runs`: `20260913-210034-231-spatial-home-court-a88629`, `20260913-210057-609-spatial-garden-side-5d3d22`, `20260913-210135-783-spatial-first-legal-94da1c`, `20260913-210147-290-spatial-first-legal-3df1f8`, `20260913-210208-830-spatial-home-court-47298c`. Normal Godot process, starts paused; no uncoached/native human play, continuous observation, listening or frame-performance acceptance. No periodic or independent whole-game review claimed for this bounded experiment.
