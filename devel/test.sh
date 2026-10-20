#!/usr/bin/env bash
# Usage: devel/test.sh
# Runs the unit tests in the same pinned dotnet SDK container that devel/build.sh uses:
# src/judge-csharp.Tests for the shared sources, src/judge-csharp-dotnet.Tests for the judge
# and src/nunit-fork.Tests for the NUnit fork in nunit/.
set -euo pipefail

sdk_image=mcr.microsoft.com/dotnet/sdk:10.0.402
root=$(cd "$(dirname "$0")/.." && pwd)

run_tests() { # $1 = test project in src/
  # The tests link sources by relative path, so src/ and nunit/ are copied as a whole.
  docker run --rm \
    -v "$root/src:/src/src:ro" \
    -v "$root/nunit:/src/nunit:ro" \
    "$sdk_image" bash -euc '
      mkdir /tmp/build
      tar -C /src --exclude=bin --exclude=obj -cf - . | tar -C /tmp/build -xf -
      dotnet test "/tmp/build/src/$0" --nologo
    ' "$1"
}

run_tests judge-csharp.Tests
run_tests judge-csharp-dotnet.Tests
run_tests nunit-fork.Tests
