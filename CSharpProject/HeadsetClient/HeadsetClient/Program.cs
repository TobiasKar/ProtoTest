using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Google.Protobuf; // Required for Protobuf serialization

class Program
{
    static async Task Main(string[] args)
    {
        // Step 1: Create and populate your StatusMessage object
        StatusMessage statusMessage = new StatusMessage
        {
            HeadsetID = "fsdf",
            BatteryLevel = 100,
            BatState = BatteryState.Full
        };

        // Step 2: Serialize the StatusMessage object to Protobuf (byte array)
        byte[] serializedData;
        using (MemoryStream memoryStream = new MemoryStream())
        {
            statusMessage.WriteTo(memoryStream); // Serialize to memory stream
            serializedData = memoryStream.ToArray(); // Convert stream to byte array
        }

        // Step 3: Send the serialized data via HTTP POST to a local server
        string url = "http://localhost:5000/api/status"; // Replace with your local endpoint
        using (HttpClient httpClient = new HttpClient())
        {
            // Create a ByteArrayContent to send the data
            using (var content = new ByteArrayContent(serializedData))
            {
                // Set the content type to "application/x-protobuf"
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-protobuf");

                Console.WriteLine("Sending POST request to server...");

                // Perform the POST request
                HttpResponseMessage response = await httpClient.PostAsync(url, content);

                // Step 4: Handle the response
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Data sent successfully!");
                    Console.WriteLine($"Response: {await response.Content.ReadAsStringAsync()}");
                }
                else
                {
                    Console.WriteLine($"Failed to send data. Status code: {response.StatusCode}");
                    Console.WriteLine($"Reason: {response.ReasonPhrase}");
                }
            }
        }
    }
}