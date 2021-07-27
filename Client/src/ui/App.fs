module App

open Sutil
open Sutil.Attr
open Stores
open ClientTypes
open Pages

let private getPageObs (state: IStore<AppState>) =
  state .> (fun state -> state.page)

let includeNavObs (state: IStore<AppState>) =
  state
  .> (fun state ->
    match state.page with
    | Page.Game -> false
    // I'll keep it explicit in case I add/remove a page that does not need  the nav
    | Page.About
    | Page.Home
    | Page.Signin
    | Page.Signup -> true)

let setPage (page: Page) (state: IStore<AppState>) =
  state
  |> Store.modify (fun state -> { state with page = page })

let appNav (state: IStore<AppState>) =

  let isPage page =
    state .> (fun state -> state.page = page)

  let navToPage page _ =
    if (Store.current state).page = page then
      ()
    else
      setPage page state

  let nav =
    Html.nav [

      Html.button [
        onClick (navToPage Page.Home) [ StopPropagation ]
        text "Home"
      ]
      Html.button [
        onClick (navToPage Page.About) [ StopPropagation ]
        text "About"
      ]
      Html.hr []
      Html.button [
        onClick (navToPage Page.Signin) [ StopPropagation ]
        text "Signin"
      ]
      Html.button [
        onClick (navToPage Page.Signup) [ StopPropagation ]
        text "Signup"
      ]
    ]

  bindFragment (includeNavObs state)
  <| fun includeNav -> if includeNav then nav else Html.none

let content (state: IStore<AppState>) =
  bindFragment (getPageObs state)
  <| fun page ->
       match page with
       | Page.Home -> Home.View()
       | Page.About -> About.view ()
       | Page.Game -> Game.view ()
       | Page.Signin -> Signin.View()
       | Page.Signup -> Signup.view ()

let view () =
  Html.app [
    Html.article [
      appNav AppStore
      content AppStore
    ]
  ]
