#r "nuget: FSharp.Data"
open FSharp.Data

let [<Literal>] holidaysJson = "https://date.nager.at/api/v2/publicholidays/2021/US"
type PublicHolidays = JsonProvider<holidaysJson>

let GetPublicHolidays countryCode year =
    PublicHolidays.Load($"https://date.nager.at/api/v2/publicholidays/{year}/{countryCode}")
    
let GetHolidayList country year =
    GetPublicHolidays country year
    |> Array.map (fun x -> (x.LocalName, x.Date.ToShortDateString()))
    
let GetHolidaysInMonth country year month =
    GetPublicHolidays country year
    |> Array.filter (fun x -> x.Date.Month = month)
    |> Array.map (fun x -> x.LocalName)
    
let us2021Holidays = GetHolidayList "US" 2021
    
let gt2021Holidays = GetHolidayList "GT" 2021
    
let ca2021Holidays = GetHolidayList "CA" 2021

let us2021JanuaryHolidays = GetHolidaysInMonth "US" 2021 1
let us2021DecemberHolidays = GetHolidaysInMonth "US" 2021 12