#r "nuget: FSharp.Data"
open FSharp.Data

fsi.ShowDeclarationValues <- false

[<Literal>]
let nobelPrizesJson = "http://api.nobelprize.org/v1/prize.json"

[<Literal>]
let nobelPrizesLaureatesJson = "http://api.nobelprize.org/v1/laureate.json"

type NobelPrizes = JsonProvider<nobelPrizesJson>
type NobelLaureates = JsonProvider<nobelPrizesLaureatesJson>

let nprizes = NobelPrizes.Load(nobelPrizesJson)
let nlaureates = NobelLaureates.Load(nobelPrizesLaureatesJson)

let CountsByAward = nprizes.Prizes
                    |> Array.groupBy (fun x -> x.Category)
                    |> Array.map (fun x -> (fst x, snd x |> Array.length))

let GetLaureates =
    nlaureates.Laureates |> Array.map (fun x -> match x.Surname with
                                                   | None -> $"{x.Firstname}"
                                                   | Some y -> $"{y}, {x.Firstname}") |> Set.ofArray