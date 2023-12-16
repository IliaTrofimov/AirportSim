using System.Text;
using System.Text.Json;
using AirportSim.Library.Messages;
using RabbitMQ.Client;

namespace AirportSim.Library.Communications;


public sealed class RabbitMqClient : IMessengingClient
{
    public const string TopicExchange = "agents_topic";
    
    private readonly JsonSerializerOptions jsonOpt = new () { IncludeFields = true };
    private readonly string defaultExchange;
    private readonly ConnectionFactory connectionFactory;
    private IConnection connection;
    private IModel model;


    public RabbitMqClient(string host, int port, string username = "", string password = "")
    {
        connectionFactory = new ConnectionFactory
        {
            UserName = username,
            Password = password,
            HostName = host,
            Port = port
        };
    }
    
    public void SendMessage<TMessage>(TMessage message) where TMessage : Message
    {
        var json = JsonSerializer.Serialize(message, jsonOpt);
        var body = Encoding.UTF8.GetBytes(json);
        model.BasicPublish(TopicExchange, message.RoutingKey, body: body);
    }
    
    public void SendMessage<TMessage, TPayload>(TMessage message) where TMessage : Message<TPayload>
    {
        var json = JsonSerializer.Serialize(message, jsonOpt);
        var body = Encoding.UTF8.GetBytes(json);
        model.BasicPublish(TopicExchange, message.RoutingKey, body: body);
    }

    public List<Message> ReceiveMessages(string queue)
    {
        var count = model.MessageCount(queue);
        var messages = new List<Message>((int)count);
        for (var i = 0; i < count; i++)
        {
            var result = model.BasicGet(queue, true);
            if (result is null) continue;
            
            var json = Encoding.UTF8.GetString(result.Body.ToArray());
            var msg = JsonSerializer.Deserialize<Message>(json, jsonOpt);
            if (msg is not null)
                messages.Add(msg);
        }

        return messages;
    }
    
    public Message? ReceiveMessage(string queue)
    {
        var result = model.BasicGet(queue, true);
        if (result is null) return null;
        
        var json = Encoding.UTF8.GetString(result.Body.ToArray());
        var msg = JsonSerializer.Deserialize<Message>(json, jsonOpt);
        return msg;
    }
    
    public Message? PeekMessage(string queue)
    {
        var result = model.BasicGet(queue, true);
        if (result is null) return null;
        
        var json = Encoding.UTF8.GetString(result.Body.ToArray());
        var msg = JsonSerializer.Deserialize<Message>(json, jsonOpt);
        return msg;
    }
    
    
    public void Connect()
    {
        connection = connectionFactory.CreateConnection();
        model = connection.CreateModel();
        model.ExchangeDeclare(TopicExchange, ExchangeType.Topic, durable: true);
    }
    
    public void Connect(string type, string id)
    {
        Connect();
        
        var q = $"{type}.{id}";
        model.QueueDeclare(q, durable: true, autoDelete: false);
        model.QueueBind(q, TopicExchange, "");          // all
        model.QueueBind(q, TopicExchange, $"{type}");   // by type
        model.QueueBind(q, TopicExchange, $"{type}.*"); // by type and id

    }

    public void Disconnect()
    {
        model.Close();
        connection.Close();
    }
    
    public void Dispose()
    {
        model.Dispose();
        connection.Dispose();
    }
}