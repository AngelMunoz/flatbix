module Shared.Types


[<RequireQualifiedAccess>]
type Action =
    | IncrementCount of int
    | DecrementCount of int
    | RandomCharacter

[<RequireQualifiedAccess>]
type Response =
    | NewCount of int
    | RandomCharacter of string

module Endpoints =
    [<Literal>]
    let Root = "/SignalR"
