using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day_12;
public class Route
{
    public required Map Map { get; init; }
    public required Tile Start { get; init; }
    public required Tile End { get; set; }

    public static Route FromFile(string path)
    {
        var lines = File.ReadLines(path);
        var map = new Map(lines.Select(line => line.Select(c => new Tile(ParseElevation(c))).ToArray()).ToArray());

        var (width, height) = (lines.Count(), lines.First().Length);
    
        Tile? tileAt(char c)
        {
            var coords = lines?.Select((s, i) => (s.IndexOf(c), i)).First(coord => coord.Item1 >= 0);
            return map.TileAt((int) coords?.Item1, (int) coords?.i);
        }

        var (start, end) = (tileAt('S'), tileAt('E'));
        if (start == null || end == null)
            throw new ArgumentException("Route did not contain a start or end location");

        return new Route()
        {
            Map = map,
            Start = start,
            End = end
        };
    }

    private static int ParseElevation(char elevation) => elevation switch
    {
        'S' => ParseElevation('a'),
        'E' => ParseElevation('z'),
        _ => elevation - 'a'
    };
}
