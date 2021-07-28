namespace Flatbix.Server

open System
open System.Security.Claims
open System.Threading.Tasks

open FSharp.Control.Tasks

open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging

open Giraffe
open Saturn.Auth
open FsToolkit.ErrorHandling

open Database
open Shared.Types
open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
module Auth =

  type private AuthErrors =
      | BadRequest of string

  let private getToken (email: string) =
    let claims = [ Claim(ClaimTypes.Email, email) ]

    let expiresAt = DateTimeOffset.Now.AddDays(1.).Date

    generateJWT
      (Constants.JWT_SECRET, "HS256")
      Constants.SERVER_URL
      expiresAt
      claims


  let SignIn (next: HttpFunc) (ctx: HttpContext) =
    let verifyCredentials (payload: Task<Result<SigninPayload, AuthErrors>>) =
        taskResult {
            let! payload = payload
            do! Auth.TrySignIn (payload.email, payload.password) |> TaskResult.requireTrue (BadRequest "InvalidCredentials")
            return payload
        }
    let successResponse (email: string) =
        result {
            return
              {| token = email |> getToken
                 user = email |}
        }
    let getPayload() =
      taskResult {
        let! payload =
          ctx.TryBindJson<SigninPayload>()
          |> TaskResult.requireSome (
            BadRequest "Failed to parse sign in payload"
          )
        do! FlbUsers.Exists payload.email |> TaskResult.requireTrue (BadRequest "InvalidCredentials")
        return payload
      }
    task {
      match! getPayload() |> verifyCredentials   with
      | Ok result ->
          let response = successResponse result.email
          return! json response next ctx
      | Error (BadRequest msg) ->
        let response = json {| message = msg |}
        return! RequestErrors.badRequest response next ctx
    }
  type private SignupErrors =
    | BadRequest of string
    | Exists
    | UnsafePassword
    | FailedToCreateUser

  let Signup (next: HttpFunc) (ctx: HttpContext) =
    let checkPassword (pw: string) =
          let hasNumber = new Regex(@"[0-9]+");
          let hasUpperChar = new Regex(@"[A-Z]+");
          let hasLowerChar = new Regex(@"[a-z]+");
          hasNumber.IsMatch(pw) && hasUpperChar.IsMatch(pw) && hasLowerChar.IsMatch(pw) && pw.Length > 6

    task {
      let! getResult =
        taskResult {
          let! payload =
            ctx.TryBindJson<SignupPayload>()
            |> TaskResult.requireSome (
              BadRequest "Failed to parse sign up payload"
            )
          do! checkPassword payload.password |> Result.requireTrue UnsafePassword
          do! FlbUsers.Exists payload.email |> TaskResult.requireFalse Exists

          do! Auth.TrySignUp payload |> TaskResult.requireTrue FailedToCreateUser

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
      | Error Exists ->
        let response = json {| message = "That emails already exists in our database" |}
        return! RequestErrors.conflict response next ctx
      | Error UnsafePassword ->
        let response = json {| message = "That emails already exists in our database" |}
        return! RequestErrors.badRequest response next ctx
      | Error FailedToCreateUser ->
        let response = json {| message = "We were unable to create that user" |}
        return! ServerErrors.internalError response next ctx
    }
