# Small browser prototype

Two bounded comparisons: **The broken crossing** and **Before the river closes**. The C# simulation runs on this Windows laptop; Babylon.js renders a touch-friendly village in the browser. The browser and agent use the same ordinary commands. No Godot window is needed.

## Run

From the repository root, with the local .NET SDK and Node available:

```powershell
./Browser.ps1 Build
./Browser.ps1 Start
./Browser.ps1 Status
./Browser.ps1 Stop
```

Open `http://127.0.0.1:5180` for the short recovery, or `http://127.0.0.1:5180/?scenario=river-closing` for the longer settlement. The private Tailscale URL accepts the same query. Each scenario keeps its own browser session. The server listens only on loopback. Start runs a hidden background process; it does not install a startup service or change sleep settings. Stop verifies the recorded process identity before stopping it. Stop before rebuilding; a prototype build has its own save/recording identity.

Client dependencies are pinned in `client/package-lock.json`. Standard Node installations can use `npm ci` and `npm run build` in `Browser/client`. This laptop has Node without npm on PATH; the project-local npm bootstrap is `.tools/npm/package/bin/npm-cli.js`. The launcher currently uses that local installation. No global packages are required.

## Private iPhone access

Install/sign in to Tailscale on the laptop and iPhone using the same account. With the server running, configure private HTTPS serving on the laptop:

```powershell
& 'C:/Program Files/Tailscale/tailscale.exe' serve --bg http://127.0.0.1:5180
& 'C:/Program Files/Tailscale/tailscale.exe' serve status
```

Complete Tailscale's HTTPS enablement if prompted. Open the resulting `https://…ts.net` URL in Safari while Tailscale is connected. Use **Serve**, not public Funnel. The service deliberately trusts this private access boundary; there is no additional application login. Do not publish it to the open internet unchanged.

The laptop must be awake and connected. No cloud hosting, port-forwarding or native iOS packaging is needed. Actual iPhone performance and Safari lifecycle/audio still require device testing; a desktop phone-size viewport is not that evidence.

## Play

In the trade and longer settlement scenarios, **Food sources** groups gardens (4 logs), fields (10) and fishing docks (12). Place a dock on dry shore and rotate its blue launch toward water. Workers handle fishing automatically. Each trip catches up to four fish from renewing grounds shared by all docks; inspection shows stocks, renewal and route distance. The short crossing deliberately keeps its smaller action set. See [the fishing comparison](../docs/BROWSER_FISHING_261.md) for evidence and remaining design questions.

- Repair the existing crossing for six logs, or place a garden on the home bank for four.
- Work a grove by tapping two corners around trees, then confirming. Neighbors harvest and renew the selected trees automatically.
- Drag to move the map; pinch/scroll or use the zoom buttons. Tap people/buildings to inspect. Unfinished plans can be cancelled.
- Pause, choose 1×/3×/6×, or use **Next event** while paused to advance up to fifteen simulated seconds. No free resources or instant construction.
- Three objectives remain visible. Fresh meals are distinct from eating the initial provisions.
- Backgrounding the page requests an immediate pause/save. A six-second missing-heartbeat fallback covers lost connections; returning does not resume automatically.
- **Mark moment** records a bookmark. Village → Review this session opens a read-only timeline with objectives, camera, selection, preview and explanation. Recording is disclosed in the opening screen.

## Longer settlement

The crossing already works and the first field is growing. Prepare for four neighbors at minute8 (+24 food), four more at minute16 (no food), and closure at minute24. Major arrivals, closure and completion pause the village with a visible explanation. Prepare early to enable **Welcome the boat early** / **Begin the final trial**; there is no compulsory wait after readiness.

Three main intents remain visible: build a cottage (6logs,2beds), grow food (garden4logs/8perharvest, field10logs/20perharvest), and work a grove. Crops, delivery distance and labor affect real supply; yield is not a per-minute production promise. No manual worker assignment. The short crossing remains available in Village menu.

