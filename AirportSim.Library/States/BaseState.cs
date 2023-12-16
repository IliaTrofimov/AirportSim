namespace AirportSim.Library.States;


/// <summary>Base class for agent's state. Contains only information about time when this state was created.</summary>
public class BaseAgentState
{
    public DateTime Time { get; private set; } = DateTime.Now;
    
    public virtual string ToCsv() => string.Empty;
}