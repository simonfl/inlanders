param([switch]$Rendered)
$ErrorActionPreference = 'Stop'
Set-Location -LiteralPath $PSScriptRoot
$env:DOTNET_ROOT = Join-Path $PSScriptRoot '.tools\dotnet'
$env:PATH = "$env:DOTNET_ROOT;$env:PATH"
$env:DOTNET_CLI_HOME = Join-Path $PSScriptRoot '.tools\dotnet-home'
$env:APPDATA = Join-Path $PSScriptRoot '.tools\appdata'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
& "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj
if ($LASTEXITCODE -ne 0) { throw 'Simulation tests failed' }
if ($Rendered) {
    & ./Play.ps1 -SmokeTest
}
