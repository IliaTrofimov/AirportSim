using System.Diagnostics.CodeAnalysis;
using AirportSim.Library.Exceptions;

namespace AirportSim.Library.Agents.Dispatcher;

public sealed class DispatcherSettings : AgentSettings
{
    private float minPlaneDist = 200;
    private float airportZoneRadius = 1000;
    private int routesCount = 3;
    private float airstripLength = 300;

    
    /// <summary>Minimal distance between two planes at which plane can be directed to the same route. Must be > 0.</summary>
    public float MinPlanesDistance
    {
        get => minPlaneDist;
        set
        {
            RangeException.ThrowOnOpen(value, nameof(MinPlanesDistance), min: 0);
            minPlaneDist = value;
        }
    } 
    
    /// <summary>Airport zone radius. Must be in range (<see cref="AirstripLength"/>, 1000] meters.</summary>
    public float AirportZoneRadius
    {
        get => airportZoneRadius;
        set
        {
            RangeException.ThrowOnClosedRight(value, nameof(AirportZoneRadius), min: AirstripLength, max: 1000f);
            airportZoneRadius = value;
        }
    } 
    
    
    /// <summary>Landing routes count. Must be in range [1, 10].</summary>
    public int RoutesCount
    {
        get => routesCount;
        set
        {
            RangeException.ThrowOnClosed(value, nameof(RoutesCount), min: 1, max: 10);
            routesCount = value;
        }
    } 
    
    /// <summary>Airstrip length. Must be in range [100, <see cref="AirportZoneRadius"/>] meters.</summary>
    public float AirstripLength  
    {
        get => airstripLength;
        set
        {
            RangeException.ThrowOnClosed(value, nameof(AirstripLength), min: 100f, max: float.Max(AirportZoneRadius, 1000f));
            airstripLength = value;
        }
    } 
    
    public int MaxPlanesAtRoute { get; set; }
    

    public DispatcherSettings() {}
    
    
    [SetsRequiredMembers]
    public DispatcherSettings(float timeStep, float sleepTime = 0, string? logFile = null)
        : base(timeStep, sleepTime, logFile)
    {
    }
}