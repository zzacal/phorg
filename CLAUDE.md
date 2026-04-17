# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build src/Phorg/Phorg.csproj

# Run
dotnet run --project src/Phorg/Phorg.csproj

# Publish self-contained (osx-arm64)
dotnet publish src/Phorg/Phorg.csproj -c Release -r osx-arm64 --self-contained
```

There are no automated tests in this repo.

## Architecture

Two projects:

- **`Phorg.Core`** — file system utilities (`Recon` for recursive file discovery, `FileStore` for parallel copying)
- **`Phorg`** — CLI entry point, job orchestration, and UI

### Data flow

1. `Program.cs` prompts user for source and destination paths via `SpecterPrompt.BrowsePath`
2. `Processor.Prepare` calls `Recon.GetFilesRecursively` then `JobHelpers.CreateJobs` to group files by `CreationTime` date (`yyyyMMdd`) into `Movables` records
3. For each date group, the user is prompted for an optional suffix (e.g. `"20240715 Beach Trip"`); the folder path is `{destination}/{key} {suffix}`
4. `Processor.Start` → `FileStore.Copy` copies files in parallel (`MaxDegreeOfParallelism = 2`) with success/failure callbacks updating the Spectre progress display

### Key types

- `Movables(string Folder, List<FileInfo> Sources)` — a date group with its target folder and source files
- `IPrompt` — interface over Spectre.Console interactions; `SpecterPrompt` is the only implementation
- `JobHelpers` depends on `IPrompt` (injected), making it testable without UI
