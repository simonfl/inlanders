# Les Habitants — thematic direction review

September 19/20, 2026. Fixed source commit **`e3f575ff2219f20bdad3945748281eda08b129f1`**, after F33a and T07. This is a planning review at playable checkpoint **36**, not a new playable outcome or a replacement for periodic reviews **40 and 45**.

The user chose 17th-century French settlers in New France, in modern Quebec, farming and living from the land to make a better life. **Descendants are narrative motivation only. No inheritance, aging, genealogy, succession, generational handoffs or legacy scoring.** Inlanders remains the internal codename; Les Habitants is the planned public identity. F13 seasons remains removed.

## Whole-game verdict and decision

**Partially convincing foundation; refocus the experience around establishing a livelihood and improving an inhabited farm settlement.** Physical work, meals, homes, useful paths, recoverable arrangements and optional growth support the theme. The current opening and generic roof clusters do not yet make agriculture, the land or visible improvement central enough. A title and facade change alone would leave that gap.

The strongest case against the present direction is that the founding village can succeed by adding homes while its starting food setup does the essential work. Prior route evidence includes twelve residents fed for fifteen minutes without establishing a new livelihood. The optional hall then supplies a construction recipe and first-visitor finish, rather than evidence of sustained interest. Comfortable homes reduce rest demands, but prior comparisons did not establish a food-output gain. Those are useful observations about the rules, not proof that the player has nothing enjoyable to do.

The most defensible pleasure today is shaping a place and seeing people use it. We retain physical supply and arrangement because they connect player decisions to visible journeys; whether that sustains interest remains open. All eighteen buildings do not need protection or replacement at once. Current occupation/rest/recreation, shared work and forgiving moves are enough to test the new direction before adding needs or production chains.

## Independent disciplinary findings

Reviewers worked read-only against the same source commit before synthesis. Two contexts were fresh; three were reused after agent/thread capacity prevented another fresh allocation. All received the no-inheritance correction. They did not edit the project.

| Role / context | Verdict and strongest contribution |
| --- | --- |
| Game designer — `habitants_design`, fresh | Partially convincing. A cultivation-led homestead is a substantial alternative to the current composition-led opening. Useful ground, retained woods and labor should create choices; mandatory farmers or bigger quotas would only disguise the weak decision. Test comparable ordinary approaches and a poor but recoverable arrangement. |
| UX/onboarding — `habitants_ux`, fresh | Partially convincing. F33a makes food choices and finish actions visible, but a free instant-building introduction followed by normal founding and an optional hall does not teach one coherent livelihood. Prefer contextual teaching in the actual first settlement, clear Free rules, optional completion and readable consequences. |
| Playtest — `full19_playtest`, reused | Theme gives a clearer purpose, but neither enjoyment nor its identity is established. Compare land commitments, work, actual meal collection and recovery; then let the player choose an improvement, continued growth or stopping. Reject the experiment if both approaches become the same producer package or only a report reveals the payoff. |
| Development lead — `full19_lead`, reused | Retain Godot/C#, existing simulation and local save workflow. Change geography and the player experience before building a historical framework. A short, satisfying settlement-making game is a valid alternative if extended growth continues to repeat itself. Prefer small display-name/reference investments and existing comparison tools. |
| Visual/audio — `commons_presentation_review`, reused | Promising foundation, insufficient identity. Roof clusters and incidental fields need a whole-scene composition change. Retain warmth and readable work, but reconsider proportions, cultivated area, shoreline and household surroundings together. Audio identity is unjudged until listened to. |

The roles agree on the weakness of a reskin and on avoiding compulsory growth, extra meters and historical feature accumulation. They differ in emphasis: cultivation-led management versus village composition; a contextual opening versus short independent episodes; economic improvements versus improvements that are mainly satisfying to arrange and watch. The evidence does not settle those choices. The first themed situation will compare them rather than silently treating all as requirements.

## Direction alternatives and selected experiment

| Alternative | Value | Main risk / decision |
| --- | --- | --- |
| Rename and restyle the current opening | Small scope, preserves a working playable route. | Leaves its automatic livelihood and generic project ladder intact. Useful as a control, insufficient as the theme integration. |
| Establish and improve a farm settlement | Connects land, production, home surroundings and visible activity. | Could become crop waiting and duplicate producers. **Selected experiment:** one authored river/field/woodland situation, using current ordinary rules. |
| Primarily arrange a picturesque village | Strong fit with free moves, paths and watching daily life. | Could make survival language misleading or leave no reason to continue. Retain Free arrangement and consider this direction if its experience is stronger than management. |
| Short settlement episodes with satisfying endings | Offers distinct practical problems without a repetitive growth ladder. | Could recreate a tutorial/checklist campaign. Keep five loose situation sketches; commit to further episodes only after the first offers an actual choice. |

