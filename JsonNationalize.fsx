#r "nuget: FSharp.Data"
open FSharp.Data

let [<Literal>] nationalizeTemplate = "https://api.nationalize.io/?name=John"
type NationalizedName = JsonProvider<nationalizeTemplate>

let GetNameStatistics name =
    NationalizedName.Load($"https://api.nationalize.io/?name={name}")
    
type CountryProbability =
    {
        CountryCode: string
        Probability: decimal
    }
    
type NameStatistics =
    {
        Name: string
        PossibleCountries: CountryProbability []
    }
let SerializeNameStats name =
    let nameDetails = GetNameStatistics name
    {
        Name = nameDetails.Name
        PossibleCountries = nameDetails.Country |> Array.map (fun x ->
            {CountryCode = x.CountryId; Probability = x.Probability})
    }
    
let NameStatsMaxResult name =
    let serializedNameDetails = SerializeNameStats name
    if serializedNameDetails.PossibleCountries |> Array.length > 0
    then
        let topCountry = serializedNameDetails.PossibleCountries |> Array.maxBy (fun x -> x.Probability)
        let percentage = ((topCountry.Probability |> float) * 100.0) |> int
        (name, topCountry.CountryCode, percentage |> sprintf "%d%%")
    else
        (name, "N/A", "0%") // handled name not found