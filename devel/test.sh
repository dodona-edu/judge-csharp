#!/usr/bin/env bash
# Usage: devel/test.sh
# Runs the unit tests in the same pinned dotnet SDK containers that devel/build.sh uses:
# src/judge-csharp.Tests for the mono judge, src/judge-csharp-dotnet.Tests for the .NET judge
# and src/nunit-fork.Tests for the NUnit fork in nunit/.
set -euo pipefail

mono_sdk_image=mcr.microsoft.com/dotnet/sdk:8.0.426
dotnet_sdk_image=mcr.microsoft.com/dotnet/sdk:10.0.402
root=$(cd "$(dirname "$0")/.." && pwd)

run_tests() { # $1 = SDK image, $2 = test project in src/
  # The tests link sources by relative path, so src/ and nunit/ are copied as a whole.
  docker run --rm \
    -v "$root/src:/src/src:ro" \
    -v "$root/nunit:/src/nunit:ro" \
    "$1" bash -euc '
      mkdir /tmp/build
      tar -C /src --exclude=bin --exclude=obj -cf - . | tar -C /tmp/build -xf -
      dotnet test "/tmp/build/src/$0" --nologo
    ' "$2"
}

run_tests "$mono_sdk_image" judge-csharp.Tests
run_tests "$dotnet_sdk_image" judge-csharp-dotnet.Tests
run_tests "$dotnet_sdk_image" nunit-fork.Tests
