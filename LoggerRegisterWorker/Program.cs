using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory() 
{ 
    HostName = "localhost",
    Password = "admin",
    UserName = "admin"
};

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "direct_logs", type: ExchangeType.Direct);

var queueDeclare = await channel.QueueDeclareAsync();
var queueName = queueDeclare.QueueName;

await channel.QueueBindAsync(queueName, exchange: "direct_logs", routingKey: args[0]);

Console.WriteLine(" [*] Waiting for logs.");

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += (model, ea) => 
{
    var message = ea.Body.ToArray();
    var log = Encoding.UTF8.GetString(message);
    var routingKey = ea.RoutingKey;
    Console.WriteLine($" [x] Received '{routingKey}':'{log}'");

    return Task.CompletedTask;
};

await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();