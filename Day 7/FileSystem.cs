namespace Day_7;
public class FileSystem
{
    public Directory Root { get; init; } = new Directory() { Name = "/" };
    public required long Size { get; init; }
    public Directory WorkingDirectory;

    public FileSystem()
    {
        WorkingDirectory = Root;
    }

    public IEnumerable<IFile> Files => Traverse(Root);

    private IEnumerable<IFile> Traverse(IFile file)
    {
        if (file is File f)
            return new IFile[] { f };
        else if (file is Directory dir)
            return dir.Files.SelectMany(Traverse).Append(dir);
        throw new InvalidOperationException("File was not a file or directory");
    }
    
    public void ExecuteCommand(PeekableStreamReader commandReader)
    {
        string line = commandReader.ReadLine() ?? throw new EndOfStreamException();
        Console.WriteLine(line);

        line = line.TrimStart(new char[] {'$', ' '});

        string cmd = string.Concat(line.TakeWhile(x => x != ' '));

        Action runner = cmd switch
        {
            "cd" => () => ChangeDirectory(line[(cmd.Length+1)..]),
            "ls" => () => IndexDirectory(commandReader),
            _ => throw new ArgumentException($"Invalid Command {cmd}")
        };
        
        runner();
    }

    private void ChangeDirectory(string? path)
    {
        if (path is null)
            throw new ArgumentNullException(nameof(path));
        else if (path == string.Empty) return;
        
        if (path == ".." || path.StartsWith("../"))
        {
            WorkingDirectory = (Directory) (WorkingDirectory?.Parent
                ?? throw new DirectoryNotFoundException("Already at root of filesystem"));
            
            if (path.Length > 2) ChangeDirectory(path[2..]);
            return;
        }
        else if (path.StartsWith('/'))
        {
            WorkingDirectory = Root;
            ChangeDirectory(path[1..]);
            return;
        }
        else if (path.Contains('/'))
        {
            var dir = string.Concat(path.TakeWhile(x => x != '/'));
            ChangeDirectory(dir);
            ChangeDirectory(path[dir.Length..]);
            return;
        }

        var file = WorkingDirectory.Files
            .FirstOrDefault(f => f.Name == path);

        if (file is null)
            throw new DirectoryNotFoundException($"Command cd failed, directory {WorkingDirectory.Path}/{path} does not exist");

        if (file is not Directory directory)
            throw new DirectoryNotFoundException("Command cd failed, argument has to be a directory");

        WorkingDirectory = directory;
    }

    private void IndexDirectory(PeekableStreamReader commandReader)
    {
        while (!commandReader.EndOfStream)
        {
            if (commandReader.PeekLine()!.StartsWith('$')) break;

            string item = commandReader.ReadLine() ?? throw new EndOfStreamException();
            string name = string.Concat(item.SkipWhile(x => x != ' ').Skip(1))
                ?? throw new ArgumentException("Command ls failed, encountered file or directory without a name");

            if (item.StartsWith("dir"))
            {
                WorkingDirectory.AddFile(new Directory() { Name = name });
                continue;
            }

            string sizeString = string.Concat(item.TakeWhile(x => x != ' '));
            if (!long.TryParse(sizeString, out long size))
                throw new ArgumentException($"Command ls failed, file size {sizeString} was not a valid number");

            WorkingDirectory.AddFile(new File() { Name = name, Size = size });
        }
    }

    private delegate void CommandRunner(StreamReader commandReader);
}
