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
        _musicRest.Stop();_musicThemeIndex=0;_music.Stream=_musicThemes[0];_music.Play();
        ExportMusicCandidates();
        var stream=(AudioStreamWav)_music.Stream;
        var data=stream.Data; int peak=0;
        for(int i=0;i<data.Length;i+=2) peak=Math.Max(peak,Math.Abs((int)BitConverter.ToInt16(data,i)));
        Check(peak>1000 && peak<26000 && stream.GetLength()==96,"Music length/PCM bounds incorrect");
        Check(stream.LoopMode==AudioStreamWav.LoopModeEnum.Disabled && Math.Abs(BitConverter.ToInt16(data,0))<10 && Math.Abs(BitConverter.ToInt16(data,data.Length-2))<10,"Music phrase boundaries do not fade to silence");
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
            _musicRest.Stop();_musicGapIndex=0;_music.Seek(95.8f); await Wait(.6f);
            Check(!_music.Playing && _musicRest.TimeLeft>16 && _musicRest.TimeLeft<=18,"Phrase did not enter quiet interval");
            capture.ClearBuffer();await Wait(.3f);Check(Peak()<.00001f,"Quiet interval has music output");
            double remaining=_musicRest.TimeLeft;
            var originalStream=_music.Stream;var playerId=_music.GetInstanceId();
            AdoptWorld(Inlanders.Simulation.World.LoadJson(saved));_paused=true;
            _speed=4;await Wait(.5f);
            Check(!_music.Playing && _musicRest.TimeLeft<remaining && _musicRest.TimeLeft>remaining-2,"Load or speed changed quiet interval");
            Check(_music.Stream==originalStream,"Load changed current theme");
            await Wait((float)_musicRest.TimeLeft+.3f);
            Check(_music.Playing && _music.GetPlaybackPosition()<2 && _musicRest.IsStopped(),"Quiet interval did not resume music once");
            Check(_musicThemeIndex==1 && _music.Stream==_musicThemes[1] && _music.GetInstanceId()==playerId,"Sequencing failed to advance with existing player/stream");
            _music.Seek(95.8f);await Wait(.6f);
            Check(!_music.Playing && _musicRest.TimeLeft>24 && _musicRest.TimeLeft<=26,"Second phrase did not select next quiet interval");
            _musicRest.Stop();PlayNextMusicTheme();Check(_musicThemeIndex==2 && _music.Stream==_musicThemes[2],"Third theme not selected");
            PlayNextMusicTheme();Check(_musicThemeIndex==0 && _music.Stream==_musicThemes[0],"Theme cycle does not wrap");
            _musicRest.Stop();_music.Play();_speed=1;
            Check(saved==_world.SaveJson(),"Music changed simulation");
            _drawerPages[3].EnsureControlVisible(_musicSlider); await Wait(.1f);
            await Capture("artifacts/f17-music-controls.png");
            GD.Print("PASS: 96-second original music, faded phrase boundaries, actual timed silence/resume, bounded player/stream, load/speed independence, live mute/volume and settings.");
        }
        finally { AudioServer.RemoveBusEffect(0,index); _effectsVolume=65; _ambienceVolume=40; ApplyAudioSettings(); SaveAudioSettings(); }
    }
}
