using System.Numerics;

namespace AirportSim.Library.Utils;

public static class PhysicsHelpers
{
    /// <summary>Degrees to radians.</summary>
    public static float ToRadians(float degrees) => degrees / 180.0f * MathF.PI;
    
    /// <summary>Radians to degrees.</summary>
    public static float ToDegrees(float radians) => radians / MathF.PI * 180.0f;
    
    /// <summary>Degrees to radians.</summary>
    public static double ToRadians(double degrees) => degrees / 180.0 * Math.PI;
    
    /// <summary>Radians to degrees.</summary>
    public static double ToDegrees(double radians) => radians / Math.PI * 180.0;
    

    public static float AngleTo(this Vector2 a, Vector2? b) => b is not null 
        ? float.Atan2(b.Value.Y - a.Y, b.Value.X - a.X)
        : 0f;

    public static float AngleTo(this Vector2 a) => float.Atan2(-a.Y, -a.X);

    public static bool InRange(this Vector2 a, Vector2 b, float range) => (a - b).Length() <= range;
    
    public static bool InRange(this Vector2 a, float range) => a.Length() <= range;

}