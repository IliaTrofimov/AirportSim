using System.Text.Json;

namespace AirportSim.Library.Messages;

public static class MessageConverter
{
    private static readonly JsonSerializerOptions jsonOpt = new () { IncludeFields = true };

    public static string Serialize<T>(T message) where T : Message
    {
        return JsonSerializer.Serialize(message, jsonOpt);
    }

    public static object? Deserialize(string json)
    {
        var msgBase = JsonSerializer.Deserialize<Message>(json, jsonOpt);
        return msgBase?.Type switch
        {
            nameof(LandingRequestMessage) => msgBase as LandingRequestMessage ?? msgBase,
            nameof(LandingResponseMessage) => msgBase as LandingResponseMessage ?? msgBase,
            nameof(PlanePositionMessage) => msgBase as PlanePositionMessage ?? msgBase,
            nameof(SystemExitMessage) => msgBase as SystemExitMessage ?? msgBase,
            nameof(PlaneStatusMessage) => msgBase as PlaneStatusMessage ?? msgBase,
            nameof(WeatherUpdateMessage) => msgBase as WeatherUpdateMessage ?? msgBase,
            _ => msgBase
        };
    }
}