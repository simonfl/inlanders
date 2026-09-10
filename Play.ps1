param([switch]$CameraDragSmokeTest, [switch]$ComfortSmokeTest, [switch]$RenderIsolationSmokeTest, [switch]$WatchSmokeTest, [switch]$HudServicesSmokeTest, [switch]$PantrySmokeTest, [switch]$SurveySmokeTest, [switch]$WildlifeSmokeTest, [switch]$QuarrySmokeTest, [switch]$SocialSmokeTest, [switch]$FieldWorkSmokeTest, [switch]$HandoffSmokeTest, [switch]$LoggingSmokeTest, [switch]$PlankStorageSmokeTest, [switch]$RoutesSmokeTest, [switch]$WoodlandSmokeTest, [switch]$LakeReviewSmokeTest, [switch]$SmokeTest, [switch]$AudioSmokeTest, [switch]$HudSmokeTest, [switch]$CampaignSmokeTest, [switch]$MapSmokeTest, [switch]$ClearingSmokeTest, [switch]$MenuSmokeTest, [switch]$ArtSmokeTest, [switch]$CatalogSmokeTest, [switch]$ProductionSmokeTest, [switch]$RiverSmokeTest, [switch]$RenderingSmokeTest, [switch]$HomeSmokeTest, [switch]$FishingSmokeTest)
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
if ($CameraDragSmokeTest -or $ComfortSmokeTest -or $RenderIsolationSmokeTest -or $WatchSmokeTest -or $HudServicesSmokeTest -or $PantrySmokeTest -or $SurveySmokeTest -or $WildlifeSmokeTest -or $QuarrySmokeTest -or $SocialSmokeTest -or $FieldWorkSmokeTest -or $HandoffSmokeTest -or $LoggingSmokeTest -or $PlankStorageSmokeTest -or $RoutesSmokeTest -or $WoodlandSmokeTest -or $LakeReviewSmokeTest -or $FishingSmokeTest -or $HomeSmokeTest -or $RenderingSmokeTest -or $RiverSmokeTest -or $SmokeTest -or $AudioSmokeTest -or $HudSmokeTest -or $CampaignSmokeTest -or $MapSmokeTest -or $ClearingSmokeTest -or $MenuSmokeTest -or $ArtSmokeTest -or $CatalogSmokeTest -or $ProductionSmokeTest) {
    $engine = $engine.Replace('_win64.exe', '_win64_console.exe')
    $testArgument = if ($PantrySmokeTest) { '--pantry-smoke-test' } elseif ($SurveySmokeTest) { '--survey-smoke-test' } elseif ($WildlifeSmokeTest) { '--wildlife-smoke-test' } elseif ($QuarrySmokeTest) { '--quarry-smoke-test' } elseif ($SocialSmokeTest) { '--social-smoke-test' } elseif ($FieldWorkSmokeTest) { '--field-work-smoke-test' } elseif ($HandoffSmokeTest) { '--handoff-smoke-test' } elseif ($LoggingSmokeTest) { '--logging-smoke-test' } elseif ($PlankStorageSmokeTest) { '--plank-storage-smoke-test' } elseif ($RoutesSmokeTest) { '--routes-smoke-test' } elseif ($WoodlandSmokeTest) { '--woodland-smoke-test' } elseif ($LakeReviewSmokeTest -or $FishingSmokeTest) { '--fishing-smoke-test' } elseif ($HomeSmokeTest) { '--home-smoke-test' } elseif ($RenderingSmokeTest -or $RiverSmokeTest) { '--river-smoke-test' } elseif ($ProductionSmokeTest) { '--production-smoke-test' } elseif ($CatalogSmokeTest) { '--catalog-smoke-test' } elseif ($ArtSmokeTest) { '--art-smoke-test' } elseif ($MenuSmokeTest) { '--menu-smoke-test' } elseif ($ClearingSmokeTest) { '--clearing-smoke-test' } elseif ($MapSmokeTest) { '--map-smoke-test' } elseif ($CampaignSmokeTest) { '--campaign-smoke-test' } elseif ($HudSmokeTest) { '--hud-smoke-test' } elseif ($AudioSmokeTest) { '--audio-smoke-test' } else { '--smoke-test' }
    if ($LakeReviewSmokeTest) {
        & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj -- --lake
        if ($LASTEXITCODE -ne 0) { throw 'Lake review fixture failed' }
        & $engine --path $PSScriptRoot -- --fishing-smoke-test --lake-review
    }
    elseif ($CameraDragSmokeTest) { & $engine --path $PSScriptRoot -- --hud-smoke-test --camera-drag-only }
    elseif ($ComfortSmokeTest) { & $engine --path $PSScriptRoot -- --hud-smoke-test --comfort-only }
    elseif ($RenderIsolationSmokeTest) { & $engine --path $PSScriptRoot -- --pantry-smoke-test --render-isolation }
    elseif ($WatchSmokeTest) { & $engine --path $PSScriptRoot -- --hud-smoke-test --watch-only }
    elseif ($HudServicesSmokeTest) { & $engine --path $PSScriptRoot -- --hud-smoke-test --hud-services-only }
    elseif ($RenderingSmokeTest) { & $engine --path $PSScriptRoot -- --river-smoke-test --render-profile }
    else { & $engine --path $PSScriptRoot -- $testArgument }
    if ($LASTEXITCODE -ne 0) { throw 'Rendered smoke test failed' }
} else {
    & $engine --path $PSScriptRoot
}
