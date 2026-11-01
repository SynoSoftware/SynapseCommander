# SynapseCommander has no Rust

SynapseCommander's file engine, listings, viewer reads, archives and search
client are C# libraries in this repository. C or C++ is used only where C#
cannot do a job, such as an existing native library (owner ruling,
2026-11-01: "no more rust here.. c# and c or c++ if we must"). The owner does
not want to compile Rust for this product: goz's build folder holds 6 GB today
and grows to 20–50 GB between cleans.

Speed should not suffer. Copy and delete wait on the disk, and their speed
comes from Windows calls, such as `CopyFile2`, unbuffered and overlapped I/O
and POSIX delete, which C# calls through P/Invoke at a cost of tens of
nanoseconds per call. That is reasoning, not a measurement, so stage 1
measures the C# copy and delete against goz `docs/BENCHMARKS.md` on the same
machine and chooses the methods from the result.

## Considered options

- The shared Rust engine through a C ABI DLL: rejected by the owner. It would
  have shared engine fixes with gozler-ui and kept one query parser.
- A prebuilt DLL from goz, so the owner never compiles it: rejected with it;
  the owner wants no Rust here at all.

## Consequences

gozler-ui keeps its own Rust engine, so the two products no longer share
engine fixes. goz's `docs/DESIGN.md`, `docs/research` and `docs/BENCHMARKS.md`
become this product's specification and reference. The goz engine was written
by AI agents after that research, and its tests have not been reviewed, so
SynapseCommander writes its own tests from the specification. gozlerd's query
parser is ported to C#, with checks built from goz's query tests so that drift
between the two parsers fails a check.