The selected slice compares cultivation-led and mixed-land strategies on the same map, population, starting resources and rules. These are player strategies, not preset classes or a route menu. A modest village that never grows must remain valid. Author initial supply and useful land so the player has a livelihood to establish; do not compensate for a self-sufficient starting setup with a farmer quota, source ban or global berry nerf.

Once supply works, observe a self-chosen improvement and actual resident use. A field/home route, storage change or modest shared place can compete with inviting more people. Preserve mistakes and recovery. Compare the inhabited scene with current founding and Free at ordinary zoom and opposing cameras. Reject or revise if the choice is bypassed, one strategy trivially dominates, both converge on the same package, or the benefit is understandable only through statistics. A technically successful result still needs player feedback.

This replaces the generic F33b shore/woodland comparison with an agrarian one and adds F34a public identity/reference work and F34b whole-farmstead presentation. F33c comparison and F33d integration remain, with revised acceptance. It does not authorize five themed levels, a catalogue rewrite, new family systems or annual seasons. Retain the working public routes until a replacement is playable; archive obsolete plans instead of leaving multiple active queues.

## Tooling decisions

Effort estimates below are rough planning estimates, not measured delivery promises.

| Decision | Beneficiary and expected saving | Cost / maintenance | Validation |
| --- | --- | --- | --- |
| Accept a small public-title/display-name boundary | UI work avoids repeated branding misses while internal identifiers stay stable. | Roughly half a day or less; low upkeep. Reuse current building-name access; no general localization system. | Check menu/HUD/window at 960/1440, ordinary launch/resume, and unchanged serialized names/paths. |
| Accept one dated, place-specific reference sheet | Art, scenario and UI work share decisions rather than repeatedly researching inconsistent buildings and clothes. | Roughly half to one day; update when the chosen palette changes. | Each representative dwelling, plot, work structure, landing and person/tool reference is supported, explicitly stylized or marked provisional. |
| Reuse matched snapshots, captures and frame summaries | Designer, playtest and presentation reviewers can compare the same situation without rebuilding it by hand. | A few hours of setup per comparison; existing tooling, little new maintenance. | Reproduce the same inputs and opposing views; retain an imperfect recovery branch and actual activity. |
| Prioritize a short observation and audio audition that reviewers actually consume | Resolves the missing comprehension, motion and listening evidence that another export cannot answer. | Preparation under half a day; session availability remains a constraint. | Record what the player understood/chose and what was heard or seen, with timestamps and honest limits. Do not mark acceptance just because files exist. |
| Defer speculative performance and platform work | Avoids maintaining systems without a reproduced problem or design beneficiary. | No new framework. T07 did not reproduce a sustained version of the earlier stalls. | Profile when a recurrence appears; measure before choosing a fix. |

## Evidence, coverage and limits

Source and existing evidence cover current founding, the short introduction, Free arrangement and older campaigns; building/resource/need relationships; F33a controls and onboarding; ordinary/dense village stills; audio implementation; saves and simulation/interaction traces. Historical scenarios are context, not the selected progression. Relevant project records are [review 35](REVIEW_CHECKPOINT_35.md), [F33a](CHOICES_F33A.md), [T07](INTERACTION_T07.md) and the F32 comparisons linked from the roadmap.

No new native playthrough, build/test run, motion viewing, listening or uncoached human session was performed for this planning review. The playtest role produced an evidence review and experiment design, not a fresh playthrough. Historical accuracy was not independently assessed by the five agents. The lead consulted the primary museum/heritage sources cited in [the theme brief](LES_HABITANTS.md); exact date/place filtering remains future work.

The fixed identifier above is the source baseline. F33a's existing captures were produced on a dirty `a8c4c48` tree with pending implementation changes; review-35 stills are older again. They illustrate the delivered UI and visual concerns but are not clean-build captures of `e3f575f`. T07 measured scripted native interactions, not perceived smoothness. Neither those traces nor the stills establish enjoyment, compelling animation, an acceptable mix or a convincing historical scene.

The [revised roadmap](ROADMAP.md), [active queue](NEXT_CHUNKS.md), [theme brief](LES_HABITANTS.md) and [campaign concepts](CAMPAIGN_SYSTEMS.md) are the chosen planning result. Playable count stays **36**. Periodic review 40, then 45, remains the prior implementation horizon; substantial presentation work also requires its own visual/audio review.
