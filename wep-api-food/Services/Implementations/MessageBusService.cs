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

        public MessageBusService(IConfiguration configuration)
        {
            _configuration = configuration;
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
            }
        }
        public void SendMessage(T entity)
        {
            try
            {
                var message = JsonSerializer.Serialize(entity);
                Console.WriteLine($"Serialized message -----> {message}");

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
                Console.WriteLine("MessageSended");
            } catch (Exception ex)
            {
                Console.WriteLine($"While sending message was ----> {ex.Message} ");
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
            Console.WriteLine("RabbitMQ Showdown");
        }
    }
}
