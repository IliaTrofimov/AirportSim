using System.Diagnostics.CodeAnalysis;
using AirportSim.Library.Communications;
using AirportSim.Library.Exceptions;

namespace AirportSim.Library.Agents.Plane;

public sealed class PlaneSettings : AgentSettings
{
    private float crashP = 0.5f;
    private float randCrash = 0.1f;
    private float landingDeceleration = 10;
    
    
    /// <summary>Minimal probability level at which two planes will crash. Must be in range [0, 1].</summary>
    public float CrashProbability
    {
        get => crashP;
        set
        {
            RangeException.ThrowOnClosedLeft(value, nameof(CrashProbability), min: 0, max: 1);
            crashP = value;
        }
    }

    /// <summary>Additional random error added to crash probability. Must be in range [0, 0.2].</summary>
    public float RandomCrashError 
    {
        get => randCrash;
        set
        {
            RangeException.ThrowOnClosed(value, nameof(RandomCrashError), min: 0, max: 0.2);
            randCrash = value;
        }
    }
    
    /// <summary>Speed decreasing rate when plane is braking at landing strip. Must be in range [1, 20] m/s^2.</summary>
    public float LandingDeceleration 
    {
        get => landingDeceleration;
        set
        {
            RangeException.ThrowOnClosed(value, nameof(LandingDeceleration), min: 1, max: 20);
            randCrash = value;
        }
    }
    

    public PlaneSettings() {}
    
    [SetsRequiredMembers]
    public PlaneSettings(float timeStep, float sleepTime = 0, string? logFile = null)
        : base(timeStep, sleepTime, logFile)
    {
    }
}