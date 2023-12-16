namespace AirportSim.Library.States;


public enum WeatherType
{
    Clear, Clouds, Rain, Fog, Storm  
}


public sealed class EnvironmentState : BaseAgentState
{
    public WeatherType Weather { get; set; } = WeatherType.Clear;

    public override string ToString() => $"Weather={Weather}";

    public EnvironmentState()
    {
    }
    
    public EnvironmentState( WeatherType initialWeather)
    {
        Weather = initialWeather;
    }
}
    