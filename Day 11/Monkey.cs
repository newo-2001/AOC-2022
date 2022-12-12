using System.Numerics;

namespace Day_11;
public class Monkey
{
    public LinkedList<BigInteger> Items { get; set; } = new();
    public required NextFunction NextMonkey { private get; init; }
    public required Operation InspectOperation { private get; init; }
    public required int Divisor { get; init; }

    public void GiveItem(BigInteger item) => Items.AddLast(item);

    public LinkedList<BigInteger> CopyItems() =>
        Items.Aggregate(new LinkedList<BigInteger>(), (list, x) => 
        {
            list.AddLast(x);
            return list;
        });

    public (BigInteger, int) Throw(Operation worryScalar)
    {
        if (Items.First is null)
            throw new InvalidOperationException("Monkey does not have any items to throw");

        var item = Items.First.Value;
        Items.RemoveFirst();

        var newItem = worryScalar(InspectOperation(item));
        var next = NextMonkey(newItem);

        return (newItem, next);
    }

    public static Monkey Parse(string str)
    {
        static int readLastInt(string line) => int.Parse(line[line.LastIndexOf(' ')..]);

        StringReader reader = new StringReader(str);
        reader.ReadLine();

        var items = reader.ReadLine().Split(": ")[1].Split(", ")
            .Select(n => new BigInteger(int.Parse(n)));

        var operation = ParseOperation(reader.ReadLine().Split("= ")[1]);
        var divisor = int.Parse(reader.ReadLine().Split("by ")[1]);
        var (t, f) = (readLastInt(reader.ReadLine()), readLastInt(reader.ReadLine()));
        NextFunction next = (BigInteger n) => n % divisor == 0 ? t : f;

        return new Monkey()
        {
           Items = new LinkedList<BigInteger>(items),
           InspectOperation = operation,
           NextMonkey = next,
           Divisor = divisor
        };
    }

    private static Operation ParseOperation(string str)
    {
        Func<BigInteger, BigInteger, BigInteger> parseOperator(char c) => c switch
        {
            '+' => BigInteger.Add,
            '-' => BigInteger.Subtract,
            '*' => BigInteger.Multiply,
            '/' => BigInteger.Divide,
            _ => throw new ArgumentException($"Invalid operator {c}")
        };

        Operation parseOperand(string str) => str switch
        {
            "old" => (BigInteger n) => n,
            _ => _ => new BigInteger(int.Parse(str)),
        };

        var items = str.Split(' ');
        var left = parseOperand(items[0]);
        var right = parseOperand(items[2]);
        var op = parseOperator(items[1].First());

        return (BigInteger n) => op(left(n), right(n));
    }

    public delegate BigInteger Operation(BigInteger n);
    public delegate int NextFunction(BigInteger n);
}
