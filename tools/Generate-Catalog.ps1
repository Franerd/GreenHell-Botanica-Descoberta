param(
    [Parameter(Mandatory = $true)] [string] $CatalogPath,
    [Parameter(Mandatory = $true)] [string] $OutputPath
)

$catalog = Get-Content -LiteralPath $CatalogPath -Raw | ConvertFrom-Json
if ($catalog.schema -ne 2) { throw "Unsupported catalog schema: $($catalog.schema)" }

function Escape-CSharp([string] $value) {
    if ($null -eq $value) { return '' }
    return $value.Replace('\', '\\').Replace('"', '\"').Replace("`r", '').Replace("`n", '\n')
}

$lines = [Collections.Generic.List[string]]::new()
$lines.Add('// Generated from botany-catalog.json. Do not edit by hand.')
$lines.Add('internal static partial class BotanicaCatalog {')
$lines.Add('    private static void PopulateGeneratedCatalog() {')

$seen = [Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
$count = 0
foreach ($species in $catalog.species) {
    foreach ($item in $species.items) {
        if (-not $seen.Add([string]$item.id)) { throw "Duplicate ItemID: $($item.id)" }
        foreach ($language in $catalog.languages) {
            if ([string]::IsNullOrWhiteSpace([string]$item.names.$language)) {
                throw "Missing $language name for ItemID: $($item.id)"
            }
        }
        if ([string]::IsNullOrWhiteSpace([string]$item.scientific)) {
            throw "Missing scientific name for ItemID: $($item.id)"
        }

        $values = @(
            $item.id,
            $item.names.'pt-BR',
            $item.names.en,
            $item.names.es,
            $item.scientific,
            $species.key,
            $species.confidence,
            $species.taxonomicSynonym,
            $species.gameIdentification
        ) | ForEach-Object { Escape-CSharp ([string]$_) }

        $lines.Add(('        Add(new Entry("{0}", "{1}", "{2}", "{3}", "{4}", "{5}", "{6}", "{7}", "{8}"));' -f $values))
        $count++
    }
}

if ($count -ne 90) { throw "Expected 90 items, found $count" }
$lines.Add('    }')
$lines.Add('}')

[IO.File]::WriteAllLines($OutputPath, $lines, [Text.UTF8Encoding]::new($false))
