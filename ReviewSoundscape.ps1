$ErrorActionPreference = 'Stop'
$reviewDir = Join-Path $PSScriptRoot 'artifacts/soundscape'
$reviewResults = Get-Content -LiteralPath (Join-Path $reviewDir 'results.json') -Raw | ConvertFrom-Json
$reviewRows = foreach ($reviewEntry in $reviewResults | Where-Object { $_.name -like '*-before' }) {
    $reviewBase = $reviewEntry.name -replace '-before$',''
    $reviewLabel = [Net.WebUtility]::HtmlEncode($reviewBase)
    foreach ($reviewVersion in @('before','after')) {
        if (-not (Test-Path -LiteralPath (Join-Path $reviewDir "$reviewBase-$reviewVersion.wav"))) { throw "Missing recording: $reviewBase-$reviewVersion" }
    }
    "<tr><th>$reviewLabel</th><td><audio controls preload='none' src='$reviewBase-before.wav'></audio></td><td><audio controls preload='none' src='$reviewBase-after.wav'></audio></td></tr>"
}
$reviewHtml = @"
<!doctype html><html lang='en'><meta charset='utf-8'><title>Inlanders sound comparison</title>
<style>body{max-width:1000px;margin:40px auto;padding:0 20px;font:16px system-ui;background:#f5f2e8;color:#253c32}table{width:100%;border-collapse:collapse}th,td{text-align:left;padding:12px;border-bottom:1px solid #ccc}audio{max-width:100%}</style>
<h1>Spatial sound comparison</h1><p>Same starting worlds and settings: Effects 65, Nature 40, Music 35. Labels include zoom (12 close / 40 wide) and simulation speed. Play one recording at a time. Short excerpts establish comparison points; they do not cover a full music loop.</p>
<table><thead><tr><th>Scene</th><th>Before: camera listener</th><th>After: ground / zoom listener</th></tr></thead><tbody>$($reviewRows -join "`n")</tbody></table>
<script>document.addEventListener('play',e=>{for(const player of document.querySelectorAll('audio'))if(player!==e.target)player.pause()},true)</script></html>
"@
Set-Content -LiteralPath (Join-Path $reviewDir 'index.html') -Value $reviewHtml
Write-Output (Join-Path $reviewDir 'index.html')
