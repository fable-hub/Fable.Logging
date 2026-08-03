namespace Fable.Logging

open Fable.Core

/// Aggregating logger over a snapshot of the factory's providers.
///
/// The provider loggers are held in an immutable list on purpose. A mutable
/// collection field is enough on its own to make Fable compile the whole
/// instance to a `make_ref()` key into the process dictionary, which on the
/// BEAM confines the logger to the process that created it — reading it from a
/// spawned process fails with `{badmap,undefined}`. With no mutable state the
/// instance is a plain map and crosses process boundaries freely.
type internal Logger(name: string, providers: ILoggerProvider seq, minimumLevel: LogLevel) =
    let loggers =
        providers
        |> Seq.map (fun p -> p.CreateLogger(name))
        |> List.ofSeq

    interface ILogger with
        member _.Log(state: LogState) =
            if state.Level >= minimumLevel then
                loggers
                |> Seq.iter (fun l -> l.Log state)

        member _.IsEnabled(logLevel: LogLevel) =
            logLevel >= minimumLevel
            && loggers
               |> Seq.exists (fun l -> l.IsEnabled logLevel)

        member _.BeginScope(state: obj) = failwith "Not implemented"

[<Mangle>]
type ILoggingBuilder =
    abstract member AddProvider: ILoggerProvider -> unit
    abstract member ClearProviders: unit -> unit
    abstract member SetMinimumLevel: LogLevel -> unit

type LoggerFactory(providers: ILoggerProvider seq) =
    let providers = ResizeArray<ILoggerProvider>(providers)
    let mutable minimumLevel = LogLevel.Information

    new() = new LoggerFactory(Seq.empty)

    /// The minimum level loggers created by this factory are configured with.
    member _.MinimumLevel = minimumLevel

    interface ILoggerFactory with
        member this.CreateLogger(name) =
            Logger(name, providers, minimumLevel) :> ILogger

        /// Registers a provider for loggers created from this point on.
        /// Loggers already handed out keep the provider set they were created
        /// with — they snapshot it, so they stay free of mutable state and
        /// remain usable from other BEAM processes.
        member this.AddProvider(provider) = providers.Add(provider)

        member this.Dispose() =
            for provider in providers do
                provider.Dispose()

            providers.Clear()

    interface ILoggingBuilder with
        member x.AddProvider(provider: ILoggerProvider) = providers.Add(provider)

        member x.ClearProviders() =
            for provider in providers do
                provider.Dispose()

            providers.Clear()

        member x.SetMinimumLevel(logLevel: LogLevel) = minimumLevel <- logLevel

    member x.CreateLogger name =
        (x :> ILoggerFactory).CreateLogger(name)

    member x.AddProvider provider =
        (x :> ILoggerFactory).AddProvider(provider)

    static member Create() = new LoggerFactory()

    static member Create(configure: ILoggingBuilder -> unit) : LoggerFactory =
        let factory = new LoggerFactory()
        configure (factory :> ILoggingBuilder)
        factory
