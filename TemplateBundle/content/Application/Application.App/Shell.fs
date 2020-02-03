namespace Application.App

open Avalonia.Input

module Shell =
    open Elmish
    open Avalonia
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.FuncUI
    open Avalonia.FuncUI.Builder
    open Avalonia.FuncUI.Components.Hosts
    open Avalonia.FuncUI.Elmish

    type State =
        { count: int
          blankpageState: BlankPage.State }

    type Msg =
        | Increment
        | Decrement
        | Reset
        | BlankPageMsg of BlankPage.Msg

    let init =
        let bpState, bpCmd = BlankPage.init
        { count = 0
          blankpageState = bpState },
        /// you can add more init commands as you need
        Cmd.batch [ bpCmd ]

    let update (msg: Msg) (state: State): State * Cmd<_> =
        match msg with
        | Increment ->
            { state with count = state.count + 1 }, Cmd.none
        | Decrement ->
            { state with count = state.count - 1 }, Cmd.none
        | Reset -> init
        | BlankPageMsg bpmsg ->
            let bpState, cmd =
                BlankPage.update bpmsg state.blankpageState
            { state with blankpageState = bpState }, Cmd.map BlankPageMsg cmd

    let view (state: State) (dispatch) =
        DockPanel.create
            [ DockPanel.children
                [ TabControl.create
                    [ TabControl.tabStripPlacement Dock.Top
                      TabControl.viewItems
                          [ TabItem.create
                              [ TabItem.header "Blank Page"
                                TabItem.content (BlankPage.view state.blankpageState (BlankPageMsg >> dispatch)) ]
                            TabItem.create
                                [ TabItem.header "TreeView Page"
                                  TabItem.content (ViewBuilder.Create<TreeViewPage.Host>([])) ]
                            TabItem.create
                                [ TabItem.header "User Profiles Page"
                                  TabItem.content (ViewBuilder.Create<UserProfiles.Host>([])) ] ] ] ] ]

    type MainWindow() as this =
        inherit HostWindow()
        do
            base.Title <- "Application.App"
            base.Width <- 800.0
            base.Height <- 600.0
            base.MinWidth <- 800.0
            base.MinHeight <- 600.0

            //this.VisualRoot.VisualRoot.Renderer.DrawFps <- true
            //this.VisualRoot.VisualRoot.Renderer.DrawDirtyRects <- true
#if DEBUG
            this.AttachDevTools(KeyGesture(Key.F12))
#endif

            Elmish.Program.mkProgram (fun () -> init) update view
            |> Program.withHost this
#if DEBUG
            |> Program.withConsoleTrace
#endif
            |> Program.run
