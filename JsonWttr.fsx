#r "nuget: FSharp.Data"
open FSharp.Data

let [<Literal>] wttrTemplate = "https://wttr.in/Detroit?format=j1"

type Wttr = JsonProvider<wttrTemplate>

type CurrentWeatherDetails =
    {
        FeelsLikeF: int
        FeelsLikeC: int
        TempF: int
        TempC: int
    }
    
type FormatCurrentWeatherStruct =
    {
        City: string
        Latitude: decimal
        Longitude: decimal
        Condition: CurrentWeatherDetails
    }

let GetCityWeather city =
    Wttr.Load($"https://wttr.in/{city}?format=j1")    
    
let FormatCurrentCondition (weather: Wttr.CurrentCondition) =
    {
        FeelsLikeF = weather.FeelsLikeF
        FeelsLikeC = weather.FeelsLikeC
        TempF = weather.TempF
        TempC = weather.TempC
    }
    
let GetCurrentConditionWeather (city: string) =
    let cityWeather = GetCityWeather city
    {
        City = city
        Latitude = cityWeather.NearestArea.[0].Latitude
        Longitude = cityWeather.NearestArea.[0].Longitude
        Condition = cityWeather.CurrentCondition.[0] |> FormatCurrentCondition
    }
    
let raleighWeather = GetCurrentConditionWeather "Raleigh"
let atlantaWeather = GetCurrentConditionWeather "Atlanta"
let nycWeather = GetCurrentConditionWeather "NYC"
let torontoWeather = GetCurrentConditionWeather "Toronto"