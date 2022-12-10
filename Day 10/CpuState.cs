namespace Day_10;
public class CpuState
{
    public int Cycle { get; private set; } = 1;
    public int X { get; set; } = 1;

    public void NextCycle()
    {
        Cycle++;
    }
}
