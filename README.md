# SynapseCommander

SynapseCommander is a new C#/WinUI 3 file manager for Windows, developed by
SynoSoftware. Its baseline is the complete working feature set of gozler-ui,
with the everyday improvements in the product plan.

File operations use the shared Rust engine, loaded as an application-local DLL.
There is no additional engine executable or service. The existing indexing
service remains responsible for indexing and search.

## Visual Studio

Open [SynapseCommander.sln](SynapseCommander.sln), select **Debug | x64**, and
set **SettingsSample** as the startup project. The solution contains the
settings prototype, its library and model checks; the file manager is planned.
See the [sample setup](lib/Settings/README.md#visual-studio) for shared dependencies.

## Plans

- [Product and feature inventory](PRODUCT.md): existing capabilities, additions
  and the scope of parity.
- [Architecture](docs/ARCHITECTURE.md): ownership, shared code, native interop
  and application lifetime.
- [Implementation plan](docs/plans/synapsecommander.md): ordered stages,
  dependencies and completion criteria.
- [Contributor guidance](AGENTS.md): coding, naming and verification.
- [Settings component](lib/Settings/README.md): the current WinUI library trial,
  its shared presentation language, sample and TableView integration.

These documents describe planned work, not an implemented WinUI application.
The superseded Rust extraction and inherited tooling and documents have been
removed. The reusable Settings component and sample are being developed here;
the complete WinUI file manager and Rust-library integration remain pending.

The shared engine's source currently lives in the separate goz working folder,
`C:\SynoSoftware\TinyTorrent2\3rdParty\goz`. SynapseCommander will build and ship
the shared DLL without copying its implementation or importing goz history.
Existing license notices remain with any reused code.

Distribution will follow TinyTorrent2: a per-user Inno Setup `.exe` installer
published through GitHub Releases. Shared libraries ship with the application;
publishing them as separate packages is not required. The
[architecture](docs/ARCHITECTURE.md#dependency-and-release-contract) owns the
release design; the installer is not implemented yet.
