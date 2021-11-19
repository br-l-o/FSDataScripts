open System

#r "nuget: FSharp.Data"
open FSharp.Data

[<Literal>]
let mediaWikiBase =
    "https://en.wikipedia.org/w/rest.php/v1/page/Earth"

type MediaWikiResponse = JsonProvider<mediaWikiBase>

type MediaWikiContents =
    { Id: int
      Title: string
      LatestTimestamp: DateTimeOffset
      License: string }

let WikipediaSearchResponse (languageCode: string) (query: string) =
    MediaWikiResponse.Load($"https://{languageCode}.wikipedia.org/w/rest.php/v1/page/{query}")

let EnglishWikipediaSearchResponse (query: string) = WikipediaSearchResponse "en" query

let englishSearchDetails =
    let englishSearch = EnglishWikipediaSearchResponse "Google"

    { Id = englishSearch.Id
      Title = englishSearch.Title
      LatestTimestamp = englishSearch.Latest.Timestamp
      License = englishSearch.License.Title }

let spanishSearch = WikipediaSearchResponse "es" "Google"
