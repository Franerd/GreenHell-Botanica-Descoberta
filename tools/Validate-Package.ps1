param(
    [Parameter(Mandatory = $true)] [string] $ProjectDirectory,
    [Parameter(Mandatory = $true)] [string] $GameDirectory
)

$ErrorActionPreference = 'Stop'
$catalogPath = Join-Path $ProjectDirectory 'botany-catalog.json'
$catalog = Get-Content -LiteralPath $catalogPath -Raw | ConvertFrom-Json
if ($catalog.schema -ne 2) { throw 'Catalog schema must be 2.' }
if ($catalog.species.Count -ne 27) { throw "Expected 27 species/groups; found $($catalog.species.Count)." }

$seen = [Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
$allowedConfidence = @('alta', 'media_alta', 'media', 'alta_para_genero', 'baixa_para_especie')
$count = 0
foreach ($species in $catalog.species) {
    if ([string]::IsNullOrWhiteSpace($species.key)) { throw 'Species key is missing.' }
    if ([string]::IsNullOrWhiteSpace($species.confidence)) { throw "Confidence missing: $($species.key)" }
    if ($species.confidence -notin $allowedConfidence) { throw "Unknown confidence value: $($species.confidence)" }
    foreach ($item in $species.items) {
        if (-not $seen.Add([string]$item.id)) { throw "Duplicate ItemID: $($item.id)" }
        if ([string]::IsNullOrWhiteSpace($item.scientific)) { throw "Scientific name missing: $($item.id)" }
        if ($item.scientific -ne $species.scientificName) { throw "Scientific name differs from species record: $($item.id)" }
        foreach ($language in $catalog.languages) {
            if ([string]::IsNullOrWhiteSpace($item.names.$language)) { throw "Translation $language missing: $($item.id)" }
        }
        $count++
    }
}
if ($count -ne 89) { throw "Expected 89 ItemIDs; found $count." }

$temporaryGenerated = Join-Path ([IO.Path]::GetTempPath()) ('BotanicaCatalog.Generated.' + [guid]::NewGuid().ToString('N') + '.cs')
try {
    & (Join-Path $ProjectDirectory 'tools\Generate-Catalog.ps1') -CatalogPath $catalogPath -OutputPath $temporaryGenerated
    $expected = [IO.File]::ReadAllText($temporaryGenerated)
    $actual = [IO.File]::ReadAllText((Join-Path $ProjectDirectory 'BotanicaCatalog.Generated.cs'))
    if ($expected -cne $actual) { throw 'Generated catalog is stale. Run Generate-Catalog.ps1.' }
} finally {
    if (Test-Path -LiteralPath $temporaryGenerated) { Remove-Item -LiteralPath $temporaryGenerated -Force }
}

$manifest = Get-Content -LiteralPath (Join-Path $ProjectDirectory 'modinfo.json') -Raw | ConvertFrom-Json
if ([string]$manifest.version -notmatch '^\d+\.\d+\.\d+$') { throw 'Manifest version must use semantic versioning.' }
$entrySource = Get-Content -LiteralPath (Join-Path $ProjectDirectory 'BotanicaDescoberta.cs') -Raw
$versionPattern = 'private const string Version = "' + [regex]::Escape([string]$manifest.version) + '";'
if ($entrySource -notmatch $versionPattern) { throw 'Manifest and runtime versions must match.' }
if ($manifest.gameVersion -ne '2.9.5') { throw 'Manifest gameVersion must be 2.9.5.' }

$managed = Join-Path $GameDirectory 'GH_Data\Managed'
$compiler = 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe'
$compileOutput = Join-Path ([IO.Path]::GetTempPath()) ('BotanyDiscovery.Validation.' + [guid]::NewGuid().ToString('N') + '.dll')
$sources = Get-ChildItem -LiteralPath $ProjectDirectory -File -Filter '*.cs' | ForEach-Object FullName
$sources += Join-Path $ProjectDirectory 'tools\CompileStubs.cs'
$references = @(
    (Join-Path $managed 'Assembly-CSharp.dll'),
    (Join-Path $managed 'UnityEngine.dll'),
    (Join-Path $managed 'UnityEngine.CoreModule.dll'),
    (Join-Path $managed 'UnityEngine.UIModule.dll'),
    (Join-Path $managed 'UnityEngine.TextRenderingModule.dll'),
    (Join-Path $managed 'UnityEngine.UI.dll')
)
$arguments = @('/nologo', '/target:library', ('/out:' + $compileOutput))
$arguments += $references | ForEach-Object { '/reference:' + $_ }
$arguments += $sources
try {
    & $compiler $arguments
    if ($LASTEXITCODE -ne 0) { throw "C# validation compilation failed with exit code $LASTEXITCODE." }
} finally {
    if (Test-Path -LiteralPath $compileOutput) { Remove-Item -LiteralPath $compileOutput -Force }
}

[pscustomobject]@{
    Schema = $catalog.schema
    Species = $catalog.species.Count
    ItemIDs = $count
    Languages = $catalog.languages.Count
    ModVersion = $manifest.version
    GeneratedFresh = $true
    CompileValidation = $true
}
