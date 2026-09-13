$ErrorActionPreference='Stop'
Set-StrictMode -Version Latest
. (Join-Path $PSScriptRoot 'ReviewIdentity.ps1')
$scratch=Join-Path (Split-Path $PSScriptRoot) ('artifacts/review-identity-tests/'+[Guid]::NewGuid().ToString('N'))
foreach($dir in @('Simulation','Tests/obj','Development')){[IO.Directory]::CreateDirectory((Join-Path $scratch $dir)) | Out-Null}
function Put([string]$Name,[string]$Text){[IO.File]::WriteAllText((Join-Path $scratch $Name),$Text)}
foreach($name in @('Hud.cs','Simulation/World.cs','Tests/Generator.cs','Tests/SimulationTests.csproj','Development/ReviewWorlds.cs','Development/review-scenarios.json','Development/ReviewIdentity.ps1','NuGet.Config','global.json','Directory.Build.props','Review.ps1')){Put $name 'original'}
function Identity([switch]$Fixture,[string]$Generator='gathering',[string]$Sdk='8.0.424') {
    Get-ReviewFingerprint -Root $scratch -Toolchain $Sdk -Fixture:$Fixture -Generator $Generator
}
$full=Identity;$fixture=Identity -Fixture
foreach($case in @(
    @('Hud.cs',$false),@('Review.ps1',$false),@('Development/review-scenarios.json',$false),
    @('Simulation/World.cs',$true),@('Tests/Generator.cs',$true),@('Tests/SimulationTests.csproj',$true),
    @('Development/ReviewWorlds.cs',$true),@('Development/ReviewIdentity.ps1',$true),@('NuGet.Config',$true),@('global.json',$true),@('Directory.Build.props',$true))) {
    Put $case[0] 'edited'
    if((Identity) -eq $full -or (((Identity -Fixture) -ne $fixture) -ne $case[1])){throw "Wrong invalidation: $($case[0])"}
    Put $case[0] 'original'
}
Put 'Tests/obj/generated.cs' 'ignored'
if((Identity) -ne $full -or (Identity -Fixture) -ne $fixture){throw 'Generated build output invalidates identity'}
if((Identity -Fixture -Generator 'dense') -eq $fixture -or (Identity -Fixture -Sdk '8.0.999') -eq $fixture){throw 'Generator/SDK identity ignored'}
Put 'Simulation/NewRule.cs' 'new file'
if((Identity -Fixture) -eq $fixture){throw 'New simulation input ignored'}
Remove-Item -LiteralPath (Join-Path $scratch 'Simulation/NewRule.cs')
if((Identity -Fixture) -ne $fixture){throw 'Identity did not restore after removing added input'}
Write-Output 'PASS: UI/catalog/launcher separation; simulation/generator/config/SDK/contract invalidation; added inputs and ignored build output.'
