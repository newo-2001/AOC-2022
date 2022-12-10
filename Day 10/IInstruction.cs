using Day_10.Instructions;

namespace Day_10;
public interface IInstruction
{
    bool Execute(CpuState state);

    public static IInstruction Parse(string instruction)
    {
        var arguments = instruction.Split(' ');

        return arguments[0] switch
        {
            "addx" => AddInstruction.Parse(arguments[1]),
            "noop" => new NoopInstruction(),
            _ => throw new ArgumentException($"Invalid instruction {arguments[0]}")
        };
    }
}
