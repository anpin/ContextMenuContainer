#!/bin/sh
set -e

dotnet publish ../../../sample -f:net9.0-android -c Release \
    -p:RunAOTCompilation=true \
    -p:AndroidPackageFormat=apk
adb install ../../../sample/bin/Release/net9.0-android/publish/com.apes.maui.sample-Signed.apk
dotnet test 
