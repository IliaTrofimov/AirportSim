using AirportSim.Library.Messages;

namespace AirportSim.Library.Communications;

public interface IMessengingClient : IDisposable
{
    public void SendMessage<TMessage>(TMessage message) where TMessage : Message;
    public List<Message> ReceiveMessages(string queue);
    public Message? ReceiveMessage(string queue);
    public Message? PeekMessage(string queue);
    public void Connect();
    public void Connect(string queue, string routingKey);
    public void Disconnect();
}