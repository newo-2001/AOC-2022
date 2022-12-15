open System.IO
open System
open FParsec

let skipTill p = manyCharsTillApply anyChar p (fun _ x -> x)
let kv key value = key >>. (pchar '=' >>. value)
let pcoord = (kv (pchar 'x') pint32 .>> pstring ", ") .>>. (kv (pchar 'y') pint32)
let psensor = skipTill pcoord .>>. skipTill pcoord

let dist (x1, y1) (x2, y2) = abs(x1 - x2) + abs(y1 - y2)

let sensors =
    File.ReadAllText "sensors.txt"
    |> run (sepBy psensor skipNewline)
    |> function
    | Failure(err, _, _) -> raise (new Exception($"Parsing failed: {err}"))
    | Success(result, _, _) -> result

let projectRow row sensor =
    let ((x, y), _) = sensor
    let radius = sensor ||> dist
    let offset = abs(y - row)
    let length = radius - offset
    if length > 0 then Some (x - length, x + length) else None

let collapseRanges ranges =
    let rec collapse = function
    | [] -> []
    | [ head ] -> [ head ]
    | head::tail ->
        match (head, tail.Head) with
        | (_, finish), (start, _) when start > finish -> head::collapse tail
        | (_, finish), (_, finish2) when finish >= finish2 -> collapse (head::tail.Tail)
        | (start, _), (_, finish) -> collapse ((start, finish)::tail.Tail)

    ranges
    |> List.sortBy fst
    |> collapse


let coveredRanges row =
    sensors
    |> List.choose (projectRow row)
    |> collapseRanges

// Part 1
coveredRanges 2_000_000
|> List.sumBy (fun (start, finish) -> finish + 1 - start)
|> fun x -> x - 1
|> printf "There are %i squares for Y = 2000000 where there can't be any beacons\n"

// Part 2
let findColumn =
    coveredRanges
    >> List.head
    >> snd
    >> fun x -> x + 1

let x, y =
    {0..4_000_000}
    |> Seq.skipWhile (coveredRanges >> (fun x -> x.Length = 1))
    |> Seq.head
    |> (fun y -> findColumn y, y)

(uint64 x) * 4_000_000UL + (uint64 y)
|> printf "The distress beacon is located at (%i, %i) with tuning frequency %i\n" x y
