# judge-csharp

## Source and binaries

Dodona runs the committed `bin/` directly. The source lives in `src/`, imported with its history from [dodona-edu/judge-csharp-source](https://github.com/dodona-edu/judge-csharp-source).
The scripts in `devel/` need Docker and run the .NET SDK in a pinned container, so no local .NET install is needed.

- `devel/build.sh [out dir]` rebuilds `src/judge-csharp` reproducibly into the out dir (default `build/bin`).
- `devel/check-bin.sh` compares that rebuild with `bin/` and fails when a file that should be identical differs.
- `devel/test.sh` runs the unit tests in `src/judge-csharp.Tests`. A test wrapped in `KnownBug.Expect` asserts the intended behaviour of a known bug: it is skipped while the bug exists and fails once the bug is fixed, so the marker gets removed.
- `devel/run-e2e.sh [image]` runs the golden suite below on a copy of this repository whose `bin/` is a fresh rebuild.

`devel/run-e2e.sh` honours `DOCKER_RUN_ARGS` like the suite itself, so on Apple Silicon use `DOCKER_RUN_ARGS="--platform linux/amd64 -e MONO_ENV_OPTIONS=--interpreter" devel/run-e2e.sh`.

## Tests

The golden-file e2e suite runs this judge in the Dodona image once per `tests/exercises/*/submissions/*.cs` and diffs the result with the matching file in `expected/`.
Run it with `tests/run-exercises ghcr.io/dodona-edu/dodona-csharp:latest`.
Pass a second argument to only run the cases whose `exercise/case` name contains it.
Set `UPDATE_EXPECTED=1` to rewrite the expected files instead of diffing.
On Apple Silicon, mono's JIT aborts under amd64 emulation, so run it as `DOCKER_RUN_ARGS="--platform linux/amd64 -e MONO_ENV_OPTIONS=--interpreter" tests/run-exercises <image>`.
