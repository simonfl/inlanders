# Browser prototype — outcome 258

November 9, 2026. The owner accepted the browser comparison and asked to keep it small, easy for agent play and accessible from an iPhone. This implements one recovery scenario, not a campaign rewrite.

## Experience and scope

Keep the known **broken crossing** situation: four housed neighbors, eight initial portions, four logs, and a ripe field across a washed-out crossing. Repair access or grow a local garden. Forestry is an area order with renewal; work is automatic. Three persistent objectives show access, actual fresh-food delivery and four named neighbors eating fresh meals. Current hunger is distinct from this last objective.

The browser uses TypeScript/Vite, Babylon.js and HTML/CSS controls. The existing independent C# simulation runs in a small ASP.NET Core service. The same validated commands drive phone actions and agent decisions. No simulation rule or native campaign was changed. Godot excludes the browser tree from compilation/import.

The scene retains homes, trees, cultivated ground, water, moving residents, carried baskets and simple work poses. It deliberately does not reproduce the full native art, music or building catalogue. Sound is an optional small food/completion chime. No new needs, levels, inheritance, weather or seasons.

## Delivered workflow

- Start a paused, isolated village; preview/confirm repair, garden or a two-corner grove. Rotate gardens through four sides and cancel unfinished plans.
- Drag/pinch or use zoom buttons. Tap people/buildings to inspect; named fresh-meal status is also in Village.
- Pause, run at1×/3×/6×, or advance to an event/up to15seconds while paused. The agent API allows the same bounded stepping, up to30seconds per call.
- Backgrounding requests pause/save. A missing-heartbeat fallback pauses after six wall-clock seconds; a spectator does not keep an absent player running. Return resumes deliberately.
- Record ordinary commands, rejection messages, visible world/objectives and camera/panel/selection/preview state. Mark moment adds a bookmark and world snapshot.
- Review a read-only frame timeline or bookmark using the recorded objectives, view and explanation. This is reconstructed experience evidence, not video or an audio recording.
- One hidden local service, a small agent CLI and focused API checks replace bespoke native interaction scripts for this slice.

The owner chose Tailscale, installed/logged in, and approved the Windows installer. Private HTTPS Serve is configured to proxy only the loopback game service. Public Funnel was explicitly left disabled. The private URL was verified by both a health request and loading the actual welcome screen. The owner then confirmed: **It opens on my iPhone.** This verifies actual device access, not yet a touch/performance/enjoyment verdict. No LAN firewall rule, public tunnel, router forwarding, startup service or power-policy change was made. The laptop must stay awake. See [run/access instructions](../Browser/README.md).

## Evidence

| Evidence | Result and limits |
| --- | --- |
| Independent decision play | Evaluator used only public observations/previews/commands on build`7BBFA133C89A`, session`42132c78e19e4b3cadfa4a5202419b37`. Rejected an overlapping garden, deliberately began an underfunded repair, read the two-log shortage, chose an observed tree and recovered. Finished at165.7simulated seconds, four fresh meals, eight food and six logs. No source, save internals or authored solution was read. |
| Independent recording check |178NDJSON entries on that fixed build. Bookmark35 at30s preserved the shortage, visible objectives and evaluator rationale. This is actual agent decision play, not ordinary UI or preference evidence. |
| Review fixes | Completed bridges now say Crossing open instead of Waiting for a worker. Each named resident exposes fresh-meal progress in both UI and the agent's visible observation. |
| Ordinary visible-control play | Parent played through the browser at390×844 using actual buttons and canvas taps: selected a two-tree grove, confirmed repair, ran at3×, observed all three goals complete, marked the moment, inspected named residents, opened replay and sought the bookmark. Build`EED9730F18AB`, session`74b748de06194b3da815db912e0d2c92`. Distinct from direct API orders. |
| UI corrections | Modular Babylon imports initially omitted ray registration; actual canvas tapping exposed it and the corrected tap path was exercised. A refreshing inspector replaced its Cancel button during input; final build`7CC195BA1500` keeps the controls stable. A fresh garden was placed, inspected and cancelled successfully through the ordinary UI. View recording no longer makes unrelated player taps busy; first grove corners are recorded too. |
| Focused mechanics/API checks | Both food routes succeed using legal commands: repair150s; local garden after240s no-action shortage420s. Invalid placement/cancellation/time requests are rejected; garden cancellation/replacement works. Bookmark preserves displayed explanation, preview cells and objective state. Background/disconnect pause and cross-site rejection pass. `artifacts/browser/checks.json` records build`EED9730F18AB`; final changes are client interaction fixes, separately retested. |
| Persistence | An exact world/people/resource state reloads paused after a service restart on build`7BBFA133C89A`. Current-format snapshots only. Frequent save replacement encountered Dropbox locking; immutable checkpoint files with publish-by-rename resolved the observed failure. |
| Build and access | Delivered build`A2CEE19B61A0`: TypeScript/client build and C# zero-warning build pass. Browser bundle reduced from~6.3MB to~1.12MB uncompressed through targeted imports; server compression is enabled. Private HTTPS welcome screen and health verified, followed by owner-confirmed actual iPhone access. After the7CC interaction/visual checks, the final client-only corrections keep additional gardens available to humans exactly as in the agent API and resume optional audio on the user's resume/speed gesture. No rules changed. |

