using System.Text.Json.Serialization;
using AirportSim.Library.Agents.Dispatcher;
using AirportSim.Library.Agents.Plane;
using AirportSim.Library.States;
    
namespace AirportSim.Library.Messages;

public sealed class PlaneStatusMessage : Message
{
    [JsonInclude] public new PlaneState Payload { get; private set; }
    
    public PlaneStatusMessage() {}
    
    public PlaneStatusMessage(PlaneState payload, string senderType, string senderId, string receiverType, string? receiverId = null) 
        : base(senderType, senderId, receiverType, receiverId)
    {
        Payload = payload;
    }
    
    public PlaneStatusMessage(PlaneState state, PlaneAgent sender) 
        : this(state, sender.Type, sender.Id, nameof(DispatcherAgent))
    {
    }
}