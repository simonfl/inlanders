"""Summarize completed native frame intervals; no third-party dependencies."""
import json, math, statistics, sys
from pathlib import Path
for name in sys.argv[1:]:
    path=Path(name)
    samples=json.loads(path.read_text(encoding="utf-8-sig"))
    frames=[f for f in samples if f["WallMs"] > 0]
    out={"file":str(path),"completed":len(frames),"excluded":len(samples)-len(frames),"percentile":"nearest rank","wallSeconds":sum(f["WallMs"] for f in frames)/1000}
    for key in ["WallMs","InputMs","SimulationMs","ActorsMs","HudMs","ProcessMs","AllocatedBytes"]:
        values=sorted(f[key] for f in frames)
        out[key]={"median":round(statistics.median(values),3),"p95":round(values[max(0,math.ceil(len(values)*.95)-1)],3),"max":round(max(values),3)} if values else None
    print(json.dumps(out))
