#!/usr/bin/env bash
# Usage: devel/build.sh [out dir]
# Builds the judge and the NUnit fork in a pinned dotnet SDK container into the out dir (default build/):
# src/judge-csharp-dotnet (net10.0) into bin-dotnet/ and the NUnit fork in nunit/ (net10.0) into lib-dotnet/.
set -euo pipefail

dotnet_sdk_image=mcr.microsoft.com/dotnet/sdk:10.0.402
root=$(cd "$(dirname "$0")/.." && pwd)
out=${1:-$root/build}
mkdir -p "$out/bin-dotnet" "$out/lib-dotnet"
out=$(cd "$out" && pwd)

# The source is copied to a fixed path inside the container so the build is reproducible and the host tree gets no obj/ or bin/.
# The SDK's named mutexes fail for a non-root user, so the output is chowned back instead.
# The judge links the sources in src/judge-csharp, so src/ is copied as a whole.
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

# Only the assembly: the .NET judge compiles against and copies lib-dotnet/*.dll.
docker run --rm \
  -e HOST_OWNER="$(id -u):$(id -g)" \
  -v "$root/nunit:/src:ro" \
  -v "$out/lib-dotnet:/out" \
  "$dotnet_sdk_image" bash -euc '
    mkdir /tmp/build
    tar -C /src --exclude=bin --exclude=obj -cf - . | tar -C /tmp/build -xf -
    dotnet build /tmp/build/framework/nunit.framework.csproj -c Release --nologo -v quiet
    cp /tmp/build/bin/Release/net10.0/nunit.framework.dll /out/
    chown -R "$HOST_OWNER" /out
  '
