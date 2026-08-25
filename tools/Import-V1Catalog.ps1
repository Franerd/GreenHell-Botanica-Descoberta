param(
    [Parameter(Mandatory = $true)] [string] $V1Directory,
    [Parameter(Mandatory = $true)] [string] $OutputPath
)

$catalogSource = Get-Content -LiteralPath (Join-Path $V1Directory 'BotanicaCatalog.cs') -Raw
$localizationSource = Get-Content -LiteralPath (Join-Path $V1Directory 'BotanicaLocalization.cs') -Raw
$metadata = Get-Content -LiteralPath (Join-Path $V1Directory 'catalogo-botanica-descoberta.json') -Raw | ConvertFrom-Json

function Read-LocalizedBlock([string] $source, [string] $startMarker, [string] $endMarker) {
    $start = $source.IndexOf($startMarker, [StringComparison]::Ordinal)
    if ($start -lt 0) { throw "Localization block not found: $startMarker" }
    $end = if ([string]::IsNullOrEmpty($endMarker)) { $source.Length } else { $source.IndexOf($endMarker, $start, [StringComparison]::Ordinal) }
    if ($end -lt 0) { throw "Localization block terminator not found: $endMarker" }
    $block = $source.Substring($start, $end - $start)
    $map = [ordered]@{}
    foreach ($match in [regex]::Matches($block, '\{\s*"([^"]+)",\s*"([^"]*)"\s*\}')) {
        $map[$match.Groups[1].Value] = $match.Groups[2].Value
    }
    return $map
}

$english = Read-LocalizedBlock $localizationSource 'private static readonly Dictionary<string, string> English' 'private static readonly Dictionary<string, string> Spanish'
$spanish = Read-LocalizedBlock $localizationSource 'private static readonly Dictionary<string, string> Spanish' 'internal static string GetCommonName'

$items = [ordered]@{}
foreach ($match in [regex]::Matches($catalogSource, '\{\s*"([^"]+)",\s*E\("([^"]*)",\s*"([^"]*)"\)\s*\}')) {
    $id = $match.Groups[1].Value
    $items[$id] = [ordered]@{
        id = $id
        scientific = $match.Groups[3].Value
        names = [ordered]@{
            'pt-BR' = $match.Groups[2].Value
            en = $english[$id]
            es = $spanish[$id]
        }
    }
}

$species = @()
foreach ($sourceSpecies in $metadata.especies) {
    $speciesItems = @()
    foreach ($property in $sourceSpecies.itens.PSObject.Properties) {
        if (-not $items.Contains($property.Name)) { throw "Metadata contains unknown ItemID: $($property.Name)" }
        $speciesItems += [pscustomobject]$items[$property.Name]
        $items.Remove($property.Name)
    }
    $species += [ordered]@{
        key = $sourceSpecies.chave
        commonNamePtBr = $sourceSpecies.nomeComum
        scientificName = $sourceSpecies.nomeCientifico
        confidence = $sourceSpecies.confianca
        taxonomicSynonym = if ($sourceSpecies.sinonimoTaxonomico) { $sourceSpecies.sinonimoTaxonomico } else { '' }
        gameIdentification = if ($sourceSpecies.identificacaoDoJogo) { $sourceSpecies.identificacaoDoJogo } else { '' }
        items = $speciesItems
    }
}

if ($items.Count -ne 0) {
    throw "Items without species metadata: $([string]::Join(', ', [string[]]$items.Keys))"
}

$result = [ordered]@{
    schema = 2
    catalogVersion = '2.0.0'
    gameVersion = '2.9.5'
    defaultLanguage = 'en'
    languages = @('pt-BR', 'en', 'es')
    species = $species
}

$json = $result | ConvertTo-Json -Depth 12
[IO.File]::WriteAllText($OutputPath, $json + [Environment]::NewLine, [Text.UTF8Encoding]::new($false))
