# Archive formats are modules behind one interface

Archives open in a panel like read-only folders. Each format, ZIP, TAR, TGZ,
7z and RAR, is a module behind one C# interface in `Syno.Files` (owner
rulings, 2026-11-01). The owner asked for plugin modules with a clean
interface. Modules compiled into the library give that interface without
loading plugins at run time, which would bring version mismatches and a
plugin crash that takes the application down.

## Considered options

- Runtime plugins, one assembly or DLL per format: not now. A well-designed
  interface can later load modules that way without changing them.
- A Rust trait in the shared engine: dropped with Rust
  ([ADR 0001](0001-no-rust.md)).

## Consequences

ZIP, TAR and TGZ use .NET's own classes. 7z and RAR reading uses SharpCompress
(MIT license). RAR is read only in every library. 7z creation uses
SharpCompress if its current release writes 7z, otherwise 7-Zip's `7z.dll`
(C++, LGPL) as the one native module; stage 6 checks and records which.
Search never looks inside archives.
