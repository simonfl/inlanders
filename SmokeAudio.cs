using Godot;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class Game
{
    private async void RunAudioSmoke()
    {
        try { await CheckAudio(); GD.Print("SMOKE PASS: PCM bounds/loop seam, live mixer output, mute silence, UI volume persistence, pause suppression, voice limits, and simulation independence."); GetTree().Quit(); }
        catch (Exception e) { GD.PrintErr("AUDIO SMOKE FAIL: " + e); GetTree().Quit(1); }
    }
    private async Task CheckAudio()
    {
        void Check(bool value, string message) { if (!value) throw new Exception(message); }
        async Task Wait(float seconds) => await ToSignal(GetTree().CreateTimer(seconds), SceneTreeTimer.SignalName.Timeout);
        if (!_paused) await Press(Key.Space);
        Directory.CreateDirectory("artifacts/f10-audio");
        foreach (var (cue, stream) in _sounds)
        {
            var data = stream.Data; int peak = 0;
            for (int i = 0; i < data.Length; i += 2) peak = Math.Max(peak, Math.Abs((int)BitConverter.ToInt16(data, i)));
            Check(peak > 10 && peak < 30000, $"Silent/clipped PCM: {cue}");
            if (cue == Cue.Wind) Check(Math.Abs(BitConverter.ToInt16(data, 0) - BitConverter.ToInt16(data, data.Length - 2)) < 500, "Wind loop discontinuity");
            Check(stream.SaveToWav($"artifacts/f10-audio/{cue.ToString().ToLowerInvariant()}.wav") == Error.Ok, "Audio preview export failed");
        }
        _soundMuted = false; _effectsSlider.Value = 65; _ambienceSlider.Value = 0;
        _effectsVolume = 65; _ambienceVolume = 0; ApplyAudioSettings();
        var capture = new AudioEffectCapture { BufferLength = 2 };
        int captureIndex = AudioServer.GetBusEffectCount(0); AudioServer.AddBusEffect(0, capture);
        float Peak()
        {
            var frames = capture.GetBuffer(capture.GetFramesAvailable());
            return frames.Length == 0 ? 0 : frames.Max(f => Math.Max(Math.Abs(f.X), Math.Abs(f.Y)));
        }
        await Wait(0.2f); capture.ClearBuffer(); UiCue(Cue.Place); await Wait(0.4f);
        float audible = Peak(); Check(audible > 0.0001f && audible < 0.95f, $"Mixer silent or clipping: {audible}");
        await UiClick(_muteSoundButton); Check(_soundMuted, "Mute button failed");
        await Wait(0.25f); capture.ClearBuffer(); UiCue(Cue.Place); await Wait(0.25f);
        Check(Peak() < 0.00001f, "Mute did not silence the mixer");
        await Press(Key.M); Check(!_soundMuted, "M did not unmute");
        _drawerPages[3].EnsureControlVisible(_effectsSlider); await Wait(0.1f);
        var rect = _effectsSlider.GetGlobalRect(); await Click(rect.Position + new Vector2(rect.Size.X * 0.35f, rect.Size.Y / 2));
        Check(_effectsVolume > 0 && _effectsVolume < 65, "Effects slider did not respond");
        _drawerPages[3].EnsureControlVisible(_ambienceSlider); await Wait(0.1f);
        rect = _ambienceSlider.GetGlobalRect(); await Click(rect.Position + new Vector2(rect.Size.X * 0.55f, rect.Size.Y / 2));
        Check(_ambienceVolume > 0, "Nature slider did not respond");
        await Wait(0.65f);
        float effects = _effectsVolume, ambience = _ambienceVolume;
        _effectsVolume = 99; _ambienceVolume = 99; ReadAudioSettings(); ApplyAudioSettings();
        Check(_effectsVolume == effects && _ambienceVolume == ambience, "Volume settings did not persist");
        string before = _world.SaveJson(); int sounds = _worldSoundCount;
        for (int i = 0; i < 20; i++) UpdateAudio(0.1f);
        Check(_worldSoundCount == sounds && _voices.All(v => !v.Playing), "Paused work emitted sound");
        _paused = false;
        for (int i = 0; i < 30; i++) WorldCue(Cue.Hammer, Vector3.Zero);
        _paused = true;
        Check(_worldSoundCount - sounds <= 1 && _voices.Count == 6, "Repeated events bypassed voice/cadence limits");
        Check(_world.SaveJson() == before, "Audio mutated the simulation");
        ResetWorldAudio(); Check(_voices.All(v => !v.Playing), "Reset left work sounds playing");
        Check(_effectsVolume == effects && _ambienceVolume == ambience, "World reset changed volume settings");
        AudioServer.RemoveBusEffect(0, captureIndex);
        _noticeUntil = 0; await Capture("artifacts/f10-controls.png");
        GD.Print($"AUDIO: mixer peak {audible:F4}; exported {_sounds.Count} WAV previews under artifacts/f10-audio.");
    }
}
