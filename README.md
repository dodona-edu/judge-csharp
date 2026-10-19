# judge-csharp

## Tests

The golden-file e2e suite runs this judge in the Dodona image once per `tests/exercises/*/submissions/*.cs` and diffs the result with the matching file in `expected/`.
Run it with `tests/run-exercises ghcr.io/dodona-edu/dodona-csharp:latest`.
Pass a second argument to only run the cases whose `exercise/case` name contains it.
Set `UPDATE_EXPECTED=1` to rewrite the expected files instead of diffing.
On Apple Silicon, mono's JIT aborts under amd64 emulation, so run it as `DOCKER_RUN_ARGS="--platform linux/amd64 -e MONO_ENV_OPTIONS=--interpreter" tests/run-exercises <image>`.
