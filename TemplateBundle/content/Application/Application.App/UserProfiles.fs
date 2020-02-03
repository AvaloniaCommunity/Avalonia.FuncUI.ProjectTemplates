namespace Application.App


module UserProfiles =
    open Avalonia.Controls
    open Avalonia.Media.Imaging
    open Avalonia.FuncUI.DSL
    open Avalonia.FuncUI.Components
    open Avalonia.FuncUI.Elmish
    open Elmish
    open Application.Core

    let loadInit() =
        Users.getUsers None None


    type State =
        { users: (Users.UserEndpoint.Result * Bitmap) array }

    type Msg =
        | SetUsers of (Users.UserEndpoint.Result * Bitmap) array
        | LoadImages of Users.UserEndpoint.Result array

    let init = { users = Array.empty }, Cmd.OfAsync.perform loadInit () LoadImages

    let update (msg: Msg) (state: State): State * Cmd<_> =
        match msg with
        | LoadImages users ->
            let loadingImgs() =
                async {
                    let! requests = users
                                    |> Array.map (fun user -> Users.getImageFromUrl user user.Picture.Large)
                                    |> Async.Parallel
                    return requests |> Array.Parallel.map (fun (user, src) -> user, new Bitmap(src))
                }
            state, Cmd.OfAsync.perform loadingImgs () SetUsers
        | SetUsers users ->
            { state with users = users }, Cmd.none

    let private userProfile (user: Users.UserEndpoint.Result, img: Bitmap) =
        WrapPanel.create
            [ WrapPanel.classes [ "userprofile" ]
              WrapPanel.children
                  [ Image.create
                      [ Image.classes [ "profileimg" ]
                        Image.source img ]
                    DockPanel.create
                        [ DockPanel.classes [ "profilepanel" ]
                          DockPanel.children
                              [ TextBlock.create
                                  [ TextBlock.classes [ "profiletitle" ]
                                    TextBlock.dock Dock.Top
                                    TextBlock.text user.Name.Title ]
                                TextBlock.create
                                    [ TextBlock.classes [ "profilename" ]
                                      TextBlock.dock Dock.Top
                                      TextBlock.text (Users.getFulNameStr user.Name) ]
                                TextBlock.create
                                    [ TextBlock.classes [ "profileemail" ]
                                      TextBlock.dock Dock.Top
                                      TextBlock.text user.Email ] ] ] ] ]

    let view (state: State) (dispatch: Msg -> unit) =
        WrapPanel.create
            [ WrapPanel.classes [ "userprofilesgrid" ]
              WrapPanel.children
                  [ for user in state.users do
                      yield userProfile user ] ]



    type Host() as this =
        inherit Hosts.HostControl()
        do
            Elmish.Program.mkProgram (fun () -> init) update view
            |> Program.withHost this
            |> Program.withConsoleTrace
            |> Program.run
