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

// this can fail, so throw a Result error to make life easier
let WikipediaSearchResponse (languageCode: string) (query: string) =
    try
        Ok(MediaWikiResponse.Load($"https://{languageCode}.wikipedia.org/w/rest.php/v1/page/{query.Replace(' ', '_')}"))
    with
    | :? System.InvalidOperationException as ex -> Error ex.Message

let WikipediaSearchResponseContents languageCode query =
    let response =
        WikipediaSearchResponse languageCode query

    match response with
    | Ok x ->
        Ok
            { Id = x.Id
              Title = x.Title
              LatestTimestamp = x.Latest.Timestamp
              License = (x.License.Title, x.License.Url) }
    | Error x -> Error x

let EnglishWikipediaSearchResponse (query: string) = WikipediaSearchResponse "en" query

let CountResults (results: seq<Result<MediaWikiContents, string>>) =
    results
    |> Seq.map
        (fun x ->
            match x with
            | Ok _ -> "SUCCESS"
            | Error _ -> "FAILURE")
    |> Seq.countBy id

let countrySeq =
    seq {
        "Liberia"
        "Lisbon"
        "Raleigh, North Carolina"
        "Atlanta"
        "Memphis, Tennessee"
        "dfasfsdfgs"
        "eeeeeeeeeeeeee"
    }

let deserializedResults =
    countrySeq
    |> Seq.map (WikipediaSearchResponseContents "en")
    |> Seq.map
        (fun x ->
            match x with
            | Ok x -> Some x
            | Error _ -> None)
    |> Seq.filter (fun x -> x |> Option.isSome)
    |> Seq.map (fun x -> x.Value)

let responseSuccessCount =
    countrySeq
    |> Seq.map (WikipediaSearchResponseContents "en")
    |> CountResults
