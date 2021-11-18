#r "nuget: XPlot.Plotly"
open System.IO
open System.Text.RegularExpressions
open XPlot.Plotly

fsi.ShowDeclarationValues <- false

let countWords (contents: string) =
    let regex = Regex("[^a-zA-Z0-9 -]")
    regex.Replace(contents, "").Split()
    |> Array.sortByDescending id
    |> Array.countBy id
    
let getMostCommonWords limit (contents: string) =
    contents
    |> countWords
    |> Array.sortByDescending (snd)
    |> Seq.truncate limit
    
let getMostUncommonWords limit (contents: string) =
    contents
    |> countWords
    |> Array.sortBy (snd)
    |> Seq.truncate limit
    
let groupCounts (contents: string) =
    contents
    |> countWords
    |> Array.groupBy (fun x -> snd x)
    |> Array.sortByDescending (fun x -> snd x |> Array.length)
    |> Array.map (fun x -> $"Words with {fst x} appearances", snd x |> Array.length)
    
// test!
let iliad = __SOURCE_DIRECTORY__ + @"/data/iliad.txt" |> File.ReadAllText
let iliadTop10Words = iliad |> getMostCommonWords 10

let titleLayout = Layout(title = $"Occurences")