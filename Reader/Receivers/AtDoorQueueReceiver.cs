using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Reader
{
    public class AtDoorQueueReceiver
    {
        private readonly IMessageReceiver receiver;

        public AtDoorQueueReceiver()
        {
            receiver = new MessageReceiver(new ServiceBusConnection("Endpoint=sb://mentorshipspace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=2o/+2f18Gq4XBXTtYv4ZAZyt2ldPwqoBqS3u69xZeEk="), "at_door");
        }

        public void Subscribe<T>(Func<Message, CancellationToken, Task> messageHandler)
        {
            receiver.RegisterMessageHandler(
                async (message, token) => await messageHandler(message, token).ConfigureAwait(false),
                new MessageHandlerOptions(e => throw new Exception(e.Exception.Message, e.Exception))
                {
                    AutoComplete = false
                });
        }

        public Task CompleteAsync(string lockToken)
        {
            return receiver.CompleteAsync(lockToken);
        }
    }
}
