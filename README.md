# Fable.Logging

A cross-platform logging framework for [Fable](https://fable.io). It mirrors the
.NET [Microsoft.Extensions.Logging](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging)
pattern with `ILogger`, `ILoggerFactory`, and `ILoggerProvider` interfaces, letting you
write idiomatic logging code in F# that works across JavaScript, Python, and Erlang/BEAM.

## Packages

| Package | NuGet | Description |
|---------|-------|-------------|
| `Fable.Logging` | [![NuGet](https://img.shields.io/nuget/v/Fable.Logging.svg)](https://www.nuget.org/packages/Fable.Logging) | Core interfaces, LoggerFactory, ConsoleLogger, JS console logger |
| `Fable.Logging.Structlog` | [![NuGet](https://img.shields.io/nuget/v/Fable.Logging.Structlog.svg)](https://www.nuget.org/packages/Fable.Logging.Structlog) | Python [structlog](https://www.structlog.org/) provider |
| `Fable.Logging.Beam` | [![NuGet](https://img.shields.io/nuget/v/Fable.Logging.Beam.svg)](https://www.nuget.org/packages/Fable.Logging.Beam) | Erlang/OTP logger provider |

## Quick Start

Create a logger factory, configure it with providers, and start logging:

```fsharp
open Fable.Logging

let factory =
    LoggerFactory.Create(fun builder ->
        builder.AddProvider(ConsoleLoggerProvider())
        builder.SetMinimumLevel(LogLevel.Debug))

let logger = factory.CreateLogger("MyApp.Service")

logger.LogInformation("Application started")
logger.LogDebug("Processing request for {UserId}", 42)
logger.LogError("Something went wrong")
```

## Platform-Specific Providers

### JavaScript (console)

The built-in JS logger maps log levels to the appropriate `console.*` methods
(`console.debug`, `console.info`, `console.warn`, `console.error`).

```fsharp
open Fable.Logging
open Fable.Logging.JS

let factory =
    LoggerFactory.Create(fun builder ->
        builder.AddProvider(LoggerProvider()))

let logger = factory.CreateLogger("MyApp")
logger.LogInformation("Hello from {Platform}!", "JavaScript")
```

### Python (structlog)

Uses [structlog](https://www.structlog.org/) for structured logging with support for
both console and JSON output.

```fsharp
open Fable.Logging
open Fable.Logging.Structlog

// Console output (human-readable)
let factory =
    LoggerFactory.Create(fun builder ->
        builder.AddProvider(ConsoleLoggerProvider()))

// JSON output (machine-readable)
let jsonFactory =
    LoggerFactory.Create(fun builder ->
        builder.AddProvider(JsonLoggerProvider()))

let logger = factory.CreateLogger("MyApp")
logger.LogInformation("User {Name} logged in", "Alice")
```

### Erlang/BEAM (OTP logger)

Bridges to the OTP `logger` module for applications targeting the BEAM runtime.

```fsharp
open Fable.Logging
open Fable.Logging.Beam

let factory =
    LoggerFactory.Create(fun builder ->
        builder.AddProvider(LoggerProvider()))

let logger = factory.CreateLogger("MyApp")
logger.LogWarning("Connection pool running low: {Available} remaining", 3)
```

## Log Levels

Log levels match the .NET `LogLevel` enum:

| Level | Value | Method | Description |
|-------|-------|--------|-------------|
| Trace | 0 | `LogTrace` | Most detailed messages, may contain sensitive data |
| Debug | 1 | `LogDebug` | Debugging and development |
| Information | 2 | `LogInformation` | General flow of the application |
| Warning | 3 | `LogWarning` | Abnormal or unexpected events |
| Error | 4 | `LogError` | Errors and exceptions |
| Critical | 5 | `LogCritical` | Failures requiring immediate attention |
| None | 6 | | Suppresses all logging |

## Filtering

Set a minimum log level to filter out less severe messages:

```fsharp
let factory =
    LoggerFactory.Create(fun builder ->
        builder.AddProvider(ConsoleLoggerProvider())
        builder.SetMinimumLevel(LogLevel.Warning))

let logger = factory.CreateLogger("MyApp")
logger.LogDebug("This is filtered out")
logger.LogWarning("This is logged")
```

## Message Templates

Use named placeholders in log messages for structured logging. The placeholder names
become keys in the structured log output, while values are substituted positionally:

```fsharp
logger.LogInformation("Order {OrderId} placed by {Customer}", 1234, "Alice")
// Output: MyApp - Order 1234 placed by Alice
```

## Logging Exceptions

```fsharp
try
    failwith "Something broke"
with ex ->
    logger.LogError("Operation failed", ex)
```

## Multiple Providers

The factory dispatches log messages to all registered providers:

```fsharp
let factory =
    LoggerFactory.Create(fun builder ->
        builder.AddProvider(consoleProvider)
        builder.AddProvider(jsonProvider))

// Messages are sent to both providers
let logger = factory.CreateLogger("MyApp")
logger.LogInformation("This goes to all providers")
```

## Writing a Custom Provider

Implement `ILoggerProvider` and `ILogger` to create your own logging backend:

```fsharp
open Fable.Logging

type MyLogger(name: string) =
    interface ILogger with
        member _.Log(state: LogState) =
            printfn "[%A] %s: %s" state.Level name state.Format

        member _.IsEnabled(logLevel: LogLevel) = true
        member _.BeginScope(_) = failwith "Not implemented"

type MyLoggerProvider() =
    interface ILoggerProvider with
        member _.CreateLogger(name) = MyLogger(name)
        member _.Dispose() = ()
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
