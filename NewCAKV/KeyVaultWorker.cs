using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewCAKV
{
    internal class KeyVaultWorker
    {
        public static async Task<string> FetchSecret(string CLIENT_ID, string CLIENT_SECRET, string BASE_URI)
        {
            var client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(
                 async (string authority, string resource, string scope) =>
                 {
                     var context = new AuthenticationContext(authority);
                     ClientCredential clientCredential = new ClientCredential(CLIENT_ID, CLIENT_SECRET);
                     AuthenticationResult result = await context.AcquireTokenAsync(resource, clientCredential);
                     if (result == null)
                     {
                         throw new InvalidOperationException("Failed to retrieve token");
                     }
                     return result.AccessToken;
                 }
                 ));
            var secretValue = await client.GetSecretAsync(BASE_URI, "FirstSecret");
            return secretValue.Value;
        }
        public static async Task<bool> WriteSecret(string CLIENT_ID, string CLIENT_SECRET, string BASE_URI, string secret, string secretValue)
        {

            var keyclient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(
                 async (string authority, string resource, string scope) =>
                 {
                     var context = new AuthenticationContext(authority);
                     ClientCredential clientCredential = new ClientCredential(CLIENT_ID, CLIENT_SECRET);
                     AuthenticationResult result = await context.AcquireTokenAsync(resource, clientCredential);
                     if (result == null)
                     {
                         throw new InvalidOperationException("Failed to retrieve token");
                     }
                     return result.AccessToken;
                 }
                 ));
            var result = keyclient.SetSecretAsync(BASE_URI, secret, secretValue).Result;

            if (result != null)
                return true;
            else
                return false;
            // Print indented JSON response
            /*string prettyResult = JsonConvert.SerializeObject(result, Formatting.Indented);
            Console.WriteLine($"SetSecretAsync completed: {prettyResult}\n");

            // Read back secret
            string secretUrl = $"{vaultBaseUrl}/secrets/{secret}";
            var secretWeJustWroteTo = keyclient.GetSecretAsync(secretUrl).Result;
            Console.WriteLine($"secret: {secretWeJustWroteTo.Id} = {secretWeJustWroteTo.Value}");*/
        }
    }
}
