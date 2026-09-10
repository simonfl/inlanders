using Godot;
using System;
using System.IO;
using System.Linq;

public partial class Game
{
    private Godot.Environment _villageEnvironment = null!;
    private DirectionalLight3D _sun = null!;
    private bool _goldenHour, _foliageMotion = true;
    private bool _frameSync;
    private Button _frameSyncButton = null!;
    private Button _lightMoodButton = null!, _foliageButton = null!;
    private string _atmospherePath = "saves/atmosphere.cfg";

    private static Color GroundTint(int x, int z)
    {
        float patch = MathF.Sin(x * 0.31f) * MathF.Cos(z * 0.27f) * 0.025f;
        float grain = MathF.Sin(x * 12.989f + z * 78.233f) * 0.009f;
        return new(0.43f + patch + grain, 0.50f + patch + grain, 0.33f + patch + grain);
    }
    private void LoadAtmosphere()
    {
        var config = new ConfigFile();
        if (config.Load(_atmospherePath) != Error.Ok) return;
        _goldenHour = config.GetValue("view", "golden_hour", false).AsBool();
        _foliageMotion = config.GetValue("view", "foliage_motion", true).AsBool();
        _showWorldLabels = config.GetValue("view", "world_labels", true).AsBool();
        _frameSync = config.GetValue("view", "frame_sync", false).AsBool();
    }
    private void SaveAtmosphere()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(_atmospherePath))!);
            var config = new ConfigFile();
            config.SetValue("view", "golden_hour", _goldenHour);
            config.SetValue("view", "foliage_motion", _foliageMotion);
            config.SetValue("view", "world_labels", _showWorldLabels);
            config.SetValue("view", "frame_sync", _frameSync);
            if (config.Save(_atmospherePath) != Error.Ok) Notice("Atmosphere changed, but settings could not be saved.");
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException) { Notice("Atmosphere changed, but settings could not be saved."); }
    }
    private void ApplyAtmosphere()
    {
        DisplayServer.WindowSetVsyncMode(_frameSync?DisplayServer.VSyncMode.Enabled:DisplayServer.VSyncMode.Disabled);
        if(_frameSyncButton!=null) _frameSyncButton.Text=_frameSync?"Frame sync: on":"Frame sync: off";
        _sun.RotationDegrees = _goldenHour ? new(-36, -35, 0) : new(-52, -30, 0);
        _sun.LightColor = _goldenHour ? new("ffd5a1") : new("fff0d6");
        _sun.LightEnergy = _goldenHour ? 0.78f : 0.72f;
        _villageEnvironment.AmbientLightColor = _goldenHour ? new("cbd8df") : new("dbe5df");
        _villageEnvironment.AmbientLightEnergy = _goldenHour ? 0.55f : 0.52f;
        _villageEnvironment.BackgroundColor = _goldenHour ? new("c8bda7") : new("b1c4b9");
        if (_lightMoodButton != null)
        {
            _lightMoodButton.Text = _goldenHour ? "Light: golden hour" : "Light: soft daylight";
            _foliageButton.Text = _foliageMotion ? "Foliage motion: on" : "Foliage motion: off";
        }
        UpdateAtmosphere();
    }
    private void MakeAtmosphereUi(VBoxContainer column)
    {
        column.AddChild(Text("ATMOSPHERE", 12));
        _lightMoodButton = Button("", () => { _goldenHour = !_goldenHour; ApplyAtmosphere(); SaveAtmosphere(); });
        _foliageButton = Button("", () => { _foliageMotion = !_foliageMotion; ApplyAtmosphere(); SaveAtmosphere(); });
        column.AddChild(_lightMoodButton); column.AddChild(_foliageButton);
        _frameSyncButton=Button("",()=> { _frameSync=!_frameSync; ApplyAtmosphere(); SaveAtmosphere(); }); column.AddChild(_frameSyncButton);
        _frameSyncButton.TooltipText="Off renders without waiting for display sync and can improve responsiveness. Enable if you notice tearing; it may limit frame rate. This does not change village speed.";
        _worldLabelsButton = Button("", ToggleWorldLabels); column.AddChild(_worldLabelsButton); UpdateLabelButtons();
        column.AddChild(Text("Light is a visual choice. Gentle foliage movement follows village time and pauses with the game.", 14, true));
        ApplyAtmosphere();
    }
    private void UpdateAtmosphere()
    {
        float time = _world.Food.Time;
        foreach (Node3D crown in GetTree().GetNodesInGroup("foliage"))
        {
            if (crown.IsQueuedForDeletion()) continue;
            bool preview = _ghostModel != null && _ghostModel.IsAncestorOf(crown);
            float phase = crown.GetMeta("breeze_phase").AsSingle();
            crown.Rotation = !_foliageMotion || preview ? Vector3.Zero :
                new(MathF.Sin(time * 0.7f + phase) * 0.018f, 0, MathF.Sin(time * 0.53f + phase) * 0.025f);
        }
    }
}
