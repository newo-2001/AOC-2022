open System.IO
open FParsec
open System

type Packet =
    | Scalar of int
    | Vector of Packet list

type Parser<'a> = Parser<'a, unit>

let packetData, packetDataImpl = createParserForwardedToRef()
let packet: Parser<Packet> = (pint32 |>> Scalar) <|> (packetData |>> Vector)
packetDataImpl.Value <- (pchar '[' >>. (sepBy packet (pchar ',')) .>> pchar ']')

let fullPacket = packet .>> (eof <|> skipNewline)
let packetPair = (fullPacket .>>. fullPacket) .>> (eof <|> skipNewline)

let rec compare = function
    | (Scalar left, Scalar right) -> right - left
    | (Scalar scalar, Vector vec) -> compare (Vector [ Scalar scalar ], Vector vec)
    | (Vector vec, Scalar scalar) -> compare (Vector vec, Vector [ Scalar scalar ])
    | (Vector [], Vector []) -> 0
    | (Vector [], Vector _) -> 1
    | (Vector _, Vector []) -> -1
    | (Vector (leftHead::leftTail), Vector (rightHead::rightTail)) ->
        match compare (leftHead, rightHead) with
        | 0 -> compare (Vector leftTail, Vector rightTail)
        | x -> x

let packets =
    match run (many packetPair) (File.ReadAllText "packets.txt") with
    | Success(result, _, _) -> result
    | Failure(err, _, _) -> raise (new Exception(err))


// Part 1
packets
|> List.indexed
|> List.choose (fun (i, x) -> if compare x > 0 then Some(i + 1) else None)
|> List.sum
|> printf "The sum of the indices of the ordered packets is %i\n"

// Part 2
let dividers = [Vector [Vector [Scalar 2]]; Vector [Vector [Scalar 6]]]
let sorted =
    packets
    |> List.fold (fun list (a, b) -> a::b::list) []
    |> List.append dividers
    |> List.sortWith (fun left right -> compare(right, left))

let find needle =
    sorted
    |> List.findIndex(fun x -> x = needle)
    |> fun x -> x + 1

(find dividers.Head) * (find dividers.Tail.Head)
|> printf "The decoder key is %i"