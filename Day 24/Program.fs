open System.IO
open System.Collections.Generic

type Direction = North | East | South | West
type Tile =
    | Blizzard of Direction
    | Wall

type TileMap = Map<int * int, Tile list>
type State = {
    position: int * int
    time: int
}

let directionVector = function
    | North -> (0, -1)
    | East -> (1, 0)
    | South -> (0, 1)
    | West -> (-1, 0)

let parseTile = function
    | '.' -> None
    | '#' -> Some Wall
    | '^' -> Some (Blizzard(North))
    | '>' -> Some (Blizzard(East))
    | 'v' -> Some (Blizzard(South))
    | '<' -> Some (Blizzard(West))
    | c -> failwith $"Expected tile, got: {c}"

let initialTiles =
    File.ReadAllLines "blizzards.txt"
    |> Seq.mapi (fun y line -> Seq.mapi(fun x c ->
        parseTile c |> Option.bind (fun tile -> Some ((x, y), tile))) line)
    |> Seq.collect (Seq.choose id)
    |> Map.ofSeq
    |> Map.map (fun key value -> [ value ])

let translate (x1, y1) (x2, y2) = x1 + x2, y1 + y2
let objectAt (map: TileMap) pos =
    match Map.tryFind pos map with
    | None -> None
    | Some(list) -> Some(list.Head)

let addObject (map: TileMap) (pos, objectType) =
    map.Change (pos, (fun list ->
        match list with
            | None -> Some [ objectType ]
            | Some(original) -> Some (objectType::original)))

let bounds =
    initialTiles.Keys |> Seq.map (fun pos -> fst pos) |> Seq.max,
    initialTiles.Keys |> Seq.map (fun pos -> snd pos) |> Seq.max

let moveObject (pos, objectType) =
    match objectType with
    | Wall -> pos
    | Blizzard(direction) ->
        translate pos (directionVector direction)
        |> function
        | (x, y) when x = 0 -> (fst bounds - 1, y)
        | (x, y) when x = fst bounds -> (1, y)
        | (x, y) when y = 0 -> (x, snd bounds - 1)
        | (x, y) when y = snd bounds -> (x, 1)
        | newPos -> newPos

let rec gcd m n = if n = 0 then abs m else gcd n (m % n)
let lcm m n = m * n / (gcd m n)
let commonMultiple = bounds |> (fun (x, y) -> x - 1, y - 1) ||> lcm

let stateHash state = state.position, state.time % commonMultiple

let rec simulateBlizzards (states: TileMap list) count =
    if count = 0 then states else
    let nextState =
        Map.toSeq states.Head
        |> Seq.collect (fun (pos, tiles) -> Seq.map (fun tile -> pos, tile) tiles)
        |> Seq.map (fun (pos, tile) -> (moveObject (pos, tile)), tile)
        |> Seq.fold addObject Map.empty

    simulateBlizzards (nextState::states) (count - 1)

let tileStates =
    simulateBlizzards [ initialTiles ] (commonMultiple - 1)
    |> List.rev

let move state target =
    let seen = new HashSet<(int * int) * int>();
    seen.Add(stateHash state) |> ignore

    let queue = new Queue<State>()
    queue.Enqueue(state)

    let rec move (queue: Queue<State>) =
        let state = queue.Dequeue()
        if state.position = target then state else

        let tiles = tileStates[state.time % commonMultiple]
        let newState = { state with time = state.time + 1; }
        let possibleStates = 
            [ North; East; South; West ]
            |> List.map (directionVector >> (translate state.position))
            |> List.filter(fun (_, y) -> y >= 0)
            |> List.map (fun pos -> pos, objectAt tiles pos)
            |> List.choose (function
            | (pos, None) -> Some(pos)
            | _ -> None)
            |> fun moves ->
                match objectAt tiles state.position with
                | Some(Blizzard(_)) -> moves
                | _ -> state.position::(List.rev moves) |> List.rev
            |> List.map (fun pos -> { newState with position = pos })
            |> List.filter(fun state -> not (seen.Contains (stateHash state)))
        
        for state in possibleStates do
            seen.Add(stateHash state) |> ignore
            queue.Enqueue(state)

        move queue
    move queue

let initialState = { position = (1, 0); time = 0 }
let destination = (fst bounds - 1, snd bounds)

let finalState = move initialState destination
printf "The fewest number of minutes required to reach the goal is %i\n" (finalState.time - 1)

move finalState (1, 0)
|> move <| destination
|> fun state -> state.time - 1
|> printf "The fewest number of minutes required to the goal, back to the start and back to the goal again is %i"
