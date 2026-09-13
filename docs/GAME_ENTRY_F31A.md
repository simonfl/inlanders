# F31a — one clear game entry

September 13, 2026. Checkpoint **26**. Whole-game review remains due at **30**, including visual/audio.

## Delivered

The main menu now offers **Play** and **Free arrangement**. Play opens the short **A place to gather** introduction; Free arrangement opens **An open court** without a project. Both use the existing sixteen-resident world, actual meals/shared work and free construction with forgiving hunger. The introduction still allows watching and building after finishing.

Each page has its own New/Resume actions. Existing `court-finite.json` and `court-open.json` paths remain authoritative; no new mode flag, migration or save format was added. New asks before replacing the selected village. Continue deliberately restores the most recently opened/saved village, including an archived one; explicit Resume always selects its named mode.

The guided neighborhood, constrained court, inlet and meadow are now under **Earlier prototypes → Earlier settlements**. The older eight-person free court is under **Earlier prototypes → Earlier free court**. Existing campaign/free-play/older rule controls remain archived. Back returns from these pages to the archive, then to the main menu. Their simulation code, maps and fixtures remain available.

This implements the checkpoint25 decision in the normal entry flow. It does not claim the introduction is a challenging campaign or that finite play has earned user preference. F31b now tests a meaningful spatial compromise rather than adding another menu or completion condition.

## Checks

Rendered control journeys exercise the actual main menu, project/free New, both explicit Resumes, Continue after switching, cancellation of a replacement, archive reachability and Back navigation. Browsing archives must not mutate either current save. Existing actual-placement/meal/ending/watch/reopen and current-save checks remain in the journey.

The menu keyboard check covers disabled Continue focus, wrapping, sliders/settings, Back navigation, archive campaign/legacy Creative entry and replacement cancellation. Existing prototype review scripts now navigate through the archive rather than treating retired entries as primary.

Final results: game/test builds report zero warnings/errors. Both full UI journeys passed, as did `./Play.ps1 -MenuKeyboardSmokeTest` at both widths and `git diff --check`.

- 960: `20260913-203938-777-court-experience-2f9895`
- 1440: `20260913-204014-150-court-experience-7f35c4`
- Capture source fingerprint: `43EE27492E3217C9FFA7D7AF77F06204483FC605E62902DA8CA86C1530427001`
- Captured game assembly: `2D32C3A7429BDD18EB84439CD3130467E985EA3CA842532E929660AA87806B74`

Artifacts precede the delivery commit; fingerprints identify the tested source. The keyboard launcher rebuilt the same source after the captures. [Final main menu at960](images/game-entry-960.png), inspected by the lead. The familiar title art is unchanged. No new simulation rules changed, so a broad simulation rerun was not warranted. These scripted checks and still inspection are not uncoached play, listening or a preference verdict.
