module Part1

open System.IO

type Tree = {
    id: int
    height: int
}

let makeTree id height = {
    height = height - '0' |> int
    id = id
}

let forest =
    File.ReadLines "forest.txt" |>
    Seq.toList |>
    List.mapi (fun y s ->
        s.ToCharArray() |>
        Array.toList |>
        List.mapi (fun x height -> makeTree (x + 99 * y) height)
    )

let forestSize = List.length forest

let rec transpose = function
    | (_::_)::_ as M ->
        List.map List.head M :: transpose (List.map List.tail M)
    | _ -> []

let countUnique =
    List.distinctBy (fun tree -> tree.id) >>
    List.length

let visible =
    List.fold (fun (max, trees) tree ->
        if tree.height > max then (tree.height, tree :: trees)
        else (max, trees)
    ) (-1, List.empty<Tree>) >> snd

let trees = [forest;
    forest |> List.map List.rev;
    transpose forest;
    forest |> transpose |> List.map List.rev] |> List.collect id

let visibleTrees =
    trees |>
    List.map visible |>
    List.collect id

let uniqueVisibleTrees =
    visibleTrees |>
    List.distinctBy (fun tree -> tree.id) |>
    List.sortBy (fun tree -> tree.id)

let solvePart1 =
    printf $"{List.length uniqueVisibleTrees} trees can be seen from outside the forest\n"
