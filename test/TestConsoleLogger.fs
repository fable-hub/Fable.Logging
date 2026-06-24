module Fable.Logging.Tests.ConsoleLogger

open Fable.Logging
open Fable.Logging.Tests.Utils

[<Fact>]
let ``test ConsoleLogger implements ILogger`` () =
    let logger = ConsoleLogger("test") :> ILogger
    logger.IsEnabled(LogLevel.Information) |> equal true

[<Fact>]
let ``test ConsoleLogger is always enabled`` () =
    let logger = ConsoleLogger("test") :> ILogger
    logger.IsEnabled(LogLevel.Trace) |> equal true
    logger.IsEnabled(LogLevel.Debug) |> equal true
    logger.IsEnabled(LogLevel.Critical) |> equal true

[<Fact>]
let ``test ConsoleLogger logs without error`` () =
    let logger = ConsoleLogger("test") :> ILogger
    // Should not throw
    logger.Log(LogLevel.Information, "hello")
    logger.Log(LogLevel.Debug, "debug message")

[<Fact>]
let ``test ConsoleLogger logs with format args`` () =
    let logger = ConsoleLogger("test") :> ILogger
    // ConsoleLogger uses String.Format internally, so use indexed placeholders
    logger.Log(LogLevel.Information, "hello {0}", "World")
