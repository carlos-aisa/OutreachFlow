param(
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [Parameter(Mandatory = $true)]
    [string]$SetupPath,

    [Parameter(Mandatory = $true)]
    [string]$MsiPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if ($Version -notmatch "^\d+\.\d+\.\d+$")
{
    throw "Version '$Version' is not a valid SemVer (X.Y.Z)."
}

if (-not (Test-Path -LiteralPath $SetupPath))
{
    throw "Setup artifact was not found at '$SetupPath'."
}

if (-not (Test-Path -LiteralPath $MsiPath))
{
    throw "MSI artifact was not found at '$MsiPath'."
}

$setupInfo = Get-Item -LiteralPath $SetupPath
$msiInfo = Get-Item -LiteralPath $MsiPath

if ($setupInfo.Length -le 0)
{
    throw "Setup artifact '$SetupPath' is empty."
}

if ($msiInfo.Length -le 0)
{
    throw "MSI artifact '$MsiPath' is empty."
}

$expectedSetupName = "OutreachFlow-v$Version-win-x64-setup.exe"
$expectedMsiName = "OutreachFlow-v$Version-win-x64.msi"

if ($setupInfo.Name -ne $expectedSetupName)
{
    throw "Setup filename '$($setupInfo.Name)' does not match expected '$expectedSetupName'."
}

if ($msiInfo.Name -ne $expectedMsiName)
{
    throw "MSI filename '$($msiInfo.Name)' does not match expected '$expectedMsiName'."
}

Write-Host "Installer artifact validation passed:"
Write-Host "  Setup: $($setupInfo.FullName)"
Write-Host "  MSI:   $($msiInfo.FullName)"
