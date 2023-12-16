namespace AirportSim.Library.Utils;

public static class IO
{
    private static ConsoleColor PromtColor = ConsoleColor.Yellow;
    private static ConsoleColor MessageColor = ConsoleColor.White;
    private static ConsoleColor InputColor = ConsoleColor.White;

    
    public static float? ReadFloat(string prompt)
    {
        Console.ForegroundColor = PromtColor;
        Console.Write(prompt + ": ");
        
        Console.ForegroundColor = InputColor;
        float? res = null;
        if (float.TryParse(Console.ReadLine(), out var _res))
            res = _res;
        
        Console.ResetColor();
        return res;
    }
    
    public static int? ReadInt(string prompt)
    {
        Console.ForegroundColor = PromtColor;
        Console.Write(prompt + ": ");
        
        Console.ForegroundColor = InputColor;

        int? res = null;
        if (int.TryParse(Console.ReadLine(), out var _res))
            res = _res;
        
        Console.ResetColor();
        return res;
    }
    
    public static string? ReadStr(string prompt)
    {
        Console.ForegroundColor = PromtColor;
        Console.Write(prompt + ": ");
        
        Console.ForegroundColor = InputColor;
        var res = Console.ReadLine();
        
        Console.ResetColor();
        return res;
    }
    
    public static void Write(string prompt, ConsoleColor? color = null)
    {
        Console.ForegroundColor = color ?? MessageColor;
        Console.Write(prompt);
        Console.ResetColor();
    }
    
    public static void WriteLine(string prompt, ConsoleColor? color = null)
    {
        Console.ForegroundColor = color ?? MessageColor;
        Console.WriteLine(prompt);
        Console.ResetColor();
    }
    
    public static void Write(params (string prompt, ConsoleColor? color)[] prompts)
    {
        foreach (var (prompt, color) in prompts)
            Write(prompt, color);
    }
    
    public static void WriteLine(params (string prompt, ConsoleColor? color)[] prompts)
    {
        foreach (var (prompt, color) in prompts)
            Write(prompt, color);
        Console.WriteLine();
    }
}