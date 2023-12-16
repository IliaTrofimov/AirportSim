using AirportSim.Library.States;

namespace AirportSim.Library.Messages;

public sealed class WeatherData
{
    public WeatherType Weather { get; set; }
    public float AccidentProbability { get; set; }

    public WeatherData()
    {
    }

    public WeatherData(WeatherType weather, float accidentProbability)
    {
        Weather = weather;
        AccidentProbability = accidentProbability;
    }
}