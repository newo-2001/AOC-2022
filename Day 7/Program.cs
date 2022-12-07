using Day_7;

var fs = new FileSystem() { Size = 70000000 };
var commands = System.IO.File.OpenRead("commands.txt");

using (var streamReader = new StreamReader(commands))
{
    var commandReader = new PeekableStreamReader(streamReader);
    while (!commandReader.EndOfStream)
    {
        fs.ExecuteCommand(commandReader);
    }
}

// Part 1
var size = fs.Files
    .Where(file => file is Day_7.Directory && file.Size <= 100000)
    .Select(x => x.Size)
    .Sum();

Console.WriteLine($"Total sum of directories bigger than 100000 bytes is {size} bytes");

// Part 2
const long UPDATE_SIZE = 30000000;
var availableSpace = fs.Size - fs.Root.Size;
var requiredSpace = UPDATE_SIZE - availableSpace;

var directory = fs.Files
    .Where(file => file is Day_7.Directory && file.Size >= requiredSpace)
    .OrderBy(x => x.Size)
    .First();

Console.WriteLine($"The directory to delete is {directory.Size} bytes");
