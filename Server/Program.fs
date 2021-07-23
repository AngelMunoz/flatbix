module Flatbix.Server.Program

open FSharp.Control.Tasks


open Microsoft.AspNetCore.SignalR
open Fable.SignalR


open Saturn

open Shared.Types

let update (msg: Action) =
    match msg with
    | Action.IncrementCount i -> Response.NewCount(i + 1)
    | Action.DecrementCount i -> Response.NewCount(i - 1)
    | Action.RandomCharacter ->
        let characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"

        System.Random().Next(0, characters.Length - 1)
        |> fun i -> characters.ToCharArray().[i]
        |> string
        |> Response.RandomCharacter

let sInvoke (msg: Action) _ = task { return update msg }

let sSend (msg: Action) (hubContext: FableHub<Action, Response>) =
    update msg |> hubContext.Clients.Caller.Send

[<EntryPoint>]
let main args =
    let app =
        application {
            use_developer_exceptions
            use_static "wwwroot"
            no_router


            use_signalr (
                configure_signalr {
                    endpoint Endpoints.Root
                    send sSend
                    invoke sInvoke
                    use_messagepack
                    with_log_level Microsoft.Extensions.Logging.LogLevel.Debug
                }
            )

            use_cors
                "Any"
                (fun policy ->
                    policy
                        .WithOrigins("http://localhost:8080", "http://127.0.0.1:8080")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                    |> ignore)
        }

    run app
    0