Before closure, the current goal shows beds, reserve and a measured fresh-supply window. After closure, housing/fed counts, reserve/fresh-meal counts, steady time and delivery-versus-consumption are separate conditions. The full4:00 clock alone is not completion. Remaining-food deficits are explicit; waiting residents are named in the hint and listed individually in Village. Hunger is recoverable.

This is a longer simulation, not a promised15–20-minute real-time duration. Effective early preparation and faster speed can shorten it substantially. Whether the final sustain phase is rewarding still needs owner play.

## Agent play

```powershell
node Browser/agent.mjs guide
node Browser/agent.mjs new
node Browser/agent.mjs new river-closing
node Browser/agent.mjs observe SESSION_ID
node Browser/agent.mjs preview SESSION_ID '{"type":"garden","x":0,"z":6}'
node Browser/agent.mjs command SESSION_ID '{"type":"advance","seconds":15,"untilEvent":true}'
node Browser/agent.mjs recording SESSION_ID
```

The first two commands provide the situation and legal actions; no authored placement recipe is required. All observations describe player-visible objects, resources, objectives and events. Engineering checks in `checks.mjs` intentionally use known solutions and must not be described as independent play.

HTTP equivalents:

| Method/path | Use |
| --- | --- |
| GET `/api/guide` | Discover commands and premise |
| POST `/api/sessions?scenario=crossing` or `river-closing` | New isolated paused village |
| GET `/api/sessions/{id}` | Observe; never keeps the player session running |
| POST `/api/sessions/{id}/preview` | Validate proposed placement or grove |
| POST `/api/sessions/{id}/command` | Ordinary player command, accepted/rejected receipt and state |
| POST `/api/sessions/{id}/heartbeat` | Human presence and displayed view; inactive pauses |
| GET `/api/sessions/{id}/recording` | JSONL experience frames and command receipts |

`/?watch=SESSION_ID` is a read-only spectator view; it does not keep an absent player's village alive. `/?replay=SESSION_ID` reconstructs recorded frames. These are private URLs under the same service boundary.

## Evidence, persistence and limits

`artifacts/browser/sessions/{id}` contains an append-only experience journal, immutable world checkpoints and bookmark snapshots. Completed checkpoint files become visible via rename; unfinished temporary files are ignored. This avoids replacing a hot save while Dropbox indexes it. Prototype sessions are disposable; saves from different builds are rejected. No migrations.

The build identity covers the server and built client assets. Keep the matching code/build when comparing evidence. Replay renders recorded visible frames rather than re-simulating commands; it is not a video, does not prove pixel-identical iPhone rendering, and does not reproduce the audible mix. Its initial purpose is to explain exactly which objective and action the player was considering. Current playback is seek/bookmark based, not a polished video editor.

State is checkpointed after commands, on interruption, and every five seconds while running. Unexpected process termination can lose the last few seconds of free-running simulation. Recordings/checkpoints are kept locally without automatic pruning in this first slice. A running session writes roughly one visible frame per second; long sessions consume more disk space. Normal disconnected sessions pause instead of recording indefinitely.

Sound is a small optional fresh-food/completion chime, enabled by an explicit browser gesture. The desktop music and full asset catalogue are deliberately not ported. Continuous movement, carrying and simple work poses remain visible.

Checks:

```powershell
node Browser/checks.mjs
node Browser/river-checks.mjs
# After a completed, owned river test session exists:
node Browser/concurrency-checks.mjs SESSION_ID after
# Restart the same build, then:
node Browser/checks.mjs --restored
```

Checks cover rejected actions, both food routes, cancellation/recovery after shortage, recorded preview/objective/bookmark state, pause on interruption/disconnect, cross-site request rejection and exact saved state after service restart. Actual visible-control play and independent decision play are separately recorded in the experiment report.

Sessions have separate simulation locks. Advancing one agent village does not hold another village's input/heartbeat behind that simulation; commands within a village remain serialized. The clock skips a busy world instead of holding up every world. This removes cross-session simulation blocking, not CPU or storage contention under unlimited load.
