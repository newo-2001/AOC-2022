var priority = (char item) => item < 'a' ? item - 'A' + 27 : item - 'a' + 1;

var commonPriority = (string sack) =>
{
    var low = sack.Take(sack.Length / 2);
    var high = sack.Skip(sack.Length / 2);

    return low.Intersect(high)
        .Select(priority)
        .Single();
};

var badge = (IEnumerable<string> elfs) => elfs
    .Aggregate<IEnumerable<char>>((acc, x) => acc.Intersect(x))
    .Single();

var sacks = File.ReadAllLines("rucksack.txt");

// Part 1
var totalPriority = sacks
    .Select(commonPriority)
    .Sum();

Console.WriteLine($"The total priority the compartments have in common is {totalPriority}");

// Part 2
var totalBadgePriority = sacks
    .Chunk(3)
    .Select(x => priority(badge(x)))
    .Sum();

Console.WriteLine($"The total priority of the badges for all groups is {totalBadgePriority}");
