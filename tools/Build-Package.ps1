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
    $runtimeFiles = @(
        'BotanicaCatalog.cs',
        'BotanicaCatalog.Generated.cs',
        'BotanicaDescoberta.cs',
        'BotanicaLocalization.cs',
        'BotanicaRuntime.cs',
        'BotanicaSettings.cs',
        'modinfo.json',
        'icon.png',
        'banner.jpg'
    )
    foreach ($runtimeFile in $runtimeFiles) {
        $runtimePath = Join-Path $ProjectDirectory $runtimeFile
        if (-not (Test-Path -LiteralPath $runtimePath -PathType Leaf)) {
            throw "Required runtime file is missing: $runtimeFile"
        }
        Copy-Item -LiteralPath $runtimePath -Destination $staging
    }

    if (Test-Path -LiteralPath $archive) { Remove-Item -LiteralPath $archive -Force }
    if (Test-Path -LiteralPath $package) { Remove-Item -LiteralPath $package -Force }
    if (Test-Path -LiteralPath $sourceArchive) { Remove-Item -LiteralPath $sourceArchive -Force }

    Compress-Archive -Path (Join-Path $staging '*') -DestinationPath $archive -CompressionLevel Optimal
    Move-Item -LiteralPath $archive -Destination $package

    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $packageArchive = [IO.Compression.ZipFile]::OpenRead($package)
    try {
        $packagedFiles = @($packageArchive.Entries | ForEach-Object FullName | Sort-Object)
        $expectedFiles = @($runtimeFiles | Sort-Object)
        if (Compare-Object $expectedFiles $packagedFiles) {
            throw 'Runtime package contents differ from the approved file list.'
        }
    } finally {
        $packageArchive.Dispose()
    }

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
