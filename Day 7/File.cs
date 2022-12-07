namespace Day_7;
public class File : IFile
{
    public IFile? Parent { get; set; }
    public required string Name { get; init; }
    public long Size { get; init; } = 0;
    public string Path => Parent?.Path ?? "" + ((Parent?.Path.EndsWith('/') ?? true) ? "" : "/") + Name;
}
