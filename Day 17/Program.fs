open System.IO
open System.Collections.Generic
open System.Linq

type Tile = Air | Rock | Unknown

type Cave = {
    tiles: LinkedList<Tile array>
    offset: int64
    width: int
    highestRock: int64
}

let horizonalLine = [(0, 0L); (1, 0L); (2, 0L); (3, 0L)]
let plus = [(0, 1L); (1, 0L); (1, 1L); (2, 1L); (1, 2L)]
let corner = [(0, 0L); (1, 0L); (2, 0L); (2, 1L); (2, 2L)]
let verticalLine = [(0, 0L); (0, 1L); (0, 2L); (0, 3L)]
let cube = [(0, 0L); (1, 0L); (0, 1L); (1, 1L)]

let shapes = [ horizonalLine; plus; corner; verticalLine; cube ]

// Higher values will have less false positives
// but will be more computationally expensive
let matchHeight = 40

let maxHeight =
    shapes
    |> List.collect (List.map snd)
    |> List.max
    |> fun x -> x + 1L

let jets =
    File.ReadAllText "jets.txt"
    |> Seq.map (function
    | '>' -> (1, 0L)
    | '<' -> (-1, 0L)
    | c -> failwith $"Invalid direction {c}")

let produceJet iteration =
    jets
    |> Seq.skip (iteration % Seq.length jets)
    |> Seq.head

let produceShape (iteration: int64) =
    shapes
    |> List.skip ((iteration % (List.length shapes |> int64)) |> int)
    |> List.head

let translate (ax, ay) (bx, by) = (ax + bx, ay + by)

let tileAt cave = function
    | (_, y) when y > cave.highestRock -> Air
    | (_, y) when y < cave.offset -> Unknown
    | (x, y) ->
        cave.tiles
        |> (fun list -> list.Skip (int(y - cave.offset)))
        |> Enumerable.First
        |> Array.get <| x

let setTile cave tile (x, y) =
    let index = y - cave.offset
    if index >= cave.tiles.Count then
        for _ in {0..5} do
            (cave.tiles.AddLast [| for i in 0..6 do Air |])
            |> ignore

    Array.set (cave.tiles.Skip(int index).First()) x tile

let pruneList cave =
    let height = cave.tiles.Reverse().SkipWhile(fun x -> x.All(fun x -> x = Air)).Skip(matchHeight).TakeWhile(fun x -> not(x.All(fun x -> x = Rock))).Count();
    let spliceIndex = int (cave.highestRock - cave.offset) - matchHeight - height

    if spliceIndex > 0 then
        for i in { 0..spliceIndex } do
            cave.tiles.RemoveFirst()
        { cave with offset = cave.offset + int64 spliceIndex + 1L }
    else cave

let materialize cave shape origin =
    shape
    |> List.map (translate origin)
    |> List.map (setTile cave Rock)
    |> ignore
    
    let maxHeight =
        shape
        |> List.map ((translate origin) >> snd)
        |> List.max

    { cave with highestRock = max maxHeight cave.highestRock }

let canTranslate cave shape translation origin =
    let newOrigin = translate origin translation
    let width = shape |> List.map fst |> List.max
    match newOrigin with
    | (x, y) when x < 0 || y < cave.offset || x >= cave.width - width -> false
    | _ -> List.forall (translate newOrigin >> (tileAt cave) >> (fun x -> x = Air)) shape

let simulate cave iteration shape =
    let rec sim origin iteration =
        let directionVector = produceJet iteration
        let newOrigin =
            if canTranslate cave shape directionVector origin
            then translate origin directionVector else origin

        if not (canTranslate cave shape (0, -1) newOrigin)
            then materialize cave shape newOrigin, iteration + 1
            else sim (translate newOrigin (0, -1)) (iteration + 1)
    
    sim (2, cave.highestRock + 4L) iteration

let tileStr = function
    | Air -> "."
    | Rock -> "#"
    | Unknown -> "?"

let caveStr cave height =
    { cave.highestRock - height - 1L .. cave.highestRock }
    |> Seq.map (fun y ->
        {0..6}
        |> Seq.map (fun x -> (tileAt cave (x, y)) |> tileStr)
        |> Seq.reduce(+))
    |> Seq.rev
    |> String.concat "\n"

let iterations = 1000000000000L

let mutable cave =
    new LinkedList<array<Tile>>()
    |> (fun tiles -> { tiles = tiles; highestRock = -1; offset = 0; width = 7 })
cave.tiles.AddLast([| for i in 0 .. 6 -> Air |]) |> ignore

let knownStates = new Dictionary<string * int * int, int64 * int64>()
let mutable rockCount = 0L
let mutable jetIndex = 0
let mutable loopRange = (0L, 0L)

let exploitLoop loopStartHeight rocksBeforeLoop =
    let rocksInLoop = rockCount - rocksBeforeLoop
    let length = cave.highestRock - loopStartHeight
    let remainingLoops = (iterations - rockCount) / rocksInLoop
    let newTop = (remainingLoops + 1L) * length + loopStartHeight
    let newOffset = newTop - (cave.highestRock - cave.offset)
    rockCount <- rockCount + remainingLoops * rocksInLoop
    cave <- { cave with highestRock = newTop; offset = newOffset }

while rockCount < iterations do
    let result =
        produceShape rockCount
        |> (simulate cave jetIndex)
    
    cave <- pruneList (fst result)
    jetIndex <- snd result
    rockCount <- rockCount + 1L
    if rockCount = 2022L then
        cave.highestRock + 1L
        |> printf "The highest rock in the cave after dropping 2022 rocks is at an altitude of %i\n"

    let loop = ref (0L, 0L)
    let state = (caveStr cave matchHeight, (rockCount % (List.length shapes |> int64)) |> int, jetIndex % Seq.length jets)
    if knownStates.TryGetValue(state, loop) && rockCount < 100000L then
        let (height, rocksBeforeLoop) = loop.Value
        match loopRange with
        | (start, finish) when start = height -> exploitLoop start rocksBeforeLoop
        | (0L, _) -> loopRange <- (fst loop.Value, 0)
        | (start, _) -> loopRange <- (start, fst loop.Value)
    else if rockCount < 100000L then
        knownStates.Add(state, (cave.highestRock, rockCount))

printf $"The highest rock in the cave after dropping {iterations} rocks is at an altitude of {cave.highestRock + 1L}\n"
