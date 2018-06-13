using System;
using log4net;
using log4net.Config;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;

namespace AzureServiceBusQueueReceiver
{
    public class Functions
    {
        private static readonly Lazy<ILog> LazyLog = new Lazy<ILog>(() =>
        {
            var log = LogManager.GetLogger(typeof(Functions));
            XmlConfigurator.Configure();
            return log;
        });

        private static readonly ILog Logger = LazyLog.Value;

        public static void ProcessQueueMessage([ServiceBusTrigger("testing")] BrokeredMessage message)
        {
            var messageBody = message.GetBody<string>();
            var messageId = message.MessageId;
            Logger.Error($"{messageId}:{messageBody}");
        }
    }
}