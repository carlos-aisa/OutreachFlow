param(
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [Parameter(Mandatory = $true)]
    [string]$ArtifactPath,

    [Parameter(Mandatory = $false)]
    [string]$ExpectedArchitecture = "x64"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if ($Version -notmatch "^\d+\.\d+\.\d+$")
{
    throw "Version '$Version' is not a valid SemVer (X.Y.Z)."
}

if (-not (Test-Path -LiteralPath $ArtifactPath))
{
    throw "Installer artifact was not found at '$ArtifactPath'."
}

$artifactInfo = Get-Item -LiteralPath $ArtifactPath
if ($artifactInfo.Length -le 0)
{
    throw "Installer artifact '$ArtifactPath' is empty."
}

$expectedFileNamePattern = "OutreachFlow-v$Version-win-$ExpectedArchitecture-installer.zip"
if ($artifactInfo.Name -ne $expectedFileNamePattern)
{
    throw "Installer filename '$($artifactInfo.Name)' does not match expected '$expectedFileNamePattern'."
}

Write-Host "Installer artifact validation passed:"
Write-Host "  File: $($artifactInfo.FullName)"
Write-Host "  Size: $($artifactInfo.Length) bytes"
