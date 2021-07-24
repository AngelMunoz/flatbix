module Shared.Types


type SigninPayload = { email: string; password: string }

type SignupPayload =
    { email: string
      password: string
      username: string }


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
