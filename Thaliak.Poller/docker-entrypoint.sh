#!/bin/sh
while true; do
  dotnet Thaliak.Poller.dll
  echo "[$(date)] Poller exited with code $?. Sleeping..."
  sleep 300
done
