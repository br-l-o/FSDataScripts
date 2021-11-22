#r "nuget: FSharp.Data"
open FSharp.Data

[<Literal>]
let userProfilePath =
    "https://openwhyd.org/adrien?format=json"

[<Literal>]
let userPlaylistPath =
    "https://openwhyd.org/adrien/playlist/61?format=json"

[<Literal>]
let hotTracksPath =
    "https://openwhyd.org/hot/electro?format=json"

type UserProfile = JsonProvider<userProfilePath>
type UserPlaylist = JsonProvider<userPlaylistPath>
type HotTracks = JsonProvider<hotTracksPath>

let GetUserProfile (user: string) =
    UserProfile.Load($"https://openwhyd.org/{user}?format=json")

let GetUserPlaylist (user: string) (playListNum: int) =
    UserPlaylist.Load($"https://openwhyd.org/{user}/playlist/{playListNum}?format=json")

let GetHotTracks (genre: string) =
    HotTracks.Load($"https://openwhyd.org/hot/{genre}?format=json")

let userProfileTest = GetUserProfile "adrien"
let userPlaylistTest = GetUserPlaylist "adrien" 61
let hotTracksTest = GetHotTracks "electro"

let getUserPlaylistNames =
    userProfileTest |> Array.map (fun x -> x.Pl.Name)

let getPlaylistSongTitles =
    userPlaylistTest |> Array.map (fun x -> x.Name)

let getHotTrackDetails =
    hotTracksTest.Tracks
    |> Array.map
        (fun x ->
            (x.Name
             , match x.Pl with
               | None -> "N/A"
               | Some x -> x.Name.JsonValue.ToString()
             , x.Score))