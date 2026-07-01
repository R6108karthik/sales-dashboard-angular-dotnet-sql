using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using SalesDashboard.Application.Messaging;

namespace SalesDashboard.Infrastructure.Messaging
{
  public  class RabbitMqPublisher: IMessagePublisher
  {
        private readonly IConfiguration _configuration;

        public RabbitMqPublisher(IConfiguration configuratio)
        {
            _configuration = configuratio;
        }

      public async  Task PublicAsync<T>(T message)
        {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"] ?? "localhost",
            Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
            Password = _configuration["RabbitMQ:Password"] ?? "guest",
            AutomaticRecoveryEnabled = true
        };

        var queueName =
            _configuration["RabbitMQ:QueueName"] ?? "order-created";

        await using var connection =
            await factory.CreateConnectionAsync();

        await using var channel =
            await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent
        };

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queueName,
            mandatory: false,
            basicProperties: properties,
            body: body);
    }
        
    }
}
