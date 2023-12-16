using System.Numerics;
using System.Text.Json.Serialization;
using AirportSim.Library.Agents.Dispatcher;
using AirportSim.Library.Agents.Plane;

namespace AirportSim.Library.Messages;


public sealed class LandingResponseMessage : Message
{
    [JsonInclude] public new LandingResponse Payload { get; protected set; }
    
    
    public LandingResponseMessage() {}
    
    public LandingResponseMessage(LandingResponse payload, string senderType, string senderId, string receiverType, string? receiverId = null) 
        : base(senderType, senderId, receiverType, receiverId)
    {
        Payload = payload;
    }
    
    public LandingResponseMessage(LandingResponse payload, DispatcherAgent sender, string receiverId) 
        : this(payload, sender.Type, sender.Id, nameof(PlaneAgent), receiverId)
    {
    }
    
    public LandingResponseMessage(Vector2 enter, Vector2 landing, float airportZoneRadius, DispatcherAgent sender, string receiverId) 
        : this(new LandingResponse(enter, landing, airportZoneRadius), sender.Type, sender.Id, nameof(PlaneAgent), receiverId)
    {
    }
    
    public LandingResponseMessage(float airportZoneRadius, DispatcherAgent sender, string receiverId) 
        : this(new LandingResponse(airportZoneRadius), sender.Type, sender.Id, nameof(PlaneAgent), receiverId)
    {
    }
}