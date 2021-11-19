#r "nuget: FSharp.Data"
open FSharp.Data

fsi.ShowDeclarationValues <- false

let [<Literal>] creditCardsTemplate = "https://fakerapi.it/api/v1/credit_cards?_quantity=1"
type CreditCards = JsonProvider<creditCardsTemplate>

let GetCreditCards (quantity: int) = 
    try
        Ok (CreditCards.Load($"https://fakerapi.it/api/v1/credit_cards?_quantity={quantity}"))
    with
        | _ -> Error "Error retrieving addresses. Try again later."

let testCards = GetCreditCards(3)

let cards = 
    match testCards with
    | Error x -> Error "No cards to process."
    | Ok y ->
        Ok (
            y.Data |> Array.map (fun x -> (x.Owner, x.Type, x.Number, x.Expiration))
        )