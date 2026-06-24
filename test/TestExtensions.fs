module Fable.Logging.Tests.Extensions

open Fable.Logging
open Fable.Logging.Tests.Utils

[<Fact>]
let ``test LogDebug dispatches with Debug level`` () =
    let provider = new MockLoggerProvider()

    let factory =
        LoggerFactory.Create(fun builder ->
            builder.AddProvider(provider)
            builder.SetMinimumLevel(LogLevel.Trace))

    let logger = factory.CreateLogger("test")
    logger.LogDebug("debug message")
    provider.Loggers.[0].Logs.[0].Level |> equal LogLevel.Debug

[<Fact>]
let ``test LogInformation dispatches with Information level`` () =
    let provider = new MockLoggerProvider()
    let factory = LoggerFactory.Create(fun builder -> builder.AddProvider(provider))
    let logger = factory.CreateLogger("test")
    logger.LogInformation("info message")
    provider.Loggers.[0].Logs.[0].Level |> equal LogLevel.Information

[<Fact>]
let ``test LogWarning dispatches with Warning level`` () =
    let provider = new MockLoggerProvider()
    let factory = LoggerFactory.Create(fun builder -> builder.AddProvider(provider))
    let logger = factory.CreateLogger("test")
    logger.LogWarning("warning message")
    provider.Loggers.[0].Logs.[0].Level |> equal LogLevel.Warning

[<Fact>]
let ``test LogError dispatches with Error level`` () =
    let provider = new MockLoggerProvider()
    let factory = LoggerFactory.Create(fun builder -> builder.AddProvider(provider))
    let logger = factory.CreateLogger("test")
    logger.LogError("error message")
    provider.Loggers.[0].Logs.[0].Level |> equal LogLevel.Error

[<Fact>]
let ``test LogCritical dispatches with Critical level`` () =
    let provider = new MockLoggerProvider()
    let factory = LoggerFactory.Create(fun builder -> builder.AddProvider(provider))
    let logger = factory.CreateLogger("test")
    logger.LogCritical "critical message"
    provider.Loggers.[0].Logs.[0].Level |> equal LogLevel.Critical

[<Fact>]
let ``test Log with exception`` () =
    let provider = new MockLoggerProvider()
    let factory = LoggerFactory.Create(fun builder -> builder.AddProvider(provider))
    let logger = factory.CreateLogger("test")
    let ex = System.Exception "test error"
    logger.Log(LogLevel.Error, ex, "something failed")
    let log = provider.Loggers.[0].Logs.[0]
    log.Level |> equal LogLevel.Error
    log.Exception |> equal (Some ex)

[<Fact>]
let ``test extension methods respect IsEnabled`` () =
    let provider = new MockLoggerProvider()

    let factory =
        LoggerFactory.Create(fun builder ->
            builder.AddProvider(provider)
            builder.SetMinimumLevel(LogLevel.Error))

    let logger = factory.CreateLogger("test")
    logger.LogDebug("filtered")
    logger.LogInformation("filtered")
    logger.LogWarning("filtered")
    logger.LogError("passes")
    logger.LogCritical("passes")
    provider.Loggers.[0].Logs.Length |> equal 2

[<Fact>]
let ``test LogDebug with format args`` () =
    let provider = new MockLoggerProvider()
    let factory = LoggerFactory.Create(fun builder -> builder.AddProvider(provider))
    let logger = factory.CreateLogger("test")
    logger.LogInformation("hello {name}", "World")
    let log = provider.Loggers.[0].Logs.[0]
    log.Format |> equal "hello {name}"
    log.Args.Length |> equal 1