Source/fixture checks, independent API decisions, ordinary browser controls and owner preference remain separate categories. Desktop phone-size layout is **not** an actual iPhone Safari, GPU, touch or audio test. No human fun verdict or subjective listening verdict is inferred.

## Independent presentation review

The independent visual evaluator inspected actual desktop1280×720 and390×844 screenshots on fixed build`7CC195BA1500`, using an isolated spectator session and read-only inspection of the paused normal controls page. Verdict: **partially convincing; suitable for the bounded owner test**, with no established must-fix blocker. Three objectives are readable, miniature houses/trees/field and people are identifiable, phone order buttons are large, and about473px of world height remains with controls present.

Defer framing refinements until real phone feedback: zoom controls can overlap the ripe-field label and part of the field; desktop framing leaves excessive empty space; tiny people need actual motion evidence. No new motion, listening, physical touch, broader campaign/Creative-mode or enjoyment verdict was obtained. The later final client corrections do not change layout or scene rendering. The five-role architecture review in STACK_REASSESSMENT_257.md remains the strategic baseline; this slice is not promotion of the entire campaign into a new engine.

## Decision and next work

Subsequent actual-phone owner feedback: **a bit blurry, surprisingly smooth, and the reduced interface is liked**. The restricted actions are intentional for this slice. This supports retaining a focused interface provisionally, not a full-game enjoyment verdict or a requirement to restore the catalogue.

Inspection found a resolution bug: Babylon divides CSS canvas dimensions by its hardware scaling level, but the old formula increased that level with device pixel ratio. At DPR3 it produced0.5render pixels per CSS pixel. The source correction uses the reciprocal of capped density, producing1.5render pixels per CSS pixel at DPR2/3 and keeping DPR1 at1. This increases raster work on dense screens, so actual-phone smoothness must be rechecked. The correction was first built separately, then deployed at the owner's request as build `8F426BA89689`. Completed objectives now collapse into “✓ Village recovered”, with an accessible Details button to reopen the full checklist and explanation. The expanded state is recorded and restored during replay. This delivery remains a correction within258, not another feature experiment.

The shared-action approach has demonstrated independent agent play, and ordinary UI play now exercises the same decisions. Private browser delivery also opens on the owner's actual iPhone. The focused presentation review accepts this bounded trial provisionally. Phone control comfort, interruption/resume and sustained enjoyment remain to be observed.

The delivered client and server binaries are archived at `artifacts/browser/build-A2CEE19B61A0.zip`, preserving the exact build alongside its recorded evidence. The prior7CC comparison build is separately archived. These local artifacts are intentionally excluded from Git.

Next: use the real phone session and any bookmarked confusion to decide the smallest clarity/access correction. Measure time to open/resume and diagnose a reported moment. Keep the C# bridge provisionally because it allowed this comparison without rewriting rules. Do not expand the catalogue, port the campaign or rewrite the simulation before learning from this access/observation loop.

Known limits: the service is manually started and requires an awake laptop; one browser stores its own current-session ID; different builds reject old live saves; replay requires the matching prototype shape/build rather than a compatibility promise; records are local and not automatically pruned; a force-killed service can lose the last five seconds of free-running simulation. The current session manager serializes simulation requests, so long agent advances can briefly delay other sessions. If concurrent owner/agent work makes that noticeable, per-session scheduling is a concrete next tooling candidate. None of these limits justifies a general orchestration platform.

Small runtime/backend files and [the public command guide](../Browser/README.md) should remain the experimentation surface. The success criterion for further tooling is easier ordinary play and diagnosis, not more generated checkpoints.

## Phone display correction — delivered

Build `8F426BA89689` is live through the existing private HTTPS link. TypeScript/Vite and zero-warning C# builds pass. Focused API regressions again pass both food routes, invalid commands, recording, interruption pause and cross-site rejection. An additional isolated bookmark verifies expanded-goal state serialization. Ordinary browser interaction at390×700 verifies initial objectives remain visible, completed details expand/collapse, and seeking between two completion bookmarks restores the corresponding disclosure state. The compact row measures52px versus149.7px expanded; no horizontal overflow. Canvas585px across390CSSpx verifies the1.5× raster density on the test browser. Desktop1280px layout was also inspected. These are browser observations, not actual iPhone performance or subjective sharpness acceptance. No new gameplay or catalogue expansion.

Refresh loads the updated client and starts a fresh village because saves are build-specific. The exact delivered binaries are archived at `artifacts/browser/build-8F426BA89689.zip`. Next: owner comparison of phone sharpness and smoothness; retain the reduced interface provisionally.
