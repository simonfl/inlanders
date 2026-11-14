import './style.css';
import { VillageView } from './world';
import { VillageSound } from './workSound';
import type { State, Order, Cell, Preview, View, Frame } from './types';

document.querySelector<HTMLDivElement>('#app')!.innerHTML = `
  <header><div><strong>Les Habitants</strong><span id="scenario-title" class="subtitle">The broken crossing</span></div><button id="tools-open" aria-label="Open village menu">Village ☰</button></header>
  <section id="status" aria-label="Village resources"><span id="food">Food —</span><span id="logs">Timber —</span><span id="neighbors">4 neighbors</span><span id="clock">Paused</span></section>
  <section id="goals" aria-label="Your objectives"><div class="goal-heading"><strong id="goal-title">Help four neighbors feed themselves again</strong><span id="connection">Connecting…</span><button id="goal-toggle" hidden aria-expanded="false" aria-controls="goal-details">Details</button></div><div id="goal-details"><div id="checklist"></div><p id="hint"></p></div></section>
  <div id="river-status" hidden><span id="timetable"></span><button id="early-event" hidden></button></div>
  <section id="trade-offer" hidden aria-label="Trader offer"><div><strong id="trade-title"></strong><span id="trade-consequence"></span></div><div class="trade-actions"><button id="trade-buy">Trade</button><button id="trade-decline">Keep timber</button></div></section>
  <main id="world"><canvas id="scene" aria-label="Village map. Drag to move, pinch or scroll to zoom, tap to inspect or choose a site."></canvas><div class="map-tools"><button id="zoom-in" aria-label="Zoom in">+</button><button id="zoom-out" aria-label="Zoom out">−</button><button id="map-home" aria-label="Show whole village">⌂</button></div><div id="room-actions" hidden><button id="shortcut">Shortcut <small>6 logs · optional</small></button><button id="welcome-neighbors" hidden>Welcome neighbors</button></div><div id="selection" hidden></div><div id="loading">Preparing your village…</div></main>
  <section id="orders" aria-label="Village orders"><div id="order-help" role="status">Choose how to bring fresh food home.</div><div id="choices"><button id="home" hidden>Build cottage <small>6 logs · 2 beds</small></button><button id="food-choice" hidden>Food sources <small>gardens, fields & fishing</small></button><button id="repair">Repair crossing <small>6 logs</small></button><button id="garden">Plant a garden <small>4 logs</small></button><button id="grove">Work a grove <small>harvest & renew</small></button></div><div id="food-options" hidden><button id="small-garden">Garden <small>4 logs · 8 per harvest</small></button><button id="large-field">Field <small>10 logs · 20 per harvest</small></button><button id="fishing-dock">Fishing dock <small>12 logs · small catches</small></button><button id="food-back">Back</button></div><div id="placement" hidden><button id="rotate">Rotate ↻</button><button id="confirm" class="primary" disabled>Confirm</button><button id="cancel-mode">Cancel</button></div></section>
  <footer><button id="play">▶ Begin</button><button id="speed" aria-label="Change game speed">1×</button><button id="advance" title="While paused, advance 15 seconds or until a goal/event changes">Next event</button><button id="mark" aria-label="Mark a confusing moment">? Mark moment</button></footer>
  <dialog id="welcome"><p class="eyebrow">A small browser playtest</p><h1 id="welcome-title">The broken crossing</h1><p id="welcome-story">The flood spared your neighbors’ homes. Their ripe harvest is stranded across the river.</p><p id="welcome-task"><strong>Eight portions remain.</strong> Repair the crossing, or grow food on this bank. Your neighbors do the work automatically.</p><p class="muted">Drag to explore · pinch to zoom · tap to inspect.<br>Play pauses when you leave. Actions and your game view are recorded locally so we can review confusing moments.</p><button id="begin" class="primary">Help the village</button></dialog>
  <dialog id="tools"><div class="dialog-heading"><h2>Your village</h2><button id="tools-close" aria-label="Close village menu">×</button></div><div id="people"></div><p class="muted">Your playtest is saved on this laptop. “Mark moment” saves a bookmark with the view you were looking at.</p><button id="replay">Review this session</button><button id="sound">Sound: off</button><button id="restart">Start a fresh village</button><a href="/?scenario=room-by-river">Try Room by the river</a><a id="other-scenario" href="/?scenario=river-closing">Try the longer river settlement</a><a href="/?scenario=timber-trade">Try The price of provisions</a><a id="ripe-start" hidden href="/?scenario=timber-trade-ripe">Try the nearly ripe crop</a><p id="build" class="muted"></p></dialog>
  <section id="replay-bar" hidden><strong>Recorded session · read only</strong><input id="timeline" type="range" min="0" max="0" value="0" aria-label="Replay timeline"><div><button id="previous-mark">Previous bookmark</button><span id="frame-description"></span><button id="next-mark">Next bookmark</button><button id="leave-replay">Back to village</button></div></section>`;

