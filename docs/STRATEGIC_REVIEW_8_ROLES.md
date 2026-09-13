# Checkpoint 8 strategic review — disciplinary findings

September 12, 2026. Reviewed source: `aea4dc40e49d2b88ed25723dce33c1318cb8e605`. Existing assembly SHA-256: `525634AE57CD9C88EE49ED1078AF5666488A40A411D2C283720C6002FE619B1F`. No new build was produced for this review. These are condensed reviewer reports; the [synthesis](STRATEGIC_REVIEW_8.md) owns the decisions.

## Method and independence

Three read-only agents independently reviewed game design, UX/onboarding and development leadership under the [critical mandate](DESIGN_REVIEW_MANDATE.md). Attempts to create fresh playtest and presentation reviewers failed with `agent thread limit reached`, including after initial reports completed. The UX agent therefore performed the playtest pass and the designer performed the visual/audio pass with their previous context. Five disciplines are covered, but this is **not five independent reviewers**. Carry that limitation into the next cadence review; do not weaken the standing policy to make this run appear compliant.

All three initial reviewers judged the simulation partially convincing and the campaign direction unconvincing. Agreement is meaningful but not a player study: they share source, intent and historical evidence. No reviewer establishes human enjoyment.

## Game design — strategy8_design

The valuable activity is arranging an inhabited landscape and seeing the arrangement affect everyday life. Current campaign progression increasingly substitutes a certification system: mandated components, assessment initiation, rolling service predicates, population expansion, repetition.

Concrete objections:

- River objectives count recent east-bank Square visits while excluding gardens/halls. This is a rule about a particular implementation rather than a recognizable neighborhood accomplishment (`Simulation/RiverCampaign.cs`).
- Bakers always fetch grain from `YardAccess` (`Simulation/Food.cs`), defeating the intuitive farm-next-to-bakery chain. The [finale comparisons](CAMPAIGN_REVIEW_F11D.md) demonstrate the consequence for tested layouts; explaining it does not justify retaining it.
- Eighteen buildings provide breadth, but recreation venue distinctions and comfort timers have not demonstrated equivalent experiential depth. Mixed-food assessment reduces six edible resources largely to dominant versus other food.
- Five opening recipes delay meaningful settlement strategy. Adding quotas or more minutes would not repair the later assessment loop.

Keep physical construction, transport, growing crops, geography, residents, communal gatherings and recoverable rebuilding because they make decisions observable. Cut manual assessments, arbitrary venue qualification, mandatory coverage of every building, and repetitive population-invitation thresholds from the alternative. Retain diagnostic counters for development evidence. Test local supply and workplace-first staffing. Treat recreation venues as a family until their differences earn a strategic purpose; withhold new mandatory needs and consider comfort primarily as visible customization.

Recommended alternative: a living neighborhood game with competing land uses, comprehensible local supply, growth as a commitment and a visible arrival/gathering payoff. A coherent competing direction is an explicit logistics puzzle game with restrictive hubs and exposed flows; pure village-making is also plausible. Neither deserves rejection solely because it would discard work.

Experiment: one existing west-bank hamlet, two plausible eastern arrangements, homes, contrasting food approaches, crossing and communal space. Real travel, meals, building and recovery remain. Failure of the bet includes obvious placement recipes, trivial local supply, a single dominant arrangement, continued explanatory essays, or an event that merely disguises another counter unlock. Observe whether the player wants to keep watching or improving after success.

Evidence: current campaign/economy/needs/Creative source, viewed ordinary river and historical dense screenshots, prior native play and simulation reports. No new play or listening in this pass. Costs mostly lie at 4–8 logs with uniform construction time; do not broadly rebalance prices before evaluating labor, land and journey opportunity costs in the slice.

## UX/onboarding — strategy8_ux

The interaction model makes simulation administration compete with arranging and watching. At 960px the observed river view gives roughly 430px to two scrolling drawers. Goals explains accounting windows while the selected garden prominently explains demolition. The immediately useful question—whether this site helps the neighborhood—is difficult to answer from the world.

`CampaignUi.cs`, `ServiceCoverageUi.cs`, `EconomyUi.cs` and `WorkplaceAssignmentUi.cs` expose closed requests, historical failures, delivered versus transferred stock, demand, profession assignment and optional exclusive workplaces. These distinctions may be legitimate internals; making them necessary player knowledge is optional. A typography pass cannot solve that burden.

Keep physical activity, camera controls, pause/speed, contextual selection, place links and Creative expression. Remove assessment management from ordinary play. Redesign staffing around visible building slots and a shared-worker default; keep individual policies only if useful. Make local supply spatially truthful. Prefer one compact objective and contextual world feedback, with diagnostics deliberately opened. Put production/staffing/problems before demolition. Bring ordinary management closer to Watch mode's clarity.

Compare current and neighborhood variants on the same river situation. Ask whether a player can predict placement consequences, explain a shortage without an essay, recover in two plausible ways, spend decision time with the village, and name a satisfying moment beyond passing a threshold. Reject a redesign that becomes service-radius stamping or removes tradeoffs without improving comprehension.

Tool needs: named scenarios and matching evidence; lightweight interaction events (selection, panels, refusals, staffing, objective changes) with separate wall/simulation clocks. Do not build a universal editor or automated UX score.

Evidence: source, two viewed screenshots and prior native observations. No new native play, animation or listening in the initial UX pass.

