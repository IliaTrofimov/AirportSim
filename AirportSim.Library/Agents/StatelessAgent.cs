using AirportSim.Library.Communications;

namespace AirportSim.Library.Agents;


/// <summary>Base class for agents without state. Agents must implement <see cref="Loop()"/> method which controls lifecycle.</summary>
public abstract class StatelessAgent(string? id, AgentSettings settings, IMessengingClient messengingClient) 
    : BaseAgent(id, settings, messengingClient)
{
    public sealed override void Loop()
    {
        startTime = DateTime.Now;
        currentStep = 0;
        LogConsole($"{ToString()} loop started at {startTime:HH:mm:ss.ff}");
        
        MessengingClient.Connect(Type, Id);
        LogConsole("MessengingClient.Connect");
        
        Initialize();
        SendMessages();
        LogConsole("Initialize");
        
        for (var proceed = true; proceed; currentStep++)
        {
            var stepStart = DateTime.Now;
            
            OutgoingMessages.Clear();
            IncomingMessages = MessengingClient.ReceiveMessages(ToString());
            if (IncomingMessages.Count != 0)
                LogConsole($"Has {IncomingMessages.Count} incoming messages");
            
            proceed = NextStep();
            SendMessages();
            LogConsole("Step is completed");

            Sleep(stepStart);
        }
        
        TearDown();
        SendMessages();
        LogConsole("TearDown");

        MessengingClient.Disconnect();
        LogConsole("MessengingClient.Disconnect");
        LogConsole($"Agent loop finished at {DateTime.Now:HH:mm:ss.ff}");
    }

    protected abstract bool NextStep();
    
    protected virtual void Initialize() {}
    protected virtual void TearDown() {}
}