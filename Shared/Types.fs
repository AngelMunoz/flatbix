module Shared.Types

open System
#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

type FlatbixUser =
  { id: Guid
    username: string
    email: string }
  static member Decoder: Decoder<FlatbixUser> =
    Decode.object
      (fun get ->
        { id = get.Required.Field "id" Decode.guid
          username = get.Required.Field "username" Decode.string
          email = get.Required.Field "email" Decode.string })

  static member Encoder(payload: FlatbixUser) =
    Encode.object [ "id", Encode.guid payload.id
                    "username", Encode.string payload.username
                    "email", Encode.string payload.email ]

type SigninPayload =
  { email: string
    password: string }

  static member Decoder: Decoder<SigninPayload> =
    Decode.object
      (fun get ->
        { email = get.Required.Field "email" Decode.string
          password = get.Required.Field "password" Decode.string })

  static member Encoder(payload: SigninPayload) =
    Encode.object [ "email", Encode.string payload.email
                    "password", Encode.string payload.password ]


type SignupPayload =
  { email: string
    password: string
    username: string }

  static member Decoder: Decoder<SignupPayload> =
    Decode.object
      (fun get ->
        { email = get.Required.Field "email" Decode.string
          password = get.Required.Field "password" Decode.string
          username = get.Required.Field "username" Decode.string })

  static member Encoder(payload: SignupPayload) =
    Encode.object [ "email", Encode.string payload.email
                    "password", Encode.string payload.password
                    "username", Encode.string payload.username ]

[<RequireQualifiedAccess>]
type Action =
  | IncrementCount of int
  | DecrementCount of int
  | RandomCharacter

[<RequireQualifiedAccess>]
type Response =
  | NewCount of int
  | RandomCharacter of string

[<Literal>]
let HubRootUrl = "/flatbix-hub"
