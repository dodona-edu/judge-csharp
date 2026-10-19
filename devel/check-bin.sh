#!/usr/bin/env bash
# Usage: devel/check-bin.sh
# Rebuilds src/judge-csharp with devel/build.sh and compares every file of the build with the same file in bin/.
set -euo pipefail

# The deployed judge was built from older source; empty this list once bin/ is rebuilt from src/.
allowed_to_differ=(judge-csharp.exe judge-csharp.exe.config judge-csharp.pdb)

root=$(cd "$(dirname "$0")/.." && pwd)
build=$(mktemp -d)
trap 'rm -rf "$build"' EXIT

"$root/devel/build.sh" "$build"

is_allowed() {
  local name
  for name in ${allowed_to_differ[@]+"${allowed_to_differ[@]}"}; do
    [ "$name" = "$1" ] && return 0
  done
  return 1
}

status=0
for file in "$build"/*; do
  name=$(basename "$file")
  if [ ! -f "$root/bin/$name" ]; then
    echo "DIFFERS $name (missing in bin/)"
    status=1
  elif cmp -s "$file" "$root/bin/$name"; then
    echo "same $name"
  elif is_allowed "$name"; then
    echo "differs (allowed) $name"
  else
    echo "DIFFERS $name (the rebuild is not byte-identical to bin/$name)"
    status=1
  fi
done

for file in "$root"/bin/* "$root"/bin/.[!.]*; do
  [ -e "$file" ] || continue
  [ -e "$build/$(basename "$file")" ] || echo "only in bin/ $(basename "$file")"
done

exit "$status"
