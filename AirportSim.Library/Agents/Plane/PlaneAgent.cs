using System.Numerics;
using AirportSim.Library.Communications;
using AirportSim.Library.Messages;
using AirportSim.Library.States;
using AirportSim.Library.Utils;

namespace AirportSim.Library.Agents.Plane;


/// <summary>
/// Plane agent class. Plane can fly towards the airport, collide with other planes, request landing,
/// change its direction (following the landing route) and land.
/// </summary>
/// <param name="id"></param>
/// <param name="settings"></param>
public sealed class PlaneAgent(string? id, PlaneSettings settings, IMessengingClient messengingClient) 
    : BaseAgent<PlaneState>(id, settings, messengingClient)
{
    private readonly float crashProbability = settings.CrashProbability;
    private readonly float randomCrashError = settings.RandomCrashError;
    private readonly float landingDeceleration = settings.LandingDeceleration;
    private float airportZoneRadius = 1500;
    private Vector2 routeEnter;
    private Vector2 landingZone;


    protected override PlaneState Initialize(PlaneState initialState)
    {
        initialState.Status = PlaneStatus.Approaching;
        initialState.Angle = initialState.Position.AngleTo();
        
        OutgoingMessages.Add(new PlaneStatusMessage(initialState, this));
        OutgoingMessages.Add(new PlanePositionMessage(initialState.Position, crashProbability, this));

        return initialState;
    }


    protected override (bool, PlaneState) NextStep(PlaneState currentState)
    {
        var prevStatus = currentState.Status;
        var status = currentState.Status;
        
        foreach (var message in SelectMessage(IncomingMessages))
        {
            switch (message)
            {
                case PlanePositionMessage positionMessage:
                    status = OnPlanePositionMessage(currentState, positionMessage);
                    break;
                case LandingResponseMessage landingResponseMessage:
                    status = OnLandingResponse(currentState, landingResponseMessage);
                    break;
                case WeatherUpdateMessage weatherUpdateMessage:
                    status = OnWeatherUpdateMessage(currentState, weatherUpdateMessage);
                    break;
                case SystemExitMessage:
                    LogConsole(nameof(SystemExitMessage));
                    return (false, currentState);
                default:
                    if (message.Type == nameof(SystemExitMessage))
                    {
                        LogConsole(nameof(SystemExitMessage));
                        return (false, currentState);
                    }
                    break;
            }

            // leaving loop if crashed
            if (status is PlaneStatus.Crashed)
            {
                currentState.Status = PlaneStatus.Crashed;
                OutgoingMessages.Add(new PlaneStatusMessage(currentState, this));
                return (false, currentState);
            }
        }
        
        // not crashed, calculating next movement
        currentState.Status = status;
        Move(currentState);
        
        ChangeMovement(currentState);
        
        if (prevStatus != currentState.Status)
            OutgoingMessages.Add(new PlaneStatusMessage(currentState, this));

        if (currentState.Status is PlaneStatus.Approaching or PlaneStatus.InQueue)
        {
            LogConsole("Landing requested");
            OutgoingMessages.Add(new LandingRequestMessage(this));
        }
        
        return (currentState.Status != PlaneStatus.Landed, currentState);
    }


    #region Messages handlers

    /// <summary>Checks if this agent can collide with given plane. Returns Crashed if collision will occur.</summary>
    private PlaneStatus OnPlanePositionMessage(PlaneState currentState, PlanePositionMessage message)
    {
        if (currentState.Status is not PlaneStatus.Crashed &&
            IsCollided(currentState.Position, message.Payload.Position, message.Payload.CrashProbability))
        {
            LogConsole("Collied with", $"{message.SenderType}.{message.SenderId}");
            return PlaneStatus.Crashed;
        }
        return currentState.Status;
    }
    
    /// <summary>If landing is authorized resets landing zone and route enter, changes state. Else does nothing.</summary>
    private PlaneStatus OnLandingResponse(PlaneState currentState, LandingResponseMessage message)
    {
        if (!message.Payload.IsLandingAccepted)
        {
            LogConsole("Landing denied", $"queue at {message.Payload.AirportZoneRadius:F1} m");
            airportZoneRadius = message.Payload.AirportZoneRadius;
            return currentState.Status;
        }
        
        LogConsole("Landing authorized", $"enter={message.Payload.Enter}, lz={message.Payload.LandingZone}");
        landingZone = message.Payload.LandingZone.Value;
        routeEnter = message.Payload.Enter.Value;

        if (currentState.Status is PlaneStatus.Approaching)
        {
            currentState.Angle = currentState.Position.AngleTo(message.Payload.Enter);
            currentState.Status = PlaneStatus.Entering;
        }
        else if (currentState.Status is PlaneStatus.InQueue)
        {
            currentState.Status = PlaneStatus.ExitingQueue;
        }
        return currentState.Status;
    }

    /// <summary>Checks probability of random accident due to bad weather. </summary>
    private PlaneStatus OnWeatherUpdateMessage(PlaneState currentState, WeatherUpdateMessage message)
    {
        if (Random.NextSingle() < (message.Payload.AccidentProbability - float.Epsilon))
        {
            LogConsole($"Accidental crash due harsh weather {message.Payload.Weather}");
            return PlaneStatus.Crashed;
        }
        return currentState.Status;
    }

    #endregion
    
    #region Actions and helpers
    
    /// <summary>Move plane agent. If status is InQueue or ExitingQueue, then moving in circle, else in straight line.</summary>
    private void Move(PlaneState currentState)
    {
        if (currentState.Status is PlaneStatus.InQueue or PlaneStatus.ExitingQueue)
        {
            currentState.Angle += currentState.Speed / airportZoneRadius * TimeStep;
            currentState.Position = new Vector2(float.Cos(currentState.Angle), float.Sin(currentState.Angle))
                                    * airportZoneRadius;
        }
        else
        {   
            var dx = currentState.Speed * TimeStep;
            currentState.Position += dx * new Vector2(MathF.Cos(currentState.Angle), MathF.Sin(currentState.Angle));
        }
    }

    /// <summary>Change current movement.</summary>
    private void ChangeMovement(PlaneState currentState)
    {
        var dr = currentState.Speed * TimeStep;
        /*LogConsole("D_0\tR_Zone\tD_ENTR\tD_LZ\tPOSITION\tENTER\tLANDING_ZONE",
            $"{currentState.Position.Length():F1}\t" +
            $"{airportZoneRadius + dr/2:F1}\t" +
            $"{(currentState.Position - routeEnter).Length():F1}\t" +
            $"{(currentState.Position - landingZone).Length():F1}\t" +
            $"{currentState.Position}\t" +
            $"{routeEnter}\t" +
            $"{landingZone}");
        */ 
        switch (currentState.Status)
        {
            // too close to airport zone, must go to the 2nd approach
            case PlaneStatus.Approaching when currentState.Position.InRange(airportZoneRadius + dr/2):
                currentState.Status = PlaneStatus.InQueue;
                currentState.Angle = float.Pi + currentState.Angle;
                LogConsole($"State is {PlaneStatus.InQueue}", "Landing denied or has entered airport zone");
                break;
            // has entered landing route
            case (PlaneStatus.Entering or PlaneStatus.ExitingQueue) when currentState.Position.InRange(routeEnter, dr):
                currentState.Status = PlaneStatus.Descending;
                currentState.Angle = currentState.Position.AngleTo(landingZone);
                LogConsole($"State is {PlaneStatus.Descending}", "Has entered landing route");
                break;
            // has entered landing strip
            case PlaneStatus.Descending when currentState.Position.InRange(landingZone, dr):
                currentState.Status = PlaneStatus.Landing;
                currentState.Angle = currentState.Position.AngleTo();
                LogConsole($"State is {PlaneStatus.Landing}", "Has entered landing strip");
                break;
            // landing and braking
            case PlaneStatus.Landing:
                currentState.Speed -= landingDeceleration;
                if (currentState.Speed <= 0)
                {
                    currentState.Status = PlaneStatus.Landed;
                    LogConsole($"State is {PlaneStatus.Landed}");
                }
                break;
        }
    }
    
    /// <summary>Check if two planes can collide.</summary>
    private bool IsCollided(Vector2 planeA, Vector2 planeB, float? crashB = null)
    {
        var d = (planeA - planeB).Length();
        var p = (crashProbability + (crashB ?? crashProbability)) / 2f;
        var rand = Random.NextSingle() * randomCrashError;
        return 1 / (d * d / 500 + 1) + rand >= p;
    }
    
    #endregion
}