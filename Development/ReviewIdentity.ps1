# Explicit conservative boundaries, not a source dependency graph.
function Get-ReviewFingerprint {
    param([string]$Root, [string]$Toolchain, [switch]$Fixture, [string]$Generator='')
    $Root=[IO.Path]::GetFullPath($Root).TrimEnd([char[]]'\/')
    $rootExtensions=if($Fixture){@('.csproj','.props','.targets','.config')}else{@('.cs','.csproj','.props','.targets','.config','.godot','.tscn','.ps1')}
    $inputs=@(Get-ChildItem -LiteralPath $Root -File | Where-Object { $_.Extension -in $rootExtensions -or $_.Name -in @('global.json','packages.lock.json') })
    foreach($folder in @('Simulation','Tests','Development')) {
        $inputs+=Get-ChildItem -LiteralPath (Join-Path $Root $folder) -Recurse -File |
            Where-Object {
                $_.FullName -notmatch '[\\/](bin|obj)[\\/]' -and
                $_.Extension -in @('.cs','.csproj','.props','.targets','.config','.json','.ps1') -and
                (-not $Fixture -or ($_.Extension -ne '.ps1' -and $_.FullName -ne (Join-Path $Root 'Development/review-scenarios.json')) -or $_.Name -eq 'ReviewIdentity.ps1')
            }
    }
    $lines=@("identity-v2;fixture=$($Fixture.IsPresent);sdk=$Toolchain;generator=$Generator")+
        @($inputs | Sort-Object FullName | ForEach-Object { $_.FullName.Substring($Root.Length).Replace('\','/')+':'+(Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash })
    $sha=[Security.Cryptography.SHA256]::Create()
    try { ([BitConverter]::ToString($sha.ComputeHash([Text.Encoding]::UTF8.GetBytes(($lines -join "`n"))))).Replace('-','') }
    finally { $sha.Dispose() }
}
