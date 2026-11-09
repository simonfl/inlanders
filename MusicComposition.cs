using Godot;
using System;
using System.Collections.Generic;

public partial class Game
{
    // Original modal miniatures, inspired by an acoustic folk ensemble rather than
    // a claim of historical reconstruction. Note caches keep synthesis off the frame loop.
    private static AudioStreamWav ComposeMusic(int theme)
    {
        const int rate=22050;
        const float beat=.94f, duration=96;
        int frames=(int)(rate*duration);
        var left=new float[frames]; var right=new float[frames];
        var notes=new Dictionary<(int,bool),float[]>();
        var random=new Random(830+theme);
        float[] Voice(int midi,bool flute)
        {
            if(notes.TryGetValue((midi,flute),out var cached))return cached;
            float length=flute?3.1f:4.5f;
            var tone=new float[(int)(length*rate)];
            float hz=440*MathF.Pow(2,(midi-69)/12f), breath=0;
            var noise=new Random(midi*47+(flute?13:0));
            for(int i=0;i<tone.Length;i++)
            {
                float t=i/(float)rate;
                breath+=((float)noise.NextDouble()*2-1-breath)*.24f;
                if(flute)
                {
                    // Gentle breath and delayed, shallow vibrato; leave space between phrases.
                    float phase=MathF.Tau*hz*t+.055f*MathF.Sin(MathF.Tau*4.7f*t)*Math.Min(1,t/.5f);
                    tone[i]=(MathF.Sin(phase)+.19f*MathF.Sin(phase*2)+.065f*MathF.Sin(phase*3)+breath*.08f)*.65f;
                }
                else
                {
                    // Pluck position shapes partials; upper partials decay first, leaving a warm body.
                    float value=0;
                    for(int h=1;h<=12;h++)
                        value+=MathF.Sin(MathF.PI*h*.23f)/MathF.Pow(h,1.45f)*
                            MathF.Sin(MathF.Tau*hz*h*t)*MathF.Exp(-t*(1.1f+h*.24f));
                    tone[i]=value*.8f+breath*.09f*MathF.Exp(-t*90);
                }
            }
            notes[(midi,flute)]=tone;return tone;
        }
        void Note(float start,int midi,float length,float gain,float pan,bool flute=false)
        {
            if(midi==0)return;
            var tone=Voice(midi,flute);
            int first=Math.Max(0,(int)((start+random.NextDouble()*.022)*rate));
            int count=Math.Min(Math.Min((int)(length*rate),tone.Length),frames-first);
            float l=MathF.Sqrt((1-pan)/2),r=MathF.Sqrt((1+pan)/2);
            gain*=.91f+(float)random.NextDouble()*.16f;
            for(int i=0;i<count;i++)
            {
                float t=i/(float)rate;
                float envelope=flute?Math.Min(1,t/.11f)*Math.Min(1,(length-t)/.24f):Math.Min(1,t/.004f)*Math.Min(1,(length-t)/.18f);
                float value=tone[i]*gain*envelope;
                left[first+i]+=value*l;right[first+i]+=value*r;
            }
        }
        int[][][] harmony={
            new[]{new[]{50,54,57},new[]{43,47,50},new[]{47,50,54},new[]{45,49,52}},
            new[]{new[]{50,53,57},new[]{48,52,55},new[]{43,47,50},new[]{50,53,57}},
            new[]{new[]{43,47,50},new[]{48,52,55},new[]{52,55,59},new[]{50,54,57}}
        };
        int[][][] phrases={
            new[]{new[]{74,78,76,74,71,69,71,74},new[]{78,81,78,76,74,71,69,74}},
            new[]{new[]{69,72,74,77,76,74,72,69},new[]{74,77,79,77,76,74,72,74}},
            new[]{new[]{71,74,76,74,71,69,67,71},new[]{74,79,78,76,74,71,69,67}}
        };
        int[] progression={0,0,1,1,2,2,3,0};
        for(int bar=0;bar<32;bar++)
        {
            int section=bar/8, within=bar%8;
            var chord=harmony[theme][progression[within]];
            float start=bar*3*beat;
            bool coda=bar>=28;
            Note(start,chord[0]-12,3.5f,.10f,-.1f);
            // Broken chords, with quieter phrase endings and a sparse final cadence.
            for(int n=0;n<(coda?2:3);n++)
                Note(start+n*beat+.04f,chord[(n+section)%3]+12,3,.085f*(within==7?.65f:1),-.32f);
            var phrase=phrases[theme][section==2?1:0];
            if(within is 3 or 7 || bar>=30)continue;
            bool flute=section is 1 or 2;
            int pitch=phrase[within];
            Note(start+.3f*beat,pitch,flute?1.6f:2.8f,flute?.067f:.13f,.24f,flute);
            if(within%2==0)
                Note(start+1.8f*beat,phrase[(within+1)%8],flute?.85f:2.3f,flute?.05f:.095f,.24f,flute);
        }
        // Short, quiet room reflections, no wash of sustained synthesizer chords.
        foreach(var (delay,gain) in new[]{(.047f,.13f),(.083f,.09f),(.139f,.055f)})
        {
            int offset=(int)(delay*rate);
            for(int i=frames-1;i>=offset;i--)
            {float l=left[i-offset],r=right[i-offset];left[i]+=r*gain;right[i]+=l*gain;}
        }
        var data=new byte[frames*4];
        for(int i=0;i<frames;i++)
        {
            float t=i/(float)rate,fade=Math.Min(1,t/1.5f)*Math.Min(1,(duration-t)/3);
            for(int c=0;c<2;c++)
            {
                short value=(short)(Math.Clamp((c==0?left[i]:right[i])*fade,-.8f,.8f)*32767);
                data[i*4+c*2]=(byte)value;data[i*4+c*2+1]=(byte)(value>>8);
            }
        }
        return new AudioStreamWav{Format=AudioStreamWav.FormatEnum.Format16Bits,MixRate=rate,Stereo=true,Data=data,LoopMode=AudioStreamWav.LoopModeEnum.Disabled};
    }
}
