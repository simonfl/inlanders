param([switch]$OrchardSmokeTest, [switch]$SurveyKeyboardSmokeTest, [switch]$GoalsKeyboardSmokeTest, [switch]$EconomyKeyboardSmokeTest, [switch]$GroundColorSmokeTest, [switch]$CompositionSmokeTest, [switch]$FrameStallSmokeTest, [switch]$LargeVillageSmokeTest, [switch]$RestorationSmokeTest, [switch]$PeopleKeyboardSmokeTest, [switch]$SoundscapeSmokeTest, [switch]$CivicIdentitySmokeTest, [switch]$CatalogKeyboardSmokeTest, [switch]$MenuKeyboardSmokeTest, [switch]$DecorationBrushSmokeTest, [switch]$FenceSmokeTest, [switch]$CottageFinishSmokeTest, [switch]$RiverMeadowSmokeTest, [switch]$FinaleCampaignSmokeTest, [switch]$BreadSupplySmokeTest, [switch]$FinalePrototypeSmokeTest, [switch]$ShoreArtSmokeTest, [switch]$WoodsArtSmokeTest, [switch]$WoodsCampaignSmokeTest, [switch]$HallArtSmokeTest, [switch]$LooseStockSmokeTest, [switch]$QuarryCampaignSmokeTest, [switch]$GoalsSmokeTest, [switch]$ComfortReviewSmokeTest, [switch]$CameraDragSmokeTest, [switch]$ComfortSmokeTest, [switch]$RenderIsolationSmokeTest, [switch]$WatchSmokeTest, [switch]$HudServicesSmokeTest, [switch]$PantrySmokeTest, [switch]$SurveySmokeTest, [switch]$WildlifeSmokeTest, [switch]$QuarrySmokeTest, [switch]$SocialSmokeTest, [switch]$FieldWorkSmokeTest, [switch]$HandoffSmokeTest, [switch]$LoggingSmokeTest, [switch]$PlankStorageSmokeTest, [switch]$RoutesSmokeTest, [switch]$WoodlandSmokeTest, [switch]$LakeReviewSmokeTest, [switch]$SmokeTest, [switch]$AudioSmokeTest, [switch]$HudSmokeTest, [switch]$CampaignSmokeTest, [switch]$MapSmokeTest, [switch]$ClearingSmokeTest, [switch]$MenuSmokeTest, [switch]$ArtSmokeTest, [switch]$CatalogSmokeTest, [switch]$ProductionSmokeTest, [switch]$RiverSmokeTest, [switch]$RenderingSmokeTest, [switch]$HomeSmokeTest, [switch]$FishingSmokeTest)
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
if ($OrchardSmokeTest -or $SurveyKeyboardSmokeTest -or $GoalsKeyboardSmokeTest -or $EconomyKeyboardSmokeTest -or $GroundColorSmokeTest -or $CompositionSmokeTest -or $FrameStallSmokeTest -or $LargeVillageSmokeTest -or $RestorationSmokeTest -or $PeopleKeyboardSmokeTest -or $SoundscapeSmokeTest -or $CivicIdentitySmokeTest -or $CatalogKeyboardSmokeTest -or $MenuKeyboardSmokeTest -or $DecorationBrushSmokeTest -or $FenceSmokeTest -or $CottageFinishSmokeTest -or $RiverMeadowSmokeTest -or $FinaleCampaignSmokeTest -or $BreadSupplySmokeTest -or $FinalePrototypeSmokeTest -or $ShoreArtSmokeTest -or $WoodsArtSmokeTest -or $WoodsCampaignSmokeTest -or $HallArtSmokeTest -or $LooseStockSmokeTest -or $QuarryCampaignSmokeTest -or $GoalsSmokeTest -or $ComfortReviewSmokeTest -or $CameraDragSmokeTest -or $ComfortSmokeTest -or $RenderIsolationSmokeTest -or $WatchSmokeTest -or $HudServicesSmokeTest -or $PantrySmokeTest -or $SurveySmokeTest -or $WildlifeSmokeTest -or $QuarrySmokeTest -or $SocialSmokeTest -or $FieldWorkSmokeTest -or $HandoffSmokeTest -or $LoggingSmokeTest -or $PlankStorageSmokeTest -or $RoutesSmokeTest -or $WoodlandSmokeTest -or $LakeReviewSmokeTest -or $FishingSmokeTest -or $HomeSmokeTest -or $RenderingSmokeTest -or $RiverSmokeTest -or $SmokeTest -or $AudioSmokeTest -or $HudSmokeTest -or $CampaignSmokeTest -or $MapSmokeTest -or $ClearingSmokeTest -or $MenuSmokeTest -or $ArtSmokeTest -or $CatalogSmokeTest -or $ProductionSmokeTest) {
    $engine = $engine.Replace('_win64.exe', '_win64_console.exe')
    $testArgument = if ($PantrySmokeTest) { '--pantry-smoke-test' } elseif ($SurveySmokeTest) { '--survey-smoke-test' } elseif ($WildlifeSmokeTest) { '--wildlife-smoke-test' } elseif ($QuarrySmokeTest) { '--quarry-smoke-test' } elseif ($SocialSmokeTest) { '--social-smoke-test' } elseif ($FieldWorkSmokeTest) { '--field-work-smoke-test' } elseif ($HandoffSmokeTest) { '--handoff-smoke-test' } elseif ($LoggingSmokeTest) { '--logging-smoke-test' } elseif ($PlankStorageSmokeTest) { '--plank-storage-smoke-test' } elseif ($RoutesSmokeTest) { '--routes-smoke-test' } elseif ($WoodlandSmokeTest) { '--woodland-smoke-test' } elseif ($LakeReviewSmokeTest -or $FishingSmokeTest) { '--fishing-smoke-test' } elseif ($HomeSmokeTest) { '--home-smoke-test' } elseif ($RenderingSmokeTest -or $RiverSmokeTest) { '--river-smoke-test' } elseif ($ProductionSmokeTest) { '--production-smoke-test' } elseif ($CatalogSmokeTest) { '--catalog-smoke-test' } elseif ($ArtSmokeTest) { '--art-smoke-test' } elseif ($MenuSmokeTest) { '--menu-smoke-test' } elseif ($ClearingSmokeTest) { '--clearing-smoke-test' } elseif ($MapSmokeTest) { '--map-smoke-test' } elseif ($CampaignSmokeTest) { '--campaign-smoke-test' } elseif ($HudSmokeTest) { '--hud-smoke-test' } elseif ($AudioSmokeTest) { '--audio-smoke-test' } else { '--smoke-test' }
    if ($OrchardSmokeTest) { & $engine --path $PSScriptRoot -- --hud-smoke-test --orchard }
    elseif ($SurveyKeyboardSmokeTest) { & $engine --path $PSScriptRoot -- --hud-smoke-test --survey-keyboard }
    elseif ($GoalsKeyboardSmokeTest) { & $engine --path $PSScriptRoot -- --hud-smoke-test --goals-keyboard }
    elseif ($EconomyKeyboardSmokeTest) { & $engine --path $PSScriptRoot -- --hud-smoke-test --economy-keyboard }
    elseif ($GroundColorSmokeTest) { & $engine --path $PSScriptRoot -- --hud-smoke-test --composition --ground-color }
    elseif ($CompositionSmokeTest) { & $engine --path $PSScriptRoot -- --hud-smoke-test --composition }
    elseif ($FrameStallSmokeTest) { & $engine --path $PSScriptRoot -- --hud-smoke-test --frame-stalls }
    elseif ($LargeVillageSmokeTest) { & $engine --path $PSScriptRoot -- --hud-smoke-test --large-village }
    elseif ($RestorationSmokeTest) { & $engine --path $PSScriptRoot -- --hud-smoke-test --restoration-views }
    elseif ($PeopleKeyboardSmokeTest) { & $engine --path $PSScriptRoot -- --hud-smoke-test --people-keyboard }
    elseif ($SoundscapeSmokeTest) { & $engine --path $PSScriptRoot -- --audio-smoke-test --soundscape }
    elseif ($CivicIdentitySmokeTest) { & $engine --path $PSScriptRoot -- --hud-smoke-test --civic-identities }
    elseif ($CatalogKeyboardSmokeTest) { & $engine --path $PSScriptRoot -- --hud-smoke-test --catalog-keyboard }
    elseif ($MenuKeyboardSmokeTest) { & $engine --path $PSScriptRoot -- --menu-smoke-test --menu-keyboard }
    elseif ($DecorationBrushSmokeTest) { & $engine --path $PSScriptRoot -- --hud-smoke-test --decoration-brush }
    elseif ($FenceSmokeTest) { & $engine --path $PSScriptRoot -- --hud-smoke-test --fences }
    elseif ($CottageFinishSmokeTest) { & $engine --path $PSScriptRoot -- --hud-smoke-test --cottage-finishes }
    elseif ($FinaleCampaignSmokeTest -or $RiverMeadowSmokeTest) {
        & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj -- --finale-campaign
        if ($LASTEXITCODE -ne 0) { throw "Finale campaign routes failed" }
        if ($RiverMeadowSmokeTest) { & $engine --path $PSScriptRoot -- --hud-smoke-test --river-meadow }
        else { & $engine --path $PSScriptRoot -- --hud-smoke-test --finale-campaign }
    }
    elseif ($BreadSupplySmokeTest -or $FinalePrototypeSmokeTest) {
        & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj -- --finale-decision
        if ($LASTEXITCODE -ne 0) { throw 'Finale prototype routes failed' }
        if ($BreadSupplySmokeTest) { & $engine --path $PSScriptRoot -- --hud-smoke-test --bread-supply }
        else { & $engine --path $PSScriptRoot -- --hud-smoke-test --finale-prototype }
    }
    elseif ($ShoreArtSmokeTest) { & $engine --path $PSScriptRoot -- --hud-smoke-test --shore-art }
    elseif ($WoodsCampaignSmokeTest -or $WoodsArtSmokeTest) {
        & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj -- --woods-campaign
        if ($LASTEXITCODE -ne 0) { throw 'Woodland campaign fixtures failed' }
        if ($WoodsArtSmokeTest) { & $engine --path $PSScriptRoot -- --hud-smoke-test --woods-art }
        else { & $engine --path $PSScriptRoot -- --hud-smoke-test --woods-campaign }
    }
    elseif ($LooseStockSmokeTest) { & $engine --path $PSScriptRoot -- --logging-smoke-test --loose-stock }
    elseif ($QuarryCampaignSmokeTest -or $HallArtSmokeTest) {
        & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj -- --quarry-campaign
        if ($LASTEXITCODE -ne 0) { throw 'Quarry campaign fixtures failed' }
        if ($HallArtSmokeTest) { & $engine --path $PSScriptRoot -- --hud-smoke-test --hall-art }
        else { & $engine --path $PSScriptRoot -- --hud-smoke-test --quarry-campaign }
    }
    elseif ($LakeReviewSmokeTest) {
        & "$env:DOTNET_ROOT\dotnet.exe" run --project Tests/SimulationTests.csproj -- --lake
        if ($LASTEXITCODE -ne 0) { throw 'Lake review fixture failed' }
        & $engine --path $PSScriptRoot -- --fishing-smoke-test --lake-review
    }
    elseif ($GoalsSmokeTest) { & $engine --path $PSScriptRoot -- --hud-smoke-test --goals-only }
    elseif ($ComfortReviewSmokeTest) { & $engine --path $PSScriptRoot -- --hud-smoke-test --comfort-review }
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
