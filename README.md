# judge-csharp

## Runtime

`run` compiles the submission and the tests with the SDK's Roslyn (`csc.dll`, against the net10.0 reference assemblies and every dll in `lib-dotnet/`) and runs `bin-dotnet/judge-csharp.dll`.

The judge runs the tests in a child process, a second `dotnet bin-dotnet/judge-csharp.dll` with the invariant culture.
The child drives the `nunit.framework.dll` next to the test assembly through NUnit's `FrameworkController` and sends its events to the judge over an anonymous pipe, so nothing student code writes to stdout can reach the judge's output (it ends up on stderr).
When the child ends before the run is complete, the judge closes the levels it left open: exit code 137 (killed by the memory limit) gives "memory limit exceeded", any other end a runtime error with a message that names the exit code.

`lib-dotnet/` holds the NUnit framework the tests compile against: the judge's NUnit 3.13 fork in `nunit/`, built for net10.0.
`nunit/` is imported with its history from [dodona-edu/judge-csharp-nunit](https://github.com/dodona-edu/judge-csharp-nunit).
The fork times out `[Timeout]` tests without `Thread.Abort`, which .NET does not support, so a timed-out test keeps running in the background until the run ends.

## Source and binaries

Dodona runs the committed `bin-dotnet/` and `lib-dotnet/` directly. The source lives in `src/`, imported with its history from [dodona-edu/judge-csharp-source](https://github.com/dodona-edu/judge-csharp-source): `src/judge-csharp-dotnet` is the judge, which links the `OutputHandler`, `Json/`, `Feedback/` and `Util/` sources in `src/judge-csharp`.
The scripts in `devel/` need Docker and run the .NET SDK in a pinned container (10.0.402), so no local .NET install is needed.

- `devel/build.sh [out dir]` rebuilds the judge and the NUnit fork reproducibly into `bin-dotnet/` and `lib-dotnet/` of the out dir (default `build/`).
- `devel/check-bin.sh` compares that rebuild with `bin-dotnet/` and `lib-dotnet/` and fails when any file differs: every file must match.
- `devel/test.sh` runs the unit tests in `src/judge-csharp.Tests` (the shared sources), `src/judge-csharp-dotnet.Tests` (the judge) and `src/nunit-fork.Tests` (the fork in `nunit/`, which runs these tests itself). A test wrapped in `KnownBug.Expect` asserts the intended behaviour of a known bug: it is skipped while the bug exists and fails once the bug is fixed, so the marker gets removed.
- `devel/run-e2e.sh [image]` runs the golden suite below on a copy of this repository whose `bin-dotnet/` and `lib-dotnet/` are a fresh rebuild.

`devel/run-e2e.sh` honours `DOCKER_RUN_ARGS` like the suite itself, so for an amd64-only image on Apple Silicon use `DOCKER_RUN_ARGS="--platform linux/amd64" devel/run-e2e.sh`.

## Tests

The golden-file e2e suite runs this judge in the Dodona image once per `tests/exercises/*/submissions/*.cs` and diffs the result with the matching file in `expected/`.
Run it with `tests/run-exercises ghcr.io/dodona-edu/dodona-csharp:latest`.
Pass a second argument to only run the cases whose `exercise/case` name contains it.
Set `UPDATE_EXPECTED=1` to rewrite the expected files instead of diffing.
