namespace Application.App


module BlankPage =
    open Elmish
    open Avalonia.Controls
    open Avalonia.FuncUI.Components.Hosts
    open Avalonia.FuncUI.Elmish
    open Avalonia.FuncUI.DSL


    type State =
        { noop: bool }

    type Msg = Noop

    let init = { noop = false }, Cmd.none

    let update (msg: Msg) (state: State) =
        match msg with
        | Noop -> state, Cmd.none

    let view (state: State) (dispatch: Msg -> unit) =
        StackPanel.create
            [ StackPanel.spacing 8.0
              StackPanel.children [] ]
