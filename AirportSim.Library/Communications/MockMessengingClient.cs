using AirportSim.Library.Messages;

namespace AirportSim.Library.Communications;


public sealed class MockMessengingClient : IMessengingClient
{
    private readonly List<Message> sentMessages = new List<Message>();
    private readonly Queue<Message> messages = new();
    private readonly int maxReceiveMsg;

    public IEnumerable<Message> SentMessages => sentMessages;
    
    public MockMessengingClient(int maxReceiveMsg = 5, ICollection<Message>? initialMessages = null)
    {
        this.maxReceiveMsg = maxReceiveMsg;
        if (initialMessages != null)
        {
            foreach (var msg in initialMessages)
                messages.Enqueue(msg);
        }
    }

    public void CreateStubMessage<TMessage>(Message message) => messages.Enqueue(message);
    public void CreateStubMessage<TMessage, T>(Message<T> message) => messages.Enqueue(message);


    public List<Message> ReceiveMessages(string queue)
    {
        var received = new List<Message>(maxReceiveMsg);
        for (var i = 0; i < maxReceiveMsg && messages.Count != 0; i++)
            received.Add(messages.Dequeue());
        return received;
    }

    public Message? ReceiveMessage(string queue)
    {
        return messages.Count != 0 ? messages.Dequeue() : null;
    }
    
    public Message? PeekMessage(string queue)
    {
        return messages.Count != 0 ? messages.Peek() : null;
    }

    public void SendMessage<TMessage>(TMessage message) where TMessage : Message
    {
        sentMessages.Add(message);
    }
    
    public void Connect() { }
    public void Connect(string queue, string routingKey) { }
    public void Disconnect() { }

    public void Dispose() { }
}