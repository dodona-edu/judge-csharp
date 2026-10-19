#!/usr/bin/env bash
# Usage: devel/build.sh [out dir]
# Builds src/judge-csharp in a pinned dotnet SDK container and copies bin/Release/net40 to the out dir (default build/bin).
set -euo pipefail

sdk_image=mcr.microsoft.com/dotnet/sdk:8.0.426
root=$(cd "$(dirname "$0")/.." && pwd)
out=${1:-$root/build/bin}
mkdir -p "$out"
out=$(cd "$out" && pwd)

# The source is copied to a fixed path inside the container so the build is reproducible and the host tree gets no obj/ or bin/.
# The SDK's named mutexes fail for a non-root user, so the output is chowned back instead.
docker run --rm \
  -e HOST_OWNER="$(id -u):$(id -g)" \
  -v "$root/src/judge-csharp:/src/judge-csharp:ro" \
  -v "$out:/out" \
  "$sdk_image" bash -euc '
    mkdir /tmp/build
    tar -C /src --exclude=bin --exclude=obj -cf - judge-csharp | tar -C /tmp/build -xf -
    dotnet build /tmp/build/judge-csharp/judge-csharp.csproj -c Release --nologo -v quiet
    cp -R /tmp/build/judge-csharp/bin/Release/net40/. /out/
    chown -R "$HOST_OWNER" /out
  '
