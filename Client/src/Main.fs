module Flatbix.Client.Main

open Fable.SignalR
open Fable.Core
open Shared.Types

let start () = printfn "Olv root: %s" Endpoints.Root

let private serverHub : HubConnection<Action, obj, obj, Response, obj> =
    SignalR.connect<Action, _, _, Response, _>
        (fun hub ->
            hub
                .withUrl($"http://localhost:5000{Endpoints.Root}")
                .withAutomaticReconnect()
                .useMessagePack()
                .configureLogging(LogLevel.Debug)
                .onMessage (
                    function
                    | Response.NewCount i -> JS.console.log (i)
                    | Response.RandomCharacter str -> JS.console.log (str)
                ))

serverHub.startNow ()
