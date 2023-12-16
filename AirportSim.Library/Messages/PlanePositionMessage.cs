using System.Numerics;
using AirportSim.Library.Agents.Plane;

namespace AirportSim.Library.Messages;


public sealed class PlanePositionMessage : Message<PlanePositionPayload>
{
    
    public PlanePositionMessage() {}
    
    public PlanePositionMessage(PlanePositionPayload payload, string senderType, string senderId, string receiverType, string? receiverId = null) 
        : base(payload, senderType, senderId, receiverType, receiverId)
    {
    }
    
    public PlanePositionMessage(Vector2 position, float crashProbability, PlaneAgent sender) 
        : base(new PlanePositionPayload(position, crashProbability), sender.Type, sender.Id, nameof(PlaneAgent))
    {
    }
    
    public PlanePositionMessage(Vector2 position, float crashProbability, string senderType, string senderId) 
        : base(new PlanePositionPayload(position, crashProbability), senderType, senderId, senderType)
    {
    }
}