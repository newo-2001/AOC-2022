module Tree

type Operator = Plus | Minus | Times | Over

type BinaryOperator = {
    operator: Operator
    operands: ExpressionTree * ExpressionTree
}

and ExpressionTree =
    | Number of int64
    | Variable
    | OpBinary of BinaryOperator

    static member evaluate f x =
        let rec eval = function
        | Number(n) -> n
        | Variable -> x
        | OpBinary(operation) ->
            let left, right = operation.operands
            match operation.operator with
            | Plus -> eval left + eval right
            | Minus -> eval left - eval right
            | Times -> eval left * eval right
            | Over -> eval left / eval right
        eval f

    static member isConstant f =
        let rec isConstant = function
        | Variable -> false
        | Number(_) -> true
        | OpBinary(operation) ->
            let left, right = operation.operands
            isConstant left && isConstant right
        isConstant f

    static member solve =
        let rec solve f n =
            match f with
            | Number(_) -> failwith "How did we get here?"
            | Variable -> n
            | OpBinary(operation) ->
                let left, right = operation.operands
                if ExpressionTree.isConstant left then
                    let leftValue = ExpressionTree.evaluate left 0L
                    match operation.operator with
                    | Plus -> n - leftValue
                    | Minus -> leftValue - n
                    | Times -> n / leftValue
                    | Over -> leftValue / n
                    |> solve right
                else
                    let rightValue = ExpressionTree.evaluate right 0L
                    match operation.operator with
                    | Plus -> n - rightValue
                    | Minus -> n + rightValue
                    | Times -> n / rightValue
                    | Over -> n * rightValue
                    |> solve left
        
        function
        | OpBinary(operation) ->
            let left, right = operation.operands
            let dependant, independant =
                if ExpressionTree.isConstant left then right, left else left, right

            ExpressionTree.evaluate independant 0L
            |> solve dependant
        | _ -> failwith "Root node of expression is not an operator!"


