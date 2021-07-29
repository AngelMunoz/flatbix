[<AutoOpen>]
module Pages.Signin

open Browser.Types
open Fable.Core.JsInterop

open Sutil
open Sutil.Attr

open Sutil.DOM
open Shared.Types
open Stores

open Thoth.Fetch
open Thoth.Json
open Fetch


let ServerUrl: string = importMember "@src/environment.js"


let private getEmailObs (state: IStore<SigninPayload>) =
  state .> (fun s -> s.email)

let private updateEmail (store: IStore<SigninPayload>) newEmail =
  store
  |> Store.modify
       (fun (state: SigninPayload) -> { state with email = newEmail })

let private getPasswordObs (state: IStore<SigninPayload>) =
  state .> (fun s -> s.password)

let private updatePassword store newPassword =
  store
  |> Store.modify
       (fun (state: SigninPayload) -> { state with password = newPassword })

let private trySingin (payload: SigninPayload) =
  promise {
    let! result =
      Fetch.tryPost<SigninPayload, {| token: string; user: string |}> (
        $"{ServerUrl}/auth/signin",
        payload,
        headers =
          [ Accept "application/json"
            ContentType "application/json" ]
      )

    return
      match result with
      | Ok value -> Some value.token
      | Result.Error (err) ->
        eprintf "%A" err
        None
  }


let private submitForm store (e: Event) =
  e.preventDefault ()

  promise {
    match! trySingin (Store.current store) with
    | Some token ->
      Store.modify
        (fun appstore -> { appstore with AuthToken = Some token })
        AppStore
    | None -> ()
  }
  |> Promise.start


type Signin =
  static member View(?isWidget: bool) =
    let formState = Store.make { email = ""; password = "" }

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
              bindAttrBoth
                "value"
                (getEmailObs formState)
                (updateEmail formState)
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
              bindAttrBoth
                "value"
                (getPasswordObs formState)
                (updatePassword formState)
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
