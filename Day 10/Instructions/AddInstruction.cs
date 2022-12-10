using System.Diagnostics.CodeAnalysis;

namespace Day_10.Instructions;
public class AddInstruction : IInstruction
{
    private const int INSTRUCTION_DURATION = 2;

    public required int Value { get; init; }
    private int _progress = 0;

    [SetsRequiredMembers]
    public AddInstruction(int value)
    {
        Value = value;
    }

    public bool Execute(CpuState state)
    {
        if (++_progress != INSTRUCTION_DURATION) return false;

        state.X += Value;
        return true;
    }

    public static AddInstruction Parse(string argument)
    {
        return !int.TryParse(argument, out int v)
            ? throw new ArgumentException($"addx requires an int argument, got: {argument}")
            : new AddInstruction(v);
    }
}
