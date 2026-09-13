param([int[]]$Widths=@(960,1440),[int[]]$Turns=@(0,2))
$ErrorActionPreference='Stop'
Set-StrictMode -Version Latest
Set-Location -LiteralPath $PSScriptRoot
$timer=[Diagnostics.Stopwatch]::StartNew()
$rows=[Collections.Generic.List[string]]::new()
$rows.Add('# Court daily-life comparison')
$rows.Add('')
$rows.Add('Each pair shows the same authoritative world and activity. Camera and presentation differ. These are scripted stills, not human recognition or listening results.')
$rows.Add('')
$rows.Add('| View/activity | Control | Candidate |')
$rows.Add('| --- | --- | --- |')
$commonSource=$null;$commonAssembly=$null
foreach($width in $Widths){foreach($turn in $Turns){
    $output=@(& "$PSScriptRoot/Review.ps1" Capture -Scenario court-life -Width $width -Turn $turn -Speed 1 -ProbeControls)
    $output | Write-Output
    $line=@($output | Where-Object {$_ -match '^Review PID \d+: (.+)$'})
    if($line.Count -ne 1){throw 'Expected one capture run'}
    $null=$line[0] -match '^Review PID \d+: (.+)$';$directory=$Matches[1]
    $request=Get-Content -LiteralPath (Join-Path $directory 'request.json') -Raw | ConvertFrom-Json
    $checks=Get-Content -LiteralPath (Join-Path $directory 'checks.json') -Raw | ConvertFrom-Json
    if(!$checks.passed){throw 'Control checks failed'}
    if($null -eq $commonSource){$commonSource=$request.sourceFingerprint;$commonAssembly=$request.assemblyHash}
    if($request.sourceFingerprint -ne $commonSource -or $request.assemblyHash -ne $commonAssembly){throw 'Build changed during comparison'}
    $captures=@(Get-ChildItem -LiteralPath $directory -Directory -Filter 'capture-*' | ForEach-Object {
        $manifest=Join-Path $_.FullName 'manifest.json'
        if(Test-Path -LiteralPath $manifest){[pscustomobject]@{Folder=$_.Name;Data=(Get-Content -LiteralPath $manifest -Raw | ConvertFrom-Json)}}
    })
    foreach($activity in @('work','meal','rest')){
        $control=@($captures | Where-Object {$_.Data.semantic -eq "court-life-$activity-control"})
        $candidate=@($captures | Where-Object {$_.Data.semantic -eq "court-life-$activity-candidate"})
        if($control.Count -ne 1 -or $candidate.Count -ne 1 -or $control[0].Data.worldHash -ne $candidate[0].Data.worldHash){throw "Unmatched $activity pair"}
        $run=Split-Path -Leaf $directory
        $a="review/runs/$run/$($control[0].Folder)";$b="review/runs/$run/$($candidate[0].Folder)"
        $rows.Add("| $width / turn $turn / $activity | [![control]($a/view.png)]($a/manifest.json) | [![candidate]($b/view.png)]($b/manifest.json) |")
    }
}}
$rows.Add('');$rows.Add("Source: $commonSource · assembly: $commonAssembly")
$rows.Add("Elapsed setup/capture/index time: $([Math]::Round($timer.Elapsed.TotalSeconds,2))s. Not game frame performance.")
$path=Join-Path $PSScriptRoot 'artifacts/court-life-comparison.md'
[IO.File]::WriteAllLines($path,$rows)
Write-Output "Verified comparison index: $path"
