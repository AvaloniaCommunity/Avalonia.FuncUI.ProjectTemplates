namespace QuickStart.App

/// This is the UserProfiles sample
/// you can use the Host to create a view control that has an independent program
/// if you want to be aware of the control's state  you should to refer to  Shell.fs and BlankPage.fs
/// and see how they relate to each other
module UserProfiles =
    open Avalonia.Controls
    open Avalonia.Media.Imaging
    open Avalonia.FuncUI.DSL
    open Avalonia.FuncUI.Components
    open Avalonia.FuncUI.Elmish
    open Elmish
    open QuickStart.Core

    /// sample function to load the initial data
    let loadInit() =
        Users.getUsers None None


    type State =
        { users: (Users.UserEndpoint.Result * Bitmap) array }

    type Msg =
        | SetUsers of (Users.UserEndpoint.Result * Bitmap) array
        | LoadImages of Users.UserEndpoint.Result array

    /// you can dispatch commands in your init if you chose to use `Program.mkProgram`
    /// instead of `Program.mkSimple`
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
            /// if you need to do asynchronous requests
            /// F# and Elmish provide nice constructs like in this case async {} blocks 
            /// and Cmd.OfAsync module. Learn more at
            /// https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/asynchronous-workflows
            /// https://elmish.github.io/elmish/cmd.html
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
            /// You can use `.mkSimple` yo remove the need of passing a command
            /// if you choose to do so, you  need to remove command from the tuple on your init fn
            /// you can learn more at https://elmish.github.io/elmish/basics.html
            let startFn () =
                init
            Elmish.Program.mkSimple startFn update view
            |> Program.withHost this
#if DEBUG
            |> Program.withConsoleTrace
#endif
            |> Program.run
