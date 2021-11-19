#r "nuget: FSharp.Data"
#r "nuget: ExcelProvider"
open FSharp.Data
open FSharp.Interop.Excel

// the file in this repo is null; you'll have to fetch the file yourself
// https://data.ed.gov/dataset/college-scorecard-all-data-files-through-6-2020/resources?resource=823ac095-bdfc-41b0-b508-4e8fc3110082
[<Literal>]
let collegeScoreCardPath = __SOURCE_DIRECTORY__ + @"/data/Most-Recent-Cohorts-All-Data-Elements.csv"

type CollegeScoreCard = ExcelFile<collegeScoreCardPath>
let cscFile = new CollegeScoreCard()