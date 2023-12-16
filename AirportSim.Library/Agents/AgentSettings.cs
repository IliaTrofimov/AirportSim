using System.Diagnostics.CodeAnalysis;
using AirportSim.Library.Exceptions;

namespace AirportSim.Library.Agents;


/// <summary>
/// Basic settings for each agent.
/// </summary>
public class AgentSettings
{
    private float timeStep, sleepTime = 0;
    
    /// <summary>Path to file log.</summary>
    public string? LogFile { get; set; }

    /// <summary>
    /// Step at which simulation will increase its local time. Must be in range (0, 10] seconds.
    /// </summary>
    /// <remarks>Only used to calculate physics, execution time will not be affected.</remarks>
    public required float TimeStep 
    {
        get => timeStep;
        set
        {
            RangeException.ThrowOnClosedRight(value, nameof(TimeStep), min: 0, max: 10);
            timeStep = value;
        }
    }
    
    /// <summary>
    /// Simulation will pause its execution for given time to synchronize with real time. Must be in range (0, 10] seconds.
    /// </summary>
    /// <remarks>If SleepTime is less then 0, TimeStep will be used instead.</remarks>
    public float SleepTime
    {
        get => sleepTime;
        set
        {
            if (value < 0)
            {
                sleepTime = timeStep;
            }
            else
            {
                RangeException.ThrowOnClosedRight(value, nameof(SleepTime), min: 0, max: 10);
                sleepTime = value;   
            }
        }
    }
    
    /// <summary>Seed for random values generator.</summary>
    public int? Seed { get; set; }
    
    
    /// <summary>Create new agent's settings object. Use this constructor with class initializer.</summary>
    public AgentSettings() { }
    
    
    /// <summary>Create new agent's settings object with given required parameters.</summary>
    /// <param name="timeStep">Step at which simulation will increase its local time. Must be in range (0, 10] seconds.</param>
    /// <param name="sleepTime">Simulation will pause its execution for given time to synchronize with real time. Must be in range (0, 1] seconds.</param>
    /// <param name="logFile">Path to the file log.</param>
    [SetsRequiredMembers]
    public AgentSettings(float timeStep, float sleepTime = 0, string? logFile = null)
    {
        TimeStep = timeStep;
        LogFile = logFile;
        SleepTime = sleepTime;
    }
}