param([switch]$SmokeTest, [switch]$AudioSmokeTest, [switch]$HudSmokeTest, [switch]$CampaignSmokeTest, [switch]$MapSmokeTest, [switch]$ClearingSmokeTest, [switch]$MenuSmokeTest, [switch]$ArtSmokeTest, [switch]$CatalogSmokeTest, [switch]$ProductionSmokeTest, [switch]$RiverSmokeTest)
$ErrorActionPreference = 'Stop'
Set-Location -LiteralPath $PSScriptRoot
$env:DOTNET_ROOT = Join-Path $PSScriptRoot '.tools\dotnet'
$env:PATH = "$env:DOTNET_ROOT;$env:PATH"
$env:DOTNET_CLI_HOME = Join-Path $PSScriptRoot '.tools\dotnet-home'
$env:APPDATA = Join-Path $PSScriptRoot '.tools\appdata'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
& "$env:DOTNET_ROOT\dotnet.exe" build Inlanders.csproj --nologo
if ($LASTEXITCODE -ne 0) { throw 'Build failed' }
$engine = Join-Path $PSScriptRoot '.tools\godot\Godot_v4.6-stable_mono_win64\Godot_v4.6-stable_mono_win64.exe'
if ($RiverSmokeTest -or $SmokeTest -or $AudioSmokeTest -or $HudSmokeTest -or $CampaignSmokeTest -or $MapSmokeTest -or $ClearingSmokeTest -or $MenuSmokeTest -or $ArtSmokeTest -or $CatalogSmokeTest -or $ProductionSmokeTest) {
    $engine = $engine.Replace('_win64.exe', '_win64_console.exe')
    $testArgument = if ($RiverSmokeTest) { '--river-smoke-test' } elseif ($ProductionSmokeTest) { '--production-smoke-test' } elseif ($CatalogSmokeTest) { '--catalog-smoke-test' } elseif ($ArtSmokeTest) { '--art-smoke-test' } elseif ($MenuSmokeTest) { '--menu-smoke-test' } elseif ($ClearingSmokeTest) { '--clearing-smoke-test' } elseif ($MapSmokeTest) { '--map-smoke-test' } elseif ($CampaignSmokeTest) { '--campaign-smoke-test' } elseif ($HudSmokeTest) { '--hud-smoke-test' } elseif ($AudioSmokeTest) { '--audio-smoke-test' } else { '--smoke-test' }
    & $engine --path $PSScriptRoot -- $testArgument
    if ($LASTEXITCODE -ne 0) { throw 'Rendered smoke test failed' }
} else {
    & $engine --path $PSScriptRoot
}
