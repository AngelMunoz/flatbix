namespace Flatbix.Server

open FsToolkit.ErrorHandling

open type BCrypt.Net.BCrypt

open MongoDB.Bson
open MongoDB.Driver
open Mondocks.Queries
open Mondocks.Aggregation
open Mondocks.Types

open Shared.Types

module Database =

  let private client =
    lazy (MongoClient(Constants.FLATBIX_DB_URL))

  let private db () = client.Value.GetDatabase("flatbixdb")

  [<Literal>]
  let private ColUsers = "fla_users"

  module Commands =
    let findUser (email: string) =
      find ColUsers {
        filter {| email = email |}
        limit 1
      }
      |> JsonCommand<FindResult<FlatbixUser>>

    let findUserWithPassword (email: string) =
      find ColUsers {
        filter {| email = email |}
        projection {| password = 1 |}
        limit 1
      }
      |> JsonCommand<FindResult<{| password: string |}>>

    let createUser (payload: SignupPayload) =
      insert ColUsers {
        documents [ {| id = ObjectId.GenerateNewId().ToString()
                       email = payload.email
                       username = payload.username
                       pasword = payload.password |} ]
      }
      |> JsonCommand<InsertResult>

    let userExists (email: string) =
      count {
        collection ColUsers
        query {| email = email |}
      }
      |> JsonCommand<CountResult>

  module Auth =
    [<RequireQualifiedAccess>]
    type SignInError =
      | GenericError of string
      | Exists
      | NotFound
      | InvalidCredentials

    let SignIn (email: string, password: string) =
      taskResult {
        let! found =
          db()
            .RunCommandAsync(Commands.findUserWithPassword email)

        do!
          found.cursor.firstBatch
          |> Result.requireNotEmpty SignInError.NotFound

        let user = found.cursor.firstBatch |> Seq.head

        do!
          EnhancedVerify(password, user.password)
          |> Result.requireTrue SignInError.InvalidCredentials

        return true
      }
      |> TaskResult.catch (fun ex -> SignInError.GenericError ex.Message)

    [<RequireQualifiedAccess>]
    type SignUpError =
      | GenericError of string
      | Exists
      | NotFound

    let SignUp (payload: SignupPayload) =
      taskResult {

        let! exists =
          db()
            .RunCommandAsync(Commands.userExists payload.email)

        do!
          (exists.n = 1 && exists.ok = 1.0)
          |> Result.requireTrue SignUpError.NotFound

        let payload =
          { payload with
              password = EnhancedHashPassword(payload.password) }

        let! created = db().RunCommandAsync(Commands.createUser payload)

        do!
          (created.ok = 1.0 && created.n = 1)
          |> Result.requireTrue (
            SignUpError.GenericError "Could not create user"
          )
      }
