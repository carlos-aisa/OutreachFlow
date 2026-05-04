[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [Parameter(Mandatory = $true)]
    [string]$OutputPath,

    [string]$RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot "../..")).Path
)

$ErrorActionPreference = "Stop"

$changelogPath = Join-Path $RepositoryRoot "CHANGELOG.md"
$lines = Get-Content $changelogPath
$heading = "## [$Version]"
$startIndex = -1

for ($index = 0; $index -lt $lines.Count; $index++) {
    if ($lines[$index].StartsWith($heading, [StringComparison]::Ordinal)) {
        $startIndex = $index
        break
    }
}

if ($startIndex -lt 0) {
    throw "CHANGELOG.md does not contain a release section for version '$Version'."
}

$endIndex = $lines.Count
for ($index = $startIndex + 1; $index -lt $lines.Count; $index++) {
    if ($lines[$index] -match '^## \[') {
        $endIndex = $index
        break
    }
}

$sectionLines = $lines[$startIndex..($endIndex - 1)]
$outputDirectory = Split-Path -Parent $OutputPath

if ($outputDirectory -and -not (Test-Path $outputDirectory)) {
    New-Item -ItemType Directory -Path $outputDirectory | Out-Null
}

Set-Content -Path $OutputPath -Value ($sectionLines -join [Environment]::NewLine)
Write-Output "Wrote changelog section for version '$Version' to '$OutputPath'."
