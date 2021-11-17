#r "nuget: FSharp.Data"
open FSharp.Data

type Football = CsvProvider< @"data/FootballResults.csv">
let data = Football.GetSample().Rows |> Seq.toArray

printf "Football Games Results: "
for x in data do
    printfn $"{x.Date}: {x.``Away Team``} ({x.``Full Time Away Goals``}) vs {x.``Home Team``} ({x.``Full Time Home Goals``})"
