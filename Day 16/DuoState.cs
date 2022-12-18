namespace Day_16;
public record DuoState(Node PlayerLocation, Node ElephantLocation, int TimeRemaining, int PressureReleased, List<Node> OpenValves)
{
    public string StateName()
    {
        return (string.Compare(PlayerLocation.Name, ElephantLocation.Name) < 0
            ? PlayerLocation.Name : ElephantLocation.Name)
            + (string.Compare(PlayerLocation.Name, ElephantLocation.Name) > 0
            ? PlayerLocation.Name : ElephantLocation.Name);
    }

    public bool IsOpen(Node location) => OpenValves.Contains(location);
}