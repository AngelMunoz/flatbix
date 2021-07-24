namespace Flatbix.Server

open FSharp.Control.Tasks
open Giraffe
open Shared.Types
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open FsToolkit.ErrorHandling

[<RequireQualifiedAccess>]
module Auth =

    type private AuthErrors = BadRequest of string


    let tryBindJson<'Type> (ctx: HttpContext) =
        task {
            try
                let! value = ctx.BindJsonAsync<'Type>()
                return Some value
            with
            | ex ->
                let logger = ctx.GetLogger("Auth: tryBindJson")
                logger.Log(LogLevel.Debug, ex.Message)
                return None
        }

    let SignIn (next: HttpFunc) (ctx: HttpContext) =
        task {
            let! getResult =
                taskResult {
                    let! payload =
                        tryBindJson<SigninPayload> ctx
                        |> TaskResult.requireSome (BadRequest "Failed to parse sign in payload")

                    return {|  |}
                }

            match getResult with
            | Ok result -> return! json result next ctx
            | Error (BadRequest msg) ->
                let response = json {| message = msg |}
                return! RequestErrors.badRequest response next ctx
        }

    let Signup (next: HttpFunc) (ctx: HttpContext) =
        task {
            let! getResult =
                taskResult {
                    let! payload =
                        tryBindJson<SignupPayload> ctx
                        |> TaskResult.requireSome (BadRequest "Failed to parse sign in payload")

                    return {|  |}
                }

            match getResult with
            | Ok result -> return! json result next ctx
            | Error (BadRequest msg) ->
                let response = json {| message = msg |}
                return! RequestErrors.badRequest response next ctx
        }
