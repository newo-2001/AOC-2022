using Day_18;
using static Day_18.Vector3;

static Vector3 parseVoxel(string point)
{
    var coords = point.Split(',').Select(int.Parse).ToList();
    return new Vector3(coords[0], coords[1], coords[2]);
}
var voxels = File.ReadAllLines("lava.txt").Select(parseVoxel);
var world = new World(voxels);

var exposedSides = world.Lava.Select(x => world.ExposedSides(x).Count()).Sum();
Console.WriteLine($"Total exposed sides of lava droplet is {exposedSides}");

var min = new Vector3(
    voxels.MinBy(x => x.X).X,
    voxels.MinBy(x => x.Y).Y,
    voxels.MinBy(x => x.Z).Z) - 2;

var max = new Vector3(
    voxels.MaxBy(x => x.X).X,
    voxels.MaxBy(x => x.Y).Y,
    voxels.MaxBy(x => x.Z).Z) + 2;

bool InBounds(Vector3 vec) =>
    vec.X >= min.X && vec.Y >= min.Y && vec.Z > min.Z
        && vec.X <= max.X && vec.Y <= max.Y && vec.Z <= max.Z;

var todo = new Queue<Vector3>();
ISet<Vector3> seen = new SortedSet<Vector3>() { new Vector3(min.X, min.Y, min.Z) };

foreach (var item in seen)
{
    todo.Enqueue(item);
}

var seenFaces = 0;
while (todo.Any())
{
    var position = todo.Dequeue();
    seenFaces += world.OccludedSides(position).Count();

    var neighbours = World.NeighboursWhere(position, x => !world.IsLava(x) && InBounds(x));
    
    foreach (var neighbour in neighbours.Where(x => !seen.Contains(x)))
    {
        seen.Add(neighbour);
        todo.Enqueue(neighbour);
    }
}

Console.WriteLine($"A total of {seenFaces} lava droplet faces are visible from the outside");
