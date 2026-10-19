# Whole-game review240 — October19,2026

Fixed source: `3b3555307c76419d405f06c06f4758f9f13f7d1f`. Five independent read-only reviewers: game designer, UX/onboarding, playtest, development lead and visual/audio. All returned **partially convincing**. Synthesis completed before selecting241; validation results are appended separately, not retroactively attributed to reviewers.

## Verdict and direction

The simulation is more trustworthy; the game remains unproven. Actual meals, automatic labor, area forestry, grouped building choices, transported materials and meaningful access deserve retention. Grouping and grove renewal have direct positive human feedback. Correctness and more controls do not establish interesting decisions.

Choose a smaller game of authored restoration situations as the next design experiment, not a larger catalogue. Compare reconnecting established distant production with a credible near-home livelihood using existing fishing/gathering/cultivation. Each must offer an intelligible benefit and sacrifice. If one dominates, or players merely follow instructions and wait, redesign the situation or test household/farmstead scale. Do not preserve the island geometry or twelve-person scale by inertia. No new chapters, needs, decorative controls, arbitrary timers or higher quotas until this comparison earns them.

The current comparison is unfair in ways we must expose: the far bridge costs6 logs and unlocks two ripe fields containing40 portions; new island cultivation costs10 logs per empty field. Travel is the main apparent salvage disadvantage. Relaxed removes material and construction commitments, so assess it independently. Earlier successful island comparisons relied on an unintended channel bypass and cannot establish current balance. A scripted expansion at240 seconds proves neither player discovery nor strategic depth. A short rescue success is not proof of a sustainable livelihood.

## Independent findings and disagreement

| Role | Main objection | Preferred241 |
|---|---|---|
| Designer | Established salvage may dominate rebuilding cultivation; apparent alternatives are not sufficient | Credible home-bank livelihood comparison, or correct a concrete blocker and leave redesign explicit |
| UX | Explanation grows faster than clarity; collapsed housing status conceals missed meals | Visible combined housing/food trouble through existing recovery controls |
| Playtest | Scripted branch choice and expansion do not demonstrate reconsideration; Food first is a narrow3–4-portion band | Remove Food first while retaining automatic food safeguards |
| Development lead | Ordinary FOOD count still includes stranded stock although reserves/goals now exclude it | Complete usable-food accounting in the main HUD |
| Visual/audio | Straight intact strips read as puzzle trenches, not disrupted livelihoods; yard invitation competes with recovery | Remove automatic yard invitation from ordinary house placement |

**Selected241: truthful reachable-food HUD.** It fixes a concrete contradiction in the primary feedback without introducing a new control. Retain total/stranded stock in the tooltip and existing Economy navigation. Verify both public modes with disconnected stock and actual rebuilt crossings. This is bounded correctness, explicitly not a substitute for the structural experiment.

Food first remains provisional, not vindicated by its test. Prefer removing it unless the next comparison demonstrates a useful intention beyond automatic safeguards. Harvest-then-pause supports a clearer concrete intention and remains. De-emphasizing unsolicited yard styling is a small later information-priority correction, not a decoration-removal campaign. Combine existing food/housing feedback before adding more panels.

## Whole-game evidence and limitations

- Campaign: two named public situations, newcomer introduction and restoration, with map previews. Useful differentiation, not a substantial campaign. Native240 logs cover scripted menu, recovery actions, work controls, save/Continue and imported simulated completion; no ordinary end-to-end human play.
- Economy/buildings/needs: source and focused checks cover access, reserves, cultivation, construction commitments and recovery. Bread, planks, stone, hunting, orchards and comfort remain optional and have no fresh comprehensive balance verdict.
- Normal/Relaxed/Creative: both public modes receive focused checks; archived Creative and broad free play are distinct experiences. No fresh self-directed extended session in either.
- Controls: entry960 and construction-budget960 stills show useful independent actions but substantial panel dominance. Building grouping and automatic labor have stronger justification than new scheduling knobs.
- Presentation/audio: warm readable low-poly baseline retained. Flood geometry needs to express a proven relationship, not simply acquire props. Dense still has overlapping labels but is a stress fixture. No continuous-motion or listening verdict. Audio source inspection is not timbre/repetition evidence.
- Reliability/performance: access fixes address reproduced demolition/hauling exceptions. Scoped dispatch reachability preserves exact100-tick state while model timing drops3.43 to0.63s; matched short native blocked-state simulation p95160.3 to6.6ms. This does not prove sustained whole-game performance.
- Fixed240 dense run `20261019-130806-788-dense-124175`:32 residents,30 buildings; normal process advances5.0 simulated seconds over2.26 wall seconds at3x. Very short before/after observation, not a recording or sustained3x benchmark.
- Broad84-suite regression and corrected six-branch comparison were pending during independent reviews. Reviewers explicitly rejected the stale old comparison JSON. Fresh results must be labelled separately below.

## Bounded tooling decisions

Prioritize lifecycle/provenance in the existing island report: run ID, test assembly/build, start time, running/failed/completed status, current branch and results. Replace stale output at start. Beneficiaries are every reviewer and debugger; small local change with low maintenance, validated through completed and failed runs. Do not build a replay framework.

Next, source-label deliveries and record material commitments/intervention reasons so kitchen-garden output cannot masquerade as salvage payoff. Event-linked stills can help identify when a player could notice and correct inadequate supply. Budget hours, retain only if another reviewer can interpret the sequence without solver source. A matched60–90s1x/short3x audiovisual excerpt would close motion/listening gaps; no content expansion is justified by absent evidence.

## Validation closeout

Pending at synthesis; fresh results appended afterward:

- Fixed240 corrected Normal comparisons at450s: bridge completes129.8s,12/12 fed,49food; single island field remains incomplete with12food but12/12 fed; adaptive second field placed240s completes362.1s,34food and12/12 fed. Both island routes leave40 ripe portions disconnected. No observed missed meal in these branches. Thus the failure is reserve rebuilding, not demonstrated starvation; describe it accurately. Scripted expansion does not establish player recognition or a fair strategic tradeoff.
- Dense49-frame sample includes setup: wall median26.72/p9558.10/max676.47ms; simulation p9530.83ms. The large startup frame and tiny sample prohibit a sustained-performance claim.

- Relaxed bridge completes142.3s with44food at450s. Single-field and adaptive-policy branches both complete128.0s with32food; the adaptive policy correctly never expands. Initial broad run stopped after15 passing suites because the comparison incorrectly required an expansion in Relaxed. Corrected the assertion to require that intervention only in Normal; no gameplay numbers changed to satisfy the experiment. Original report/log preserved as240-prefix-report.json and240-current.log; resume starts explicitly at FloodIsland and records selected scope.

- Post-review241: zero-warning game/test build; native restoration probe passes both modes with12 reachable/60total provisions, low HUD warning, then actual rebuilt crossings and recovered count. Existing controls/save/Continue pass. Compact still inspected. No new simulation behavior in241; changes are HUD correctness, probe and comparison/reporting tooling.
