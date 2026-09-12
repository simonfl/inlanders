# F17b1 — phrase completion and quiet intervals

The original 96-second miniature now finishes instead of looping directly. A single reusable timer leaves 18, 26 and 22 real seconds of quiet in rotation before replaying it. These timings are provisional; they are not a claim of an auditioned mix or final musical pacing.

One music player and one pre-rendered stream remain alive across settlement loads, menu transitions and pause. Mute/zero volume silence the bus while transport continues. The gap timer ignores engine time scale and does not use simulation speed or the capped frame delta. There are no per-frame sample allocations, additional voices, abrupt scene-driven fades or new assets. The existing two-second edge envelopes finish and begin at silence. Transport is session-local, not part of a settlement save.

## Verification

- `Play.ps1 -AudioSmokeTest`: existing effects checks, music PCM bounds, faded endpoints, captured silence during a real gap, actual timer completion/restart, next interval selection, unchanged player/stream, load/speed/pause independence, mute/volume/master mute and preferences.
- `Play.ps1 -MenuSmokeTest`: real Settings, new settlement, return and Continue transitions retain playback position or remaining quiet interval. Existing menu/save/recovery checks pass.
- Matched soundscape capture explicitly starts the piece at the same offset for each case; a previous quiet interval cannot accidentally omit music from a comparison.

The tests seek near the phrase end to exercise completion promptly; they wait through the actual first quiet interval. PCM inspection and mixer capture do not establish perceptual appeal. Only the existing piece is present: related themes and avoidance of consecutive theme repeats remain F17b2.

## Roadmap review

F17b1 is delivered. F10b2 listening and F17b theme/mix review remain unfinished. Next, create two related candidate themes within a fixed stream budget and alternate them without immediate repeats, preserving the transport checks. Export full pieces and transitions for audition before calling the score complete. A listening result may still change or reject the instrumentation, intervals or mix.
