module Fable.Logging.Tests.LogLevel

open Fable.Logging
open Fable.Logging.Tests.Utils

[<Fact>]
let ``test log levels have correct ordering`` () =
    (LogLevel.Trace < LogLevel.Debug) |> equal true
    (LogLevel.Debug < LogLevel.Information) |> equal true
    (LogLevel.Information < LogLevel.Warning) |> equal true
    (LogLevel.Warning < LogLevel.Error) |> equal true
    (LogLevel.Error < LogLevel.Critical) |> equal true
    (LogLevel.Critical < LogLevel.None) |> equal true

[<Fact>]
let ``test log level filtering with mock logger`` () =
    let provider = new MockLoggerProvider()

    let factory =
        LoggerFactory.Create(fun builder ->
            builder.AddProvider(provider)
            builder.SetMinimumLevel(LogLevel.Warning))

    let logger = factory.CreateLogger("test")

    logger.Log(LogLevel.Trace, "trace")
    logger.Log(LogLevel.Debug, "debug")
    logger.Log(LogLevel.Information, "info")
    logger.Log(LogLevel.Warning, "warning")
    logger.Log(LogLevel.Error, "error")
    logger.Log(LogLevel.Critical, "critical")

    // Only Warning, Error, Critical should pass through
    provider.Loggers.[0].Logs.Length |> equal 3

[<Fact>]
let ``test LogState.Create defaults`` () =
    let state = LogState.Create(LogLevel.Information, "hello")
    state.Level |> equal LogLevel.Information
    state.Format |> equal "hello"
    state.Args |> equal [||]
    state.Exception |> equal None

[<Fact>]
let ``test LogState.Create with parameters`` () =
    let args = [| box "world" |]
    let state = LogState.Create(LogLevel.Debug, "hello {name}", args)
    state.Args |> equal args

[<Fact>]
let ``test LogState.Create with exception`` () =
    let ex = System.Exception("test error")
    let state = LogState.Create(LogLevel.Error, "failed", error = ex)
    state.Exception |> equal (Some ex)
