// Part 1
var computeCalories = (string rations) => rations
    .Split("\n")
    .Select(int.Parse)
    .Sum();

var calories = File.ReadAllText("rations.txt")
    .Replace("\r\n", "\n")
    .Split("\n\n")
    .Select(computeCalories)
    .Order();

Console.WriteLine($"The Elf carrying the most calories is carrying {calories.Last()} calories");

// Part 2
var total = calories.TakeLast(3).Sum();
Console.WriteLine($"The 3 Elfs carrying the most calories are carrying {total} total calories");