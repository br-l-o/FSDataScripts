#r "nuget: FSharp.Data"
#r "nuget: ExcelProvider"

open FSharp.Interop.Excel

// NOTE: I ended up inadventenly using the Excel provider when attempting an earlier version
// when I ended up working with a CSV file
// this still works, but it's much better to use CsvProvider in this case.

// the file in this repo is null; you'll have to fetch the file yourself
// https://data.ed.gov/dataset/college-scorecard-all-data-files-through-6-2020/resources?resource=823ac095-bdfc-41b0-b508-4e8fc3110082
[<Literal>]
let collegeScoreCardPath =
    __SOURCE_DIRECTORY__
    + @"/data/Most-Recent-Cohorts-All-Data-Elements.csv"

type CollegeScoreCard = ExcelFile<collegeScoreCardPath>
let cscFile = new CollegeScoreCard()

type UniversityStats =
    { Name: string
      City: string
      State: string
      Zip: string
      Coordinates: (string * string)
      AccreditingAgency: string
      Website: string }

let DeserializeRow (data: CollegeScoreCard.Row) =
    { Name = data.INSTNM
      City = data.CITY
      State = data.STABBR
      Zip = data.ZIP
      Coordinates = (data.LATITUDE, data.LONGITUDE)
      AccreditingAgency = data.ACCREDAGENCY
      Website = data.INSTURL }

let GetUniversityStats (data: CollegeScoreCard) = data.Data |> Seq.map DeserializeRow

let GetUniversityStatsByState state (data: CollegeScoreCard) =
    data.Data
    |> Seq.filter (fun x -> state = x.STABBR)
    |> Seq.map DeserializeRow

let totalEntries = cscFile.Data |> Seq.length

let row =
    cscFile.Data |> Seq.head |> DeserializeRow

let universitiesPerState =
    cscFile.Data
    |> Seq.map (fun x -> x.STABBR)
    |> Seq.countBy id
    |> Seq.sortByDescending (snd)

let universitiesPerAccredAgency =
    cscFile.Data
    |> Seq.map (fun x -> x.ACCREDAGENCY)
    |> Seq.countBy id
    |> Seq.sortByDescending (snd)

let ncUniversityStats =
    cscFile |> GetUniversityStatsByState "NC"
