open System.IO
open FParsec
open System

type Tile = Air | Rock | Sand

let ppoint = (pint32 .>> pchar ',') .>>. pint32

let ppath =
    sepBy ppoint (pstring " -> ")
    |>> List.pairwise

let ppaths =
    sepBy ppath skipNewline
    |>> List.collect id

let rec placePath cave (start, finish) =
    start ||> (Array2D.set cave) <| Rock
    match (start, finish) with
    | (start, finish) when start = finish -> ()
    | ((startX, startY) as start, ((endX, endY) as finish)) ->
        let (dx, dy) = (sign (endX - startX), sign (endY - startY))
        placePath cave ((startX + dx, startY + dy), finish)

let rec simulate tileAt (x, y) =
    let sim = simulate tileAt
    match tileAt (x, y + 1) with
    | Air -> sim (x, y + 1)
    | _ -> match tileAt (x - 1, y + 1) with
            | Air -> sim (x - 1, y + 1)
            | _ -> match tileAt (x + 1, y + 1) with
                    | Air -> sim (x + 1, y + 1)
                    | _ -> (x, y)

let simulateVoid cave (x, y) =
    let tileAt (x, y) = Array2D.get cave x y 
    try
        simulate tileAt (x, y)
        ||> Array2D.set cave <| Sand
        true
    with
        | :? IndexOutOfRangeException -> false

let simulateFloor cave (x, y) =
    let tileAt (x, y) =
        if y = Array2D.length2 cave - 1 then Rock
        else Array2D.get cave x y

    simulate tileAt (x, y)
    ||> Array2D.set cave <| Sand

let paths =
    File.ReadAllText "rocks.txt"
    |> run ppaths
    |> function
        | Failure(err, _, _) -> raise (new Exception($"Parsing failed: {err}"))
        | Success(result, _, _) -> result

let dimensions =
    paths
    |> List.fold (fun (x, y) ((a, b), (c, d)) ->
        (List.max [x; a; c], List.max [y; b; d])) (0, 0)
    |> (fun (x, y) -> ((x + 1) * 2, y + 3))

let cave =
    dimensions
    ||> Array2D.create <| Air

paths
|> List.map (placePath cave)
|> ignore

let source = (500, 0)

// Part 1
let newCave = Array2D.copy cave

let mutable count = 0
while simulateVoid newCave source do
    count <- count + 1

count
|> printf "Sand dropped in the void after dropping %i grains of sand\n"

// Part 2
count <- 0
while source ||> Array2D.get cave = Air do
    simulateFloor cave source |> ignore
    count <- count + 1

count
|> printf "Sand reached the source after dropping %i grains of sand\n"
