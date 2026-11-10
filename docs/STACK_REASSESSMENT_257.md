# Browser access and agent-led play: strategic reassessment

November 9, 2026. Requested by the owner after outcome 257: reconsider the stack from first principles for agent play, observing owner play, and iPhone access while development stays on the idle Windows laptop. This is a recommendation and bounded comparison plan, not an implemented migration or another playable outcome.

## Verdict and direction

The game is partially convincing. Owner feedback supports helping residents, clear progress, recoverable food pressure and watching useful work. Sustained decisions and replay value remain uncertain. The native development/observation loop is a concrete bottleneck: scripted successful solutions and still captures repeatedly missed confusion that ordinary owner play exposed.

Recommend browser delivery, a shared player-command interface, and recorded sessions. Keep continuous animated village life with pause and speed controls. An agent can inspect a situation, make ordinary orders and advance to an event without forcing the human game into turns. Moving development to a cloud desktop alone would not solve access, phone controls or observation.

First compare a TypeScript/Vite browser client, Babylon.js world rendering and HTML/CSS interface against the existing native experience. Use WebGL2 as the initial graphics baseline. Put the existing independent C# simulation behind a small ASP.NET Core command/state service on Windows for this comparison. This is an economical experiment boundary, not a commitment to preserve the entire model or run two languages forever.

Only the rules needed by one small scenario belong in the comparison. If the bridge costs more than implementing that small core, replace it. After the comparison, decide whether a shared TypeScript simulation for browser and headless runs would remove enough hosting/protocol friction to justify a port. Standalone browser execution would also allow static hosting and eventual offline play; neither is delivered by the initial C# service.

## Fixed evidence and independent perspectives

Review baseline: committed `6d45ab8`, with the explicitly identified pending outcome-257 finish-panel changes. Source and recorded evidence were inspected; no reviewer performed new ordinary play, actual iPhone interaction, continuous-motion viewing or aesthetic listening in this assessment. Agreement among reviewers is architectural judgment from shared evidence, not five new playtests.

- Game designer: retain the living village and owner-supported care/progress loop; do not use longer timers or a turn-based conversion as substitutes for meaningful choices. A phone-accessible recovery slice with shared actions and replay is the decisive comparison.
- UX/onboarding: the fixed desktop panels, keyboard/mouse assumptions and hidden finish conditions require redesigned touch interactions. Semantic agent success must be separate from discovering and using visible controls.
- Playtest: current predetermined placements and button-signal scripts establish mechanics, not independent decisions. Measure fresh ordinary play, usable session evidence and diagnosis time, not simulated wins.
- Development lead: all 115 simulation files have no Godot imports and already compile independently. This makes a temporary C# service credible. The native frontend and bespoke probes are the more entangled boundary. An all-TypeScript destination remains an option, not a prerequisite.
- Visual/audio: inspected the twelve-person village, character lineup and finish panel. The miniature presentation has no identified requirement that demands Godot, but a scaled desktop layout would bury the village on a phone. Preserve visible work and a modest audible mix; actual motion/listening/mobile evidence remains missing.

The main tension is transitional reuse versus a simpler eventual browser-only runtime. Choose the short C# bridge only while it gets the access/observation comparison playable sooner. All reviewers distinguish a useful observation architecture from evidence of fun. No reviewer supplied new mobile performance or audio-quality evidence.

## Whole-game implications

| Area | Decision and unresolved evidence |
| --- | --- |
| Campaign | Retain explicit purpose and recoverable pressure. Reconsider the long scenario if preparation remains order-everything-and-wait. Porting the chapter catalogue is not an acceptance requirement. |
| Economy, buildings, needs | Start with physical wood, food and housing; include only systems that create a readable choice in the slice. The wider catalogue and secondary needs must earn their return. |
| Normal and Creative | Evaluate the real Normal challenge. Creative remains useful for development and expression, but its results cannot stand in for challenge play. Full mode parity is deferred. |
| Controls and onboarding | Redesign for touch: readable persistent goal, contextual detail sheet, explicit preview/confirm/cancel, pan/pinch and no required hover or keyboard. |
| Village presentation and audio | Keep animated residents, tangible work and a warm miniature village. Browser delivery does not itself improve art or establish pleasant music. Inspect movement and audible mixes separately from stills and numeric checks. |
| Reliability and performance | Reuse current-format snapshot correctness provisionally. Add phone interruption/reconnection and replay verification. No assumption of cross-version determinism or mobile frame rate. |

## Three interfaces, with different evidence claims

1. Player/agent actions: observe visible state, inspect an object, preview/confirm a site, rotate/cancel, work a grove, pause/resume and change speed. Human and agent orders use the same validation, costs and outcomes. Record rejected commands. No free resources or source-derived placement recipe during independent play.
2. Actual interface evaluation: use visible browser controls and world picking at desktop and phone viewports. Include finding the objective, touch placement and recovery from rejection. HTML UI makes inspection easier; the 3D canvas still needs world picking and visual testing. A semantic API cannot certify discoverability.
3. Engineering/debug access: raw state, fixture setup, fast stepping, balance sweeps and counterfactual branches. Keep this powerful mode explicitly separate. Its successes are mechanics evidence.

Agent decision play can stop at meaningful events or a bounded simulation interval. Screenshots accompany structured observations; current displayed objective text is part of the observation. The same simulation may run continuously for the human. A thin local tool adapter can expose these commands; a new autonomous-agent framework is not required.

## Watching owner play

