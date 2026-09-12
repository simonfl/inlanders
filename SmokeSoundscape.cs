using Godot;
using Inlanders.Simulation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async Task CheckSoundscape()
    {
        void Check(bool ok,string message){if(!ok)throw new Exception(message);}
        async Task Wait(float seconds)=>await ToSignal(GetTree().CreateTimer(seconds),SceneTreeTimer.SignalName.Timeout);
        string folder="artifacts/soundscape";Directory.CreateDirectory(folder);
        var capture=new AudioEffectCapture{BufferLength=16};int index=AudioServer.GetBusEffectCount(0);
        AudioServer.AddBusEffect(0,capture);SetProcess(false);
        float Peak(Vector2[] frames)=>frames.Length==0?0:frames.Max(f=>Math.Max(Math.Abs(f.X),Math.Abs(f.Y)));
        void Export(string name,Vector2[] frames)
        {
            Check(frames.Length>0 && Peak(frames)<.95f,"Empty/clipped mix: "+name);
            var bytes=new byte[frames.Length*4];
            for(int i=0;i<frames.Length;i++)for(int c=0;c<2;c++)
            {short pcm=(short)(Math.Clamp(c==0?frames[i].X:frames[i].Y,-1,1)*32767);bytes[i*4+c*2]=(byte)pcm;bytes[i*4+c*2+1]=(byte)(pcm>>8);}
            Check(new AudioStreamWav{Format=AudioStreamWav.FormatEnum.Format16Bits,Stereo=true,MixRate=(int)AudioServer.GetMixRate(),Data=bytes}.SaveToWav(folder+"/"+name+".wav")==Error.Ok,"Export failed");
        }
        var results=new List<object>();
        try
        {
            AdoptWorld(World.NewQuarryMap());_focus=Vector3.Zero;_camera.Size=12;UpdateCamera();
            _soundMuted=_musicMuted=false;_effectsVolume=65;_musicVolume=_ambienceVolume=0;ApplyAudioSettings();
            async Task<float> Impulse(bool legacy,float zoom,Vector3 source)
            {
                _camera.Size=zoom;UpdateCamera();
                if(legacy)_villageListener.ClearCurrent();else _villageListener.MakeCurrent();
                ResetWorldAudio();await Wait(.3f);capture.ClearBuffer();_paused=false;
                WorldCue(Cue.Hammer,source);_paused=true;await Wait(.35f);
                var frames=capture.GetBuffer(capture.GetFramesAvailable());return Peak(frames);
            }
            float before=await Impulse(true,12,Vector3.Zero),close=await Impulse(false,12,Vector3.Zero),wide=await Impulse(false,40,Vector3.Zero);
            Check(close>before*1.5f && close>wide*1.3f,"Listener does not restore close sound or respond to zoom");
            var original=_camera.GlobalPosition;_camera.GlobalPosition=_focus+(original-_focus)*3;UpdateAudioListener();
            Check(Math.Abs(_villageListener.GlobalPosition.DistanceTo(_focus)-32)<.01,"Listener follows map clipping distance");
            UpdateCamera();
            _soundMuted=true;ApplyAudioSettings();Check(await Impulse(false,12,Vector3.Zero)<.00001f,"Mute left effects audible");
            _soundMuted=false;_effectsVolume=0;ApplyAudioSettings();Check(await Impulse(false,12,Vector3.Zero)<.00001f,"Zero effects left sound");
            _effectsVolume=65;ApplyAudioSettings();_paused=true;int count=_worldSoundCount;
            WorldCue(Cue.Hammer,Vector3.Zero);Check(count==_worldSoundCount,"Paused source emitted");
            GD.Print($"SOUNDSCAPE: focused hammer peak legacy={before:F6}, close={close:F6}, wide={wide:F6}");
            results.Add(new{probe="focused hammer",legacy=before,close,wide});

            var quiet=World.NewCreative();foreach(var p in quiet.People)quiet.Assign(p.Id,Role.Unassigned);
            var busy=World.NewQuarryMap();busy.Place(new(3,-5),0,BuildingKind.Sawmill);busy.Place(new(11,-4),0,BuildingKind.Quarry);
            busy.Assign(6,Role.Quarrier);busy.Assign(7,Role.Sawyer);
            for(int i=0;i<1800;i++)busy.Tick(.1f);
            var water=World.NewCampaign(7);var dock=water.Place(new(14,0),3,BuildingKind.FishingDock)!;
            for(int i=0;i<6000 && !dock.Complete;i++)water.Tick(.1f);
            Check(dock.Complete,"Waterfront fixture construction failed");
            water.Assign(0,Role.Fisher);
            foreach(var scene in new[]{(name:"quiet",world:quiet,focus:Vector3.Zero),(name:"working",world:busy,focus:new Vector3(0,0,-3)),(name:"waterfront",world:water,focus:new Vector3(14,0,0))})
            {
                string saved=scene.world.SaveJson();File.WriteAllText(folder+"/"+scene.name+".json",saved);
                foreach(int zoom in new[]{12,40})foreach(int speed in new[]{1,4})foreach(bool legacy in new[]{true,false})
                {
                    AdoptWorld(World.LoadJson(saved));_paused=false;_focus=scene.focus;_camera.Size=zoom;UpdateCamera();ResetWorldAudio();
                    if(legacy)_villageListener.ClearCurrent();else _villageListener.MakeCurrent();
                    _effectsVolume=65;_ambienceVolume=40;_musicVolume=35;ApplyAudioSettings();
                    _music.Seek(12);_wind.Seek(0);_bird.Stop();_soundTime=0;_nextBird=2;
                    await Wait(.25f);capture.ClearBuffer();count=_worldSoundCount;
                    var copy=World.LoadJson(saved);
                    for(int tick=0;tick<80;tick++)
                    {
                        for(int s=0;s<speed;s++){_world.Tick(.1f);copy.Tick(.1f);}
                        RenderActors(.1f);RenderFoodViews();UpdateAudio(.1f);await Wait(.1f);
                    }
                    Check(copy.SaveJson()==_world.SaveJson(),"Soundscape changes simulation");
                    var frames=capture.GetBuffer(capture.GetFramesAvailable());string name=$"{scene.name}-{zoom}-{speed}x-{(legacy?"before":"after")}";
                    Export(name,frames);results.Add(new{name,peak=Peak(frames),frames=frames.Length,worldCues=_worldSoundCount-count});
                    Check(_voices.Count==6,"Voice pool grew");
                    _paused=true;ResetWorldAudio();int resetCount=_worldSoundCount;
                    _paused=false;UpdateAudio(0);_paused=true;Check(resetCount==_worldSoundCount,"Reload replayed prior one-shot");
                }
                GD.Print("SOUNDSCAPE: captured "+scene.name+" at close/wide, 1x/4x, camera/listener mix.");
            }
            File.WriteAllText(folder+"/results.json",System.Text.Json.JsonSerializer.Serialize(results,new System.Text.Json.JsonSerializerOptions{WriteIndented=true}));
        }
        finally
        {
            _paused=true;_villageListener.MakeCurrent();ResetWorldAudio();AudioServer.RemoveBusEffect(0,index);SetProcess(true);
        }
    }
}
