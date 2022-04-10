using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewCAKV
{
    public class ServiceBusUtility
    {
        public void PutMessageInQueue()
        {
            ServiceBusSender sender = new ServiceBusSender();
            do
            {
                Console.WriteLine("Please type message to send in azure and press enter: ");
                var strMessage = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(strMessage))
                {
                    sender.SendMessage(strMessage);
                    Console.WriteLine("Message send successfully");
                }
                else
                {
                    Console.WriteLine("Empty Message. Please enter some text");
                }
            }
            while (Console.ReadKey(true).Key != ConsoleKey.End);
        }
        public void GetMessageFromQueue()
        {
            ServiceBusReceiver receiver = new ServiceBusReceiver();
            receiver.Listener();
        }
    }
}
