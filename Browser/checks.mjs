import assert from 'node:assert/strict';
import { writeFile, mkdir, readFile } from 'node:fs/promises';
const base = 'http://127.0.0.1:5180';
async function api(path, body) {
  const r = await fetch(base + path, body === undefined ? {} : { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body) });
  assert.equal(r.status, 200, `${path}: ${await (r.status !== 200 ? r.text() : Promise.resolve(''))}`); return r.json();
}
const create = () => api('/api/sessions', {});
const order = (id, body) => api(`/api/sessions/${id}/command`, body);
const observe = id => api(`/api/sessions/${id}`);
if (process.argv.includes('--restored')) {
  const expected = JSON.parse(await readFile('artifacts/browser/restart-check.json', 'utf8'));
  const actual = await observe(expected.id);
  assert.equal(actual.step, expected.step); assert.deepEqual(actual.resources, expected.resources); assert.deepEqual(actual.people, expected.people);
  assert(actual.paused); console.log('PASS service restart restores the exact saved world, paused'); process.exit(0);
}
const opening = await create(); assert(opening.paused); assert.equal(opening.resources.food, 8); assert.equal(opening.resources.housed, 4);
const invalid = { type: 'garden', x: opening.buildings[0].x, z: opening.buildings[0].z };
assert.equal((await api(`/api/sessions/${opening.id}/preview`, invalid)).valid, false);
const rejected = await order(opening.id, invalid); assert.equal(rejected.accepted, false); assert.deepEqual(rejected.state.resources, opening.resources);
assert.equal((await order(opening.id, { type: 'cancel', siteId: opening.buildings[0].id })).accepted, false);
assert.equal((await order(opening.id, { type: 'advance', seconds: 300 })).accepted, false);
console.log('PASS invalid placement/cancellation/time commands preserve the village');

const results = [];
for (const route of ['repair', 'garden']) {
  let s = await create();
  if (route === 'garden') {
    for (let i = 0; i < 8; i++) s = (await order(s.id, { type: 'advance', seconds: 30, untilEvent: false })).state;
    assert.equal(s.resources.food, 0); assert(s.resources.fed < 4);
    const p = await api(`/api/sessions/${s.id}/preview`, { type: 'garden', x: 0, z: 6 }); assert(p.valid);
    assert((await order(s.id, { type: 'garden', x: 0, z: 6 })).accepted);
    const site = (await observe(s.id)).buildings.find(b => b.kind === 'VegetableGarden');
    assert((await order(s.id, { type: 'cancel', siteId: site.id })).accepted);
    assert((await order(s.id, { type: 'garden', x: 0, z: 6 })).accepted);
  } else {
    assert((await order(s.id, { type: 'repair' })).accepted);
    assert((await order(s.id, { type: 'grove', x: -11, z: 6, x2: -6, z2: 10 })).accepted);
  }
  for (let i = 0; i < 30 && !s.complete; i++) s = (await order(s.id, { type: 'advance', seconds: 30, untilEvent: false })).state;
  assert(s.complete, `${route} did not finish`); assert(s.objectives.every(t => t.startsWith('✓')));
  const marked = await order(s.id, { type: 'bookmark', note: 'Regression bookmark', view: { active: true, panel: 'garden', cameraX: 2, cameraZ: 4, zoom: 12, width: 390, height: 844, message: 'A deliberately visible explanation', previewCells: [{x:0,z:6}], previewValid: true } });
  const raw = await (await fetch(`${base}/api/sessions/${s.id}/recording`)).text(); const frames = raw.trim().split('\n').map(JSON.parse);
  const bookmark = frames.at(-1); assert.equal(bookmark.type, 'bookmark'); assert.equal(bookmark.state.view.message, 'A deliberately visible explanation');
  assert.deepEqual(bookmark.state.objectives, marked.state.objectives); assert(frames.some(f => f.type === 'frame'));
  results.push({ route, id: s.id, seconds: s.seconds, frames: frames.length, complete: s.complete });
  console.log(`PASS ${route}: ordinary legal commands complete at ${s.seconds}s; recorded objectives, preview and bookmark preserved`);
}
const interrupted = await create(); await order(interrupted.id, { type: 'resume', speed: 6 });
await api(`/api/sessions/${interrupted.id}/heartbeat`, { active: false });
assert((await observe(interrupted.id)).paused);
const resumed = (await order(interrupted.id, { type: 'resume' })).state;
await new Promise(resolve => setTimeout(resolve, 6800));
const disconnected = await observe(interrupted.id); assert(disconnected.paused); assert(disconnected.seconds - resumed.seconds <= 6.5);
const stable = await observe(interrupted.id); assert.equal(stable.step, disconnected.step);
console.log('PASS backgrounding and missing heartbeat pause without a spectator keeping the village alive');
const cors = await fetch(base + '/api/sessions', { method: 'POST', headers: { Origin: 'https://unrelated.example', 'Content-Type': 'application/json' }, body: '{}' }); assert.equal(cors.status, 403);
console.log('PASS cross-site browser requests are rejected');
const durable = (await order(opening.id, { type: 'advance', seconds: 19, untilEvent: false })).state;
await mkdir('artifacts/browser', { recursive: true });
await writeFile('artifacts/browser/restart-check.json', JSON.stringify(durable));
await writeFile('artifacts/browser/checks.json', JSON.stringify({ build: opening.build, results, interrupted: disconnected.id }, null, 2));
console.log('PASS saved restart fixture and comparison evidence');
