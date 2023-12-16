using System.Diagnostics.CodeAnalysis;
using AirportSim.Library.States;
using AirportSim.Library.Utils;

namespace AirportSim.Library.Agents.Dispatcher;

/// <summary>List of all planes known to the dispatcher.</summary>
public sealed class PlanesList
{
    private readonly Dictionary<string, PlaneState> planesWaiting = new();
    private readonly Dictionary<string, PlaneState>[] planesByRoute;

    public PlanesList(int routesCount)
    {
        planesByRoute = new Dictionary<string, PlaneState>[routesCount];
        for (var i = 0; i < routesCount; i++)
            planesByRoute[i] = new Dictionary<string, PlaneState>();
    }

    public bool TryGetPlane(string planeAgentId, [NotNullWhen(true)] out PlaneState? planeState, out int route)
    {
        route = -1;
        if (planesWaiting.TryGetValue(planeAgentId, out planeState))
            return true;
        
        for (route = 0; route < planesByRoute.Length; route++)
            if (planesByRoute[route].TryGetValue(planeAgentId, out planeState))
                return true;

        route = -1;
        planeState = null;
        return false;
    }
    
    public bool TryGetPlane(string planeAgentId, [NotNullWhen(true)] out PlaneState? planeState) 
        => TryGetPlane(planeAgentId, out planeState, out _);

    public IEnumerable<PlaneState> PlanesByRoute(int routeId)
    {
        if (routeId < 0 || routeId >= planesByRoute.Length)
            return Enumerable.Empty<PlaneState>();
        return planesByRoute[routeId].Values;
    }
    
    public IEnumerable<PlaneState> PlanesWaiting() 
        => planesWaiting.Values;

    public int PlanesWaitingCount() => planesWaiting.Count;

    public int PlanesByRouteCount(int routeId)
    {
        if (routeId < 0 || routeId >= planesByRoute.Length)
            return 0;
        return planesByRoute[routeId].Count;
    }

    
    public bool ContainsPlane(string planeAgentId) 
        => planesWaiting.ContainsKey(planeAgentId) || planesByRoute.Any(route => route.ContainsKey(planeAgentId));
    
    public bool RemovePlane(string planeAgentId) 
        => planesWaiting.Remove(planeAgentId) || planesByRoute.Any(route => route.Remove(planeAgentId));

    public void UpdatePlane(string planeAgentId, PlaneState newPlaneState)
    {
        if (planesWaiting.ContainsKey(planeAgentId))
            planesWaiting[planeAgentId] = newPlaneState;
        else
        {
            foreach (var planes in planesByRoute)
            {
                if (!planes.ContainsKey(planeAgentId)) continue;
                planes[planeAgentId] = newPlaneState;
                return;
            }
        }
    }

    public void AddToWaiting(string planeAgentId, PlaneState planeState) 
        => planesWaiting.ForceAdd(planeAgentId, planeState);
    
    public void AddToRoute(string planeAgentId, PlaneState planeState, int routeId) 
        => planesByRoute[routeId].ForceAdd(planeAgentId, planeState);


    public override string ToString() => $"Routes={planesByRoute.Length}, Waiting planes={planesWaiting.Count}";
}