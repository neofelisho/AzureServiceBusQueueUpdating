using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;

namespace AzureServiceBusEnqueuer
{
    internal class Program
    {
        public const string MessageBody = "test";
        public static string MessageId = Guid.NewGuid().ToString();

        private static async Task Main()
        {
            var message = new BrokeredMessage(MessageBody)
            {
                MessageId = MessageId
            };

            var queueClient = QueueClient.CreateFromConnectionString(
                AmbientConnectionStringProvider.Instance.GetConnectionString(ConnectionStringNames.ServiceBus),
                "testing");
            var sequenceNumber = await queueClient.ScheduleMessageAsync(message, new DateTimeOffset(DateTime.Now.AddMinutes(1)));
            Thread.Sleep(TimeSpan.FromSeconds(5));

            await queueClient.CancelScheduledMessageAsync(sequenceNumber); // without this, there will be duplicate messages.
            var message2 = new BrokeredMessage(MessageBody)
            {
                MessageId = MessageId
            };

            await queueClient.ScheduleMessageAsync(message2, new DateTimeOffset(DateTime.Now.AddMinutes(1)));
        }
    }
}