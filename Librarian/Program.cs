using Librarian.Messages;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Librarian
{
    class Program
    {
        static BookRequestQueueReceiver receiver = new BookRequestQueueReceiver();
        static DeliveryOrderTopicSender sender = new DeliveryOrderTopicSender();

        static void Main(string[] args)
        {
            Console.WriteLine("Привiт Бiблiотекарю! Зачекай, будь ласка, поки хтось замовить книгу...");

            receiver.Subscribe<BookRequest>(BookRequestMessageHandler<BookRequest>);

            Console.ReadLine();
        }

        private static async Task BookRequestMessageHandler<T>(Message message, CancellationToken token) where T : BookRequest
        {
            var data = Encoding.UTF8.GetString(message.Body);
            var item = JsonConvert.DeserializeObject<T>(data);

            var deliveryOrder = new DeliveryOrder
            {
                Book = $"{ item.Author } ''{ item.Title }''",
                Address = $"{ item.Street }, { item.Building}/{ item.Flat}"
            };
            Console.WriteLine($"Надiйшло замовлення на книгу {deliveryOrder.Book}. Доставка за адресою: {deliveryOrder.Address}");

            await sender.SendMessageToTopicAsync(deliveryOrder, item.Direction);

            Console.WriteLine("Замовлення передане на доставку");

            await receiver.CompleteAsync(message.SystemProperties.LockToken);
        }
    }
}
