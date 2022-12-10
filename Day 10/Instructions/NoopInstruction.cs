namespace Day_10.Instructions;
public class NoopInstruction : IInstruction
{
    public bool Execute(CpuState state) => true;
}
