namespace QuickStart

/// This is the TreeView sample
/// you can use the Host to create a view control that has an independent program
/// if you want to be aware of the control's state  you should to refer to  Shell.fs and BlankPage.fs
/// and see how they relate to each other
module TreeViewPage =
    open Avalonia.Controls
    open Avalonia.Layout
    open Avalonia.FuncUI.DSL
    open Avalonia.FuncUI.Components
    open Avalonia.FuncUI.Elmish
    open Elmish

    type Taxonomy =
        { Name: string
          Children: Taxonomy seq }

    let food =
        { Name = "Food"
          Children =
              [ { Name = "Fruit"
                  Children =
                      [ { Name = "Tomato"; Children = [] }
                        { Name = "Apple"; Children = [] } ] }
                { Name = "Vegetables"
                  Children =
                      [ { Name = "Carrot"; Children = [] }
                        { Name = "Salad"; Children = [] } ] } ] }

    type State = { detail: Taxonomy option }

    let init = { detail = None }

    type Msg = ShowDetail of Taxonomy

    let update (msg: Msg) (state: State): State =
        match msg with
        | ShowDetail taxonomy -> { state with detail = Some taxonomy }

    let view (state: State) (dispatch: Msg -> unit) =
        DockPanel.create [
            DockPanel.children [
                TreeView.create [
                    TreeView.dock Dock.Left
                    /// dataItems refers to the source of your control's data
                    /// these are going to be iterated to fill your template's contents
                    TreeView.dataItems [ food ]
                    TreeView.itemTemplate
                        /// You can pass the type of your data collection
                        /// to have a safe type reference in the create function
                        (DataTemplateView<Taxonomy>
                            .create((fun data -> data.Children),
                                    (fun data ->
                                        TextBlock.create [
                                            TextBlock.onTapped (fun _ -> dispatch (ShowDetail data))
                                            TextBlock.text data.Name
                                        ])))
                ]
                /// Use Pattern Matching to decide what you want to show
                /// based on your state's content
                match state.detail with
                | Some taxonomy ->
                    StackPanel.create [
                        StackPanel.horizontalAlignment HorizontalAlignment.Center
                        StackPanel.spacing 8.0
                        StackPanel.children [
                            TextBlock.create [
                                TextBlock.text (sprintf "Name: %s" taxonomy.Name)
                            ]
                            StackPanel.create [
                                StackPanel.spacing 8.0
                                StackPanel.orientation Orientation.Horizontal
                                StackPanel.children [
                                    TextBlock.create [
                                        TextBlock.text "Children: "
                                    ]
                                    for child in taxonomy.Children do
                                        TextBlock.create [
                                            TextBlock.text child.Name
                                        ]
                                ]
                            ]
                        ]
                    ]
                | None ->
                    TextBlock.create [
                        StackPanel.horizontalAlignment HorizontalAlignment.Center
                        TextBlock.text "Select a taxonomy"
                    ]
            ]
        ]

    type Host() as this =
        inherit Hosts.HostControl()

        do
            /// You can use `.mkProgram` to pass Commands around
            /// if you decide to use it, you have to also return a Command in the initFn
            /// (init, Cmd.none)
            /// you can learn more at https://elmish.github.io/elmish/basics.html
            let startFn () = init

            Elmish.Program.mkSimple startFn update view
            |> Program.withHost this
#if DEBUG
            |> Program.withConsoleTrace
#endif
            |> Program.run
