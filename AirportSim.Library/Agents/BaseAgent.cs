using AirportSim.Library.Communications;
using AirportSim.Library.Messages;
using AirportSim.Library.Utils;

namespace AirportSim.Library.Agents;


/// <summary>
/// Base class for all agents. Agents must implement <see cref="Loop"/> method which controls lifecycle.
/// </summary>
public abstract class BaseAgent
{
    protected int currentStep = 0;
    protected DateTime startTime;
    
    protected readonly float SleepTime;
    protected readonly float TimeStep;
    protected readonly IMessengingClient MessengingClient;
    protected readonly string? LogFile;
    protected readonly Random Random;
    protected List<Message> IncomingMessages = new();
    protected List<Message> OutgoingMessages = new();

    
    public string Id { get; protected set; }
    public string Type => GetType().Name;


    protected BaseAgent(string? id, AgentSettings settings, IMessengingClient messengingClient)
    {
        Id = id ?? Guid.NewGuid().ToString()[..8];
        SleepTime = settings.SleepTime;
        TimeStep = settings.TimeStep;
        Random = settings.Seed is null ? new Random() : new Random(settings.Seed.Value);
        MessengingClient = messengingClient;

        if (settings.LogFile is not null)
            LogFile = $"{settings.LogFile}/{Type}_{Id}.csv";
    }



    public void Dispose() => MessengingClient.Dispose();
    public sealed override string ToString() => $"{Type}.{Id}";
    
    /// <summary>Starts simulation loop with default agent state.</summary>
    public abstract void Loop();
    
    protected void Sleep(DateTime start)
    {
        if (SleepTime == 0)
            return;
        var ms = (int)(start.AddSeconds(SleepTime) - start).TotalMilliseconds;
        if (ms > 0)
            Thread.Sleep(ms);
    }
    
    protected void SendMessages()
    {
        foreach (var msg in OutgoingMessages)
        {
            MessengingClient.SendMessage(msg);
            LogConsole("Message was sent", msg);
        }
    }
    
    protected void LogConsole(string msg)
    {
        IO.WriteLine(
            ($"[{currentStep}][{DateTime.Now:HH:mm:ss}][{(DateTime.Now - startTime).TotalSeconds:F1} s] ", ConsoleColor.Yellow), 
            (msg, null));
    }
    
    protected void LogConsole(string msg, object state)
    {
        IO.WriteLine(
            ($"[{currentStep}][{DateTime.Now:HH:mm:ss}][{(DateTime.Now - startTime).TotalSeconds:F1} s] ", ConsoleColor.Yellow), 
            (msg, null));
        IO.WriteLine($"\t{state}", ConsoleColor.Gray);
    }

    protected IEnumerable<Message> SelectMessage(IEnumerable<Message> messages)
    {
        return messages.Where(m => (m.ReceiverId is null || m.ReceiverId.Equals(Id, StringComparison.OrdinalIgnoreCase))
                                   && !m.SenderId.Equals(Id, StringComparison.OrdinalIgnoreCase));
    }
}