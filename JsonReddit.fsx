#r "nuget: FSharp.Data"
open FSharp.Data

[<Literal>]
let subredditFormat = "https://www.reddit.com/r/all.json"

type Subreddit = JsonProvider<subredditFormat>

let GetSubredditJson sub =
    Subreddit.Load($"https://www.reddit.com/r/{sub}.json")

let GetLatestPostTitles sub =
    let currentSub = GetSubredditJson sub

    currentSub.Data.Children
    |> Array.map (fun x -> x.Data.Title)

let GetYouTubeLinks sub =
    let currentSub = GetSubredditJson sub

    currentSub.Data.Children
    |> Array.filter (fun x -> x.Data.Url.Contains("youtube.com"))
    |> Array.map (fun x -> (x.Data.Title, x.Data.Url))
