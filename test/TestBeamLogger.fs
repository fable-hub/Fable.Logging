module Fable.Logging.Tests.BeamLogger

open Fable.Logging
open Fable.Logging.Tests.Utils

#if FABLE_COMPILER_BEAM
open Fable.Logging.Beam
#endif

[<Fact>]
let ``test Beam LoggerProvider creates logger`` () =
#if FABLE_COMPILER_BEAM
    let provider = LoggerProvider() :> ILoggerProvider
    let logger = provider.CreateLogger("test")
    logger.IsEnabled(LogLevel.Information) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test Beam logger logs info`` () =
#if FABLE_COMPILER_BEAM
    let provider = LoggerProvider()
    let factory = LoggerFactory.Create(fun builder -> builder.AddProvider(provider))
    let logger = factory.CreateLogger("beam-test")
    logger.LogInformation("hello from beam logger")
#else
    ()
#endif

[<Fact>]
let ``test Beam logger logs all levels`` () =
#if FABLE_COMPILER_BEAM
    let provider = LoggerProvider()
    let factory = LoggerFactory.Create(fun builder -> builder.AddProvider(provider))
    let logger = factory.CreateLogger("beam-test")
    logger.LogDebug("debug message")
    logger.LogInformation("info message")
    logger.LogWarning("warning message")
    logger.LogError("error message")
    logger.LogCritical("critical message")
#else
    ()
#endif

[<Fact>]
let ``test Beam logger logs with format args`` () =
#if FABLE_COMPILER_BEAM
    let provider = LoggerProvider()
    let factory = LoggerFactory.Create(fun builder -> builder.AddProvider(provider))
    let logger = factory.CreateLogger("beam-test")
    logger.LogInformation("hello {name}", "World")
#else
    ()
#endif

[<Fact>]
let ``test Beam logger respects minimum level`` () =
#if FABLE_COMPILER_BEAM
    let provider = LoggerProvider()

    let factory =
        LoggerFactory.Create(fun builder ->
            builder.AddProvider(provider)
            builder.SetMinimumLevel(LogLevel.Warning))

    let logger = factory.CreateLogger("beam-test")
    logger.LogDebug("filtered")
    logger.LogWarning("passes through")
#else
    ()
#endif
