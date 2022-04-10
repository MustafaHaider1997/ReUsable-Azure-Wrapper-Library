using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Azure.Storage.Blobs.Models;

namespace NewCAKV
{
    public static class BlobWorker
    {
        /*  public static async void CreateBlobStorageContainer(string containerName, string blobName, string connectionString)
          {
              //string connectionString = "DefaultEndpointsProtocol=https;AccountName=firststorageacc1;AccountKey=HDpwTAPa9PEUFRDBl7lTGn9Fhh5oaGI7WaSmCpgwjUS0kTDa8V12b+IPVMvfhiyr89CYJ7sfpTA8+AStOcPhTw==;EndpointSuffix=core.windows.net";
              try
              {
                   CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
                   CloudBlobClient blobClient = account.CreateCloudBlobClient();
                   CloudBlobContainer blobContainer= blobClient.GetContainerReference(containerName);

                   blobContainer.CreateIfNotExistsAsync(PublicAccessType.Blob);
                 // container.CreateIfNotExists(PublicAccessType.Blob);
                  // CloudBlockBlob blockBlob= blobContainer.GetBlockBlobReference(blobName);

                  //  return blockBlob;

                  //CloudStorageAccount storageaccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["ConnectionString"].ToString());
                  //Get the reference of the Storage Blob  
                  // CloudBlobClient client = storageaccount.CreateCloudBlobClient();
                  //Get the reference of the Container. The GetConainerReference doesn't make a request to the Blob Storage but the Create() &CreateIfNotExists() method does. The method CreateIfNotExists() could be use whether the Container exists or not  
                  // CloudBlobContainer container = client.GetContainerReference("images");
                  // container.CreateIfNotExists();

                  // var result = containerClient.Result;
                  // return result;
              }
              catch (Exception ex)
              {
                  Console.WriteLine(ex);
                 // return null;
              }
          }*/

        public static async Task<bool> CreateContainerAsync(string containerName, bool isPublic, string connectionString)
        {
            // Create the container if it doesn't exist.
            CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = account.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(containerName);
            if (isPublic)
            {
                var returnData = await blobContainer.CreateIfNotExistsAsync();
                if (returnData)
                    await blobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                return returnData;
            }
            return await blobContainer.CreateIfNotExistsAsync();
        }
        public static async void UploadBlob(string localPath, string fileName, string containerName, string connectionString, string inputFileString)
        {


            var serviceClient = new BlobServiceClient(connectionString);
            var containerClient = serviceClient.GetBlobContainerClient(containerName);
            // var path = @"d:\";
            // var fileName = "DemoFile.txt";
            var localFile = Path.Combine(localPath, fileName);
            await File.WriteAllTextAsync(localFile, inputFileString);
            var blobClient = containerClient.GetBlobClient(fileName);
            Console.WriteLine("Uploading to Blob storage");

            using FileStream uploadFileStream = File.OpenRead(localFile);
            await blobClient.UploadAsync(uploadFileStream, true);
            uploadFileStream.Close();
        }
        public static async void DownloadBlob(string localPath, string fileName, string containerName, string connectionString, string inputFileString, string DownloadFileName)
        {
            var serviceClient = new BlobServiceClient(connectionString);
            var containerClient = serviceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            var localFile = Path.Combine(localPath, fileName);
            string downloadFilePath = localFile.Replace(".txt", "DownloadFileName.txt");
            Console.WriteLine($"Downloading blob to\n\t{ downloadFilePath }\n");

            BlobDownloadInfo download = await blobClient.DownloadAsync();
            using FileStream downloadFileStream = File.OpenWrite(downloadFilePath);
            await download.Content.CopyToAsync(downloadFileStream);
            downloadFileStream.Close();
        }
        public static async void DeleteContainer(string localPath, string fileName, string containerName, string connectionString, string inputFileString, string DownloadFileName)
        {
            BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);
            await containerClient.DeleteAsync();
        }
        public static async void DeleteBlob(string blobName, string containerName, string connectionString)
        {
            BlobContainerClient container = new
            BlobContainerClient(connectionString, containerName);


            container.DeleteBlob(blobName);

        }
    }
}
