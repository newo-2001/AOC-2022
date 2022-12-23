open System.IO

type PropositionFunction = (int * int) Set -> (int * int) -> (int * int) option
type State = {
    elves: (int * int) Set
    propositions: PropositionFunction list
}

let elves =
    File.ReadAllLines "elves.txt"
    |> Seq.mapi (fun y row ->
        row |> Seq.mapi (fun x c -> if c = '#' then Some(x, y) else None))
    |> Seq.collect id
    |> Seq.choose id
    |> Set

let translate (x1, y1) (x2, y2) = (x1 + x2, y1 + y2)

let rectangle elves =
    let range mapper =
        Seq.map mapper
        >> fun x -> { Seq.min x .. Seq.max x}

    range fst elves, range snd elves

let north, east, south, west = (0, -1), (1, 0), (0, 1), (-1, 0)
let northeast, southeast, southwest, northwest = (1, -1), (1, 1), (-1, 1), (-1, -1)
let directions = [north; east; south; west; northeast; southeast; southwest; northwest]

let rec move state maxRounds =
    let proposeMove elf =
        let hasNoNeighbours =
            List.map (translate elf) directions
            |> List.filter (fun pos -> Set.contains pos state.elves)
            |> List.isEmpty
        
        if hasNoNeighbours then elf, elf else
        state.propositions
        |> List.choose (fun proposition -> proposition state.elves elf)
        |> List.tryHead
        |> Option.bind(fun x -> Some(elf, x))
        |> Option.defaultValue (elf, elf)
    
    if maxRounds = 0 then state, 1 else
    
    let proposedMoves = Seq.map proposeMove state.elves
    let proposedCount = Seq.countBy snd proposedMoves |> dict

    if Seq.forall(fun (origin, target) -> origin = target) proposedMoves
    then state, 1 else

    let newElves =
        proposedMoves
        |> Seq.map (fun (origin, target) -> if proposedCount[target] = 1 then target else origin)
        |> Set

    let newPropositions =
        List.append state.propositions.Tail [ state.propositions.Head ]

    let newState =
        { state with propositions = newPropositions; elves = newElves }
    
    move newState (maxRounds - 1)
    |> fun (result, rounds) -> result, rounds + 1

let emptyTiles elves =
    let xRange, yRange = rectangle elves
    Seq.collect (fun x -> Seq.map (fun y -> (x, y)) yRange) xRange
    |> Seq.filter (fun pos -> not(Set.contains pos elves))
    |> Seq.length

let propose offsets elves pos =
    offsets
    |> Seq.map (translate pos)
    |> Seq.filter (fun pos -> Set.contains pos elves)
    |> function
    | targets when Seq.isEmpty targets -> Some(Seq.head offsets |> translate pos)
    | _ -> None

let propositions =
    [ propose [north; northeast; northwest];
      propose [south; southeast; southwest];
      propose [west; northwest; southwest];
      propose [east; northeast; southeast] ]

let initialState =
    { elves = elves
      propositions = propositions }

let finalState = fst (move initialState 10)

emptyTiles finalState.elves
|> printf "After 10 rounds there are %i empty tiles between the elves\n"

snd (move initialState 100000)
|> printf "The simulation is stagnant after %i rounds"
