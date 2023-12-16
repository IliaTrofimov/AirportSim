using System.Text.Json.Serialization;
using AirportSim.Library.Agents.Environment;
using AirportSim.Library.Agents.Plane;
using AirportSim.Library.States;

namespace AirportSim.Library.Messages;

public sealed class WeatherUpdateMessage : Message
{
    [JsonInclude] public new WeatherData Payload { get; private set; }
    
    public WeatherUpdateMessage() {}
    
    public WeatherUpdateMessage(WeatherData weatherData, string senderType, string senderId, string receiverType, string? receiverId = null) 
        : base(senderType, senderId, receiverType, receiverId)
    {
        Payload = weatherData;
    }
    
    public WeatherUpdateMessage(WeatherData weatherData, EnvironmentAgent sender) 
        : this(weatherData, sender.Type, sender.Id, nameof(PlaneAgent))
    {
    }
    
    public WeatherUpdateMessage(WeatherType weather, float accidentP, EnvironmentAgent sender) 
        : this(new WeatherData(weather, accidentP), sender.Type, sender.Id, nameof(PlaneAgent))
    {
    }
}