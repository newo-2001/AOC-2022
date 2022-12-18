using System.Diagnostics.CodeAnalysis;

namespace Day_16;

public class Node
{
    private readonly Valve _valve;

    public int FlowRate => _valve.FlowRate;
    public string Name => _valve.Name;
    public int MaxPressure { get; set; } = 0;
    public IEnumerable<Node> Edges { get; set; }

    
    [SetsRequiredMembers]
    public Node(Valve valve) => _valve = valve;

    public IEnumerable<Node> Iterate()
    {
        var seen = new List<Node>() { this };
        IterateSeen(seen);
        return seen;
    }

    private void IterateSeen(List<Node> seen)
    {
        foreach (var node in Edges)
        {
            if (!seen.Contains(node))
            {
                seen.Add(node);
                node.IterateSeen(seen);
            }
        }
    }
    
    private static DisconnectedNode ParseValve(string input)
    {
        var name = input.Split(' ')[1];
        var flowRate = int.Parse(input.Split('=')[1].Split(';')[0]);
        var connections = input.Split(" ").Skip(9).Select(x => x[..2]).ToList();

        var valve = new Valve() { Name = name, FlowRate = flowRate };
        return new() { Node = new Node(valve), Connections = connections };
    }

    public static Node ParseGraph(string filepath, string startNodeName = "AA")
    {
        var nodes = File.ReadAllLines(filepath)
            .Select(ParseValve);

        var graph = new Dictionary<string, DisconnectedNode>(
            nodes.Select(node => new KeyValuePair<string, DisconnectedNode>(node.Node.Name, node)));
        
        foreach (var node in graph.Values)
        {
            node.Node.Edges = node.Connections.Select(name => graph[name].Node);
        }

        return graph.Values.First(x => x.Node.Name == startNodeName).Node;
    }

    private class DisconnectedNode
    {
        public required Node Node { get; init; }
        public required IEnumerable<string> Connections { get; init; }
    }
}
