param(
    [Parameter(Mandatory = $true)] [string] $ProjectDirectory,
    [Parameter(Mandatory = $true)] [string] $GameDirectory,
    [Parameter(Mandatory = $true)] [string] $OutputDirectory
)

$ErrorActionPreference = 'Stop'
& (Join-Path $ProjectDirectory 'tools\Validate-Package.ps1') `
    -ProjectDirectory $ProjectDirectory -GameDirectory $GameDirectory | Out-Host

$manifest = Get-Content -LiteralPath (Join-Path $ProjectDirectory 'modinfo.json') -Raw | ConvertFrom-Json
$packageBaseName = 'botany-discovery-' + $manifest.version

New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
$staging = Join-Path ([IO.Path]::GetTempPath()) ('botany-discovery-' + [guid]::NewGuid().ToString('N'))
$archive = Join-Path $OutputDirectory ($packageBaseName + '.zip')
$package = Join-Path $OutputDirectory ($packageBaseName + '.ghmod')
$sourceArchive = Join-Path $OutputDirectory ($packageBaseName + '-source.zip')

try {
    New-Item -ItemType Directory -Path $staging | Out-Null
    Get-ChildItem -LiteralPath $ProjectDirectory -File | Where-Object {
        $_.Extension -in @('.cs', '.json', '.txt', '.png', '.jpg')
    } | Copy-Item -Destination $staging

    if (Test-Path -LiteralPath $archive) { Remove-Item -LiteralPath $archive -Force }
    if (Test-Path -LiteralPath $package) { Remove-Item -LiteralPath $package -Force }
    if (Test-Path -LiteralPath $sourceArchive) { Remove-Item -LiteralPath $sourceArchive -Force }

    Compress-Archive -Path (Join-Path $staging '*') -DestinationPath $archive -CompressionLevel Optimal
    Move-Item -LiteralPath $archive -Destination $package

    $sourceItems = Get-ChildItem -LiteralPath $ProjectDirectory -Force | Where-Object {
        $_.Name -notin @('.git', 'outputs')
    }
    Compress-Archive -Path $sourceItems.FullName -DestinationPath $sourceArchive -CompressionLevel Optimal
} finally {
    if (Test-Path -LiteralPath $staging) { Remove-Item -LiteralPath $staging -Recurse -Force }
}

$hash = Get-FileHash -LiteralPath $package -Algorithm SHA256
[pscustomobject]@{
    Package = $package
    PackageBytes = (Get-Item -LiteralPath $package).Length
    Source = $sourceArchive
    Sha256 = $hash.Hash
}
