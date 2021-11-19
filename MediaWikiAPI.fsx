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
      License: (string * string) }

let WikipediaSearchResponse (languageCode: string) (query: string) =
    MediaWikiResponse.Load($"https://{languageCode}.wikipedia.org/w/rest.php/v1/page/{query}")

let WikipediaSearchResponseContents languageCode query =
    let response =
        WikipediaSearchResponse languageCode query

    { Id = response.Id
      Title = response.Title
      LatestTimestamp = response.Latest.Timestamp
      License = (response.License.Title, response.License.Url) }

let EnglishWikipediaSearchResponse (query: string) = WikipediaSearchResponse "en" query

let spanishSearch = WikipediaSearchResponse "es" "Google"