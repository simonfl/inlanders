param([ValidateSet('Build','Start','Stop','Status')][string]$Action = 'Start')
$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot
$env:DOTNET_CLI_HOME = Join-Path $PSScriptRoot '.tools/dotnet-home'
$env:APPDATA = Join-Path $PSScriptRoot '.tools/appdata'
$runtime = Join-Path $PSScriptRoot '.tools/dotnet/dotnet.exe'
$server = Join-Path $PSScriptRoot 'Browser/Server/bin/Debug/net8.0/BrowserServer.dll'
$browserArtifacts = Join-Path $PSScriptRoot 'artifacts/browser'
New-Item -ItemType Directory -Force $browserArtifacts | Out-Null
$pidFile = Join-Path $browserArtifacts 'server.pid'
if ($Action -eq 'Build') {
    if (Test-Path $pidFile) { $serverProcess = Get-Process -Id ([int](Get-Content $pidFile)) -ErrorAction SilentlyContinue; if ($serverProcess) { throw 'Stop the browser server before rebuilding its running build.' } }
    Push-Location Browser/client
    try { node ../../.tools/npm/package/bin/npm-cli.js run build; if ($LASTEXITCODE -ne 0) { throw 'Browser client build failed.' } } finally { Pop-Location }
    & $runtime build Browser/Server/BrowserServer.csproj --nologo
    if ($LASTEXITCODE -ne 0) { throw 'Browser service build failed.' }
} elseif ($Action -eq 'Start') {
    if (Test-Path $pidFile) { $serverProcess = Get-Process -Id ([int](Get-Content $pidFile)) -ErrorAction SilentlyContinue; if ($serverProcess) { Write-Output 'Browser service already running at http://127.0.0.1:5180'; exit } }
    if (!(Test-Path $server)) { throw 'Build the browser prototype first: ./Browser.ps1 Build' }
    $serverProcess = Start-Process -FilePath $runtime -ArgumentList ('"' + $server + '"') -WorkingDirectory $PSScriptRoot -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $browserArtifacts 'server.log') -RedirectStandardError (Join-Path $browserArtifacts 'server-errors.log')
    Set-Content $pidFile $serverProcess.Id
    $ready = $false
    for ($attempt = 0; $attempt -lt 30; $attempt++) {
        try { $health = Invoke-RestMethod http://127.0.0.1:5180/api/health -TimeoutSec 1; if ($health.status -eq 'ready') { $ready = $true; break } } catch { Start-Sleep -Milliseconds 200 }
    }
    if (!$ready) { throw 'The browser service did not become ready. See artifacts/browser/server-errors.log.' }
    Write-Output 'Started browser service: http://127.0.0.1:5180'
} elseif ($Action -eq 'Stop') {
    if (Test-Path $pidFile) {
        $serverPid = [int](Get-Content $pidFile)
        $candidate = Get-CimInstance Win32_Process -Filter "ProcessId = $serverPid" -ErrorAction SilentlyContinue
        if ($candidate -and $candidate.CommandLine.Contains($server)) { Stop-Process -Id $serverPid; Wait-Process -Id $serverPid -Timeout 5 -ErrorAction SilentlyContinue }
        elseif ($candidate) { throw 'PID belongs to another process; refusing to stop it.' }
        Remove-Item -LiteralPath $pidFile
    }
} else { Invoke-RestMethod http://127.0.0.1:5180/api/health }
