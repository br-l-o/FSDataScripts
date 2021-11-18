#r "nuget: FSharp.Data"
#r "nuget: XPlot.Plotly"

open FSharp.Data
open XPlot.Plotly

// pokedex example (First 151 only)
[<Literal>]
let pokedexJson =
    "https://raw.githubusercontent.com/Biuni/PokemonGO-Pokedex/master/pokedex.json"

type Pokedex = JsonProvider<pokedexJson>
let dex = Pokedex.Load(pokedexJson)

// a function to print out a pokedex entry
let PrintPokedexEntry (x: Pokedex.Pokemon) =
    printfn $"ID: {x.Id}"
    printfn $"Name: {x.Name}"
    printfn $"Height: {x.Height}"
    printfn $"Weight: {x.Weight}"
    
let GeneratePokedexEntry (x: Pokedex.Pokemon) =
    sprintf "ID: %d\nName: %s\nHeight: %s\nWeight: %s" x.Id x.Name x.Height x.Weight

let GetTypeCounts count =
    dex.Pokemon
    |> Array.map (fun x -> x.Type)
    |> Array.concat
    |> Seq.countBy id
    |> Seq.sortByDescending (snd)
    |> Seq.truncate count
    |> List.ofSeq

//// print only details for ghost-types
//printfn "Ghost-type Pokemon basic details: "
//
//for i in
//    dex.Pokemon
//    |> Array.filter (fun x -> x.Type |> Array.contains "Ghost") do
//    PrintPokedexEntry i
let ghosts =
    dex.Pokemon
    |> Array.filter (fun x -> x.Type |> Array.contains "Ghost")
    |> Array.map (fun x -> x |> GeneratePokedexEntry)
    
//// print only details for pokemon taller than 1.5 m
//printfn "Pokemon taller than (or equal to) 1.5 m: "
//
//for i in
//    dex.Pokemon
//    |> Array.filter (fun x -> ((x.Height.Split [| ' ' |]).[0] |> float) > 1.5) do
//    PrintPokedexEntry i

let tall =
    dex.Pokemon
    |> Array.filter (fun x -> ((x.Height.Split [| ' ' |]).[0] |> float) > 1.5)
    |> Array.map (fun x -> x |> GeneratePokedexEntry)

//// print only details for pokemon smaller than 1 m
//printfn "Pokemon taller than (or equal to) 1.5 m: "
//
//for i in
//    dex.Pokemon
//    |> Array.filter (fun x -> ((x.Height.Split [| ' ' |]).[0] |> float) < 1.0) do
//    PrintPokedexEntry i
let small =
    dex.Pokemon
    |> Array.filter (fun x -> ((x.Height.Split [| ' ' |]).[0] |> float) < 1.0)
    |> Array.map (fun x -> x |> GeneratePokedexEntry)

//// print eevee's evolutions (names that contain "eon")
//for i in
//    dex.Pokemon
//    |> Array.filter (fun x -> x.Name.Contains("eon") && x.Name <> "Charmeleon") do
//    PrintPokedexEntry i
let eeveelutions =
    dex.Pokemon
    |> Array.filter (fun x -> x.Name.Contains("eon") && x.Name <> "Charmeleon")
    |> Array.map (fun x -> x |> GeneratePokedexEntry)

//// print pokemon that are weak against electric and ghost types
//for i in
//    dex.Pokemon
//    |> Array.filter
//        (fun x ->
//            (x.Weaknesses |> Array.contains "Electric")
//            && (x.Weaknesses |> Array.contains "Ghost")) do
//    PrintPokedexEntry i
let electricGhostWeak =
    dex.Pokemon
    |> Array.filter
        (fun x ->
            (x.Weaknesses |> Array.contains "Electric")
            && (x.Weaknesses |> Array.contains "Ghost"))
    |> Array.map (fun x -> x |> GeneratePokedexEntry)

let layout = Layout(title = $"Top 10 Pokemon Types")
let typeCountsTest = GetTypeCounts 10

let getHtml (chart: PlotlyChart) = chart.GetHtml()

let GetHtmlChart title (sequence: seq<'a * 'b>) =
    let titleLayout = Layout(title = title)
    sequence
    |> Chart.Bar
    |> Chart.WithLayout titleLayout
    |> Chart.WithHeight 500
    |> Chart.WithWidth 700
    |> getHtml
    
let DisplayChart title (sequence: seq<'a * 'b>)  =
    let titleLayout = Layout(title = title)
    sequence
    |> Chart.Bar
    |> Chart.WithLayout titleLayout
    |> Chart.WithHeight 500
    |> Chart.WithWidth 700
    |> Chart.Show

let typesCountHtml = typeCountsTest |> GetHtmlChart "Top 10 Pokemon types"

let typeArrayCount =
    dex.Pokemon 
    |> Array.map (fun x -> x.Type) 
    |> Seq.countBy (fun x -> x |> Array.length)
    |> GetHtmlChart "Amount of types per Pokemon"