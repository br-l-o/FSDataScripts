#r "nuget: FSharp.Data"
open FSharp.Data

// json file paths
[<Literal>]
let usdJson = "https://api.exchangerate-api.com/v4/latest/USD"

// provided types
type USD = JsonProvider<usdJson>

// exchange rate example
let exchange = USD.Load(usdJson)
printfn "Exchange rates: "

// the exchange rates are in a Rates object; can't be iterated by default
let ratesArray = exchange.Rates.JsonValue.Properties()
                 |> Array.map (fun x -> (fst x, (snd x).AsFloat()))
for i in ratesArray do
    printfn $"USD 1$ = {fst i} {snd i}"
    
// print only exchange rates where the currency is more valuable than USD
// USD value > currency value
printfn "Currencies more valuable than USD: "
for i in ratesArray |> Array.filter (fun x -> snd x < 1.) do
    printfn $"USD 1$ = {fst i} {snd i}"