using Godot;
using System;
using System.IO;
using System.Linq;
using System.Text;

public partial class Game
{
    // Test-only export; the game retains only its three individual theme streams.
    private void ExportMusicCandidates()
    {
        const string directory="artifacts/music-candidates";
        Directory.CreateDirectory(directory);
        var html=new StringBuilder("<!doctype html><html lang='en'><meta charset='utf-8'><meta name='viewport' content='width=device-width'><title>Inlanders music candidates</title><style>body{font:18px system-ui;max-width:800px;margin:40px auto;padding:0 20px;background:#eef0e6;color:#24382f}article{padding:12px 0;border-bottom:1px solid #bbc6b6}audio{width:100%}p{line-height:1.5}</style><h1>Music candidates</h1><p>Three original pieces, followed by complete phrase-to-phrase transitions with the proposed quiet intervals. Audition at a comfortable volume. These exports are dry music, not a recording of the village mix. The instrumentation and interval lengths remain provisional.</p>");
        var hashes=new System.Collections.Generic.HashSet<string>();long bytes=0;
        for(int i=0;i<_musicThemes.Length;i++)
        {
            var stream=_musicThemes[i];var data=stream.Data;bytes+=data.Length;
            int peak=0;for(int sample=0;sample<data.Length;sample+=2)peak=Math.Max(peak,Math.Abs((int)BitConverter.ToInt16(data,sample)));
            if(stream.GetLength()!=96 || peak<1000 || peak>=26000 || Math.Abs(BitConverter.ToInt16(data,0))>10 || Math.Abs(BitConverter.ToInt16(data,data.Length-2))>10)
                throw new Exception("Invalid candidate PCM: "+MusicThemeNames[i]);
            if(!hashes.Add(Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(data))))throw new Exception("Duplicate theme PCM");
            if(stream.SaveToWav($"{directory}/theme-{i}.wav")!=Error.Ok)throw new Exception("Theme export failed");
            html.Append($"<article><h2>{MusicThemeNames[i]}</h2><audio controls preload='none' src='theme-{i}.wav'></audio></article>");
            var next=_musicThemes[(i+1)%_musicThemes.Length].Data;
            var combined=new byte[data.Length+(int)(MusicQuietIntervals[i]*22050*2)+next.Length];
            Buffer.BlockCopy(data,0,combined,0,data.Length);Buffer.BlockCopy(next,0,combined,combined.Length-next.Length,next.Length);
            using var transition=new AudioStreamWav {Format=AudioStreamWav.FormatEnum.Format16Bits,MixRate=22050,Data=combined};
            if(transition.SaveToWav($"{directory}/transition-{i}.wav")!=Error.Ok)throw new Exception("Transition export failed");
            GD.Print($"MUSIC {MusicThemeNames[i]}: {data.Length} PCM bytes, peak {peak}/32767, next gap {MusicQuietIntervals[i]} seconds.");
        }
        if(_musicThemes.Length!=3 || bytes>13_000_000)throw new Exception("Music stream budget exceeded");
        html.Append("<h2>Full transitions</h2><p>Each starts with a complete 96-second piece. Silence is intentional; leave the player running to hear the next entrance.</p>");
        for(int i=0;i<3;i++)html.Append($"<article><h3>{MusicThemeNames[i]} → {MusicThemeNames[(i+1)%3]} · {MusicQuietIntervals[i]}s quiet</h3><audio controls preload='none' src='transition-{i}.wav'></audio></article>");
        html.Append("<p>Listen for repetition, harsh notes, abrupt endings, and whether the quiet intervals feel comfortable. Compare against the populated recordings in the soundscape review before choosing a final mix.</p></html>");
        File.WriteAllText(directory+"/index.html",html.ToString());
        GD.Print($"MUSIC retained PCM: {bytes} bytes in three pre-rendered streams; one playback voice.");
    }
}
