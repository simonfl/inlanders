param([switch]$BushRelocation, [switch]$WorkplaceAssignment, [switch]$StoneStagingPlayable, [switch]$StoneStorage, [switch]$StoneStaging, [switch]$CreativeStock, [switch]$CreativeRemoval, [switch]$CampaignReview, [switch]$Relocation, [switch]$OrchardPlayable, [switch]$Orchard, [switch]$Restoration, [switch]$CivicBudget, [switch]$FinaleCampaign, [switch]$FinaleDecision, [switch]$WoodsBrief, [switch]$QuarryChallenge, [switch]$Rendered, [switch]$Balance, [switch]$ComfortComparison)
$ErrorActionPreference = 'Stop'
Set-Location -LiteralPath $PSScriptRoot
$env:DOTNET_ROOT = Join-Path $PSScriptRoot '.tools\dotnet'
$env:PATH = "$env:DOTNET_ROOT;$env:PATH"
$env:DOTNET_CLI_HOME = Join-Path $PSScriptRoot '.tools\dotnet-home'
$env:APPDATA = Join-Path $PSScriptRoot '.tools\appdata'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
if ($BushRelocation) { & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj -- --bush-relocation }
elseif ($WorkplaceAssignment) { & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj -- --workplace-assignment }
elseif ($StoneStagingPlayable) { & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj -- --stone-staging-playable }
elseif ($StoneStorage) { & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj -- --stone-storage }
elseif ($StoneStaging) { & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj -- --stone-staging }
elseif ($CreativeStock) { & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj -- --creative-stock }
elseif ($CreativeRemoval) { & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj -- --creative-removal }
elseif ($CampaignReview) { & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj -- --campaign-review }
elseif ($Relocation) { & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj -- --relocation }
elseif ($OrchardPlayable) { & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj -- --orchard-playable }
elseif ($Orchard) { & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj -- --orchard }
elseif ($Restoration) { & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj -- --restoration }
elseif ($CivicBudget) { & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj -- --civic-budget }
elseif ($FinaleCampaign) { & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj -- --finale-campaign }
elseif ($FinaleDecision) { & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj -- --finale-decision }
elseif ($WoodsBrief) { & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj -- --woods-brief }
elseif ($QuarryChallenge) { & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj -- --quarry-challenge }
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
