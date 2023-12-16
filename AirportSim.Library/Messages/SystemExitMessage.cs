using System.Text.Json.Serialization;

namespace AirportSim.Library.Messages;


/// <summary>System message that indicates that simulation must be stopped.</summary>
public sealed class SystemExitMessage() : Message("system", "system", null)
{ 
    [JsonInclude] public new string Payload { get; protected set; } = nameof(SystemExitMessage);
}