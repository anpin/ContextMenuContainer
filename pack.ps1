#!/usr/bin/env pwsh

param(
  [Parameter(Mandatory=$True)][String] $PackageOutputPath
)
$ErrorActionPreference = "Stop"

msbuild .\src\APES.UI.XF.csproj -t:Build,Pack -p:Configuration=Release -p:PackageOutputPath="$PackageOutputPath"
