#r "nuget: FSharp.Data"
open FSharp.Data

[<Literal>]
let lyricBase =
    "https://api.lyrics.ovh/v1/Joy Division/Isolation"

type Lyric = JsonProvider<lyricBase>

type LyricDetails =
    { Artist: string
      SongTitle: string
      Lyrics: string }

let GetLyricDetails (artist: string) (songTitle: string) =
    try
        let query =
            $"https://api.lyrics.ovh/v1/{artist}/{songTitle}"

        let lyric = Lyric.Load(query)

        { Artist = artist
          SongTitle = songTitle
          Lyrics = lyric.Lyrics }
    with
        |_ -> {Artist = "N/A"; SongTitle = "N/A"; Lyrics = "N/A"}

let LongestSong (lyricDetails: LyricDetails list) =
    let largest = lyricDetails
                  |> List.maxBy (fun x -> x.Lyrics |> String.length)
    
    (largest.Artist, largest.SongTitle, largest.Lyrics |> String.length)
    
let lyricList =
    [ GetLyricDetails "Kate Bush" "Cloudbusting"
      GetLyricDetails "The Cure" "A Forest"
      GetLyricDetails "My Bloody Valentine" "Only Shallow"
      GetLyricDetails "JPEGMAFIA" "Jesus Forgive Me, I Am A Thot"
      GetLyricDetails "Wu-Tang Clan" "Method Man"
      GetLyricDetails "Joy Division" "Shadowplay"]
    
let longestSong = LongestSong lyricList