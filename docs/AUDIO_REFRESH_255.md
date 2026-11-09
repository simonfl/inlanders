# Acoustic presentation pass — November 9, 2026

The owner explicitly requested music and sound improvements while unable to play the longer scenario. This is presentation work; Before the river closes remains awaiting gameplay feedback.

## Direction and implementation

Three original 96-second stereo miniatures replace the simple mono sine/pluck arrangements: **Au bord du fleuve**, **Le bois et le pain**, and **La veillée**. Authored modal melodies sit over broken string chords and bass notes; the middle sections introduce sparse breathy woodwind phrases. Timing/velocity variation, frequency-dependent string decay, short room reflections, rests and a quieter coda give the arrangements shape. These are synthesized acoustic-inspired sounds, not recorded instruments or a historical reconstruction.

Everyday footsteps, chopping, hammering, rustling, sawing, drops and bird calls each have four versions. Wood impacts use brief material resonances and noise transients. Footsteps and continuous work textures sit below salient action feedback. Completion is a short rising plucked phrase, with one of the existing six voices reserved so busy workers cannot consume its channel. Real-time cadence limits keep fast-forward from multiplying the sound rate.

Wind has slower gust layers. A quiet river texture follows distance from the camera focus to actual water cells, fades across camera movements, and disappears inland/on dry maps. Nature continues while paused. Existing Effects/Nature/Music volume controls and mute behavior remain intact. Music remains independent of pause and simulation speed, with the existing 18/26/22-second quiet intervals between pieces.

All composition and synthesis are original repository code. No downloaded music, samples, licenses, external service, new runtime package or large binary asset was added. Themes render once at startup, then use one music voice. Retained music PCM is 25,401,600 bytes; temporary per-note caches are released after composition. Native synthesis measured about 2.4 seconds on this machine.

## Evidence and audition

The pre-change native audio suite passed and exported the old dry tracks/effects to artifacts/audio-refresh/before. Updated checks export new material to artifacts/audio-refresh/after. ReviewAudioRefresh.ps1 produces artifacts/audio-refresh/index.html with old/new dry arrangements and effects, plus new-build game-mixer excerpts. The soundtrack export also preserves complete quiet-interval transitions under artifacts/music-candidates.

The first native check caught a loop-boundary discontinuity in the river texture. A short endpoint correction after the crossfade resolves it. Final native results and evaluator verdict follow below.

Use the existing engine command with --audio-smoke-test --audio-refresh after building to rerun. The focused extension checks stereo exports, distinct effect variants, real-water proximity and smooth inland fading, absence on a dry map, reserved completion delivery, bounded voices/cadence, and unchanged simulation alongside actual 1x/6x mixer capture. The earlier audio suite checks actual volume/mute controls, saved settings, pause/reset behavior and timed music silence/resumption.

This is not an aesthetic listening verdict. The independent evaluator has source/PCM/mixer evidence, but no verified listening path. Numerical safety, a melodic score and passing transport checks cannot establish pleasant timbre, lack of fatigue or historical character. The owner can compare the previews later without playing the level.

## Decision

Deliver the audio pass provisionally and hold further soundtrack work for listening feedback. Keep the longer-level gameplay stop intact. Ask whether the sound feels warmer and whether any specific cue repeats too often, is too artificial, or masks the village. Do not add dynamic musical states, more tracks, or another audio framework before that evidence.
## Final verification and independent verdict

Fixed build: `CE639BF5A91B77864D0D50FDF1B6F5E62A6AB353E45D13B4BA286704B2F76562`, zero warnings/errors. Full native audio and music suite plus the focused refresh checks pass. Final startup synthesis: 2.26s. Actual test-window mute/volume controls and persistence, paused music transport, timed quiet/resume, load/speed independence and current world invariance all pass.

The independent audio evaluator checked the score/source and exported PCM. Its numerical comparison found the new dry arrangements about 2.4dB quieter than the old opening theme, while wind could mask them. Final balance raises the music player by 2dB and reduces wind by 6dB, retaining the user's volume sliders. Native recordings were regenerated after that change.

| Native scene | Speed | Duration | Peak | RMS | Work cues |
| --- | ---: | ---: | ---: | ---: | ---: |
| Recovering village | 1x | 10.41s | 0.0842 | 0.01005 | 28 |
| Recovering village | 6x | 10.28s | 0.0800 | 0.01061 | 44 |
| Settled twelve-person village | 1x | 11.10s | 0.0481 | 0.00989 | 21 |
| Settled twelve-person village | 6x | 11.31s | 0.0641 | 0.01019 | 39 |

The settled fixture is the measured successful long-scenario save, not a new gameplay trial. Excerpts are native mixer output over scripted simulation; they are not an ordinary playthrough or a whole-session fatigue test. Forty local WAV exports including a short preview are available; all 22 media links in the comparison page resolve.

Evaluator verdict: ready to deliver provisionally, no blocking implementation issue. Coherent phrase structure, variants, quiet work layers, proximity and completion handling support the intended direction. Independent checks confirm three distinct stereo themes, silent music boundaries, matching ambience-loop endpoints and unclipped recorded mixes. Listening quality remains unverified. Stop here for the owner's later audition and the pending gameplay trial.