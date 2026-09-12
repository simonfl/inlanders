param([switch]$QuarryChallenge, [switch]$Rendered, [switch]$Balance, [switch]$ComfortComparison)
$ErrorActionPreference = 'Stop'
Set-Location -LiteralPath $PSScriptRoot
$env:DOTNET_ROOT = Join-Path $PSScriptRoot '.tools\dotnet'
$env:PATH = "$env:DOTNET_ROOT;$env:PATH"
$env:DOTNET_CLI_HOME = Join-Path $PSScriptRoot '.tools\dotnet-home'
$env:APPDATA = Join-Path $PSScriptRoot '.tools\appdata'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
if ($QuarryChallenge) { & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj -- --quarry-challenge }
elseif ($ComfortComparison) { & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj -- --comfort-comparison }
elseif ($Balance) { & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj -- --balance }
else { & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj }
if ($LASTEXITCODE -ne 0) { throw 'Simulation tests failed' }
if ($Rendered) {
    & ./Play.ps1 -SmokeTest
    & ./Play.ps1 -HudSmokeTest
    & ./Play.ps1 -CampaignSmokeTest
    & ./Play.ps1 -FishingSmokeTest
    & ./Play.ps1 -MapSmokeTest
    & ./Play.ps1 -ClearingSmokeTest
    & ./Play.ps1 -MenuSmokeTest
}
