namespace Application.Core

open System.Net.Http
open System.IO


module Users =
    let private baseurl = "https://randomuser.me/api/"
    type UserEndpoint = FSharp.Data.JsonProvider<"https://randomuser.me/api/">

    let getUsers (page: int option) (limit: int option) =
        let page = defaultArg page 1
        let limit = defaultArg limit 10
        async {
            let url = sprintf "%s?page=%i&results=%i" baseurl page limit
            let! sample = UserEndpoint.AsyncLoad(url)
            return sample.Results
        }

    let getUser id =
        async {
            let url = sprintf "%s?id=%i" baseurl id
            return! UserEndpoint.AsyncLoad(url)
        }

    let getImageFromUrl user (url: string) =
        async {
            use http = new HttpClient()
            let! str = http.GetStreamAsync(url) |> Async.AwaitTask
            let name = Path.GetTempFileName()
            use file = File.Create(name)
            str.CopyTo(file)
            return user, name
        }

    let getFulNameStr (name: UserEndpoint.Name) =
        sprintf "%s %s" name.First name.Last
