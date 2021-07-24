module Game

open Sutil
open Sutil.Attr

let view () =
  let store = Store.make false

  Store.set store true
  Html.article [ Attr.id "Game" ]
