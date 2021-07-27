[<RequireQualifiedAccess>]
module Pages.Home

open Sutil
open Pages

let View () =
  Html.article [
    Html.header [ Html.h1 "Flatbix!" ]
    Html.aside [ Signin.View(true) ]
    Html.section [
      Html.p [
        text
          """Flatbix is an attempt of a game by a dude who knows nothing about games
             and who is just trying to learn something new.
             So... Don't expect too much from this game.
          """
      ]
    ]
  ]
