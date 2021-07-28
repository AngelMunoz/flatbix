namespace Flatbix.Server

open FSharp.Control.Tasks

open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open System.Security.Claims

open Giraffe
open Saturn.Auth
open FsToolkit.ErrorHandling

open Shared.Types
open System

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
            tryBindJson<SigninPayload> ctx
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
            tryBindJson<SignupPayload> ctx
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
