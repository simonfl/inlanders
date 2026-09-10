$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
Push-Location $projectRoot
try {
    $baselineDir = Join-Path $projectRoot 'artifacts/f07c1-baseline'
    $baselineZip = Join-Path $projectRoot 'artifacts/f07c1-simulation.zip'
    New-Item -ItemType Directory -Force (Join-Path $projectRoot 'artifacts') | Out-Null
    & git -c "safe.directory=$($projectRoot.Replace('\','/'))" archive --format=zip "--output=$baselineZip" 634095b Simulation
    if ($LASTEXITCODE -ne 0) { throw 'Could not extract the pre-integration simulation' }
    Expand-Archive -LiteralPath $baselineZip -DestinationPath $baselineDir -Force
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'Experiments/InstantMealBaseline.cs.txt') -Destination (Join-Path $baselineDir 'Program.cs')
    Set-Content -LiteralPath (Join-Path $baselineDir 'Baseline.csproj') -Value '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup></Project>'
    $env:DOTNET_ROOT = Join-Path $projectRoot '.tools/dotnet'
    $env:DOTNET_CLI_HOME = Join-Path $projectRoot '.tools/dotnet-home'
    $env:APPDATA = Join-Path $projectRoot '.tools/appdata'
    & (Join-Path $env:DOTNET_ROOT 'dotnet.exe') run --project (Join-Path $baselineDir 'Baseline.csproj')
    if ($LASTEXITCODE -ne 0) { throw 'Instant-meal comparison failed' }
} finally { Pop-Location }
