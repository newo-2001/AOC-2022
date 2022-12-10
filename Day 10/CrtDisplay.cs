namespace Day_10;
public class CrtDisplay
{
    private const int DISPLAY_WIDTH = 40;
    private const int SPRITE_WIDTH = 3;
    private const char PIXEL_ON = '#';
    private const char PIXEL_OFF = '.';

    private string _screenBuffer = string.Empty;

    public void Draw(CpuState cpuState)
    {
        var rayX = (cpuState.Cycle - 1) % DISPLAY_WIDTH;
        var sprite = Enumerable.Range(cpuState.X-1, SPRITE_WIDTH);

        _screenBuffer += sprite.Contains(rayX) ? PIXEL_ON : PIXEL_OFF;

        if (rayX == DISPLAY_WIDTH - 1)
        {
            _screenBuffer += '\n';
        }
    }

    public void Display()
    {
        Console.Write(_screenBuffer);
        _screenBuffer = string.Empty;
    }
}
