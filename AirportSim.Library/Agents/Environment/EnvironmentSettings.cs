using System.Diagnostics.CodeAnalysis;
using AirportSim.Library.Communications;
using AirportSim.Library.Exceptions;

namespace AirportSim.Library.Agents.Environment;

public sealed class EnvironmentSettings : AgentSettings
{
    private float weatherChangeProbability = 0f;
    
    /// <summary>Probability of weather changing at given moment. Must be in range [0, 0.1].</summary>
    public float WeatherChangeProbability 
    {
        get => weatherChangeProbability;
        set
        {
            RangeException.ThrowOnClosed(value, nameof(WeatherChangeProbability), min: 0, max: 0.1);
            weatherChangeProbability = value;
        }
    }
    
    
    public EnvironmentSettings() {}
    
    [SetsRequiredMembers]
    public EnvironmentSettings(float timeStep, float sleepTime = 0, string? logFile = null)
        : base(timeStep, sleepTime, logFile)
    {
    }
}