namespace Flatbix.Server

open FSharp.Control.Tasks

open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging

open Giraffe

[<AutoOpen>]
module Extensions =
  type HttpContext with
    member this.TryBindJson<'Type>() =
      task {
        try
          let! value = this.BindJsonAsync<'Type>()
          return Some value
        with
        | ex ->
          let logger =
            this.GetLogger("HttpContext.TryBindJson")

          logger.Log(
            LogLevel.Debug,
            $"Failed to bind {nameof<'Type>}: {ex.Message}"
          )

          return None
      }
