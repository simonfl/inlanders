param([string]$Scenario='commons-recurring')
$ErrorActionPreference='Stop'
Set-StrictMode -Version Latest
$repo=Split-Path $PSScriptRoot
$runner=Join-Path $repo 'Review.ps1'
$ui=Join-Path $repo 'ReviewCacheUiProbe.cs'
$simulation=Join-Path $repo 'Simulation/ReviewCacheRuleProbe.cs'
$generator=Join-Path $repo 'Tests/ReviewCacheGeneratorProbe.cs'
foreach($path in @($ui,$simulation,$generator)){if(Test-Path -LiteralPath $path){throw "Probe already exists: $path"}}
function Reject([scriptblock]$Operation,[string]$Message) {
    try { & $Operation;throw 'Expected rejection did not occur' }
    catch { if(-not $_.Exception.Message.Contains($Message)){throw};Write-Output "PASS: rejected $Message" }
}
& $runner Prepare $Scenario
$manifest=Join-Path $repo "artifacts/review/fixtures/$Scenario/manifest.json"
$world=Join-Path $repo "artifacts/review/fixtures/$Scenario/world.json"
$originalManifest=[IO.File]::ReadAllText($manifest)
$originalWorld=[IO.File]::ReadAllBytes($world)
$baseline=$originalManifest | ConvertFrom-Json
try {
    [IO.File]::WriteAllText($ui,'// Temporary UI-only cache integration probe.')
    & $runner Build
    $timer=[Diagnostics.Stopwatch]::StartNew();& $runner Prepare $Scenario -ReuseOnly;$reuseSeconds=$timer.Elapsed.TotalSeconds
    if([IO.File]::ReadAllText($manifest) -ne $originalManifest){throw 'UI edit regenerated or relabeled fixture provenance'}
    $capture=(& $runner Capture $Scenario -ReuseOnly -Storybook) -join "`n";Write-Output $capture
    $run=[regex]::Match($capture,'Review PID \d+: ([^\r\n]+)').Groups[1].Value.Trim()
    $request=Get-Content -LiteralPath (Join-Path $run 'request.json') -Raw | ConvertFrom-Json
    if($request.sourceFingerprint -eq $baseline.sourceFingerprint -or $request.fixtureFingerprint -ne $baseline.fixtureFingerprint -or
        $request.fixture.sourceFingerprint -ne $baseline.sourceFingerprint){throw 'Capture provenance confused current build and prepared fixture'}
    Remove-Item -LiteralPath $ui
    & $runner Build
    Reject { & $runner Check $Scenario -Bundle (Join-Path $run 'capture-0001') -ReuseOnly } 'Bundle source fingerprint is missing/stale'
    foreach($path in @($simulation,$generator)) {
        [IO.File]::WriteAllText($path,'// Temporary simulation/generator invalidation probe.')
        & $runner Build
        Reject { & $runner Prepare $Scenario -ReuseOnly } "Fixture '$Scenario' is missing/stale"
        Remove-Item -LiteralPath $path
    }
    & $runner Build
    [IO.File]::AppendAllText($world,"`n")
    Reject { & $runner Prepare $Scenario -ReuseOnly } "Fixture '$Scenario' is missing/stale"
    [IO.File]::WriteAllBytes($world,$originalWorld)
    & $runner Check $Scenario -ReuseOnly
    Write-Output "PASS: real UI-only rebuild reused unchanged fixture in $([math]::Round($reuseSeconds,2))s; original preparation $([math]::Round($baseline.prepareSeconds,2))s. Simulation/generator edits, altered world and stale capture rejected."
} finally {
    foreach($path in @($ui,$simulation,$generator)){if(Test-Path -LiteralPath $path){Remove-Item -LiteralPath $path}}
    [IO.File]::WriteAllBytes($world,$originalWorld)
    [IO.File]::WriteAllText($manifest,$originalManifest)
}
