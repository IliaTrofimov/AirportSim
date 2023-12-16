using System.Text.Json.Serialization;

namespace AirportSim.Library.Messages;


[JsonPolymorphic]
[JsonDerivedType(typeof(SystemExitMessage), nameof(SystemExitMessage))]
[JsonDerivedType(typeof(LandingRequestMessage), nameof(LandingRequestMessage))]
[JsonDerivedType(typeof(LandingResponseMessage), nameof(LandingResponseMessage))]
[JsonDerivedType(typeof(PlanePositionMessage), nameof(PlanePositionMessage))]
[JsonDerivedType(typeof(WeatherUpdateMessage), nameof(WeatherUpdateMessage))]
[JsonDerivedType(typeof(PlaneStatusMessage), nameof(PlaneStatusMessage))]
public class Message
{
    [JsonInclude] public Guid Id { get; protected set; } = Guid.NewGuid();
    [JsonInclude] public DateTime Time { get; protected set; } = DateTime.Now;
    [JsonInclude] public string SenderId { get; protected set; }
    [JsonInclude] public string SenderType { get; protected set; }
    [JsonInclude] public string? ReceiverType { get; protected set; }
    [JsonInclude] public string? ReceiverId { get; protected set; }
    [JsonInclude] public object? Payload { get; protected set; }
    [JsonInclude] public string Type { get; private set; }

    [JsonIgnore] public virtual bool HasPayload => false;
    [JsonIgnore] public bool IsDirect => ReceiverId is not null;
    [JsonIgnore] public bool IsForAll => ReceiverType is null;
    
    
    [JsonIgnore]
    public string RoutingKey =>
        ReceiverType is null
            ? ""
            : ReceiverId is null
                ? ReceiverType
                : $"{ReceiverType}.{ReceiverId}";

    public Message()
    {
        Type = GetType().Name;
    }
    
    public Message(string senderType, string senderId, string? receiverType, string? receiverId = null)
    {
        Type = GetType().Name;
        SenderId = senderId;
        SenderType = senderType;
        ReceiverId = receiverId;
        ReceiverType = receiverType;
    }
        
    public sealed override string ToString() 
        => $"{GetType().Name}_{Id.ToString()[..8]}(f: {SenderType}.{SenderId}, t: {ReceiverType ?? "all"}.{ReceiverId ?? "all"})";
}