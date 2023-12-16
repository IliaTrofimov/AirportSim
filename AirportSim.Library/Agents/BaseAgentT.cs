using AirportSim.Library.Communications;
using AirportSim.Library.States;

namespace AirportSim.Library.Agents;


/// <summary>Base class for agents with some state. Agents must implement <see cref="Loop(TState)"/> method which controls lifecycle.</summary>
public abstract class BaseAgent<TState>(string? id, AgentSettings settings, IMessengingClient messengingClient) 
    : BaseAgent(id, settings, messengingClient)
    where TState : BaseAgentState, new()
{
    /// <summary>Start simulation with given initial state.</summary>
    public void Loop(TState state)
    {
        startTime = DateTime.Now;
        currentStep = 0;
        LogConsole($"{ToString()} loop started at {startTime:HH:mm:ss.ff}");
        
        MessengingClient.Connect(Type, Id);
        LogConsole("MessengingClient.Connect");
        
        state = Initialize(state);
        SendMessages();
        LogConsole("Initialize");
        
        for (var shouldContinue = true; shouldContinue; currentStep++)
        {
            var stepStart = DateTime.Now;
            
            OutgoingMessages.Clear();
            IncomingMessages = MessengingClient.ReceiveMessages(ToString());
            if (IncomingMessages.Count != 0)
                LogConsole($"Has {IncomingMessages.Count} incoming messages");
            
            (shouldContinue, state) = NextStep(state);
            SendMessages();

            LogState(state);
            LogConsole("Step is completed with state:", state);
            
            Sleep(stepStart);
        }
        
        TearDown(state);
        SendMessages();
        LogConsole("TearDown");

        MessengingClient.Disconnect();
        LogConsole("MessengingClient.Disconnect");
        LogConsole($"Agent loop finished at {DateTime.Now:HH:mm:ss.ff}");
    }

    public sealed override void Loop() => Loop(new TState());
    
    protected abstract (bool shouldContinue, TState state) NextStep(TState currentState);

    protected virtual TState Initialize(TState initialState) => initialState;
    protected virtual void TearDown(TState finalState) {}
    
    
    private void LogState(TState state)
    {
        if (LogFile is not null)
            File.AppendAllText(LogFile, state.ToCsv() + "\n");
    }
    
}
