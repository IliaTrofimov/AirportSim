using System.Numerics;

namespace AirportSim.Library.Agents.Dispatcher;


/// <summary>Parameters of landing route.</summary>
public sealed class LandingRoute
{
    public int Id { get; private set; }
    public Vector2 Enter { get; private set; }
    public Vector2 LandingZone { get; private set; }
    
    public LandingRoute(int id, Vector2 enter, Vector2 landingZone)
    {
        Id = id;
        Enter = enter;
        LandingZone = landingZone;
    }
    
    
    public override string ToString() => $"Id={Id} Enter={Enter} LZ={LandingZone}";
}