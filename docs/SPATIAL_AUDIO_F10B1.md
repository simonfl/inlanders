# Spatial sound review — F10b1

September 12, 2026. Technical part of F10b; subjective soundscape acceptance remains open as F10b2.

## Finding and change

The orthographic camera stands `max(25, 1.5 * map extent)` units horizontally from the viewed ground, plus 0.96 times that distance vertically. This distance serves map framing/clipping and does not change with zoom. Previously the camera was also the audio listener. Effects and birds have finite audible ranges, so the same nearby work could be much quieter on a larger map, or disappear altogether even when zoomed in.

A dedicated AudioListener3D now follows the viewed ground and keeps the camera's stereo orientation. Listening distance is `clamp(0.8 * camera size, 12, 40)`, independent of map extent. Close views bring work closer; wide views recede. Camera movement, orbit, following and world changes update it through the existing camera/audio paths. No Doppler effect, additional sound voices or per-frame sample generation is introduced.

This changes world effects and positional birds. UI, wind and music retain their buses and controls. Work-contact timing, cue cooldowns, sample synthesis, simulation speed, simulation state and save format are unchanged. Repetition limits and new variations were not retuned without listening evidence.

## Evidence

The live mixer probe on the quarry map measures a focused hammer at Effects 65, Nature/Music zero:

| Listener/view | Peak amplitude |
| --- | ---: |
| Previous camera listener, close view | 0.006505 |
| Dedicated listener, close view | 0.099810 |
| Dedicated listener, wide view | 0.026055 |

These demonstrate repaired spatial behavior, not a perceptual preference. Wide-view listening distance remains 32 units even if the camera's clipping distance is tripled.

`./Play.ps1 -AudioSmokeTest` passes the existing PCM/loop, live mixer, mute, effects/nature/music control persistence, paused work suppression, fixed voice pool and simulation-independence checks. Build passes with zero warnings/errors. Godot prints the existing root-certificate-store warning.

The additional review passed all 24 captures. Use `./Play.ps1 -SoundscapeSmokeTest` to build and run it, or run the built console engine directly:

```powershell
$env:APPDATA = Join-Path $PWD '.tools/appdata'
& .tools/godot/Godot_v4.6-stable_mono_win64/Godot_v4.6-stable_mono_win64_console.exe --path . -- --audio-smoke-test --soundscape
```

It constructs reproducible quiet, working and waterfront worlds, then records each at zoom 12/40 and speed 1x/4x with the old camera listener and new listener. Each eight-second simulation excerpt uses the same starting save, tick sequence, music phrase and nature schedule; timer/render overhead can slightly change recording duration. This is a comparison of actual mixes, not offline summation of isolated samples. All takes use Effects 65 / Nature 40 / Music 35.

Outputs live in `artifacts/soundscape`: the three initial saves, 24 stereo WAVs and `results.json` with peaks, sample counts and world-cue counts. The check asserts no clipped/empty capture, unchanged simulation continuation, bounded voices, reset without replayed prior events, pause, mute and zero Effects. It restores the current listener on exit. These generated files are excluded from Git; rerunning the command recreates them.

Run ./ReviewSoundscape.ps1 after the capture check to generate the [local comparison player](../artifacts/soundscape/index.html). It shows all twelve before/after pairs and pauses the other recording when playback starts. [Checked-in measurements](SPATIAL_AUDIO_RESULTS.json) preserve this run without committing the WAV files.

## Remaining listening review — F10b2

Listen to complete before/after pairs in all three scenes, including accelerated play. Judge whether focused impacts dominate music, whether close footsteps become repetitive, whether the waterfront remains too empty, and whether wide scenes lose useful cues. Choose any sample variation, per-cue gain or cadence change from those findings. A mixer peak is not evidence that a sound is more organic or enjoyable. No listening judgment is claimed in F10b1.

The queue advances to F21p resident management while this perceptual review remains explicit. F17b musical variation should use the same mix comparison. Chapel/court remain silent; no artificial arrival ceremony was added.
