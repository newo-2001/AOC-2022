open System.IO
open FParsec

type Parser<'a> = Parser<'a, unit>
type Tile = Void | Solid | Air
type Direction = Left | Right
type CardinalDirection = North | East | South | West
type Action =
    | Turn of Direction
    | Move of int

type State = {
    position: int * int
    facing: CardinalDirection
    actions: Action list
    map: Tile array2d
}

let turn facing direction =
    match (facing, direction) with
    | (North, Right) -> East
    | (East, Right) -> South
    | (South, Right) -> West
    | (West, Right) -> North
    | (North, Left) -> West
    | (West, Left) -> South
    | (South, Left) -> East
    | (East, Left) -> North

let directionVector = function
| North -> (0, -1)
| East -> (1, 0)
| South -> (0, 1)
| West -> (-1, 0)

let pTile = anyOf ".# " >>= function
    | '.' -> preturn Air
    | '#' -> preturn Solid
    | ' ' -> preturn Void
    | c -> fail $"Invalid tile {c}"

let pRow = manyTill pTile skipNewline |>> List.toArray
let pMap = manyTill pRow skipNewline |>> List.toArray
let pDirection: Parser<Action> = anyOf "LR" |>> function
    | 'L' -> Turn(Left)
    | 'R' -> Turn(Right)
    | c -> failwith $"Invalid direction {c}"

let pMove: Parser<Action> = pDirection <|> (pint32 |>> Move)
let pMoves = many pMove

let buildMap tiles =
    (tiles |> Array.map Array.length |> Array.max,
    Array.length tiles)
    ||> Array2D.init <| fun x y ->
        if x >= tiles[y].Length then Void
        else tiles[y][x]

let password x y facing =
    let facingScore = function
        | East -> 0
        | South -> 1
        | West -> 2
        | North -> 3

    1000 * (y + 1) + 4 * (x + 1) + (facingScore facing)

let add (x1, y1) (x2, y2) = (x1 + x2, y1 + y2)

let tileAt map = function
| (x, y) when x < 0 || y < 0 -> Void
| (x, y) when x >= Array2D.length1 map || y >= Array2D.length2 map -> Void
| position ->
    position
    ||> Array2D.get map

let rec moveUntil tile state =
    let newPosition = add state.position (directionVector state.facing)
    tileAt state.map newPosition
    |> function
    | next when next = tile -> state
    | _ -> moveUntil tile { state with position = newPosition }

let rec move wrap state amount =
    let move = move wrap
    if amount = 0 then state else
    let newPosition = add state.position (directionVector state.facing)
    
    tileAt state.map newPosition
    |> function
        | Solid -> state
        | Air -> move { state with position = newPosition } (amount - 1)
        | Void -> move (wrap state) (amount - 1)

let reverseWrap state =
    let behind = turn (turn state.facing Left) Left
    let desiredState = moveUntil Void { state with facing = behind }
    match tileAt state.map desiredState.position with
    | Solid -> state
    | Air -> { state with position = desiredState.position }
    | _ -> failwith "How did we get here?"

let cubeWrap state =
    let faceSize = Array2D.length1 state.map / 3
    let pos = add state.position (directionVector state.facing)
    
    let x, y, facing =
        match (fst pos, snd pos, state.facing) with
        | (x, y, facing) when facing = East && x = faceSize ->
            y - 3 * faceSize + faceSize, 3 * faceSize - 1, North // 1 -> 3
        | (x, y, facing) when facing = South && y = 3 * faceSize ->
            faceSize - 1, 3 * faceSize + x - faceSize, West // 3 -> 1
        | (x, y, facing) when facing = North && y = faceSize * 2 - 1 ->
            faceSize, faceSize + x, East // 2 -> 4
        | (x, y, facing) when facing = West && x = faceSize - 1 && y >= faceSize ->
            y - faceSize, faceSize * 2, South // 4 -> 2
        | (x, y, facing) when facing = East && x = 2 * faceSize && y < 2 * faceSize ->
            2 * faceSize + y - faceSize, faceSize - 1, North // 4 -> 6
        | (x, y, facing) when facing = South && y = faceSize ->
            2 * faceSize - 1, x - 2 * faceSize + faceSize, West // 6 -> 4
        | (x, y, facing) when facing = East && x = 2 * faceSize && y >= 2 * faceSize ->
            3 * faceSize - 1, 3 * faceSize - 1 - y, West // 3 -> 6
        | (x, y, facing) when facing = East && x = 3 * faceSize ->
            2 * faceSize - 1, 3 * faceSize - 1 - y, West // 6 -> 3
        | (x, y, facing) when facing = West && x = -1 && y < 3 * faceSize ->
            faceSize, 3 * faceSize - 1 - y, East // 2 -> 5
        | (x, y, facing) when facing = West && x = faceSize - 1 && y < faceSize ->
            0, 3 * faceSize - 1 - y, East // 5 -> 2
        | (x, y, facing) when facing = West && x = -1 && y >= 3 * faceSize ->
            faceSize + y - 3 * faceSize, 0, South // 1 -> 5
        | (x, y, facing) when facing = North && y = -1 && x < 2 * faceSize ->
            0, faceSize * 3 + x - faceSize, East // 5 -> 1
        | (x, y, facing) when facing = South && y = faceSize * 4 ->
            2 * faceSize + x, 0, South // 1 -> 6
        | (x, y, facing) when facing = North && y = -1 && x >= 2 * faceSize ->
            x - 2 * faceSize, 4 * faceSize - 1, North // 6 -> 1
        | _ -> failwith "How did we get here?"
    
    match tileAt state.map (x, y) with
    | Solid -> state
    | Air -> { state with position = (x, y); facing = facing }
    | _ -> failwith "How did we get here?"


let rec performActions moveFunction state =
    let performActions = performActions moveFunction
    if List.isEmpty state.actions then state else

    let newState =
        match state.actions.Head with
        | Turn(direction) ->
            { state with facing = turn state.facing direction }
        | Move(amount) -> moveFunction state amount
    performActions { newState with actions = newState.actions.Tail } 

let map, actions =
    File.ReadAllText "map.txt"
    |> run (pMap .>>. pMoves)
    |> function
    | Failure(err, _, _) -> failwith err
    | Success(result, _, _) -> result
    ||> fun tiles moves -> buildMap tiles, moves

let startPosition =
    map[0..(Array2D.length1 map) - 1, 0]
    |> Array.findIndex (fun tile -> tile = Air), 0
let initialState = { position = startPosition; facing = East; actions = actions; map = map }

let finalState = performActions (move reverseWrap) initialState
finalState.position
||> password <| finalState.facing
|> printf "The password is %i\n"

let cubeState = performActions (move cubeWrap) initialState
cubeState.position
||> password <| cubeState.facing
|> printf "The password actually is %i\n"
