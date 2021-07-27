module Stores

open Sutil
open ClientTypes

type AppState = { page: Page }


let AppStore = Store.make { page = Page.Home }
