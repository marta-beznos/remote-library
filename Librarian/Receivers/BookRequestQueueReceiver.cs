using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Librarian
{
    public class BookRequestQueueReceiver
    {
        private readonly IMessageReceiver receiver;

        public BookRequestQueueReceiver()
        {
            receiver = new MessageReceiver("Endpoint=sb://mentorshipspace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=2o/+2f18Gq4XBXTtYv4ZAZyt2ldPwqoBqS3u69xZeEk=", "book_request");
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
