using Godot;
using System;

public partial class Game
{
    private const string MusicBus = "Village music";
    private AudioStreamPlayer _music = null!;
    private Timer _musicRest = null!;
    private static readonly double[] MusicQuietIntervals = {18,26,22};
    private int _musicGapIndex;
    private AudioStreamWav[] _musicThemes = null!;
    private int _musicThemeIndex;
    private static readonly string[] MusicThemeNames={"Au bord du fleuve","Le bois et le pain","La veillée"};
    private float _musicVolume = 35;
    private bool _musicMuted;
    private HSlider _musicSlider = null!;
    private Button _muteMusicButton = null!;

    private void MakeMusic()
    {
        var timer=System.Diagnostics.Stopwatch.StartNew();
        _musicThemes=new[]{ComposeMusic(0),ComposeMusic(1),ComposeMusic(2)};
        GD.Print($"Rendered three acoustic themes in {timer.Elapsed.TotalSeconds:0.00}s.");
        _music=new AudioStreamPlayer { Bus=MusicBus, Stream=_musicThemes[0], VolumeDb=-4 };
        _musicRest=new Timer { OneShot=true,ProcessMode=ProcessModeEnum.Always,IgnoreTimeScale=true };
        AddChild(_musicRest);AddChild(_music);
        _music.Finished+=()=>
        {
            _musicRest.Start(MusicQuietIntervals[_musicGapIndex]);
            _musicGapIndex=(_musicGapIndex+1)%MusicQuietIntervals.Length;
        };
        _musicRest.Timeout+=PlayNextMusicTheme;
        _music.Play();
    }
    private void PlayNextMusicTheme()
    {
        _musicThemeIndex=(_musicThemeIndex+1)%_musicThemes.Length;
        _music.Stream=_musicThemes[_musicThemeIndex];_music.Play();
    }
    private void ToggleMusicMute() { _musicMuted=!_musicMuted; ApplyAudioSettings(); SaveAudioSettings(); }
    private void MakeMusicUi(VBoxContainer column)
    {
        var row=new HBoxContainer(); column.AddChild(row); row.AddChild(Text("Music",14));
        _musicSlider=new HSlider { MinValue=0,MaxValue=100,Step=5,Value=_musicVolume,CustomMinimumSize=new(90,30),
            SizeFlagsHorizontal=Control.SizeFlags.ExpandFill,FocusMode=Control.FocusModeEnum.None,
            TooltipText="Plucked strings and quiet woodwind phrases, with rests between pieces. Continues through pause and menus." };
        row.AddChild(_musicSlider);
        _musicSlider.ValueChanged+=value=> { _musicVolume=(float)value; AudioVolumeChanged(); };
        _musicSlider.DragEnded+=changed=> { if(changed) SaveAudioSettings(); };
        _muteMusicButton=Button("",ToggleMusicMute); column.AddChild(_muteMusicButton);
    }

}
