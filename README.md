# csharp-offshore-valve-stiction-diagnostic

Identify command reversals, valve deadband and persistent position mismatch from trend captures.

```sh
dotnet run -- --self-test
dotnet run < examples/input.txt
```

Offline engineering analysis. Installation-specific limits and source records determine the result; this program does not operate equipment.

MIT. Built by [brunnodev](https://brunnodev.store).

## Execution proof

[![Executable proof](https://github.com/brunnojob/csharp-offshore-valve-stiction-diagnostic/actions/workflows/proof.yml/badge.svg)](https://github.com/brunnojob/csharp-offshore-valve-stiction-diagnostic/actions/workflows/proof.yml)

[Recorded execution and downloadable evidence](https://github.com/brunnojob/csharp-offshore-valve-stiction-diagnostic/actions/workflows/proof.yml)

Run `python .proof/record.py` after installing the prerequisites above. The scenarios execute repository code and verify exit codes and expected output. CI publishes `execution-proof` with the transcript, input fingerprints and source commit. The downloadable report identifies the exact tested version; the workflow badge tracks the latest run.
