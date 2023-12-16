using System.Collections;
using AirportSim.Library.Messages;

namespace AirportSim.Library.Communications;

public sealed class LocalMessengingClient : IMessengingClient
{
    private static LocalMessengingClient? _instance;
    
    private Dictionary<string, Queue<Message>> queues = new();
    
    
    private LocalMessengingClient() {}

    public static LocalMessengingClient Create()
    {
        return _instance ??= new LocalMessengingClient();
    }



    public void SendMessage<TMessage>(TMessage message) where TMessage : Message
    {
        foreach (var kvp in queues.Where(kvp => MatchQueue(kvp.Key, message)))
            kvp.Value.Enqueue(message);
    }

    public List<Message> ReceiveMessages(string queue)
    {
        if (!queues.ContainsKey(queue))
            queues.Add(queue, new Queue<Message>());

        var messages = new List<Message>(queues[queue]);
        queues[queue].Clear();
        return messages;
    }

    public Message? ReceiveMessage(string queue)
    {
        if (!queues.ContainsKey(queue))
            queues.Add(queue, new Queue<Message>());
        return queues[queue].Dequeue();
    }

    public Message? PeekMessage(string queue)
    {
        if (!queues.ContainsKey(queue))
            queues.Add(queue, new Queue<Message>());
        return queues[queue].Peek();
    }
    

    public void Connect() { }

    public void Connect(string queue, string routingKey)
    {
       queues.Add($"{queue}_{routingKey}", new Queue<Message>());
    }

    public void Disconnect() => queues.Clear();

    public void Dispose() { }



    private static bool MatchQueue(string key, Message message)
    {
        return message.ReceiverId is null
            ? key.StartsWith(message.ReceiverType)
            : key == $"{message.ReceiverType}_{message.ReceiverId}";
    }
}