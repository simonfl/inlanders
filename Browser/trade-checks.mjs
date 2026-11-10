import assert from 'node:assert/strict';
import { readFile, writeFile, mkdir } from 'node:fs/promises';
const base=process.env.INLANDERS_URL || 'http://127.0.0.1:5180';
async function api(path,body,expected=200) {
  const r=await fetch(base+path,body===undefined?{}:{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify(body)});
  assert.equal(r.status,expected,await r.clone().text()); return r.json();
}
const file='artifacts/browser/trade260/restart.json';
const stable=s=>({build:s.build,step:s.step,resources:s.resources,trade:s.trade,objectives:s.objectives,complete:s.complete});
if(process.argv.includes('--restore')) {
  const saved=JSON.parse(await readFile(file,'utf8')), restored=await api(`/api/sessions/${saved.id}`);
  assert.deepEqual(stable(restored),saved.state); assert(restored.paused);
  console.log('PASS current-build service restart restores exact trade, goals, resources and time safely paused');
} else {
  const s=await api('/api/sessions?scenario=timber-trade',{}), id=s.id;
  const command=o=>api(`/api/sessions/${id}/command`,o);
  assert((await api(`/api/sessions/${id}/command`,{type:'next-season'},400)).error.includes('arrival time'));
  let r=await command({type:'trade-buy'});assert(r.accepted);assert.equal(r.state.resources.logs,0);assert.equal(r.state.resources.food,20);
  assert.equal((await command({type:'trade-buy'})).accepted,false);
  while(!r.state.trade.arrived && r.state.seconds<150) r=await command({type:'advance',seconds:30,untilEvent:false});
  assert(r.state.paused && r.state.trade.arrived && r.state.resources.population===8 && !r.state.complete);
  assert(r.state.objectives.some(x=>x.includes('0/8')),'Imported provisions must not satisfy fresh meals after arrival');
  await command({type:'resume',speed:3});
  const paused=await api(`/api/sessions/${id}/heartbeat`,{active:false,panel:'village'});assert(paused.paused);
  await mkdir('artifacts/browser/trade260',{recursive:true});
  await writeFile(file,JSON.stringify({id,state:stable(paused)},null,2));
  console.log(`PASS exchange once, invalid advance, arrival pause, local-food distinction, interruption; restart fixture ${id}`);
}
