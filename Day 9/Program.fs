open System.IO

let offset = function
    | 'U' -> 0, 1
    | 'D' -> 0, -1
    | 'R' -> 1, 0
    | 'L' -> -1, 0
    | _ -> failwith "Invalid direction"

let add left right =
    fst left + fst right, snd left + snd right

let sub left right =
    fst left - fst right, snd left - snd right

let sign = function
    | n when n < 0 -> -1
    | n when n > 0 -> 1
    | _ -> 0

let move (rope: (int * int) list) direction: (int * int ) list =
    let newHead = add (List.head rope) (offset direction)

    let moveTail preceding knot =
        let head = List.head preceding
        let newTail = 
            match sub head knot with
            | (x, y) when abs x > 1 || abs y > 1 -> add knot (sign x, sign y)
            | _ -> knot

        newTail::preceding

    List.fold moveTail [ newHead ] (List.tail rope)
    |> List.rev

let trackTail (visited, rope) direction =
    let newRope = move rope direction
    ((newRope |> List.rev |> List.head)::visited, newRope)

let readMove: char list -> char seq = function
    | direction::(_::(remaining)) ->
        let amount = (remaining |> List.map string |> List.reduce(+) |> int) - 1
        {0..amount} |> Seq.map (fun _ -> direction)
    | _ -> failwith "Invalid move"

let moves =
    File.ReadLines "movements.txt"
    |> Seq.map (Seq.toList >> readMove)
    |> Seq.collect id
    
let countTailPositions initialRope =
    Seq.fold trackTail ([0, 0], initialRope)
    >> fst
    >> List.distinctBy id
    >> List.length

// Part 1
moves
|> countTailPositions [0, 0; 0, 0]
|> printf "The tail of the 2 long rope visited %i unique locations\n"

// Part 2
moves
|> countTailPositions ({0..9} |> Seq.toList |> List.map (fun _ -> 0, 0))
|> printf "The tail of the 10 long rope visited %i unique locations\n"
