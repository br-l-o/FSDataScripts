#r "nuget: FSharp.Data"
open FSharp.Data

fsi.ShowDeclarationValues <- false

let [<Literal>] companiesTemplate = "https://fakerapi.it/api/v1/companies?_quantity=1"
type Companies = JsonProvider<companiesTemplate>

let GetCompanies (quantity: int) = 
    try
        Ok (Companies.Load($"https://fakerapi.it/api/v1/companies?_quantity={quantity}"))
    with
        | _ -> Error "Error retrieving addresses. Try again later."

let testCompanies = GetCompanies(10)

let companyNames = 
    match testCompanies with
    | Error x -> Error "No companies to process."
    | Ok y ->
        Ok (
            y.Data |> Array.map (fun x -> x.Name)
        )