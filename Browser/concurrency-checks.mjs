// Probe one isolated village while a separate twelve-person test village advances.
import assert from 'node:assert/strict';
import { writeFile } from 'node:fs/promises';
const [busyId, label='after'] = process.argv.slice(2);
assert(busyId, 'Pass an owned, paused test-session ID');
const base='http://127.0.0.1:5180';
async function api(path, body) {
  const r=await fetch(base+path,body===undefined?{}:{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify(body)});
  assert.equal(r.status,200);return r.json();
}
const idle=await api('/api/sessions',{}), samples=[];
await api(`/api/sessions/${busyId}/command`,{type:'pause'});
for(let i=0;i<3;i++) {
  const start=performance.now();
  const work=api(`/api/sessions/${busyId}/command`,{type:'advance',seconds:30,untilEvent:false});
  await new Promise(r=>setTimeout(r,30));
  const readStart=performance.now(), observed=await api(`/api/sessions/${idle.id}/heartbeat`,{active:false});
  const latency=performance.now()-readStart;
  assert.equal(observed.id,idle.id);assert.equal(observed.step,0);assert.equal(observed.resources.food,8);
  await work;samples.push({heartbeatMs:Math.round(latency),advanceMs:Math.round(performance.now()-start)});
}
// Concurrent commands on the SAME village must still serialize and preserve time.
const before=await api(`/api/sessions/${busyId}`);
await Promise.all([1,2].map(()=>api(`/api/sessions/${busyId}/command`,{type:'advance',seconds:15,untilEvent:false})));
const after=await api(`/api/sessions/${busyId}`);
assert.equal(after.step-before.step,300);
if(label==='after') assert(samples.every(s=>s.heartbeatMs<Math.max(500,s.advanceMs*.8)),'Other village was blocked by an agent advance');
await writeFile(`artifacts/browser/concurrency-${label}.json`,JSON.stringify({build:idle.build,busyId,idleId:idle.id,samples,sameVillageSerialization:true},null,2));
console.log(JSON.stringify({label,samples,sameVillageSerialization:true}));
