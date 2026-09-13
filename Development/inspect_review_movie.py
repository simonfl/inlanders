"""Validate Godot's MJPEG/PCM review AVI; export unchanged sample frames and WAV.

Usage: python Development/inspect_review_movie.py <run-directory>
This checks media integrity. It does not judge motion quality or musical appeal.
"""
import argparse
import array
import hashlib
import json
import math
import struct
import sys
import wave
from pathlib import Path

parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument("run", type=Path)
args = parser.parse_args()
source = args.run / "observation.avi"
data = source.read_bytes()
if data[:4] != b"RIFF" or data[8:12] != b"AVI ":
    raise ValueError("Expected a Godot AVI recording")
declared_end = struct.unpack_from("<I", data, 4)[0] + 8
if declared_end > len(data) or declared_end < 12:
    raise ValueError("AVI is truncated or lacks a RIFF container")
# The observed Godot files underreport their RIFF size by 70 bytes. Report that
# discrepancy; independently validate actual chunks, complete index and stream lengths.
frames, audio, formats, headers, indexes = [], [], [], [], []


def scan(start, end):
    while start + 8 <= end:
        tag = data[start:start + 4]
        size = struct.unpack_from("<I", data, start + 4)[0]
        begin, finish = start + 8, start + 8 + size
        if finish > end:
            raise ValueError("Chunk extends past its container")
        if tag == b"LIST":
            scan(begin + 4, finish)
        elif tag == b"00db":
            frames.append(data[begin:finish])
        elif tag == b"01wb":
            audio.append(data[begin:finish])
        elif tag == b"strf":
            formats.append(data[begin:finish])
        elif tag == b"avih":
            headers.append(data[begin:finish])
        elif tag == b"idx1":
            indexes.append(size // 16)
        start = finish + (size & 1)


scan(12, len(data))
if len(formats) != 2 or not headers or not frames or not indexes:
    raise ValueError("Missing finalized video/audio streams or index")
if formats[0][16:20] != b"MJPG":
    raise ValueError("Only the engine's MJPEG writer is supported")
codec, channels, rate, byte_rate, alignment, bits = struct.unpack("<HHIIHH", formats[1])
if codec != 1 or bits != 16 or alignment != channels * 2 or byte_rate != rate * alignment:
    raise ValueError("Expected 16-bit PCM audio")
declared = struct.unpack_from("<I", headers[0], 16)[0]
if declared != len(frames) or sum(indexes) != len(frames) + len(audio):
    raise ValueError("Frame count or stream index differs from recorded data")
pcm = b"".join(audio)
if len(pcm) % alignment:
    raise ValueError("Partial PCM audio sample")
fps = 1_000_000 / struct.unpack_from("<I", headers[0], 0)[0]
video_seconds, audio_seconds = len(frames) / fps, len(pcm) / byte_rate
if abs(video_seconds - audio_seconds) > 1 / fps:
    raise ValueError("Audio and video durations differ by more than a frame")
samples = array.array("h", pcm)
if sys.byteorder != "little":
    samples.byteswap()
out = args.run / "media-check"
out.mkdir(exist_ok=True)
for label, index in [("start", 0), ("middle", len(frames) // 2), ("end", len(frames) - 1)]:
    frame = frames[index]
    if not frame.startswith(b"\xff\xd8") or not frame.endswith(b"\xff\xd9"):
        raise ValueError("Sampled video frame is not a complete JPEG")
    (out / f"{label}.jpg").write_bytes(frame)
with wave.open(str(out / "audio.wav"), "wb") as wav:
    wav.setnchannels(channels)
    wav.setsampwidth(2)
    wav.setframerate(rate)
    wav.writeframes(pcm)
report = dict(
    sha256=hashlib.sha256(data).hexdigest(), frames=len(frames), fps=round(fps, 4),
    riffSizeDiscrepancyBytes=len(data) - declared_end,
    videoSeconds=video_seconds, audioSeconds=audio_seconds, channels=channels, sampleRate=rate,
    distinctFrames=len({hashlib.sha256(frame).digest() for frame in frames}),
    audioPeak=max((abs(value) for value in samples), default=0) / 32768,
    audioRms=math.sqrt(sum(value * value for value in samples) / max(1, len(samples))) / 32768,
    limitation="Integrity and signal measurements only; not motion observation or listening.",
)
(out / "media.json").write_text(json.dumps(report, indent=2), encoding="utf-8")
print(json.dumps(report, indent=2))
