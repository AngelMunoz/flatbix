module Auth

open Sutil

let signin () =
  Html.article [
    Html.header [ text "Welcome back!" ]
  ]

let signup () =
  Html.article [
    Html.header [ text "Welcome!" ]
  ]
