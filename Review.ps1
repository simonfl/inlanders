param(
    [ValidateSet('List','Build','Prepare','Inspect','Capture','Check')][string]$Action='List',
    [string]$Scenario='river',
    [ValidateSet(1,3,6)][int]$Speed=3,
    [ValidateSet(960,1440)][int]$Width=1440,
    [ValidateRange(0,3)][int]$Turn=0,
    [switch]$Fresh,
    [switch]$ReuseOnly,
    [string]$Bundle,
    [switch]$ProbeControls,
    [switch]$Storybook,
    [ValidateRange(0,60)][int]$ObserveSeconds=0,
    [switch]$Movie
)
$ErrorActionPreference='Stop'
Set-StrictMode -Version Latest
Set-Location -LiteralPath $PSScriptRoot
$catalog=@(Get-Content -LiteralPath Development/review-scenarios.json -Raw | ConvertFrom-Json)
if ($Action -eq 'List') { $catalog | Select-Object name,description; return }
$scenarioEntry=@($catalog | Where-Object name -eq $Scenario)
if ($scenarioEntry.Count -ne 1) { throw "Unknown scenario '$Scenario'. Run ./Review.ps1 List." }
$scenarioEntry=$scenarioEntry[0]
if($Bundle -and $Action -notin @('Inspect','Capture','Check')) { throw '-Bundle applies to Inspect, Capture or Check.' }
if($ProbeControls -and $Action -ne 'Capture') { throw '-ProbeControls requires Capture.' }
if(($Movie -or $ObserveSeconds -gt 0) -and $Action -ne 'Capture'){throw 'Observation output requires Capture.'}
if($Movie -and $ObserveSeconds -eq 0){$ObserveSeconds=5}
$env:DOTNET_ROOT=Join-Path $PSScriptRoot '.tools/dotnet'
$env:DOTNET_CLI_HOME=Join-Path $PSScriptRoot '.tools/dotnet-home'
$env:APPDATA=Join-Path $PSScriptRoot '.tools/appdata'
$env:DOTNET_CLI_TELEMETRY_OPTOUT='1'
$dotnet=Join-Path $env:DOTNET_ROOT 'dotnet.exe'
$reviewRoot=Join-Path $PSScriptRoot 'artifacts/review'
[IO.Directory]::CreateDirectory($reviewRoot) | Out-Null
$timer=[Diagnostics.Stopwatch]::StartNew()
function HashFile([string]$Path) { (Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash }
function Fingerprint {
    # Conservative content invalidation: no timestamps, documentation, artifacts or downloaded tools.
    $inputs=@(Get-ChildItem -LiteralPath $PSScriptRoot -File | Where-Object Extension -in '.cs','.csproj','.godot','.tscn','.ps1')
    foreach ($folder in @('Simulation','Tests','Development')) {
        $inputs+=Get-ChildItem -LiteralPath (Join-Path $PSScriptRoot $folder) -Recurse -File |
            Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' -and $_.Extension -in '.cs','.csproj','.json' }
    }
    $lines=@($inputs | Sort-Object FullName | ForEach-Object { $_.FullName.Substring($PSScriptRoot.Length)+':'+(HashFile $_.FullName) })
    $sha=[Security.Cryptography.SHA256]::Create()
    try { ([BitConverter]::ToString($sha.ComputeHash([Text.Encoding]::UTF8.GetBytes(($lines -join "`n"))))).Replace('-','') }
    finally { $sha.Dispose() }
}
function WriteJson($Value,[string]$Path) { [IO.File]::WriteAllText($Path,($Value | ConvertTo-Json -Depth 20)) }
function ReadJson([string]$Path) { if(Test-Path -LiteralPath $Path) { Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json } }
$fingerprint=Fingerprint
$buildPath=Join-Path $reviewRoot 'build.json'
$assembly=Join-Path $PSScriptRoot '.godot/mono/temp/bin/Debug/Inlanders.dll'
$testAssembly=Join-Path $PSScriptRoot 'Tests/bin/Debug/net8.0/SimulationTests.dll'
$build=ReadJson $buildPath
$validBuild=$null -ne $build -and $build.sourceFingerprint -eq $fingerprint -and
    (Test-Path -LiteralPath $assembly) -and (Test-Path -LiteralPath $testAssembly)
if($validBuild) { $validBuild=$build.assemblyHash -eq (HashFile $assembly) -and $build.testAssemblyHash -eq (HashFile $testAssembly) }
if ($Action -eq 'Build' -or -not $validBuild) {
    if($ReuseOnly) { throw 'Build is missing/stale. Run ./Review.ps1 Build, or omit -ReuseOnly.' }
    & $dotnet build Inlanders.csproj --nologo
    if($LASTEXITCODE -ne 0) { throw 'Game build failed' }
    & $dotnet build Tests/SimulationTests.csproj --nologo
    if($LASTEXITCODE -ne 0) { throw 'Fixture build failed' }
    $build=@{sourceFingerprint=$fingerprint;assemblyHash=(HashFile $assembly);testAssemblyHash=(HashFile $testAssembly);builtUtc=[DateTime]::UtcNow.ToString('o')}
    WriteJson $build $buildPath
}
if($Action -eq 'Build') { Write-Output "Build ready in $([math]::Round($timer.Elapsed.TotalSeconds,2))s"; return }
$bundleRecord=$null
if($Bundle) {
    $bundleDirectory=(Resolve-Path -LiteralPath $Bundle).Path
    $bundleRecord=ReadJson (Join-Path $bundleDirectory 'manifest.json')
    if($null -eq $bundleRecord -or $bundleRecord.request.sourceFingerprint -ne $fingerprint) { throw 'Bundle source fingerprint is missing/stale; regenerate with the current build.' }
    $fixturePath=Join-Path $bundleDirectory 'world.json'
    if((HashFile $fixturePath) -ne $bundleRecord.worldHash) { throw 'Bundle world hash differs from manifest' }
    $fixture=@{scenario=$bundleRecord.request.scenario;variant=$bundleRecord.request.variant;generator='captured-state';sourceFingerprint=$fingerprint;fixtureHash=$bundleRecord.worldHash;parentBundle=$bundleDirectory}
    $Scenario=$bundleRecord.request.scenario;$Speed=[int]$bundleRecord.speed;$Width=[int]$bundleRecord.window.width
} else {
$fixtureDir=Join-Path $reviewRoot ('fixtures/'+$Scenario)
[IO.Directory]::CreateDirectory($fixtureDir) | Out-Null
$fixturePath=Join-Path $fixtureDir 'world.json'
$fixtureManifest=Join-Path $fixtureDir 'manifest.json'
$fixture=ReadJson $fixtureManifest
$validFixture=$null -ne $fixture -and $fixture.sourceFingerprint -eq $fingerprint -and (Test-Path -LiteralPath $fixturePath)
if($validFixture) { $validFixture=$fixture.fixtureHash -eq (HashFile $fixturePath) }
if($Fresh -or -not $validFixture) {
    if($ReuseOnly) { throw "Fixture '$Scenario' is missing/stale. Run ./Review.ps1 Prepare $Scenario, or omit -ReuseOnly." }
    $prepareTimer=[Diagnostics.Stopwatch]::StartNew()
    & $dotnet $testAssembly --review-fixture $scenarioEntry.generator $fixturePath
    if($LASTEXITCODE -ne 0) { throw 'Scenario preparation failed' }
    $fixture=@{scenario=$Scenario;variant='current';generator=$scenarioEntry.generator;sourceFingerprint=$fingerprint;fixtureHash=(HashFile $fixturePath);preparedUtc=[DateTime]::UtcNow.ToString('o');prepareSeconds=$prepareTimer.Elapsed.TotalSeconds;seed=$null;seedNote='Authored deterministic scenario; no configurable random seed'}
    WriteJson $fixture $fixtureManifest
}
}
if($Action -eq 'Prepare') { Write-Output "Prepared $Scenario in $([math]::Round($timer.Elapsed.TotalSeconds,2))s (including validation/build if needed)."; return }
if($Action -eq 'Check') {
    & $dotnet $testAssembly --review-fixture check $fixturePath
    if($LASTEXITCODE -ne 0) { throw 'Snapshot validation failed' }
    Write-Output "Checked $Scenario in $([math]::Round($timer.Elapsed.TotalSeconds,2))s"; return
}
$runDir=Join-Path $reviewRoot ('runs/'+[DateTime]::UtcNow.ToString('yyyyMMdd-HHmmss-fff')+'-'+$Scenario+'-'+[Guid]::NewGuid().ToString('N').Substring(0,6))
[IO.Directory]::CreateDirectory($runDir) | Out-Null
Copy-Item -LiteralPath $fixturePath -Destination (Join-Path $runDir 'initial.json')
$gitSafeRoot=$PSScriptRoot.Replace('\','/')
$gitRevision=(& git -c "safe.directory=$gitSafeRoot" rev-parse HEAD) -join ''
if($LASTEXITCODE -ne 0) { throw 'Could not record Git revision' }
$gitStatus=@(& git -c "safe.directory=$gitSafeRoot" status --porcelain)
if($LASTEXITCODE -ne 0) { throw 'Could not record worktree state' }
$request=@{scenario=$Scenario;variant='current';sourceFingerprint=$fingerprint;assemblyHash=$build.assemblyHash;commit=$gitRevision;dirty=($gitStatus.Count -gt 0);gitStatus=$gitStatus;fixture=$fixture;runDirectory=$runDir;fixturePath=(Join-Path $runDir 'initial.json');width=$Width;height=$(if($Width -eq 960){640}else{900});speed=$Speed;turn=$Turn;focusX=$scenarioEntry.focusX;focusZ=$scenarioEntry.focusZ;zoom=$scenarioEntry.zoom;executionMode='normal Godot process; starts paused';requestedUtc=[DateTime]::UtcNow.ToString('o');setupSeconds=$timer.Elapsed.TotalSeconds;captureOnly=($Action -eq 'Capture')}
if($bundleRecord) {
    if($bundleRecord.rendering.PSObject.Properties.Name -contains 'storybook'){$request.storybook=$bundleRecord.rendering.storybook}
    $request.width=$bundleRecord.window.width;$request.height=$bundleRecord.window.height
    $request.focusX=$bundleRecord.camera.focusX;$request.focusZ=$bundleRecord.camera.focusZ;$request.zoom=$bundleRecord.camera.zoom
    $request.angle=$bundleRecord.camera.angle;$request.view=$bundleRecord.rendering;$request.audio=$bundleRecord.audio;$request.selected=$bundleRecord.selected
}
$requestPath=Join-Path $runDir 'request.json';WriteJson $request $requestPath
$request.observeSeconds=$ObserveSeconds;$request.movieFps=if($Movie){24}else{0};WriteJson $request $requestPath
if($Storybook -and -not $Bundle){$request.storybook=$true;WriteJson $request $requestPath}
if($ProbeControls) { $request.probeControls=$true;WriteJson $request $requestPath }
$engine=Join-Path $PSScriptRoot '.tools/godot/Godot_v4.6-stable_mono_win64/Godot_v4.6-stable_mono_win64_console.exe'
$launchArgs=@('--path',('"'+$PSScriptRoot+'"'),'--','--review-run',('"'+$requestPath+'"'))
if($Movie){$launchArgs=@('--path',('"'+$PSScriptRoot+'"'),'--write-movie',('"'+(Join-Path $runDir 'observation.avi')+'"'),'--fixed-fps','24','--','--review-run',('"'+$requestPath+'"'))}
$windowStyle=if($Action -eq 'Inspect'){'Normal'}else{'Hidden'}
$reviewProcess=Start-Process -FilePath $engine -ArgumentList $launchArgs -WindowStyle $windowStyle -PassThru -RedirectStandardOutput (Join-Path $runDir 'stdout.log') -RedirectStandardError (Join-Path $runDir 'stderr.log')
Write-Output "Review PID $($reviewProcess.Id): $runDir"
if($Action -eq 'Inspect') { Write-Output 'Starts paused. Normal controls; F8 exports a matching screenshot/state bundle. Close the window to finish.'; return }
$reviewProcess.WaitForExit();$reviewProcess.Refresh()
if($reviewProcess.ExitCode -ne 0 -or -not (Test-Path -LiteralPath (Join-Path $runDir 'capture-0001/manifest.json'))) {
    Get-Content -LiteralPath (Join-Path $runDir 'stderr.log');throw 'Review capture failed; inspect the run logs.'
}
if($Movie -and (-not (Test-Path -LiteralPath (Join-Path $runDir 'observation.avi')) -or -not (Select-String -LiteralPath (Join-Path $runDir 'stdout.log') -SimpleMatch 'Done recording movie' -Quiet))){throw 'Movie was not finalized; inspect run logs.'}
Write-Output "Captured $Scenario in $([math]::Round($timer.Elapsed.TotalSeconds,2))s: $runDir"