## Development lead — strategy8_lead

Godot/C# and the independent simulation executable are adequate for testing a different game. Physical transport, construction and demolition recovery deserve retention for visible consequences. A technology rewrite does not resolve the product question.

The late campaign exposes internal service measurements and privileges central bread as its culminating reward. Existing smoke coverage makes implementation proof accessible while alternate complete experiences remain expensive to reach. This process asymmetry encourages incremental correction.

Do not assume all ten levels deserve ongoing adaptation during redesign. Prototype one complete neighborhood experience. Remove assessment buttons and mandatory system breadth there; redesign production geography, progression and site interactions. Compare a more radical village-making direction if spatial management still fails to engage.

Ranked tooling proposals, with estimates rather than delivery promises:

| Investment | Beneficiaries / frequency | Estimated effort / upkeep | Payoff evidence |
| --- | --- | --- | --- |
| Scenario catalog and inspect launcher | All contributors, several times per chunk | 1–2 focused developer days; low if registration is single-source | One command to a valid scene; measure setup time and stale/missing fixture failures. Target under two minutes with an existing build. |
| PNG/state/run metadata bundle | Reviews and debugging each iteration | About one day initially; synchronized motion may add 1–2 days | Usable evidence time, failure rate and ability to identify exact state/settings. |
| Narrow comparison runner | Design experiments | 1–2 days alongside prototype; low/moderate upkeep | Second experiment supplies only preparation/variants/observations, without copied export loops. |
| Focused validation and fixture reuse | Every edit cycle | About one day initially; invalidation needs care | Edit-to-inspection wall time; deliberately stale artifacts rejected. |
| Audio audition/native performance capture | Presentation and stalls | Hours to inventory exports; bounded 1–2 day profiling investigation | Actual listening or attributed stalls, not more numeric proxies. |

Reuse found in source: `Smoke.cs` already exports viewport PNGs; `SmokeSoundscape.cs` records actual mixer WAVs; `ReviewSoundscape.ps1` builds a listening page; `SmokeComposition.cs` provides matched camera views; `Tests/FinaleAlternativesReview.cs` provides shared-baseline comparisons. `Play.ps1` combines unconditional builds, many switches and sometimes substantial fixture generation. Some other inspections silently depend on externally generated fixtures. Scripted sound captures use 4× while player controls use 3×/6×: record execution mode and rate rather than presenting them as equivalent.

Defer ECS, universal editor, replay framework, broad config conversion, new CI platform and save migration. A small asset-authoring seam or rule override is warranted only when used by the actual experiment. Evidence is source and documented prior work; no fresh runtime/performance/listening claim.

## Visual/audio — strategy8_design, reused context

Viewed title, ordinary/dense village, lake, settled river and native river stills. The title is stronger through larger forms, vegetation framing and negative space. In play, repeated pitched roofs dominate pale slabs; rigid shore edges expose the grid; residents and activity are small and occluded. Buildings have geometric depth, but the user's impression of flatness is valid at normal scale. More small detail would risk adding noise.

Keep the miniature viewpoint, restrained warm/cool palette, construction stages and distinctive major forms provisionally. Stop defaulting to prop passes, roof recolors and close-up acceptance. Redesign terrain/shore language, massing/silhouettes, outdoor work/social spaces, framing and the space left by management UI. Test resident visibility in motion before changing global scale.

Preferred alternative: an authored storybook landscape with shaped shores, groves and clearings, stronger building masses and a readable inhabited center. A competing crafted-tabletop direction would deliberately embrace beveled islands, bold modular shapes and graphic materials. The current scene does not fully exploit either. Neither requires photorealism or a new engine.

Compare one complete playable neighborhood, current versus alternative, at ordinary/close zoom, four orientations, normal UI and clean view, plus 30–60 seconds of activity at 1× and a busy accelerated view. Reject an alternative appealing only in a staged screenshot, obscuring work, requiring costly one-off dressing, or failing to improve user preference. Test a narrow authored Godot scene/mesh seam with a home, workplace and shared structure only if it accelerates massing edits; measure editing effort before broad migration.

Audio source and prior recording documents were inspected, but **nothing was listened to**. Existing synthesis, voice bounds and exports establish neither appeal nor unpleasantness. Audition actual quiet/busy/waterfront mixes and complete existing music before creating more themes. Expose the existing exporters through the launcher; do not build a second audio framework.

## Playtest — strategy8_ux, reused context

The reviewer read the computer-use skill, initialized `@oai/sky`, launched owned Godot process 23448, selected returned window 3541118 and obtained an activated main-menu capture. Initial activation/capture exceeded 31 seconds before yielding and subsequently succeeded. Native capability works. Root ended the bounded attempt before campaign input; **no fresh gameplay was performed**. Alt+F4 and a subsequent process lookup confirmed the owned process absent; root also checked absence.

The fresh menu observation found clear primary choices and a pleasant uncluttered miniature composition. This is not evidence of gameplay appeal. Prior opening completion and river assessment remain evidence from [F11d/F18d](CAMPAIGN_REVIEW_F11D.md), not play performed by this review. Whole-game playtest status is unverified by new play; the UX verdict is source/still inference. Future comparison requires actual ordinary play, including mistakes and recovery, with accelerated waits. Slow orchestration reinforces T01's value but does not justify relabeling scripted checks as a playtest.
