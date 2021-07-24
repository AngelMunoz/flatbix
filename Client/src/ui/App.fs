module App

open Sutil
open Sutil.Attr
open Fable.Core.JS
open Fable.Core.JsInterop
open ClientTypes

let GameService =
    importDefault<IGameService> "../index.js"

let MainSceneFactory =
    importMember<obj array option -> Promise<obj>> "../game/scenes/index.js"

let view () =
    let startGame () =
        GameService.startGame "Game"

        promise {
            let! _ = GameService.addScene ("MainScene", MainSceneFactory)

            let! _ = GameService.startScene ("MainScene")
            ()
        }

    Html.app [ Html.article [ Html.h1 [ text "Hello world!" ]
                              Html.button [ onClick (fun _ -> startGame () |> ignore) []
                                            text "Start Game" ]
                              Html.section [ Attr.id "Game" ] ] ]
