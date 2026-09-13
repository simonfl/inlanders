# F31c2 — cut the food-expansion extension

September 13, 2026. **The founding village does not require another food livelihood.** A homes-only control settles twelve residents in127.4 simulated seconds, then keeps them fed through a fifteen-minute run. Cut the proposed food/woodland difficulty extension. Retain the current founding scenario as a gentle opening. No production code changed and no playable checkpoint increment: count27, review30 remains next.

## Source audit

Each berry patch replenishes one portion every8 seconds, up to8 on the bush: potential7.5 portions/minute per patch before labor and travel. The lake has three patches, and the starting forager can use any reachable patch rather than a local catchment. That is potential22.5/minute against roughly12 meals/minute at the founding target. Potential is not actual delivered throughput; the control below tests real work.

The two fishing habitats replenish4 and7.2 portions/minute before transport. Woodland recovery caps at3 game/minute even with six mature trees. The founding habitat starts with three supporting trees (1.5/minute). Hunting is supplementary food. Loggers may harvest those trees unless the player preserves them. These are existing rules, not new balancing changes.

## Same-start comparisons

Clone `NewFoundingSettlement()`, order the same five cottages at the start, and invite households only when the existing action becomes available. Add no producer, a dock, a hunting lodge, or the same lodge with its existing habitat trees preserved. No food/resource injection, arbitrary depletion, rate changes or terrain overrides. Actual preservation and placement commands only. Each arm runs900 simulated seconds; the ending is recorded without stopping daily life.

| Arm | Ending available (seconds) | Unfed resident-seconds, entire run | Meals in final300 seconds | Berries / fish / game eaten | Final stored food |
| --- | ---: | ---: | ---: | ---: | ---: |
| Homes only | 127.4 | 0 | 61 | 174 / 0 / 0 | 33 |
| Dock | 128.6 | 0 | 59 | 98 / 75 / 0 | 173 |
| Hunting lodge | 128.7 | 0 | 60 | 173 / 0 / 1 | 35 |
| Preserve habitat + lodge | 128.7 | 0 | 58 | 151 / 0 / 20 | 58 |

The homes-only route gathers174 berries while eating174, so survival over this window is not just spending its initial40 stored portions. All arms finish with twelve housed residents. Preserved woodland retains three supporting trees and1.5 game/minute; the unpreserved hunting route loses all supporting trees. Preservation demonstrably affects food and scenery, but founding success does not need that benefit. These runs share cottage cells/facings; the preserved and unpreserved hunting arms also share lodge placement.

Inspected matching1440px normal-process stills at900 seconds: [harvested woodland](images/founding-woodland-harvested.png) and [preserved woodland](images/founding-woodland-preserved.png). The northern trees remain in the preserved arm, while timber is harvested elsewhere. Both villages retain the same crowded home layout. This is a visible choice about village character, not evidence of a deeper challenge or player preference.

## Correct the design test as well

A script that knows good coordinates and queues them while paused is not a human novice. Successful bulk ordering alone does not invalidate planning gameplay. Requiring arbitrary follow-up clicks or mid-run disruption would manufacture activity rather than satisfaction. Compare choices the player could reasonably make and their understandable consequences; allow thoughtful up-front planning to succeed.

The stronger finding here is that a control ignoring the proposed livelihood decision entirely still achieves the objective and supplies itself over fifteen minutes. Reducing berry stocks, raising the resident target or adding a diet certificate would rescue the desired test result without giving the player a better purpose. Do not ship those changes. Also do not erase preservation: it has demonstrated effects and may be useful in a project that actually values them.

## Selected next chunk: F31d — a lakeside public-works project

Give the established village a concrete thing to make: a gathering hall, using the existing timber → sawmill → planks and quarry → stone chains. Add a reachable far-shore stone outcrop to this authored settlement rather than a new resource/producer. Keep the existing opening and offer this as an optional continuation in the same Village panel, not another mode or five-level campaign rollout.

Choose the hall's site and how to supply it. A home-side hall favors daily use but brings stone farther; a material-side site shortens construction journeys but asks whether homes/services should follow. Compare those layouts and a recoverable awkward site. Preserve ordinary construction, material recovery, food and recreation. Thoughtful bulk planning is allowed; mandatory move counts, attendance streaks, reserve quotas and timers are not. A finished hall and ordinary use can support a player-declared ending, not an economy certificate.

The existing quarry campaign already proves these production chains and some route differences (360–480 simulated seconds on its own map), but its rolling assessment is specifically excluded. Reuse the systems, not its certification flow. This is a bounded playable bet about making a place worth building, not a claim that existing hall assets guarantee fun or a commitment to longer play by numbers. Judge visual purpose and actual journeys; if both sites feel interchangeable, report that and revise the project rather than add gates.

## Verification and provenance

Build and all four900-second arms passed. State/resource validation runs every10 seconds and final current saves round-trip exactly. Run `./Review.ps1 Build`, then `.tools/dotnet/dotnet.exe Tests/bin/Debug/net8.0/SimulationTests.dll --founding-land-use`. Ignored `artifacts/founding-land-use` contains the shared initial world, ordered starting worlds, final worlds and event/result JSON. This is a bounded test, not a new experiment framework.

Fixed base `1e082e6` plus this chunk's test/dispatcher. Source fingerprint `1982F32885A4772A5296CE47BF6E0BF3C8627F46C3717BF98964D66E27B4699A`; game assembly `C7CEE562DAD22C210BD9C335817E62B07C6E11EF440EEA64162ABB9F3A07D830`; test assembly `7C0538EA565AA6A50BBCC38F56E53D3979F25C07C9763735ED5B6DE7B2C6ABA1`. Existing review runner loaded hashed measured worlds; local view bundles are `artifacts/founding-land-use/views/{woods,preserved-woods}/capture-0001` with request/manifests. No native/uncoached play, continuous observation, listening, human pacing/preference or frame-performance acceptance. No independent or periodic whole-game review claimed.
