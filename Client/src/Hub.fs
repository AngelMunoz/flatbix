module Hub


open Fable.SignalR
open Fable.Core
open Fable.Core.JsInterop
open Shared.Types

let ServerUrl =
  importMember<string> "@src/environment.js"

let private serverHub: HubConnection<Action, obj, obj, Response, obj> =
  SignalR.connect<Action, _, _, Response, _>
    (fun hub ->
      hub
        .withUrl($"{ServerUrl}{HubRootUrl}")
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
