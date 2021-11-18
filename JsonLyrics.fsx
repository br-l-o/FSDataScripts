#r "nuget: FSharp.Data"
open FSharp.Data

[<Literal>]
let lyricBase =
    "https://api.lyrics.ovh/v1/Joy Division/Isolation"

type Lyric = JsonProvider<lyricBase>

let GetLyrics (artist: string) (songTitle: string) =
    let query =
        $"https://api.lyrics.ovh/v1/{artist}/{songTitle}"

    let lyric = Lyric.Load(query)
    lyric.Lyrics
