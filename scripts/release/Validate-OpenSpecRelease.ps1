[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$ChangeId,

    [Parameter(Mandatory = $true)]
    [string]$Version,

    [string]$RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot "../..")).Path
)

$ErrorActionPreference = "Stop"

if ($ChangeId -notmatch '^p\d{2}-[a-z0-9]+(-[a-z0-9]+)*$') {
    throw "Change id '$ChangeId' must use the format p01-change-name."
}

if ($Version -notmatch '^\d+\.\d+\.\d+(-[0-9A-Za-z.-]+)?$') {
    throw "Version '$Version' must be a SemVer value such as 0.2.0."
}

$archiveRoot = Join-Path $RepositoryRoot "openspec/changes/archive"
if (-not (Test-Path $archiveRoot)) {
    throw "OpenSpec archive directory was not found at '$archiveRoot'."
}

$archivedChanges = Get-ChildItem -Path $archiveRoot -Directory -Filter "*-$ChangeId"
if ($archivedChanges.Count -eq 0) {
    throw "Archived OpenSpec change '$ChangeId' was not found under '$archiveRoot'."
}

$versionPath = Join-Path $RepositoryRoot "VERSION"
$currentVersion = (Get-Content -Raw $versionPath).Trim()
if ($currentVersion -ne $Version) {
    throw "VERSION contains '$currentVersion' but release input was '$Version'."
}

$changelogPath = Join-Path $RepositoryRoot "CHANGELOG.md"
$changelog = Get-Content -Raw $changelogPath
$escapedVersion = [regex]::Escape($Version)
if ($changelog -notmatch "(?m)^## \[$escapedVersion\]") {
    throw "CHANGELOG.md does not contain a release section for version '$Version'."
}

Write-Output "Validated OpenSpec release '$ChangeId' for version '$Version'."
