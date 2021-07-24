module ClientTypes

open Fable.Core
open Fable.Core.JS
open Browser.Types

[<Interface>]
type IGameService =
  abstract member game : obj
  abstract member startGame : container: string -> unit
  abstract member startGame : container: HTMLElement -> obj

  abstract member addScene :
    key: string
    * getScene: (obj array option -> Promise<obj>)
    * ?args: (obj array) ->
    Promise<obj>

  abstract member startScene : key: string * ?data: obj -> Promise<unit>
  abstract member stopScene : key: string * ?data: obj -> Promise<unit>
