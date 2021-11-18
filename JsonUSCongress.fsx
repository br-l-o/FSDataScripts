#r "nuget: FSharp.Data"

open FSharp.Data
open System

[<Literal>]
let representativesJson =
    "https://www.govtrack.us/api/v2/role?current=true&role_type=representative&limit=438"

[<Literal>]
let senatorsJson =
    "https://www.govtrack.us/api/v2/role?current=true&role_type=senator"

type Representative = JsonProvider<representativesJson>
type Senator = JsonProvider<senatorsJson>

type CongressInfo =
    { Body: String
      Name: String
      State: String
      District: int option } // senators have no district


let representatives = Representative.Load(representativesJson)
let senators = Senator.Load(senatorsJson)

// collate all politicians from each state
let getRepresentativesByState state =
    representatives.Objects
    |> Array.sortBy (fun x -> x.District)
    |> Array.filter (fun x -> x.State = state)
    |> Array.map
        (fun x ->
            { Body = x.RoleType
              Name = $"{x.Person.Firstname} {x.Person.Lastname}"
              State = x.State
              District = Some x.District })

let getSenatorsByState state =
    senators.Objects
    |> Array.filter (fun x -> x.State = state)
    |> Array.map
        (fun x ->
            { Body = x.RoleType
              Name = $"{x.Person.Firstname} {x.Person.Lastname}"
              State = x.State
              District = None })

let printCongressMembersByState state =
    let members =
        (state |> getRepresentativesByState)
        |> Array.append (state |> getSenatorsByState)

    for i in members do
        printfn $"\nName: {i.Name}"
        printfn $"State: {i.State}"
        printfn $"Body: {i.Body}"

        match i.District with
        | Some x ->
            printfn
                "District: %s"
                (if x = 0 then
                     "At-large"
                 else
                     x |> string)
        | _ -> ()

let numberOfMembersByState state =
    (state |> getRepresentativesByState)
    |> Array.append (state |> getSenatorsByState)
    |> Array.length

printfn "Representatives from the 1st district (or at-large representative) of their respective state/territory: "

for i in
    representatives.Objects
    |> Array.filter (fun x -> x.District <= 1)
    |> Array.sortBy (fun x -> x.State) do
    let districtType =
        if i.District < 1 then
            "At-large"
        else
            "1st"

    printfn $"Rep. for {districtType} district of %s{i.State}: %s{i.Person.Firstname} %s{i.Person.Lastname}"

printfn "Representatives from North Carolina: "
printCongressMembersByState "NC"

// not sure where these end, or whether or not to include NJ congress members...
printfn "Representatives within the NY metro area: "

for i in
    "NY"
    |> getRepresentativesByState
    |> Array.filter (fun x -> x.District.Value < 18) do
    printfn $"Name: {i.Name}; District: {i.District.Value}"

printfn "Representatives "

printfn "Members of congress in AK: %d" ("AK" |> numberOfMembersByState)
printfn "Members of congress in CA: %d" ("CA" |> numberOfMembersByState)
