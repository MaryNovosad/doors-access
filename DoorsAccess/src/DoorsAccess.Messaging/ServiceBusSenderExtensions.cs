using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace DoorsAccess.Messaging
{
    public static class ServiceBusSenderExtensions
    {
        public static async Task SendJsonMessageAsync<TMessage>(this ServiceBusSender sender, TMessage message)
        {
            var busMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)))
            {
                ContentType = "application/json"
            };

            await sender.SendMessageAsync(busMessage);
        }
    }
}