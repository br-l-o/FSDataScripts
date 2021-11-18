#r "nuget: FSharp.Data"
#r "nuget: XPlot.Plotly"

open FSharp.Data
open XPlot.Plotly

fsi.ShowDeclarationValues <- false

[<Literal>]
let nobelPrizesJson =
    "http://api.nobelprize.org/v1/prize.json"

[<Literal>]
let nobelPrizesLaureatesJson =
    "http://api.nobelprize.org/v1/laureate.json"

type NobelPrizes = JsonProvider<nobelPrizesJson>
type NobelLaureates = JsonProvider<nobelPrizesLaureatesJson>

let nprizes = NobelPrizes.Load(nobelPrizesJson)

let nlaureates =
    NobelLaureates.Load(nobelPrizesLaureatesJson)

let CountsByAward =
    nprizes.Prizes
    |> Array.groupBy (fun x -> x.Category)
    |> Array.map (fun x -> (fst x, snd x |> Array.length))

let GetLaureates =
    nlaureates.Laureates
    |> Array.map
        (fun x ->
            match x.Surname with
            | None -> $"{x.Firstname}"
            | Some y -> $"{y}, {x.Firstname}")
    |> Set.ofArray

let GroupedCountryLaureates =
    nlaureates.Laureates
    |> Array.map (fun x -> x.BornCountryCode.String)
    |> Seq.countBy id
    |> Seq.filter (fun (x, _) -> x <> None)
    |> Seq.map (fun (x, y) -> (x.Value, y))
    |> Seq.sortByDescending snd

// not working with Linux for some reason...
let GenerateLaureatesChart count layout =
    GroupedCountryLaureates
    |> Seq.truncate count
    |> Chart.Bar
    |> Chart.WithLayout layout
    |> Chart.WithHeight 500
    |> Chart.WithWidth 700
    |> Chart.Show

let GenerateLaureatesHtml count layout =
    let chart =
        GroupedCountryLaureates
        |> Seq.truncate count
        |> Chart.Bar
        |> Chart.WithLayout layout
        |> Chart.WithHeight 500
        |> Chart.WithWidth 700

    chart.GetHtml()

let pdf =
    Layout(title = $"Countries with most Nobel Laureates")
    |> GenerateLaureatesHtml 10
