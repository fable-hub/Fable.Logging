module Fable.Logging.Tests.BeamLogger

open Fable.Logging
open Fable.Logging.Tests.Utils

#if FABLE_COMPILER_BEAM
open Fable.Beam
open Fable.Logging.Beam

/// Reply from the spawned probe process. A crash in that process sends nothing,
/// so the receive below times out and the test fails.
type ProbeReply = Probed of bool
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

[<Fact>]
let ``test Beam logger is usable from a spawned process`` () =
#if FABLE_COMPILER_BEAM
    // Regression test: while loggers carried mutable state Fable backed them
    // with a process-dictionary ref, so every member access from another
    // process died with {badmap,undefined}. Build the logger here, use it there.
    let provider = LoggerProvider()

    let factory =
        LoggerFactory.Create(fun builder -> builder.AddProvider(provider))

    let logger = factory.CreateLogger("cross-process")
    let parent = Erlang.self<ProbeReply> ()

    Erlang.spawn<ProbeReply> (fun () ->
        let enabled = logger.IsEnabled(LogLevel.Information)
        logger.LogInformation("hello from a spawned process")
        Erlang.send parent (Probed enabled))
    |> ignore

    match Fable.Core.BeamInterop.Erlang.receive<ProbeReply> (5000) with
    | Some (Probed enabled) -> enabled |> equal true
    | None -> failwith "spawned process never replied (it most likely crashed)"
#else
    ()
#endif
