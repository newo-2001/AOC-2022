namespace Day_7;
public class Directory : IFile
{
    private readonly List<IFile> _files = new List<IFile>();

    public IFile? Parent { get; set; }
    public IEnumerable<IFile> Files => _files;
    public required string Name { get; init; }
    public long Size => Files.Select(x => x.Size).Sum();
    public string Path => Parent?.Path ?? "" + ((Parent?.Path.EndsWith('/') ?? true) ? "" : "/") + Name;

    public void AddFile(IFile file)
    {
        file.Parent = this;
        _files.Add(file);
    }
}
