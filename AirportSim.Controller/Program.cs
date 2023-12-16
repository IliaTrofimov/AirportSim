using System.Globalization;
using System.Numerics;
using System.Text.Json;
using AirportSim.Library.Agents.Dispatcher;
using AirportSim.Library.Agents.Environment;
using AirportSim.Library.Agents.Plane;
using AirportSim.Library.Communications;
using AirportSim.Library.States;
using AirportSim.Library.Utils;


namespace AirportSim.Controller;

public class Program
{
    private const string RabbitHost = "localhost";
    private const int RabbitPort = 5672;
    private const string RabbitUser = "rmuser";
    private const string RabbitPassword = "rmpassword";

    
    public static async Task Main(string[] args)
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        
        if (args.Length == 0)
            throw new ArgumentException("Provide at least 1 parameters: <agent_type>|host [<agent_settings_path>] [<agent_id>]");
        if (!args[0].Equals("host", StringComparison.CurrentCultureIgnoreCase) && args.Length < 2)
            throw new ArgumentException("Provide at least 2 parameters: <agent_type> <agent_settings_path> [<agent_id>]");

        IO.WriteLine("*** Initializing AirportSim.Controller application ***");
        
        var rabbit = new RabbitMqClient(RabbitHost, RabbitPort, RabbitUser, RabbitPassword);
        switch (args[0].ToLower())
        {
            case "host":
                await Host.StartHost(rabbit);
                break;
            case "plane":
                StartPlaneAgent(args[1], rabbit, args.Length >= 3 ? args[2] : null);
                break;
            case "dispatcher":
                StartDispatcher(args[1], rabbit);
                break;
            case "environment":
                StartEnvironment(args[1], rabbit);
                break;
            default:
                throw new ArgumentException(
                    $"Unknown agent type '{args[0]}'. Available agents: 'plane', 'dispatcher', 'environment', 'host'");
        }
        IO.WriteLine("*** Closing AirportSim.Controller application ***");
    }
    
    private static void StartPlaneAgent(string settingsPath, IMessengingClient messengingClient, string? id = null)
    {
        if (!File.Exists(settingsPath))
            throw new ArgumentException($"Agent settings file {settingsPath} does not exist");
        
        var json = File.ReadAllText(settingsPath);
        var settings = JsonSerializer.Deserialize<PlaneSettings>(json)
                       ?? throw new Exception($"Cannot deserialize settings file {settingsPath}");

        var x = IO.ReadFloat("Enter plane Position.X (m)") ?? 2000;
        var y = IO.ReadFloat("Enter plane Position.Y (m)") ?? 1500;
        var speed = IO.ReadFloat("Enter plane speed    (m/s)") ?? 100;
        var state = new PlaneState(new Vector2(x, y), speed);
        id ??= Random.Shared.Next(1000, 9999).ToString();
        IO.WriteLine($"*** Plane_{id} agent is running ***\n");
        var agent = new PlaneAgent(id, settings, messengingClient);
        agent.Loop(state);
    }
    
    private static void StartDispatcher(string settingsPath, IMessengingClient messengingClient)
    {
        if (!File.Exists(settingsPath))
            throw new ArgumentException($"Agent settings file {settingsPath} does not exist");
        
        var json = File.ReadAllText(settingsPath);
        var settings = JsonSerializer.Deserialize<DispatcherSettings>(json)
                       ?? throw new Exception($"Cannot deserialize settings file {settingsPath}");
        
        IO.WriteLine("*** Dispatcher agent is running ***\n");
        var agent = new DispatcherAgent(settings, messengingClient);
        agent.Loop();
    }
    
    private static void StartEnvironment(string settingsPath, IMessengingClient messengingClient)
    {
        if (!File.Exists(settingsPath))
            throw new ArgumentException($"Agent settings file {settingsPath} does not exist");
        
        var json = File.ReadAllText(settingsPath);
        var settings = JsonSerializer.Deserialize<EnvironmentSettings>(json)
                       ?? throw new Exception($"Cannot deserialize settings file {settingsPath}");


        var prompt = string.Join(", ", Enum.GetValues<WeatherType>().Select(w => $"{(int)w}: {w}"));
        var weatherId = IO.ReadInt($"Enter initial weather ({prompt})");
        var weather = weatherId is null or < (int)WeatherType.Clear or > (int)WeatherType.Storm
            ? WeatherType.Clear
            : (WeatherType)weatherId.Value;

        var state = new EnvironmentState(weather);
        
        IO.WriteLine("*** Environment agent is running ***\n");
        var agent = new EnvironmentAgent(settings, messengingClient);
        agent.Loop(state);
    }
}