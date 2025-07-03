#!/usr/bin/env pwsh

param(
  [Parameter(Mandatory=$True)][String] $PackageOutputPath
)
$ErrorActionPreference = "Stop"

msbuild .\src\APES.MAUI.csproj -t:Build,Pack -p:Configuration=Release -p:PackageOutputPath="$PackageOutputPath"
