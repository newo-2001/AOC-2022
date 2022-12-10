namespace Day_10;
public class SignalStrengthObserver
{
    private const int INTERESTING_CYCLE_INTERVAL = 40;
    private const int INTERESTING_CYCLE_OFFSET = 20;

    private readonly List<int> _signalStrengths = new();

    public void Observe(CpuState cpuState)
    {
        if ((cpuState.Cycle - INTERESTING_CYCLE_OFFSET) % INTERESTING_CYCLE_INTERVAL != 0) return;
        
        int signalStrength = cpuState.Cycle * cpuState.X;
        _signalStrengths.Add(signalStrength);
    }

    public int TotalSignalStrength => _signalStrengths.Sum();
}
