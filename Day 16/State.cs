namespace Day_16;
public record State(Node Location, int TimeRemaining, int PressureReleased)
{
    private List<Node> _openValves = new List<Node>();

    public State? MoveTo(Node node)
    {
        if (node.MaxPressure > PressureReleased)
            return null;
        
        Location.MaxPressure = PressureReleased;

        return this with
        {
            Location = node,
            TimeRemaining = TimeRemaining - 1
        };
    }

    public bool TryOpenValve(out State state)
    {
        var newPressure = PressureReleased + (TimeRemaining - 1) * Location.FlowRate;
        
        if (IsOpen(Location) || newPressure <= Location.MaxPressure)
        {
            state = null!;
            return false;
        }

        state = this with
        {
            TimeRemaining = TimeRemaining - 1,
            PressureReleased = newPressure,
            _openValves = new List<Node>(_openValves) { Location }
        };

        return true;
    }

    public bool IsOpen(Node node) => _openValves.Contains(node);
}
