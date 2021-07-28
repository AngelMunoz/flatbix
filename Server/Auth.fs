namespace Flatbix.Server

open System
open System.Security.Claims

open FSharp.Control.Tasks

open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging

open Giraffe
open Saturn.Auth
open FsToolkit.ErrorHandling

open Database
open Shared.Types

[<RequireQualifiedAccess>]
module Auth =

  type private AuthErrors = BadRequest of string

  let private getToken (email: string) =
    let claims = [ Claim(ClaimTypes.Email, email) ]

    let expiresAt = DateTimeOffset.Now.AddDays(1.).Date

    generateJWT
      (Constants.JWT_SECRET, "HS256")
      Constants.SERVER_URL
      expiresAt
      claims


  let SignIn (next: HttpFunc) (ctx: HttpContext) =
    task {
      let! getResult =
        taskResult {
          let! payload =
            ctx.TryBindJson<SigninPayload>()
            |> TaskResult.requireSome (
              BadRequest "Failed to parse sign in payload"
            )

          let token = getToken payload.email

          return
            {| token = token
               user = payload.email |}
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
            ctx.TryBindJson<SigninPayload>()
            |> TaskResult.requireSome (
              BadRequest "Failed to parse sign in payload"
            )

          let token = getToken payload.email

          return
            {| token = token
               user = payload.email |}
        }

      match getResult with
      | Ok result -> return! json result next ctx
      | Error (BadRequest msg) ->
        let response = json {| message = msg |}
        return! RequestErrors.badRequest response next ctx
    }
