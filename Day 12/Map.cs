using System.Diagnostics.CodeAnalysis;

namespace Day_12;
public class Map
{
    public required Tile[][] Tiles { get; init; }

    [SetsRequiredMembers]
    public Map(Tile[][] tiles) => Tiles = tiles;

    public Tile TileAt(int x, int y) => Tiles[y][x];
}
