using static Day_18.Vector3;

namespace Day_18;
public class World
{
    private readonly ISet<Vector3> _lava = new SortedSet<Vector3>();

    public IEnumerable<Vector3> Lava => _lava;
    
    public World(IEnumerable<Vector3> lava)
    {
        _lava = new SortedSet<Vector3>(lava);    
    }

    public bool IsLava(Vector3 position) => _lava.Contains(position);
    public bool IsTouchingLava(Vector3 position) => OccludedSides(position).Any();
    
    public static IEnumerable<Vector3> NeighboursWhere(Vector3 position, Func<Vector3, bool> predicate) =>
        UnitVectors.Select(offset => position + offset).Where(predicate);

    public IEnumerable<Vector3> OccludedSides(Vector3 position) =>
        NeighboursWhere(position, IsLava);
    public IEnumerable<Vector3> ExposedSides(Vector3 position) =>
        NeighboursWhere(position, (pos => !IsLava(pos)));
}
