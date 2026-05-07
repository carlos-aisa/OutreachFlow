[CmdletBinding()]
param(
    [Parameter()] [string]$Version = "0.0.0",
    [Parameter()] [string]$Configuration = "Release"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if ($Version -notmatch "^\d+\.\d+\.\d+$")
{
    throw "Version '$Version' is not a valid SemVer (X.Y.Z)."
}

function Convert-ToMsiVersion {
    param([Parameter(Mandatory = $true)] [string]$InputVersion)

    $parts = ($InputVersion -split "[^0-9]") | Where-Object { $_ -ne "" }
    if ($parts.Count -lt 3) {
        throw "MSI requires major.minor.patch. Received '$InputVersion'."
    }

    return "{0}.{1}.{2}" -f [int]$parts[0], [int]$parts[1], [int]$parts[2]
}

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..\..\..")
$artifactsRoot = Join-Path $repoRoot "artifacts\installer"
$layoutRoot = Join-Path $artifactsRoot "msi-layout"
$apiLayout = Join-Path $layoutRoot "Api"
$webLayout = Join-Path $layoutRoot "Web"
$scriptsLayout = Join-Path $layoutRoot "installer-scripts"
$wixProject = Join-Path $PSScriptRoot "wix\OutreachFlow.Installer.wixproj"
$bootstrapperProject = Join-Path $PSScriptRoot "bootstrapper\OutreachFlow.Bootstrapper.wixproj"
$apiProject = Join-Path $repoRoot "src\OutreachFlow.Api\OutreachFlow.Api.csproj"
$webProject = Join-Path $repoRoot "src\OutreachFlow.Web\OutreachFlow.Web.csproj"

$msiFile = Join-Path $artifactsRoot ("OutreachFlow-v{0}-win-x64.msi" -f $Version)
$setupFile = Join-Path $artifactsRoot ("OutreachFlow-v{0}-win-x64-setup.exe" -f $Version)

if (Test-Path $artifactsRoot) {
    Remove-Item $artifactsRoot -Recurse -Force
}

New-Item -ItemType Directory -Path $apiLayout -Force | Out-Null
New-Item -ItemType Directory -Path $webLayout -Force | Out-Null
New-Item -ItemType Directory -Path $scriptsLayout -Force | Out-Null

Push-Location $repoRoot
try {
    dotnet publish $apiProject `
        --configuration $Configuration `
        --runtime win-x64 `
        --self-contained true `
        /p:PublishSingleFile=false `
        /p:PublishTrimmed=false `
        --output $apiLayout

    dotnet publish $webProject `
        --configuration $Configuration `
        --runtime win-x64 `
        --self-contained true `
        /p:PublishSingleFile=false `
        /p:PublishTrimmed=false `
        --output $webLayout

    Copy-Item -Path (Join-Path $PSScriptRoot "scripts\*.ps1") -Destination $scriptsLayout -Force

    if (-not (Test-Path (Join-Path $apiLayout "OutreachFlow.Api.exe"))) {
        throw "Published API executable not found in $apiLayout."
    }

    if (-not (Test-Path (Join-Path $webLayout "OutreachFlow.Web.exe"))) {
        throw "Published Web executable not found in $webLayout."
    }

    $msiVersion = Convert-ToMsiVersion -InputVersion $Version
    $msiOutputName = "OutreachFlow-v{0}-win-x64" -f $Version

    dotnet build $wixProject -c $Configuration `
        /p:ProductVersion=$msiVersion `
        /p:MsiSourceDir=$layoutRoot `
        /p:OutputName=$msiOutputName `
        /p:OutputPath="$artifactsRoot\"

    if ($LASTEXITCODE -ne 0) {
        throw "WiX MSI build failed with exit code $LASTEXITCODE."
    }

    if (-not (Test-Path $msiFile)) {
        $candidateMsi = Get-ChildItem -Path (Join-Path $PSScriptRoot "wix") -Recurse -Filter ("OutreachFlow-v{0}-win-x64.msi" -f $Version) |
            Sort-Object LastWriteTime -Descending |
            Select-Object -First 1

        if ($null -eq $candidateMsi) {
            throw "MSI artifact not found after WiX build."
        }

        Copy-Item -Path $candidateMsi.FullName -Destination $msiFile -Force
    }

    $setupOutputName = "OutreachFlow-v{0}-win-x64-setup" -f $Version

    dotnet build $bootstrapperProject -c $Configuration `
        /p:ProductVersion=$msiVersion `
        /p:InstallerMsiPath=$msiFile `
        /p:OutputName=$setupOutputName `
        /p:OutputPath="$artifactsRoot\"

    if ($LASTEXITCODE -ne 0) {
        throw "WiX bootstrapper build failed with exit code $LASTEXITCODE."
    }

    if (-not (Test-Path $setupFile)) {
        $candidateSetup = Get-ChildItem -Path (Join-Path $PSScriptRoot "bootstrapper") -Recurse -Filter ("OutreachFlow-v{0}-win-x64-setup.exe" -f $Version) |
            Sort-Object LastWriteTime -Descending |
            Select-Object -First 1

        if ($null -eq $candidateSetup) {
            throw "Setup executable not found after WiX bootstrapper build."
        }

        Copy-Item -Path $candidateSetup.FullName -Destination $setupFile -Force
    }

    [pscustomobject]@{
        Version           = $Version
        MsiPath           = (Resolve-Path $msiFile).Path
        SetupPath         = (Resolve-Path $setupFile).Path
        BuildConfiguration = $Configuration
    } | ConvertTo-Json -Depth 4
}
finally {
    Pop-Location
}
