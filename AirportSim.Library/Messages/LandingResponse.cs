using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text.Json.Serialization;

namespace AirportSim.Library.Messages;

public sealed class LandingResponse
{
    [JsonInclude] public Vector2? Enter { get; set; }
    [JsonInclude] public Vector2? LandingZone { get; set; }
    [JsonInclude] public float AirportZoneRadius { get; set; }
    
    [JsonIgnore]
    [MemberNotNullWhen(true, nameof(LandingZone), nameof(Enter))]
    public bool IsLandingAccepted => Enter is not null && LandingZone is not null;
    
    
    public LandingResponse() { }
    
    public LandingResponse(Vector2? enter, Vector2? landingZone, float airportZoneRadius)
    {
        Enter = enter;
        LandingZone = landingZone;
        AirportZoneRadius = airportZoneRadius;
    }
    
    public LandingResponse(float airportZoneRadius)
    {
        AirportZoneRadius = airportZoneRadius;
    }


    public override string ToString() => IsLandingAccepted 
        ? $"Landing(e={Enter}, lz={LandingZone}, r={AirportZoneRadius:F0})"
        : $"Landing(forbidden)";
}