using Day_10;

var instructions = File.ReadAllLines("program.txt")
    .Select(IInstruction.Parse);

var device = new Device();
var observer = new SignalStrengthObserver();
device.OnCycle += observer.Observe;

foreach (var instruction in instructions)
{
    device.Execute(instruction);
}

Console.WriteLine($"Total measured signal strength was {observer.TotalSignalStrength}\n");
device.Display.Display();
