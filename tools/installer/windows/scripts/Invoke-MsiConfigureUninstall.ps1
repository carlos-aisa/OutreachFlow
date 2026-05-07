[CmdletBinding()]
param(
    [Parameter()] [string]$RuntimeRoot = "C:\ProgramData\OutreachFlow",

    [Parameter()] [string]$ApiServiceName = "OutreachFlow.Api",

    [Parameter()] [string]$WebServiceName = "OutreachFlow.Web",

    [Parameter()] [int]$RemoveData = 0
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Remove-ServiceIfExists {
    param([Parameter(Mandatory = $true)] [string]$Name)

    $service = Get-Service -Name $Name -ErrorAction SilentlyContinue
    if ($null -eq $service) {
        return
    }

    if ($service.Status -eq [System.ServiceProcess.ServiceControllerStatus]::Running) {
        Stop-Service -Name $Name -Force
    }

    & sc.exe delete $Name | Out-Null
}

Remove-ServiceIfExists -Name $ApiServiceName
Remove-ServiceIfExists -Name $WebServiceName

if ($RemoveData -eq 1 -and (Test-Path -LiteralPath $RuntimeRoot)) {
    Remove-Item -LiteralPath $RuntimeRoot -Recurse -Force
}
