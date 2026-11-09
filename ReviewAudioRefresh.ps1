$ErrorActionPreference='Stop'
$reviewRoot=Join-Path $PSScriptRoot 'artifacts/audio-refresh'
$musicNames=@('Au bord du fleuve','Le bois et le pain','La veillée')
$rows=for($i=0;$i -lt 3;$i++) {
    foreach($version in @('before','after')) {
        if(-not (Test-Path -LiteralPath (Join-Path $reviewRoot "$version/theme-$i.wav"))) { throw "Missing $version/theme-$i.wav" }
    }
    "<article><h2>$($musicNames[$i])</h2><div class='pair'><div><p>Previous synth arrangement</p><audio controls preload='none' src='before/theme-$i.wav'></audio></div><div><p>New acoustic arrangement</p><audio controls preload='none' src='after/theme-$i.wav'></audio></div></div></article>"
}
$effects=foreach($cue in @('chop','hammer','step','rustle','saw','drop')) {
    "<article><h3>$cue</h3><div class='pair'><div><p>Previous</p><audio controls preload='none' src='before/effects/$cue.wav'></audio></div><div><p>New example (four variants in game)</p><audio controls preload='none' src='after/$cue-0.wav'></audio></div></div></article>"
}
$mixes=foreach($entry in (Get-Content -LiteralPath (Join-Path $reviewRoot 'after/mix.json') -Raw | ConvertFrom-Json)) {
    $name="$($entry.scene)-$($entry.speed)x"
    "<article><h3>$name</h3><audio controls preload='none' src='after/$name.wav'></audio></article>"
}
$html=@"
<!doctype html><html lang='en'><meta charset='utf-8'><meta name='viewport' content='width=device-width'>
<title>Les Habitants — sound audition</title>
<style>body{max-width:980px;margin:36px auto;padding:0 24px;font:17px system-ui;background:#f4f0e5;color:#293d34}p{line-height:1.5}article{padding:14px 0;border-bottom:1px solid #c9c9b8}.pair{display:grid;grid-template-columns:1fr 1fr;gap:24px}audio{width:100%}h1,h2,h3{font-weight:600}@media(max-width:650px){.pair{grid-template-columns:1fr}}</style>
<h1>Les Habitants — sound audition</h1>
<p>Original music and synthesized effects. These are acoustic-inspired sounds, not recordings of historical instruments. Start at a comfortable volume. Dry tracks below are louder than the in-game mix; compare like with like.</p>
<h2>Music</h2><p>Three 96-second pieces. The game leaves 18–26 seconds of nature sounds between them. Listen through the middle for the woodwind phrases and toward the end for the closing cadence.</p>
$($rows -join "`n")
<h2>Everyday work</h2><p>Isolated samples, without the game's quieter footsteps, distance or speed limits.</p>
$($effects -join "`n")
<h2>Actual game mix</h2><p>Native mixer recordings at Effects 65, Nature 40, Music 35. Camera zoom 26. These are new-build recordings at 1x and 6x simulated speed, not an old/new mix comparison. The music stays at normal speed.</p>
$($mixes -join "`n")
<p>What feels too repetitive, artificial or loud? Does the music make it pleasant to stay with the village? Audio checks establish playback behavior, not listening preference.</p>
<script>document.addEventListener('play',e=>{for(const player of document.querySelectorAll('audio'))if(player!==e.target)player.pause()},true)</script></html>
"@
[IO.File]::WriteAllText((Join-Path $reviewRoot 'index.html'),$html)
Write-Output (Join-Path $reviewRoot 'index.html')
