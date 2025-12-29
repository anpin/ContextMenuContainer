#!/usr/bin/env bash
set -euo pipefail

TFM="net9.0-android"
CONFIG="Debug"
PKG="com.apes.maui.sample"   # <-- change me
LOG="maui-logcat.log"

adb wait-for-device >/dev/null

dotnet build -t:Run -f "$TFM" -c "$CONFIG"

# wait for PID
echo "Waiting for PID of $PKG..."
for _ in {1..60}; do
  PID="$(adb shell pidof -s "$PKG" 2>/dev/null | tr -d '\r' || true)"
  [[ -n "${PID:-}" ]] && break
  sleep 1
done

if [[ -z "${PID:-}" ]]; then
  echo "PID not found after 60s. Full log: $LOG"
  exit 1
fi

echo "App PID=$PID (app-only logcat; Ctrl+C to stop)"
adb logcat -v time --pid="$PID" 
