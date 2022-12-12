using System.Diagnostics.CodeAnalysis;

namespace Day_12;
public class Tile
{
    public required int Height { get; init; }

    [SetsRequiredMembers]
    public Tile(int height)
    {
        Height = height;
    }
}
