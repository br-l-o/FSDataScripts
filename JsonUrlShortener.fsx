#r "nuget: FSharp.Data"

open FSharp.Data
open System.Net.Http

[<Literal>]
let urlShortTest =
    "https://api.1pt.co/addURL?long=https://www.param.me"

type UrlShortener = JsonProvider<urlShortTest>

type Urls = { short: string; long: string }

// 1pt will generate a url if it is formatted correctly,
// but does not check if the URL is given resolves
let GetDesiredShortUrlResponse (short: string) (url: string) =
    let shortVal =
        if short |> System.String.IsNullOrEmpty then
            System.String.Empty
        else
            $"&short={short}"

    try
        // let's validate the url to see if it resolves
        let _ =
            async {
                use client = new HttpClient()
                let! response = client.GetAsync(url) |> Async.AwaitTask
                return response
            }
            |> Async.RunSynchronously

        Ok(UrlShortener.Load($"https://api.1pt.co/addURL?long={url}{shortVal}"))

    with
    | :? System.InvalidOperationException as ex -> Error ex.Message
    | _ -> Error $"Unable to shorten {url}. Please check the URL."

let GetShortenedUrl desired url =
    let desiredTest =
        (desired |> System.String.IsNullOrEmpty)
        || (desired |> String.length = 5)

    if not <| desiredTest then
        Error "Desired string must be 5 characters long."
    else
        GetDesiredShortUrlResponse desired url

let GetUrlsFromShortUrlResponse desired url =
    match GetShortenedUrl desired url with
    | Ok x ->
        Some
            { short = $"https://1pt.co/{x.Short}"
              long = x.Long }
    | _ -> None

let GetShortUrl url =
    match GetUrlsFromShortUrlResponse System.String.Empty url with
    | Some x -> x.short
    | None -> System.String.Empty

// 1pt will return a random short url if the desired one isn't available
let GetDesiredShortUrl desired url =
    match GetUrlsFromShortUrlResponse desired url with
    | Some x -> x.short
    | None -> System.String.Empty
