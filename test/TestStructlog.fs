module Fable.Logging.Tests.Structlog

open Fable.Logging
open Fable.Logging.Tests.Utils

#if FABLE_COMPILER_PYTHON
open Fable.Logging.Structlog
#endif

[<Fact>]
let ``test Structlog ConsoleLoggerProvider creates logger`` () =
#if FABLE_COMPILER_PYTHON
    let provider = ConsoleLoggerProvider() :> ILoggerProvider
    let logger = provider.CreateLogger("test")
    logger.IsEnabled(LogLevel.Information) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test Structlog JsonLoggerProvider creates logger`` () =
#if FABLE_COMPILER_PYTHON
    let provider = JsonLoggerProvider() :> ILoggerProvider
    let logger = provider.CreateLogger("test")
    logger.IsEnabled(LogLevel.Information) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test Structlog logger logs info`` () =
#if FABLE_COMPILER_PYTHON
    let provider = ConsoleLoggerProvider()
    let factory = LoggerFactory.Create(fun builder -> builder.AddProvider(provider))
    let logger = factory.CreateLogger("test")
    logger.LogInformation("hello from structlog")
#else
    ()
#endif

[<Fact>]
let ``test Structlog logger logs with format args`` () =
#if FABLE_COMPILER_PYTHON
    let provider = ConsoleLoggerProvider()
    let factory = LoggerFactory.Create(fun builder -> builder.AddProvider(provider))
    let logger = factory.CreateLogger("test")
    logger.LogInformation("hello {name}", "World")
#else
    ()
#endif

[<Fact>]
let ``test Structlog logger respects minimum level`` () =
#if FABLE_COMPILER_PYTHON
    let provider = ConsoleLoggerProvider()

    let factory =
        LoggerFactory.Create(fun builder ->
            builder.AddProvider(provider)
            builder.SetMinimumLevel(LogLevel.Warning))

    let logger = factory.CreateLogger("test")
    // Should not throw - debug is filtered at factory level
    logger.LogDebug("filtered")
    logger.LogWarning("passes through")
#else
    ()
#endif

[<Fact>]
let ``test Structlog JsonLogger logs JSON`` () =
#if FABLE_COMPILER_PYTHON
    let provider = JsonLoggerProvider()
    let factory = LoggerFactory.Create(fun builder -> builder.AddProvider(provider))
    let logger = factory.CreateLogger("json-test")
    logger.LogInformation("json log entry")
#else
    ()
#endif
