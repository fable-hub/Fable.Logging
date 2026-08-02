# Fable.Logging build commands
# Install just: https://github.com/casey/just

set dotenv-load

src_path := "src"
build_path := "build"

# Default recipe - show available commands
default:
    @just --list

# Clean build output
clean:
    rm -rf {{src_path}}/Fable.Logging/obj {{src_path}}/Fable.Logging/bin
    rm -rf {{src_path}}/Fable.Logging.Structlog/obj {{src_path}}/Fable.Logging.Structlog/bin
    rm -rf {{src_path}}/Fable.Logging.Beam/obj {{src_path}}/Fable.Logging.Beam/bin
    rm -rf {{build_path}}
    rm -rf .fable

# Build all projects
build:
    dotnet build {{src_path}}/Fable.Logging
    dotnet build {{src_path}}/Fable.Logging.Structlog
    dotnet build {{src_path}}/Fable.Logging.Beam
    dotnet build test

# Create NuGet packages with version from root changelog
pack:
    #!/usr/bin/env bash
    set -euo pipefail
    VERSION=$(grep -m1 '^## ' CHANGELOG.md | sed 's/^## \([^ ]*\).*/\1/')
    dotnet pack src/Fable.Logging -c Release -o ./nupkgs -p:PackageVersion=$VERSION -p:InformationalVersion=$VERSION
    dotnet pack src/Fable.Logging.Structlog -c Release -o ./nupkgs -p:PackageVersion=$VERSION -p:InformationalVersion=$VERSION
    dotnet pack src/Fable.Logging.Beam -c Release -o ./nupkgs -p:PackageVersion=$VERSION -p:InformationalVersion=$VERSION

# Pack and push all packages to NuGet (used in CI)
release: pack
    dotnet nuget push './nupkgs/*.nupkg' -s https://api.nuget.org/v3/index.json -k $NUGET_KEY --skip-duplicate

# Run .NET tests
test:
    dotnet build test
    dotnet run --project test

# Transpile tests to Erlang and compile with rebar3.
# Fable.Logging.Structlog is excluded: its bindings are Python-only ([<Emit>] bodies
# containing Python syntax) and do not transpile to valid Erlang.
build-beam:
    dotnet build test
    dotnet fable test --lang beam --outDir {{build_path}}/tests --exclude Fable.Logging.Structlog
    cd {{build_path}}/tests && rebar3 compile

# Run tests on the BEAM (transpile F# to Erlang, compile, run under erl)
test-beam: build-beam
    @echo ""
    cd {{build_path}}/tests && erl -noshell \
        $(for d in _build/default/lib/*/ebin; do echo -n "-pa $d "; done) \
        -eval 'main:main([])' \
        -s init stop

# Format code with Fantomas
format:
    dotnet fantomas {{src_path}}

# Check code formatting without making changes
format-check:
    dotnet fantomas {{src_path}} --check

# Install .NET tools (Fable, Fantomas, etc.)
setup:
    dotnet tool restore

# Restore all dependencies
restore:
    dotnet paket install
    dotnet restore {{src_path}}/Fable.Logging
    dotnet restore {{src_path}}/Fable.Logging.Structlog
    dotnet restore {{src_path}}/Fable.Logging.Beam
    dotnet restore test

# Run EasyBuild.ShipIt for release management
shipit *args:
    dotnet shipit {{args}}
