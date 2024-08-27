using System.Text;
using Newtonsoft.Json;

// Custom certificate validation callback
HttpClientHandler handler = new HttpClientHandler();
handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

var client = new HttpClient(handler);
var directoryPath = @"/home/dorio/Downloads/Pedidos Bella Pizza"; // C:\NCR Solution\temp\backup
var url = "https://localhost:7143/Order";

if (!Directory.Exists(directoryPath))
{
    Console.WriteLine("Directory does not exist.");
    return;
}

string[] files = Directory.GetFiles(directoryPath, "*.wxt");

foreach (var file in files)
{
    Console.WriteLine($"Processing file: {file}");

    try
    {
        string[] lines = await File.ReadAllLinesAsync(file);

        string mergedLines = string.Join(Environment.NewLine, lines);

        var payload = new { data = mergedLines };
        var jsonContent = JsonConvert.SerializeObject(payload);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");


        try
        {
            HttpResponseMessage response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response received successfully.");
                Console.WriteLine(responseBody);
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception: {e.Message}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error reading file {file}: {ex.Message}");
    }
}

await Task.Delay(3000);
