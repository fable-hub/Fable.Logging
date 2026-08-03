# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Fable.Logging is a cross-platform logging framework for Fable (F# transpiler). It mirrors the .NET `Microsoft.Extensions.Logging` pattern with `ILogger`, `ILoggerFactory`, and `ILoggerProvider` interfaces. Published as separate NuGet packages with independent versioning.

## Build Commands

Uses [just](https://github.com/casey/just) as command runner:

```bash
just setup          # Restore dotnet tools (fable, paket, fantomas, shipit)
just restore        # Install Paket dependencies and restore projects
just build          # Build all projects
just format         # Format with Fantomas
just format-check   # Check formatting
just pack           # Create NuGet packages (versions from CHANGELOG.md)
just release        # Pack + push to nuget.org
just shipit         # Run EasyBuild.ShipIt for release management
```

## NuGet Packages

| Package | Path | Description |
|---------|------|-------------|
| `Fable.Logging` | `src/Fable.Logging/` | Core interfaces, LoggerFactory, ConsoleLogger, JS console logger |
| `Fable.Logging.Structlog` | `src/Fable.Logging.Structlog/` | Python structlog provider (depends on Fable.Logging) |
| `Fable.Logging.Beam` | `src/Fable.Logging.Beam/` | Erlang/OTP logger provider (depends on Fable.Logging + Fable.Beam) |

Each package has its own `CHANGELOG.md` with YAML frontmatter for ShipIt. Versions are extracted from changelog headers at pack time.

## Architecture

**Fable.Logging** (core, platform-agnostic):
- `ILogger.fs` — `LogLevel` enum, `LogState` record, `ILogger`/`ILoggerFactory`/`ILoggerProvider` interfaces, convenience extension methods (`LogDebug`, `LogError`, etc.)
- `Common.fs` — `translateFormat` converts `{Named}` format strings to indexed `{0}` format and extracts named parameters into a dictionary
- `LoggerFactory.fs` — Aggregates multiple `ILoggerProvider`s, dispatches log calls. Defines `ILoggingBuilder` for configuration
- `ConsoleLogger.fs` — Platform-agnostic logger using `Console.WriteLine`
- `JSLogger.fs` — JavaScript `console.*` logger (no external deps, included in core package)

**Fable.Logging.Structlog** (Python-only):
- `Structlog.fs` — Python structlog bindings via `[<Import>]`/`[<Emit>]`. Provides `ConsoleLogger` (text), `JsonLogger` (JSON), and their `ILoggerProvider` implementations

**Fable.Logging.Beam** (Erlang/OTP):
- `BeamLogger.fs` — Bridges `ILogger` to OTP `logger` module via `Fable.Beam.Logger`

## Conventions

- **Dependencies**: Managed via Paket (`paket.dependencies` + per-project `paket.references`)
- **Target framework**: `netstandard2.0` for all library packages
- **Fable interop**: `[<Import>]` for module imports, `[<Emit>]` for target-specific call syntax, `[<Mangle>]` for unique interface method names in transpiled output
- **Logger types must have no mutable instance state**: no `mutable` fields, no `member val ... with
  get, set`, no `ResizeArray` fields. Any of these makes Fable back the instance with a `make_ref()`
  key into the process dictionary instead of a plain map, which confines it to the process that
  created it — on the BEAM, using it from a spawned process fails with `{badmap,undefined}` at the
  first member access. Pass configuration as constructor parameters. `LoggerFactory` and
  `ILoggerProvider` implementations may hold mutable state; they stay in the configuring process.
  `test/TestBeamLogger.fs` has a cross-process regression test for this.
- **Releases**: EasyBuild.ShipIt with `--pre-release rc`, triggered by Conventional Commits
- **NuGet packaging**: F# source files included via `<Content>` with `PackagePath="fable\"` for consumer-side Fable transpilation
