
using System;
using Microsoft.Azure.ApplicationInsights.Query;
using Microsoft.Rest.Azure.Authentication;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace NewCAKV
{
    public static class ApplicationInsightsFetcherAndLogger
    {
        private static readonly TelemetryClient _client;

        static ApplicationInsightsFetcherAndLogger()
        {
            TelemetryConfiguration config = TelemetryConfiguration.CreateDefault();
            config.InstrumentationKey = "99893195-8e0e-4536-91f5-d7b1de0a1907";
            _client = new TelemetryClient(config);
        }

        public static TelemetryClient GetClient()
        {
            return _client;
        }
    }
    public class DemoMainClass
    {
        private readonly TelemetryClient _client;
        public DemoMainClass()
        {
            _client = ApplicationInsightsFetcherAndLogger.GetClient();
        }

        public void MethodDoingAThang()
        {
            _client.TrackTrace("Here's a message!");

            try
            {
                // Doing something possbly sketchy
            }
            catch (Exception ex)
            {
                _client.TrackException(ex);
            }
            finally
            {
                _client.Flush();
            }
        }

        public async Task GetLogs()
        {
            // You can generate key in Azure Portal, in Application Insights' "API Access" pane
            string applicationId = "99088f09-16e1-4088-a643-5bdf7171bc9f";
            string key = "bq6lcgbw087ahwh1ujvd4zbxwsb7bqzadjkva65x";

            // Create client
            var credentials = new ApiKeyClientCredentials(key);
            var applicationInsightsClient = new ApplicationInsightsDataClient(credentials);

            // Query Application Insights
            var query = $"search 'message'";
            var response = await applicationInsightsClient.Query.ExecuteWithHttpMessagesAsync(applicationId, query);

            // Parse result 
            if (response.Response.IsSuccessStatusCode)
            {
                foreach (var row in response.Body.Results)
                {
                    Console.WriteLine($"[{row["timestamp"]:s}][{row["message"]}] Failed request to: {row["client_City"]}");
                }
            }
        }
    }
}