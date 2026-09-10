# F21j — resource survey

Press **U** or choose **Economy → Survey map resources**. Click a map marker or choose a source in the inspector; the picker also centers offscreen sources. Fish grounds, stone outcrops and woodland habitats expose their current stock, reservations, depletion/recovery and access rules before construction. Original maps without those sources explain their absence.

Related workplace links open the existing building inspector. **Back to source** returns the camera and report, including after a dock link takes the original fishing ground offscreen. Camps/lodges are described as in range, with access still required; docks qualify through a real water route. Planned buildings remain clearly marked. No forecast or claim of local food storage is added.

Survey uses the existing inspector and one set of map buttons. Overlapping resource world labels hide while it is active; the world-label preference does not hide survey controls. Woodland boundaries remain visible. U/Esc finishes, placement ends survey, Watch hides it, and changing settlements resets it. Resource queries refresh at half-second intervals; marker positions follow the camera. Simulation rules and save format are unchanged.

## Verification

- Focused simulation checks (`Tests/SimulationTests.csproj -- --survey`): real stone reservations, source stock, missing/removed workplaces, planned dock routes, tree-loss feedback, inaccessible stone and unchanged serialized state.
- Rendered check (`./Play.ps1 -SurveySmokeTest`): actual marker clicks and workplace/return links, 960/1440 inspectors, label preference, placement/Watch/Esc/Economy controls and empty-map reset. Idle survey preserves node count and saves. Captures: `artifacts/f21j-*`.
- Visually reviewed narrow stone and wide woodland inspectors: readable wrapping/scrolling, source highlight and habitat boundary; duplicate resource labels removed.
- Full rendered HUD regression (`./Play.ps1 -HudSmokeTest`) passed, including survey, existing selection/navigation, services, placement, Watch and settlement switching. Build passed with no warnings or errors.

These checks establish controls and data behavior, not human legibility or visual appeal. Survey contains authored fish, stone and wildlife sources; berries and arbitrary trees remain outside this mode. Dense-marker overlap and a general resource filter can wait until maps warrant them.

## Roadmap review

F21j completes the immediate source-feedback chunk. Next try F25c small seating gardens: use spare neighborhood plots to serve actual recreation, with a useful land/capacity difference from squares and halls. Compare a dispersed settlement before adopting costs or adding more venue types. This follows the resident-needs plan without another production chain.

Keep F23a aesthetic acceptance before a full building-family expansion, and human river/lake pacing before new mastery scenarios. Quarry/woodland scenarios remain separate from their prototypes. Comfort follows the visual direction; local food service still needs actual meal journeys. Education/reflection, orchards, pasture and river mills remain candidates. Performance still constrains population/density growth; seasons and save migrations remain excluded.
