#r "nuget: FSharp.Data"
open FSharp.Data

[<Literal>]
let addressTemplate =
    "https://fakerapi.it/api/v1/addresses?_quantity=1"

type Address = JsonProvider<addressTemplate>

let GetAddresses (quantity: int) =
    try
        Ok(Address.Load($"https://fakerapi.it/api/v1/addresses?_quantity={quantity}"))
    with
    | _ -> Error "Error retrieving addresses. Try again later."

let testAddresses = GetAddresses(100)

let americanAddresses =
    match testAddresses with
    | Error x -> Error "No addresses to handle."
    | Ok y ->
        Ok(
            // given fake data, either one would be considered a US address
            y.Data
            |> Array.filter
                (fun x ->
                    x.Country.Contains("United States")
                    || x.CountyCode = "US")
        )
