"""Extract review stills and PCM audio from Godot MJPEG AVI without extra packages.
Usage: python Development/ExtractReviewMovie.py artifacts/review/runs/<run>
This does not judge motion/audio quality or measure native frame performance.
"""
import json
import struct
import sys
import wave
from pathlib import Path

run = Path(sys.argv[1]).resolve()
data = (run / "observation.avi").read_bytes()
if data[:4] != b"RIFF" or data[8:12] != b"AVI ":
    raise ValueError("Expected a Godot AVI movie")

def chunks(start, end):
    while start + 8 <= end:
        kind = data[start:start + 4]
        size = struct.unpack_from("<I", data, start + 4)[0]
        body = start + 8
        if body + size > end:
            raise ValueError("Truncated AVI chunk")
        yield kind, body, size
        start = body + size + (size & 1)

frames, audio = [], []
formats = []
def visit(start, end):
    for kind, body, size in chunks(start, end):
        if kind in (b"LIST", b"RIFF"):
            if data[body:body+4] == b"strl":
                entries = list(chunks(body+4, body+size))
                header = next((data[p:p+n] for k,p,n in entries if k == b"strh"), b"")
                if header[:4] == b"auds":
                    formats.extend(data[p:p+n] for k,p,n in entries if k == b"strf")
            visit(body+4, body+size)
        elif kind[2:] in (b"db", b"dc") and data[body:body+2] == b"\xff\xd8":
            frames.append((body,size))
        elif kind[2:] == b"wb":
            audio.append(data[body:body+size])
visit(12, len(data))
if not frames:
    raise ValueError("No JPEG frames found")
for name, index in (("start",0),("early",len(frames)//3),("late",len(frames)*2//3),("end",len(frames)-1)):
    at, size = frames[index]
    (run / f"movie-{name}.jpg").write_bytes(data[at:at+size])
seconds = None
if formats and audio:
    encoding, channels, rate, _, _, bits = struct.unpack_from("<HHIIHH", formats[0])
    if encoding != 1 or bits % 8:
        raise ValueError("Only integer PCM audio is supported")
    pcm = b"".join(audio)
    with wave.open(str(run / "observation.wav"), "wb") as out:
        out.setnchannels(channels)
        out.setsampwidth(bits//8)
        out.setframerate(rate)
        out.writeframes(pcm)
    seconds = len(pcm)/(channels*(bits//8)*rate)
result = dict(frames=len(frames), audioSeconds=seconds,
              limitation="Extracted frames/audio are evidence access, not listening, motion or performance acceptance")
(run / "movie-inspection.json").write_text(json.dumps(result,indent=2)+"\n",encoding="utf-8")
(run / "movie-index.md").write_text("# Review movie\n\n[Original clip](observation.avi) · [Audio](observation.wav)\n\n" +
    " · ".join(f"[{name}](movie-{name}.jpg)" for name in ("start","early","late","end")) +
    "\n\nUse request.json and capture manifests for build, snapshot, speed and settings. Extracted stills do not establish motion quality; encoding time is not native performance.\n",encoding="utf-8")
print(json.dumps(result))
