using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace IoTDevices.IntegrationFunctions
{
    public class HandleDoorAccessCommandFunction
    {
        [FunctionName("HandleDoorAccessCommand")]
        public void Run([ServiceBusTrigger("doorsaccesscommands", Connection = "DoorsAccessServiceBusListenConnectionString")]string command, ILogger log)
        {
            // TODO: add IoT devices integration implementation

            log.LogInformation($"C# ServiceBus queue trigger function processed message: {command}");
        }
    }
}
