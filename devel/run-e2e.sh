#!/usr/bin/env bash
# Usage: devel/run-e2e.sh [image]
# Runs the golden e2e suite on a copy of this repository whose bin/ is a fresh build of src/judge-csharp.
# DOCKER_RUN_ARGS is passed through, e.g. DOCKER_RUN_ARGS="--platform linux/amd64 -e MONO_ENV_OPTIONS=--interpreter" on arm64.
set -euo pipefail

if [ $# -gt 1 ]; then
  echo "Usage: $0 [image]" >&2
  exit 2
fi

image=${1:-ghcr.io/dodona-edu/dodona-csharp:latest}
root=$(cd "$(dirname "$0")/.." && pwd)

work=$(mktemp -d)
trap 'rm -rf "$work"' EXIT

tar -C "$root" --exclude=./.git --exclude=./build -cf - . | tar -C "$work" -xf -
rm -rf "${work:?}/bin"
mkdir "$work/bin"
"$root/devel/build.sh" "$work/bin"

"$work/tests/run-exercises" "$image"
