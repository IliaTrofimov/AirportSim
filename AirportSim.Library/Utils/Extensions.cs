using AirportSim.Library.States;

namespace AirportSim.Library.Utils;

public static class Extensions
{
    public static IEnumerable<T> MayBeNull<T>(this IEnumerable<T>? source) => source ?? Enumerable.Empty<T>();

    public static void ForceAdd<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, TValue value)
        where TKey : notnull
    {
        source.Remove(key);
        source.Add(key, value);
    }

    public static bool IsFinalPlaneStatus(this PlaneStatus s)
        => s is PlaneStatus.Crashed or PlaneStatus.Landed;
}