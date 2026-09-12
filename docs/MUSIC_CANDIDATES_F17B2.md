# F17b2 — related theme candidates

Three 96-second pieces now cycle in a fixed order: **First clearing**, **Under the trees**, **Homeward**. The latter two use different chord ordering, melodies and note spacing in the same C / A-minor family. Under the trees is lower and sparser. Shared synthesized plucks and sustained chords keep instrumentation consistent. There are no consecutive repeats, random state dependencies or scene-triggered restarts.

The three retained mono streams contain 12,700,800 PCM bytes (about 12.1 MiB), plus engine/runtime overhead. They are generated once at startup and share one playback voice. No general startup-time or total-memory claim follows from the PCM count. Quiet intervals remain 18, 26 and 22 seconds; final timing and musical selection await audition.

## Listening material

Run `Play.ps1 -AudioSmokeTest` to regenerate `artifacts/music-candidates/index.html`, complete theme WAVs and full transitions (two pieces with the actual intervening gap). The page explains that these are dry music exports, not the complete village mix. Transition buffers exist only during export, not normal playback. Generated WAVs are local artifacts, not committed assets.

PCM peaks are 5,559, 4,509 and 5,546 out of 32,767. All pieces have silent faded boundaries, distinct PCM and the expected duration. This establishes neither musical appeal nor preferred loudness.

## Verification and status

Live audio checks pass for real phrase completion, timed silence, progression into the second existing stream, third-theme selection and cycle wrap, stable player identity, mute/volume/preferences and load/pause/speed independence. Menu/new-game/Continue checks also pass. Soundscape fixtures explicitly select First clearing at a fixed offset to retain matched comparisons.

Candidate implementation and exports are delivered. **F10b2 listening and final F17b acceptance remain open.** Feedback was requested with the listening page. Do not infer acceptance from no response or from audio counters.

## Next roadmap decision

Move the candidate-generation entry out of the active queue. Keep sound/musical acceptance visible, and promote the existing Creative multi-object removal idea into a bounded design/implementation chunk (F16d). Arrange-and-revise tools build on the shipped relocation flow without inventing another producer or mandatory villager need.
