[CmdletBinding()]
param(
    [Parameter()] [string]$InstallRoot = "",

    [Parameter()] [string]$RuntimeRoot = "C:\ProgramData\OutreachFlow",

    [Parameter()] [int]$ApiPort = 5131,

    [Parameter()] [int]$WebPort = 5107,

    [Parameter()] [string]$ApiServiceName = "OutreachFlow.Api",

    [Parameter()] [string]$WebServiceName = "OutreachFlow.Web"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($InstallRoot)) {
    $scriptsRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
    $InstallRoot = Split-Path -Parent $scriptsRoot
}

function Ensure-Directory {
    param([Parameter(Mandatory = $true)] [string]$Path)

    if (-not (Test-Path -LiteralPath $Path)) {
        New-Item -ItemType Directory -Path $Path -Force | Out-Null
    }
}

function Update-JsonFile {
    param(
        [Parameter(Mandatory = $true)] [string]$Path,
        [Parameter(Mandatory = $true)] [scriptblock]$Mutator
    )

    if (-not (Test-Path -LiteralPath $Path)) {
        return
    }

    $json = Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
    & $Mutator $json
    $json | ConvertTo-Json -Depth 32 | Set-Content -LiteralPath $Path -Encoding utf8
}

function Ensure-Service {
    param(
        [Parameter(Mandatory = $true)] [string]$Name,
        [Parameter(Mandatory = $true)] [string]$DisplayName,
        [Parameter(Mandatory = $true)] [string]$BinaryPathWithArgs
    )

    $service = Get-Service -Name $Name -ErrorAction SilentlyContinue
    if ($null -eq $service) {
        & sc.exe create $Name "binPath= $BinaryPathWithArgs" "start= auto" "DisplayName= $DisplayName" | Out-Null
    }
    else {
        if ($service.Status -eq [System.ServiceProcess.ServiceControllerStatus]::Running) {
            Stop-Service -Name $Name -Force
        }

        & sc.exe config $Name "binPath= $BinaryPathWithArgs" "start= auto" "DisplayName= $DisplayName" | Out-Null
    }

    Start-Service -Name $Name
}

$apiExePath = Join-Path $InstallRoot "Api\OutreachFlow.Api.exe"
$webExePath = Join-Path $InstallRoot "Web\OutreachFlow.Web.exe"

if (-not (Test-Path -LiteralPath $apiExePath)) {
    throw "API executable not found at '$apiExePath'."
}

if (-not (Test-Path -LiteralPath $webExePath)) {
    throw "Web executable not found at '$webExePath'."
}

$dataRoot = Join-Path $RuntimeRoot "data"
$attachmentsRoot = Join-Path $RuntimeRoot "attachments"

Ensure-Directory -Path $RuntimeRoot
Ensure-Directory -Path $dataRoot
Ensure-Directory -Path $attachmentsRoot

$apiConfigPaths = @(
    Join-Path $InstallRoot "Api\appsettings.json",
    Join-Path $InstallRoot "Api\appsettings.Production.json"
)

foreach ($configPath in $apiConfigPaths) {
    Update-JsonFile -Path $configPath -Mutator {
        param($json)

        if ($null -eq $json.ConnectionStrings) {
            $json | Add-Member -MemberType NoteProperty -Name "ConnectionStrings" -Value ([pscustomobject]@{})
        }

        $json.ConnectionStrings.OutreachFlow = "Data Source={0}" -f (Join-Path $dataRoot "outreachflow.db")

        if ($null -eq $json.AttachmentStorage) {
            $json | Add-Member -MemberType NoteProperty -Name "AttachmentStorage" -Value ([pscustomobject]@{})
        }

        $json.AttachmentStorage.RootPath = $attachmentsRoot

        if ($null -eq $json.Hosting) {
            $json | Add-Member -MemberType NoteProperty -Name "Hosting" -Value ([pscustomobject]@{})
        }

        $json.Hosting.UseHttpsRedirection = $false
    }
}

$webConfigPaths = @(
    Join-Path $InstallRoot "Web\appsettings.json",
    Join-Path $InstallRoot "Web\appsettings.Production.json"
)

foreach ($configPath in $webConfigPaths) {
    Update-JsonFile -Path $configPath -Mutator {
        param($json)

        if ($null -eq $json.OutreachFlowApi) {
            $json | Add-Member -MemberType NoteProperty -Name "OutreachFlowApi" -Value ([pscustomobject]@{})
        }

        $json.OutreachFlowApi.BaseUrl = "http://localhost:$ApiPort"

        if ($null -eq $json.Hosting) {
            $json | Add-Member -MemberType NoteProperty -Name "Hosting" -Value ([pscustomobject]@{})
        }

        $json.Hosting.UseHttpsRedirection = $false
        $json.Hosting.UseHsts = $false
    }
}

$apiServiceCommand = '"{0}" --urls "http://localhost:{1}"' -f $apiExePath, $ApiPort
$webServiceCommand = '"{0}" --urls "http://localhost:{1}"' -f $webExePath, $WebPort

Ensure-Service -Name $ApiServiceName -DisplayName "OutreachFlow API" -BinaryPathWithArgs $apiServiceCommand
Ensure-Service -Name $WebServiceName -DisplayName "OutreachFlow Web" -BinaryPathWithArgs $webServiceCommand
