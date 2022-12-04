open System.IO
open System

type Group = Range * Range

let sepBy (sep: string) (mapper: string * string -> 'b) (str: string): 'b =
    match str.Split(sep) with
        | [| first; second |] -> mapper(first, second)
        | _ -> failwith("Parsing failed")

let parseRange = sepBy "-" (fun (x, y) -> Range(int x, int y))
let parseGroup = sepBy "," (fun (x, y) -> Group(parseRange x, parseRange y))

let overlapsCompletely (a: Range, b: Range) =
    (a.Start.Value >= b.Start.Value && a.End.Value <= b.End.Value) ||
        (b.Start.Value >= a.Start.Value && b.End.Value <= a.End.Value)

let overlapsPartly (a: Range, b: Range) =
    (a.End.Value >= b.Start.Value && a.Start.Value <= b.End.Value) ||
        (b.End.Value >= a.Start.Value && b.Start.Value <= a.End.Value)

let assignments =
    File.ReadLines "assignments.txt"
    |> Seq.map parseGroup

let countGroups filter =
    assignments
    |> Seq.filter filter
    |> Seq.length

// Part 1
let completelyOverlapping = countGroups overlapsCompletely
printf($"There are {completelyOverlapping} groups that completely overlap\n")

// Part 2
let partlyOverlapping = countGroups overlapsPartly
printf($"There are {partlyOverlapping} groups that partly overlap\n")
