#r "nuget: FSharp.Data"
open FSharp.Data

let [<Literal>] holidaysJson = "https://date.nager.at/api/v2/publicholidays/2021/US"
type PublicHolidays = JsonProvider<holidaysJson>

let GetPublicHolidays countryCode year =
    PublicHolidays.Load($"https://date.nager.at/api/v2/publicholidays/{year}/{countryCode}")
    
let us2021Holidays =
    GetPublicHolidays "US" 2021
    |> Array.map (fun x -> (x.LocalName, x.Date.ToShortDateString()))

