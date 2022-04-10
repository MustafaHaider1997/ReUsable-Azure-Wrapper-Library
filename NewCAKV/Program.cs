using Microsoft.Azure.Cosmos;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using NewCAKV;
using Newtonsoft.Json;

public class Newla
{

    async static Task Main(string[] args)
    {

       /* ServiceBusSender sender = new ServiceBusSender();
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
        while (Console.ReadKey(true).Key != ConsoleKey.End);*/

        ServiceBusReceiver receiver = new ServiceBusReceiver();
        receiver.Listener();

        RedisHandler.SetInCache("name", "value");


        Console.ReadKey();

    }

}