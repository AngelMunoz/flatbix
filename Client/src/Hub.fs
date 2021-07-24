module Hub


open Fable.SignalR
open Fable.Core
open Shared.Types

let private serverHub : HubConnection<Action, obj, obj, Response, obj> =
    SignalR.connect<Action, _, _, Response, _>
        (fun hub ->
            hub
                .withUrl($"http://localhost:5000{HubRootUrl}")
                .withAutomaticReconnect()
                .useMessagePack()
                .configureLogging(LogLevel.Debug)
                .onMessage (
                    function
                    | Response.NewCount i -> JS.console.log (i)
                    | Response.RandomCharacter str -> JS.console.log (str)
                ))


let startHub () =
    serverHub.startNow ()
    serverHub
