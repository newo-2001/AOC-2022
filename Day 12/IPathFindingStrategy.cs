namespace Day_12;
public interface IPathFindingStrategy
{
    int? FindShortestPathToTopFromStart(Route route);
    int? FindShortestPathToBottomFromEnd(Route route);
}
