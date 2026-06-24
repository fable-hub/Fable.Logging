module Fable.Logging.Tests.JSLogger

open Fable.Logging
open Fable.Logging.Tests.Utils

#if FABLE_COMPILER_JAVASCRIPT
open Fable.Logging.JS
#endif

[<Fact>]
let ``test JS LoggerProvider creates logger`` () =
#if FABLE_COMPILER_JAVASCRIPT
    let provider = LoggerProvider() :> ILoggerProvider
    let logger = provider.CreateLogger("test")
    logger.IsEnabled(LogLevel.Information) |> equal true
#else
    ()
#endif

[<Fact>]
let ``test JS logger logs info`` () =
#if FABLE_COMPILER_JAVASCRIPT
    let provider = LoggerProvider()
    let factory = LoggerFactory.Create(fun builder -> builder.AddProvider(provider))
    let logger = factory.CreateLogger("js-test")
    logger.LogInformation("hello from JS logger")
#else
    ()
#endif

[<Fact>]
let ``test JS logger logs all levels`` () =
#if FABLE_COMPILER_JAVASCRIPT
    let provider = LoggerProvider()
    let factory = LoggerFactory.Create(fun builder -> builder.AddProvider(provider))
    let logger = factory.CreateLogger("js-test")
    logger.LogDebug("debug message")
    logger.LogInformation("info message")
    logger.LogWarning("warning message")
    logger.LogError("error message")
    logger.LogCritical("critical message")
#else
    ()
#endif

[<Fact>]
let ``test JS logger logs with format args`` () =
#if FABLE_COMPILER_JAVASCRIPT
    let provider = LoggerProvider()
    let factory = LoggerFactory.Create(fun builder -> builder.AddProvider(provider))
    let logger = factory.CreateLogger("js-test")
    logger.LogInformation("hello {name}", "World")
#else
    ()
#endif

[<Fact>]
let ``test JS logger respects minimum level`` () =
#if FABLE_COMPILER_JAVASCRIPT
    let provider = LoggerProvider()

    let factory =
        LoggerFactory.Create(fun builder ->
            builder.AddProvider(provider)
            builder.SetMinimumLevel(LogLevel.Warning))

    let logger = factory.CreateLogger("js-test")
    logger.LogDebug("filtered")
    logger.LogWarning("passes through")
#else
    ()
#endif
