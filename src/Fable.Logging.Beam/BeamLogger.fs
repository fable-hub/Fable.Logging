module Fable.Logging.Beam

open Fable.Beam

let private toErlangLevel (level: LogLevel) =
    match level with
    | LogLevel.Trace
    | LogLevel.Debug -> Atom "debug"
    | LogLevel.Information -> Atom "info"
    | LogLevel.Warning -> Atom "warning"
    | LogLevel.Error -> Atom "error"
    | LogLevel.Critical -> Atom "critical"
    | _ -> Atom "info"

type Logger(name: string) =

    member val MinimumLevel = LogLevel.Trace with get, set

    interface ILogger with
        member x.Log(state: LogState) =
            let level = state.Level

            if level >= x.MinimumLevel then
                let message, _ = Common.translateFormat name state.Format state.Args

                match level with
                | LogLevel.Debug -> Fable.Beam.Logger.logger.debug (message)
                | LogLevel.Information -> Fable.Beam.Logger.logger.info (message)
                | LogLevel.Warning -> Fable.Beam.Logger.logger.warning (message)
                | LogLevel.Error -> Fable.Beam.Logger.logger.error (message)
                | LogLevel.Critical -> Fable.Beam.Logger.logger.critical (message)
                | _ -> Fable.Beam.Logger.logger.info (message)

        member x.IsEnabled(logLevel: LogLevel) = logLevel >= x.MinimumLevel
        member _.BeginScope(_) : System.IDisposable = failwith "Not implemented"

type LoggerProvider(?minimumLevel: LogLevel) =
    let level = defaultArg minimumLevel LogLevel.Trace

    do Fable.Beam.Logger.logger.set_primary_config (Atom "level", toErlangLevel level) |> ignore

    interface ILoggerProvider with
        member _.CreateLogger(name) =
            let logger = Logger(name)
            logger.MinimumLevel <- level
            logger

        member _.Dispose() = ()
