# Les Habitants — theme and design brief

Player-facing title: **Les Habitants**. Working tagline: **A home for generations**. Internal codename: **Inlanders**.

A personal, local Windows settlement game about 17th-century French settlers in New France, in the region now called Quebec: making a living through farming and the surrounding land, establishing homes, and gradually making life better. The mood is hopeful, practical and intimate. Survival gives decisions weight; visible everyday improvement gives them a payoff.

**The descendants idea is motivation only.** The user explicitly excludes inheritance gameplay. No genealogy, aging, succession, generational handoffs, legacy points or family simulation. A durable farm, a comfortable home and a village worth staying in can express the theme through the present scene and writing.

## Setting and scope

Use a fictional rural settlement along the St. Lawrence as the first reference direction. The precise location and decade remain TBD within the 17th century; choose them before approving specific architecture, clothes, crops or named historical events. This is a stylized game informed by history, not a claim of documentary reconstruction.

Keep Inlanders for repository/project names, namespaces, assemblies, scripts, internal identifiers and save paths. Public title, menu/HUD branding and runtime window title now use Les Habitants. Keep functional UI labels understandable; a French title does not require translating every control or renaming every existing file. [F34a](REFERENCE_PALETTE_F34A.md) records delivery and the provisional 1670–1680 palette.

F13 seasons remains removed. The setting is not authorization for a seasonal simulation, winter countdown, exposure meter or new survival catastrophe. Keep recoverable shortages and ordinary construction consequences. Combat, colonial conquest mechanics and detailed tax/tenure simulation are outside the proposed game. All buildings remain available; teaching order is not an unlock tree.

## The experience to test

Read the land → establish cultivation and complementary food → connect homes and useful work → adjust an imperfect arrangement → improve the village or finish when satisfied.

Farming should be visible and worthwhile. Fishing, foraging and woodland use can support a different mix without becoming compulsory stages. Do not impose a farmer quota, prohibit existing food sources, or globally nerf berries merely to force the story. Author a situation whose initial supply and useful ground actually leave a livelihood decision to make. Growth remains an option, not proof that a settlement is successful.

The first test uses current construction, shared labor, meals, fields/gardens, fishing, woodland, paths, housing and gathering places. Compare a cultivation-led arrangement with a mixed-land arrangement on the same authored map and starting resources. They are strategies a player can invent, not a route-selection menu. Keep a modest non-growing village valid and preserve an imperfect recovery branch.

Later additions must earn a distinct decision. A mill could create an interesting production site; another mandatory processing stop might only add waiting. A barn could make fields and storage legible; if it merely duplicates a pantry, use presentation or an existing storage variant. Exact costs, yields, time compression and map dimensions remain TBD.

## Presentation direction

Compose a farm settlement: river frontage and landings, houses relating to cultivated strips/kitchen gardens, a retained woodlot, work yards and modest shared outdoor ground. Avoid arranging every structure around one oversized timber hub. River access and field shape should help a player read the geography, even while the simulation remains tile-based.

Retain warm materials and readable silhouettes. Research a coherent small palette first: one dwelling, one cultivated plot and food/work structure, one shore landing, people and their tools. Show working land, practical timber/stone construction, garden fences, stored produce, domestic activity and actual journeys. Reevaluate the tall generic lodge and decorative civic center against that scene. A catalogue-wide asset rewrite is not a prerequisite.

Use names, clothing and tool poses consistently with the selected place/period; distinguish later picturesque Quebec imagery from 17th-century references. Keep French words where they aid identity, with plain functional descriptions. Do not add separate religion/education needs to justify a chapel or school. A chapel may eventually be a setting-appropriate identity for an existing communal place if the scenario and references support it.

Sound direction: audible field work, hand tools, wood construction, footsteps, water and ordinary village activity. Music should leave quiet space and support the chosen setting. The existing audio has not received a listening review; audition it before composing replacements. “Historically inspired” music must not be labelled an authentic period reconstruction without evidence.

## Historical anchors and design interpretations

These museum/heritage sources inform the direction, not numerical balance. Their scope often spans more than one century or region; date/place filtering is still required before asset approval.

- Farming households cultivated cereals and vegetables alongside other work. **Design interpretation:** make worked ground and household surroundings prominent; do not model every historical task. [Canadian Museum of History: Social Groups](https://www.historymuseum.ca/virtual-museum-of-new-france/population/social-groups/).
- Settlers used local foods while developing crop and livestock production; foodways differed by time and social setting. **Design interpretation:** cultivated food and complementary fishing/woodland supply deserve a real comparison, not a mandatory diet checklist. [Foodways](https://www.historymuseum.ca/virtual-museum-of-new-france/daily-life/foodways/).
- Long, narrow lots facing rivers or parallel roads gave access to water. **Design interpretation:** use frontage, field depth and travel as map-composition ideas; no cadastral ownership or rent system is required. [Governing New France](https://www.historymuseum.ca/digital-on-demand/history-hall-article-governing-new-france).
- Domestic architecture varied with period, region, means and available materials. **Design interpretation:** select one coherent rural reference set instead of treating every French-colonial building as interchangeable. [Vernacular Architecture](https://www.historymuseum.ca/virtual-museum-of-new-france/daily-life/vernacular-architecture-in-new-france/).
- The recorded alliance of 1603 documents relationships between French newcomers and existing Indigenous peoples. **Design interpretation:** ground any named place, community or encounter in researched context; avoid presenting the region as an empty stage. This does not commit us to a diplomacy or trading-NPC system. [Parks Canada: Alliance of 1603](https://www.pc.gc.ca/apps/dfhd/page_nhs_fra.aspx?id=10950).

No exact construction prices, yields, family systems, new needs or future chapter dates are established by these sources. See [the roadmap](ROADMAP.md), [active chunks](NEXT_CHUNKS.md) and [campaign concepts](CAMPAIGN_SYSTEMS.md) for implementation priorities.
