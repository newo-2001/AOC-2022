using Day_11;
using static Day_11.Monkey;


MonkeyGroup monkeys = new MonkeyGroup(
    File.ReadAllText("monkeys.txt")
    .Replace("\r\n", "\n")
    .Split("\n\n")
    .Select(Parse)
    .ToList());

// Part 1
var monkeyBusiness20 = monkeys.MonkeyBusiness(20, x => x / 3);
Console.WriteLine($"The monkey business after 20 rounds is {monkeyBusiness20}");

// Part 2
monkeys.UseCommonDivisor = true;
var monkeyBusiness10_000 = monkeys.MonkeyBusiness(10_000, (x => x));
Console.WriteLine($"The monkey business after 10,000 rounds is {monkeyBusiness10_000}");