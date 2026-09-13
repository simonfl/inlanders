# Fail before MSBuild's repeated copy retries when an earlier local run owns a binary.
# This is a contention check, not a scheduler: callers still sequence build and capture.
function Assert-InlandersBuildIdle([string]$Root) {
    foreach($relative in @('.godot/mono/temp/bin/Debug/Inlanders.dll','Tests/bin/Debug/net8.0/SimulationTests.dll','Tests/bin/Debug/net8.0/SimulationTests.exe')) {
        $binary=Join-Path $Root $relative
        if(-not (Test-Path -LiteralPath $binary)){continue}
        try {
            $probe=[IO.File]::Open($binary,[IO.FileMode]::Open,[IO.FileAccess]::ReadWrite,[IO.FileShare]::None)
            $probe.Dispose()
        } catch {
            throw "Build postponed: $relative is in use. Let the existing simulation/capture finish, or close the running game, then retry. No process was stopped."
        }
    }
}
