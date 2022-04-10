using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace NewCAKV
{
    public static class TableWorker
    {
        public static CloudTable AuthAndCreateTable(string TableName, string connectionString)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
              //  DefaultEndpointsProtocol = https; AccountName = firststorageacc1; AccountKey = HDpwTAPa9PEUFRDBl7lTGn9Fhh5oaGI7WaSmCpgwjUS0kTDa8V12b + IPVMvfhiyr89CYJ7sfpTA8 + AStOcPhTw ==; EndpointSuffix = core.windows.net

                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                CloudTable table = tableClient.GetTableReference(TableName);
                table.CreateIfNotExistsAsync();

                return table;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static CloudTable ConnectToExistingTable(string TableName, string connectionString)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                CloudTable table = tableClient.GetTableReference(TableName);

                return table;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static bool CreateRecord(CloudTable table, dynamic user)
        {
            TableOperation insert = TableOperation.InsertOrMerge(user);
            try
            {
                table.ExecuteAsync(insert);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
          
        }
        public static async Task<TableResult> RetrieveRecord(CloudTable table, string partitionKey, string rowKey)
        {
            TableOperation tableOperation = TableOperation.Retrieve(partitionKey, rowKey);
            Task<TableResult> tableResult= table.ExecuteAsync(tableOperation);
            if (tableResult == null)
            {
                return null;
            }
            var retrievedResult=tableResult.Result;
            return retrievedResult;

        }

        public static void UpdateRecord(CloudTable table, dynamic userAfterModification)
        {
            if (userAfterModification != null)
            {
                TableOperation update = TableOperation.Replace(userAfterModification);
                table.ExecuteAsync(update);
                Console.WriteLine("Record Updated");
            }
            else
            {
                Console.WriteLine("Record does not exists");
            }
        }

        public static void DeleteCustomer(CloudTable table, dynamic user)
        {
            try
            {
                TableOperation delete = TableOperation.Delete(user);
                table.ExecuteAsync(delete);
                Console.WriteLine("Record deleted");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static async void DropTable(CloudTable table)
        {
            var status = await table.DeleteIfExistsAsync();
            if (status == true) 
                Console.WriteLine("Table deleted");
            else 
                Console.WriteLine("Table does not exists");
        }

    }
}
