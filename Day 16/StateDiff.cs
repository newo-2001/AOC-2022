namespace Day_16;
public class StateDiff
{
    public List<Node> OpenedValves { get; init; } = new List<Node>();
    public required Node Location { get; init; }
    public int PressureReleased { get; init; } = 0;
}
