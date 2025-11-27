using Dapr.Actors;
namespace Light.Interfaces;

public class LightData
{
    public string Brightness { get; set; } = "brightness";
    public string AutoDimReminder { get; set; } = "autodim";
    public LightData() { }
    public LightData(string brightness, string autoDimReminder)
    {
        Brightness = brightness;
        AutoDimReminder = autoDimReminder;
    }
    public override string ToString()
    {
        return $"Light data(Brightness: {Brightness})";
    }
}

public interface ILight : IActor
{
    Task<int> IncreaseBrightnessAsync(int delta);
    Task<int> DecreaseBrightnessAsync(int delta);
    Task<int> GetBrightnessAsync();
    Task ResetAsync();
}

