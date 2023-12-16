using System.Numerics;
using AirportSim.Library.Communications;
using AirportSim.Library.Exceptions;
using AirportSim.Library.Messages;
using AirportSim.Library.States;
using AirportSim.Library.Utils;

namespace AirportSim.Library.Agents.Dispatcher;

public sealed class DispatcherAgent : StatelessAgent
{
    private readonly float minPlanesDistance;
    private readonly float airportZoneRadius;
    private readonly int maxPlanesAtRoute;
    private readonly LandingRoute[] routes;
    private readonly Vector2 landingZoneA; 
    private readonly Vector2 landingZoneB;

    private readonly PlanesList planes;
    
    
    public DispatcherAgent(DispatcherSettings settings, IMessengingClient messengingClient)
        : base("dispatcher", settings, messengingClient)
    {
        landingZoneA = new Vector2(-settings.AirstripLength / 2, 0);
        landingZoneB = new Vector2(settings.AirstripLength / 2, 0);
        airportZoneRadius = settings.AirportZoneRadius;
        minPlanesDistance = settings.MinPlanesDistance;
        maxPlanesAtRoute = settings.MaxPlanesAtRoute;
        
        planes = new PlanesList(settings.RoutesCount);
        
        routes = new LandingRoute[settings.RoutesCount];
        
        var dA = 360f / settings.RoutesCount;
        for (var i = 0; i < settings.RoutesCount; i++)
        {
            var enter = new Vector2(airportZoneRadius * float.Cos(dA * i), airportZoneRadius * float.Sin(dA * i));
            
            // which landingZone is closer to current enter?
            if (float.Abs(enter.X - landingZoneA.X) < float.Abs(enter.X - landingZoneB.X))
                routes[i] = new LandingRoute(i, enter, landingZoneA);
            else
                routes[i] = new LandingRoute(i, enter, landingZoneB);
        }
    }

    protected override bool NextStep()
    {
        foreach (var message in SelectMessage(IncomingMessages))
        {
            switch (message)
            {
                case PlaneStatusMessage planeStatus:
                    OnPlaneStatusMessage(planeStatus);
                    break;
                case LandingRequestMessage landingRequest:
                    OnLandingRequestMessage(landingRequest);
                    break;
                case SystemExitMessage:
                    LogConsole(nameof(SystemExitMessage));
                    return false;
                default:
                    if (message.Type == nameof(SystemExitMessage))
                        LogConsole(nameof(SystemExitMessage));
                    return false;
            }
        }
        return true;
    }
    

    #region Messages handlers
    
    private void OnLandingRequestMessage(LandingRequestMessage message)
    {
        var planeId = message.SenderId;
        LogConsole($"Landing request for {message.SenderType}.{message.SenderId}");
        
        if (!planes.TryGetPlane(planeId, out var plane, out var rId) || rId != -1) 
            return;
        
        /*LogConsole($"Closest routes for {planeId} ({plane})",
            string.Join("\n\t", routes.OrderBy(r => (plane.Position - r.Enter).LengthSquared())
                                    .Select(r => $"{r.Enter} - {(plane.Position - r.Enter).Length():F1}")));
        */
        
        foreach (var route in routes.OrderBy(r => (plane.Position - r.Enter).LengthSquared())
                     .Where(r => planes.PlanesByRouteCount(r.Id) <= maxPlanesAtRoute))
        {
            var nearestPlane = planes.PlanesByRoute(route.Id).FirstOrDefault();
            if (nearestPlane is null || (nearestPlane.Position - plane.Position).Length() > minPlanesDistance)
            {
                LogConsole($"Landing authorized for {message.SenderType}.{message.SenderId} at:" ,
                    $"route={route.Id}, enter={route.Enter}, lz={route.LandingZone}");
                
                OutgoingMessages.Add(new LandingResponseMessage(route.Enter, route.LandingZone, airportZoneRadius, this, planeId));
                planes.RemovePlane(planeId);
                planes.AddToRoute(planeId, plane, route.Id);
                return;
            }
        }
        
        LogConsole($"Landing denied for {message.SenderType}.{message.SenderId}");
        OutgoingMessages.Add(new LandingResponseMessage(airportZoneRadius, this, planeId));
    }

    private void OnPlaneStatusMessage(PlaneStatusMessage message)
    {
        var planeId = message.SenderId;
        var planeStatus = message.Payload.Status;

        if (!planes.ContainsPlane(planeId) && !planeStatus.IsFinalPlaneStatus())
        {
            LogConsole($"Added new plane {planeId}", message.Payload);
            planes.AddToWaiting(planeId, message.Payload);
        }
        else if (planeStatus.IsFinalPlaneStatus())
        {
            LogConsole($"Removed plane {planeId} with status {planeStatus}", message.Payload);
            planes.RemovePlane(planeId);
        }
        else 
            planes.UpdatePlane(planeId, message.Payload);
    }

    #endregion
}