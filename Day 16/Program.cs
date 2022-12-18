using Day_16;

var graph = Node.ParseGraph("valves.txt");
var state = new State(graph, 30, 0);

int maxPressurePlayer(State initialState)
{
    var states = new Queue<State>();
    states.Enqueue(initialState);

    while (states.Any())
    {
        var state = states.Dequeue();
        if (state.TimeRemaining == 0) continue;

        var newStates = state.Location.Edges
            .Where(x => x.MaxPressure <= state.PressureReleased)
            .Select(state.MoveTo)
            .Where(x => x != null);

        foreach (var newState in newStates)
        {
            states.Enqueue(newState!);
        }

        if (state.TryOpenValve(out var openedState))
        {
            states.Enqueue(openedState);
        }
    }

    return graph.Iterate().Select(x => x.MaxPressure).Max();
}

var released = maxPressurePlayer(state);
Console.WriteLine($"The maximum pressure that could be released in 30 minutes is {released}");

int maxPressureElephant(DuoState initialState)
{
    var states = new Queue<DuoState>();

    var maxPressure = new Dictionary<string, int>();
    foreach (var x in graph.Iterate())
    {
        foreach (var y in graph.Iterate())
        {
            maxPressure[new DuoState(x, y, 0, 0, new List<Node>()).StateName()] = -1;
        }
    }

    states.Enqueue(initialState);
    while (states.Any())
    {
        var state = states.Dequeue();

        if (state.TimeRemaining == 0) continue;

        var playerMoves = state.PlayerLocation.Edges
            .Select(x => new StateDiff() { Location = x }).ToList();

        if (state.PlayerLocation.FlowRate > 0 && !state.IsOpen(state.PlayerLocation))
            playerMoves.Add(new StateDiff()
            {
                Location = state.PlayerLocation,
                PressureReleased = (state.TimeRemaining - 1) * state.PlayerLocation.FlowRate,
                OpenedValves = new List<Node>() { state.PlayerLocation }
            });

        var elephantMoves = state.ElephantLocation.Edges
            .Select(x => new StateDiff() { Location = x }).ToList();

        if (state.ElephantLocation.FlowRate > 0 && !state.IsOpen(state.ElephantLocation))
            elephantMoves.Add(new StateDiff()
            {
                Location = state.ElephantLocation,
                PressureReleased = (state.TimeRemaining - 1) * state.ElephantLocation.FlowRate,
                OpenedValves = new List<Node>() { state.ElephantLocation }
            });

        foreach (var playerMove in playerMoves)
        {
            foreach (var elephantMove in elephantMoves)
            {
                var newValves = new List<Node>(state.OpenValves);
                newValves.AddRange(playerMove.OpenedValves);
                newValves.AddRange(elephantMove.OpenedValves);

                var newState = new DuoState(
                    playerMove.Location,
                    elephantMove.Location,
                    state.TimeRemaining - 1,
                    state.PressureReleased + elephantMove.PressureReleased + playerMove.PressureReleased,
                    newValves);

                if (newState.PressureReleased >= maxPressure[newState.StateName()])
                {
                    if (elephantMove.PressureReleased > 0 && playerMove.PressureReleased > 0
                        && elephantMove.Location == playerMove.Location)
                        continue;
                    
                    maxPressure[newState.StateName()] = state.PressureReleased;
                    states.Enqueue(newState);
                }
            }
        }
    }

    return maxPressure.Values.Max() + 1;
}

var initialState = new DuoState(graph, graph, 26, 0, new List<Node>());
var maxElephant = maxPressureElephant(initialState);
Console.WriteLine($"The maximum pressure that could be released in 26 minutes with the elephant is {maxElephant}");
