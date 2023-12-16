using System.Numerics;
using AirportSim.Library.Utils;

namespace AirportSim.Library.States;


/// <summary>Plane agent status.</summary>
public enum PlaneStatus
{
    /// <summary>Plane is moving towards the airport zone.</summary>
    Approaching,
    /// <summary>Plane is moving towards the route's enter.</summary>
    Entering,
    /// <summary>Plane is waiting in queue.</summary>
    InQueue,
    /// <summary>Plane is existing queue and moving towards the route's enter.</summary>
    ExitingQueue,
    /// <summary>Plane is moving on the route and preparing for landing.</summary>
    Descending,
    /// <summary>Plane is landing on the runaway.</summary>
    Landing,
    /// <summary>Plane has finally landed on the runaway.</summary>
    Landed,
    /// <summary>Plane has crashed.</summary>
    Crashed
}


/// <summary>Current plane agent state. Contains information about its position, speed, angle and status.</summary>
public sealed class PlaneState : BaseAgentState
{
    /// <summary>Current plane position (meters).</summary>
    public Vector2 Position { get; set; }
    
    /// <summary>Current plane speed (m/s).</summary>
    public float Speed { get; set; }
    
    /// <summary>Current plane heading angle (radians).</summary>
    public float Angle { get; set; }
    
    /// <summary>Current plane status.</summary>
    public PlaneStatus Status { get; set; }


    public PlaneState() { }
    
    public PlaneState(Vector2 pos, float speed, float angle = 0f)
    {
        Position = pos;
        Speed = speed;
        Angle = PhysicsHelpers.ToRadians(angle);
        Status = PlaneStatus.Approaching;
    }

    public override string ToCsv() => $"{DateTime.Now:HH:mm:ss};{Position.X:F1};{Position.Y:F1};{Speed:F1};{Angle:F3};{(int)Status}";
    
    public override string ToString() => $"[{Status}] Pos=<{Position.X:F0}, {Position.Y:F0}> Spd={Speed:F0}, Ang={PhysicsHelpers.ToDegrees(Angle):F0}°";
}