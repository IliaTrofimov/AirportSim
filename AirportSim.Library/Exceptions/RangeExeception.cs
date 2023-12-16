namespace AirportSim.Library.Exceptions;

[Flags]
public enum RangeType
{
    Open = 0b00,
    ClosedLeft = 0b01, 
    ClosedRight = 0b10, 
    Closed = 0b11
}

/// <summary>Given value was not in right range.</summary>
public sealed class RangeException(double min = double.NegativeInfinity,
        double max = double.PositiveInfinity,
        RangeType type = RangeType.Open,
        double? value = null,
        string? name = null)
    : ArgumentException(CreateMsg(min, max, type, value, name))
{
    public double Min { get; private set; } = min;
    public double Max { get; private set; } = max;
    public RangeType RangeType { get; private set; } = type;
    public double? Value { get; private set; } = value;
    public string? Name { get; private set; } = name;
    
    
    /// <summary>Throw <see cref="RangeException"/> if given value is not matching range.</summary>
    public static void Throw(double value, string? name,
        double min = double.NegativeInfinity, 
        double max = double.PositiveInfinity,
        RangeType type = RangeType.Open)
    {
        var condition = 
            type.HasFlag(RangeType.ClosedLeft) && value < min ||
            type.HasFlag(RangeType.ClosedRight) && value > max ||
            !type.HasFlag(RangeType.ClosedLeft) && value <= min ||
            !type.HasFlag(RangeType.ClosedRight) && value >= max;
        
        if (condition)
            throw new RangeException(min, max, type, value, name);
    }
    
    /// <summary>Throw <see cref="RangeException"/> if given value is not matching range.</summary>
    public static void ThrowOnClosedRight(double value, string name,
        double min = double.NegativeInfinity, 
        double max = double.PositiveInfinity)
    {
        Throw(value, name, min, max, RangeType.ClosedRight);
    }
    
    /// <summary>Throw <see cref="RangeException"/> if given value is not matching range.</summary>
    public static void ThrowOnClosedLeft(double value, string name,
        double min = double.NegativeInfinity, 
        double max = double.PositiveInfinity)
    {
        Throw(value, name, min, max, RangeType.ClosedLeft);
    }
    
    /// <summary>Throw <see cref="RangeException"/> if given value is not matching range.</summary>
    public static void ThrowOnClosed(double value, string? name,
        double min = double.NegativeInfinity, 
        double max = double.PositiveInfinity)
    {
        Throw(value, name, min, max, RangeType.Closed);
    }
    
    /// <summary>Throw <see cref="RangeException"/> if given value is not matching range.</summary>
    public static void ThrowOnOpen(double value, string? name,
        double min = double.NegativeInfinity, 
        double max = double.PositiveInfinity)
    {
        Throw(value, name, min, max, RangeType.Open);
    }
    
    /// <summary>Throw <see cref="RangeException"/> if given value is not matching range.</summary>
    public static void Throw(double value,
        double min = double.NegativeInfinity, 
        double max = double.PositiveInfinity,
        RangeType type = RangeType.Open)
    {
        Throw(value, null, min, max, type);
    }
    
    /// <summary>Throw <see cref="RangeException"/> if given value is not matching range.</summary>
    public static void ThrowOnClosedRight(double value,
        double min = double.NegativeInfinity, 
        double max = double.PositiveInfinity)
    {
        Throw(value, null, min, max, RangeType.ClosedRight);
    }
    
    /// <summary>Throw <see cref="RangeException"/> if given value is not matching range.</summary>
    public static void ThrowOnClosedLeft(double value,
        double min = double.NegativeInfinity, 
        double max = double.PositiveInfinity)
    {
        Throw(value, null, min, max, RangeType.ClosedLeft);
    }
    
    /// <summary>Throw <see cref="RangeException"/> if given value is not matching range.</summary>
    public static void ThrowOnClosed(double value,
        double min = double.NegativeInfinity, 
        double max = double.PositiveInfinity)
    {
        Throw(value, null, min, max, RangeType.Closed);
    }
    
    /// <summary>Throw <see cref="RangeException"/> if given value is not matching range.</summary>
    public static void ThrowOnOpen(double value,
        double min = double.NegativeInfinity, 
        double max = double.PositiveInfinity)
    {
        Throw(value, null, min, max, RangeType.Open);
    }
    

    private static string CreateMsg(double min, double max, RangeType type, double? value, string? name)
    {
        var brackets = (
            l: type.HasFlag(RangeType.ClosedLeft) ? '[' : '(', 
            r: type.HasFlag(RangeType.ClosedRight) ? ']' : ')'
            );
        
        var val = value is not null ? $" but got {value}" : "";
        return $"{name ?? "Value"} must be in range {brackets.l}{min}, {max}{brackets.r}{val}";
    }
}