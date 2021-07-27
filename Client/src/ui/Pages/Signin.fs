[<AutoOpen>]
module Pages.Signin

open Sutil
open Sutil.Attr
open Stores
open Sutil.DOM
open Browser.Types

type private FormValues =
  { email: string
    password: string
    rememberMe: bool }

let private getEmailObs state = state .> (fun s -> s.email)

let private updateEmail store newEmail =
  store
  |> Store.modify (fun state -> { state with email = newEmail })

let private getPasswordObs state = state .> (fun s -> s.password)

let private updatePassword store newPassword =
  store
  |> Store.modify (fun state -> { state with password = newPassword })

let private getRememberMeObs state = state .> (fun s -> s.rememberMe)

let private updateRememberMe store rememberMe =
  store
  |> Store.modify (fun state -> { state with rememberMe = rememberMe })


let private submitForm store (e: Event) =
  e.preventDefault ()
  Store.current store |> printfn "%A"
  ()

type Signin =
  static member View(?isWidget: bool) =
    let formState =
      Store.make
        { email = ""
          password = ""
          rememberMe = false }

    let isWidgetCls =
      if isWidget |> Option.defaultValue false then
        "widget"
      else
        ""

    Html.article [
      disposeOnUnmount [ formState ]
      class' $"{isWidgetCls}"
      Html.form [
        on "submit" (submitForm formState) []

        Html.fieldSet [
          Html.legend "Welcome to Flatbix let's get in!"
          Html.section [
            Html.label [
              Attr.for' "email"
              text "level100@flatbix.com"
            ]
            Html.input [
              Attr.type' "email"
              Attr.placeholder "Email"
              Attr.id "email"
              Attr.name "email"
              Attr.required true
              bindAttrBoth "value" (getEmailObs formState) (updateEmail formState)
            ]
          ]
          Html.section [
            Html.label [
              Attr.for' "password"
              text "Your Super Secret Password"
            ]
            Html.input [
              Attr.type' "password"
              Attr.placeholder "$super$ecret paxxword"
              Attr.id "password"
              Attr.name "password"
              Attr.required true
              bindAttrBoth "value" (getPasswordObs formState) (updatePassword formState)
            ]
          ]
          Html.section [
            Html.label [
              Attr.for' "rememberMe"
              text "Remember me "
              Html.input [
                Attr.type' "checkbox"
                Attr.id "rememberMe"
                Attr.name "rememberMe"
                bindAttrBoth "checked" (getRememberMeObs formState) (updateRememberMe formState)
              ]
            ]
          ]
        ]
        Html.section [
          Html.button [
            text "Sign in"
            Attr.type' "submit"
          ]
        ]
      ]
    ]
