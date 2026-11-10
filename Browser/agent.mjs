// The same legal actions used by the touch client. No save-file or debug access.
const base = process.env.INLANDERS_URL || 'http://127.0.0.1:5180';
const [action = 'guide', id, raw] = process.argv.slice(2);
let path = '/api/guide', method = 'GET', body;
if (action === 'new') { path = '/api/sessions'; method = 'POST'; body = {}; }
else if (['observe', 'recording'].includes(action)) path = `/api/sessions/${id}${action === 'recording' ? '/recording' : ''}`;
else if (action === 'command' || action === 'preview') { path = `/api/sessions/${id}/${action}`; method = 'POST'; body = JSON.parse(raw || '{}'); }
else if (action !== 'guide') throw new Error('Use guide | new | observe ID | command ID JSON | preview ID JSON | recording ID');
const response = await fetch(base + path, { method, headers: body ? { 'Content-Type': 'application/json' } : undefined, body: body ? JSON.stringify(body) : undefined });
const text = await response.text();
if (!response.ok) { console.error(text); process.exitCode = 1; }
else if (action === 'recording') console.log(text);
else {
  const result = JSON.parse(text), state = result.state || result;
  // Compress the visible map for model context; this does not omit obstacles.
  if (state.map?.water) {
    const rows = new Map();
    for (const {x,z} of state.map.water) { if (!rows.has(z)) rows.set(z, []); rows.get(z).push(x); }
    state.map.waterRows = [...rows].sort((a,b) => a[0]-b[0]).map(([z,xs]) => ({ z, xs: xs.sort((a,b) => a-b) }));
    delete state.map.water;
  }
  console.log(JSON.stringify(result, (key, value) => key === 'point' ? undefined : value));
}
