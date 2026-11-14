// Summarize recorded evidence; action counts and quiet intervals do not measure enjoyment.
import { readFile } from 'node:fs/promises';
import path from 'node:path';
const id = process.argv[2];
if (!/^[a-f0-9]{32}$/.test(id ?? '')) throw Error('Provide one recorded session id.');
const frames = (await readFile(path.join('artifacts/browser/sessions',id,'recording.jsonl'),'utf8')).trim().split('\n').map(JSON.parse);
const first = frames[0], last = frames.at(-1);
const gameplay = new Set(['repair','home','garden','field','dock','grove','cancel','next-season','trade-buy','trade-decline','trade-cancel','shortcut','welcome','garden-entrance']);
const orders = frames.filter(f => f.type === 'command' && gameplay.has(f.data?.order?.type)).map(f => ({seconds:f.state.seconds,...f.data}));
const phase = s => s.complete ? 'complete' : s.riverside ? s.riverside.phase : s.trade ? `${s.trade.decision}/${s.trade.arrived?'arrived':'preparing'}` : s.river?.phase ?? 'recovery';
const milestones = frames.filter((f,i) => i === 0 || phase(f.state) !== phase(frames[i-1].state)).map(f => ({seconds:f.state.seconds,phase:phase(f.state),resources:f.state.resources}));
console.log(JSON.stringify({id,build:first.state.build,scenario:first.state.scenario,complete:last.state.complete,seconds:last.state.seconds,
  minimumRecordedFood:Math.min(...frames.map(f=>f.state.resources.food)),
  maximumRecordedHungry:Math.max(...frames.map(f=>f.state.resources.population-f.state.resources.fed)),
  finalResources:last.state.resources,milestones,orders,
  secondsAfterLastAcceptedOrder:last.state.seconds-(orders.findLast(o=>o.accepted)?.seconds??0),
  bookmarks:frames.filter(f=>f.type==='bookmark').map(f=>({seconds:f.state.seconds,note:f.data?.order?.note})),
  caveat:'Recorded samples and actions only. Quiet time can contain observation or planning; it is not a boredom score.'},null,2));
