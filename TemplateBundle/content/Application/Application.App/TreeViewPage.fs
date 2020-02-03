namespace Application.App


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
                      [ { Name = "Tomato"
                          Children = [] }
                        { Name = "Apple"
                          Children = [] } ] }
                { Name = "Vegetables"
                  Children =
                      [ { Name = "Carrot"
                          Children = [] }
                        { Name = "Salad"
                          Children = [] } ] } ] }

    type State =
        { detail: Taxonomy option }

    let init = { detail = None }

    type Msg = ShowDetail of Taxonomy

    let update (msg: Msg) (state: State): State =
        match msg with
        | ShowDetail taxonomy -> { state with detail = Some taxonomy }

    let view (state: State) (dispatch: Msg -> unit) =
        DockPanel.create
            [ DockPanel.children
                [ yield TreeView.create
                            [ TreeView.dock Dock.Left
                              TreeView.dataItems [ food ]
                              TreeView.itemTemplate
                                  (DataTemplateView<Taxonomy>
                                      .create
                                          ((fun data -> data.Children),
                                           (fun data ->
                                               TextBlock.create
                                                   [ TextBlock.onTapped (fun _ -> dispatch (ShowDetail data))
                                                     TextBlock.text data.Name ]))) ]
                  match state.detail with
                  | Some taxonomy ->
                      yield StackPanel.create
                                [ StackPanel.horizontalAlignment HorizontalAlignment.Center
                                  StackPanel.spacing 8.0
                                  StackPanel.children
                                      [ TextBlock.create [ TextBlock.text (sprintf "Name: %s" taxonomy.Name) ]
                                        StackPanel.create
                                            [ StackPanel.spacing 8.0
                                              StackPanel.orientation Orientation.Horizontal
                                              StackPanel.children
                                                  [ yield TextBlock.create [ TextBlock.text "Children: " ]
                                                    for child in taxonomy.Children do
                                                        yield TextBlock.create [ TextBlock.text child.Name ] ] ] ] ]
                  | None ->
                      yield TextBlock.create
                                [ StackPanel.horizontalAlignment HorizontalAlignment.Center
                                  TextBlock.text "Select a taxonomy" ] ] ]

    type Host() as this =
        inherit Hosts.HostControl()
        do
            Elmish.Program.mkSimple (fun () -> init) update view
            |> Program.withHost this
            |> Program.withConsoleTrace
            |> Program.run
