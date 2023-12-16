using AirportSim.Library.Communications;
using AirportSim.Library.Messages;
using AirportSim.Library.States;

namespace AirportSim.Library.Agents.Environment;


public sealed class EnvironmentAgent(EnvironmentSettings settings, IMessengingClient messengingClient)
    : BaseAgent<EnvironmentState>("environment", settings, messengingClient)
{
    protected override (bool shouldContinue, EnvironmentState state) NextStep(EnvironmentState currentState)
    {
        if (IncomingMessages.Any(m => m is SystemExitMessage || m.Type == nameof(SystemExitMessage)))
        {
            LogConsole(nameof(SystemExitMessage));
            return (false, currentState);
        }
        

        if (Random.NextSingle() < settings.WeatherChangeProbability)
        {
            var shouldIncrease = Random.NextSingle() > 0.5f;

            if (shouldIncrease && currentState.Weather != WeatherType.Storm)
            {
                currentState.Weather++;
                OutgoingMessages.Add(new WeatherUpdateMessage(currentState.Weather, GetAccidentP(currentState), this));
            }
            else if (!shouldIncrease && currentState.Weather != WeatherType.Clear)
            {
                currentState.Weather--;
                OutgoingMessages.Add(new WeatherUpdateMessage(currentState.Weather, GetAccidentP(currentState), this));
            } 
        }

        return (true, currentState);
    }
    
    private float GetAccidentP(EnvironmentState currentState) =>
        currentState.Weather switch
        {
            WeatherType.Clear =>  0.00010f,
            WeatherType.Clouds => 0.00012f,
            WeatherType.Rain =>   0.00025f,
            WeatherType.Storm =>  0.00035f,
            _ => 0
        };
}