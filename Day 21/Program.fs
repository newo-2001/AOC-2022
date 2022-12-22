open System
open System.IO
open FParsec
open System.Collections.Generic
open Tree

type Operation = Add | Sub | Div | Mul
type ComputationMonkey = {
    operation: Operation
    operands: string * string
}

type Monkey =
    | Constant of int64
    | Computation of ComputationMonkey

let parseName = parray 4 anyChar |>> Array.map string |>> Array.reduce(+)
let parseScalarMonkey =
    tuple2 (parseName .>> pstring ": ") (pint64 |>> Constant)

let parseOperator = anyChar >>= function
    | '+' -> preturn Add
    | '-' -> preturn Sub
    | '*' -> preturn Mul
    | '/' -> preturn Div
    | c -> fail $"Invalid operator: {c}"

let parseComputationMonkey = 
    let space = pchar ' '
    pipe4 (parseName .>> pstring ": ") parseName
        (space >>. parseOperator .>> space) parseName
        (fun name left operator right ->
            name, Computation { operation = operator; operands = (left, right) })

let parseMonkey = attempt parseScalarMonkey <|> parseComputationMonkey

let rec buildTree variableSelector (nodes: IDictionary<string, Monkey>) =
    let buildTree = function
    | name when variableSelector name -> Variable
    | name -> buildTree variableSelector nodes nodes[name]

    function
    | Constant(n) -> Number(n)
    | Computation(computation) ->
        let makeOperator operator operands =
            OpBinary { operator = operator; operands = operands }

        let left = buildTree (fst computation.operands)
        let right = buildTree (snd computation.operands)

        match computation.operation with
        | Add -> makeOperator Plus (left, right)
        | Sub -> makeOperator Minus (left, right)
        | Mul -> makeOperator Times (left, right)
        | Div -> makeOperator Over (left, right)
        
let monkeys =
    File.ReadAllText "monkeys.txt"
    |> run (sepBy parseMonkey skipNewline)
    |> function
    | Failure(err, _, _) -> raise (new Exception $"Parsing failed: {err}")
    | Success(result, _, _) -> result
    |> List.toSeq
    |> dict

buildTree (fun _ -> false) monkeys monkeys["root"]
|> ExpressionTree.evaluate <| 0L
|> printf "The number the monkey will yell is %i\n"

let f = buildTree (fun name -> name = "humn") monkeys monkeys["root"]

ExpressionTree.solve f
|> printf "The number you need to yell is %i\n"