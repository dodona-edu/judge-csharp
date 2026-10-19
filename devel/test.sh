#!/usr/bin/env bash
# Usage: devel/test.sh
# Runs the unit tests in the same pinned dotnet SDK containers that devel/build.sh uses:
# src/judge-csharp.Tests for the mono judge and src/judge-csharp-dotnet.Tests for the .NET judge.
set -euo pipefail

mono_sdk_image=mcr.microsoft.com/dotnet/sdk:8.0.426
dotnet_sdk_image=mcr.microsoft.com/dotnet/sdk:10.0.402
root=$(cd "$(dirname "$0")/.." && pwd)

run_tests() { # $1 = SDK image, $2 = test project in src/
  # The tests link the judge's sources by relative path, so src/ is copied as a whole.
  docker run --rm \
    -v "$root/src:/src:ro" \
    "$1" bash -euc '
      mkdir /tmp/build
      tar -C /src --exclude=bin --exclude=obj -cf - . | tar -C /tmp/build -xf -
      dotnet test "/tmp/build/$0" --nologo
    ' "$2"
}

run_tests "$mono_sdk_image" judge-csharp.Tests
run_tests "$dotnet_sdk_image" judge-csharp-dotnet.Tests
