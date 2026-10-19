#!/usr/bin/env bash
# Usage: devel/check-bin.sh
# Rebuilds both judges with devel/build.sh and compares every file of the build with the same file in bin/ and bin-dotnet/.
set -euo pipefail

allowed_to_differ=()

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
for dir in bin bin-dotnet; do
  for file in "$build/$dir"/*; do
    name="$dir/$(basename "$file")"
    if [ ! -f "$root/$name" ]; then
      echo "DIFFERS $name (missing in $dir/)"
      status=1
    elif cmp -s "$file" "$root/$name"; then
      echo "same $name"
    elif is_allowed "$name"; then
      echo "differs (allowed) $name"
    else
      echo "DIFFERS $name (the rebuild is not byte-identical to $name)"
      status=1
    fi
  done

  for file in "$root/$dir"/* "$root/$dir"/.[!.]*; do
    [ -e "$file" ] || continue
    [ -e "$build/$dir/$(basename "$file")" ] || echo "only in $dir/ $(basename "$file")"
  done
done

exit "$status"
