module Stores

open Sutil
open ClientTypes

type AppState =
  { Page: Page
    AuthToken: string option }


let AppStore =
  Store.make { Page = Page.Home; AuthToken = None }
