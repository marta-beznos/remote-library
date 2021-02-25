using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Newtonsoft.Json;
using Reader.Messages;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Reader
{
    public class BookRequestQueueSender
    {
        private readonly IMessageSender sender;

        public BookRequestQueueSender()
        {
            this.sender = new MessageSender(new ServiceBusConnection("Endpoint=sb://mentorshipspace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=2o/+2f18Gq4XBXTtYv4ZAZyt2ldPwqoBqS3u69xZeEk="), "book_request");
        }

        public async Task SendMessageToQueueAsync(BookRequest messageBody)
        {
            var bytes = GetMessageBytes(messageBody);
            var message = new Message(bytes)
            {
                Label = "DemoLable", //????
                MessageId = Guid.NewGuid().ToString()
            };
            await sender.SendAsync(message);
        }

        public byte[] GetMessageBytes(object message)
        {
            if (message == null)
            {
                throw new NullReferenceException("Message body cannot be null");
            }

            var serializedObject = JsonConvert.SerializeObject(message);
            var bytes = Encoding.UTF8.GetBytes(serializedObject);
            return bytes;
        }

    }
}
