#!/bin/sh
set -e

dotnet publish ../../../sample -f:net10.0-android -c Release \
    -p:RunAOTCompilation=true \
    -p:AndroidPackageFormat=apk
adb install ../../../sample/bin/Release/net10.0-android/publish/com.apes.maui.sample-Signed.apk
dotnet test 
