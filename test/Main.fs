module Fable.Logging.Tests.Main

open Fable.Logging.Tests

let runTest name (f: unit -> unit) =
    try
        f ()
        printfn "  PASS: %s" name
        true
    with ex ->
        printfn "  FAIL: %s - %s" name ex.Message
        false

let tests =
    [
        // Common (translateFormat)
        "translateFormat with no args", Common.``test translateFormat with no args``
        "translateFormat with named placeholder", Common.``test translateFormat with named placeholder``
        "translateFormat with multiple placeholders", Common.``test translateFormat with multiple placeholders``
        "translateFormat with category name", Common.``test translateFormat with category name``
        "translateFormat empty category name", Common.``test translateFormat empty category name``
        "translateFormat sets category_name parameter", Common.``test translateFormat sets category_name parameter``

        // ConsoleLogger
        "ConsoleLogger implements ILogger", ConsoleLogger.``test ConsoleLogger implements ILogger``
        "ConsoleLogger is always enabled", ConsoleLogger.``test ConsoleLogger is always enabled``
        "ConsoleLogger logs without error", ConsoleLogger.``test ConsoleLogger logs without error``
        "ConsoleLogger logs with format args", ConsoleLogger.``test ConsoleLogger logs with format args``

        // LoggerFactory
        "LoggerFactory.Create returns factory", LoggerFactory.``test LoggerFactory.Create returns factory``
        "LoggerFactory with provider", LoggerFactory.``test LoggerFactory with provider``
        "LoggerFactory creates logger with correct name", LoggerFactory.``test LoggerFactory creates logger with correct name``
        "LoggerFactory dispatches to provider", LoggerFactory.``test LoggerFactory dispatches to provider``
        "LoggerFactory dispatches to multiple providers", LoggerFactory.``test LoggerFactory dispatches to multiple providers``
        "LoggerFactory with configure sets minimum level", LoggerFactory.``test LoggerFactory with configure sets minimum level``
        "LoggerFactory.Create empty constructor", LoggerFactory.``test LoggerFactory.Create empty constructor``
        "LoggerFactory AddProvider affects only later loggers", LoggerFactory.``test LoggerFactory AddProvider affects only later loggers``
        "LoggerFactory exposes minimum level", LoggerFactory.``test LoggerFactory exposes minimum level``
        "LoggerFactory ClearProviders", LoggerFactory.``test LoggerFactory ClearProviders``
        "LoggerFactory Dispose clears providers", LoggerFactory.``test LoggerFactory Dispose clears providers``

        // LogLevel
        "log levels have correct ordering", LogLevel.``test log levels have correct ordering``
        "log level filtering with mock logger", LogLevel.``test log level filtering with mock logger``
        "LogState.Create defaults", LogLevel.``test LogState.Create defaults``
        "LogState.Create with parameters", LogLevel.``test LogState.Create with parameters``
        "LogState.Create with exception", LogLevel.``test LogState.Create with exception``

        // Extensions
        "LogDebug dispatches with Debug level", Extensions.``test LogDebug dispatches with Debug level``
        "LogInformation dispatches with Information level", Extensions.``test LogInformation dispatches with Information level``
        "LogWarning dispatches with Warning level", Extensions.``test LogWarning dispatches with Warning level``
        "LogError dispatches with Error level", Extensions.``test LogError dispatches with Error level``
        "LogCritical dispatches with Critical level", Extensions.``test LogCritical dispatches with Critical level``
        "Log with exception", Extensions.``test Log with exception``
        "extension methods respect IsEnabled", Extensions.``test extension methods respect IsEnabled``
        "LogDebug with format args", Extensions.``test LogDebug with format args``

        // Structlog (Python only - no-ops on .NET)
        "Structlog ConsoleLoggerProvider creates logger", Structlog.``test Structlog ConsoleLoggerProvider creates logger``
        "Structlog JsonLoggerProvider creates logger", Structlog.``test Structlog JsonLoggerProvider creates logger``
        "Structlog logger logs info", Structlog.``test Structlog logger logs info``
        "Structlog logger logs with format args", Structlog.``test Structlog logger logs with format args``
        "Structlog logger respects minimum level", Structlog.``test Structlog logger respects minimum level``
        "Structlog JsonLogger logs JSON", Structlog.``test Structlog JsonLogger logs JSON``

        // Beam Logger (BEAM only - no-ops on .NET)
        "Beam LoggerProvider creates logger", BeamLogger.``test Beam LoggerProvider creates logger``
        "Beam logger logs info", BeamLogger.``test Beam logger logs info``
        "Beam logger logs all levels", BeamLogger.``test Beam logger logs all levels``
        "Beam logger logs with format args", BeamLogger.``test Beam logger logs with format args``
        "Beam logger respects minimum level", BeamLogger.``test Beam logger respects minimum level``
        "Beam logger is usable from a spawned process", BeamLogger.``test Beam logger is usable from a spawned process``

        // JS Logger (JS only - no-ops on .NET)
        "JS LoggerProvider creates logger", JSLogger.``test JS LoggerProvider creates logger``
        "JS logger logs info", JSLogger.``test JS logger logs info``
        "JS logger logs all levels", JSLogger.``test JS logger logs all levels``
        "JS logger logs with format args", JSLogger.``test JS logger logs with format args``
        "JS logger respects minimum level", JSLogger.``test JS logger respects minimum level``
    ]

[<EntryPoint>]
let main _argv =
    printfn "Fable.Logging Tests"
    printfn "==================="

    let results = tests |> List.map (fun (name, f) -> runTest name f)
    let passed = results |> List.filter id |> List.length
    let total = results.Length

    printfn ""
    printfn "%d/%d tests passed" passed total

    if passed = total then 0 else 1
