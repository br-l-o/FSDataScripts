#r "nuget: FSharp.Data"
open FSharp.Data

[<Literal>]
let nationalizeTemplate = "https://api.nationalize.io/?name=John"

type NationalizedName = JsonProvider<nationalizeTemplate>
type CountryCodes = JsonProvider<"http://country.io/names.json">

let countryCodesDict =
    let codes =
        CountryCodes.Load("http://country.io/names.json")

    JsonValue
        .Parse(codes.JsonValue.ToString())
        .Properties()
    |> Array.map (fun (x, y) -> (x, y.ToString() |> String.filter (fun x -> x <> '\"')))
    |> Map.ofArray

let GetNameStatistics name =
    try
        Ok(NationalizedName.Load($"https://api.nationalize.io/?name={name}"))
    with
    | :? System.Net.WebException as ex -> Error ex.Message

type CountryProbability =
    { CountryCode: string
      Probability: decimal }

type NameStatistics =
    { Name: string
      PossibleCountries: CountryProbability [] }

let SerializeNameStats name =
    match GetNameStatistics name with
    | Error x -> None
    | Ok nameDetails ->
        Some
            { Name = nameDetails.Name
              PossibleCountries =
                  nameDetails.Country
                  |> Array.map
                      (fun x ->
                          { CountryCode = x.CountryId
                            Probability = x.Probability }) }

let MostLikelyCountryOfOrigin name =
    match SerializeNameStats name with
    | None -> ("Error Processing Name", "N/A", "NaN")
    | Some serializedNameDetails ->
        if serializedNameDetails.PossibleCountries
           |> Array.length > 0 then
            let topCountry =
                serializedNameDetails.PossibleCountries
                |> Array.maxBy (fun x -> x.Probability)

            let percentage =
                ((topCountry.Probability |> float) * 100.0) |> int

            let countryOrCode =
                match countryCodesDict
                      |> Map.tryFind topCountry.CountryCode
                    with
                | None -> topCountry.CountryCode
                | Some x -> x

            (name |> string, countryOrCode, percentage |> sprintf "%d%%")
        else
            (name |> string, "N/A", "0%") // handled name not found
