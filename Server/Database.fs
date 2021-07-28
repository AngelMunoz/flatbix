namespace Flatbix.Server

open FSharp.Control.Tasks

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

  module private Commands =
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

  module FlbUsers =
      let Exists (email: string) =
          task {
            let! found = db().RunCommandAsync(Commands.userExists email)
            return found.n > 0 && found.ok = 1.
          }

  module Auth =
    let TrySignIn (email: string, password: string) =
      task {
        let! found = db().RunCommandAsync(Commands.findUserWithPassword email)
        match found.cursor.firstBatch |> Seq.tryHead with
        | Some user ->
          return EnhancedVerify(password, user.password)
        | None -> return false
      }

    let TrySignUp (payload: SignupPayload) =
      task {

        let payload =
          { payload with
              password = EnhancedHashPassword(payload.password) }

        let! created = db().RunCommandAsync(Commands.createUser payload)
        return (created.n > 0 && created.ok = 1.)
      }
