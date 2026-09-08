# Feature roadmap

A living menu of things we might build. Pick one chunk at a time; details, numbers, and order can change as we play. This is a feature plan, not a prototype cleanup backlog.

## Direction

A small, peaceful settlement that is satisfying to arrange and watch. Villagers carry real goods, buildings depend on one another, and short scenarios give the village a purpose. Keep Windows, local play, Godot, and C# as the starting point.

Current baseline: milestones 1–3 are playable, with eight workers, timber and construction, foraging/farming/baking, housing, a village supper, and save/load.

## Feature menu

Everything below is **Idea** unless marked otherwise. IDs stay stable so we can say “let's do F03.” Dependencies are provisional.

| ID | Feature | First playable chunk | Depends on |
| --- | --- | --- | --- |
| F01 | Paths and village layout | Paint/remove simple paths; villagers prefer them and walk faster on them. | — |
| F02 | Renewable woodland | Plant trees that grow into harvestable timber, making continued building possible. | — |
| F03 | A village that feels alive | Distinct work animations, recognizable carried goods, and a few idle actions. | — |
| F04 | Gathering places | Build a village square with benches/table; villagers visit during a short leisure period and gather there for supper. | — |
| F05 | New food choices | Add one alternative food chain, such as vegetables or an orchard, with its own building and visible harvest. Exact choice TBD. | — |
| F06 | More villagers | A small group arrives when spare housing is available; grow beyond the original eight. | — |
| F07 | Local storage and hauling | Place another stockpile and assign haulers so a distant work area can function efficiently. | — |
| F08 | More construction materials | Add a sawmill and planks, then one building that uses them. | F02 suggested |
| F09 | Village character | Place gardens, fences, flowers, and decorative trees; give cottages a few visual variants. | — |
| F10 | Sound and atmosphere | Ambient village/nature sounds and a few work sounds; explore a gentle visual day/night cycle. | — |
| F11 | Scenarios and objectives | Choose between the current supper scenario and one new authored challenge with different resources and goals. | — |
| F12 | Landscape and exploration | A larger authored map with water, a bridge, and another useful area to settle. Terrain height TBD. | F11 suggested |
| F13 | Seasons | A visible seasonal cycle that changes one food source, giving stored food a purpose. | F05 suggested |
| F14 | Village happiness | A simple satisfaction measure driven by food variety and leisure, with visible villager reactions. Effects TBD. | F04, F05 |
| F15 | Small events and choices | Occasional visitors or requests with a modest reward or tradeoff. Start with one event. | F11 suggested |
| F16 | Free-build mode | An open-ended scenario with optional objectives and enough renewable resources to keep expanding. | F02, F06 suggested |

## Where to start

Suggested next pick: **F01 — Paths**, because arranging the settlement is a big part of the fun and this gives layout an immediate purpose.

Other good independent picks: **F03** for more charm, **F02** for continued expansion, or **F11** for a new reason to play. These are alternatives, not a required sequence.

## How we take a chunk

1. Pick an ID and mark it **Next**, then **In progress** when work starts.
2. Add a short entry below: what we want to play, the smallest useful version, and any decisions needed now. Leave the rest TBD.
3. Implement enough to play it, including save/load for new persistent state and relevant checks.
4. Play it, record what we learned, and mark it **Done** or split out a follow-up.

Keep refactors and supporting UI inside the feature that needs them. A broad feature can have several small chunks; finishing its first chunk does not commit us to every possible extension.

### Chunk template

Copy this when we choose a feature; no need to fill it out for every idea upfront.

```markdown
### Fxx — Feature name / chunk name

Status: Next | In progress | Done | Parked

Want to play: One sentence describing the new player experience.
First version: A few concrete things we will add.
Later / TBD: Anything deliberately left open.
Playable when: A short in-game check that demonstrates the feature.
After playing: Notes, follow-ups, and implementation commit if useful.
```

## Parking lot

Possibilities without a commitment: fishing, trade, building upgrades, professions tied to individual workplaces, household routines, weather, procedural maps, and a photo mode.

Combat, multiplayer, a large technology tree, and a full life simulation are outside the current direction. Revisit only if they sound fun later.

## Completed chunks

- Milestone 1: harvesting, hauling, and the first cottage.
- Milestone 2: eight villagers, work assignments, reservations, and construction priorities.
- Milestone 3: food production, meals, the village supper, and save/load.
