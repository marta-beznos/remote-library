using Courier.Messages;
using Courier.Receivers;
using Courier.Senders;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Courier
{
    class Program
    {
        private static AtDoorQueueSender sender = new AtDoorQueueSender();

        static void Main(string[] args)
        {
            Console.WriteLine("Привiт Кур'єр! Скоро будуть замовлення на доставку...");

            var connectionString = "Endpoint=sb://mentorshipspace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=2o/+2f18Gq4XBXTtYv4ZAZyt2ldPwqoBqS3u69xZeEk=";
            var topic = "delivery_order";
            SubscriptionReceiver centerDataReceiver = new SubscriptionReceiver(connectionString, topic, "center");
            centerDataReceiver.Subscribe<DeliveryOrder>(CenterSubscriptionMessageHandler<DeliveryOrder>);
            SubscriptionReceiver naukovaDataReceiver = new SubscriptionReceiver(connectionString, topic, "naukova");
            naukovaDataReceiver.Subscribe<DeliveryOrder>(NaukovaSubscriptionMessageHandler<DeliveryOrder>);
            SubscriptionReceiver syhivDataReceiver = new SubscriptionReceiver(connectionString, topic, "syhiv");
            syhivDataReceiver.Subscribe<DeliveryOrder>(SyhivSubscriptionMessageHandler<DeliveryOrder>);

            Console.ReadLine();
        }

        private static async Task SubscriptionMessageHandler<T>(Message message, CancellationToken token, string direction, double price) where T : DeliveryOrder
        {
            var data = Encoding.UTF8.GetString(message.Body);
            var order = JsonConvert.DeserializeObject<T>(data);

            Console.WriteLine($"Поїхав {direction}");

            var atDoor = new AtDoor()
            {
                Book = order.Book,
                DeliveryPrice = price
            };
            await sender.SendMessageToQueueAsync(atDoor);

            Console.WriteLine($"На мiсцi");
        }

        private static async Task CenterSubscriptionMessageHandler<T>(Message message, CancellationToken token) where T : DeliveryOrder
        {
            await SubscriptionMessageHandler<DeliveryOrder>(message, token, "в центр", 20);
        }

        private static async Task NaukovaSubscriptionMessageHandler<T>(Message message, CancellationToken token) where T : DeliveryOrder
        {
            await SubscriptionMessageHandler<DeliveryOrder>(message, token, "на Наукову", 30);
        }

        private static async Task SyhivSubscriptionMessageHandler<T>(Message message, CancellationToken token) where T : DeliveryOrder
        {
            await SubscriptionMessageHandler<DeliveryOrder>(message, token, "на Сихiв", 50);
        }
    }
}
