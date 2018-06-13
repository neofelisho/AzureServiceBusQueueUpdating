using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.ServiceBus;

namespace AzureServiceBusQueueReceiver
{
    internal class Program
    {
        private static void Main()
        {
            var config = new JobHostConfiguration();

            if (config.IsDevelopment) config.UseDevelopmentSettings();
            var serviceBusConfig = new ServiceBusConfiguration
            {
                ConnectionString =
                    AmbientConnectionStringProvider.Instance.GetConnectionString(ConnectionStringNames.ServiceBus)
            };
            config.UseServiceBus(serviceBusConfig);

            var host = new JobHost(config);
            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }
    }
}