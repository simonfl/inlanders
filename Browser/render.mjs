// Local development only: isolated headless Chrome, real DOM controls, ordinary game API.
// Produces rendered evidence when the Windows desktop is unavailable; not a phone playtest.
import { spawn } from 'node:child_process';
import { mkdir, readFile, readdir, writeFile } from 'node:fs/promises';
import { resolve, join } from 'node:path';

const args=process.argv.slice(2),arg=(name,fallback)=>args.includes(name)?args[args.indexOf(name)+1]:fallback;
const url=new URL(arg('--url','http://127.0.0.1:5180/?scenario=willow-home'));
if(url.hostname!=='127.0.0.1'||url.port!=='5180')throw Error('Only the local game is supported.');
const folder=resolve(arg('--out','artifacts/browser/willow-home/render'));
if(!folder.startsWith(resolve('artifacts')+'\\'))throw Error('Keep capture output inside repository artifacts.');
await mkdir(folder,{recursive:true});
const profile=resolve(arg('--profile',join(folder,`profile-${Date.now()}`)));
if(!profile.startsWith(resolve('artifacts')+'\\'))throw Error('Use an isolated profile inside repository artifacts.');
await mkdir(profile,{recursive:true});
const browser=spawn('C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe',[
  '--headless=new','--no-first-run','--no-default-browser-check',`--user-data-dir=${profile}`,'--remote-debugging-port=0',
  '--use-angle=swiftshader','--enable-unsafe-swiftshader','--hide-scrollbars','about:blank'
],{windowsHide:true,stdio:'ignore'});
const sleep=ms=>new Promise(r=>setTimeout(r,ms));let socket,serial=0,session,gameId;const pending=new Map();
function call(method,params={},target=session){return new Promise((res,rej)=>{const id=++serial;pending.set(id,{res,rej});socket.send(JSON.stringify({id,method,params,...(target?{sessionId:target}:{})}));});}
async function value(expression){return (await call('Runtime.evaluate',{expression,returnByValue:true,awaitPromise:true})).result.value;}
try{
  let endpoint;
  for(let i=0;i<100;i++){try{const lines=(await readFile(join(profile,'DevToolsActivePort'),'utf8')).trim().split(/\r?\n/);endpoint=`ws://127.0.0.1:${lines[0]}${lines[1]}`;break;}catch{await sleep(100);}}
  if(!endpoint)throw Error('Headless Chrome did not start.');
  socket=new WebSocket(endpoint);await new Promise((r,j)=>{socket.onopen=r;socket.onerror=j;});
  socket.onmessage=e=>{const m=JSON.parse(e.data),p=pending.get(m.id);if(p){pending.delete(m.id);m.error?p.rej(Error(JSON.stringify(m.error))):p.res(m.result);}};
  const target=await call('Target.createTarget',{url:'about:blank'},null);
  session=(await call('Target.attachToTarget',{targetId:target.targetId,flatten:true},null)).sessionId;
  await call('Page.enable');await call('Runtime.enable');
  await call('Emulation.setDeviceMetricsOverride',{width:Number(arg('--width','402')),height:Number(arg('--height','740')),deviceScaleFactor:1,mobile:true});
  await call('Page.navigate',{url:url.href});
  for(let i=0;i<100;i++){if(await value('document.querySelector("#loading")?.hidden===true'))break;await sleep(100);}
  const buildText=await value('document.querySelector("#build")?.textContent');
  const prefix=buildText?.match(/session ([a-f0-9]+)/)?.[1];
  if(!prefix)throw Error(`Game did not load: ${await value('document.body.innerText')}`);
  gameId=(await readdir(resolve('artifacts/browser/sessions'))).find(x=>x.startsWith(prefix));
  if(!gameId)throw Error('Could not identify this test session.');
  console.log(JSON.stringify({gameId,buildText}));
  async function capture(name){await sleep(600);const shot=await call('Page.captureScreenshot',{format:'png'});await writeFile(join(folder,`${name}.png`),Buffer.from(shot.data,'base64'));await writeFile(join(folder,`${name}.txt`),await value('document.body.innerText'));const state=await(await fetch(`${url.origin}/api/sessions/${gameId}`)).json();await writeFile(join(folder,`${name}.json`),JSON.stringify(state,null,2));}
  async function tap(x,y){await call('Input.dispatchMouseEvent',{type:'mousePressed',x,y,button:'left',clickCount:1});await call('Input.dispatchMouseEvent',{type:'mouseReleased',x,y,button:'left',clickCount:1});await sleep(450);}
  const steps=args.includes('--steps')?JSON.parse(await readFile(arg('--steps'),'utf8')):[];
  await capture('opening');
  for(const step of steps){
    if(step.click){const rect=await value(`(()=>{const e=document.querySelector(${JSON.stringify(step.click)});if(!e||e.disabled)return null;const r=e.getBoundingClientRect();if(!r.width||!r.height)return null;return {x:r.x+r.width/2,y:r.y+r.height/2};})()`);if(!rect)throw Error(`Missing, hidden or disabled control ${step.click}`);await tap(rect.x,rect.y);}
    if(step.tap)await tap(step.tap.x,step.tap.y);
    if(step.drag){const {from,to}=step.drag;await call('Input.dispatchMouseEvent',{type:'mousePressed',x:from.x,y:from.y,button:'left',clickCount:1});for(let i=1;i<=12;i++){await call('Input.dispatchMouseEvent',{type:'mouseMoved',x:from.x+(to.x-from.x)*i/12,y:from.y+(to.y-from.y)*i/12,button:'left',buttons:1});await sleep(25);}await call('Input.dispatchMouseEvent',{type:'mouseReleased',x:to.x,y:to.y,button:'left',clickCount:1});await sleep(450);}
    if(step.command)for(let i=0;i<(step.repeat??1);i++){const r=await fetch(`${url.origin}/api/sessions/${gameId}/command`,{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify(step.command)});const result=await r.json();if(!r.ok||!result.accepted)throw Error(JSON.stringify(result));}
    if(step.until){let matched=false;for(let i=0;i<=Math.min(60,step.maxSteps??40);i++){const state=await(await fetch(`${url.origin}/api/sessions/${gameId}`)).json();if(step.until.split('.').reduce((v,k)=>v?.[k],state)===step.equals){matched=true;break;}const r=await fetch(`${url.origin}/api/sessions/${gameId}/command`,{method:'POST',headers:{'Content-Type':'application/json'},body:'{"type":"advance","seconds":15,"untilEvent":false}'});const result=await r.json();if(!r.ok||!result.accepted)throw Error(JSON.stringify(result));}if(!matched)throw Error(`Public condition not reached: ${step.until}`);await sleep(600);}
    if(step.wait)await sleep(Math.min(step.wait,15000));
    if(step.motion)for(let i=0;i<Math.min(30,step.motion);i++){await capture(`motion-${String(i).padStart(2,'0')}`);await sleep(400);}
    if(step.capture)await capture(step.capture);
  }
  await capture('final');
  console.log(JSON.stringify({gameId,folder,viewport:await value('({width:innerWidth,height:innerHeight,mapHeight:document.querySelector("#world").clientHeight})')}));
}finally{
  if(gameId&&!url.searchParams.has('watch')&&!url.searchParams.has('replay'))await fetch(`${url.origin}/api/sessions/${gameId}/command`,{method:'POST',headers:{'Content-Type':'application/json'},body:'{"type":"pause"}'}).catch(()=>{});
  if(socket?.readyState===1){await call('Browser.close',{},null).catch(()=>{});socket.close();}else browser.kill();
}
