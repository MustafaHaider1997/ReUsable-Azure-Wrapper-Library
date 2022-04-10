using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewCAKV
{
    public class ServiceBusReceiver
    {
        string QueueAccessKey = "Endpoint=sb://firstsbqueue.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=M1oHGjF0+n+tjHixmmESeKnhd06uKfe8LVMLY41OmHA=;EntityPath=firstnewqueue";
        public void Listener()
        {
            ServiceBusConnectionStringBuilder conStr;
            QueueClient client;
            try
            {
                conStr = new ServiceBusConnectionStringBuilder(QueueAccessKey);
                client = new QueueClient(conStr, ReceiveMode.ReceiveAndDelete, RetryPolicy.Default);
                var messageHandler = new MessageHandlerOptions(ListenerExceptionHandler)
                {
                    MaxConcurrentCalls = 1,
                    AutoComplete = false
                };
                client.RegisterMessageHandler(ReceiveMessageFromQ, messageHandler);
            }
            catch (Exception exe)
            {
                Console.WriteLine("{0}", exe.Message);
                Console.WriteLine("Please restart application ");
            }
        }
        public async Task ReceiveMessageFromQ(Message message, CancellationToken token)
        {
            string result = Encoding.UTF8.GetString(message.Body);
            Console.WriteLine("Message received from Queue = {0}", result);
            await Task.CompletedTask;
            Console.WriteLine("---- Task completed----");
            Console.WriteLine();
            Console.WriteLine("Please press END key to terminate.");
            Console.WriteLine("Please press Entry key to type and send next message.");
        }
        public Task ListenerExceptionHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine("{0}", exceptionReceivedEventArgs.Exception);
            return Task.CompletedTask;
        }
    }
}
