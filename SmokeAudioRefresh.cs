using Godot;
using Inlanders.Simulation;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckAudioRefresh()
    {
        void Check(bool ok,string message){if(!ok)throw new Exception(message);}
        async Task Wait(float seconds)=>await ToSignal(GetTree().CreateTimer(seconds),SceneTreeTimer.SignalName.Timeout);
        const string folder="artifacts/audio-refresh/after";Directory.CreateDirectory(folder);
        foreach(var (cue,variants) in _soundVariants)
        {
            Check(variants.Select(s=>Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(s.Data))).Distinct().Count()==4,"Duplicate sound variants: "+cue);
            for(int i=0;i<variants.Length;i++)Check(variants[i].SaveToWav($"{folder}/{cue.ToString().ToLowerInvariant()}-{i}.wav")==Error.Ok,"Variant export failed");
        }
        for(int i=0;i<_musicThemes.Length;i++)Check(_musicThemes[i].Stereo && _musicThemes[i].SaveToWav($"{folder}/theme-{i}.wav")==Error.Ok,"Stereo theme export failed");
        foreach(var cue in new[]{Cue.River,Cue.Wind,Cue.Complete,Cue.Place})_sounds[cue].SaveToWav($"{folder}/{cue.ToString().ToLowerInvariant()}.wav");
        var capture=new AudioEffectCapture{BufferLength=16};int index=AudioServer.GetBusEffectCount(0);
        AudioServer.AddBusEffect(0,capture);SetProcess(false);
        var reports=new List<object>();
        try
        {
            _paused=false;_soundMuted=_musicMuted=false;_effectsVolume=65;_ambienceVolume=40;_musicVolume=35;ApplyAudioSettings();
            AdoptWorld(World.NewRiverClosing());_focus=Vector3.Zero;ResetWorldAudio();
            for(int i=0;i<40;i++){_soundTime+=.1f;UpdateShoreAmbience(.1f);}
            Check(_riverGain>.3f,"River is absent near actual water");
            _focus=new(200,0,200);
            float near=_riverGain;_soundTime+=.3f;UpdateShoreAmbience(.1f);
            Check(_riverGain>0 && _riverGain<near,"Camera jump did not fade water smoothly");
            for(int i=0;i<60;i++){_soundTime+=.1f;UpdateShoreAmbience(.1f);}
            Check(_riverGain<.001f,"Water stays audible inland");
            AdoptWorld(World.NewScenario());_focus=Vector3.Zero;
            for(int i=0;i<40;i++){_soundTime+=.1f;UpdateShoreAmbience(.1f);}
            Check(_riverTarget==0,"Dry map has river ambience");
            // A completion still reaches its reserved channel when all work voices are busy.
            _paused=false;_cueCooldown.Clear();
            foreach(var voice in _voices.Take(5)){voice.Stream=_sounds[Cue.Saw];voice.Play();}
            WorldCue(Cue.Complete,Vector3.Zero);
            Check(_voices[^1].Playing && _voices[^1].Stream==_sounds[Cue.Complete],"Work swallowed completion cue");
            ResetWorldAudio();
            var opening=World.NewRiverClosing();
            opening.Place(World.RecoveryCrossing,0,BuildingKind.Bridge);
            opening.ApplyWoodlandCare(new(-11,5),new(6,12),WoodlandIntent.Renew,false);
            string grownPath="artifacts/river-closing/True-False-end.json";
            // Use the measured long-level village when available; report the fallback honestly.
            var settled=File.Exists(grownPath)?World.LoadFile(grownPath):World.NewScenario();
            foreach(var scene in new[]{(name:"recovering",world:opening),(name:File.Exists(grownPath)?"settled":"baseline",world:settled)})
            foreach(int speed in new[]{1,6})
            {
                string saved=scene.world.SaveJson();AdoptWorld(World.LoadJson(saved));var control=World.LoadJson(saved);
                _focus=new(-1,0,-3);_camera.Size=26;UpdateCamera();ResetWorldAudio();_paused=false;
                _musicRest.Stop();_musicThemeIndex=0;_music.Stream=_musicThemes[0];_music.Play(24);_nextBird=_soundTime+2;
                await Wait(.3f);capture.ClearBuffer();int sounds=_worldSoundCount;
                for(int tick=0;tick<100;tick++)
                {
                    for(int step=0;step<speed;step++){_world.Tick(.1f);control.Tick(.1f);}
                    RenderActors(.1f);RenderFoodViews();UpdateAudio(.1f);await Wait(.1f);
                }
                Check(control.SaveJson()==_world.SaveJson(),"Audio changed world progression");
                var frames=capture.GetBuffer(capture.GetFramesAvailable());
                float peak=frames.Max(f=>Math.Max(Math.Abs(f.X),Math.Abs(f.Y)));
                double rms=Math.Sqrt(frames.Average(f=>((double)f.X*f.X+(double)f.Y*f.Y)/2));
                Check(peak>.0001f && peak<.95f,"Empty/clipped village mix");
                Check(_voices.Count==6 && _worldSoundCount-sounds<130,"Fast-forward sound density exceeded bound");
                var bytes=new byte[frames.Length*4];
                for(int i=0;i<frames.Length;i++)for(int c=0;c<2;c++)
                {short value=(short)((c==0?frames[i].X:frames[i].Y)*32767);bytes[i*4+c*2]=(byte)value;bytes[i*4+c*2+1]=(byte)(value>>8);}
                using var stream=new AudioStreamWav{Format=AudioStreamWav.FormatEnum.Format16Bits,Stereo=true,MixRate=(int)AudioServer.GetMixRate(),Data=bytes};
                Check(stream.SaveToWav($"{folder}/{scene.name}-{speed}x.wav")==Error.Ok,"Mix export failed");
                reports.Add(new{scene=scene.name,speed,peak,rms,worldCues=_worldSoundCount-sounds,seconds=frames.Length/AudioServer.GetMixRate()});
                _paused=true;ResetWorldAudio();
            }
            File.WriteAllText(folder+"/mix.json",System.Text.Json.JsonSerializer.Serialize(reports,new System.Text.Json.JsonSerializerOptions{WriteIndented=true}));
            GD.Print("PASS: varied samples, stereo themes, shoreline fade/dry map, reserved completion voice, native 1x/6x mixes, bounded density and unchanged simulation.");
        }
        finally{_paused=true;ResetWorldAudio();AudioServer.RemoveBusEffect(0,index);SetProcess(true);}
    }
}
