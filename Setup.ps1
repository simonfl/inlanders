# Optional: restores the portable tools if .tools has been removed.
$ErrorActionPreference = 'Stop'
Set-Location -LiteralPath $PSScriptRoot
New-Item -ItemType Directory -Force .tools | Out-Null
Set-Content -LiteralPath .tools/.gdignore -Value ''
$engine = '.tools/godot/Godot_v4.6-stable_mono_win64/Godot_v4.6-stable_mono_win64.exe'
if (!(Test-Path -LiteralPath $engine)) {
    Invoke-WebRequest -UseBasicParsing -Uri 'https://github.com/godotengine/godot/releases/download/4.6-stable/Godot_v4.6-stable_mono_win64.zip' -OutFile .tools/godot.zip
    Expand-Archive -LiteralPath .tools/godot.zip -DestinationPath .tools/godot -Force
}
if (!(Test-Path -LiteralPath .tools/dotnet/dotnet.exe)) {
    Invoke-WebRequest -UseBasicParsing -Uri 'https://dot.net/v1/dotnet-install.ps1' -OutFile .tools/dotnet-install.ps1
    & ./.tools/dotnet-install.ps1 -Version '8.0.424' -InstallDir (Join-Path $PSScriptRoot '.tools/dotnet') -NoPath
    if (!(Test-Path -LiteralPath .tools/dotnet/dotnet.exe)) { throw 'SDK installation failed' }
}
Write-Host 'Ready. Double-click Play.cmd.'
