using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using DoorsAccess.Messaging.Messages;
using Microsoft.Extensions.Options;

namespace DoorsAccess.Messaging
{
    public class DoorAccessServiceBusSender : IDoorAccessMessageSender, IAsyncDisposable
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly DoorAccessServiceBusSenderOptions _options;

        public DoorAccessServiceBusSender(ServiceBusClient client, IOptions<DoorAccessServiceBusSenderOptions> options)
        {
            _serviceBusClient = client ?? throw new ArgumentNullException(nameof(client));
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task SendAsync(DoorAccessEvent message)
        {
            var serviceBusSender = _serviceBusClient.CreateSender(_options.DoorsAccessEventsTopic);

            await serviceBusSender.SendJsonMessageAsync(message);
        }

        public async Task SendAsync(DoorAccessCommand message)
        {
            var serviceBusSender = _serviceBusClient.CreateSender(_options.DoorsAccessCommandsQueue);

            await serviceBusSender.SendJsonMessageAsync(message);
        }

        public async ValueTask DisposeAsync()
        {
            await _serviceBusClient.DisposeAsync();
        }
    }
}