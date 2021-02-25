using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Courier.Receivers
{
    public class SubscriptionReceiver
    {
        private readonly IMessageReceiver receiver;

        public SubscriptionReceiver(string connectionString, string topicName, string subscriptionName)
        {
            this.receiver = new MessageReceiver(connectionString, EntityNameHelper.FormatSubscriptionPath(topicName, subscriptionName));
        }

        public void Subscribe<T>(Func<Message, CancellationToken, Task> messageHandler)
        {
            receiver.RegisterMessageHandler(
                async (message, token) => await messageHandler(message, token).ConfigureAwait(false),
                new MessageHandlerOptions(e => throw new Exception(e.Exception.Message, e.Exception))
                {
                    AutoComplete = true
                });
        }

        public Task CompleteAsync(string lockToken)
        {
            return receiver.CompleteAsync(lockToken);
        }
    }
}
