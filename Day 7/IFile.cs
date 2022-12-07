namespace Day_7;
public interface IFile
{
    IFile? Parent { get; set; }
    string Name { get; }
    string Path { get; }
    long Size { get; }
}
