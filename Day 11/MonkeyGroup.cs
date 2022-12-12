using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using static Day_11.Monkey;

namespace Day_11;
public class MonkeyGroup
{
    private readonly List<LinkedList<BigInteger>> _startingItems;

    public List<Monkey> Monkeys { get; } = new();
    public BigInteger CommonDivisor { get; }
    public bool UseCommonDivisor { get; set; } = false;


    [SetsRequiredMembers]
    public MonkeyGroup(IEnumerable<Monkey> monkeys)
    {
        Monkeys = monkeys.ToList();
        CommonDivisor = Monkeys.Aggregate(1L, (acc, x) => acc * x.Divisor);
        _startingItems = new List<LinkedList<BigInteger>>(Monkeys.Select(x => x.CopyItems()));
    }

    public long MonkeyBusiness(int rounds, Operation worryScalar) =>
        DoRounds(rounds, worryScalar)
        .OrderDescending()
        .Take(2)
        .Aggregate(1L, (acc, x) => acc * x);
    
    private IEnumerable<int> DoRound(Operation worryScalar)
    {
        List<int> throwCount = new List<int>();
        for (var i = 0; i < Monkeys.Count; i++)
        {
            var monkey = Monkeys[i];
            var thrown = monkey.Items.Count;
            while (monkey.Items.Count > 0)
            {
                var (item, next) = monkey.Throw(worryScalar);
                Monkeys[next].GiveItem(UseCommonDivisor ? item % CommonDivisor : item);
            }
            throwCount.Add(thrown);
        };
        return throwCount;
    }

    public IEnumerable<int> DoRounds(int rounds, Operation worryScalar)
    {
        for (var i = 0; i < Monkeys.Count; i++)
        {
            var monkey = Monkeys[i];
            monkey.Items = _startingItems[i].Aggregate(new LinkedList<BigInteger>(), (list, x) => 
            {
                list.AddLast(x);
                return list;
            });
        }

        int[] throwCounts = new int[Monkeys.Count];
        for (var i = 0; i < Monkeys.Count; i++)
        {
            throwCounts[i] = 0;
        }

        for (int i = 0; i < rounds; i++)
        {
            var newCounts = DoRound(worryScalar).ToArray();
            for (int j = 0; j < newCounts.Length; j++)
            {
                throwCounts[j] += newCounts[j];
            }
        }

        return throwCounts;
    }
}
