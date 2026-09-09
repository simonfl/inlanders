using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckMusic()
    {
        void Check(bool ok,string message) { if(!ok) throw new Exception(message); }
        async Task Wait(float seconds) => await ToSignal(GetTree().CreateTimer(seconds),SceneTreeTimer.SignalName.Timeout);
        var stream=(AudioStreamWav)_music.Stream;
        var data=stream.Data; int peak=0;
        for(int i=0;i<data.Length;i+=2) peak=Math.Max(peak,Math.Abs((int)BitConverter.ToInt16(data,i)));
        Check(peak>1000 && peak<26000 && stream.GetLength()==96,"Music length/PCM bounds incorrect");
        Check(Math.Abs(BitConverter.ToInt16(data,0)-BitConverter.ToInt16(data,data.Length-2))<10,"Music loop clicks");
        Check(stream.SaveToWav("artifacts/f17-music.wav")==Error.Ok,"Music export failed");
        _paused=true; _soundMuted=_musicMuted=false;
        _effectsVolume=_ambienceVolume=0; _musicSlider.Value=65; _musicVolume=65; ApplyAudioSettings();
        string saved=_world.SaveJson();
        int index=AudioServer.GetBusEffectCount(0);
        var capture=new AudioEffectCapture { BufferLength=2 }; AudioServer.AddBusEffect(0,capture);
        float Peak() { var frames=capture.GetBuffer(capture.GetFramesAvailable()); return frames.Length==0?0:frames.Max(f=>Math.Max(Math.Abs(f.X),Math.Abs(f.Y))); }
        try
        {
            _music.Seek(6); await Wait(.3f); capture.ClearBuffer(); await Wait(.7f);
            Check(Peak()>.0001f,"Music did not reach mixer while paused");
            float position=_music.GetPlaybackPosition();
            await UiClick(_muteMusicButton); await Wait(.2f); capture.ClearBuffer(); await Wait(.3f);
            Check(_musicMuted && Peak()<.00001f,"Music mute failed");
            Check(_music.GetPlaybackPosition()>position,"Mute stopped transport");
            await UiClick(_muteMusicButton);
            _musicSlider.Value=25; await Wait(.7f);
            _musicVolume=99; ReadAudioSettings(); ApplyAudioSettings();
            Check(_musicVolume==25 && !_musicMuted,"Music settings did not persist");
            _musicSlider.Value=0; await Wait(.2f); capture.ClearBuffer(); await Wait(.3f);
            Check(Peak()<.00001f,"Zero music volume did not silence");
            _musicSlider.Value=35; _soundMuted=true; ApplyAudioSettings();
            await Wait(.2f); capture.ClearBuffer(); await Wait(.3f);
            Check(Peak()<.00001f,"Master mute left music audible");
            _soundMuted=false; ApplyAudioSettings();
            _music.Seek(95.8f); await Wait(.6f);
            Check(_music.Playing && _music.GetPlaybackPosition()<2,"Music did not loop");
            Check(saved==_world.SaveJson(),"Music changed simulation");
            _drawerPages[3].EnsureControlVisible(_musicSlider); await Wait(.1f);
            await Capture("artifacts/f17-music-controls.png");
            GD.Print("PASS: 96-second original music, PCM/loop, isolated live mix, independent mute/volume, master mute, persistence and pause independence.");
        }
        finally { AudioServer.RemoveBusEffect(0,index); _effectsVolume=65; _ambienceVolume=40; ApplyAudioSettings(); SaveAudioSettings(); }
    }
}
