using System.Numerics;

namespace AirportSim.Library.Messages;

public sealed class PlanePositionPayload
{
    public Vector2 Position { get; set; }
    public float? CrashProbability { get; set; } = 0.5f;

    
    public PlanePositionPayload() {}
    
    public PlanePositionPayload(Vector2 position, float? crashProbability)
    {
        Position = position;
        CrashProbability = crashProbability;
    }

    public override string ToString() => $"Plane({Position})";
}