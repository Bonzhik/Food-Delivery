using RabbitMQ.Client;
using System.Text.Json.Serialization;
using System.Text.Json;
using wep_api_food.Models;
using wep_api_food.Services.Intefaces;
using System.Text;

namespace wep_api_food.Services.Implementations
{
    public class MessageBusService<T> : IMessageBusService<T> where T : class
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<MessageBusService<T>> _logger;

        public MessageBusService(IConfiguration configuration, ILogger<MessageBusService<T>> logger)
        {
            _configuration = configuration;
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

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка подключения к RabbitMQ - {DateTime.UtcNow}");
            }
        }
        public void SendMessage(T entity)
        {
            try
            {
                var message = JsonSerializer.Serialize(entity);
                _logger.LogInformation($"Отправка в очередь объекта {typeof(T).Name} - {DateTime.UtcNow}");
                if (_connection.IsOpen)
                {
                    var body = Encoding.UTF8.GetBytes(message);

                    _channel.BasicPublish(
                        exchange: "trigger",
                        routingKey: "",
                        basicProperties: null,
                        body: body
                        );
                }
            } catch (Exception ex)
            {
                _logger.LogError($"При отправке сообщения произошла ошибка {ex.Message}");
            }
            
        }
        public void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogInformation("RabbitMQ Showdown");
        }
    }
}
