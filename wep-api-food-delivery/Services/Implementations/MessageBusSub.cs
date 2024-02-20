
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using wep_api_food_delivery.Dtos;

namespace wep_api_food_delivery.Services.Implementations
{
    public class MessageBusSub : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private string _queueName;
        private readonly EventProcessor _eventProcessor;
        private readonly ILogger<MessageBusSub> _logger;

        public MessageBusSub(IConfiguration configuration, EventProcessor eventProcessor, ILogger<MessageBusSub> logger)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;
            _logger = logger;
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                _queueName = _channel.QueueDeclare().QueueName;
                _channel.QueueBind(queue: _queueName,
                    exchange: "trigger",
                    routingKey: "");

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка подключения к RabbitMQ - {DateTime.UtcNow} -> {ex.Message}");
            }
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (ModuleHandle, ea) =>
            {
                var body = ea.Body;
                try
                {
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    var eventData = JsonSerializer.Deserialize<OrderMessage>(message);
                    _logger.LogInformation($"Сообщение из очереди успешно получено {DateTime.UtcNow}");
                    await _eventProcessor.HandleEvent(eventData);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message,$"Ошибка при обработке сообщения из очереди {DateTime.UtcNow}");
                }
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogInformation($"RabbitMQ Showdown - {DateTime.UtcNow}");
        }
        public override void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }

            base.Dispose();
        }
    }
}
