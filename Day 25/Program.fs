open System.IO

let parseSNAFU (number: string) =
    let SNAFUDigit = function        
        | '-' -> -1
        | '=' -> -2
        | c -> int (c - '0')

    let rec SNAFU place = function
        | [] -> 0L
        | number ->
            let digit = List.head number
            let value = int64 (SNAFUDigit digit) * (pown 5L place)
            (SNAFU (place + 1) (List.tail number)) + value
        
    Seq.toList number
    |> List.rev
    |> SNAFU 0

let toSNAFU n =
    let SNAFUDigit = function
        | -1L -> '-'
        | -2L -> '='
        | c -> '0' + char c

    let rec toSNAFU n =
        if n = 0L then [] else
        let remainder = n % 5L
        let carry = if remainder > 2L then 5L - remainder else 0L
        SNAFUDigit(((n + 2L) % 5L) - 2L)::toSNAFU((n + carry) / 5L)

    toSNAFU n
    |> List.rev
    |> List.map string
    |> List.reduce (+)

File.ReadAllLines "snafu.txt"
|> Seq.map parseSNAFU
|> Seq.sum
|> toSNAFU
|> printf "The number to be entered on the console is %s"
