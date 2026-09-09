using Godot;

public partial class Game
{
    private Button _muteSoundButton = null!;
    private HSlider _effectsSlider = null!, _ambienceSlider = null!;
    private void MakeAudioUi(Control root)
    {
        var column = new VBoxContainer(); column.AddThemeConstantOverride("separation", 10); root.AddChild(column);
        var row = new HBoxContainer(); column.AddChild(row);
        _muteSoundButton = Button("", ToggleSoundMute, 165); row.AddChild(_muteSoundButton);
        row = new HBoxContainer(); column.AddChild(row);
        row.AddChild(Text("Effects", 14));
        _effectsSlider = new HSlider { MinValue = 0, MaxValue = 100, Step = 5, Value = _effectsVolume, CustomMinimumSize = new(90, 30), FocusMode = Control.FocusModeEnum.None };
        _effectsSlider.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        row.AddChild(_effectsSlider); _effectsSlider.TooltipText = "Work sounds and UI cues";
        _effectsSlider.ValueChanged += value => { _effectsVolume = (float)value; AudioVolumeChanged(); };
        _effectsSlider.DragEnded += changed => { if (changed) { SaveAudioSettings(); UiCue(Cue.Click); } };
        row = new HBoxContainer(); column.AddChild(row); row.AddChild(Text("Nature", 14));
        _ambienceSlider = new HSlider { MinValue = 0, MaxValue = 100, Step = 5, Value = _ambienceVolume, CustomMinimumSize = new(90, 30), FocusMode = Control.FocusModeEnum.None };
        _ambienceSlider.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        row.AddChild(_ambienceSlider); _ambienceSlider.TooltipText = "Wind and birds; continues while paused";
        _ambienceSlider.ValueChanged += value => { _ambienceVolume = (float)value; AudioVolumeChanged(); };
        _ambienceSlider.DragEnded += changed => { if (changed) SaveAudioSettings(); };
        MakeMusicUi(column);
        ApplyAudioSettings();
    }
}