const el = <T extends HTMLElement = HTMLElement>(id: string) => document.getElementById(id) as T;
const params = new URLSearchParams(location.search);
const replayId = params.get('replay'), watchId = params.get('watch');
const requestedScenario = ['room-by-river', 'room-by-river-far', 'river-closing', 'timber-trade', 'timber-trade-ripe', 'timber-control'].includes(params.get('scenario') ?? '') ? params.get('scenario')! : 'crossing';
const sessionKey = `inlanders-browser-${requestedScenario}`;
const readonly = !!(replayId || watchId);
let state: State | undefined, sessionId = '', mode = '', rotation = 0, firstCorner: Cell | null = null, chosen: Cell | null = null;
let preview: Preview | null = null, previewSerial = 0, busy = false, refreshing = false, selection: string | null = null, frames: Frame[] = [], frameIndex = 0;
let workSound: VillageSound | undefined;
let audio: AudioContext | undefined, soundOn = false, lastSound = 0, lastComplete = false;
let messageUntil = 0, goalsExpanded = false, entranceSiteId = -1;
const village = new VillageView(el<HTMLCanvasElement>('scene'), el('world'));
const view = (): View => ({ active: !document.hidden && !readonly, panel: el<HTMLDialogElement>('tools').open ? 'menu' : mode || 'village', selection,
  ...village.getView(), width: innerWidth, height: innerHeight, message: el('order-help').textContent ?? '', previewCells: preview?.cells ?? [], previewValid: preview?.valid ?? false, goalsExpanded });

