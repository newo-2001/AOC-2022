namespace Day_7;
public class PeekableStreamReader
{
    private readonly StreamReader _reader;
    private readonly Queue<string> _buffered = new Queue<string>();

    public bool EndOfStream => _buffered.Count == 0 && _reader.EndOfStream;

    public PeekableStreamReader(StreamReader reader)
    {
        _reader = reader;
    }

    public string? PeekLine()
    {
        var line = _reader.ReadLine();
        if (line is null) return null;

        _buffered.Enqueue(line);
        return line;
    }

    public string? ReadLine() => _buffered.Count > 0 ? _buffered.Dequeue() : _reader.ReadLine();
}
