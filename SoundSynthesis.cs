using Godot;
using System;

public partial class Game
{
    private enum Cue { Step, Chop, Hammer, Rustle, Saw, Bake, Drop, Click, Place, Reject, Complete, Bird, Wind, River }

    // Original procedural PCM: short, softly enveloped sounds; no downloaded audio assets.
    private static AudioStreamWav Synthesize(Cue cue, int variant=0)
    {
        const int rate = 22050;
        bool loop=cue is Cue.Wind or Cue.River;
        float duration = cue switch { Cue.Wind or Cue.River => 12, Cue.Bird => 1.4f, Cue.Complete => 1.15f,
            Cue.Saw => 0.55f, Cue.Bake => 0.7f, Cue.Rustle => 0.42f, Cue.Place=>.35f, _ => 0.28f };
        int count = (int)(rate * duration); var samples = new float[count];
        var random = new Random(417 + (int)cue*31+variant*991); float low = 0, slow = 0, body=0;
        for (int i = 0; i < count; i++)
        {
            float t = i / (float)rate, p = t / duration;
            float noise = (float)random.NextDouble() * 2 - 1;
            low += (noise - low) * 0.13f; slow += (noise - slow) * 0.018f;
            body+=(slow-body)*.006f;
            float Tone(float hz) => MathF.Sin(MathF.Tau * hz*(1+variant*.018f) * t);
            float Knock(float start,float pitch,float strength)
            {
                float age=t-start;if(age<0)return 0;
                return strength*(MathF.Sin(MathF.Tau*pitch*age)*MathF.Exp(-age*35)+
                    .43f*MathF.Sin(MathF.Tau*pitch*2.71f*age)*MathF.Exp(-age*62)+
                    noise*.55f*MathF.Exp(-age*180));
            }
            float decay = MathF.Exp(-t * 22);
            float value = cue switch
            {
                Cue.Step => Knock(.008f,95+variant*6,.09f)+low*.30f*MathF.Exp(-MathF.Pow((t-.055f)/.045f,2)),
                Cue.Chop => Knock(.006f,165+variant*9,.32f)+Knock(.043f,280,.07f)+low*.27f*MathF.Exp(-t*24),
                Cue.Hammer => Knock(.005f,370+variant*17,.23f)+Knock(.065f,540,.035f),
                Cue.Rustle => (noise-low)*.10f*MathF.Pow(MathF.Sin(MathF.PI*p),2)*(1+.4f*MathF.Sin(t*61+variant)),
                Cue.Saw => (noise-low)*.095f*MathF.Sin(MathF.PI*p)*(0.55f+.45f*MathF.Sin(t*95)),
                Cue.Bake => slow*.20f+(random.NextDouble()<.0015?noise*.10f:0),
                Cue.Drop => Knock(.006f,115+variant*8,.20f)+Knock(.075f,185,.06f)+low*.18f*decay,
                Cue.Click => Knock(.003f,610,.085f),
                Cue.Place => Knock(.005f,260,.12f)+Knock(.085f,390,.08f),
                Cue.Reject => (Tone(155)+.25f*Tone(232))* .08f*MathF.Exp(-t*20),
                Cue.Complete => CompletionTone(t),
                Cue.Bird => BirdTone(t,variant),
                Cue.Wind => (slow*.50f+body*1.3f)*(.7f+.18f*MathF.Sin(t*.73f)+.12f*MathF.Sin(t*1.31f)),
                Cue.River => (low*.20f+slow*.8f)*(.7f+.2f*MathF.Sin(t*2.4f)+.1f*MathF.Sin(t*5.7f))+
                    Tone(640+170*MathF.Sin(t*.8f))* .009f*MathF.Pow(Math.Max(0,MathF.Sin(t*3.7f)),10),
                _ => 0
            };
            float envelope = loop ? 1 : Math.Min(1, t / 0.005f) * Math.Min(1, (duration - t) / 0.025f);
            samples[i] = value * envelope;
        }
        if (loop)
        {
            // Crossfade the end into the beginning to avoid a discontinuity at the loop seam.
            int fade = rate / 2;
            for (int i = 0; i < fade; i++)
            {
                float blend = i / (float)(fade - 1);
                samples[count - fade + i] = samples[count - fade + i] * (1 - blend) + samples[i] * blend;
            }
            var looped = new float[count - fade]; Array.Copy(samples, fade, looped, 0, looped.Length); samples = looped;
            // Match the final sample as well as the overlapping texture. No isolated
            // broadband jump when the river's noisier layer wraps around.
            int seam=rate/200;float correction=samples[0]-samples[^1];
            for(int i=0;i<seam;i++)
            {float blend=i/(float)(seam-1);samples[samples.Length-seam+i]+=correction*blend*blend*(3-2*blend);}
        }
        var data = new byte[samples.Length * 2];
        for (int i = 0; i < samples.Length; i++)
        {
            short pcm = (short)(Math.Clamp(samples[i], -0.9f, 0.9f) * short.MaxValue);
            data[i * 2] = (byte)pcm; data[i * 2 + 1] = (byte)(pcm >> 8);
        }
        return new AudioStreamWav { Format = AudioStreamWav.FormatEnum.Format16Bits, MixRate = rate, Stereo = false, Data = data,
            LoopMode = loop ? AudioStreamWav.LoopModeEnum.Forward : AudioStreamWav.LoopModeEnum.Disabled,
            LoopBegin = 0, LoopEnd = samples.Length };
    }

    private static float CompletionTone(float t)
    {
        float value=0;
        for(int n=0;n<3;n++)
        {
            float age=t-n*.12f;if(age<0)continue;
            float hz=n==0?293.665f:n==1?369.994f:440;
            value+=(MathF.Sin(MathF.Tau*hz*age)+.18f*MathF.Sin(MathF.Tau*hz*2*age))*
                Math.Min(1,age/.008f)*MathF.Exp(-age*6)*.10f;
        }
        return value;
    }

    private static float BirdTone(float t,int variant)
    {
        float value=0;
        for(int chirp=0;chirp<2;chirp++)
        {
            float age=t-.12f-chirp*.42f;if(age<0 || age>.27f)continue;
            float hz=2250+variant*130-chirp*240;
            float phase=MathF.Tau*(hz*age-380*age*age);
            value+=MathF.Sin(phase)*MathF.Pow(MathF.Sin(MathF.PI*age/.27f),2)*.065f;
        }
        return value;
    }
}