async function request<T>(path: string, body?: unknown): Promise<T> {
  const response = await fetch(path, body === undefined ? undefined : { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body) });
  if (!response.ok) { let message = `Connection problem (${response.status})`; try { message = (await response.json()).error ?? message; } catch { /* keep useful status */ } throw new Error(message); }
  return response.json();
}
function tell(message: string) { el('order-help').textContent = message; messageUntil = performance.now() + 5000; }
function renderScenario(s: State) {
  const river = s.river, trade = s.trade, settlement = !!(river || trade || s.riverside);
  el('app').classList.toggle('riverside',!!s.riverside);
  el('room-actions').hidden=!s.riverside || readonly || !!mode;
  if(s.riverside){
    const room=s.riverside;
    el<HTMLButtonElement>('shortcut').disabled=s.buildings.some(b=>b.kind==='Bridge' && b.x===room.shortcut.x && b.z===room.shortcut.z);
    el('shortcut').innerHTML=room.shortcutBuilt?'Shortcut open':'Shortcut <small>6 logs · optional</small>';
    el('welcome-neighbors').hidden=!!room.welcomeProblem;
    el('welcome-title').textContent=s.title;
    el('welcome-story').textContent='Four neighbors already live here. Their garden and renewing woodlot are working. Four more people are coming to make this their home.';
    el('welcome-task').textContent='Make room nearby by clearing useful woodland, or preserve it and build across the stream. The old crossing works; an optional shortcut can shorten the journey. Prepare homes and food before the neighbors arrive in three minutes. You can recover from shortages.';
  }
  el('home').hidden = el('food-choice').hidden = !settlement;
  el('repair').hidden = el('garden').hidden = settlement;
  el('river-status').hidden = !(river || trade);
  if (river) {
    el('timetable').textContent = river.timetable;
    el('early-event').hidden = river.closed || !!river.earlyProblem || readonly;
    el('early-event').textContent = river.earlyLabel;
    el('welcome-title').textContent = s.title;
    el('welcome-story').textContent = 'The crossing stands and your first field is growing. Two boats will bring eight more neighbors before the river closes.';
    el('welcome-task').textContent = 'Eight portions remain. Prepare homes and food for the first four arrivals, then support all twelve. Shortages slow work, but you can recover. Major arrivals pause the village so you can plan.';
  }
  el('trade-offer').hidden = !trade || !['open','ordered'].includes(trade.decision);
  el('ripe-start').hidden = !trade;
  if (trade) {
    el('early-event').hidden = true;
    el('timetable').textContent = s.complete ? 'Village established · stay as long as you like' : `${trade.arrived ? 'All eight neighbors are here' : `Four neighbors in ${Math.floor(trade.arrivalIn / 60)}:${String(trade.arrivalIn % 60).padStart(2, '0')}`} · ${trade.crop}`;
    const ordered = trade.decision === 'ordered';
    el('trade-title').textContent = ordered ? `Timber packed · ${trade.packedLogs}/${trade.cost} logs` : `Trader · ${trade.cost} logs for ${trade.food} food`;
    el('trade-consequence').textContent = ordered
      ? `${trade.food} food when ready. Available timber goes here before new buildings. Work a grove if needed. No delivery deadline.`
      : `Agree even with no logs. The next ${trade.cost} available logs go here before new buildings; receive ${trade.food} food when ready. ${s.paused ? 'Paused. ' : ''}Decide within ${trade.departureIn}s of play.`;
    el('trade-buy').hidden = ordered;
    el('trade-buy').textContent = `Prepare ${trade.cost} logs`;
    el<HTMLButtonElement>('trade-buy').disabled = !!trade.problem || readonly || busy;
    el('trade-buy').title = trade.problem ?? 'Agree to a shipment, even with no logs stored. Provisions arrive only when the timber is ready.';
    el('trade-decline').textContent = ordered ? `Cancel · reclaim ${trade.packedLogs} logs` : 'Keep timber for village';
    el<HTMLButtonElement>('trade-decline').disabled = readonly || busy;
    el('welcome-title').textContent = s.title;
    el('welcome-story').textContent = 'Four neighbors are coming to join your village in two minutes. They need two cottages and food.';
    el('welcome-task').textContent = `Start by working a grove: your neighbors will harvest and renew it for you. You'll need timber for two more cottages and a steady food supply. ${trade.nearHarvest ? 'Your first crop is almost ripe.' : 'Your first crop is already growing.'} You have four portions for now; shortages are recoverable.`;
  }
  const link = el<HTMLAnchorElement>('other-scenario');
  link.href = river ? '/' : '/?scenario=river-closing';
  link.textContent = river ? 'Play the short crossing recovery' : 'Try the longer river settlement';
}
function renderGoalDetails() {
  const complete = !!state?.complete;
  el('goals').classList.toggle('complete', complete);
  el('goal-toggle').hidden = !complete && !state?.riverside;
  el('hint').hidden=!!state?.riverside && !goalsExpanded;
  el('goal-toggle').textContent = goalsExpanded ? 'Hide details' : 'Details';
  el('goal-toggle').setAttribute('aria-expanded', String(goalsExpanded));
  el('goal-details').hidden = complete && !goalsExpanded;
}
el('goal-toggle').onclick = () => { goalsExpanded = !goalsExpanded; renderGoalDetails(); void recordView(); };
function render(s: State) {
  if (!replayId && state?.id === s.id && (s.step < state.step || s.sequence < state.sequence)) return;
  if (state?.id !== s.id && (s.river || s.trade || s.riverside)) village.setZoom(s.riverside ? 12 : s.trade ? 9 : 15);
  if (state?.id !== s.id || replayId) goalsExpanded = s.view.goalsExpanded ?? false;
  state = s; sessionId = s.id;
  el('loading').hidden = true;
  el('connection').textContent = replayId ? 'Replay' : watchId ? 'Watching' : 'Saved locally';
  el('food').textContent = `Food ${s.resources.food}`; el('food').classList.toggle('low', s.resources.food < s.resources.population);
  el('logs').textContent = `Logs ${s.resources.logs}`;
  el('neighbors').textContent = s.resources.fed === s.resources.population ? `${s.resources.population} neighbors` : `${s.resources.population - s.resources.fed} hungry`;
  el('clock').textContent = `${Math.floor(s.seconds / 60)}:${String(Math.floor(s.seconds % 60)).padStart(2, '0')} · ${s.paused ? 'paused' : s.speed + '×'}`;
  el('goal-title').textContent = s.goalTitle;
  el('scenario-title').textContent = s.title;
  document.title = `Les Habitants · ${s.title}`;
  renderScenario(s);
  el('checklist').replaceChildren(...s.objectives.map(text => { const row = document.createElement('div'); row.textContent = text; row.className = text.startsWith('✓') ? 'done' : ''; return row; }));
  el('hint').textContent = s.hint;
  if (s.notice && !s.complete) el('hint').textContent = `${s.notice} Press Resume when ready.`;
  renderGoalDetails();
  el('play').textContent = s.paused ? '▶ Resume' : 'Ⅱ Pause'; el('speed').textContent = `${s.speed}×`;
  el<HTMLButtonElement>('advance').disabled = !s.paused || busy || readonly;
  el<HTMLButtonElement>('repair').disabled = s.buildings.some(b => b.kind === 'Bridge');
  el('build').textContent = `Build ${s.build} · session ${s.id.slice(0, 8)}`;
  const people = s.people.map(p => { const row = document.createElement('p'); const name = document.createElement('strong'); name.textContent = `${p.name}${p.fed ? '' : ' · hungry'}`; row.append(name, document.createElement('br'), document.createTextNode(p.status), document.createElement('br'), document.createTextNode(p.freshMeal ? '✓ Has eaten fresh food' : '○ Still waiting for a fresh meal')); return row; }); el('people').replaceChildren(...people);
  village.update(s);
  if (!mode && !selection && performance.now() > messageUntil) tell(s.riverside && s.notice && !s.complete ? s.notice + ' Resume when ready.' : s.complete ? 'You did it. You can keep watching your neighbors.' : s.riverside ? 'Tap homes or people to see their work. Drag to explore both banks.' : s.river || s.trade ? 'Neighbors share building, farming and timber work automatically.' : 'Choose a plan. Neighbors take care of the work.');
  if (selection) renderSelection();
  workSound?.update(s, village.getView(), soundOn && !readonly);
  if (soundOn && !readonly && s.complete && !lastComplete) chime();
  lastComplete = s.complete;
}
async function command(order: Order) {
  if (!sessionId || readonly || busy) return;
  busy = true; el<HTMLButtonElement>('confirm').disabled = true;
  try {
    const result = await request<{ accepted: boolean; message: string; state: State }>(`/api/sessions/${sessionId}/command`, { ...order, view: view() });
    if (order.type === 'view') return result;
    render(result.state); tell(result.message);
    return result;
  } catch (e) { tell((e as Error).message); el('connection').textContent = 'Disconnected · reconnect to resume'; }
  finally { busy = false; el<HTMLButtonElement>('confirm').disabled = !preview?.valid; if (state) el<HTMLButtonElement>('advance').disabled = !state.paused || readonly; }
}
async function recordView() {
  // Observing a camera/panel change must not lock out the player's next tap.
  if (sessionId && !readonly && !busy) {
    try { await request(`/api/sessions/${sessionId}/command`, { type: 'view', view: view() }); }
    catch { /* The heartbeat reports connection state; never overwrite the placement explanation. */ }
  }
}
function clearMode() { el('room-actions').hidden=!state?.riverside || readonly; el('food-options').hidden = true; mode = ''; village.showFishingGrounds(false); previewSerial++; firstCorner = null; chosen = null; preview = null; village.showPreview(null); el('choices').hidden = false; el('placement').hidden = true; }
function startMode(type: string) {
  if (readonly) return;
  village.showJourney([]); el('room-actions').hidden=true; el('food-options').hidden = true; mode = type; rotation = 0; firstCorner = null; chosen = null; selection = null; el('selection').hidden = true;
  preview = null; village.showPreview(null); el('choices').hidden = true; el('placement').hidden = false;
  el('rotate').hidden = !['garden', 'field', 'home', 'dock'].includes(type); el<HTMLButtonElement>('confirm').disabled = true;
  tell(state?.riverside && ['home','garden','field'].includes(type) ? 'Choose clear or wooded ground. The preview explains any clearing; neighbors do both jobs automatically.' : type === 'grove' ? 'Tap two corners around a grove on your bank. Dragging still moves the map.' : ['garden', 'field', 'home', 'dock'].includes(type) ? 'Tap clear ground. Preview the footprint, then confirm. Neighbors build automatically.' : 'Repair this crossing to reach the ripe harvest.');
  if (type === 'dock') { village.showFishingGrounds(true); village.focus({x:8,z:0}); tell('Tap the dry riverbank, then Rotate until the blue launch faces water. Fish stocks are shared by all docks.'); }
  if(type === 'garden-entrance'){void previewOrder();return;}
  if (type === 'shortcut' && state?.riverside) { chosen=state.riverside.shortcut; village.focus(chosen); void previewOrder(); return; }
  if (type === 'repair') { chosen = { x: 0, z: -3 }; void previewOrder(); } else void recordView();
}
function currentOrder(): Order | null {
  if(mode === 'garden-entrance')return {type:'garden-entrance',siteId:entranceSiteId};
  if (mode === 'shortcut') return { type: 'shortcut' };
  if (mode === 'repair') return { type: 'repair' };
  if (['garden', 'field', 'home', 'dock'].includes(mode) && chosen) return { type: mode, ...chosen, rotation };
  if (mode === 'grove' && firstCorner && chosen) return { type: 'grove', ...firstCorner, x2: chosen.x, z2: chosen.z };
  return null;
}
async function previewOrder() {
  const order = currentOrder(); if (!order) return;
  const serial = ++previewSerial;
  el<HTMLButtonElement>('confirm').disabled = true;
  try {
    const p = await request<Preview>(`/api/sessions/${sessionId}/preview`, order);
    if (serial !== previewSerial) return;
    preview = p; village.showPreview(p); tell(p.valid ? `${p.label.replace(/\.$/, '')}. Confirm to order the work.` : p.problem ?? 'Choose another spot.');
    el<HTMLButtonElement>('confirm').disabled = !p.valid; await recordView();
  } catch (e) { if (serial === previewSerial) { preview = null; village.showPreview(null); tell((e as Error).message); } }
}
function renderSelection() {
  if (!selection || !state) return;
  const box = el('selection'); box.hidden = false;
  const [kind, id] = selection.split(':');
  const selected = kind === 'person' ? state.people.find(p => p.id === Number(id)) : state.buildings.find(b => b.id === Number(id));
  if (!selected) { village.showJourney([]); selection = null; box.hidden = true; village.showFishingGrounds(false); return; }
  village.showFishingGrounds(kind === 'building' && 'kind' in selected && selected.kind === 'FishingDock');
  village.showJourney('route' in selected ? selected.route : []);
  const description = `${selected.name} · ${selected.status}${'entranceChange' in selected && selected.entranceChange ? ` · Food-store walk: ${selected.entranceChange.before} steps` : ''}${'fishing' in selected && selected.fishing ? ' · ' + selected.fishing : ''}${'freshMeal' in selected ? selected.freshMeal ? ' · ✓ Fresh meal eaten' : ' · Waiting for a fresh meal' : ''}`;
  const key = `${selection}:${'complete' in selected ? selected.complete : ''}`;
  if (box.dataset.selection === key && box.firstElementChild) { box.firstElementChild.textContent = description; return; }
  box.dataset.selection = key; box.replaceChildren();
  const text = document.createElement('span'); text.textContent = description; box.append(text);
  if (kind === 'building' && 'complete' in selected && !selected.complete && !readonly) {
    const cancel = document.createElement('button'); cancel.textContent = 'Cancel plan'; cancel.onclick = async () => { const result = await command({ type: 'cancel', siteId: selected.id }); if (result?.accepted) { selection = null; box.hidden = true; village.showFishingGrounds(false); } }; box.append(cancel);
  }
  if('entranceChange' in selected && selected.entranceChange && !readonly){const change=document.createElement('button');change.textContent='Change garden entrance';change.onclick=()=>{entranceSiteId=selected.id;startMode('garden-entrance');};box.append(change);}
  const close = document.createElement('button'); close.textContent = '×'; close.ariaLabel = 'Close inspection'; close.onclick = () => { village.showJourney([]); selection = null; box.hidden = true; village.showFishingGrounds(false); void recordView(); }; box.append(close);
}
village.onPick = c => {
  if (!state || readonly) return;
  if (mode === 'grove') { if (!firstCorner || chosen) { firstCorner = c; chosen = null; preview = { valid: false, problem: null, cells: [c], logs: 0, label: 'First corner' }; village.showPreview(preview); el<HTMLButtonElement>('confirm').disabled = true; tell('First corner set. Tap the opposite corner to include your trees.'); void recordView(); } else { chosen = c; void previewOrder(); } return; }
  if (['garden', 'field', 'home', 'dock'].includes(mode)) { chosen = c; void previewOrder(); return; }
  if (mode) return;
  const p = state.people.find(p => Math.hypot(p.x - c.x, p.z - c.z) < .75), b = state.buildings.find(b => b.footprint.some(f => f.x === c.x && f.z === c.z));
  selection = p ? `person:${p.id}` : b ? `building:${b.id}` : null;
  if (selection) renderSelection(); else { village.showJourney([]); el('selection').hidden = true; const tree = state.trees.find(t => t.x === c.x && t.z === c.z); if (tree) tell(tree.managed ? 'This grove is being harvested and renewed automatically.' : 'This tree is protected. Choose Work a grove to harvest and renew an area.'); }
  void recordView();
};
village.onViewChange = () => { void recordView(); };
el('shortcut').onclick=()=>startMode('shortcut');
el('welcome-neighbors').onclick=()=>{void command({type:'welcome'});};
el('home').onclick = () => startMode('home');
el('food-choice').onclick = () => { mode = 'food'; el('room-actions').hidden=true; el('choices').hidden = true; el('food-options').hidden = false; tell('Fields yield large crops after growing. Docks bring small catches sooner, but share a limited river supply.'); void recordView(); };
el('fishing-dock').onclick = () => startMode('dock');
el('small-garden').onclick = () => startMode('garden'); el('large-field').onclick = () => startMode('field');
el('food-back').onclick = () => { clearMode(); void recordView(); };
el('trade-buy').onclick = () => { void command({ type: 'trade-buy' }); };
el('trade-decline').onclick = () => { void command({ type: state?.trade?.decision === 'ordered' ? 'trade-cancel' : 'trade-decline' }); };
el('early-event').onclick = () => { void command({ type: 'next-season' }); };
el('repair').onclick = () => startMode('repair'); el('garden').onclick = () => startMode('garden'); el('grove').onclick = () => startMode('grove');
el('rotate').onclick = () => { rotation = (rotation + 1) % 4; el('rotate').textContent = `Rotate ↻ ${rotation * 90}°`; void previewOrder(); };
el('cancel-mode').onclick = () => { clearMode(); tell('Plan dismissed. Nothing was ordered.'); void recordView(); };
el('confirm').onclick = async () => { const order = currentOrder(); if (!order || !preview?.valid) return; const result = await command(order); if (result?.accepted) { clearMode(); tell(result.message); await recordView(); } };
el('play').onclick = async () => { if (!state) return; if (state.paused && soundOn) void audio?.resume(); await command(state.paused ? { type: 'resume', speed: state.speed } : { type: 'pause' }); };
el('speed').onclick = async () => { if (!state) return; if (soundOn) void audio?.resume(); await command({ type: 'resume', speed: state.speed === 1 ? 3 : state.speed === 3 ? 6 : 1 }); };
el('advance').onclick = () => { void command({ type: 'advance', seconds: 15, untilEvent: true }); };
el('mark').onclick = () => { void command({ type: 'bookmark', note: 'Player marked this moment for review' }); };
el('map-home').onclick = () => village.home(); el('zoom-in').onclick = () => { village.setZoom(village.zoom * .8); void recordView(); }; el('zoom-out').onclick = () => { village.setZoom(village.zoom * 1.25); void recordView(); };
el('tools-open').onclick = () => { el<HTMLDialogElement>('tools').showModal(); void recordView(); };
el('tools-close').onclick = () => { el<HTMLDialogElement>('tools').close(); void recordView(); };
el('replay').onclick = async () => { await command({ type: 'pause' }); location.href = `/?scenario=${state?.scenario ?? requestedScenario}&replay=${sessionId}`; };
el('restart').onclick = async () => { await command({ type: 'pause' }); localStorage.removeItem(sessionKey); location.href = `/?scenario=${requestedScenario}`; };
el('begin').onclick = async () => { el<HTMLDialogElement>('welcome').close(); await command({ type: 'resume' }); };
function chime() {
  if (!audio || performance.now() - lastSound < 2500) return;
  lastSound = performance.now();
  for (const [i, frequency] of [392, 493.88, 587.33].entries()) {
    const oscillator = audio.createOscillator(), gain = audio.createGain(), start = audio.currentTime + i * .12;
    oscillator.type = 'sine'; oscillator.frequency.value = frequency; gain.gain.setValueAtTime(0, start); gain.gain.linearRampToValueAtTime(.035, start + .02); gain.gain.exponentialRampToValueAtTime(.001, start + .5);
    oscillator.connect(gain); gain.connect(audio.destination); oscillator.start(start); oscillator.stop(start + .6);
  }
}
el('sound').onclick = async () => { soundOn = !soundOn; if (soundOn) { audio ??= new AudioContext(); workSound ??= new VillageSound(audio); await audio.resume(); chime(); } if(!soundOn)workSound?.quiet(); el('sound').textContent = `Sound: ${soundOn ? 'on' : 'off'}`; };

