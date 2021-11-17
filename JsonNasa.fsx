#r "nuget: FSharp.Data"
open FSharp.Data

// we don't need all that info
fsi.ShowDeclarationValues <- false

[<Literal>]
let peopleInSpaceJson = "http://api.open-notify.org/astros.json"

[<Literal>]
let meteoriteLandingsJson = "https://data.nasa.gov/resource/y77d-th95.json"

type PeopleInSpace = JsonProvider<peopleInSpaceJson>
type MeteoriteLanding = JsonProvider<meteoriteLandingsJson>
let peopleSpaceValues = PeopleInSpace.Load(peopleInSpaceJson)
let meteoriteLandingValues = MeteoriteLanding.Load(meteoriteLandingsJson)

let groupedPeople = peopleSpaceValues.People |> Array.groupBy (fun x -> x.Craft)
let groupedPeopleCount = groupedPeople |> Array.map (fun x -> fst x, snd x |> Array.length)
let groupedPeopleList = groupedPeople |> Array.map (fun x -> fst x, snd x |> Array.map (fun y -> y.Name))

let GetYearlyMeteoriteLandings year =
    meteoriteLandingValues
        |> Array.filter (fun x -> x.Year.String <> None)
        |> Array.filter (fun x -> x.Year.String.Value.[0..3] |> int = year)
        
let GetYearlyMeteoriteLandingsCount year = year |> GetYearlyMeteoriteLandings |> Array.length