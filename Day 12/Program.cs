using Day_12;

Route route = Route.FromFile("map.txt");

IPathFindingStrategy strategy = new DepthFirstSearchPathFindingStrategy();

// Part 1
var distanceToTop = strategy.FindShortestPathToTopFromStart(route);
Console.WriteLine($"The shortest distance from the start to the top is {distanceToTop} tiles");

// Part 2
var distanceToBottom = strategy.FindShortestPathToBottomFromEnd(route);
Console.WriteLine($"The shortest distance from the top to the bottom is {distanceToBottom} tiles");
