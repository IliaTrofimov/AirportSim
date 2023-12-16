using AirportSim.Library.Agents.Dispatcher;
using AirportSim.Library.Agents.Plane;

namespace AirportSim.Library.Messages;

public sealed class LandingRequestMessage : Message
{
    public LandingRequestMessage() {}
    
    public LandingRequestMessage(string senderType, string senderId, string receiverType, string? receiverId = null) 
        : base(senderType, senderId, receiverType, receiverId)
    {
    }
    
    public LandingRequestMessage(PlaneAgent sender) 
        : base(sender.Type, sender.Id, nameof(DispatcherAgent))
    {
    }
}