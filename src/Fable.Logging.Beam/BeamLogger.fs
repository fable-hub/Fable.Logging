module Fable.Logging.Beam

open Fable.Beam

let private toErlangLevel (level: LogLevel) =
    match level with
    | LogLevel.Trace
    | LogLevel.Debug -> Atom.ofString "debug"
    | LogLevel.Information -> Atom.ofString "info"
    | LogLevel.Warning -> Atom.ofString "warning"
    | LogLevel.Error -> Atom.ofString "error"
    | LogLevel.Critical -> Atom.ofString "critical"
    | _ -> Atom.ofString "info"

/// Logger writing to the OTP `logger` module.
///
/// Deliberately has no mutable instance state: Fable compiles a class with a
/// settable member to a `make_ref()` key into the process dictionary, which
/// makes the instance unusable from any process other than the one that built
/// it. Without it the instance is a plain map, so it can be sent between
/// processes, stored in ETS, or closed over by a spawned process.
type Logger(name: string, minimumLevel: LogLevel) =

    new(name: string) = Logger(name, LogLevel.Trace)

    member _.MinimumLevel = minimumLevel

    interface ILogger with
        member _.Log(state: LogState) =
            let level = state.Level

            if level >= minimumLevel then
                let message, _ = Common.translateFormat name state.Format state.Args

                match level with
                | LogLevel.Debug -> Fable.Beam.Logger.logger.debug (message)
                | LogLevel.Information -> Fable.Beam.Logger.logger.info (message)
                | LogLevel.Warning -> Fable.Beam.Logger.logger.warning (message)
                | LogLevel.Error -> Fable.Beam.Logger.logger.error (message)
                | LogLevel.Critical -> Fable.Beam.Logger.logger.critical (message)
                | _ -> Fable.Beam.Logger.logger.info (message)

        member _.IsEnabled(logLevel: LogLevel) = logLevel >= minimumLevel
        member _.BeginScope(_) : System.IDisposable = failwith "Not implemented"

type LoggerProvider(?minimumLevel: LogLevel) =
    let level = defaultArg minimumLevel LogLevel.Trace

    do
        Fable.Beam.Logger.logger.set_primary_config (Atom.ofString "level", toErlangLevel level)
        |> ignore

    interface ILoggerProvider with
        member _.CreateLogger(name) = Logger(name, level)

        member _.Dispose() = ()
