using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Cosmos;

namespace NewCAKV
{
    public class CosmosDBWorker
    {
        
        public async Task<CosmosClient> CreateCosmosClientAsync(string EndpointUri, string PrimaryKey)
        {
            // Create a new instance of the Cosmos Client
            CosmosClient cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions()
            {
                ConnectionMode = ConnectionMode.Gateway
            });
            return cosmosClient;
        }
        private async Task<Database> CreateDatabaseAsync(string databaseId, CosmosClient cosmosClient)
        {
            // Create a new database
            Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            Console.WriteLine("Created Database: {0}\n", database.Id);
            return database;
            
        }
        private async Task<Container> CreateContainerAsync(Database database, string containerId, string partitionProperty)
        {
            // Create a new container
            Container container = await database.CreateContainerIfNotExistsAsync(containerId, partitionProperty/*"/LastName"*/);
            Console.WriteLine("Created Container: {0}\n", container.Id);
            return container;
        }


        private async Task<dynamic> AddItemsToContainerAsync(dynamic user, Container container, string partitionKey)
        { 

            try
            {
                ItemResponse<Family> userResponse = await container.ReadItemAsync(user.Id, new PartitionKey(partitionKey));
                Console.WriteLine("Item in database already exists\n");
                return null;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                ItemResponse<Family> userResponse = await container.CreateItemAsync(user, new PartitionKey(partitionKey));

               
                Console.WriteLine("Created item in database");
                return userResponse;
                
            }

           
        }


        private async Task QueryItemsAsync(string sqlQueryText, Container container)
        {
            //var sqlQueryText = "SELECT * FROM c WHERE c.LastName = 'Andersen'";

            Console.WriteLine("Running query: {0}\n", sqlQueryText);

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            using FeedIterator<Family> queryResultSetIterator = container.GetItemQueryIterator<Family>(queryDefinition);

            List<Family> families = new List<Family>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Family> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (Family family in currentResultSet)
                {
                    families.Add(family);
                    Console.WriteLine("\tRead {0}\n", family);
                }
            }
        }


        /// Replace an item in the container

        private async Task ReplaceFamilyItemAsync(Container container, string partitionKey)
        {
            ItemResponse<Family> wakefieldFamilyResponse = await container.ReadItemAsync<Family>("Wakefield.7", new PartitionKey(partitionKey));
            var itemBody = wakefieldFamilyResponse.Resource;

            // update registration status from false to true
            itemBody.IsRegistered = true;
            // update grade of child
            itemBody.Children[0].Grade = 6;

            // replace the item with the updated content
            wakefieldFamilyResponse = await container.ReplaceItemAsync<Family>(itemBody, itemBody.Id, new PartitionKey(itemBody.LastName));
            Console.WriteLine("Updated Family [{0},{1}].\n \tBody is now: {2}\n", itemBody.LastName, itemBody.Id, wakefieldFamilyResponse.Resource);
        }


        /// Delete an item in the container
         private async Task DeleteFamilyItemAsync(Container container, string partitionKeyValue)
        {
           // var partitionKeyValue = "Wakefield";
            var familyId = "Wakefield.7";

            // Delete an item. Note we must provide the partition key value and id of the item to delete
            ItemResponse<Family> wakefieldFamilyResponse = await container.DeleteItemAsync<Family>(familyId, new PartitionKey(partitionKeyValue));
            Console.WriteLine("Deleted Family [{0},{1}]\n", partitionKeyValue, familyId);
        }


        /// Delete the database and dispose of the Cosmos Client instance

        private async Task DeleteDatabaseAndCleanupAsync(Database database, CosmosClient cosmosClient)
        {
            DatabaseResponse databaseResourceResponse = await database.DeleteAsync();
            // Also valid: await this.cosmosClient.Databases["FamilyDatabase"].DeleteAsync();

            Console.WriteLine("Deleted Database");

            //Dispose of CosmosClient
            cosmosClient.Dispose();
        }
    }
}
