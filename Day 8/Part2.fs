module Part2

open System.IO

let forest =
    File.ReadLines "forest.txt"
    |> Seq.map (Seq.map (fun c -> c - '0' |> int))
    |> Seq.collect id

let forestSize =
    forest |> Seq.length |> double |> sqrt |> int

let distance trees = 
    let tree = Seq.head trees
    let neighbours = Seq.skip 1 trees
    
    match Seq.length neighbours with
    | 0 -> 0
    | _ -> neighbours
        |> Seq.takeWhile (fun height -> height < tree)
        |> Seq.length
        |> fun x -> x + if x = Seq.length neighbours then 0 else 1

let nth (n: int) =
    Seq.mapi (fun i e -> if i % n = 0 then Some e else None)
    >> Seq.choose id

let scenicScore tree =
    let x, y = tree % forestSize, tree / forestSize
    
    let row = Seq.chunkBySize forestSize >> Seq.skip y >> Seq.head >> Array.toSeq
    let column = Seq.skip x >> nth forestSize

    let right = row >> Seq.skip x
    let left = row >> Seq.take (x + 1) >> Seq.rev
    let below = column >> Seq.skip y
    let above = column >> Seq.take (y + 1) >> Seq.rev

    [| right; left; above; below |]
    |> Seq.map (fun x -> (x >> distance) forest)
    |> Seq.reduce (fun acc x -> acc * x)

let maxScenic =
    forest
    |> Seq.mapi (fun i _ -> scenicScore i)
    |> Seq.max

let solvePart2 =
    printf $"The highest scenic score in the forest is {maxScenic}\n"
