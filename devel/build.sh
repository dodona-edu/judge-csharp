#!/usr/bin/env bash
# Usage: devel/build.sh [out dir]
# Builds both judges in pinned dotnet SDK containers into the out dir (default build/):
# src/judge-csharp (mono, net40) into bin/ and src/judge-csharp-dotnet (net10.0) into bin-dotnet/.
set -euo pipefail

mono_sdk_image=mcr.microsoft.com/dotnet/sdk:8.0.426
dotnet_sdk_image=mcr.microsoft.com/dotnet/sdk:10.0.402
root=$(cd "$(dirname "$0")/.." && pwd)
out=${1:-$root/build}
mkdir -p "$out/bin" "$out/bin-dotnet"
out=$(cd "$out" && pwd)

# The source is copied to a fixed path inside the container so the build is reproducible and the host tree gets no obj/ or bin/.
# The SDK's named mutexes fail for a non-root user, so the output is chowned back instead.
docker run --rm \
  -e HOST_OWNER="$(id -u):$(id -g)" \
  -v "$root/src/judge-csharp:/src/judge-csharp:ro" \
  -v "$out/bin:/out" \
  "$mono_sdk_image" bash -euc '
    mkdir /tmp/build
    tar -C /src --exclude=bin --exclude=obj -cf - judge-csharp | tar -C /tmp/build -xf -
    dotnet build /tmp/build/judge-csharp/judge-csharp.csproj -c Release --nologo -v quiet
    cp -R /tmp/build/judge-csharp/bin/Release/net40/. /out/
    chown -R "$HOST_OWNER" /out
  '

# The .NET judge links the mono judge's shared sources, so src/ is copied as a whole.
docker run --rm \
  -e HOST_OWNER="$(id -u):$(id -g)" \
  -v "$root/src:/src:ro" \
  -v "$out/bin-dotnet:/out" \
  "$dotnet_sdk_image" bash -euc '
    mkdir /tmp/build
    tar -C /src --exclude=bin --exclude=obj -cf - . | tar -C /tmp/build -xf -
    dotnet build /tmp/build/judge-csharp-dotnet/judge-csharp-dotnet.csproj -c Release --nologo -v quiet
    cp -R /tmp/build/judge-csharp-dotnet/bin/Release/net10.0/. /out/
    chown -R "$HOST_OWNER" /out
  '
