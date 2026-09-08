param([switch]$SmokeTest)
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
if ($SmokeTest) {
    $engine = $engine.Replace('_win64.exe', '_win64_console.exe')
    & $engine --path $PSScriptRoot -- --smoke-test
    if ($LASTEXITCODE -ne 0) { throw 'Rendered smoke test failed' }
} else {
    & $engine --path $PSScriptRoot
}