function showFrame(index: number) {
  frameIndex = Math.max(0, Math.min(frames.length - 1, index)); const frame = frames[frameIndex]; if (!frame) return;
  render(frame.state); village.restore(frame.state.view); selection = frame.state.view.selection; if (selection) renderSelection(); else el('selection').hidden = true;
  if (frame.state.view.previewCells?.length) village.showPreview({ cells: frame.state.view.previewCells, valid: frame.state.view.previewValid ?? false, logs: 0, label: '', problem: null }); else village.showPreview(null);
  tell(frame.state.view.message || frame.state.hint);
  el<HTMLInputElement>('timeline').value = String(frameIndex);
  el('frame-description').textContent = `${frame.type} · ${Math.floor(frame.state.seconds)}s · ${frame.state.view.panel}${frame.data?.message ? ' · ' + frame.data.message : ''}`;
}
el('timeline').oninput = () => showFrame(Number(el<HTMLInputElement>('timeline').value));
el('previous-mark').onclick = () => { const index = frames.findLastIndex((f, i) => i < frameIndex && f.type === 'bookmark'); showFrame(index < 0 ? 0 : index); };
el('next-mark').onclick = () => { const index = frames.findIndex((f, i) => i > frameIndex && f.type === 'bookmark'); showFrame(index < 0 ? frames.length - 1 : index); };
el('leave-replay').onclick = () => { location.href = `/?scenario=${state?.scenario ?? requestedScenario}`; };
async function boot() {
  try {
    if (replayId) {
      const response = await fetch(`/api/sessions/${replayId}/recording`); if (!response.ok) throw new Error('This recording could not be opened.');
      frames = (await response.text()).trim().split('\n').filter(Boolean).map(line => JSON.parse(line) as Frame);
      el('choices').hidden = true; document.querySelector('footer')!.hidden = true; el('replay-bar').hidden = false; el<HTMLInputElement>('timeline').max = String(frames.length - 1);
      showFrame(0); return;
    }
    let id = watchId || localStorage.getItem(sessionKey);
    let fresh = false;
    if (id) {
      try { const saved = await request<State>(`/api/sessions/${id}`); render(saved); village.restore(saved.view); }
      catch (e) { if (watchId) throw e; id = null; }
    }
    if (!id) { render(await request<State>(`/api/sessions?scenario=${requestedScenario}`, {})); localStorage.setItem(sessionKey, sessionId); fresh = true; }
    if (watchId) { el('orders').hidden = true; document.querySelector('footer')!.hidden = true; }
    else if (fresh) el<HTMLDialogElement>('welcome').showModal();
    else await command({ type: 'pause' });
    setInterval(async () => {
      if (refreshing || busy || document.hidden) return;
      refreshing = true;
      try { render(await request<State>(`/api/sessions/${sessionId}${watchId ? '' : '/heartbeat'}`, watchId ? undefined : view())); }
      catch { el('connection').textContent = 'Connection lost · village pauses automatically'; }
      finally { refreshing = false; }
    }, 400);
  } catch (e) { el('loading').textContent = (e as Error).message; }
}
function suspend() {
  if (!sessionId || readonly) return;
  navigator.sendBeacon(`/api/sessions/${sessionId}/heartbeat`, new Blob([JSON.stringify({ ...view(), active: false })], { type: 'application/json' }));
}
document.addEventListener('visibilitychange', () => { if (document.hidden) { suspend(); void audio?.suspend(); } });
window.addEventListener('pagehide', suspend);
void boot();
