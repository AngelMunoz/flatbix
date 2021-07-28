namespace Flatbix.Server

[<RequireQualifiedAccess>]
module Constants =
  open System

  let JWT_SECRET =
    Environment.GetEnvironmentVariable "FLATBIX_JWT_SECRET"
    |> Option.ofObj
    |> Option.defaultValue "much secret; so wow;"

  let SERVER_URL =
    Environment.GetEnvironmentVariable "FLATBIX_SERVER_URL"
    |> Option.ofObj
    |> Option.defaultValue "http://localhost:5000"

  let FLATBIX_DB_URL =
    Environment.GetEnvironmentVariable "FLATBIX_DB_URL"
    |> Option.ofObj
    |> Option.defaultValue "mongodb://192.168.100.5:27017/flatbix"
