# A lasting village — campaign integration

F11b6 / F18b6, September 12, 2026. **Level ten is playable** from Campaign or Goals, following The living woods. It uses the [tested decision prototype](LASTING_VILLAGE_F11B5.md) and [bread investigation](BREAD_SERVICE_F21N.md).

## What the player does

The working eight-person village occupies cramped western land. Bridge the channel and decide what belongs in the center versus the eastern neighborhood. All seventeen building types remain available. There are no deadlines, forced failures or mandatory production layouts.

1. Prepare at least twelve housed residents, then start the first assessment from Goals. Prove actual reliable meal requests and fresh deliveries for everyone currently present.
2. That milestone stays earned. Expand to at least twenty and start the second assessment. Prove food again, with recent completed home rest for three quarters of residents and recreation visits within two minutes for half. Squares, gardens and halls count; no compulsory diet mix.
3. Both assessments stay earned while the village prepares supper. Provide two unreserved central loaves per current resident, homes for everyone and reachable gathering space. Meals continue to consume bread. Goals links to bread deliveries, actual eating, storage and bakery/pantry controls.
4. Host supper. Once everyone arrives and shares it, the campaign completes immediately. There is no additional post-supper assessment or wait.

Extra residents during an assessment increase its actual meal/rest/recreation requirements. Arrivals after the assessments are earned increase housing and supper cost without restarting those milestones. Supper cannot begin before both assessments, so an early eight-person gathering cannot bank completion. During gathering, Goals shows arrivals rather than requesting the bread already spent.

Completion offers continued play or replay. Replay retains the previous village for restoration, including completion records. Selecting a different settlement now resets Goals to its objectives instead of retaining the level-list scroll position.

## Verified routes

Times are simulated seconds from creation, not measured human play times.

| Route | First assessment earned | Expanded assessment earned | Supper completed |
| --- | ---: | ---: | ---: |
| Central gardens, 20 residents | 247 | 680 | 1584 |
| Eastern pantry/gardens, 20 | 247 | 669 | 1789 |
| Eastern layout, poor bread placement and recovery, 20 | 247 | 669 | 2337 |
| Eastern layout, arrivals during assessment, 22 | 247 | 1210 | 2324 |

The twenty-person routes retain the prototype timings. The twenty-two-person route prepares an additional northern cottage and meets scaled conditions: seventeen recently rested residents, eleven recent recreation visitors and forty-four supper loaves. Its additional home affects travel and subsequent development; it is an example, not an optimized growth route.

The recovery route physically dismantles its remote grain/bread production, reloads after recovery and develops central production with two bakeries. The already-earned assessments remain intact. Both food layouts use that same later bread arrangement; these tests do not establish that all production strategies are equally effective. Earlier prebuilding probes remain documented in the prototype report.

## Checks and remaining questions

`./Test.ps1 -FinaleCampaign` exercises the four routes through real campaign states, early-supper rejection, exact saves before/during/after assessments and gathering, physical recovery and completion-book persistence. The regular `./Test.ps1` suite includes these routes.

`./Play.ps1 -FinaleCampaignSmokeTest` regenerates fixtures and verifies the level picker, phase buttons, unrestricted building controls, service/bread links, scaled supper, actual gathering/completion and replay/restore at 960×640 and 1440×900. The rendered test advances real simulation steps while yielding for rendering; it is not a frame-rate measurement. The woodland smoke check also verifies its Next settlement button reaches level ten.

The full simulation suite and focused finale/woodland rendered checks pass. [Opening Goals](images/finale-campaign-opening.png) and [completed settlement](images/finale-campaign-complete.png) retain rendered evidence.

Human comprehension, pacing and enjoyment remain unverified. The original 40–60 minute ambition is not an acceptance claim. The finale emphasizes development and civic production; woodland restoration remains the preceding level's theme instead of becoming a repeated compulsory gate here.

## Roadmap review

Proceed with **F12g / F23b9 — authored landscape composition**. The opening expansion bank is still sparse and geometric. Improve its natural grouping, shoreline and neighborhood views while preserving the budget and credible alternative plots; rerun routes if geometry affects play. Then review the remaining optional backlog against the resulting settlement rather than adding a new compulsory need by default.
