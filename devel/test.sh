#!/usr/bin/env bash
# Usage: devel/test.sh
# Runs the unit tests in the same pinned dotnet SDK container that devel/build.sh uses.
set -euo pipefail

sdk_image=mcr.microsoft.com/dotnet/sdk:8.0.426
root=$(cd "$(dirname "$0")/.." && pwd)

# The tests link the judge's sources by relative path, so src/ is copied as a whole.
docker run --rm \
  -v "$root/src:/src:ro" \
  "$sdk_image" bash -euc '
    mkdir /tmp/build
    tar -C /src --exclude=bin --exclude=obj -cf - . | tar -C /tmp/build -xf -
    dotnet test /tmp/build/judge-csharp.Tests --nologo
  '
