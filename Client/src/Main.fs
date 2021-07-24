module Flatbix.Client.Main

open Sutil.DOM

mountElement "flatbix-app" (App.view ())
