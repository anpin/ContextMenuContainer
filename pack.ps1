#!/usr/bin/env pwsh

param(
  [Parameter(Mandatory=$True)][String] $PackageOutputPath
)
$ErrorActionPreference = "Stop"

$absolutePackageOutputPath = Convert-Path -Path $PackageOutputPath -ErrorAction Stop
Write-Host "Using PackageOutputPath: $absolutePackageOutputPath"

msbuild .\src\APES.UI.XF.csproj -t:Build,Pack -p:Configuration=Release -p:PackageOutputPath="$absolutePackageOutputPath"
