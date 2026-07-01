using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SalesDashboard.Application.Messaging;

namespace SalesDashboard.Infrastructure.Messaging;

public class OrderCreatedConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(IConfiguration configuration, ILogger<OrderCreatedConsumer> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"] ?? "localhost",
            Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
            Password = _configuration["RabbitMQ:Password"] ?? "guest",
            AutomaticRecoveryEnabled = true
        };

        var queueName = _configuration["RabbitMQ:QueueName"] ?? "order-created";

        var connection = await factory.CreateConnectionAsync(stoppingToken);
        var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            try
            {
                var body = eventArgs.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<OrderCreatedMessage>(json);

                _logger.LogInformation(
                    "RabbitMQ Consumer received order. OrderId: {OrderId}, CustomerId: {CustomerId}, TotalAmount: {TotalAmount}",
                    message?.OrderId,
                    message?.CustomerId,
                    message?.TotalAmount);

                await channel.BasicAckAsync(
                    deliveryTag: eventArgs.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process RabbitMQ message.");
            }
        };

        await channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

       
    }
}
