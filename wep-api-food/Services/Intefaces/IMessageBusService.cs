using wep_api_food.Models;

namespace wep_api_food.Services.Intefaces
{
    public interface IMessageBusService<T> where T : BaseMessage
    {
        void SendMessage(T message);
    }
}
