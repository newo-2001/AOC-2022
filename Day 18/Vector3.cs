namespace Day_18;
public record Vector3(int X, int Y, int Z) : IComparable<Vector3>
{
    public static readonly Vector3 Up = new Vector3(0, 1, 0);
    public static readonly Vector3 Down = new Vector3(0, -1, 0);
    public static readonly Vector3 South = new Vector3(0, 0, 1);
    public static readonly Vector3 North = new Vector3(0, 0, -1);
    public static readonly Vector3 East = new Vector3(1, 0, 0);
    public static readonly Vector3 West = new Vector3(-1, 0, 0);

    public static readonly IEnumerable<Vector3> UnitVectors =
        new Vector3[] { Up, Down, North, East, South, West };

    public int CompareTo(Vector3? other)
    {
        if (other == null && this == null) return 0;
        else if (other == null) return 1;
        
        return other switch
        {
            _ when X > other.X => -1,
            _ when X < other.X => 1,
            _ when Y > other.Y => -1,
            _ when Y < other.Y => 1,
            _ when Z > other.Z => -1,
            _ when Z < other.Z => 1,
            _ => 0
        };
    }

    public static Vector3 operator +(Vector3 left, Vector3 right) =>
        new Vector3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

    public static Vector3 operator -(Vector3 vec) => new Vector3(-vec.X, -vec.Y, -vec.Z);
    public static Vector3 operator -(Vector3 left, Vector3 right) => left + -right;
    public static Vector3 operator -(Vector3 vec, int scalar) => vec.Map(x => x - scalar);
    public static Vector3 operator -(int scalar, Vector3 vec) => vec - scalar;
    public static Vector3 operator +(Vector3 vec, int scalar) => vec.Map(x => x + scalar);
    public static Vector3 operator +(int scalar, Vector3 vec) => vec + scalar;
    public static Vector3 operator *(Vector3 vec, int scalar) => vec.Map(x => x * scalar);
    public static Vector3 operator *(int scalar, Vector3 vec) => vec * scalar;
    public Vector3 Map(Func<int, int> mapper) => new Vector3(mapper(X), mapper(Y), mapper(Z));

    public Vector3 Cross(Vector3 other) =>
        new Vector3(
            Y * other.Z - Z * other.Y,
            Z * other.X - X * other.Z,
            X * other.Y - Y * other.X);
}
