[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$SummaryPath,

    [Parameter(Mandatory = $true)]
    [string]$OutputPath
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $SummaryPath)) {
    throw "Coverage summary file was not found at '$SummaryPath'."
}

$summary = Get-Content -Raw $SummaryPath | ConvertFrom-Json
$lineCoverage = [decimal]$summary.summary.linecoverage
$coverageText = "{0:0.#}%" -f $lineCoverage

$color = "red"
if ($lineCoverage -ge 90) {
    $color = "brightgreen"
}
elseif ($lineCoverage -ge 80) {
    $color = "green"
}
elseif ($lineCoverage -ge 70) {
    $color = "yellowgreen"
}
elseif ($lineCoverage -ge 60) {
    $color = "yellow"
}
elseif ($lineCoverage -ge 50) {
    $color = "orange"
}

$badge = [ordered]@{
    schemaVersion = 1
    label = "coverage"
    message = $coverageText
    color = $color
}

$outputDirectory = Split-Path -Parent $OutputPath
if ($outputDirectory -and -not (Test-Path $outputDirectory)) {
    New-Item -ItemType Directory -Path $outputDirectory | Out-Null
}

$badge | ConvertTo-Json | Set-Content -Path $OutputPath
Write-Output "Coverage badge updated to $coverageText."
