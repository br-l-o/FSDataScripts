#r "nuget: FSharp.Data"
open FSharp.Data

fsi.ShowDeclarationValues <- false

let [<Literal>] booksTemplate = "https://fakerapi.it/api/v1/books?_quantity=1"

type Books = JsonProvider<booksTemplate>

let GetBooks (quantity: int) = 
    try
        Ok (Books.Load($"https://fakerapi.it/api/v1/books?_quantity={quantity}"))
    with
        | _ -> Error "Error retrieving addresses. Try again later."

let testAddresses = GetBooks(100)

let ISBNValues = 
    match testAddresses with
    | Error x -> Error "No books to process."
    | Ok x -> 
        Ok (
            x.Data |> Array.map (fun x -> x.Isbn)
        )