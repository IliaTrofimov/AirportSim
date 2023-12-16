using System.Diagnostics;
using AirportSim.Library.Communications;
using AirportSim.Library.Messages;
using AirportSim.Library.Utils;

namespace AirportSim.Controller;

public class Host
{
    public static async Task StartHost(IMessengingClient messengingClient)
    {
        messengingClient.Connect();
        
        IO.WriteLine("*** Host is running ***");
        IO.Write("Press any key to send SystemExitMessage for all agents... ", ConsoleColor.Gray);
        Console.ReadKey();
        Console.WriteLine();
        messengingClient.SendMessage(new SystemExitMessage());
       
        IO.WriteLine("SystemExitMessage was sent.");
    }
}