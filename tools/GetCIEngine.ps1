# Managed build/test fixtures only; never launches the game.
param([Parameter(Mandatory)][string]$Output)
$ErrorActionPreference = 'Stop'
$version = '0.22.1'
$hash = 'e9d1c41330d0859902b35db450209f86a5cf5093c846545153c3eb131161375f'
New-Item -ItemType Directory -Force -Path $Output | Out-Null
$archive = Join-Path $Output "Meitou-$version-win-x64.zip"
if (-not (Test-Path -LiteralPath $archive)) {
    Invoke-WebRequest "https://github.com/VlcekM/MeitouClient/releases/download/v$version/Meitou-$version-win-x64.zip" -OutFile $archive
}
if ((Get-FileHash -LiteralPath $archive -Algorithm SHA256).Hash -ne $hash) { throw 'Meitou archive SHA-256 mismatch' }
$destination = Join-Path $Output $version
Expand-Archive -LiteralPath $archive -DestinationPath $destination -Force
$readers = Join-Path $destination "Meitou-$version-win-x64"
foreach ($assembly in 'Meitou.Core.dll', 'Meitou.Data.dll') {
    if (-not (Test-Path -LiteralPath (Join-Path $readers $assembly))) { throw "Missing $assembly" }
}
$env:MEITOU_DIR = (Resolve-Path -LiteralPath $readers).Path
if ($env:GITHUB_ENV) { "MEITOU_DIR=$env:MEITOU_DIR" | Out-File $env:GITHUB_ENV -Encoding utf8 -Append }
Write-Host "Verified Meitou $version fixtures"
