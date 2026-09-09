using Godot;
using System;

public partial class Game
{
    private const string MusicBus = "Village music";
    private AudioStreamPlayer _music = null!;
    private float _musicVolume = 35;
    private bool _musicMuted;
    private HSlider _musicSlider = null!;
    private Button _muteMusicButton = null!;

    private void MakeMusic()
    {
        _music=new AudioStreamPlayer { Bus=MusicBus, Stream=ComposeMusic(), VolumeDb=-6 };
        AddChild(_music); _music.Play();
    }
    private void ToggleMusicMute() { _musicMuted=!_musicMuted; ApplyAudioSettings(); SaveAudioSettings(); }
    private void MakeMusicUi(VBoxContainer column)
    {
        var row=new HBoxContainer(); column.AddChild(row); row.AddChild(Text("Music",14));
        _musicSlider=new HSlider { MinValue=0,MaxValue=100,Step=5,Value=_musicVolume,CustomMinimumSize=new(90,30),
            SizeFlagsHorizontal=Control.SizeFlags.ExpandFill,FocusMode=Control.FocusModeEnum.None,
            TooltipText="Quiet original music; continues through pause and menus." };
        row.AddChild(_musicSlider);
        _musicSlider.ValueChanged+=value=> { _musicVolume=(float)value; AudioVolumeChanged(); };
        _musicSlider.DragEnded+=changed=> { if(changed) SaveAudioSettings(); };
        _muteMusicButton=Button("",ToggleMusicMute); column.AddChild(_muteMusicButton);
    }

    // An original 32-bar miniature in C: two related phrases over a slow four-chord cycle.
    // Soft plucks and sustained triads are rendered once; transport is independent of simulation time.
    private static AudioStreamWav ComposeMusic()
    {
        const int rate=22050;
        const float beat=.75f, duration=96;
        var samples=new float[(int)(rate*duration)];
        void Note(float start,int midi,float length,float gain,bool pad=false)
        {
            float hz=440*MathF.Pow(2,(midi-69)/12f);
            int first=(int)(start*rate), count=Math.Min((int)(length*rate),samples.Length-first);
            for(int i=0;i<count;i++)
            {
                float t=i/(float)rate, p=t/length;
                float envelope=pad?MathF.Pow(MathF.Sin(MathF.PI*p),2):Math.Min(1,t/.012f)*MathF.Exp(-t*2.5f)*Math.Min(1,(length-t)/.25f);
                float phase=MathF.Tau*hz*t;
                float tone=MathF.Sin(phase)+(pad?.08f:.22f)*MathF.Sin(phase*2)+ (pad?0:.07f)*MathF.Sin(phase*3);
                samples[first+i]+=tone*envelope*gain;
            }
        }
        int[][] chords={new[]{48,52,55},new[]{45,48,52},new[]{41,45,48},new[]{43,47,50}};
        int[][] melody={new[]{72,76,79,76},new[]{72,69,76,72},new[]{69,72,77,76},new[]{74,71,67,71}};
        for(int bar=0;bar<32;bar++)
        {
            int harmony=bar/2%4; float start=bar*4*beat;
            if(bar%2==0) foreach(int note in chords[harmony]) Note(start,note,6,.024f,true);
            if(bar%4==3) continue; // Leave room for wind, birds, and the village.
            for(int n=0;n<3;n++)
            {
                int index=(n+(bar>=16?1:0))%4;
                Note(start+(n+.25f)*beat,melody[harmony][index],2.5f,.10f*(n==0?1:.72f));
            }
        }
        var data=new byte[samples.Length*2];
        for(int i=0;i<samples.Length;i++)
        {
            float t=i/(float)rate, fade=Math.Min(1,t/2)*Math.Min(1,(duration-t)/2);
            short value=(short)(Math.Clamp(samples[i]*fade,-.8f,.8f)*short.MaxValue);
            data[i*2]=(byte)value; data[i*2+1]=(byte)(value>>8);
        }
        return new AudioStreamWav { Format=AudioStreamWav.FormatEnum.Format16Bits,MixRate=rate,Data=data,
            LoopMode=AudioStreamWav.LoopModeEnum.Forward,LoopBegin=0,LoopEnd=samples.Length };
    }
}
