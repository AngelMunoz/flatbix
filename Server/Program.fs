module Flatbix.Server.Program

open System

open FSharp.Control.Tasks

open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Builder

open Fable.SignalR

open Giraffe
open Saturn
open Saturn.Endpoint

open Thoth.Json.Giraffe

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

let apiPipeline =
  pipeline {
    plug acceptJson
    plug requestId
    set_header "x-flatbix-pipeline" "api"
  }

let apiRouter =
  router {
    pipe_through apiPipeline
    post "/auth/signin" Auth.SignIn
    post "/auth/signup" Auth.Signup
  }

[<EntryPoint>]
let main args =
  let app =
    application {
      use_developer_exceptions
      use_endpoint_router apiRouter
      use_static "wwwroot"
      use_jwt_authentication Constants.JWT_SECRET "http://locahost:5000"

      use_json_serializer (
        ThothSerializer(
          Thoth.Json.Net.CaseStrategy.CamelCase,
          skipNullField = true
        )
      )

      use_signalr (
        configure_signalr {
          endpoint HubRootUrl
          send sSend
          invoke sInvoke
          use_messagepack
          with_log_level LogLevel.Debug
          with_after_routing (fun app -> app.UseAuthorization())
          with_endpoint_config (fun builder -> builder.RequireAuthorization())
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
