using Godot;
using System;

public partial class Game
{
    private enum Cue { Step, Chop, Hammer, Rustle, Saw, Bake, Drop, Click, Place, Reject, Complete, Bird, Wind }

    // Original procedural PCM: short, softly enveloped sounds; no downloaded audio assets.
    private static AudioStreamWav Synthesize(Cue cue)
    {
        const int rate = 22050;
        float duration = cue switch { Cue.Wind => 8, Cue.Bird => 0.7f, Cue.Complete => 0.65f,
            Cue.Saw => 0.48f, Cue.Bake => 0.5f, Cue.Rustle => 0.3f, _ => 0.2f };
        int count = (int)(rate * duration); var samples = new float[count];
        var random = new Random(417 + (int)cue); float low = 0, slow = 0;
        for (int i = 0; i < count; i++)
        {
            float t = i / (float)rate, p = t / duration;
            float noise = (float)random.NextDouble() * 2 - 1;
            low += (noise - low) * 0.13f; slow += (noise - slow) * 0.018f;
            float Tone(float hz) => MathF.Sin(MathF.Tau * hz * t);
            float decay = MathF.Exp(-t * 22);
            float value = cue switch
            {
                Cue.Step => (low * 0.8f + Tone(100) * 0.13f) * MathF.Exp(-t * 28),
                Cue.Chop => (Tone(190) * 0.4f + Tone(310) * 0.18f + noise * MathF.Exp(-t * 80) * 0.3f) * decay,
                Cue.Hammer => (Tone(420) * 0.24f + Tone(910) * 0.13f + noise * MathF.Exp(-t * 95) * 0.2f) * decay,
                Cue.Rustle => low * 0.8f * MathF.Sin(MathF.PI * p),
                Cue.Saw => (noise - low) * 0.18f * (0.4f + 0.6f * MathF.Sin(MathF.PI * p)),
                Cue.Bake => low * 0.22f + (random.NextDouble() < 0.003 ? noise * 0.18f : 0),
                Cue.Drop => (Tone(125) * 0.3f + low * 0.6f) * decay,
                Cue.Click => Tone(700) * 0.2f * MathF.Exp(-t * 50),
                Cue.Place => (Tone(330) + Tone(495)) * 0.15f * MathF.Exp(-t * 14),
                Cue.Reject => Tone(180) * 0.18f * MathF.Exp(-t * 18),
                Cue.Complete => (Tone(440) + Tone(550) + Tone(660)) * 0.10f * MathF.Exp(-t * 5),
                Cue.Bird => MathF.Sin(MathF.Tau * (1900 * t + 160 * t * t) + 2 * MathF.Sin(t * 35)) *
                    MathF.Pow(Math.Max(0, MathF.Sin(t * 21)), 3) * 0.14f,
                Cue.Wind => slow * (0.8f + 0.2f * MathF.Sin(MathF.Tau * p)),
                _ => 0
            };
            float envelope = cue == Cue.Wind ? 1 : Math.Min(1, t / 0.005f) * Math.Min(1, (duration - t) / 0.025f);
            samples[i] = value * envelope;
        }
        if (cue == Cue.Wind)
        {
            // Crossfade the end into the beginning to avoid a discontinuity at the loop seam.
            int fade = rate / 2;
            for (int i = 0; i < fade; i++)
            {
                float blend = i / (float)(fade - 1);
                samples[count - fade + i] = samples[count - fade + i] * (1 - blend) + samples[i] * blend;
            }
            var looped = new float[count - fade]; Array.Copy(samples, fade, looped, 0, looped.Length); samples = looped;
        }
        var data = new byte[samples.Length * 2];
        for (int i = 0; i < samples.Length; i++)
        {
            short pcm = (short)(Math.Clamp(samples[i], -0.9f, 0.9f) * short.MaxValue);
            data[i * 2] = (byte)pcm; data[i * 2 + 1] = (byte)(pcm >> 8);
        }
        return new AudioStreamWav { Format = AudioStreamWav.FormatEnum.Format16Bits, MixRate = rate, Stereo = false, Data = data,
            LoopMode = cue == Cue.Wind ? AudioStreamWav.LoopModeEnum.Forward : AudioStreamWav.LoopModeEnum.Disabled,
            LoopBegin = 0, LoopEnd = samples.Length };
    }
}
