namespace Day_10;
public class Device
{
    private readonly CrtDisplay _display = new CrtDisplay();

    public CpuState State { get; init; } = new CpuState();
    public CrtDisplay Display => _display;

    public delegate void OnCycleEventHandler(CpuState state);
    public event OnCycleEventHandler OnCycle;

    public Device()
    {
        OnCycle += _display.Draw;
    }

    public void Execute(IInstruction instruction)
    {
        do
        {
            OnCycle?.Invoke(State);
            State.NextCycle();
        } while (!instruction.Execute(State));
    }
}
