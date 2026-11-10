// Engineering comparison through ordinary commands, not independent play.
import assert from 'node:assert/strict';
import { writeFile, mkdir } from 'node:fs/promises';
const base = process.env.INLANDERS_URL || 'http://127.0.0.1:5180';
async function api(path, body) {
  const r = await fetch(base + path, body === undefined ? {} : { method:'POST', headers:{'Content-Type':'application/json'}, body:JSON.stringify(body) });
  assert.equal(r.status, 200, `${path}: ${r.status === 200 ? '' : await r.text()}`);
  return r.json();
}
const results = [];
for (const route of process.argv.slice(2).length ? process.argv.slice(2) : ['fields', 'gardens', 'late-recovery']) {
  let s = process.env.INLANDERS_TEST_SESSION ? await api(`/api/sessions/${process.env.INLANDERS_TEST_SESSION}`) : await api('/api/sessions?scenario=river-closing', {});
  const id = s.id, milestones = [], ordered = [];
  assert(s.paused && s.resources.population === 4 && s.resources.beds === 4);
  assert(s.buildings.find(b => b.kind === 'Bridge')?.complete);
  async function order(o) {
    const before = s.river.phase;
    const r = await api(`/api/sessions/${id}/command`, o); s = r.state;
    if (s.river.phase !== before) { assert(s.paused); milestones.push({seconds:s.seconds,phase:s.river.phase,resources:s.resources}); }
    return r;
  }
  assert.equal((await order({type:'next-season'})).accepted, false);
  const invalid = {type:'home',x:s.buildings[0].x,z:s.buildings[0].z};
  assert.equal((await api(`/api/sessions/${id}/preview`,invalid)).valid,false);
  assert.equal((await order(invalid)).accepted,false);
  async function advanceTo(seconds) {
    while(s.seconds < seconds && !s.complete) await order({type:'advance',seconds:30,untilEvent:false});
  }
  async function place(type, target) {
    const cells = [];
    for(let z=s.map.minZ;z<s.map.minZ+s.map.depth;z++) for(let x=s.map.minX;x<s.map.minX+s.map.width;x++) cells.push({x,z});
    cells.sort((a,b)=>Math.hypot(a.x-target.x,a.z-target.z)-Math.hypot(b.x-target.x,b.z-target.z));
    for(const cell of cells) for(const rotation of [0,1]) {
      const o={type,...cell,rotation};
      const p=await api(`/api/sessions/${id}/preview`,o);
      if(p.valid) { assert((await order(o)).accepted); ordered.push(o); return; }
    }
    throw Error(`No legal site for ${type}`);
  }
  if(route==='late-recovery') {
    await advanceTo(1470);
    assert(!s.complete && s.resources.population===12 && s.resources.housed<12);
    console.log(`PASS neglect does not win: ${s.resources.fed}/12 fed, ${s.resources.housed}/12 housed`);
  }
  assert((await order({type:'grove',x:-11,z:0,x2:8,z2:12})).accepted);
  await advanceTo(s.seconds+90);
  await place('home',{x:1,z:6});
  const testSite=s.buildings.filter(b=>!b.complete&&b.kind==='Cottage').at(-1);
  assert((await order({type:'cancel',siteId:testSite.id})).accepted);
  await place('home',{x:1,z:6});
  await place('home',{x:5,z:6});
  const gardenOnly=route==='gardens';
  await place(gardenOnly?'garden':'field',{x:-6,z:5});
  await place('garden',{x:2,z:10});
  await advanceTo(s.seconds+210);
  await place('home',{x:6,z:2});
  await place('home',{x:-8,z:2});
  await place(gardenOnly?'garden':'field',{x:4,z:10});
  await place('garden',{x:-6,z:10});
  if(gardenOnly) { await place('garden',{x:-3,z:4}); await place('garden',{x:6,z:9}); }
  const limit=route==='late-recovery'?3600:2400;
  while(!s.complete && s.seconds<limit) {
    if(route==='fields' && !s.river.closed && !s.river.earlyProblem) assert((await order({type:'next-season'})).accepted);
    else await order({type:'advance',seconds:30,untilEvent:false});
  }
  results.push({route,id,build:s.build,complete:s.complete,seconds:s.seconds,resources:s.resources,objectives:s.objectives,hint:s.hint,milestones,ordered});
  await mkdir('artifacts/browser',{recursive:true});
  await writeFile(`artifacts/browser/river-${route}.json`,JSON.stringify(results.at(-1),null,2));
  assert(s.complete, `${route}: ${s.hint}`);
  assert.equal(s.resources.population,12); assert.equal(s.resources.housed,12); assert.equal(s.river.stableSeconds,240);
  assert(s.objectives.every(line=>line.startsWith('✓')));
  await order({type:'bookmark',note:`Engineering ${route} completion`});
  console.log(`PASS ${route} completes at ${s.seconds}s with ${s.resources.food} food; arrival pauses and all finish checks hold`);
}
