using System.Diagnostics.CodeAnalysis;

namespace Day_12;
public class DepthFirstSearchPathFindingStrategy : IPathFindingStrategy
{
    private IEnumerable<Node> _nodes = new List<Node>();

    public class Node
    {
        public int? Distance { get; set; } = null;
        public Tile Tile { get; init; }
        public int Height => Tile.Height;
        public List<Node> Destinations { get; init; } = new List<Node>();

        [SetsRequiredMembers]
        public Node(Tile tile) => Tile = tile;

        public void Visit(int depth)
        {
            Distance = depth;
            foreach (var node in Destinations)
            {
                if (node.Distance is null || node.Distance > depth + 1)
                {
                    node.Visit(depth + 1);
                }
            }
        }
    }

    public int? FindShortestPathToTopFromStart(Route route)
    {
        var (start, end) = BuildGraph(route, (from, to) => to.Height <= from.Height + 1);
        start.Visit(0);
        return end.Distance;
    }

    public int? FindShortestPathToBottomFromEnd(Route route)
    {
        var (_, end) = BuildGraph(route, (from, to) => to.Height >= from.Height - 1);
        end.Visit(0);
        return _nodes.Where(x => x.Height == 0)
            .Select(x => x.Distance)
            .Min();
    }

    private (Node, Node) BuildGraph(Route route, Func<Node, Node, bool> traversable)
    {
        var map = route.Map;
        var nodes = map.Tiles.Select(x => x.Select(x => new Node(x)).ToArray()).ToArray();
        _nodes = nodes.SelectMany(x => x);

        var height = nodes.Length;
        
        for (int y = 0; y < height; y++)
        {
            var width = nodes[y].Length;
            for (int x = 0; x < width; x++)
            {
                var node = nodes[y][x];
                
                IEnumerable<Node> neighbours = new Node?[]
                {
                    y + 1 < height ? nodes[y + 1][x] : null,
                    y - 1 >= 0 ? nodes[y - 1][x] : null,
                    x + 1 < width ? nodes[y][x + 1] : null,
                    x - 1 >= 0 ? nodes[y][x - 1] : null
                }.Where(x => x != null && traversable(node, x)).Select(x => x!);

                foreach (var neighbour in neighbours)
                {
                    node.Destinations.Add(neighbour);
                }
            }
        }
        
        Node findNode(Tile tile) => nodes.SelectMany(x => x).First(x => x.Tile == tile);
        return (findNode(route.Start), findNode(route.End));
    }
}
