open System.IO

#r "nuget: FSharp.Data"
open FSharp.Data
open System

fsi.ShowDeclarationValues <- false

[<Literal>]
let usCovidCaseDataJson = "https://data.cdc.gov/api/views/9mfq-cb36/rows.csv"
type USCovidCaseData = CsvProvider<usCovidCaseDataJson>

let covidData = USCovidCaseData.Load(usCovidCaseDataJson)

type BasicCovidData =
    {
        SubmissionDate: DateTime
        State: string
        TotalCases: int
        TotalDeaths: int
    }

let basicCovidDataByDay startFromToday =
      let orderedCovidData =
          if startFromToday
          then covidData.Rows |> Seq.sortByDescending (fun x -> x.Submission_date)
          else covidData.Rows |> Seq.sortByDescending (fun x -> x.Submission_date)
          
      orderedCovidData
          |> Seq.map (fun x -> {
              SubmissionDate = x.Submission_date
              State = x.State
              TotalCases = x.Tot_cases
              TotalDeaths = x.Tot_death
          })
          
[<Literal>]
let usCovidCasePath = __SOURCE_DIRECTORY__ + @"/data/united_states_covid19_cases_deaths_and_testing_by_state.csv"
type SevenDayCaseRates = CsvProvider<usCovidCasePath, SkipRows=2>
let caseRates = SevenDayCaseRates.Load(usCovidCasePath)

type StateCaseRate =
    {
        Territory: string
        TotalCases: int
        SpreadLevel: string
    }

let StatesByCovidCases ascending =
    let sortedCaseRates =
        if ascending
        then caseRates.Rows |> Seq.sortBy (fun x -> x.``Total Cases``)
        else caseRates.Rows |> Seq.sortByDescending (fun x -> x.``Total Cases``)
        
    sortedCaseRates
        // ignore total US values and states/territories that don't report numbers
        |> Seq.filter (fun x -> x.``State/Territory`` <> "United States of America")
        |> Seq.filter (fun x -> not <| System.Double.IsNaN(x.``Total Cases``))
        |> Seq.map (fun x ->
                          {
                               Territory = x.``State/Territory``
                               TotalCases = x.``Total Cases`` |> int
                               SpreadLevel = x.``Level of Community Transmission``
                          })
        
let groupStatesByCommunityTransmissionLevel =
    caseRates.Rows
        |> Seq.groupBy (fun x -> x.``Level of Community Transmission``)
        |> Seq.map (fun (x,y) -> (x, y |> Seq.map (fun x -> x.``State/Territory``)))
        
[<Literal>]
let stateCovidVaxxCsv = __SOURCE_DIRECTORY__ + @"/data/COVID-19_Vaccinations_in_the_United_States_Jurisdiction.csv"
type StateVaccinationData = CsvProvider<stateCovidVaxxCsv>

let stateVaxxData = StateVaccinationData.Load(stateCovidVaxxCsv)

type VaccinationData =
    {
        Date: System.DateTime
        State: string
        TotalDoses: int
        TotalJanssen: int
        TotalModerna: int
        TotalPfizer: int
    }
    
let stateVaxxDataSerialized = stateVaxxData.Rows
                              // this column could come up null
                              |> Seq.filter (fun x -> not <| System.Double.IsNaN(x.Additional_Doses_Vax_Pct |> double))
                              // ignore total US and long-term care facility counts
                              |> Seq.filter (fun x -> x.Location <> "US" || x.Location <> "LTC")
                              |> Seq.map (fun x ->
                                            {
                                                Date = x.Date
                                                State = x.Location
                                                TotalDoses = x.Distributed
                                                TotalJanssen = x.Distributed_Janssen
                                                TotalModerna = x.Distributed_Moderna
                                                TotalPfizer = x.Distributed_Pfizer
                                            })
                              
let orderStatesByCurrentTotalDoses ascending =
    let maxDateTime = stateVaxxDataSerialized |> Seq.map (fun x -> x.Date) |> Seq.max
    let mostCurrentSerializedData = stateVaxxDataSerialized |> Seq.filter (fun x -> x.Date = maxDateTime)
    if ascending
        then mostCurrentSerializedData |> Seq.sortBy (fun x -> x.TotalDoses)
        else mostCurrentSerializedData |> Seq.sortByDescending (fun x -> x.TotalDoses)