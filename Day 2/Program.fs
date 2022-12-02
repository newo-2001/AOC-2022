open System.IO
open Microsoft.FSharp.Core.LanguagePrimitives

type Shape =
    | Rock = 0
    | Paper = 1
    | Scissors = 2

type Result =
    | Win = 6
    | Tie = 3
    | Loss = 0

type Game = Shape * Shape

let play(game: Game) =
    match game with
    | (Shape.Rock, Shape.Paper) -> Result.Win
    | (Shape.Rock, Shape.Scissors) -> Result.Loss
    | (Shape.Paper, Shape.Rock) -> Result.Loss
    | (Shape.Paper, Shape.Scissors) -> Result.Win
    | (Shape.Scissors, Shape.Rock) -> Result.Win
    | (Shape.Scissors, Shape.Paper) -> Result.Loss
    | _ -> Result.Tie

let playForResult(game: Shape * Result) =
    let (them, _) = game
    let you =
        match game with
        | (Shape.Rock, Result.Win) -> Shape.Paper
        | (Shape.Rock, Result.Loss) -> Shape.Scissors
        | (Shape.Paper, Result.Win) -> Shape.Scissors
        | (Shape.Paper, Result.Loss) -> Shape.Rock
        | (Shape.Scissors, Result.Win) -> Shape.Rock
        | (Shape.Scissors, Result.Loss) -> Shape.Paper
        | _ -> them
    (them, you)

let scoreForGame (playstyle: ('a * 'b) -> Game) (game: 'a * 'b): int =
    let (_, you) = playstyle game
    (playstyle >> play >> EnumToValue) game + (EnumToValue you) + 1

let parseShape (shape: char) = enum("ABCXYZ".IndexOf(shape) % 3)
let parseResult (result: char): Result = enum("XYZ".IndexOf(result) * 3)
let parseGame (game: string) = Game(parseShape game[0], parseShape game[2])
let parseDesiredGame (game: string): Shape * Result =
    parseShape game[0], parseResult game[2]

let guide = File.ReadLines "strategyGuide.txt"
let score (strat: string -> int) = guide |> Seq.map strat |> Seq.sum

// Part 1
let guideScore = score (parseGame >> scoreForGame id)
printf($"The strategy guide would've scored you {guideScore} points\n")

// Part 2
let desiredScore = score (parseDesiredGame >> scoreForGame playForResult)
printf($"Always playing for the desired outcome would've scored you {desiredScore} points\n")
