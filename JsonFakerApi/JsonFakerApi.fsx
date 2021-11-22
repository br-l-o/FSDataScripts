#r "nuget: FSharp.Data"
open FSharp.Data

let [<Literal>] addressTemplate = "https://fakerapi.it/api/v1/addresses?_quantity=1"
let [<Literal>] booksTemplate = "https://fakerapi.it/api/v1/books?_quantity=1"
let [<Literal>] companiesTemplate = "https://fakerapi.it/api/v1/companies?_quantity=1"
let [<Literal>] creditCardsTemplate = "https://fakerapi.it/api/v1/credit_cards?_quantity=1"
let [<Literal>] imagesTemplate = "https://fakerapi.it/api/v1/images?_quantity=1&_type=kittens&_height=300"
let [<Literal>] personsTemplate = "https://fakerapi.it/api/v1/persons?_quantity=1&_gender=male&_birthday_start=2005-01-01"
let [<Literal>] placesTemplate = "https://fakerapi.it/api/v1/places?_quantity=1"
let [<Literal>] productsTemplate = "https://fakerapi.it/api/v1/products?_quantity=1&_taxes=12&_categories_type=uuid"
let [<Literal>] textsTemplate = "https://fakerapi.it/api/v1/texts?_quantity=1&_characters=500"
let [<Literal>] usersTemplate = "https://fakerapi.it/api/v1/users?_quantity=1&_gender=male"

type Address = JsonProvider<addressTemplate>

let GetAddresses (quantity: int) = 
    try
        Ok (Address.Load($"https://fakerapi.it/api/v1/addresses?_quantity={quantity}"))
    with
        | _ -> Error "Error retrieving addresses. Try again later."

let testAddresses = GetAddresses(100)

let americanAddresses =
    match testAddresses with
    | Error x -> Error "No addresses to handle."
    | Ok x -> 
        Ok (
            // given fake data, either one would be considered a US address
            x.Data |> Array.filter (fun x -> x.Country.Contains("United States") || x.CountyCode = "US")
        )