using System.Text.Json.Serialization;

namespace AirportSim.Library.Messages;


[JsonPolymorphic]
[JsonDerivedType(typeof(LandingRequestMessage), typeDiscriminator: nameof(LandingRequestMessage))]
[JsonDerivedType(typeof(LandingResponseMessage), typeDiscriminator: nameof(LandingResponseMessage))]
[JsonDerivedType(typeof(PlanePositionMessage), typeDiscriminator: nameof(PlanePositionMessage))]
[JsonDerivedType(typeof(WeatherUpdateMessage), typeDiscriminator: nameof(WeatherUpdateMessage))]
public class Message<TPayload> : Message
{
    public sealed override bool HasPayload => true;
    [JsonInclude] public new TPayload Payload { get; protected set; }

    
    [JsonConstructor]
    public Message() {}
    
    public Message(TPayload payload, string senderType, string senderId, string receiverType, string? receiverId = null) 
        : base(senderType, senderId, receiverType, receiverId)
    {
        Payload = payload;
    }
}