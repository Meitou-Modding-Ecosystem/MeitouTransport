param([string]$Dotnet = 'dotnet')
$ErrorActionPreference = 'Stop'
function Invoke-Checked([string]$Command, [string[]]$Arguments) {
    & $Command @Arguments
    if ($LASTEXITCODE -ne 0) { throw "$Command failed ($LASTEXITCODE): $Arguments" }
}
Push-Location (Split-Path $PSScriptRoot -Parent)
try {
    foreach ($project in 'framework/Meitou.Transport.csproj', 'tests/Meitou.Transport.Tests.csproj',
            'tests/LiveVoyage/LiveVoyage.csproj', 'tools/DesertRide/DesertRide.csproj') {
        Invoke-Checked $Dotnet @('build', $project, '-c', 'Release')
    }
    Invoke-Checked $Dotnet @('tests/bin/Release/net11.0/Meitou.Transport.Tests.dll')
    Invoke-Checked python @('tools/package.py', '--handoff')
    Invoke-Checked python @('tests/check_handoff.py')
} finally { Pop-Location }
