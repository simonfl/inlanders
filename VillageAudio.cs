using Godot;
using Inlanders.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Game
{
    private const string EffectsBus = "Village effects", AmbienceBus = "Village ambience";
    private readonly Dictionary<Cue, AudioStreamWav> _sounds = new();
    private readonly List<AudioStreamPlayer3D> _voices = new();
    private AudioStreamPlayer _uiSound = null!, _wind = null!;
    private AudioStreamPlayer3D _bird = null!;
    private sealed class SoundTrace { public System.Numerics.Vector2 Position; public float Distance, Next; public int Cargo; }
    private readonly Dictionary<int, SoundTrace> _soundTraces = new();
    private readonly HashSet<int> _heardBuildings = new();
    private readonly Dictionary<Cue, float> _cueCooldown = new();
    private float _soundTime, _nextBird = 4;
    private bool _heardSupper, _soundMuted;
    private float _effectsVolume = 65, _ambienceVolume = 40;
    private string _audioSettingsPath = "saves/audio.cfg";
    private int _worldSoundCount;
    private bool _audioSettingsDirty;
    private float _audioSettingsWriteAt;

    private void MakeAudio()
    {
        if (OS.GetCmdlineUserArgs().Any(a => a.EndsWith("smoke-test"))) _audioSettingsPath = "artifacts/f10-audio.cfg";
        foreach (var name in new[] { EffectsBus, AmbienceBus, MusicBus })
        {
            if (AudioServer.GetBusIndex(name) >= 0) continue;
            AudioServer.AddBus(); AudioServer.SetBusName(AudioServer.BusCount - 1, name);
        }
        foreach (var cue in Enum.GetValues<Cue>()) _sounds[cue] = Synthesize(cue);
        for (int i = 0; i < 6; i++)
        {
            var voice = new AudioStreamPlayer3D { Bus = EffectsBus, UnitSize = 25, MaxDistance = 80, VolumeDb = -8, MaxPolyphony = 1 };
            AddChild(voice); _voices.Add(voice);
        }
        _uiSound = new AudioStreamPlayer { Bus = EffectsBus, VolumeDb = -8 }; AddChild(_uiSound);
        _wind = new AudioStreamPlayer { Bus = AmbienceBus, Stream = _sounds[Cue.Wind], VolumeDb = -6 }; AddChild(_wind);
        _bird = new AudioStreamPlayer3D { Bus = AmbienceBus, Stream = _sounds[Cue.Bird], UnitSize = 25, MaxDistance = 90, VolumeDb = -12 };
        AddChild(_bird); MakeMusic(); ReadAudioSettings(); ApplyAudioSettings(); _wind.Play();
    }

    private void ResetWorldAudio()
    {
        foreach (var voice in _voices) voice.Stop();
        _soundTraces.Clear(); _heardBuildings.Clear(); _cueCooldown.Clear();
        foreach (var v in _world.People) _soundTraces[v.Id] = new SoundTrace { Position = v.Position, Cargo = v.Carried, Next = _soundTime + v.Id * 0.07f };
        foreach (var c in _world.Cottages.Where(c => c.Complete)) _heardBuildings.Add(c.Id);
        _heardSupper = _world.Food.SupperComplete;
    }
    private void UiCue(Cue cue)
    {
        if (_uiSound == null || _soundMuted || _effectsVolume == 0) return;
        _uiSound.Stream = _sounds[cue]; _uiSound.Play();
    }
    private void WorldCue(Cue cue, Vector3 position, int variation = 0)
    {
        if (_paused || _soundMuted || _effectsVolume == 0 || _cueCooldown.GetValueOrDefault(cue) > _soundTime) return;
        var voice = _voices.FirstOrDefault(p => !p.Playing);
        if (voice == null) return;
        voice.Position = position; voice.Stream = _sounds[cue]; voice.PitchScale = 0.96f + variation % 5 * 0.02f;
        voice.Play(); _worldSoundCount++; _cueCooldown[cue] = _soundTime + 0.12f;
    }
    private void UpdateAudio(float dt)
    {
        _soundTime += dt;
        if (_audioSettingsDirty && _soundTime >= _audioSettingsWriteAt) SaveAudioSettings();
        if (_soundTime >= _nextBird)
        {
            _nextBird = _soundTime + 9 + (_nextBird % 5);
            if (!_soundMuted && _ambienceVolume > 0)
            { _bird.Position = new(MathF.Sin(_soundTime) * 8, 2, -7); _bird.PitchScale = 0.95f + 0.1f * MathF.Sin(_soundTime); _bird.Play(); }
        }
        if (_paused) { foreach (var voice in _voices) voice.Stop(); return; }
        foreach (var v in _world.People)
        {
            if (!_soundTraces.TryGetValue(v.Id, out var trace))
                _soundTraces[v.Id] = trace = new SoundTrace { Position = v.Position, Cargo = v.Carried, Next = _soundTime + 0.1f };
            trace.Distance += System.Numerics.Vector2.Distance(trace.Position, v.Position); trace.Position = v.Position;
            if (trace.Cargo > v.Carried) WorldCue(Cue.Drop, new(v.Position.X, 0.5f, v.Position.Y), v.Id);
            trace.Cargo = v.Carried;
            if (_soundTime < trace.Next) continue;
            Cue? cue = v.Route.Count > 0 ? trace.Distance >= 0.65f ? Cue.Step : null : v.Task switch
            {
                Work.Chopping => _world.Trees.Any(t => t.Id == v.TreeId && !t.Felled) ? Cue.Chop : Cue.Drop, Work.Building => Cue.Hammer,
                Work.ClearingStump or Work.Planting or Work.PlantingTree or Work.Harvesting or Work.Foraging => Cue.Rustle,
                Work.Sawing => Cue.Saw, Work.Baking => Cue.Bake, _ => null
            };
            if (cue == null) continue;
            WorldCue(cue.Value, new(v.Position.X, 0.5f, v.Position.Y), v.Id);
            trace.Distance = 0;
            // Use real-time limits so fast-forward does not become a wall of sound.
            trace.Next = _soundTime + (cue == Cue.Step ? 0.32f : cue == Cue.Hammer ? 0.48f : 0.75f);
        }
        foreach (var site in _world.Cottages.Where(c => c.Complete))
            if (_heardBuildings.Add(site.Id)) WorldCue(Cue.Complete, new(site.Cell.X, 1, site.Cell.Z));
        if (_world.Food.SupperComplete && !_heardSupper) { _heardSupper = true; UiCue(Cue.Complete); }
    }

    private void ReadAudioSettings()
    {
        var config = new ConfigFile();
        if (config.Load(_audioSettingsPath) != Error.Ok) return;
        double effects = config.GetValue("audio", "effects", 65.0).AsDouble();
        double ambience = config.GetValue("audio", "ambience", 40.0).AsDouble();
        _effectsVolume = double.IsFinite(effects) ? (float)Math.Clamp(effects, 0, 100) : 65;
        _ambienceVolume = double.IsFinite(ambience) ? (float)Math.Clamp(ambience, 0, 100) : 40;
        double music = config.GetValue("audio", "music", 35.0).AsDouble();
        _musicVolume = double.IsFinite(music) ? (float)Math.Clamp(music, 0, 100) : 35;
        _musicMuted = config.GetValue("audio", "musicMuted", false).AsBool();
        _soundMuted = config.GetValue("audio", "muted", false).AsBool();
    }
    private void ApplyAudioSettings()
    {
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex(MusicBus), Mathf.LinearToDb(Math.Max(.0001f, _musicVolume / 100)));
        AudioServer.SetBusMute(AudioServer.GetBusIndex(MusicBus), _soundMuted || _musicMuted || _musicVolume == 0);
        if (_muteMusicButton != null) _muteMusicButton.Text = _musicMuted ? "Unmute music" : "Mute music";
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex(EffectsBus), Mathf.LinearToDb(Math.Max(0.0001f, _effectsVolume / 100)));
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex(AmbienceBus), Mathf.LinearToDb(Math.Max(0.0001f, _ambienceVolume / 100)));
        AudioServer.SetBusMute(AudioServer.GetBusIndex(EffectsBus), _soundMuted || _effectsVolume == 0);
        AudioServer.SetBusMute(AudioServer.GetBusIndex(AmbienceBus), _soundMuted || _ambienceVolume == 0);
        if (_soundMuted || _effectsVolume == 0) { foreach (var voice in _voices) voice.Stop(); _uiSound.Stop(); }
        if (_soundMuted || _ambienceVolume == 0) _bird.Stop();
        if (_muteSoundButton != null) _muteSoundButton.Text = _soundMuted ? "Sound: muted [M]" : "Mute sound [M]";
    }
    private void SaveAudioSettings()
    {
        _audioSettingsDirty = false;
        var config = new ConfigFile(); config.SetValue("audio", "effects", _effectsVolume);
        config.SetValue("audio", "music", _musicVolume); config.SetValue("audio", "musicMuted", _musicMuted);
        config.SetValue("audio", "ambience", _ambienceVolume); config.SetValue("audio", "muted", _soundMuted);
        try
        {
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(_audioSettingsPath)!);
            if (config.Save(_audioSettingsPath) != Error.Ok) Notice("Audio settings changed, but could not be saved.");
        }
        catch (System.IO.IOException) { Notice("Audio settings changed, but could not be saved."); }
        catch (UnauthorizedAccessException) { Notice("Audio settings changed, but could not be saved."); }
    }
    private void ToggleSoundMute() { _soundMuted = !_soundMuted; ApplyAudioSettings(); SaveAudioSettings(); }
    private void AudioVolumeChanged()
    {
        ApplyAudioSettings(); _audioSettingsDirty = true; _audioSettingsWriteAt = _soundTime + 0.5f;
    }
    public override void _ExitTree() { if (_audioSettingsDirty) SaveAudioSettings(); }
}