Offer an explicit recorded playtest session. Store build/scenario/seed, initial snapshot, simulation and wall-clock times, accepted and rejected commands, viewport, camera, selection, open panels, speed/pause state, and the objective text actually shown. Periodic snapshots and hashes detect replay divergence. Keep the matching build for replay rather than adding save migrations.

An optional confusing/good/stuck bookmark should identify the exact moment. Reconstruct a read-only replay and allow a separate branch to try another action without changing the owner's session. This directly addresses recent boat-timer and full-looking-finish confusion. Record camera/UI as well as simulation; a save file alone is insufficient.

Prefer asynchronous evidence the agent can inspect later. Live spectating can use the same stream while an agent session is active, but a recorder is not a promise of an always-running model. Playwright traces cover agent browser runs, not arbitrary owner Safari sessions. A reconstructed replay is not pixel-identical evidence of the owner's device: sampled world frames, UI state and actual-device clips remain distinct evidence, especially for rendering/audio faults.

Hesitation or idleness cannot distinguish confusion, pleasant watching and interruption by itself. Treat these as leads, then use occasional owner reactions to calibrate taste. The agent should make and evaluate reversible design decisions while the owner is away; absence of human feedback does not justify endless feature accumulation.

## Windows host and iPhone access

Keep development on the current laptop. For the bridge prototype, a background service hosts the simulation, client and session evidence; the phone renders its own village. No visible Godot window or desktop stream is required. Use a stable playtest build, not a development server that reloads during a session.

Private HTTPS access through Tailscale Serve is a suitable first option, with Tailscale on laptop and phone. This requires setup and an awake, reachable laptop; none has been configured by this assessment. Keep debug controls private and separate from player access. If availability becomes the bottleneck, move the small play service to a cloud host while development stays on Windows. A cloud development workstation is not presently justified.

Pause and persist when the phone backgrounds or disconnects, with a server-side inactivity policy as fallback; deliberately resume on return. Do not let unattended villagers starve because the owner answered a message. Browser emulation can catch layout/control mistakes, but actual iPhone rendering, audio startup and interruption behavior still need a short real-device check.

## Bounded next comparison

Question: can the owner open and resume meaningful play from a phone, and can an independent agent play and diagnose that same experience without bespoke native scripts?

Use one compact recovery situation with a few residents, forestry, housing and a food source, clear objectives and recoverable pressure. Include animated work and restrained sound. Reuse a known situation so the access comparison is not confounded by designing a new campaign at the same time. A useful session can be five minutes without making every eventual level five minutes long.

First implementation session: one rendered browser scene and one end-to-end validated command against the simulation. Reassess bridge cost before porting more. Then add the minimum touch play, interruption recovery and session journal needed to evaluate the entire loop. Do not promise a full migration within the process's normal half-day experiment budget; stop and reassess the first seam before extending its scope.

The comparison earns further migration only when:

- A phone link reaches a stable playable start; one meaningful order, interruption and paused resume work on the actual device.
- An independent evaluator without the authored recipe understands the visible goal, chooses ordinary actions and recovers from a mistake.
- A separate UI run reaches those actions through visible controls, including a rejected placement and its explanation.
- Another reviewer reconstructs a bookmarked confusing moment from the recorded build/session without asking the owner to reproduce it.
- A workable strategy and a weaker alternative expose consequences; completion alone does not count as enjoyable play.
- Normal-speed movement and audio are available for inspection, and measured phone frame times are reported rather than assumed.

If access/observation still requires bespoke scripts or the village becomes an illegible dashboard, revise or reject the approach before porting the catalogue. Even a successful tooling comparison leaves the longer scenario's enjoyment provisional.

## Investment priorities and costs

| Investment | Beneficiary and repeated savings | Cost / validation |
| --- | --- | --- |
| Shared command/observation seam | Builder and independent evaluator, every playtest; removes new hard-coded success scripts as the only control route | Moderate initial model mapping and ongoing command/schema upkeep. First-session single-action proof; second scenario must reuse the interface without bespoke control plumbing. |
| Browser touch client and stable link | Owner whenever away from laptop; agent ordinary UI play without desktop focus | Highest initial cost: renderer/UI replacement and mobile adaptation. Validate real-device start, action and resume before broad migration. |
| Snapshot plus command/UI journal | Owner and reviewer for each confusing moment; avoids reproductions/interviews as the only diagnosis path | Moderate storage/version/replay work. Begin with one session; measure time to reconstruct its reported problem and detect divergence. |
| Optional cloud play host | Owner only if laptop availability prevents play | Defer recurring hosting/deployment cost until awake/reachable local hosting is demonstrably inadequate. |

Record checkout-to-play time, usable recorded sessions, diagnosis time and maintenance effort. No savings are measured yet. Defer universal scenario editors, generalized ECS migration, agent orchestration platforms, full campaign conversion and offline synchronization.

## Primary technical references

- [Godot stable web export](https://docs.godotengine.org/en/stable/tutorials/export/exporting_for_web.html): current Godot 4 C# projects cannot export to web. Rewriting scripts for Godot web would still require touch UI and agent/session interfaces.
- [Babylon.js specifications](https://www.babylonjs.com/specifications/): browser 3D rendering, picking, animation and audio capabilities. WebGL compatibility is the proposed starting baseline, not a measured iPhone result.
- [Playwright Trace Viewer](https://playwright.dev/docs/trace-viewer): browser automation action/screenshot/network evidence. Separate from the game's owner-session recorder.
- [Tailscale Serve](https://tailscale.com/docs/features/tailscale-serve): private service access and HTTPS within the tailnet; public Funnel is a different capability.
