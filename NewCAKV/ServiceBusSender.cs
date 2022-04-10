using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Microsoft.Azure.ServiceBus;

namespace NewCAKV
{
   
    public class ServiceBusSender
    {
        string QueueAccessKey = "Endpoint=sb://firstsbqueue.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=M1oHGjF0+n+tjHixmmESeKnhd06uKfe8LVMLY41OmHA=;EntityPath=firstnewqueue";
        public void SendMessage(string messgae)
        {
            ServiceBusConnectionStringBuilder conStr;
            QueueClient client;
            try
            {
                conStr = new ServiceBusConnectionStringBuilder(QueueAccessKey);
                client = new QueueClient(conStr);
                Message msg = new Message();
                msg.Body = Encoding.UTF8.GetBytes(messgae);
                Console.WriteLine("Please wait....message sending operation in progress.");
                client.SendAsync(msg).Wait();
            }
            catch (Exception exe)
            {
                Console.WriteLine("{0}", exe.Message);
                Console.WriteLine("Please restart application ");
            }
        }
    }
}
