#r "nuget: XPlot.Plotly"
open System
open System.IO
open System.Linq
open System.Text.RegularExpressions
open XPlot.Plotly

let countWords (contents: string) =
    let regex = Regex("[^a-zA-Z0-9 -]")
    regex.Replace(contents, "").Split()
    |> Array.countBy id
    
let getMostCommonWords limit (contents: string) =
    contents
    |> countWords
    |> Array.sortByDescending (snd)
    |> Seq.truncate limit
     
// test!
let iliad = __SOURCE_DIRECTORY__ + @"/data/iliad.txt" |> File.ReadAllText
let iliadTop10Words = iliad |> getMostCommonWords 10

let titleLayout = Layout(title = $"most common words in the Iliad")

iliadTop10Words
|> Chart.Bar
|> Chart.WithLayout titleLayout
|> Chart.WithHeight 500
|> Chart.WithWidth 700
|> Chart.Show