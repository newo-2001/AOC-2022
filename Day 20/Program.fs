open System.IO

let numbers =
    File.ReadAllLines "encrypted.txt"
    |> Seq.map int64
    |> Seq.toList

let length = List.length numbers |> int64

let getItem index =
    List.skip (int (index % length))
    >> List.head

let equals = LanguagePrimitives.PhysicalEquality

let moveItem list (item: int64 ref) =
    let index = List.findIndex (fun x -> equals x item) list |> int64
    let newIndex =
        match (index + item.Value) % (length - 1L) with
        | x when x < 0L -> x + length - 1L
        | x -> x
    
    list
    |> List.removeAt (int index)
    |> List.insertAt (int newIndex) item

let groveCoordinates (list: (int64 ref) list) =
    let zeroIndex = 
        list
        |> (List.findIndex(fun (x: int64 ref) -> x.Value = 0))
        |> int64

    [1000L; 2000L; 3000L]
    |> List.map (fun x -> (getItem (x + zeroIndex) list).Value)
    |> List.sum

let mix rounds list =
    let rec mix rounds order list =
        order
        |> List.fold moveItem list
        |> function
        | list when rounds = 1 -> list
        | list -> mix (rounds - 1) order list

    list
    |> List.map (fun x -> ref x)
    |> (fun list -> mix rounds list list)

numbers
|> mix 1
|> groveCoordinates
|> printf "The grove coordinates are %i\n"

numbers
|> List.map (fun x -> x * 811589153L)
|> mix 10
|> groveCoordinates
|> printf "The grove coordinates actually are %i\n"
