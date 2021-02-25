using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Reader.Messages;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Reader
{
    class Program
    {
        private static AtDoorQueueReceiver atDoorQueueReceiver = new AtDoorQueueReceiver();

        static async Task Main(string[] args)
        {
            var sender = new BookRequestQueueSender();
            atDoorQueueReceiver.Subscribe<AtDoor>(AtDoorMessageHandler<AtDoor>);

            Console.WriteLine("Привiт Читачу! Натайпай '+' - щоб замовити книжку, 'exit' - щоб вийти");
            while (true)
            {
                string text = Console.ReadLine();
                if (text == "exit")
                    break;
                else
                if (text == "+")
                {
                    var message = new BookRequest();
                    Console.WriteLine("Автор: ");
                    message.Author = Console.ReadLine().Trim();

                    Console.WriteLine("Назва: ");
                    message.Title = Console.ReadLine().Trim();

                    Console.WriteLine("Район (0 - Центр, 1 - Сихiв, 2 - Наукова): ");
                    message.Direction = int.Parse(Console.ReadLine().Trim());

                    Console.WriteLine("Вулиця: ");
                    message.Street = Console.ReadLine().Trim();

                    Console.WriteLine("Будинок: ");
                    message.Building = Console.ReadLine().Trim();

                    Console.WriteLine("Квартира: ");
                    message.Flat = Console.ReadLine().Trim();
                    
                    await sender.SendMessageToQueueAsync(message);
                }
            }
        }

        private static async Task AtDoorMessageHandler<T>(Message message, CancellationToken token) where T : AtDoor
        {
            var data = Encoding.UTF8.GetString(message.Body);
            var item = JsonConvert.DeserializeObject<T>(data);

            Console.WriteLine($"Доставка книг приїхала. З вас {item.DeliveryPrice} грн");

            await atDoorQueueReceiver.CompleteAsync(message.SystemProperties.LockToken);
        }
    }
}
